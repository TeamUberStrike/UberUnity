using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PickupItem : MonoBehaviour
{
	[SerializeField]
	protected int _respawnTime = 20;

	[SerializeField]
	private ParticleSystem _emitter;

	[SerializeField]
	protected Transform _pickupItem;

	protected MeshRenderer[] _renderers;

	private bool _isAvailable;

	private int _pickupID;

	private Collider _collider;

	private static int _instanceCounter = 0;

	private static Dictionary<int, PickupItem> _instances = new Dictionary<int, PickupItem>();

	private static List<ushort> _pickupRespawnDurations = new List<ushort>();

	public bool IsAvailable
	{
		get
		{
			return _isAvailable;
		}
		protected set
		{
			_isAvailable = value;
		}
	}

	protected virtual bool CanPlayerPickup
	{
		get
		{
			return true;
		}
	}

	public int PickupID
	{
		get
		{
			return _pickupID;
		}
		set
		{
			_pickupID = value;
		}
	}

	public int RespawnTime
	{
		get
		{
			return _respawnTime;
		}
	}

	protected virtual void Awake()
	{
		_collider = GetComponent<Collider>();
		if ((bool)_pickupItem)
		{
			_renderers = _pickupItem.GetComponentsInChildren<MeshRenderer>(true);
		}
		else
		{
			_renderers = new MeshRenderer[0];
		}
		_collider.isTrigger = true;
		if ((bool)_emitter)
		{
			var emission = _emitter.emission; emission.enabled = false;
		}
		base.gameObject.layer = 2;
	}

	private void OnEnable()
	{
		IsAvailable = true;
		_pickupID = AddInstance(this);
		MeshRenderer[] renderers = _renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = true;
		}
		EventHandler.Global.AddListener<GameEvents.PickupItemChanged>(OnRemotePickupEvent);
		EventHandler.Global.AddListener<GameEvents.PickupItemReset>(OnResetEvent);
	}

	private void OnDisable()
	{
		EventHandler.Global.RemoveListener<GameEvents.PickupItemChanged>(OnRemotePickupEvent);
		EventHandler.Global.RemoveListener<GameEvents.PickupItemReset>(OnResetEvent);
	}

	private void OnResetEvent(GameEvents.PickupItemReset ev)
	{
		StopAllCoroutines();
		SetItemAvailable(true);
	}

	private void OnRemotePickupEvent(GameEvents.PickupItemChanged ev)
	{
		if (PickupID == ev.Id)
		{
			if (!ev.Enable && IsAvailable)
			{
				OnRemotePickup();
			}
			SetItemAvailable(ev.Enable);
		}
	}

	protected virtual void OnRemotePickup()
	{
	}

	private void OnTriggerEnter(Collider c)
	{
		if (IsAvailable && c.tag == "Player" && GameState.Current.PlayerData.IsAlive && GameState.Current.IsMatchRunning && OnPlayerPickup())
		{
			SetItemAvailable(false);
		}
	}

	protected void PlayLocalPickupSound(AudioClip audioClip)
	{
		AutoMonoBehaviour<SfxManager>.Instance.PlayInGameAudioClip(audioClip);
	}

	protected void PlayRemotePickupSound(AudioClip audioClip, Vector3 position)
	{
		AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(audioClip, position, 1f);
	}

	protected IEnumerator StartHidingPickupForSeconds(int seconds)
	{
		IsAvailable = false;
		ParticleEffectController.ShowPickUpEffect(_pickupItem.position, 100);
		MeshRenderer[] renderers = _renderers;
		foreach (Renderer r in renderers)
		{
			if (r != null)
			{
				r.enabled = false;
			}
		}
		if (seconds > 0)
		{
			yield return new WaitForSeconds(seconds);
			ParticleEffectController.ShowPickUpEffect(_pickupItem.position, 5);
			yield return new WaitForSeconds(1f);
			MeshRenderer[] renderers2 = _renderers;
			foreach (Renderer r2 in renderers2)
			{
				r2.enabled = true;
			}
			IsAvailable = true;
		}
		else
		{
			base.enabled = false;
			yield return new WaitForSeconds(2f);
			Object.Destroy(base.gameObject);
		}
	}

	public void SetItemAvailable(bool isVisible)
	{
		if (isVisible)
		{
			ParticleEffectController.ShowPickUpEffect(_pickupItem.position, 5);
		}
		else if (IsAvailable)
		{
			ParticleEffectController.ShowPickUpEffect(_pickupItem.position, 100);
		}
		MeshRenderer[] renderers = _renderers;
		foreach (Renderer renderer in renderers)
		{
			if ((bool)renderer)
			{
				renderer.enabled = isVisible;
			}
		}
		IsAvailable = isVisible;
	}

	protected virtual bool OnPlayerPickup()
	{
		return true;
	}

	public static void Reset()
	{
		_instanceCounter = 0;
		_instances.Clear();
		_pickupRespawnDurations.Clear();
	}

	public static int GetInstanceCounter()
	{
		return _instanceCounter;
	}

	public static List<ushort> GetRespawnDurations()
	{
		return _pickupRespawnDurations;
	}

	private static int AddInstance(PickupItem i)
	{
		int num = _instanceCounter++;
		_instances[num] = i;
		_pickupRespawnDurations.Add((ushort)i.RespawnTime);
		return num;
	}

	public static PickupItem GetInstance(int id)
	{
		PickupItem value = null;
		_instances.TryGetValue(id, out value);
		return value;
	}
}

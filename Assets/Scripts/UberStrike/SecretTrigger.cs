using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SecretTrigger : BaseGameProp
{
	[SerializeField]
	private Renderer[] _visuals;

	[SerializeField]
	private float _activationTime = 15f;

	private SecretBehaviour _reciever;

	private float _showVisualsEndTime;

	public float ActivationTimeOut
	{
		get
		{
			return _showVisualsEndTime;
		}
	}

	private void Awake()
	{
		base.gameObject.layer = 21;
	}

	private void OnDisable()
	{
		Renderer[] visuals = _visuals;
		foreach (Renderer renderer in visuals)
		{
			renderer.material.SetColor("_Color", Color.black);
		}
	}

	private void Update()
	{
		if (_showVisualsEndTime > Time.time)
		{
			Renderer[] visuals = _visuals;
			foreach (Renderer renderer in visuals)
			{
				renderer.material.SetColor("_Color", new Color((Mathf.Sin(Time.time * 4f) + 1f) * 0.3f, 0f, 0f));
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	public override void ApplyDamage(DamageInfo shot)
	{
		if ((bool)_reciever)
		{
			base.enabled = true;
			_showVisualsEndTime = Time.time + _activationTime;
			_reciever.SetTriggerActivated(this);
		}
		else
		{
			Debug.LogError("The SecretTrigger " + base.gameObject.name + " is not assigned to a SecretReciever!");
		}
	}

	public void SetSecretReciever(SecretBehaviour logic)
	{
		_reciever = logic;
	}
}

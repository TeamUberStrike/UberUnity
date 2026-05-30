using System.Collections;
using UberStrike.Core.Models;
using UnityEngine;

public class PlayerDropCoinPickupItem : PickupItem
{
	public float Timeout = 10f;

	private float _timeout;

	private IEnumerator Start()
	{
		_timeout = Time.time + Timeout;
		Vector3 oldpos = base.transform.position;
		Vector3 newpos = oldpos;
		RaycastHit hit;
		if (Physics.Raycast(oldpos + Vector3.up, Vector3.down, out hit, 100f, UberstrikeLayerMasks.ProtectionMask) && oldpos.y > hit.point.y + 1f)
		{
			newpos = hit.point + Vector3.up;
		}
		_timeout = Time.time + Timeout;
		float time = 0f;
		while (_timeout > Time.time)
		{
			yield return new WaitForEndOfFrame();
			time += Time.deltaTime;
			base.transform.position = Vector3.Lerp(oldpos, newpos, time);
		}
		SetItemAvailable(false);
		base.enabled = false;
		yield return new WaitForSeconds(2f);
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if ((bool)_pickupItem)
		{
			_pickupItem.Rotate(Vector3.up, 150f * Time.deltaTime, Space.Self);
		}
	}

	protected override bool OnPlayerPickup()
	{
		GameState.Current.Actions.PickupPowerup(base.PickupID, PickupItemType.Coin, 1);
		GameData.Instance.OnItemPickup.Fire("Point", PickUpMessageType.Coin);
		PlayLocalPickupSound(GameAudio.GetPoints);
		StartCoroutine(StartHidingPickupForSeconds(0));
		return true;
	}

	protected override void OnRemotePickup()
	{
		PlayRemotePickupSound(GameAudio.GetPoints, base.transform.position);
	}
}

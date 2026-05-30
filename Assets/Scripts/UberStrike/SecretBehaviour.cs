using System;
using UnityEngine;

public class SecretBehaviour : MonoBehaviour
{
	[Serializable]
	public class Door
	{
		public string _description;

		[SerializeField]
		private SecretDoor _door;

		[SerializeField]
		private SecretTrigger[] _trigger;

		public SecretTrigger[] Trigger
		{
			get
			{
				return _trigger;
			}
		}

		public void CheckAllTriggers()
		{
			bool flag = true;
			SecretTrigger[] trigger = _trigger;
			foreach (SecretTrigger secretTrigger in trigger)
			{
				flag &= secretTrigger.ActivationTimeOut > Time.time;
			}
			if (flag)
			{
				_door.Open();
			}
		}
	}

	[SerializeField]
	private Door[] _doors;

	private void Awake()
	{
		Door[] doors = _doors;
		foreach (Door door in doors)
		{
			SecretTrigger[] trigger = door.Trigger;
			foreach (SecretTrigger secretTrigger in trigger)
			{
				secretTrigger.SetSecretReciever(this);
			}
		}
	}

	public void SetTriggerActivated(SecretTrigger trigger)
	{
		Door[] doors = _doors;
		foreach (Door door in doors)
		{
			door.CheckAllTriggers();
		}
	}
}

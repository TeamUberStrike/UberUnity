using UnityEngine;

public class DoorTrigger : BaseGameProp
{
	private DoorBehaviour _doorLogic;

	private void Awake()
	{
		base.gameObject.layer = 21;
	}

	public void SetDoorLogic(DoorBehaviour logic)
	{
		_doorLogic = logic;
	}

	public override void ApplyDamage(DamageInfo shot)
	{
		if ((bool)_doorLogic)
		{
			_doorLogic.Open();
		}
		else
		{
			Debug.LogError("The DoorCollider " + base.gameObject.name + " is not assigned to a DoorMechanism!");
		}
	}
}

using UberStrike.Core.Models;
using UnityEngine;

public class HealthPickupItem : PickupItem
{
	public enum Category
	{
		HP_100 = 0,
		HP_50 = 1,
		HP_25 = 2,
		HP_5 = 3
	}

	[SerializeField]
	private Category _healthPoints;

	protected override bool CanPlayerPickup
	{
		get
		{
			return (int)GameState.Current.PlayerData.Health < 100;
		}
	}

	protected override bool OnPlayerPickup()
	{
		int num = 0;
		int num2 = 100;
		switch (_healthPoints)
		{
		case Category.HP_5:
			num = 5;
			num2 = 200;
			break;
		case Category.HP_25:
			num = 25;
			num2 = 100;
			break;
		case Category.HP_50:
			num = 50;
			num2 = 100;
			break;
		case Category.HP_100:
			num = 100;
			num2 = 200;
			break;
		default:
			num = 0;
			num2 = 100;
			break;
		}
		PlayerData playerData = GameState.Current.PlayerData;
		if ((int)playerData.Health < num2)
		{
			playerData.Health.Value = Mathf.Clamp((int)playerData.Health + num, 0, num2);
			GameState.Current.Actions.PickupPowerup(base.PickupID, PickupItemType.Health, num);
			switch (_healthPoints)
			{
			case Category.HP_5:
				GameData.Instance.OnItemPickup.Fire("Mini Health", PickUpMessageType.Health5);
				break;
			case Category.HP_25:
				GameData.Instance.OnItemPickup.Fire("Medium Health", PickUpMessageType.Health25);
				break;
			case Category.HP_50:
				GameData.Instance.OnItemPickup.Fire("Big Health", PickUpMessageType.Health50);
				break;
			case Category.HP_100:
				GameData.Instance.OnItemPickup.Fire("Uber Health", PickUpMessageType.Health100);
				break;
			}
			switch (_healthPoints)
			{
			case Category.HP_100:
				PlayLocalPickupSound(GameAudio.MegaHealth2D);
				break;
			case Category.HP_50:
				PlayLocalPickupSound(GameAudio.BigHealth2D);
				break;
			case Category.HP_25:
				PlayLocalPickupSound(GameAudio.MediumHealth2D);
				break;
			case Category.HP_5:
				PlayLocalPickupSound(GameAudio.SmallHealth2D);
				break;
			default:
				PlayLocalPickupSound(GameAudio.SmallHealth2D);
				break;
			}
			if (GameState.Current.IsSinglePlayer)
			{
				StartCoroutine(StartHidingPickupForSeconds(_respawnTime));
			}
			return true;
		}
		return false;
	}

	protected override void OnRemotePickup()
	{
		switch (_healthPoints)
		{
		case Category.HP_100:
			PlayRemotePickupSound(GameAudio.MegaHealth, base.transform.position);
			break;
		case Category.HP_50:
			PlayRemotePickupSound(GameAudio.BigHealth, base.transform.position);
			break;
		case Category.HP_25:
			PlayRemotePickupSound(GameAudio.MediumHealth, base.transform.position);
			break;
		case Category.HP_5:
			PlayRemotePickupSound(GameAudio.SmallHealth, base.transform.position);
			break;
		default:
			PlayRemotePickupSound(GameAudio.SmallHealth, base.transform.position);
			break;
		}
	}
}

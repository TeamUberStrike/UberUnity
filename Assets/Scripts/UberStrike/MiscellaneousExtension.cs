using System.Collections.Generic;
using System.Text;
using UberStrike.Core.Models;
using UberStrike.Core.Models.Views;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public static class MiscellaneousExtension
{
	public static string ToCustomString(this GameActorInfo info)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Name: {0} - {1}/{2}\n", info.PlayerName, info.Cmid, info.PlayerId);
		stringBuilder.AppendLine("Play: " + CmunePrint.Flag(info.PlayerState));
		stringBuilder.AppendLine("CurrentWeapon: " + info.CurrentWeaponSlot + "/" + info.CurrentWeaponID);
		stringBuilder.AppendLine("Life: " + info.Health + "/" + info.ArmorPoints);
		stringBuilder.AppendLine("Team: " + info.TeamID);
		stringBuilder.AppendLine("Color: " + info.SkinColor);
		stringBuilder.AppendLine("Weapons: " + CmunePrint.Values(info.Weapons));
		stringBuilder.AppendLine("Gear: " + CmunePrint.Values(info.Gear));
		return stringBuilder.ToString();
	}

	public static Transform FindChildWithName(this Transform tr, string name)
	{
		Transform[] componentsInChildren = tr.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.name == name)
			{
				return transform;
			}
		}
		return null;
	}

	public static void ShareParent(this Transform _this, Transform transform)
	{
		Vector3 localPosition = transform.localPosition;
		Quaternion localRotation = transform.localRotation;
		Vector3 localScale = transform.localScale;
		_this.parent = transform.parent;
		_this.localPosition = localPosition;
		_this.localRotation = localRotation;
		_this.localScale = localScale;
	}

	public static void Reparent(this Transform _this, Transform newParent)
	{
		Vector3 localPosition = _this.localPosition;
		Quaternion localRotation = _this.localRotation;
		Vector3 localScale = _this.localScale;
		_this.parent = newParent;
		_this.localPosition = localPosition;
		_this.localRotation = localRotation;
		_this.localScale = localScale;
	}

	public static string ToCustomString(this GameActorInfoDelta info)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Delta ").Append(info.Id).Append(":");
		foreach (KeyValuePair<GameActorInfoDelta.Keys, object> change in info.Changes)
		{
			stringBuilder.Append(change.Key.ToString()).Append("|");
		}
		return stringBuilder.ToString();
	}

	public static MapSettings Default(this MapSettings info)
	{
		MapSettings mapSettings = new MapSettings();
		mapSettings.KillsCurrent = 5;
		mapSettings.KillsMax = 100;
		mapSettings.KillsMin = 1;
		mapSettings.PlayersCurrent = 0;
		mapSettings.PlayersMax = 16;
		mapSettings.PlayersMin = 1;
		mapSettings.TimeCurrent = 60;
		mapSettings.TimeMax = 600;
		mapSettings.TimeMin = 1;
		return mapSettings;
	}
}

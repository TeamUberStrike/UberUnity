using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UberStrike.Core.Types;
using UnityEngine;

public class UnityItemConfiguration : MonoBehaviour
{
	[Serializable]
	public class FunctionalItemHolder
	{
		public string Name;

		public Texture2D Icon;

		public int ItemId;
	}

	[SerializeField]
	private TextAsset m_ItemPrefabXml;

	public List<GearItem> UnityItemsDefaultGears;

	public List<WeaponItem> UnityItemsDefaultWeapons;

	public List<FunctionalItemHolder> UnityItemsFunctional;

	public List<Texture2D> DefaultWeaponIcons;

	private Dictionary<string, string> m_AvailablePrefabs;

	public static UnityItemConfiguration Instance { get; private set; }

	public static bool Exists
	{
		get
		{
			return Instance != null;
		}
	}

	private void Awake()
	{
		Instance = this;
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(m_ItemPrefabXml.text);
		XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/ItemAssetBundle/Item");
		m_AvailablePrefabs = new Dictionary<string, string>();
		foreach (XmlNode item in xmlNodeList)
		{
			string value = item.Attributes.GetNamedItem("Prefab").Value;
			value = value.Replace(".prefab", string.Empty).Replace("Assets/", string.Empty);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(value);
			m_AvailablePrefabs[fileNameWithoutExtension] = value;
		}
	}

	public GameObject GetDefaultItem(UberstrikeItemClass itemClass)
	{
		if (UnityItemsDefaultGears.Exists((GearItem i) => i.TestItemClass == itemClass))
		{
			return UnityItemsDefaultGears.Find((GearItem i) => i.TestItemClass == itemClass).gameObject;
		}
		if (UnityItemsDefaultWeapons.Exists((WeaponItem i) => i.TestItemClass == itemClass))
		{
			return UnityItemsDefaultWeapons.Find((WeaponItem i) => i.TestItemClass == itemClass).gameObject;
		}
		Debug.LogError("Couldn't find default item with class: " + itemClass);
		return null;
	}

	public string GetPrefabPath(string prefabName)
	{
		string value = string.Empty;
		if (m_AvailablePrefabs.TryGetValue(prefabName, out value))
		{
			return value;
		}
		return string.Empty;
	}

	public bool IsPrefabAvailable(string prefabName)
	{
		return m_AvailablePrefabs.ContainsKey(prefabName);
	}

	public bool Contains(string prefabName)
	{
		return (bool)UnityItemsDefaultGears.Find((GearItem item) => item.name.Equals(prefabName)) || (bool)UnityItemsDefaultWeapons.Find((WeaponItem item) => item.name.Equals(prefabName));
	}

	public Texture2D GetDefaultIcon(UberstrikeItemClass itemClass)
	{
		switch (itemClass)
		{
		case UberstrikeItemClass.WeaponCannon:
			return DefaultWeaponIcons.Find((Texture2D icon) => icon.name.Contains("Cannon"));
		case UberstrikeItemClass.WeaponLauncher:
			return DefaultWeaponIcons.Find((Texture2D icon) => icon.name.Contains("Launcher"));
		case UberstrikeItemClass.WeaponMachinegun:
			return DefaultWeaponIcons.Find((Texture2D icon) => icon.name.Contains("Machine"));
		case UberstrikeItemClass.WeaponMelee:
			return DefaultWeaponIcons.Find((Texture2D icon) => icon.name.Contains("Melee"));
		case UberstrikeItemClass.WeaponShotgun:
			return DefaultWeaponIcons.Find((Texture2D icon) => icon.name.Contains("Shot"));
		case UberstrikeItemClass.WeaponSniperRifle:
			return DefaultWeaponIcons.Find((Texture2D icon) => icon.name.Contains("Sniper"));
		case UberstrikeItemClass.WeaponSplattergun:
			return DefaultWeaponIcons.Find((Texture2D icon) => icon.name.Contains("Splatter"));
		default:
			return null;
		}
	}

	public Texture2D GetFunctionalItemIcon(int itemId)
	{
		FunctionalItemHolder functionalItemHolder = UnityItemsFunctional.Find((FunctionalItemHolder holder) => holder.ItemId == itemId);
		if (functionalItemHolder == null)
		{
			Debug.LogWarning("Failed to find icon for functional item with id: " + itemId);
			return null;
		}
		return functionalItemHolder.Icon;
	}
}

using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UnityEngine;

public class ProxyItem : IUnityItem
{
	private Texture2D m_Icon;

	public bool Equippable
	{
		get
		{
			return true;
		}
	}

	public string Name
	{
		get
		{
			return View.Name;
		}
	}

	public bool IsLoaded { get; private set; }

	public GameObject Prefab { get; private set; }

	public BaseUberStrikeItemView View { get; private set; }

	public int CriticalStrikeBonus
	{
		get
		{
			if (View.ItemProperties.ContainsKey(ItemPropertyType.CritDamageBonus))
			{
				return View.ItemProperties[ItemPropertyType.CritDamageBonus];
			}
			return 0;
		}
	}

	public ProxyItem(BaseUberStrikeItemView view)
	{
		View = view;
		if (UnityItemConfiguration.Instance.IsPrefabAvailable(View.PrefabName))
		{
			string prefabPath = UnityItemConfiguration.Instance.GetPrefabPath(view.PrefabName);
			prefabPath += "-Icon";
			m_Icon = Resources.Load<Texture2D>(prefabPath);
		}
		else if (View.ItemClass == UberstrikeItemClass.FunctionalGeneral)
		{
			m_Icon = UnityItemConfiguration.Instance.GetFunctionalItemIcon(View.ID);
		}
		else
		{
			m_Icon = UnityItemConfiguration.Instance.GetDefaultIcon(view.ItemClass);
		}
	}

	public void UpdateProxyItem(BaseUberStrikeItemView view)
	{
		View = view;
	}

	public void Unload()
	{
	}

	public GameObject Create(Vector3 position, Quaternion rotation)
	{
		if (UnityItemConfiguration.Instance.IsPrefabAvailable(View.PrefabName))
		{
			string prefabPath = UnityItemConfiguration.Instance.GetPrefabPath(View.PrefabName);
			Debug.Log("Create Item:" + View.ID + ", " + View.Name + ", " + prefabPath);
			Object obj = Resources.Load<GameObject>(prefabPath);
			Prefab = (GameObject)obj;
		}
		else
		{
			Debug.Log("Create DEFAULT Item:" + View.ID + ", " + View.Name + ", " + View.PrefabName);
			Prefab = UnityItemConfiguration.Instance.GetDefaultItem(View.ItemClass);
		}
		if (View.ItemType == UberstrikeItemType.QuickUse)
		{
			QuickItem component = Prefab.GetComponent<QuickItem>();
			if (component != null && (bool)component.Sfx)
			{
				Singleton<QuickItemSfxController>.Instance.RegisterQuickItemEffect(component.Logic, component.Sfx);
			}
		}
		GameObject gameObject = null;
		if (Prefab != null)
		{
			if (View.ItemClass == UberstrikeItemClass.GearHolo)
			{
				HoloGearItem component2 = Prefab.GetComponent<HoloGearItem>();
				if ((bool)component2 && (bool)component2.Configuration.Avatar)
				{
					gameObject = Object.Instantiate(component2.Configuration.Avatar.gameObject) as GameObject;
				}
			}
			else
			{
				gameObject = Object.Instantiate(Prefab, position, rotation) as GameObject;
			}
			if ((bool)gameObject && View.ItemType == UberstrikeItemType.Weapon)
			{
				WeaponItem component3 = gameObject.GetComponent<WeaponItem>();
				if ((bool)component3)
				{
					ItemConfigurationUtil.CopyCustomProperties(View, component3.Configuration);
					if (View.ItemProperties.ContainsKey(ItemPropertyType.CritDamageBonus))
					{
						component3.Configuration.CriticalStrikeBonus = View.ItemProperties[ItemPropertyType.CritDamageBonus];
					}
					else
					{
						component3.Configuration.CriticalStrikeBonus = 0;
					}
				}
			}
		}
		else
		{
			Debug.LogError("Trying to create item prefab, but it was null. Item Name:" + View.Name);
		}
		IsLoaded = true;
		return gameObject;
	}

	public void DrawIcon(Rect position)
	{
		GUI.DrawTexture(position, m_Icon);
	}
}

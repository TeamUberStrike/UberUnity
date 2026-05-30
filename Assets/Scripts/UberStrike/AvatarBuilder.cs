using UnityEngine;

public static class AvatarBuilder
{
	public static void Destroy(GameObject obj)
	{
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			Material[] materials = renderer.materials;
			foreach (Material obj2 in materials)
			{
				Object.Destroy(obj2);
			}
		}
		SkinnedMeshRenderer componentInChildren = obj.GetComponentInChildren<SkinnedMeshRenderer>();
		if ((bool)componentInChildren)
		{
			Object.Destroy(componentInChildren.sharedMesh);
		}
		Object.Destroy(obj);
	}

	public static AvatarDecorator CreateLocalAvatar()
	{
		AvatarDecorator avatarDecorator = CreateAvatarMesh(Singleton<LoadoutManager>.Instance.Loadout.GetAvatarGear());
		Object.DontDestroyOnLoad(avatarDecorator.gameObject);
		SetupLocalAvatar(avatarDecorator);
		return avatarDecorator;
	}

	public static void UpdateLocalAvatar(AvatarGearParts gear)
	{
		if (GameState.Current.Avatar.Decorator.name != gear.Base.name)
		{
			AvatarDecorator decorator = GameState.Current.Avatar.Decorator;
			AvatarDecorator avatarDecorator = CreateAvatarMesh(gear);
			Object.DontDestroyOnLoad(avatarDecorator.gameObject);
			avatarDecorator.transform.ShareParent(decorator.transform);
			avatarDecorator.gameObject.SetActive(decorator.gameObject.activeSelf);
			ReparentChildren(decorator.WeaponAttachPoint, avatarDecorator.WeaponAttachPoint);
			Object.Destroy(decorator.gameObject);
			GameState.Current.Avatar.SetDecorator(avatarDecorator);
			SetupLocalAvatar(GameState.Current.Avatar.Decorator);
		}
		else if ((bool)GameState.Current.Avatar.Decorator)
		{
			UpdateAvatarMesh(GameState.Current.Avatar.Decorator, gear);
			SetupLocalAvatar(GameState.Current.Avatar.Decorator);
		}
		else
		{
			Debug.LogError("No local Player created yet! Call 'CreateLocalPlayerAvatar' first!");
		}
	}

	private static void ReparentChildren(Transform oldParent, Transform newParent)
	{
		while (oldParent.childCount > 0)
		{
			Transform child = oldParent.GetChild(0);
			child.Reparent(newParent);
		}
	}

	public static AvatarDecorator CreateRemoteAvatar(AvatarGearParts gear, Color skinColor)
	{
		AvatarDecorator avatarDecorator = CreateAvatarMesh(gear);
		SetupRemoteAvatar(avatarDecorator, skinColor);
		return avatarDecorator;
	}

	public static AvatarDecorator UpdateRemoteAvatar(AvatarDecorator avatar, AvatarGearParts gear, Color skinColor)
	{
		if (avatar.name != gear.Base.name)
		{
			AvatarDecorator avatarDecorator = avatar;
			avatar = CreateAvatarMesh(gear);
			avatar.transform.ShareParent(avatarDecorator.transform);
			avatar.gameObject.SetActive(avatarDecorator.gameObject.activeSelf);
			ReparentChildren(avatarDecorator.WeaponAttachPoint, avatar.WeaponAttachPoint);
			Object.Destroy(avatarDecorator.gameObject);
			SetupRemoteAvatar(avatar, skinColor);
		}
		else
		{
			UpdateAvatarMesh(avatar, gear);
			SetupRemoteAvatar(avatar, skinColor);
		}
		return avatar;
	}

	public static AvatarDecoratorConfig InstantiateRagdoll(AvatarGearParts gear, Color skinColor)
	{
		SkinnedMeshCombiner.Combine(gear.Base, gear.Parts);
		LayerUtil.SetLayerRecursively(gear.Base.transform, UberstrikeLayer.Ragdoll);
		gear.Base.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
		AvatarDecoratorConfig component = gear.Base.GetComponent<AvatarDecoratorConfig>();
		component.SetSkinColor(skinColor);
		return component;
	}

	private static void UpdateAvatarMesh(AvatarDecorator avatar, AvatarGearParts gear)
	{
		if (!avatar)
		{
			Debug.LogError("AvatarDecorator is null!");
			return;
		}
		gear.Parts.Add(gear.Base);
		ParticleSystem[] componentsInChildren = avatar.GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			Object.Destroy(particleSystem.gameObject);
		}
		SkinnedMeshCombiner.Update(avatar.gameObject, gear.Parts);
		avatar.GetComponent<Renderer>().receiveShadows = false;
		gear.Parts.ForEach(delegate(GameObject obj)
		{
			Object.Destroy(obj);
		});
	}

	private static AvatarDecorator CreateAvatarMesh(AvatarGearParts gear)
	{
		SkinnedMeshCombiner.Combine(gear.Base, gear.Parts);
		gear.Parts.ForEach(delegate(GameObject obj)
		{
			Object.Destroy(obj);
		});
		return gear.Base.GetComponent<AvatarDecorator>();
	}

	private static void SetupLocalAvatar(AvatarDecorator avatar)
	{
		if ((bool)avatar)
		{
			avatar.SetLayers(UberstrikeLayer.RemotePlayer);
			avatar.Configuration.SetSkinColor(PlayerDataManager.SkinColor);
			avatar.HudInformation.SetAvatarLabel(PlayerDataManager.NameAndTag);
		}
		else
		{
			Debug.LogError("No AvatarDecorator to setup!");
		}
	}

	private static void SetupRemoteAvatar(AvatarDecorator avatar, Color skinColor)
	{
		if ((bool)avatar)
		{
			avatar.SetLayers(UberstrikeLayer.RemotePlayer);
			avatar.Configuration.SetSkinColor(skinColor);
		}
		else
		{
			Debug.LogError("No AvatarDecorator to setup!");
		}
	}
}

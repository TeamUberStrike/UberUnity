using System.Collections.Generic;
using UnityEngine;

public class RagdollBuilder : Singleton<RagdollBuilder>
{
	private RagdollBuilder()
	{
	}

	public AvatarDecoratorConfig CreateRagdoll(Loadout gearLoadout)
	{
		IUnityItem item;
		if (gearLoadout.TryGetItem(LoadoutSlotType.GearHolo, out item))
		{
			return CreateHolo(item);
		}
		return CreateLutzRavinoff(gearLoadout);
	}

	private AvatarDecoratorConfig CreateLutzRavinoff(Loadout gearLoadout)
	{
		AvatarDecoratorConfig defaultRagdoll = PrefabManager.Instance.DefaultRagdoll;
		AvatarDecoratorConfig avatarDecoratorConfig = Object.Instantiate(defaultRagdoll) as AvatarDecoratorConfig;
		List<GameObject> list = new List<GameObject>();
		SkinnedMeshCombiner.Combine(avatarDecoratorConfig.gameObject, list);
		foreach (GameObject item in list)
		{
			Object.Destroy(item);
		}
		return avatarDecoratorConfig;
	}

	private AvatarDecoratorConfig CreateHolo(IUnityItem holo)
	{
		AvatarDecoratorConfig avatarDecoratorConfig = null;
		GameObject gameObject = holo.Create(Vector3.zero, Quaternion.identity);
		HoloGearItem component = gameObject.GetComponent<HoloGearItem>();
		if ((bool)component && (bool)component.Configuration.Ragdoll)
		{
			avatarDecoratorConfig = Object.Instantiate(component.Configuration.Ragdoll) as AvatarDecoratorConfig;
			LayerUtil.SetLayerRecursively(avatarDecoratorConfig.transform, UberstrikeLayer.Ragdoll);
			SkinnedMeshCombiner.Combine(avatarDecoratorConfig.gameObject, new List<GameObject>());
		}
		Object.Destroy(gameObject);
		return avatarDecoratorConfig;
	}
}

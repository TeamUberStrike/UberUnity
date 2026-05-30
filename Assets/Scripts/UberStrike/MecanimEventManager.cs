using System.Collections.Generic;
using UnityEngine;

public static class MecanimEventManager
{
	private static MecanimEventData[] eventDataSources;

	private static Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>> loadedData;

	private static Dictionary<int, Dictionary<int, AnimatorStateInfo>> lastStates = new Dictionary<int, Dictionary<int, AnimatorStateInfo>>();

	public static void SetEventDataSource(MecanimEventData dataSource)
	{
		if (dataSource != null)
		{
			eventDataSources = new MecanimEventData[1];
			eventDataSources[0] = dataSource;
			LoadDataInGame();
		}
	}

	public static void SetEventDataSource(MecanimEventData[] dataSources)
	{
		if (dataSources != null)
		{
			eventDataSources = dataSources;
			LoadDataInGame();
		}
	}

	public static void OnLevelLoaded()
	{
		lastStates.Clear();
	}

	public static ICollection<MecanimEvent> GetEvents(int animatorControllerId, Animator animator)
	{
		List<MecanimEvent> list = new List<MecanimEvent>();
		int hashCode = animator.GetHashCode();
		if (!lastStates.ContainsKey(hashCode))
		{
			lastStates[hashCode] = new Dictionary<int, AnimatorStateInfo>();
		}
		int layerCount = animator.layerCount;
		Dictionary<int, AnimatorStateInfo> dictionary = lastStates[hashCode];
		for (int i = 0; i < layerCount; i++)
		{
			if (!dictionary.ContainsKey(i))
			{
				dictionary[i] = default(AnimatorStateInfo);
			}
			AnimatorStateInfo animatorStateInfo = dictionary[i];
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
			int num = (int)animatorStateInfo.normalizedTime;
			int num2 = (int)currentAnimatorStateInfo.normalizedTime;
			float normalizedTimeStart = animatorStateInfo.normalizedTime - (float)num;
			float normalizedTimeEnd = currentAnimatorStateInfo.normalizedTime - (float)num2;
			if (animatorStateInfo.nameHash == currentAnimatorStateInfo.nameHash)
			{
				if (currentAnimatorStateInfo.loop)
				{
					if (num == num2)
					{
						list.AddRange(CollectEvents(animator, animatorControllerId, i, currentAnimatorStateInfo.nameHash, currentAnimatorStateInfo.tagHash, normalizedTimeStart, normalizedTimeEnd));
					}
					else
					{
						list.AddRange(CollectEvents(animator, animatorControllerId, i, currentAnimatorStateInfo.nameHash, currentAnimatorStateInfo.tagHash, normalizedTimeStart, 1.00001f));
						list.AddRange(CollectEvents(animator, animatorControllerId, i, currentAnimatorStateInfo.nameHash, currentAnimatorStateInfo.tagHash, 0f, normalizedTimeEnd));
					}
				}
				else
				{
					float num3 = Mathf.Clamp01(animatorStateInfo.normalizedTime);
					float num4 = Mathf.Clamp01(currentAnimatorStateInfo.normalizedTime);
					if (num == 0 && num2 == 0)
					{
						if (num3 != num4)
						{
							list.AddRange(CollectEvents(animator, animatorControllerId, i, currentAnimatorStateInfo.nameHash, currentAnimatorStateInfo.tagHash, num3, num4));
						}
					}
					else if (num == 0 && num2 > 0)
					{
						list.AddRange(CollectEvents(animator, animatorControllerId, i, animatorStateInfo.nameHash, animatorStateInfo.tagHash, num3, 1.00001f));
					}
				}
			}
			else
			{
				list.AddRange(CollectEvents(animator, animatorControllerId, i, currentAnimatorStateInfo.nameHash, currentAnimatorStateInfo.tagHash, 0f, normalizedTimeEnd));
				if (!animatorStateInfo.loop)
				{
					list.AddRange(CollectEvents(animator, animatorControllerId, i, animatorStateInfo.nameHash, animatorStateInfo.tagHash, normalizedTimeStart, 1.00001f, true));
				}
			}
			dictionary[i] = currentAnimatorStateInfo;
		}
		return list;
	}

	private static ICollection<MecanimEvent> CollectEvents(Animator animator, int animatorControllerId, int layer, int nameHash, int tagHash, float normalizedTimeStart, float normalizedTimeEnd, bool onlyCritical = false)
	{
		if (loadedData.ContainsKey(animatorControllerId) && loadedData[animatorControllerId].ContainsKey(layer) && loadedData[animatorControllerId][layer].ContainsKey(nameHash))
		{
			List<MecanimEvent> list = loadedData[animatorControllerId][layer][nameHash];
			List<MecanimEvent> list2 = new List<MecanimEvent>();
			{
				foreach (MecanimEvent item in list)
				{
					if (item.normalizedTime >= normalizedTimeStart && item.normalizedTime < normalizedTimeEnd && item.condition.Test(animator) && (!onlyCritical || item.critical))
					{
						MecanimEvent mecanimEvent = new MecanimEvent(item);
						mecanimEvent.SetContext(new EventContext
						{
							controllerId = animatorControllerId,
							layer = layer,
							stateHash = nameHash,
							tagHash = tagHash
						});
						list2.Add(mecanimEvent);
					}
				}
				return list2;
			}
		}
		return new MecanimEvent[0];
	}

	private static void LoadDataInGame()
	{
		if (eventDataSources == null)
		{
			return;
		}
		loadedData = new Dictionary<int, Dictionary<int, Dictionary<int, List<MecanimEvent>>>>();
		MecanimEventData[] array = eventDataSources;
		foreach (MecanimEventData mecanimEventData in array)
		{
			if (mecanimEventData == null)
			{
				continue;
			}
			MecanimEventDataEntry[] data = mecanimEventData.data;
			MecanimEventDataEntry[] array2 = data;
			foreach (MecanimEventDataEntry mecanimEventDataEntry in array2)
			{
				int instanceID = mecanimEventDataEntry.animatorController.GetInstanceID();
				if (!loadedData.ContainsKey(instanceID))
				{
					loadedData[instanceID] = new Dictionary<int, Dictionary<int, List<MecanimEvent>>>();
				}
				if (!loadedData[instanceID].ContainsKey(mecanimEventDataEntry.layer))
				{
					loadedData[instanceID][mecanimEventDataEntry.layer] = new Dictionary<int, List<MecanimEvent>>();
				}
				loadedData[instanceID][mecanimEventDataEntry.layer][mecanimEventDataEntry.stateNameHash] = new List<MecanimEvent>(mecanimEventDataEntry.events);
			}
		}
	}
}

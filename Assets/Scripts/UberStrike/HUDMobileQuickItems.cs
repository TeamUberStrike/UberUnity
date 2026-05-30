using System.Collections.Generic;
using UnityEngine;

public class HUDMobileQuickItems : MonoBehaviour
{
	[SerializeField]
	private NGUIScrollList scrollList;

	[SerializeField]
	private HUDQuickItem slot1;

	[SerializeField]
	private HUDQuickItem slot2;

	[SerializeField]
	private HUDQuickItem slot3;

	[SerializeField]
	private GameObject selectorBackground;

	private List<GameObject> availableSlots = new List<GameObject>();

	private void OnEnable()
	{
		GameData.Instance.OnQuickItemsChanged.Fire();
		UpdateActiveItemsInView();
	}

	private void Start()
	{
		AutoMonoBehaviour<TouchInput>.Instance.Shooter.IgnoreRect(new Rect((float)Screen.width - 240f, 200f, 240f, 100f));
		slot1.actionButton.OnClicked = delegate
		{
			FireActiveQuickItem(slot1);
		};
		slot2.actionButton.OnClicked = delegate
		{
			FireActiveQuickItem(slot2);
		};
		slot3.actionButton.OnClicked = delegate
		{
			FireActiveQuickItem(slot3);
		};
		availableSlots.Add(slot1.gameObject);
		availableSlots.Add(slot2.gameObject);
		availableSlots.Add(slot3.gameObject);
		PropertyExt.AddEventAndFire(GameData.Instance.OnQuickItemsChanged, delegate
		{
			QuickItem[] quickItems = Singleton<QuickItemController>.Instance.QuickItems;
			int currentSlotIndex = Singleton<QuickItemController>.Instance.CurrentSlotIndex;
			slot1.SetQuickItem((quickItems.Length <= 0) ? null : quickItems[0], 0 == currentSlotIndex);
			slot2.SetQuickItem((quickItems.Length <= 1) ? null : quickItems[1], 1 == currentSlotIndex);
			slot3.SetQuickItem((quickItems.Length <= 2) ? null : quickItems[2], 2 == currentSlotIndex);
		}, this);
		UpdateActiveItemsInView();
		GameData.Instance.OnQuickItemsCooldown.AddEventAndFire(delegate(int index, float progress)
		{
			int currentSlotIndex = Singleton<QuickItemController>.Instance.CurrentSlotIndex;
			switch (index)
			{
			case 0:
				slot1.SetCooldown(progress, 0 == currentSlotIndex);
				break;
			case 1:
				slot2.SetCooldown(progress, 1 == currentSlotIndex);
				break;
			case 2:
				slot3.SetCooldown(progress, 2 == currentSlotIndex);
				break;
			}
		}, this);
	}

	private void UpdateActiveItemsInView()
	{
		List<GameObject> activeSlots = new List<GameObject>();
		availableSlots.ForEach(delegate(GameObject el)
		{
			if (el.activeInHierarchy)
			{
				activeSlots.Add(el);
			}
		});
		scrollList.SetActiveElements(activeSlots);
		selectorBackground.SetActive(activeSlots.Count > 1);
	}

	private void FireActiveQuickItem(HUDQuickItem item)
	{
		switch (GetSlotIndex(item))
		{
		case 0:
			EventHandler.Global.Fire(new GlobalEvents.InputChanged(GameInputKey.QuickItem1, 1f));
			break;
		case 1:
			EventHandler.Global.Fire(new GlobalEvents.InputChanged(GameInputKey.QuickItem2, 1f));
			break;
		case 2:
			EventHandler.Global.Fire(new GlobalEvents.InputChanged(GameInputKey.QuickItem3, 1f));
			break;
		}
	}

	private int GetSlotIndex(HUDQuickItem item)
	{
		if (availableSlots.Contains(item.gameObject))
		{
			return availableSlots.IndexOf(item.gameObject);
		}
		return -1;
	}
}

using UnityEngine;

public class HUDDesktopQuickItems : MonoBehaviour
{
	[SerializeField]
	private HUDQuickItem item0;

	[SerializeField]
	private HUDQuickItem item1;

	[SerializeField]
	private HUDQuickItem item2;

	private void Start()
	{
		PropertyExt.AddEventAndFire(GameData.Instance.OnQuickItemsChanged, delegate
		{
			QuickItem[] quickItems = Singleton<QuickItemController>.Instance.QuickItems;
			int currentSlotIndex = Singleton<QuickItemController>.Instance.CurrentSlotIndex;
			item0.SetQuickItem((quickItems.Length <= 0) ? null : quickItems[0], 0 == currentSlotIndex);
			item1.SetQuickItem((quickItems.Length <= 1) ? null : quickItems[1], 1 == currentSlotIndex);
			item2.SetQuickItem((quickItems.Length <= 2) ? null : quickItems[2], 2 == currentSlotIndex);
		}, this);
		GameData.Instance.OnQuickItemsCooldown.AddEventAndFire(delegate(int index, float progress)
		{
			int currentSlotIndex = Singleton<QuickItemController>.Instance.CurrentSlotIndex;
			switch (index)
			{
			case 0:
				item0.SetCooldown(progress, 0 == currentSlotIndex);
				break;
			case 1:
				item1.SetCooldown(progress, 1 == currentSlotIndex);
				break;
			case 2:
				item2.SetCooldown(progress, 2 == currentSlotIndex);
				break;
			}
		}, this);
	}

	private void OnEnable()
	{
		GameData.Instance.OnQuickItemsChanged.Fire();
	}
}

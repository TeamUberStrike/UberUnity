using System;
using System.Collections.Generic;
using UberStrike.Core.Types;
using UnityEngine;

public class ItemListPopupDialog : BasePopupDialog
{
	private List<IUnityItem> _items;

	private string _customMessage = string.Empty;

	private ItemListPopupDialog()
	{
		_cancelCaption = LocalizedStrings.OkCaps;
		_alertType = PopupSystem.AlertType.Cancel;
	}

	public ItemListPopupDialog(string title, string text, List<IUnityItem> items, Action callbackOk = null)
		: this()
	{
		Title = title;
		Text = text;
		_size.y = 320f;
		_items = new List<IUnityItem>(items);
		foreach (IUnityItem item in _items)
		{
			if (item != null)
			{
				Singleton<InventoryManager>.Instance.HighlightItem(item.View.ID, true);
			}
		}
		if (items.Count > 1)
		{
			_callbackOk = callbackOk;
			_alertType = PopupSystem.AlertType.OK;
			_actionType = PopupSystem.ActionType.Positive;
			_okCaption = LocalizedStrings.OkCaps;
		}
	}

	public ItemListPopupDialog(IUnityItem item, string customMessage = "")
		: this()
	{
		Title = LocalizedStrings.NewItem;
		_customMessage = customMessage;
		if (item != null)
		{
			_items = new List<IUnityItem> { item };
			foreach (IUnityItem item2 in _items)
			{
				if (item2 != null)
				{
					Singleton<InventoryManager>.Instance.HighlightItem(item2.View.ID, true);
				}
			}
			if (item.View.ItemType != UberstrikeItemType.Gear && item.View.ItemType != UberstrikeItemType.Weapon && item.View.ItemType != UberstrikeItemType.QuickUse)
			{
				return;
			}
			_alertType = PopupSystem.AlertType.OKCancel;
			_actionType = PopupSystem.ActionType.Positive;
			_okCaption = LocalizedStrings.Equip;
			_cancelCaption = LocalizedStrings.NotNow;
			_callbackOk = delegate
			{
				IUnityItem unityItem = _items[0];
				if (unityItem != null)
				{
					Singleton<InventoryManager>.Instance.EquipItem(unityItem);
				}
			};
			_callbackCancel = delegate
			{
			};
		}
		else
		{
			_items = new List<IUnityItem>();
		}
	}

	protected override void DrawPopupWindow()
	{
		if (_items.Count == 0)
		{
			GUI.Label(new Rect(17f, 115f, _size.x - 34f, 20f), "There are no items", BlueStonez.label_interparkbold_13pt);
		}
		else if (_items.Count == 1)
		{
			if (_items[0] == null)
			{
				return;
			}
			_items[0].DrawIcon(new Rect(_size.x * 0.5f - 32f, 55f, 64f, 64f));
			GUI.Label(new Rect(17f, 115f, _size.x - 34f, 20f), _items[0].Name, BlueStonez.label_interparkbold_13pt);
			if (_items[0].View != null)
			{
				string text = _items[0].View.Description + _customMessage;
				if (string.IsNullOrEmpty(text))
				{
					text = "No description available.";
				}
				GUI.Label(new Rect(17f, 140f, _size.x - 34f, 40f), text, BlueStonez.label_interparkmed_11pt);
			}
		}
		else if (_items.Count <= 4)
		{
			DrawItemsInColumns(2);
		}
		else if (_items.Count <= 6)
		{
			DrawItemsInColumns(3);
		}
		else if (_items.Count <= 8)
		{
			DrawItemsInColumns(4);
		}
		else
		{
			GUI.Label(new Rect(17f, 150f, _size.x - 34f, 20f), Text, BlueStonez.label_interparkbold_13pt);
		}
	}

	private void DrawItemsInColumns(int columns)
	{
		int num = 0;
		int num2 = 0;
		float num3 = _size.x * 0.5f - (float)(64 * columns) / 2f - (float)(15 * (columns - 1)) / 2f;
		foreach (IUnityItem item in _items)
		{
			if (item != null)
			{
				item.DrawIcon(new Rect(num3 + (float)(num % columns * 79), 55 + num2 * 70, 48f, 48f));
				GUI.Label(new Rect(num3 + (float)(num % columns * 79) - 7f, 110 + num2 * 70, 79f, 20f), item.Name, BlueStonez.label_interparkmed_11pt);
			}
			num++;
			num2 = num / columns;
		}
		GUI.Label(new Rect(17f, 220f, _size.x - 34f, 40f), Text, BlueStonez.label_interparkmed_11pt);
	}
}

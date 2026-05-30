using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.ViewModel;
using UberStrike.Realtime.UnitySdk;
using UberStrike.WebService.Unity;
using UnityEngine;

public class TransactionHistory : Singleton<TransactionHistory>
{
	private enum TransactionType
	{
		Item = 0,
		Point = 1,
		Credit = 2
	}

	private enum AccountArea
	{
		Items = 0,
		Points = 1,
		Credits = 2
	}

	public class TransactionCache<T>
	{
		private float _refreshLastPage;

		public SortedList<int, T> PageCache { get; private set; }

		public int CurrentPageIndex { get; set; }

		public T CurrentPage
		{
			get
			{
				if (PageCache.ContainsKey(CurrentPageIndex))
				{
					return PageCache[CurrentPageIndex];
				}
				return default(T);
			}
		}

		public bool CurrentPageNeedsRefresh
		{
			get
			{
				return CurrentPage == null || (CurrentPageIndex > 0 && CurrentPageIndex == PageCount - 1 && _refreshLastPage < Time.time);
			}
		}

		public int ElementCount { get; set; }

		public int PageCount
		{
			get
			{
				return Mathf.CeilToInt(ElementCount / 15) + 1;
			}
		}

		public TransactionCache()
		{
			PageCache = new SortedList<int, T>();
		}

		public void SetPage(int index, T page)
		{
			PageCache[index] = page;
			if (index + 1 == PageCount)
			{
				_refreshLastPage = Time.time + 30f;
			}
		}
	}

	private const float RowHeight = 23f;

	private const float ButtonWidth = 100f;

	private const float ButtonHeight = 32f;

	private const int ElementsPerPage = 15;

	private static string DATE_FORMAT = "yyyy/MM/dd";

	private int _selectedTab;

	private GUIContent[] _tabs;

	private Vector2 _scrollControls;

	private string[] _itemsTableColumnHeadingArray;

	private string[] _pointsTableColumnHeadingArray;

	private string[] _creditsTableColumnHeadingArray;

	private string _prevPageButtonLabel;

	private string _nextPageButtonLabel;

	private TransactionCache<ItemTransactionsViewModel> _itemTransactions;

	private TransactionCache<PointDepositsViewModel> _pointTransactions;

	private TransactionCache<CurrencyDepositsViewModel> _creditTransactions;

	private TransactionHistory()
	{
		_itemTransactions = new TransactionCache<ItemTransactionsViewModel>();
		_pointTransactions = new TransactionCache<PointDepositsViewModel>();
		_creditTransactions = new TransactionCache<CurrencyDepositsViewModel>();
		_tabs = new GUIContent[3]
		{
			new GUIContent("Items"),
			new GUIContent("Points"),
			new GUIContent("Credits")
		};
		_itemsTableColumnHeadingArray = new string[5] { "Date", "Item Name", "Points", "Credits", "Duration" };
		_pointsTableColumnHeadingArray = new string[3] { "Date", "Points", "Type" };
		_creditsTableColumnHeadingArray = new string[6] { "Transaction Key", "Date", "Cost", "Credits", "Points", "Bundle Name" };
		_prevPageButtonLabel = "Prev Page";
		_nextPageButtonLabel = "Next Page";
	}

	public void DrawPanel(Rect panelRect)
	{
		GUI.BeginGroup(panelRect, GUIContent.none, BlueStonez.window_standard_grey38);
		DrawTabs(new Rect(2f, 5f, panelRect.width - 4f, 30f));
		DrawTable(new Rect(2f, 35f, panelRect.width - 4f, panelRect.height - 35f));
		GUI.EndGroup();
	}

	private void DrawTable(Rect panelRect)
	{
		Rect headingRect = new Rect(panelRect.x + 5f, panelRect.y, panelRect.width - 10f, 25f);
		Rect scrollViewRect = new Rect(panelRect.x + 5f, panelRect.y + 30f, panelRect.width - 10f, panelRect.height - 35f - 32f - 5f);
		Rect rect = new Rect(0f, scrollViewRect.y + scrollViewRect.height, panelRect.width, panelRect.height - scrollViewRect.height);
		switch ((TransactionType)_selectedTab)
		{
		case TransactionType.Item:
			DrawItemsTableHeadingBar(headingRect);
			DrawItemsTableContent(scrollViewRect);
			DrawItemsButtons(rect);
			break;
		case TransactionType.Point:
			DrawPointsTableHeadingBar(headingRect);
			DrawPointsTableContent(scrollViewRect);
			DrawPointsButtons(rect);
			break;
		case TransactionType.Credit:
			DrawCreditsTableHeadingBar(headingRect);
			DrawCreditsTableContent(scrollViewRect);
			DrawCreditsButtons(rect);
			break;
		}
	}

	private void DrawTabs(Rect tabRect)
	{
		int num = UnityGUI.Toolbar(tabRect, _selectedTab, _tabs, _tabs.Length, BlueStonez.tab_medium);
		if (num != _selectedTab)
		{
			_selectedTab = num;
			GetCurrentTransactions();
		}
	}

	private float GetColumnOffset(AccountArea area, int index, float totalWidth)
	{
		switch (area)
		{
		case AccountArea.Items:
			switch (index)
			{
			case 0:
				return 0f;
			case 1:
				return 100f;
			case 2:
				return 100f + Mathf.Max(totalWidth, 400f) - 300f;
			case 3:
				return 100f + Mathf.Max(totalWidth, 400f) - 300f + 50f;
			case 4:
				return 100f + Mathf.Max(totalWidth, 400f) - 300f + 50f + 50f;
			default:
				return 0f;
			}
		case AccountArea.Points:
			return index * Mathf.RoundToInt(totalWidth / 3f);
		case AccountArea.Credits:
			switch (index)
			{
			case 0:
				return 0f;
			case 1:
				return 150f;
			case 2:
				return 220f;
			case 3:
				return 270f;
			case 4:
				return 320f;
			case 5:
				return 370f;
			default:
				return 0f;
			}
		default:
			return 0f;
		}
	}

	private float GetColumnWidth(AccountArea area, int index, float totalWidth)
	{
		switch (area)
		{
		case AccountArea.Items:
			switch (index)
			{
			case 0:
				return 151f;
			case 1:
				return Mathf.Max(totalWidth, 400f) - 300f + 1f;
			case 2:
				return 51f;
			case 3:
				return 51f;
			case 4:
				return 100f;
			default:
				return 0f;
			}
		case AccountArea.Points:
			switch (index)
			{
			case 0:
				return Mathf.RoundToInt(totalWidth / 3f) + 1;
			case 1:
				return Mathf.RoundToInt(totalWidth / 3f) + 1;
			case 2:
				return Mathf.RoundToInt(totalWidth / 3f);
			default:
				return 0f;
			}
		case AccountArea.Credits:
			switch (index)
			{
			case 0:
				return 151f;
			case 1:
				return 71f;
			case 2:
				return 51f;
			case 3:
				return 51f;
			case 4:
				return 51f;
			case 5:
				return Mathf.Max(totalWidth, 450f) - 370f;
			default:
				return 0f;
			}
		default:
			return 0f;
		}
	}

	private void DrawItemsTableHeadingBar(Rect headingRect)
	{
		GUI.BeginGroup(headingRect);
		for (int i = 0; i < _itemsTableColumnHeadingArray.Length; i++)
		{
			Rect position = new Rect(GetColumnOffset(AccountArea.Items, i, headingRect.width), 0f, GetColumnWidth(AccountArea.Items, i, headingRect.width), headingRect.height);
			GUI.Button(position, string.Empty, BlueStonez.box_grey50);
			GUI.Label(position, new GUIContent(_itemsTableColumnHeadingArray[i]), BlueStonez.label_interparkmed_11pt);
		}
		GUI.EndGroup();
	}

	private void DrawItemsTableContent(Rect scrollViewRect)
	{
		GUI.Box(scrollViewRect, GUIContent.none, BlueStonez.window_standard_grey38);
		if (_itemTransactions.CurrentPage == null)
		{
			return;
		}
		_scrollControls = GUITools.BeginScrollView(scrollViewRect.Expand(0, -1), _scrollControls, new Rect(0f, 0f, scrollViewRect.width - 17f, (float)_itemTransactions.CurrentPage.ItemTransactions.Count * 23f));
		float num = 0f;
		foreach (ItemTransactionView itemTransaction in _itemTransactions.CurrentPage.ItemTransactions)
		{
			IUnityItem itemInShop = Singleton<ItemManager>.Instance.GetItemInShop(itemTransaction.ItemId);
			string text = ((itemInShop == null) ? string.Format("item[{0}]", itemTransaction.ItemId) : TextUtility.ShortenText(itemInShop.Name, 20, true));
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Items, 0, scrollViewRect.width), num, GetColumnWidth(AccountArea.Items, 0, scrollViewRect.width), 23f), itemTransaction.WithdrawalDate.ToString(DATE_FORMAT), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Items, 1, scrollViewRect.width), num, GetColumnWidth(AccountArea.Items, 1, scrollViewRect.width), 23f), text, BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Items, 2, scrollViewRect.width), num, GetColumnWidth(AccountArea.Items, 2, scrollViewRect.width), 23f), itemTransaction.Points.ToString(), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Items, 3, scrollViewRect.width), num, GetColumnWidth(AccountArea.Items, 3, scrollViewRect.width), 23f), itemTransaction.Credits.ToString(), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Items, 4, scrollViewRect.width), num, GetColumnWidth(AccountArea.Items, 4, scrollViewRect.width), 23f), ShopUtils.PrintDuration(itemTransaction.Duration), BlueStonez.label_interparkmed_11pt);
			num += 23f;
		}
		GUITools.EndScrollView();
	}

	private void DrawItemsButtons(Rect rect)
	{
		GUIStyle button = BlueStonez.button;
		GUI.enabled = _itemTransactions.CurrentPageIndex != 0;
		if (GUITools.Button(new Rect(rect.x + 6f, rect.y + 5f, 100f, 32f), new GUIContent(_prevPageButtonLabel), button))
		{
			_itemTransactions.CurrentPageIndex--;
			AsyncGetItemTransactions();
		}
		GUI.enabled = true;
		if (_itemTransactions.ElementCount > 0)
		{
			GUI.Label(new Rect((rect.x + rect.width) / 2f - 100f, rect.y + 5f, 200f, 32f), string.Format("Page {0} of {1}", _itemTransactions.CurrentPageIndex + 1, _itemTransactions.PageCount), BlueStonez.label_interparkbold_11pt);
			GUI.enabled = _itemTransactions.CurrentPageIndex + 1 < _itemTransactions.PageCount;
			if (GUITools.Button(new Rect(rect.x + rect.width - 100f - 2f, rect.y + 5f, 100f, 32f), new GUIContent(_nextPageButtonLabel), button))
			{
				_itemTransactions.CurrentPageIndex++;
				AsyncGetItemTransactions();
			}
			GUI.enabled = true;
		}
	}

	private void DrawPointsTableHeadingBar(Rect headingRect)
	{
		GUI.BeginGroup(headingRect);
		for (int i = 0; i < _pointsTableColumnHeadingArray.Length; i++)
		{
			Rect position = new Rect(GetColumnOffset(AccountArea.Points, i, headingRect.width), 0f, GetColumnWidth(AccountArea.Points, i, headingRect.width), headingRect.height);
			GUI.Button(position, string.Empty, BlueStonez.box_grey50);
			GUI.Label(position, new GUIContent(_pointsTableColumnHeadingArray[i]), BlueStonez.label_interparkmed_11pt);
		}
		GUI.EndGroup();
	}

	private void DrawPointsTableContent(Rect scrollViewRect)
	{
		GUI.Box(scrollViewRect, GUIContent.none, BlueStonez.window_standard_grey38);
		if (_pointTransactions.CurrentPage == null)
		{
			return;
		}
		_scrollControls = GUITools.BeginScrollView(scrollViewRect.Expand(0, -1), _scrollControls, new Rect(0f, 0f, scrollViewRect.width - 17f, (float)_pointTransactions.CurrentPage.PointDeposits.Count * 23f));
		float num = 0f;
		foreach (PointDepositView pointDeposit in _pointTransactions.CurrentPage.PointDeposits)
		{
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Points, 0, scrollViewRect.width), num, GetColumnWidth(AccountArea.Points, 0, scrollViewRect.width), 23f), pointDeposit.DepositDate.ToString(DATE_FORMAT), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Points, 1, scrollViewRect.width), num, GetColumnWidth(AccountArea.Points, 1, scrollViewRect.width), 23f), pointDeposit.Points.ToString(), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Points, 2, scrollViewRect.width), num, GetColumnWidth(AccountArea.Points, 2, scrollViewRect.width), 23f), pointDeposit.DepositType.ToString(), BlueStonez.label_interparkmed_11pt);
			num += 23f;
		}
		GUITools.EndScrollView();
	}

	private void DrawPointsButtons(Rect rect)
	{
		GUIStyle button = BlueStonez.button;
		GUI.enabled = _pointTransactions.CurrentPageIndex != 0;
		if (GUITools.Button(new Rect(rect.x + 6f, rect.y + 5f, 100f, 32f), new GUIContent(_prevPageButtonLabel), button))
		{
			_pointTransactions.CurrentPageIndex--;
			AsyncGetPointsDeposits();
		}
		GUI.enabled = true;
		if (_pointTransactions.ElementCount > 0)
		{
			GUI.Label(new Rect((rect.x + rect.width) / 2f - 100f, rect.y + 5f, 200f, 32f), string.Format("Page {0} of {1}", _pointTransactions.CurrentPageIndex + 1, _pointTransactions.PageCount), BlueStonez.label_interparkbold_11pt);
			GUI.enabled = _pointTransactions.CurrentPageIndex + 1 < _pointTransactions.PageCount;
			if (GUITools.Button(new Rect(rect.x + rect.width - 100f - 2f, rect.y + 5f, 100f, 32f), new GUIContent(_nextPageButtonLabel), button))
			{
				_pointTransactions.CurrentPageIndex++;
				AsyncGetPointsDeposits();
			}
			GUI.enabled = true;
		}
	}

	private void DrawCreditsTableHeadingBar(Rect headingRect)
	{
		GUI.BeginGroup(headingRect);
		for (int i = 0; i < _creditsTableColumnHeadingArray.Length; i++)
		{
			Rect position = new Rect(GetColumnOffset(AccountArea.Credits, i, headingRect.width), 0f, GetColumnWidth(AccountArea.Credits, i, headingRect.width), headingRect.height);
			GUI.Button(position, string.Empty, BlueStonez.box_grey50);
			GUI.Label(position, new GUIContent(_creditsTableColumnHeadingArray[i]), BlueStonez.label_interparkmed_11pt);
		}
		GUI.EndGroup();
	}

	private void DrawCreditsTableContent(Rect scrollViewRect)
	{
		GUI.Box(scrollViewRect, GUIContent.none, BlueStonez.window_standard_grey38);
		if (_creditTransactions.CurrentPage == null)
		{
			return;
		}
		_scrollControls = GUITools.BeginScrollView(scrollViewRect.Expand(0, -1), _scrollControls, new Rect(0f, 0f, scrollViewRect.width - 17f, (float)_creditTransactions.CurrentPage.CurrencyDeposits.Count * 23f));
		float num = 0f;
		foreach (CurrencyDepositView currencyDeposit in _creditTransactions.CurrentPage.CurrencyDeposits)
		{
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Credits, 0, scrollViewRect.width), num, GetColumnWidth(AccountArea.Credits, 0, scrollViewRect.width), 23f), TextUtility.ShortenText(currencyDeposit.TransactionKey, 20, true), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Credits, 1, scrollViewRect.width), num, GetColumnWidth(AccountArea.Credits, 1, scrollViewRect.width), 23f), currencyDeposit.DepositDate.ToString(DATE_FORMAT), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Credits, 2, scrollViewRect.width), num, GetColumnWidth(AccountArea.Credits, 2, scrollViewRect.width), 23f), currencyDeposit.CurrencyLabel + currencyDeposit.Cash.ToString("#0.00"), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Credits, 3, scrollViewRect.width), num, GetColumnWidth(AccountArea.Credits, 3, scrollViewRect.width), 23f), currencyDeposit.Credits.ToString(), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Credits, 4, scrollViewRect.width), num, GetColumnWidth(AccountArea.Credits, 4, scrollViewRect.width), 23f), currencyDeposit.Points.ToString(), BlueStonez.label_interparkmed_11pt);
			GUI.Label(new Rect(GetColumnOffset(AccountArea.Credits, 5, scrollViewRect.width), num, GetColumnWidth(AccountArea.Credits, 5, scrollViewRect.width), 23f), TextUtility.ShortenText(currencyDeposit.BundleName, 14, true), BlueStonez.label_interparkmed_11pt);
			num += 23f;
		}
		GUITools.EndScrollView();
	}

	private void DrawCreditsButtons(Rect rect)
	{
		GUIStyle button = BlueStonez.button;
		GUI.enabled = _creditTransactions.CurrentPageIndex != 0;
		if (GUITools.Button(new Rect(rect.x + 6f, rect.y + 5f, 100f, 32f), new GUIContent(_prevPageButtonLabel), button))
		{
			_creditTransactions.CurrentPageIndex--;
			AsyncGetCurrencyDeposits();
		}
		GUI.enabled = true;
		if (_creditTransactions.ElementCount > 0)
		{
			GUI.Label(new Rect((rect.x + rect.width) / 2f - 100f, rect.y + 5f, 200f, 32f), string.Format("Page {0} of {1}", _creditTransactions.CurrentPageIndex + 1, _creditTransactions.PageCount), BlueStonez.label_interparkbold_11pt);
			GUI.enabled = _creditTransactions.CurrentPageIndex + 1 < _creditTransactions.PageCount;
			if (GUITools.Button(new Rect(rect.x + rect.width - 100f - 2f, rect.y + 5f, 100f, 32f), new GUIContent(_nextPageButtonLabel), button))
			{
				_creditTransactions.CurrentPageIndex++;
				AsyncGetCurrencyDeposits();
			}
			GUI.enabled = true;
		}
	}

	private void AsyncGetItemTransactions()
	{
		if (_itemTransactions.CurrentPageNeedsRefresh)
		{
			int nextPageIndex = _itemTransactions.CurrentPageIndex;
			UserWebServiceClient.GetItemTransactions(PlayerDataManager.AuthToken, nextPageIndex + 1, 15, delegate(ItemTransactionsViewModel ev)
			{
				_itemTransactions.SetPage(nextPageIndex, ev);
				_itemTransactions.ElementCount = ev.TotalCount;
			}, delegate
			{
			});
		}
	}

	private void AsyncGetCurrencyDeposits()
	{
		if (_creditTransactions.CurrentPageNeedsRefresh)
		{
			int nextPageIndex = _creditTransactions.CurrentPageIndex;
			UserWebServiceClient.GetCurrencyDeposits(PlayerDataManager.AuthToken, nextPageIndex + 1, 15, delegate(CurrencyDepositsViewModel ev)
			{
				_creditTransactions.SetPage(nextPageIndex, ev);
				_creditTransactions.ElementCount = ev.TotalCount;
			}, delegate
			{
			});
		}
	}

	private void AsyncGetPointsDeposits()
	{
		if (_pointTransactions.CurrentPageNeedsRefresh)
		{
			int nextPageIndex = _pointTransactions.CurrentPageIndex;
			UserWebServiceClient.GetPointsDeposits(PlayerDataManager.AuthToken, nextPageIndex + 1, 15, delegate(PointDepositsViewModel ev)
			{
				_pointTransactions.SetPage(nextPageIndex, ev);
				_pointTransactions.ElementCount = ev.TotalCount;
			}, delegate
			{
			});
		}
	}

	public void GetCurrentTransactions()
	{
		switch ((TransactionType)_selectedTab)
		{
		case TransactionType.Credit:
			AsyncGetCurrencyDeposits();
			break;
		case TransactionType.Item:
			AsyncGetItemTransactions();
			break;
		case TransactionType.Point:
			AsyncGetPointsDeposits();
			break;
		}
	}
}

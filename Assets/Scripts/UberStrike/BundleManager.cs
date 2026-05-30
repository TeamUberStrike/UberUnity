using System;
using System.Collections;
using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using Steamworks;
using UberStrike.WebService.Unity;
using UnityEngine;

public class BundleManager : Singleton<BundleManager>
{
	private BasePopupDialog _appStorePopup;

	private Dictionary<BundleCategoryType, List<BundleUnityView>> _bundlesPerCategory;

	private Callback<MicroTxnAuthorizationResponse_t> MicroTxnCallback;

	private float dialogTimer;

	public int Count { get; private set; }

	public bool CanMakeMasPayments { get; private set; }

	public IEnumerable<BundleUnityView> AllItemBundles
	{
		get
		{
			foreach (KeyValuePair<BundleCategoryType, List<BundleUnityView>> category in _bundlesPerCategory)
			{
				if (category.Key == BundleCategoryType.None)
				{
					continue;
				}
				foreach (BundleUnityView item in category.Value)
				{
					yield return item;
				}
			}
		}
	}

	public IEnumerable<BundleUnityView> AllBundles
	{
		get
		{
			foreach (List<BundleUnityView> bundleUnityViews in _bundlesPerCategory.Values)
			{
				foreach (BundleUnityView item in bundleUnityViews)
				{
					yield return item;
				}
			}
		}
	}

	private BundleManager()
	{
		_bundlesPerCategory = new Dictionary<BundleCategoryType, List<BundleUnityView>>();
	}

	private void OnMicroTxnCallback(MicroTxnAuthorizationResponse_t param)
	{
		Debug.Log("Steam MicroTxnParams: " + param);
		if (param.m_bAuthorized > 0)
		{
			ShopWebServiceClient.FinishBuyBundleSteam(param.m_ulOrderID.ToString(), delegate(bool success)
			{
				if (success)
				{
					PopupSystem.ClearAll();
					PopupSystem.ShowMessage("Purchase Successful", "Thank you, your purchase was successful.", PopupSystem.AlertType.OK, delegate
					{
						ApplicationDataManager.RefreshWallet();
					});
				}
				else
				{
					Debug.Log("Managed error from WebServices");
					PopupSystem.ClearAll();
					PopupSystem.ShowMessage("Purchase Failed", "Sorry, there was a problem processing your payment. Please visit support.uberstrike.com for help.", PopupSystem.AlertType.OK);
				}
			}, delegate(Exception ex)
			{
				Debug.Log(ex.Message);
				PopupSystem.ClearAll();
				PopupSystem.ShowMessage("Purchase Failed", "Sorry, there was a problem processing your payment. Please visit support.uberstrike.com for help.", PopupSystem.AlertType.OK);
			});
		}
		else
		{
			Debug.Log("Purchase canceled");
			PopupSystem.ClearAll();
		}
	}

	public List<BundleUnityView> GetCreditBundles()
	{
		List<BundleUnityView> value = new List<BundleUnityView>();
		_bundlesPerCategory.TryGetValue(BundleCategoryType.None, out value);
		return value;
	}

	public void Initialize()
	{
		MicroTxnCallback = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnCallback);
		ShopWebServiceClient.GetBundles(ApplicationDataManager.Channel, delegate(List<BundleView> bundles)
		{
			SetBundles(bundles);
		}, delegate
		{
			Debug.LogError(string.Concat("Error getting ", ApplicationDataManager.Channel, " bundles from the server."));
		});
	}

	private void SetBundles(List<BundleView> bundleViews)
	{
		if (bundleViews != null && bundleViews.Count > 0)
		{
			foreach (BundleView bundleView in bundleViews)
			{
				List<BundleUnityView> value;
				if (!_bundlesPerCategory.TryGetValue(bundleView.Category, out value))
				{
					value = new List<BundleUnityView>();
					_bundlesPerCategory[bundleView.Category] = value;
				}
				value.Add(new BundleUnityView(bundleView));
			}
			Count = 0;
			{
				foreach (BundleUnityView allBundle in AllBundles)
				{
					allBundle.CurrencySymbol = "$";
					allBundle.Price = allBundle.BundleView.USDPrice.ToString("N2");
					allBundle.IsOwned = false;
					Count++;
				}
				return;
			}
		}
		Debug.LogError("SetBundles: Bundles received from the server were null or empty!");
	}

	public IEnumerator StartCancelDialogTimer()
	{
		if (dialogTimer < 5f)
		{
			dialogTimer = 5f;
		}
		while (_appStorePopup != null && dialogTimer > 0f)
		{
			dialogTimer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		if (_appStorePopup != null)
		{
			_appStorePopup.SetAlertType(PopupSystem.AlertType.Cancel);
		}
	}

	public void BuyBundle(BundleUnityView bundle)
	{
		Debug.Log("Trying to buy bundle with id id: " + bundle.BundleView.Id);
		int id = bundle.BundleView.Id;
		string steamId = PlayerDataManager.SteamId;
		string authToken = PlayerDataManager.AuthToken;
		ShopWebServiceClient.BuyBundleSteam(id, steamId, authToken, delegate(bool success)
		{
			if (!success)
			{
				Debug.Log("Starting steam payment failed! (Handled WS Error)");
				PopupSystem.ClearAll();
				PopupSystem.ShowMessage("Purchase Failed", "Sorry, there was a problem processing your payment. Please visit support.uberstrike.com for help.", PopupSystem.AlertType.OK);
			}
		}, delegate(Exception ex)
		{
			Debug.Log(ex.Message);
			PopupSystem.ClearAll();
			PopupSystem.ShowMessage("Purchase Failed", "Sorry, there was a problem processing your payment. Please visit support.uberstrike.com for help.", PopupSystem.AlertType.OK);
		});
		_appStorePopup = PopupSystem.ShowMessage("In App Purchase", "Purchasing, please wait...", PopupSystem.AlertType.None) as BasePopupDialog;
		UnityRuntime.StartRoutine(StartCancelDialogTimer());
	}

	private bool IsItemPackOwned(List<BundleItemView> items)
	{
		if (items.Count > 0)
		{
			foreach (BundleItemView item in items)
			{
				if (!Singleton<InventoryManager>.Instance.Contains(item.ItemId))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public BundleUnityView GetNextItem(BundleUnityView currentItem)
	{
		List<BundleUnityView> list = new List<BundleUnityView>(AllItemBundles);
		if (list.Count > 0)
		{
			int num = list.FindIndex((BundleUnityView i) => i == currentItem);
			if (num < 0)
			{
				return list[UnityEngine.Random.Range(0, list.Count)];
			}
			int index = (num + 1) % list.Count;
			return list[index];
		}
		return currentItem;
	}

	public BundleUnityView GetPreviousItem(BundleUnityView currentItem)
	{
		List<BundleUnityView> list = new List<BundleUnityView>(AllItemBundles);
		if (list.Count > 0)
		{
			int num = list.FindIndex((BundleUnityView i) => i == currentItem);
			if (num < 0)
			{
				return list[UnityEngine.Random.Range(0, list.Count)];
			}
			int index = (num - 1 + list.Count) % list.Count;
			return list[index];
		}
		return currentItem;
	}
}

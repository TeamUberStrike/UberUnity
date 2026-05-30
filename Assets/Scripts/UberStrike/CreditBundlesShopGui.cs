using System.Collections.Generic;
using Cmune.DataCenter.Common.Entities;
using UnityEngine;

public class CreditBundlesShopGui
{
	private Vector2 bundleScroll = Vector2.zero;

	private int scrollHeight;

	private Dictionary<int, float> _alpha = new Dictionary<int, float>();

	public void Draw(Rect position)
	{
		float height = Mathf.Max(position.height, scrollHeight);
		bundleScroll = GUI.BeginScrollView(position, bundleScroll, new Rect(0f, 0f, position.width - 17f, height), false, true);
		List<BundleUnityView> creditBundles = Singleton<BundleManager>.Instance.GetCreditBundles();
		if (creditBundles.Count == 0)
		{
			GUI.Label(new Rect(4f, 4f, position.width - 20f, 24f), "No credit packs are currently on sale.", BlueStonez.label_interparkbold_16pt);
		}
		else
		{
			int num = 4;
			int num2 = 0;
			List<string> list = new List<string>();
			GUI.Label(new Rect(4f, num + 4, position.width - 20f, 20f), "Credit Packs", BlueStonez.label_interparkbold_18pt_left);
			num += 30;
			foreach (BundleUnityView item in creditBundles)
			{
				int num3 = ((num2 % 2 == 1) ? 187 : 0);
				if ((float)num < position.height && num + 95 > 0)
				{
					DrawPackSlot(new Rect(num3, num, 188f, 95f), item);
					list.Add(item.BundleView.IconUrl);
				}
				num += ((num2 % 2 == 1) ? 94 : 0);
				num2++;
			}
			if (num2 % 2 == 1)
			{
				num += 94;
			}
			GUI.Label(new Rect(4f, num, position.width - 8f, 1f), GUIContent.none, BlueStonez.horizontal_line_grey95);
			scrollHeight = num;
		}
		GUI.EndScrollView();
	}

	private void DrawPackSlot(Rect position, BundleUnityView bundleUnityView)
	{
		int id = bundleUnityView.BundleView.Id;
		bool flag = position.Contains(Event.current.mousePosition);
		if (!_alpha.ContainsKey(id))
		{
			_alpha[id] = 0f;
		}
		_alpha[id] = Mathf.Lerp(_alpha[id], flag ? 1 : 0, Time.deltaTime * (float)((!flag) ? 10 : 3));
		GUI.BeginGroup(position);
		GUI.Label(new Rect(2f, 2f, position.width - 4f, 79f), GUIContent.none, BlueStonez.gray_background);
		bundleUnityView.Icon.Draw(new Rect(4f, 4f, 75f, 75f));
		GUI.Label(new Rect(81f, 0f, position.width - 80f, 44f), bundleUnityView.BundleView.Name, BlueStonez.label_interparkbold_13pt_left);
		GUI.enabled = GUITools.SaveClickIn(1f);
		BuyButton(position, bundleUnityView);
		GUI.enabled = true;
		GUI.EndGroup();
	}

	private void BuyButton(Rect position, BundleUnityView bundleUnityView)
	{
		if (GUI.Button(new Rect(81f, 51f, position.width - 110f, 20f), new GUIContent(bundleUnityView.CurrencySymbol + bundleUnityView.Price, "Buy the " + bundleUnityView.BundleView.Name + " pack."), BlueStonez.buttongold_medium))
		{
			GUITools.Clicked();
			if (ApplicationDataManager.Channel == ChannelType.Steam)
			{
				Singleton<BundleManager>.Instance.BuyBundle(bundleUnityView);
				return;
			}
			PopupSystem.ClearAll();
			PopupSystem.ShowMessage("Purchase Failed", "Sorry, only Steam players can purchase credit bundles.", PopupSystem.AlertType.OK);
		}
	}
}

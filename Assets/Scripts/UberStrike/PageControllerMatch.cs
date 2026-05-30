using UberStrike.Core.Types;
using UnityEngine;

public class PageControllerMatch : PageControllerBase
{
	[SerializeField]
	private GameObject healthBar;

	[SerializeField]
	private GameObject ammoBar;

	[SerializeField]
	private GameObject armorBar;

	[SerializeField]
	private GameObject quickItems;

	[SerializeField]
	private GameObject hudReticleController;

	[SerializeField]
	private GameObject hudStatusPanel;

	[SerializeField]
	private GameObject desktopChat;

	[SerializeField]
	private HUDDesktopEventStream eventStream;

	[SerializeField]
	private GameObject itemPickup;

	[SerializeField]
	private GameObject fps;

	[SerializeField]
	private GameObject weaponScroller;

	private void Start()
	{
		GameData.Instance.PlayerState.AddEventAndFire(delegate(PlayerStateId el)
		{
			HandleSharedViews(el, healthBar, ammoBar, armorBar, hudReticleController, hudStatusPanel, itemPickup);
			bool flag = el == PlayerStateId.Playing;
			bool flag2 = el == PlayerStateId.Spectating;
			bool flag3 = el == PlayerStateId.Killed;
			bool flag4 = el == PlayerStateId.Paused;
			bool flag5 = el == PlayerStateId.PrepareForMatch;
			desktopChat.SetActive(flag || flag4 || flag5 || flag3 || flag2);
			eventStream.gameObject.SetActive(flag || flag4 || flag5 || flag3 || flag2);
			weaponScroller.SetActive(flag || flag5);
			quickItems.SetActive(flag || flag5);
			if (eventStream.gameObject.activeInHierarchy)
			{
				eventStream.DoAnimateDown(flag4 || flag3);
			}
			fps.SetActive(true);
		}, this);
		PropertyExt.AddEvent(GameData.Instance.OnHUDStreamClear, delegate
		{
			Singleton<DamageFeedbackHud>.Instance.ClearAll();
		}, this);
	}

	public static void HandleSharedViews(PlayerStateId state, GameObject healthBar, GameObject ammoBar, GameObject armorBar, GameObject hudReticleController, GameObject hudStatusPanel, GameObject itemPickup)
	{
		bool flag = state == PlayerStateId.Playing;
		bool flag2 = state == PlayerStateId.Spectating;
		bool flag3 = state == PlayerStateId.Killed;
		bool flag4 = state == PlayerStateId.Paused;
		bool flag5 = state == PlayerStateId.PrepareForMatch;
		bool flag6 = GameState.Current.GameMode == GameModeType.None;
		bool flag7 = GameData.Instance.GameState.Value == GameStateId.WaitingForPlayers;
		flag2 |= flag4 && (!GameState.Current.Players.ContainsKey(PlayerDataManager.Cmid) || GameState.Current.PlayerData.IsSpectator);
		healthBar.SetActive((flag || flag4 || flag5) && !flag2);
		armorBar.SetActive((flag || flag4 || flag5) && !flag2);
		ammoBar.SetActive((flag || flag5) && !flag2);
		hudReticleController.SetActive(flag || flag5);
		hudStatusPanel.SetActive((flag || flag4 || flag5 || flag2 || flag3) && !flag6 && !flag7);
		if (hudStatusPanel.activeInHierarchy)
		{
			hudStatusPanel.GetComponent<HUDStatusPanel>().IsOnPaused(flag4 || flag3 || flag2);
		}
		itemPickup.SetActive(flag);
	}

	private void Update()
	{
		Singleton<DamageFeedbackHud>.Instance.Update();
	}

	private void OnGUI()
	{
		Singleton<DamageFeedbackHud>.Instance.Draw();
	}
}

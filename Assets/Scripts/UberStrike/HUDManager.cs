using UnityEngine;

public class HUDManager : MonoBehaviour
{
	[SerializeField]
	private PageControllerBase pregameLoadoutPage;

	[SerializeField]
	private PageControllerBase matchRunningPage;

	[SerializeField]
	private PageControllerBase endOfMatchPage;

	private void Start()
	{
		GameData.Instance.GameState.AddEventAndFire(delegate(GameStateId el)
		{
			bool flag = el == GameStateId.MatchRunning;
			bool flag2 = el == GameStateId.PregameLoadout;
			bool flag3 = el == GameStateId.WaitingForPlayers;
			bool flag4 = el == GameStateId.EndOfMatch;
			bool flag5 = el == GameStateId.PrepareNextRound;
			TrySetActive(pregameLoadoutPage, flag2);
			TrySetActive(matchRunningPage, flag || flag3 || flag5);
			TrySetActive(endOfMatchPage, flag4);
			GameData.Instance.PlayerState.Fire();
		}, this);
		EventHandler.Global.AddListener<GlobalEvents.CameraWidthChanged>(OnCameraWidthChanged);
		OnCameraWidthChanged(null);
	}

	private void OnDestroy()
	{
		EventHandler.Global.RemoveListener<GlobalEvents.CameraWidthChanged>(OnCameraWidthChanged);
	}

	private void OnCameraWidthChanged(GlobalEvents.CameraWidthChanged obj)
	{
		UICamera.eventHandler.cachedCamera.rect = new Rect(0f, 0f, AutoMonoBehaviour<CameraRectController>.Instance.NormalizedWidth, 1f);
	}

	private void TrySetActive(MonoBehaviour page, bool active)
	{
		if (page != null)
		{
			page.gameObject.SetActive(active);
		}
	}
}

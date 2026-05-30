using System.Collections;
using System.Collections.Generic;
using System.Text;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class StatsPageGUI : MonoBehaviour
{
	private enum StatisticValueType
	{
		Counter = 0,
		Percent = 1
	}

	private const int StatsWidth = 490;

	[SerializeField]
	private Texture2D _mostSplatsIcon;

	[SerializeField]
	private Texture2D _mostXPEarnedIcon;

	[SerializeField]
	private Texture2D _mostHealthPickedUpIcon;

	[SerializeField]
	private Texture2D _mostArmorPickedUpIcon;

	[SerializeField]
	private Texture2D _mostDamageDealtIcon;

	[SerializeField]
	private Texture2D _mostDamageReceivedIcon;

	[SerializeField]
	private Texture2D _mostHeadshotsIcon;

	[SerializeField]
	private Texture2D _mostNutshotsIcon;

	[SerializeField]
	private Texture2D _mostConsecutiveSnipesIcon;

	[SerializeField]
	private Texture2D _mostMeleeSplatsIcon;

	[SerializeField]
	private Texture2D _mostMachinegunSplatsIcon;

	[SerializeField]
	private Texture2D _mostCannonSplatsIcon;

	[SerializeField]
	private Texture2D _mostShotgunSplatsIcon;

	[SerializeField]
	private Texture2D _mostSniperSplatsIcon;

	[SerializeField]
	private Texture2D _mostSplattergunSplatsIcon;

	[SerializeField]
	private Texture2D _mostLauncherSplatsIcon;

	private Vector2 _scrollGeneral;

	private Vector2 _filterScroll;

	private Rect statsPage;

	private int _selectedStatsTab;

	private int _playerCurrentLevelXpReq;

	private int _playerNextLevelXpReq;

	private Dictionary<string, Texture2D> _weaponIcons;

	private CmunePairList<float, string> _weaponStatList;

	private bool _isFilterDropDownOpen;

	private int _selectedFilterIndex;

	private float _maxWeaponStat;

	private float _xpSliderPos;

	private GUIContent[] _statsTabs;

	private string[] _selectionsToShow;

	private StatisticValueType _currentStatsType;

	private float statsPositionX;

	private void Awake()
	{
		_weaponStatList = new CmunePairList<float, string>();
		EventHandler.Global.AddListener<GlobalEvents.Logout>(delegate
		{
			_selectedStatsTab = 0;
			_selectedFilterIndex = 0;
		});
	}

	private IEnumerator ScrollStatsFromRight(float time)
	{
		float t = 0f;
		while (t < time)
		{
			t += Time.deltaTime;
			statsPositionX = Mathf.Lerp(0f, 490f, t / time * (t / time));
			if (MenuPageManager.Instance != null)
			{
				MenuPageManager.Instance.LeftAreaGUIOffset = statsPositionX;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	private void Start()
	{
		_statsTabs = new GUIContent[3]
		{
			new GUIContent(LocalizedStrings.Personal),
			new GUIContent(LocalizedStrings.Weapons),
			new GUIContent("Account")
		};
		_selectionsToShow = new string[4]
		{
			LocalizedStrings.Damage,
			LocalizedStrings.Kills,
			LocalizedStrings.Accuracy,
			LocalizedStrings.Hits
		};
		_weaponIcons = new Dictionary<string, Texture2D>
		{
			{
				LocalizedStrings.MeleeWeapons,
				_mostMeleeSplatsIcon
			},
			{
				LocalizedStrings.Machineguns,
				_mostMachinegunSplatsIcon
			},
			{
				LocalizedStrings.Cannons,
				_mostCannonSplatsIcon
			},
			{
				LocalizedStrings.Shotguns,
				_mostShotgunSplatsIcon
			},
			{
				LocalizedStrings.Splatterguns,
				_mostSplattergunSplatsIcon
			},
			{
				LocalizedStrings.Launchers,
				_mostLauncherSplatsIcon
			},
			{
				LocalizedStrings.SniperRifles,
				_mostSniperSplatsIcon
			}
		};
	}

	private void OnGUI()
	{
		GUI.depth = 11;
		GUI.skin = BlueStonez.Skin;
		statsPage = new Rect((float)Screen.width - statsPositionX, GlobalUIRibbon.Instance.Height(), statsPositionX, Screen.height - GlobalUIRibbon.Instance.Height());
		GUI.BeginGroup(statsPage, GUIContent.none, BlueStonez.window_standard_grey38);
		GUI.Label(new Rect(0f, 0f, statsPage.width, 56f), LocalizedStrings.YourProfileCaps, BlueStonez.tab_strip);
		GUI.changed = false;
		_selectedStatsTab = UnityGUI.Toolbar(new Rect(0f, 34f, 260f, 22f), _selectedStatsTab, _statsTabs, 3, BlueStonez.tab_medium);
		if (GUI.changed)
		{
			if (_selectedStatsTab == 2)
			{
				Singleton<TransactionHistory>.Instance.GetCurrentTransactions();
			}
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.ButtonClick, 0uL, 1f, 1f);
		}
		GUI.BeginGroup(new Rect(0f, 55f, statsPage.width, statsPage.height - 55f), string.Empty, BlueStonez.window_standard_grey38);
		switch (_selectedStatsTab)
		{
		case 0:
			DrawPersonalStatsTab(new Rect(0f, 0f, statsPage.width, statsPage.height - 56f));
			break;
		case 1:
			DrawWeaponsStatsTab(new Rect(0f, 0f, statsPage.width, statsPage.height - 56f));
			break;
		case 2:
			DrawAccountStatsTab(new Rect(0f, 0f, statsPage.width, statsPage.height - 55f));
			break;
		}
		GUI.EndGroup();
		GUI.EndGroup();
	}

	private void OnEnable()
	{
		XpPointsUtil.GetXpRangeForLevel(PlayerDataManager.PlayerLevel, out _playerCurrentLevelXpReq, out _playerNextLevelXpReq);
		StartCoroutine(ScrollStatsFromRight(0.25f));
		UpdateWeaponStatList();
		if (MouseOrbit.Instance != null)
		{
			MouseOrbit.Instance.MaxX = Screen.width - 490;
		}
	}

	private void OnDisable()
	{
		if (MouseOrbit.Instance != null)
		{
			MouseOrbit.Instance.MaxX = Screen.width;
		}
	}

	private void DrawWeaponsStatsTab(Rect rect)
	{
		bool flag = GUI.enabled;
		GUI.enabled = !_isFilterDropDownOpen;
		GUI.changed = false;
		int num = UnityGUI.Toolbar(new Rect(2f, 5f, rect.width - 4f, 22f), _selectedFilterIndex, _selectionsToShow, 4, BlueStonez.tab_medium);
		if (GUI.changed)
		{
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.ButtonClick, 0uL, 1f, 1f);
		}
		if (num != _selectedFilterIndex)
		{
			_selectedFilterIndex = num;
			UpdateWeaponStatList();
		}
		string title = LocalizedStrings.WeaponPerformaceTotal;
		switch (num)
		{
		case 0:
			title = LocalizedStrings.BestWeaponByDamageDealt;
			break;
		case 1:
			title = LocalizedStrings.BestWeaponByKills;
			break;
		case 2:
			title = LocalizedStrings.BestWeaponByAccuracy;
			break;
		case 3:
			title = LocalizedStrings.BestWeaponByHits;
			break;
		}
		_scrollGeneral = GUITools.BeginScrollView(new Rect(0f, 26f, rect.width - 2f, rect.height - 26f), _scrollGeneral, new Rect(0f, 0f, 340f, 680f));
		DrawGroupControl(new Rect(14f, 16f, rect.width - 40f, 646f), title, BlueStonez.label_group_interparkbold_18pt);
		int num2 = Mathf.RoundToInt((statsPage.width - 80f) * 0.5f);
		int num3 = 0;
		foreach (KeyValuePair<float, string> weaponStat in _weaponStatList)
		{
			float barValue = ((num != 2) ? ((!(_maxWeaponStat > 0f)) ? 0f : (weaponStat.Key / _maxWeaponStat)) : (weaponStat.Key / 100f));
			DrawWeaponStat(new Rect(36f, 32 + num3 * 76, num2, 60f), weaponStat.Value, weaponStat.Key, barValue, _weaponIcons[weaponStat.Value]);
			num3++;
		}
		GUITools.EndScrollView();
		GUI.enabled = flag;
	}

	private void UpdateWeaponStatList()
	{
		_weaponStatList.Clear();
		PlayerWeaponStatisticsView weaponStatistics = Singleton<PlayerDataManager>.Instance.ServerLocalPlayerStatisticsView.WeaponStatistics;
		switch (_selectedFilterIndex)
		{
		case 0:
			_weaponStatList.Add(weaponStatistics.MeleeTotalDamageDone, LocalizedStrings.MeleeWeapons);
			_weaponStatList.Add(weaponStatistics.MachineGunTotalDamageDone, LocalizedStrings.Machineguns);
			_weaponStatList.Add(weaponStatistics.CannonTotalDamageDone, LocalizedStrings.Cannons);
			_weaponStatList.Add(weaponStatistics.ShotgunTotalDamageDone, LocalizedStrings.Shotguns);
			_weaponStatList.Add(weaponStatistics.SplattergunTotalDamageDone, LocalizedStrings.Splatterguns);
			_weaponStatList.Add(weaponStatistics.LauncherTotalDamageDone, LocalizedStrings.Launchers);
			_weaponStatList.Add(weaponStatistics.SniperTotalDamageDone, LocalizedStrings.SniperRifles);
			_currentStatsType = StatisticValueType.Counter;
			break;
		case 1:
			_weaponStatList.Add(weaponStatistics.MeleeTotalSplats, LocalizedStrings.MeleeWeapons);
			_weaponStatList.Add(weaponStatistics.MachineGunTotalSplats, LocalizedStrings.Machineguns);
			_weaponStatList.Add(weaponStatistics.CannonTotalSplats, LocalizedStrings.Cannons);
			_weaponStatList.Add(weaponStatistics.ShotgunTotalSplats, LocalizedStrings.Shotguns);
			_weaponStatList.Add(weaponStatistics.SplattergunTotalSplats, LocalizedStrings.Splatterguns);
			_weaponStatList.Add(weaponStatistics.LauncherTotalSplats, LocalizedStrings.Launchers);
			_weaponStatList.Add(weaponStatistics.SniperTotalSplats, LocalizedStrings.SniperRifles);
			_currentStatsType = StatisticValueType.Counter;
			break;
		case 2:
			_weaponStatList.Add((weaponStatistics.MeleeTotalShotsHit != 0) ? (Mathf.Clamp01((float)weaponStatistics.MeleeTotalShotsHit / (float)weaponStatistics.MeleeTotalShotsFired) * 100f) : 0f, LocalizedStrings.MeleeWeapons);
			_weaponStatList.Add((weaponStatistics.MachineGunTotalShotsHit != 0) ? (Mathf.Clamp01((float)weaponStatistics.MachineGunTotalShotsHit / (float)weaponStatistics.MachineGunTotalShotsFired) * 100f) : 0f, LocalizedStrings.Machineguns);
			_weaponStatList.Add((weaponStatistics.CannonTotalShotsHit != 0) ? (Mathf.Clamp01((float)weaponStatistics.CannonTotalShotsHit / (float)weaponStatistics.CannonTotalShotsFired) * 100f) : 0f, LocalizedStrings.Cannons);
			_weaponStatList.Add((weaponStatistics.ShotgunTotalShotsHit != 0) ? (Mathf.Clamp01((float)weaponStatistics.ShotgunTotalShotsHit / (float)weaponStatistics.ShotgunTotalShotsFired) * 100f) : 0f, LocalizedStrings.Shotguns);
			_weaponStatList.Add((weaponStatistics.SplattergunTotalShotsHit != 0) ? (Mathf.Clamp01((float)weaponStatistics.SplattergunTotalShotsHit / (float)weaponStatistics.SplattergunTotalShotsFired) * 100f) : 0f, LocalizedStrings.Splatterguns);
			_weaponStatList.Add((weaponStatistics.LauncherTotalShotsHit != 0) ? (Mathf.Clamp01((float)weaponStatistics.LauncherTotalShotsHit / (float)weaponStatistics.LauncherTotalShotsFired) * 100f) : 0f, LocalizedStrings.Launchers);
			_weaponStatList.Add((weaponStatistics.SniperTotalShotsHit != 0) ? (Mathf.Clamp01((float)weaponStatistics.SniperTotalShotsHit / (float)weaponStatistics.SniperTotalShotsFired) * 100f) : 0f, LocalizedStrings.SniperRifles);
			_currentStatsType = StatisticValueType.Percent;
			break;
		case 3:
			_weaponStatList.Add(weaponStatistics.MeleeTotalShotsHit, LocalizedStrings.MeleeWeapons);
			_weaponStatList.Add(weaponStatistics.MachineGunTotalShotsHit, LocalizedStrings.Machineguns);
			_weaponStatList.Add(weaponStatistics.CannonTotalShotsHit, LocalizedStrings.Cannons);
			_weaponStatList.Add(weaponStatistics.ShotgunTotalShotsHit, LocalizedStrings.Shotguns);
			_weaponStatList.Add(weaponStatistics.SplattergunTotalShotsHit, LocalizedStrings.Splatterguns);
			_weaponStatList.Add(weaponStatistics.LauncherTotalShotsHit, LocalizedStrings.Launchers);
			_weaponStatList.Add(weaponStatistics.SniperTotalShotsHit, LocalizedStrings.SniperRifles);
			_currentStatsType = StatisticValueType.Counter;
			break;
		}
		_weaponStatList.Sort((KeyValuePair<float, string> a, KeyValuePair<float, string> b) => -a.Key.CompareTo(b.Key));
		_maxWeaponStat = 0f;
		foreach (KeyValuePair<float, string> weaponStat in _weaponStatList)
		{
			if (weaponStat.Key > _maxWeaponStat)
			{
				_maxWeaponStat = weaponStat.Key;
			}
		}
	}

	private void DebugWeaponStatistics()
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<float, string> weaponStat in _weaponStatList)
		{
			stringBuilder.AppendLine(num++ + ": " + weaponStat.Key + " " + weaponStat.Value);
		}
	}

	private void DrawPersonalStatsTab(Rect rect)
	{
		_scrollGeneral = GUITools.BeginScrollView(rect, _scrollGeneral, new Rect(0f, 0f, 340f, 915f));
		int num = Mathf.RoundToInt((rect.width - 80f) * 0.5f);
		PlayerPersonalRecordStatisticsView personalRecord = Singleton<PlayerDataManager>.Instance.ServerLocalPlayerStatisticsView.PersonalRecord;
		DrawGroupControl(new Rect(14f, 16f, rect.width - 40f, 100f), LocalizedStrings.LevelAndXP, BlueStonez.label_group_interparkbold_18pt);
		DrawXPMeter(new Rect(24f, 32f, rect.width - 60f, 64f));
		DrawGroupControl(new Rect(14f, 142f, rect.width - 40f, 405f), LocalizedStrings.PersonalRecordsPerLife, BlueStonez.label_group_interparkbold_18pt);
		DrawPersonalStat(36, 158, num, LocalizedStrings.MostKills, personalRecord.MostSplats.ToString(), _mostSplatsIcon);
		DrawPersonalStat(36, 234, num, LocalizedStrings.MostDamageDealt, personalRecord.MostDamageDealt.ToString(), _mostDamageDealtIcon);
		DrawPersonalStat(36, 310, num, LocalizedStrings.MostHealthPickedUp, personalRecord.MostHealthPickedUp.ToString(), _mostHealthPickedUpIcon);
		DrawPersonalStat(36, 386, num, LocalizedStrings.MostHeadshots, personalRecord.MostHeadshots.ToString(), _mostHeadshotsIcon);
		DrawPersonalStat(36, 462, num, LocalizedStrings.MostConsecutiveSnipes, personalRecord.MostConsecutiveSnipes.ToString(), _mostConsecutiveSnipesIcon);
		DrawPersonalStat(36 + num, 158, num, LocalizedStrings.MostXPEarned, personalRecord.MostXPEarned.ToString(), _mostXPEarnedIcon);
		DrawPersonalStat(36 + num, 234, num, LocalizedStrings.MostDamageReceived, personalRecord.MostDamageReceived.ToString(), _mostDamageReceivedIcon);
		DrawPersonalStat(36 + num, 310, num, LocalizedStrings.MostArmorPickedUp, personalRecord.MostArmorPickedUp.ToString(), _mostArmorPickedUpIcon);
		DrawPersonalStat(36 + num, 386, num, LocalizedStrings.MostNutshots, personalRecord.MostNutshots.ToString(), _mostNutshotsIcon);
		DrawGroupControl(new Rect(14f, 575f, statsPage.width - 40f, 328f), "Weapon Records (per Life)", BlueStonez.label_group_interparkbold_18pt);
		DrawPersonalStat(36, 593, num, LocalizedStrings.MostMeleeKills, personalRecord.MostMeleeSplats.ToString(), _mostMeleeSplatsIcon);
		DrawPersonalStat(36, 669, num, LocalizedStrings.MostMachinegunKills, personalRecord.MostMachinegunSplats.ToString(), _mostMachinegunSplatsIcon);
		DrawPersonalStat(36, 745, num, LocalizedStrings.MostShotgunKills, personalRecord.MostShotgunSplats.ToString(), _mostShotgunSplatsIcon);
		DrawPersonalStat(36, 821, num, LocalizedStrings.MostSplattergunKills, personalRecord.MostSplattergunSplats.ToString(), _mostSplattergunSplatsIcon);
		DrawPersonalStat(36 + num, 669, num, LocalizedStrings.MostCannonKills, personalRecord.MostCannonSplats.ToString(), _mostCannonSplatsIcon);
		DrawPersonalStat(36 + num, 745, num, LocalizedStrings.MostSniperRifleKills, personalRecord.MostSniperSplats.ToString(), _mostSniperSplatsIcon);
		DrawPersonalStat(36 + num, 821, num, LocalizedStrings.MostLauncherKills, personalRecord.MostLauncherSplats.ToString(), _mostLauncherSplatsIcon);
		GUITools.EndScrollView();
	}

	private void DrawAccountStatsTab(Rect rect)
	{
		Singleton<TransactionHistory>.Instance.DrawPanel(rect);
	}

	private void DoDropDownList(Rect position)
	{
		Rect rect = new Rect(position.x, position.y, position.width - position.height, position.height);
		Rect position2 = new Rect(position.x + position.width - position.height - 2f, position.y - 1f, position.height, position.height);
		if (GUI.Button(position2, GUIContent.none, BlueStonez.dropdown_button))
		{
			_isFilterDropDownOpen = !_isFilterDropDownOpen;
		}
		if (_isFilterDropDownOpen)
		{
			Rect position3 = new Rect(position.x, position.y + position.height, position.width - position.height, _selectionsToShow.Length * 20 + 1);
			GUI.Box(position3, string.Empty, BlueStonez.window_standard_grey38);
			_filterScroll = GUITools.BeginScrollView(position3, _filterScroll, new Rect(0f, 0f, position3.width - 20f, _selectionsToShow.Length * 20));
			for (int i = 0; i < _selectionsToShow.Length; i++)
			{
				GUI.Label(new Rect(4f, i * 20, position3.width, 20f), _selectionsToShow[i], BlueStonez.label_interparkbold_11pt_left);
				if (GUI.Button(new Rect(2f, i * 20, position3.width, 20f), string.Empty, BlueStonez.dropdown_list))
				{
					_isFilterDropDownOpen = false;
					_selectedFilterIndex = i;
					UpdateWeaponStatList();
				}
			}
			GUITools.EndScrollView();
		}
		else if (GUITools.Button(rect, new GUIContent(_selectionsToShow[_selectedFilterIndex]), BlueStonez.label_dropdown))
		{
			_isFilterDropDownOpen = !_isFilterDropDownOpen;
		}
	}

	private void DrawPersonalStat(int x, int y, int width, string statName, string statValue, Texture2D icon)
	{
		GUI.Label(new Rect(x, y, width, 20f), statName, BlueStonez.label_interparkbold_13pt_left);
		GUI.DrawTexture(new Rect(x, y + 22, 48f, 48f), icon);
		GUI.Label(new Rect(x + 54, y + 36, width - 54, 20f), statValue, BlueStonez.label_interparkbold_18pt_left);
	}

	private void DrawWeaponStat(Rect position, string statName, float statValue, float barValue, Texture2D icon)
	{
		GUI.Label(new Rect(position.x, position.y, position.width, 20f), statName, BlueStonez.label_interparkmed_18pt_left);
		GUI.DrawTexture(new Rect(position.x, position.y + 22f, 48f, 48f), icon);
		DrawLevelBar(new Rect(position.x + 54f, position.y + 32f, position.width - 54f, 12f), barValue, ColorScheme.ProgressBar);
		string text = ((_currentStatsType != StatisticValueType.Percent) ? statValue.ToString("f0") : (statValue.ToString("f1") + "%"));
		GUI.Label(new Rect(position.x + 54f, position.y + 48f, position.width - 54f, 20f), text, BlueStonez.label_interparkbold_11pt_left);
	}

	private void DrawLevelBar(Rect position, float amount, Color barColor)
	{
		GUI.BeginGroup(position);
		GUI.Label(new Rect(0f, 0f, position.width, 12f), GUIContent.none, BlueStonez.progressbar_background);
		GUI.color = barColor;
		GUI.Label(new Rect(2f, 2f, (position.width - 4f) * Mathf.Clamp01(amount), 8f), GUIContent.none, BlueStonez.progressbar_thumb);
		GUI.color = Color.white;
		GUI.EndGroup();
	}

	private void DrawXPMeter(Rect position)
	{
		float num = _playerNextLevelXpReq - _playerCurrentLevelXpReq;
		_xpSliderPos = ((!(num > 0f)) ? 0f : Mathf.Clamp01((float)(PlayerDataManager.PlayerExperience - _playerCurrentLevelXpReq) / num));
		GUI.BeginGroup(position);
		if (PlayerDataManager.PlayerLevel < XpPointsUtil.MaxPlayerLevel)
		{
			GUI.Label(new Rect(0f, 0f, 200f, 16f), LocalizedStrings.CurrentXP + " " + PlayerDataManager.PlayerExperience.ToString("N0"), BlueStonez.label_interparkbold_11pt_left);
			GUI.Label(new Rect(position.width - 200f, 0f, 200f, 16f), LocalizedStrings.RemainingXP + " " + Mathf.Max(0, _playerNextLevelXpReq - PlayerDataManager.PlayerExperience).ToString("N0"), BlueStonez.label_interparkbold_11pt_right);
			GUI.Box(new Rect(0f, 25f, position.width, 23f), GUIContent.none, BlueStonez.progressbar_large_background);
			GUI.color = ColorScheme.ProgressBar;
			GUI.Box(new Rect(2f, 27f, Mathf.RoundToInt((position.width - 4f) * _xpSliderPos), 19f), GUIContent.none, BlueStonez.progressbar_large_thumb);
			GUI.color = Color.white;
			GUI.Label(new Rect(0f, 50f, position.width, 16f), XpPointsUtil.GetLevelDescription(PlayerDataManager.PlayerLevel), BlueStonez.label_interparkbold_11pt_left);
			GUI.Label(new Rect(0f, 50f, position.width, 16f), XpPointsUtil.GetLevelDescription(PlayerDataManager.PlayerLevel + 1), BlueStonez.label_interparkbold_11pt_right);
		}
		else
		{
			GUI.Label(new Rect(0f, 0f, 200f, 16f), LocalizedStrings.CurrentXP + " " + PlayerDataManager.PlayerExperience.ToString("N0"), BlueStonez.label_interparkbold_11pt_left);
			GUI.Label(new Rect(position.width - 200f, 0f, 200f, 16f), "(Lvl " + PlayerDataManager.PlayerLevel + ")", BlueStonez.label_interparkbold_11pt_right);
			GUI.color = new Color(0f, 0.607f, 0.662f);
			GUI.Box(new Rect(0f, 30f, position.width, 23f), "YOU ARE FLOATING IN UBER SPACE", BlueStonez.label_interparkbold_18pt);
			GUI.color = Color.white;
		}
		GUI.EndGroup();
	}

	private void DrawStatLabel(Rect position, string label, string text)
	{
		GUI.Label(position, label + ": " + text, BlueStonez.label_interparkbold_16pt);
	}

	private Rect CenteredAspectRect(float aspectRatio, int screenWidth, int screenHeight, int offsetTop, int minpWidth, int minHeight)
	{
		float num = (float)screenWidth / (float)screenHeight;
		Rect result;
		if (num > aspectRatio)
		{
			int num2 = Mathf.Clamp(Mathf.RoundToInt((float)screenHeight * aspectRatio), minpWidth, 2048);
			result = new Rect((float)(screenWidth - num2) * 0.5f, offsetTop, num2, Mathf.Clamp(screenHeight, minHeight, 2048));
		}
		else
		{
			result = new Rect(0f, offsetTop, Mathf.Clamp(screenWidth, minpWidth, 2048), Mathf.Clamp(Mathf.RoundToInt((float)screenWidth / aspectRatio), minHeight, 2048));
		}
		return result;
	}

	private void DrawGroupControl(Rect rect, string title, GUIStyle style)
	{
		GUI.BeginGroup(rect, string.Empty, BlueStonez.group_grey81);
		GUI.EndGroup();
		GUI.Label(new Rect(rect.x + 18f, rect.y - 8f, style.CalcSize(new GUIContent(title)).x + 10f, 16f), title, style);
	}
}

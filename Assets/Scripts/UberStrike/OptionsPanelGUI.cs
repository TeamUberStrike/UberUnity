using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPanelGUI : PanelGuiBase
{
	private class ScreenRes
	{
		public int Index;

		public string Resolution;

		public ScreenRes(int index, string res)
		{
			Index = index;
			Resolution = res;
		}
	}

	private const int MasterTextureLimit = 5;

	private const int TAB_CONTROL = 0;

	private const int TAB_AUDIO = 1;

	private const int TAB_VIDEO = 2;

	private const int TAB_SYSINFO = 3;

	private const int GroupMarginX = 8;

	private bool showResolutions;

	private bool graphicsChanged;

	private string[] qualitySet;

	private string[] vsyncSet = new string[3] { "Off", "Low", "High" };

	private string[] antiAliasingSet = new string[4] { "Off", "2x", "4x", "8x" };

	private int _currentQuality;

	private float _textureQuality;

	private int _vsync;

	private int _antiAliasing;

	private bool _postProcessing;

	private Rect _rect;

	private Vector2 _scrollVideo;

	private Vector2 _scrollControls;

	private int _desiredWidth;

	private int _selectedOptionsTab = 2;

	private GUIContent[] _optionsTabs;

	private UserInputMap _targetMap;

	private bool _showWaterModeMenu;

	private int _keyCount;

	private string[] _screenResText;

	private bool INSTANT_SCREEN_RES_CHANGE = true;

	private bool _isFullscreenBefore;

	private float _screenResChangeDelay;

	private int _newScreenResIndex;

	private void Awake()
	{
		List<string> list = new List<string>();
		int num = 0;
		string arg = string.Empty;
		foreach (Resolution resolution in ScreenResolutionManager.Resolutions)
		{
			if (++num == ScreenResolutionManager.Resolutions.Count)
			{
				arg = string.Format("({0})", LocalizedStrings.FullscreenOnly);
			}
			list.Add(string.Format("{0} X {1} {2}", resolution.width, resolution.height, arg));
		}
		ArrayList arrayList = new ArrayList(QualitySettings.names);
		if (arrayList.Contains("Mobile"))
		{
			arrayList.Remove("Mobile");
		}
		qualitySet = new string[arrayList.Count + 1];
		for (int i = 0; i < qualitySet.Length; i++)
		{
			if (i < arrayList.Count)
			{
				qualitySet[i] = arrayList[i].ToString();
			}
			else
			{
				qualitySet[i] = LocalizedStrings.Custom;
			}
		}
		_screenResText = list.ToArray();
	}

	private void OnEnable()
	{
		SyncGraphicsSettings();
	}

	private void Start()
	{
		if (ApplicationDataManager.IsMobile)
		{
			_optionsTabs = new GUIContent[2]
			{
				new GUIContent(LocalizedStrings.ControlsCaps),
				new GUIContent(LocalizedStrings.AudioCaps)
			};
			_selectedOptionsTab = 0;
		}
		else
		{
			_optionsTabs = new GUIContent[3]
			{
				new GUIContent(LocalizedStrings.ControlsCaps),
				new GUIContent(LocalizedStrings.AudioCaps),
				new GUIContent(LocalizedStrings.VideoCaps)
			};
			_keyCount = AutoMonoBehaviour<InputManager>.Instance.KeyMapping.Values.Count;
		}
	}

	private void OnGUI()
	{
		GUI.depth = -97;
		_rect = new Rect((Screen.width - 528) / 2, (Screen.height - 320) / 2, 528f, 320f);
		GUI.BeginGroup(_rect, GUIContent.none, BlueStonez.window_standard_grey38);
		if (_screenResChangeDelay > 0f)
		{
			DrawScreenResChangePanel();
		}
		else
		{
			DrawOptionsPanel();
		}
		GUI.EndGroup();
		GuiManager.DrawTooltip();
	}

	private void DrawOptionsPanel()
	{
		GUI.SetNextControlName("OptionPanelHeading");
		GUI.Label(new Rect(0f, 0f, _rect.width, 56f), LocalizedStrings.OptionsCaps, BlueStonez.tab_strip);
		if (GUI.GetNameOfFocusedControl() != "OptionPanelHeading")
		{
			GUI.FocusControl("OptionPanelHeading");
		}
		_selectedOptionsTab = UnityGUI.Toolbar(new Rect(2f, 31f, _rect.width - 5f, 22f), _selectedOptionsTab, _optionsTabs, _optionsTabs.Length, BlueStonez.tab_medium);
		if (GUI.changed)
		{
			GUI.changed = false;
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.ButtonClick, 0uL, 1f, 1f);
		}
		GUI.BeginGroup(new Rect(16f, 55f, _rect.width - 32f, _rect.height - 56f - 44f), string.Empty, BlueStonez.window_standard_grey38);
		switch (_selectedOptionsTab)
		{
		case 0:
			DoControlsGroup();
			break;
		case 2:
			DoVideoGroup();
			break;
		case 1:
			DoAudioGroup();
			break;
		}
		GUI.EndGroup();
		GUI.enabled = !_showWaterModeMenu;
		if (GUI.Button(new Rect(_rect.width - 136f, _rect.height - 40f, 120f, 32f), new GUIContent(LocalizedStrings.OkCaps), BlueStonez.button))
		{
			ApplicationDataManager.ApplicationOptions.SaveApplicationOptions();
			PanelManager.Instance.ClosePanel(PanelType.Options);
		}
		if (AutoMonoBehaviour<InputManager>.Instance.HasUnassignedKeyMappings)
		{
			GUI.contentColor = Color.red;
			GUI.Label(new Rect(166f, _rect.height - 40f, _rect.width - 136f - 166f, 32f), LocalizedStrings.UnassignedKeyMappingsWarningMsg, BlueStonez.label_interparkmed_11pt);
			GUI.contentColor = Color.white;
		}
		if (_selectedOptionsTab == 0 && !ApplicationDataManager.IsMobile && GUITools.Button(new Rect(16f, _rect.height - 40f, 150f, 32f), new GUIContent(LocalizedStrings.ResetDefaults), BlueStonez.button))
		{
			AutoMonoBehaviour<InputManager>.Instance.Reset();
		}
		else if (_selectedOptionsTab == 2)
		{
			GUI.Label(new Rect(16f, _rect.height - 40f, 150f, 32f), "FPS: " + (1f / Time.smoothDeltaTime).ToString("F1"), BlueStonez.label_interparkbold_16pt_left);
		}
		GUI.enabled = true;
	}

	private void DrawScreenResChangePanel()
	{
		GUI.depth = 1;
		GUI.Label(new Rect(0f, 0f, _rect.width, 56f), LocalizedStrings.ChangingScreenResolution, BlueStonez.tab_strip);
		GUI.BeginGroup(new Rect(16f, 55f, _rect.width - 32f, _rect.height - 56f - 54f), string.Empty, BlueStonez.window_standard_grey38);
		GUI.Label(new Rect(24f, 18f, 460f, 20f), LocalizedStrings.ChooseNewResolution + _screenResText[_newScreenResIndex] + " ?", BlueStonez.label_interparkbold_16pt_left);
		GUI.Label(new Rect(0f, 0f, _rect.width - 32f, _rect.height - 56f - 54f), ((int)_screenResChangeDelay).ToString(), BlueStonez.label_interparkbold_48pt);
		GUI.EndGroup();
		if (GUITools.Button(new Rect(_rect.width - 136f - 140f, _rect.height - 40f, 120f, 32f), new GUIContent(LocalizedStrings.OkCaps), BlueStonez.button))
		{
			ScreenResolutionManager.SetResolution(_newScreenResIndex, true);
			_screenResChangeDelay = 0f;
			GuiLockController.ReleaseLock(GuiDepth.Popup);
		}
		if (GUITools.Button(new Rect(_rect.width - 136f, _rect.height - 40f, 120f, 32f), new GUIContent(LocalizedStrings.CancelCaps), BlueStonez.button))
		{
			_screenResChangeDelay = 0f;
			GuiLockController.ReleaseLock(GuiDepth.Popup);
			if (_isFullscreenBefore)
			{
				ScreenResolutionManager.IsFullScreen = true;
			}
		}
	}

	private void Update()
	{
		if (_screenResChangeDelay > 0f)
		{
			_screenResChangeDelay -= Time.deltaTime;
			if (_screenResChangeDelay <= 0f)
			{
				GuiLockController.ReleaseLock(GuiDepth.Popup);
			}
		}
		if (Input.GetMouseButtonUp(0) && graphicsChanged)
		{
			UpdateTextureQuality();
			UpdateVSyncCount();
			UpdateAntiAliasing();
			UpdatePostProcessing();
			graphicsChanged = false;
		}
	}

	private void SyncGraphicsSettings()
	{
		_currentQuality = QualitySettings.GetQualityLevel();
		_textureQuality = 5 - QualitySettings.globalTextureMipmapLimit;
		switch (QualitySettings.antiAliasing)
		{
		case 2:
			_antiAliasing = 1;
			break;
		case 4:
			_antiAliasing = 2;
			break;
		case 8:
			_antiAliasing = 3;
			break;
		default:
			_antiAliasing = 0;
			break;
		}
		_vsync = QualitySettings.vSyncCount;
	}

	public static bool HorizontalScrollbar(Rect rect, string title, ref float value, float min, float max)
	{
		float num = value;
		GUI.BeginGroup(rect);
		GUI.Label(new Rect(0f, 4f, rect.width, rect.height), title, BlueStonez.label_interparkbold_11pt_left);
		value = GUI.HorizontalSlider(new Rect(150f, 10f, rect.width - 200f, 30f), value, min, max, BlueStonez.horizontalSlider, BlueStonez.horizontalSliderThumb);
		GUI.Label(new Rect(rect.width - 40f, 4f, 50f, rect.height), (!(value < 0f)) ? Mathf.RoundToInt(value).ToString() : LocalizedStrings.Auto, BlueStonez.label_interparkbold_11pt_left);
		GUI.EndGroup();
		return value != num;
	}

	public static bool HorizontalGridbar(Rect rect, string title, ref int value, string[] set)
	{
		int num = value;
		GUI.BeginGroup(rect);
		GUI.Label(new Rect(0f, 5f, rect.width, rect.height), title, BlueStonez.label_interparkbold_11pt_left);
		value = UnityGUI.Toolbar(new Rect(150f, 5f, rect.width - 200f, 30f), value, set, set.Length, BlueStonez.tab_medium);
		GUI.EndGroup();
		return value != num;
	}

	private void DoVideoGroup()
	{
		GUI.skin = BlueStonez.Skin;
		Rect position = new Rect(1f, 1f, _rect.width - 33f, _rect.height - 55f - 47f);
		Rect contentRect = new Rect(0f, 0f, _desiredWidth, _rect.height + 200f - 55f - 46f - 20f);
		int num = 10;
		int num2 = 150;
		int num3 = _screenResText.Length * 16 + 16;
		float width = position.width - 8f - 8f - 20f;
		if (!false || showResolutions)
		{
			contentRect.height += _screenResText.Length * 16;
		}
		_scrollVideo = GUITools.BeginScrollView(position, _scrollVideo, contentRect);
		GUI.enabled = true;
		int num4 = UnityGUI.Toolbar(new Rect(0f, 5f, position.width - 10f, 22f), _currentQuality, qualitySet, qualitySet.Length, BlueStonez.tab_medium);
		if (num4 != _currentQuality)
		{
			SetCurrentQuality(num4);
			AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.ButtonClick, 0uL, 1f, 1f);
		}
		if (HorizontalScrollbar(new Rect(8f, 30f, width, 30f), LocalizedStrings.TextureQuality, ref _textureQuality, 0f, 5f))
		{
			graphicsChanged = true;
			SetCurrentQuality(qualitySet.Length - 1);
		}
		if (HorizontalGridbar(new Rect(8f, 60f, width, 30f), LocalizedStrings.VSync, ref _vsync, vsyncSet))
		{
			graphicsChanged = true;
			SetCurrentQuality(qualitySet.Length - 1);
		}
		if (HorizontalGridbar(new Rect(8f, 90f, width, 30f), LocalizedStrings.AntiAliasing, ref _antiAliasing, antiAliasingSet))
		{
			graphicsChanged = true;
			SetCurrentQuality(qualitySet.Length - 1);
		}
		int num5 = 130;
		if (!ApplicationDataManager.IsMobile)
		{
			_postProcessing = GUI.Toggle(new Rect(8f, num5, width, 30f), ApplicationDataManager.ApplicationOptions.VideoPostProcessing, LocalizedStrings.ShowPostProcessingEffects, BlueStonez.toggle);
			if (_postProcessing != ApplicationDataManager.ApplicationOptions.VideoPostProcessing)
			{
				graphicsChanged = true;
				SetCurrentQuality(qualitySet.Length - 1);
			}
			num5 += 30;
		}
		bool flag = GUI.Toggle(new Rect(8f, num5, width, 30f), ApplicationDataManager.ApplicationOptions.VideoShowFps, LocalizedStrings.ShowFPS, BlueStonez.toggle);
		if (flag != ApplicationDataManager.ApplicationOptions.VideoShowFps)
		{
			ApplicationDataManager.ApplicationOptions.VideoShowFps = flag;
			GameData.Instance.VideoShowFps.Fire();
		}
		num5 += 30;
		if (!false || showResolutions)
		{
			DrawGroupControl(new Rect(8f, num5, width, num3), LocalizedStrings.ScreenResolution, BlueStonez.label_group_interparkbold_18pt);
			GUI.BeginGroup(new Rect(8f, num5, width, num3));
			GUI.changed = false;
			Rect position2 = new Rect(10f, 10f, num + num2 * 2, num3);
			int num6 = GUI.SelectionGrid(position2, ScreenResolutionManager.CurrentResolutionIndex, _screenResText, 1, BlueStonez.radiobutton);
			if (num6 != ScreenResolutionManager.CurrentResolutionIndex)
			{
				if (INSTANT_SCREEN_RES_CHANGE)
				{
					ScreenResolutionManager.SetResolution(num6, Screen.fullScreen);
				}
				else
				{
					ShowScreenResChangeConfirmation(ScreenResolutionManager.CurrentResolutionIndex, num6);
				}
			}
			GUI.EndGroup();
		}
		GUITools.EndScrollView();
	}

	private void DoAudioGroup()
	{
		float num = 130f;
		float width = ((!(_rect.height - 55f - 46f < num)) ? (_rect.width - 50f) : (_rect.width - 65f));
		_scrollControls = GUITools.BeginScrollView(new Rect(1f, 1f, _rect.width - 33f, _rect.height - 55f - 46f), _scrollControls, new Rect(0f, 0f, _rect.width - 50f, num));
		DrawGroupControl(new Rect(8f, 20f, width, 130f), LocalizedStrings.Volume, BlueStonez.label_group_interparkbold_18pt);
		GUI.BeginGroup(new Rect(8f, 20f, width, 130f));
		ApplicationDataManager.ApplicationOptions.AudioEnabled = !GUI.Toggle(new Rect(15f, 105f, 100f, 30f), !ApplicationDataManager.ApplicationOptions.AudioEnabled, LocalizedStrings.Mute, BlueStonez.toggle);
		if (GUI.changed)
		{
			GUI.changed = false;
			AutoMonoBehaviour<SfxManager>.Instance.EnableAudio(ApplicationDataManager.ApplicationOptions.AudioEnabled);
		}
		GUITools.PushGUIState();
		GUI.enabled = ApplicationDataManager.ApplicationOptions.AudioEnabled;
		GUI.Label(new Rect(15f, 10f, 110f, 30f), LocalizedStrings.MasterVolume, BlueStonez.label_interparkbold_11pt_left);
		ApplicationDataManager.ApplicationOptions.AudioMasterVolume = GUI.HorizontalSlider(new Rect(145f, 17f, 200f, 30f), Mathf.Clamp01(ApplicationDataManager.ApplicationOptions.AudioMasterVolume), 0f, 1f, BlueStonez.horizontalSlider, BlueStonez.horizontalSliderThumb);
		if (GUI.changed)
		{
			GUI.changed = false;
			AutoMonoBehaviour<SfxManager>.Instance.UpdateMasterVolume();
		}
		GUI.Label(new Rect(350f, 10f, 100f, 30f), (ApplicationDataManager.ApplicationOptions.AudioMasterVolume * 100f).ToString("f0") + " %", BlueStonez.label_interparkbold_11pt_left);
		GUI.Label(new Rect(15f, 40f, 110f, 30f), LocalizedStrings.MusicVolume, BlueStonez.label_interparkbold_11pt_left);
		ApplicationDataManager.ApplicationOptions.AudioMusicVolume = GUI.HorizontalSlider(new Rect(145f, 47f, 200f, 30f), Mathf.Clamp01(ApplicationDataManager.ApplicationOptions.AudioMusicVolume), 0f, 1f, BlueStonez.horizontalSlider, BlueStonez.horizontalSliderThumb);
		if (GUI.changed)
		{
			GUI.changed = false;
			AutoMonoBehaviour<SfxManager>.Instance.UpdateMusicVolume();
		}
		GUI.Label(new Rect(350f, 40f, 100f, 30f), (ApplicationDataManager.ApplicationOptions.AudioMusicVolume * 100f).ToString("f0") + " %", BlueStonez.label_interparkbold_11pt_left);
		GUI.Label(new Rect(15f, 70f, 110f, 30f), LocalizedStrings.EffectsVolume, BlueStonez.label_interparkbold_11pt_left);
		ApplicationDataManager.ApplicationOptions.AudioEffectsVolume = GUI.HorizontalSlider(new Rect(145f, 77f, 200f, 30f), Mathf.Clamp01(ApplicationDataManager.ApplicationOptions.AudioEffectsVolume), 0f, 1f, BlueStonez.horizontalSlider, BlueStonez.horizontalSliderThumb);
		if (GUI.changed)
		{
			GUI.changed = false;
			AutoMonoBehaviour<SfxManager>.Instance.UpdateEffectsVolume();
		}
		GUI.Label(new Rect(350f, 70f, 100f, 30f), (ApplicationDataManager.ApplicationOptions.AudioEffectsVolume * 100f).ToString("f0") + " %", BlueStonez.label_interparkbold_11pt_left);
		GUITools.PopGUIState();
		GUI.EndGroup();
		GUITools.EndScrollView();
	}

	private void DoControlsGroup()
	{
		GUITools.PushGUIState();
		GUI.enabled = _targetMap == null;
		GUI.skin = BlueStonez.Skin;
		_scrollControls = GUITools.BeginScrollView(new Rect(1f, 3f, _rect.width - 33f, _rect.height - 55f - 50f), _scrollControls, new Rect(0f, 0f, _rect.width - 50f, 210 + _keyCount * 21));
		DrawGroupControl(new Rect(8f, 20f, _rect.width - 65f, 65f), LocalizedStrings.Mouse, BlueStonez.label_group_interparkbold_18pt);
		GUI.BeginGroup(new Rect(8f, 20f, _rect.width - 65f, 65f));
		GUI.Label(new Rect(15f, 10f, 130f, 30f), LocalizedStrings.MouseSensitivity, BlueStonez.label_interparkbold_11pt_left);
		float num = GUI.HorizontalSlider(new Rect(155f, 17f, 200f, 30f), ApplicationDataManager.ApplicationOptions.InputXMouseSensitivity, 1f, 10f, BlueStonez.horizontalSlider, BlueStonez.horizontalSliderThumb);
		GUI.Label(new Rect(370f, 10f, 100f, 30f), ApplicationDataManager.ApplicationOptions.InputXMouseSensitivity.ToString("N1"), BlueStonez.label_interparkbold_11pt_left);
		if (num != ApplicationDataManager.ApplicationOptions.InputXMouseSensitivity)
		{
			ApplicationDataManager.ApplicationOptions.InputXMouseSensitivity = num;
		}
		bool flag = GUI.Toggle(new Rect(15f, 38f, 200f, 30f), ApplicationDataManager.ApplicationOptions.InputInvertMouse, LocalizedStrings.InvertMouseButtons, BlueStonez.toggle);
		if (flag != ApplicationDataManager.ApplicationOptions.InputInvertMouse)
		{
			ApplicationDataManager.ApplicationOptions.InputInvertMouse = flag;
		}
		GUI.EndGroup();
		int num2 = 105;
		if (Input.GetJoystickNames().Length > 0)
		{
			DrawGroupControl(new Rect(8f, 105f, _rect.width - 65f, 50f), LocalizedStrings.Gamepad, BlueStonez.label_group_interparkbold_18pt);
			GUI.BeginGroup(new Rect(8f, 105f, _rect.width - 65f, 50f));
			bool flag2 = GUI.Toggle(new Rect(15f, 15f, 400f, 30f), AutoMonoBehaviour<InputManager>.Instance.IsGamepadEnabled, Input.GetJoystickNames()[0], BlueStonez.toggle);
			if (flag2 != AutoMonoBehaviour<InputManager>.Instance.IsGamepadEnabled)
			{
				AutoMonoBehaviour<InputManager>.Instance.IsGamepadEnabled = flag2;
			}
			GUI.EndGroup();
			num2 += 70;
		}
		else if (AutoMonoBehaviour<InputManager>.Instance.IsGamepadEnabled)
		{
			AutoMonoBehaviour<InputManager>.Instance.IsGamepadEnabled = false;
		}
		DrawGroupControl(new Rect(8f, num2, _rect.width - 65f, _keyCount * 21 + 20), LocalizedStrings.Keyboard, BlueStonez.label_group_interparkbold_18pt);
		GUI.BeginGroup(new Rect(8f, num2, _rect.width - 65f, _keyCount * 21 + 20));
		DoInputControlMapping(new Rect(5f, 5f, _rect.width - 60f, _keyCount * 21 + 20));
		GUI.EndGroup();
		GUITools.EndScrollView();
		GUITools.PopGUIState();
	}

	private void UseMultiTouch()
	{
		ApplicationDataManager.ApplicationOptions.UseMultiTouch = true;
		PanelManager.Instance.OpenPanel(PanelType.Options);
	}

	private void DoInputControlMapping(Rect rect)
	{
		int num = 0;
		GUI.Label(new Rect(20f, 13f, 150f, 20f), LocalizedStrings.Movement, BlueStonez.label_interparkbold_11pt_left);
		GUI.Label(new Rect(220f, 13f, 150f, 20f), LocalizedStrings.KeyButton, BlueStonez.label_interparkbold_11pt_left);
		foreach (UserInputMap value in AutoMonoBehaviour<InputManager>.Instance.KeyMapping.Values)
		{
			bool flag = value == _targetMap;
			GUI.Label(new Rect(20f, 35 + num * 20, 140f, 20f), value.Description, BlueStonez.label_interparkmed_10pt_left);
			if (value.IsConfigurable && GUI.Toggle(new Rect(180f, 35 + num * 20, 20f, 20f), flag, string.Empty, BlueStonez.radiobutton))
			{
				_targetMap = value;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			if (flag)
			{
				GUI.TextField(new Rect(220f, 35 + num * 20, 100f, 20f), string.Empty);
			}
			else
			{
				GUI.contentColor = ((value.Channel == null) ? Color.red : Color.white);
				GUI.Label(new Rect(220f, 35 + num * 20, 150f, 20f), value.Assignment, BlueStonez.label_interparkmed_10pt_left);
				GUI.contentColor = Color.white;
			}
			num++;
		}
		if (_targetMap != null && Event.current.type == EventType.Layout && AutoMonoBehaviour<InputManager>.Instance.ListenForNewKeyAssignment(_targetMap))
		{
			_targetMap = null;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Event.current.Use();
		}
	}

	private void DrawGroupLabel(Rect position, string label, string text)
	{
		GUI.Label(position, label + ": " + text, BlueStonez.label_interparkbold_16pt_left);
	}

	private void DrawContent(Rect position, string label, string text)
	{
		GUI.Label(position, label + ": " + text, BlueStonez.label_interparkbold_11pt_left);
	}

	private void DrawGroupLabelWithWidth(Rect position, string label, string text)
	{
		string text2 = label + ": " + text;
		int num = Mathf.RoundToInt(BlueStonez.label_interparkbold_16pt.CalcSize(new GUIContent(text2)).x);
		GUI.Label(new Rect(position.x, position.y, num, position.height), text2, BlueStonez.label_interparkbold_16pt_left);
		_desiredWidth = ((num <= _desiredWidth) ? _desiredWidth : num);
	}

	private void DrawGroupControl(Rect rect, string title, GUIStyle style)
	{
		GUI.BeginGroup(rect, string.Empty, BlueStonez.group_grey81);
		GUI.EndGroup();
		GUI.Label(new Rect(rect.x + 18f, rect.y - 8f, GetWidth(title, style), 16f), title, style);
	}

	private float GetWidth(string content)
	{
		return GetWidth(content, BlueStonez.label_group_interparkbold_18pt);
	}

	private float GetWidth(string content, GUIStyle style)
	{
		return style.CalcSize(new GUIContent(content)).x + 10f;
	}

	private void ShowScreenResChangeConfirmation(int oldRes, int newRes)
	{
		_screenResChangeDelay = 15f;
		_newScreenResIndex = newRes;
		_isFullscreenBefore = ScreenResolutionManager.IsFullScreen;
		ScreenResolutionManager.IsFullScreen = false;
	}

	private void SetCurrentQuality(int qualityLevel)
	{
		_currentQuality = qualityLevel;
		if (_currentQuality < QualitySettings.names.Length)
		{
			ApplicationDataManager.ApplicationOptions.IsUsingCustom = false;
			GraphicSettings.SetQualityLevel(_currentQuality);
			SyncGraphicsSettings();
		}
		else
		{
			ApplicationDataManager.ApplicationOptions.IsUsingCustom = true;
		}
	}

	private void UpdateTextureQuality()
	{
		_textureQuality = Mathf.RoundToInt(_textureQuality);
		QualitySettings.globalTextureMipmapLimit = 5 - (int)_textureQuality;
		ApplicationDataManager.ApplicationOptions.VideoTextureQuality = QualitySettings.globalTextureMipmapLimit;
	}

	private void UpdateVSyncCount()
	{
		ApplicationDataManager.ApplicationOptions.VideoVSyncCount = _vsync;
		QualitySettings.vSyncCount = _vsync;
	}

	private void UpdateAntiAliasing()
	{
		switch (_antiAliasing)
		{
		case 1:
			QualitySettings.antiAliasing = 2;
			break;
		case 2:
			QualitySettings.antiAliasing = 4;
			break;
		case 3:
			QualitySettings.antiAliasing = 8;
			break;
		default:
			QualitySettings.antiAliasing = 0;
			break;
		}
		ApplicationDataManager.ApplicationOptions.VideoAntiAliasing = QualitySettings.antiAliasing;
	}

	private void UpdatePostProcessing()
	{
		ApplicationDataManager.ApplicationOptions.VideoPostProcessing = _postProcessing;
		RenderSettingsController.Instance.EnableImageEffects();
	}

	public override void Show()
	{
		base.Show();
		if (ApplicationDataManager.ApplicationOptions.IsUsingCustom)
		{
			_currentQuality = qualitySet.Length - 1;
		}
		else
		{
			_currentQuality = ApplicationDataManager.ApplicationOptions.VideoQualityLevel;
		}
	}
}

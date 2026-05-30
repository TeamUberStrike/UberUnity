using UnityEngine;

public class SceneGuiController : MonoBehaviour
{
	[SerializeField]
	private string _title;

	[SerializeField]
	private PageGUI[] _guiPages;

	[SerializeField]
	private float _width;

	[SerializeField]
	private IngamePageType _pageType;

	private Rect _rect;

	private GUIContent[] _guiPageTabs;

	private int _currentGuiPageIndex;

	private FloatAnim _currentWidth;

	public IngamePageType PageType
	{
		get
		{
			return _pageType;
		}
	}

	private void Awake()
	{
		_currentWidth = new FloatAnim(delegate
		{
			AutoMonoBehaviour<CameraRectController>.Instance.SetAbsoluteWidth((float)Screen.width - _currentWidth.Value);
		});
		_guiPageTabs = new GUIContent[_guiPages.Length];
		for (int num = 0; num < _guiPages.Length; num++)
		{
			_guiPageTabs[num] = new GUIContent(_guiPages[num].Title);
		}
	}

	private void OnEnable()
	{
		if (_guiPages.Length > 0)
		{
			SetCurrentPage(0);
			_currentWidth.Value = _width;
		}
		EventHandler.Global.AddListener<GlobalEvents.ScreenResolutionChanged>(OnScreenResolutionChange);
	}

	private void OnDisable()
	{
		if (_guiPages[_currentGuiPageIndex] != null)
		{
			_guiPages[_currentGuiPageIndex].enabled = false;
		}
		_currentWidth.Value = 0f;
		_currentGuiPageIndex = -1;
		EventHandler.Global.RemoveListener<GlobalEvents.ScreenResolutionChanged>(OnScreenResolutionChange);
	}

	private void OnGUI()
	{
		GUI.depth = 11;
		_rect.x = (float)Screen.width - _width;
		_rect.y = GlobalUIRibbon.Instance.Height();
		_rect.width = _width;
		_rect.height = (float)Screen.height - _rect.y;
		GUI.skin = BlueStonez.Skin;
		GUI.BeginGroup(_rect, GUIContent.none, BlueStonez.window_standard_grey38);
		GUI.Label(new Rect(0f, 0f, _rect.width, 56f), _title, BlueStonez.tab_strip);
		GUI.changed = false;
		_currentGuiPageIndex = UnityGUI.Toolbar(new Rect(0f, 34f, 140 * _guiPageTabs.Length, 22f), _currentGuiPageIndex, _guiPageTabs, _guiPageTabs.Length, BlueStonez.tab_medium);
		if (GUI.changed)
		{
			SetCurrentPage(_currentGuiPageIndex);
			return;
		}
		GUI.EndGroup();
		_guiPages[_currentGuiPageIndex].DrawGUI(new Rect(_rect.x, _rect.y + 57f, _rect.width, _rect.height - 56f));
		GuiManager.DrawTooltip();
	}

	private void SetCurrentPage(int index)
	{
		for (int i = 0; i < _guiPages.Length; i++)
		{
			_guiPages[i].IsOnGUIEnabled = false;
			_guiPages[i].enabled = false;
		}
		if (index >= 0 && index < _guiPages.Length)
		{
			_currentGuiPageIndex = index;
			_guiPages[_currentGuiPageIndex].enabled = true;
		}
	}

	private void OnScreenResolutionChange(GlobalEvents.ScreenResolutionChanged ev)
	{
		AutoMonoBehaviour<CameraRectController>.Instance.SetAbsoluteWidth((float)Screen.width - _currentWidth.Value);
	}
}

using System.Collections.Generic;
using UnityEngine;

public class GamePageManager : MonoBehaviour
{
	private static IDictionary<IngamePageType, SceneGuiController> _pageByPageType;

	private static IngamePageType _currentPageType;

	public static GamePageManager Instance { get; private set; }

	public static bool Exists
	{
		get
		{
			return Instance != null;
		}
	}

	public static bool HasPage
	{
		get
		{
			return _currentPageType != IngamePageType.None;
		}
	}

	private void Awake()
	{
		Instance = this;
		_pageByPageType = new Dictionary<IngamePageType, SceneGuiController>();
	}

	private void Start()
	{
		SceneGuiController[] componentsInChildren = GetComponentsInChildren<SceneGuiController>(true);
		foreach (SceneGuiController sceneGuiController in componentsInChildren)
		{
			_pageByPageType[sceneGuiController.PageType] = sceneGuiController;
		}
	}

	public static bool IsCurrentPage(IngamePageType type)
	{
		return _currentPageType == type;
	}

	public SceneGuiController GetCurrentPage()
	{
		SceneGuiController value;
		_pageByPageType.TryGetValue(_currentPageType, out value);
		return value;
	}

	public void UnloadCurrentPage()
	{
		SceneGuiController currentPage = GetCurrentPage();
		if ((bool)currentPage)
		{
			currentPage.gameObject.SetActive(false);
			_currentPageType = IngamePageType.None;
		}
		EventHandler.Global.Fire(new GlobalEvents.GamePageChanged());
	}

	public void LoadPage(IngamePageType pageType)
	{
		if (pageType == _currentPageType)
		{
			return;
		}
		SceneGuiController value = null;
		if (_pageByPageType.TryGetValue(pageType, out value))
		{
			SceneGuiController value2 = null;
			_pageByPageType.TryGetValue(_currentPageType, out value2);
			if ((bool)value2)
			{
				value2.gameObject.SetActive(false);
			}
			_currentPageType = pageType;
			value.gameObject.SetActive(true);
			EventHandler.Global.Fire(new GlobalEvents.GamePageChanged());
		}
	}
}

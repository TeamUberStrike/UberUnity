using UnityEngine;

public class SplashScreen : MonoBehaviour
{
	[SerializeField]
	private Texture2D initializingScreen;

	[SerializeField]
	private Texture2D initializingScreenBlurred;

	private Texture mainScreen;

	private Texture blurScreen;

	private Texture2D loadingBarBackground;

	private float _blurBackgroundAlpha = 1f;

	private Vector2 barSize = new Vector2(300f, 20f);

	private float TotalProgress
	{
		get
		{
			return GlobalSceneLoader.GlobalSceneProgress;
		}
	}

	private void Start()
	{
		loadingBarBackground = new Texture2D(1, 1, TextureFormat.RGB24, false);
		loadingBarBackground.SetPixels(new Color[1] { Color.white });
		loadingBarBackground.Apply(false);
		mainScreen = initializingScreen;
		blurScreen = initializingScreenBlurred;
	}

	private void OnGUI()
	{
		GUI.skin = PopupSkin.Skin;
		GUIStyle label_loading = PopupSkin.label_loading;
		float alpha = (Mathf.Sin(Time.time * 2f) + 1.3f) * 0.5f;
		if (!GlobalSceneLoader.IsError)
		{
			label_loading.normal.textColor = label_loading.normal.textColor.SetAlpha(alpha);
			if (GlobalSceneLoader.IsGlobalSceneLoaded && GlobalSceneLoader.IsItemAssetBundleLoaded)
			{
				_blurBackgroundAlpha = Mathf.Lerp(_blurBackgroundAlpha, 0f, Time.deltaTime);
			}
			float num = (float)Screen.width / (float)mainScreen.width;
			float num2 = (float)Screen.height / (float)mainScreen.height;
			float num3 = ((!(num > num2)) ? num2 : num);
			Rect position = new Rect((float)(Screen.width / 2) - (float)mainScreen.width * num3 / 2f, (float)(Screen.height / 2) - (float)mainScreen.height * num3 / 2f, (float)mainScreen.width * num3, (float)mainScreen.height * num3);
			GUI.depth = 100;
			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.DrawTexture(position, mainScreen);
			GUI.color = new Color(1f, 1f, 1f, 1f - _blurBackgroundAlpha);
			GUI.DrawTexture(position, blurScreen);
			GUI.color = Color.white;
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				label_loading.normal.textColor = label_loading.normal.textColor.SetAlpha(1f);
				GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), "No internet connection available", label_loading);
			}
			else if (!GlobalSceneLoader.IsGlobalSceneLoaded)
			{
				Vector2 v = label_loading.CalcSize(new GUIContent("Loading game. Please wait..."));
				GUITools.LabelShadow(new Rect(0f, Screen.height - 150, Screen.width, v.Height()), "Loading game. Please wait...", label_loading, label_loading.normal.textColor);
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				GUI.DrawTexture(new Rect((float)Screen.width * 0.5f - barSize.x * 0.5f, (float)(Screen.height - 150) + barSize.Height() + 8f, barSize.x, 8f), loadingBarBackground);
				GUI.color = Color.white;
				GUI.DrawTexture(new Rect((float)Screen.width * 0.5f - barSize.x * 0.5f, (float)(Screen.height - 150) + barSize.Height() + 8f, Mathf.RoundToInt(TotalProgress * barSize.x), 8f), loadingBarBackground);
			}
			else if (!GlobalSceneLoader.IsInitialised)
			{
				Vector2 v2 = label_loading.CalcSize(new GUIContent("Connecting..."));
				GUITools.LabelShadow(new Rect(0f, Screen.height - 150, Screen.width, v2.Height()), "Connecting...", label_loading, label_loading.normal.textColor);
			}
		}
		else if (!PopupSystem.IsAnyPopupOpen)
		{
			label_loading.normal.textColor = label_loading.normal.textColor.SetAlpha(1f);
			if (GUI.Button(new Rect(0f, 0f, Screen.width, Screen.height), "There was a problem loading UberStrike. Try reloading or visit support.uberstrike.com if the problem persists.", label_loading))
			{
				Application.Quit();
			}
		}
	}
}

using System;
using UnityEngine;

public class HUDDesktopFPS : MonoBehaviour
{
	[SerializeField]
	private UILabel label;

	private void Start()
	{
		GameData.Instance.VideoShowFps.AddEventAndFire((Action<Tuple>)delegate
		{
			label.enabled = ApplicationDataManager.ApplicationOptions.VideoShowFps;
		}, (MonoBehaviour)this);
	}

	private void OnEnable()
	{
		GameData.Instance.VideoShowFps.Fire();
	}

	private void Update()
	{
		if (label.enabled)
		{
			label.text = ApplicationDataManager.FrameRate;
		}
	}
}

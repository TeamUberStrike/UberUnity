using System.Collections.Generic;
using UnityEngine;

public static class UIAudio
{
	public enum Clips
	{
		AudioVolumn = 0,
		ButtonClick = 1,
		ButtonRollover = 2,
		ClickReady = 3,
		ClickUnready = 4,
		CloseLoadout = 5,
		ClosePanel = 6
	}

	private static Dictionary<Clips, AudioClip> _allClips;

	static UIAudio()
	{
		_allClips = new Dictionary<Clips, AudioClip>();
		AudioClipConfigurator component = ((GameObject)Resources.Load("UIAudio", typeof(GameObject))).GetComponent<AudioClipConfigurator>();
		_allClips[Clips.AudioVolumn] = component.Assets[0];
		_allClips[Clips.ButtonClick] = component.Assets[1];
		_allClips[Clips.ButtonRollover] = component.Assets[2];
		_allClips[Clips.ClickReady] = component.Assets[3];
		_allClips[Clips.ClickUnready] = component.Assets[4];
		_allClips[Clips.CloseLoadout] = component.Assets[5];
		_allClips[Clips.ClosePanel] = component.Assets[6];
	}

	public static AudioClip Get(Clips clip)
	{
		return _allClips[clip];
	}
}

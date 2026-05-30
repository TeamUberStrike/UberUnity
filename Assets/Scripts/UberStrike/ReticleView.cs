using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

public class ReticleView : MonoBehaviour
{
	private List<UISprite> sprites = new List<UISprite>();

	private List<UITweener> tweens = new List<UITweener>();

	private void Awake()
	{
		sprites = new List<UISprite>(base.gameObject.GetComponentsInChildren<UISprite>());
		tweens = new List<UITweener>(GetComponentsInChildren<UITweener>());
	}

	public void Shoot()
	{
		tweens.ForEach(delegate(UITweener el)
		{
			if (el.direction == Direction.Reverse)
			{
				el.Toggle();
			}
			else
			{
				el.Play(true);
			}
		});
	}

	public void SetColor(Color color)
	{
		sprites.ForEach(delegate(UISprite el)
		{
			if ((bool)el)
			{
				el.color = color;
			}
		});
	}
}

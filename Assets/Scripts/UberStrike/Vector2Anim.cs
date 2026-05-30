using System;
using UnityEngine;

public class Vector2Anim
{
	private Vector2 _vec2;

	private Action<Vector2, Vector2> _onVec2Change;

	private bool _isAnimating;

	private Vector2 _animSrc;

	private Vector2 _animDest;

	private float _animTime;

	private float _animStartTime;

	private EaseType _animEaseType;

	public Vector2 Vec2
	{
		get
		{
			return _vec2;
		}
		set
		{
			Vector2 vec = _vec2;
			_vec2 = value;
			if (_onVec2Change != null)
			{
				_onVec2Change(vec, _vec2);
			}
		}
	}

	public bool IsAnimating
	{
		get
		{
			return _isAnimating;
		}
	}

	public Vector2Anim(Action<Vector2, Vector2> onVec2Change = null)
	{
		_isAnimating = false;
		if (onVec2Change != null)
		{
			_onVec2Change = onVec2Change;
		}
	}

	public void Update()
	{
		if (_isAnimating)
		{
			float num = Time.time - _animStartTime;
			if (num <= _animTime)
			{
				float t = Mathf.Clamp01(num * (1f / _animTime));
				Vec2 = Vector2.Lerp(_animSrc, _animDest, Mathfx.Ease(t, _animEaseType));
			}
			else
			{
				Vec2 = _animDest;
				_isAnimating = false;
			}
		}
	}

	public void AnimTo(Vector2 destPosition, float time = 0f, EaseType easeType = EaseType.None, float startDelay = 0f)
	{
		if (time <= 0f)
		{
			Vec2 = destPosition;
			return;
		}
		_isAnimating = true;
		_animSrc = Vec2;
		_animDest = destPosition;
		_animTime = time;
		_animEaseType = easeType;
		_animStartTime = Time.time + startDelay;
	}

	public void AnimBy(Vector2 deltaPosition, float time = 0f, EaseType easeType = EaseType.None)
	{
		Vector2 destPosition = Vec2 + deltaPosition;
		AnimTo(destPosition, time, easeType, 0f);
	}

	public void StopAnim()
	{
		_isAnimating = false;
	}
}

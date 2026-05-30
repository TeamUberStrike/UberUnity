using UnityEngine;

public class ColorAnim
{
	public delegate void OnValueChange(Color oldValue, Color newValue);

	private Color _color;

	private OnValueChange _onColorChange;

	private bool _isAnimating;

	private Color _animSrc;

	private Color _animDest;

	private float _animTime;

	private float _animStartTime;

	private EaseType _animEaseType;

	public Color Color
	{
		get
		{
			return _color;
		}
		set
		{
			Color color = _color;
			_color = value;
			if (_onColorChange != null)
			{
				_onColorChange(color, _color);
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

	public float Alpha
	{
		get
		{
			return _color.a;
		}
		set
		{
			Color color = _color;
			_color.a = value;
			if (_onColorChange != null)
			{
				_onColorChange(color, _color);
			}
		}
	}

	public ColorAnim(OnValueChange onColorChange = null)
	{
		_isAnimating = false;
		if (onColorChange != null)
		{
			_onColorChange = onColorChange;
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
				Color = Color.Lerp(_animSrc, _animDest, Mathfx.Ease(t, _animEaseType));
				Alpha = Color.a;
			}
			else
			{
				Color = _animDest;
				Alpha = Color.a;
				_isAnimating = false;
			}
		}
	}

	public void FadeAlphaTo(float destAlpha, float time = 0f, EaseType easeType = EaseType.None)
	{
		if (time <= 0f)
		{
			Alpha = destAlpha;
			return;
		}
		_isAnimating = true;
		_animSrc = Color;
		_animDest = Color;
		_animDest.a = destAlpha;
		_animTime = time;
		_animEaseType = easeType;
		_animStartTime = Time.time;
	}

	public void FadeAlpha(float deltaAlpha, float time = 0f, EaseType easeType = EaseType.None)
	{
		float destAlpha = Color.a + deltaAlpha;
		FadeAlphaTo(destAlpha, time, easeType);
	}

	public void FadeColorTo(Color destColor, float time = 0f, EaseType easeType = EaseType.None)
	{
		if (time <= 0f)
		{
			Color = destColor;
			return;
		}
		_isAnimating = true;
		_animSrc = Color;
		_animDest = destColor;
		_animTime = time;
		_animEaseType = easeType;
		_animStartTime = Time.time;
	}

	public void FadeColor(Color deltaColor, float time = 0f, EaseType easeType = EaseType.None)
	{
		Color destColor = Color + deltaColor;
		FadeColorTo(destColor, time, easeType);
	}

	public void StopFading()
	{
		_isAnimating = false;
	}
}

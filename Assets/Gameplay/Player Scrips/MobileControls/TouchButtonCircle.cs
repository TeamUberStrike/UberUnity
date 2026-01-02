// using UnityEngine;

// public class TouchButtonCircle : TouchButton
// {
// 	private Vector2 _centerPosition;

// 	public Rect QuadRect;

// 	public Rect QuadRectActive;

// 	public bool ShowEffect = true;

// 	public float EffectTime = 0.25f;

// 	private float sqrRadius;

// 	private float initialScale;

// 	private bool _showHighlight;

// 	private MeshGUIQuad _quad;

// 	private float _timer;

// 	public Vector2 CenterPosition
// 	{
// 		get
// 		{
// 			return _centerPosition;
// 		}
// 		set
// 		{
// 			if (value != _centerPosition)
// 			{
// 				if (Quad != null)
// 				{
// 					float num = Quad.Width / 2f;
// 					Boundary = new Rect(value.x - num, value.y - num, Quad.Width, Quad.Width);
// 					num += 5f;
// 					sqrRadius = num * num;
// 					Quad.Position = new Vector2(Boundary.x, Boundary.y);
// 				}
// 				_centerPosition = value;
// 			}
// 		}
// 	}

// 	public override bool Enabled
// 	{
// 		get
// 		{
// 			return base.Enabled;
// 		}
// 		set
// 		{
// 			base.Enabled = value;
// 			if (!base.Enabled)
// 			{
// 				ShowHighlight = false;
// 				Quad.UpdateUVOffset(QuadRect);
// 				if (_quad != null)
// 				{
// 					_quad.FreeObject();
// 					_quad = null;
// 				}
// 			}
// 		}
// 	}

// 	public bool ShowHighlight
// 	{
// 		get
// 		{
// 			return _showHighlight;
// 		}
// 		set
// 		{
// 			_showHighlight = value;
// 			initialScale = Quad.Width / (float)ConsumableHudTextures.CircleWhite.width;
// 			if (_showHighlight)
// 			{
// 				Quad.UpdateUVOffset(QuadRectActive);
// 			}
// 			else
// 			{
// 				Quad.UpdateUVOffset(QuadRect);
// 			}
// 		}
// 	}

// 	public TouchButtonCircle(Rect rect)
// 		: base(rect)
// 	{
// 		QuadRect = rect;
// 	}

// 	public TouchButtonCircle(Rect rect, Rect activeRect)
// 		: base(rect)
// 	{
// 		QuadRect = rect;
// 		QuadRectActive = activeRect;
// 	}

// 	public override void FinalUpdate()
// 	{
// 		base.FinalUpdate();
// 		if (!ShowEffect)
// 		{
// 			return;
// 		}
// 		if (_quad == null && finger.FingerId != -1)
// 		{
// 			_quad = new MeshGUIQuad(ConsumableHudTextures.CircleWhite, TextAnchor.MiddleCenter);
// 			_quad.Position = CenterPosition;
// 			_quad.Scale = new Vector2(initialScale, initialScale);
// 			_quad.SetVisible(true);
// 			_timer = 0f;
// 		}
// 		if (_quad != null)
// 		{
// 			_quad.Scale = new Vector2((_timer / EffectTime + 1f) * initialScale, (_timer / EffectTime + 1f) * initialScale);
// 			_quad.Alpha = 1f - _timer / EffectTime;
// 			_timer += Time.deltaTime;
// 		}
// 		if (_timer > EffectTime)
// 		{
// 			_timer = 0f;
// 			if (finger.FingerId == -1)
// 			{
// 				_quad.FreeObject();
// 				_quad = null;
// 			}
// 		}
// 	}

// 	protected override bool TouchInside(Vector2 position)
// 	{
// 		Vector2 vector = new Vector2(Boundary.x + Boundary.width / 2f, Boundary.y + Boundary.height / 2f);
// 		vector.y = (float)Screen.height - vector.y;
// 		return (vector - position).sqrMagnitude < sqrRadius;
// 	}
// }

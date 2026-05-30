using UnityEngine;

public class GuiText : MonoBehaviour
{
	[SerializeField]
	private Font _font;

	[SerializeField]
	private string _text;

	[SerializeField]
	private Color _color;

	[SerializeField]
	private Vector3 _offset;

	[SerializeField]
	private Transform _target;

	[SerializeField]
	private bool _hasTimeLimit;

	[SerializeField]
	private float _distanceCap = -1f;

	private UnityEngine.UI.Text _guiText;

	private Transform _transform;

	private Material _material;

	private float _visibleTime;

	private bool _isVisible = true;

	public bool IsTextVisible
	{
		get
		{
			return _isVisible;
		}
		set
		{
			if (_isVisible != value)
			{
				_isVisible = value;
				_guiText.enabled = value;
			}
		}
	}

	private void Awake()
	{
		_transform = base.transform;
	}

	private void Start()
	{
		_guiText = base.gameObject.AddComponent(typeof(UnityEngine.UI.Text)) as UnityEngine.UI.Text;
		_guiText.alignment = TextAnchor.MiddleCenter;
//		_guiText.anchor = TextAnchor.MiddleCenter;
		_guiText.font = _font;
		_guiText.text = _text;
		_guiText.material = _font.material;
		_material = _guiText.material;
	}

	private void LateUpdate()
	{
		if (!(Camera.main != null) || !_isVisible)
		{
			return;
		}
		Vector3 position = Camera.main.WorldToViewportPoint(_target.localPosition + _offset);
		_transform.position = position;
		if (_hasTimeLimit)
		{
			_visibleTime -= Time.deltaTime;
			if (_visibleTime > 0f)
			{
				_color.a = _visibleTime;
				_material.color = _color;
			}
			else
			{
				_guiText.enabled = false;
			}
		}
		else
		{
			if (_distanceCap > 0f)
			{
				float a = 1f - Mathf.Clamp01(position.z / _distanceCap);
				_color.a = a;
			}
			_material.color = _color;
		}
	}

	public void ShowText(int seconds)
	{
		_visibleTime = seconds;
	}

	public void ShowText()
	{
		ShowText(5);
	}
}

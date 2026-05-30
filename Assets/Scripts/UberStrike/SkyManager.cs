using UnityEngine;

public class SkyManager : MonoBehaviour
{
	[SerializeField]
	private float _dayNightCycle;

	[SerializeField]
	private float _sunsetOffset;

	[SerializeField]
	private float _sunsetVisibility;

	[SerializeField]
	private Color _daySkyColor;

	[SerializeField]
	private Color _horizonColor;

	[SerializeField]
	private Color _sunsetColor;

	private Vector2 _dayCloudMoveVector = new Vector2(0f, 0f);

	private Vector2 _dayCloudHorizonMoveVector = new Vector2(0f, 0f);

	private float _cloudXAxisRot = 0.005f;

	private float _cloudYAxisRot = 0.005f;

	private float _cloudXAxisRotIndex = 0.001f;

	private float _cloudYAxisRotIndex = 0.001f;

	private Material _skyMaterial;

	public float DayNightCycle
	{
		get
		{
			return _dayNightCycle;
		}
		set
		{
			_dayNightCycle = value;
		}
	}

	public float CloudXAxisRot
	{
		get
		{
			return _cloudXAxisRot;
		}
		set
		{
			_cloudXAxisRot = value;
		}
	}

	public float CloudYAxisRot
	{
		get
		{
			return _cloudYAxisRot;
		}
		set
		{
			_cloudYAxisRot = value;
		}
	}

	private void OnEnable()
	{
		_skyMaterial = new Material(base.GetComponent<Renderer>().material);
	}

	private void OnDisable()
	{
		base.GetComponent<Renderer>().material = _skyMaterial;
	}

	private void Update()
	{
		_dayCloudMoveVector.x += Time.deltaTime * _cloudXAxisRot;
		_dayCloudHorizonMoveVector.y += Time.deltaTime * _cloudYAxisRot;
		if (_dayCloudMoveVector.x > 1f)
		{
			_dayCloudMoveVector.x = 0f;
			if (_cloudXAxisRot > 0.008f)
			{
				_cloudXAxisRotIndex = -0.001f;
			}
			if (_cloudXAxisRot < 0.002f)
			{
				_cloudXAxisRotIndex = 0.001f;
			}
			_cloudXAxisRot += _cloudXAxisRotIndex;
		}
		if (_dayCloudHorizonMoveVector.y > 1f)
		{
			_dayCloudHorizonMoveVector.y = 0f;
			if (_cloudYAxisRot > 0.008f)
			{
				_cloudYAxisRotIndex = -0.001f;
			}
			if (_cloudYAxisRot < 0.002f)
			{
				_cloudYAxisRotIndex = 0.001f;
			}
			_cloudYAxisRot += _cloudYAxisRotIndex;
		}
		base.GetComponent<Renderer>().material.SetTextureOffset("_DayCloudTex", _dayCloudMoveVector);
		base.GetComponent<Renderer>().material.SetTextureOffset("_NightCloudTex", _dayCloudHorizonMoveVector);
		_dayNightCycle = Mathf.Clamp01(_dayNightCycle);
		base.GetComponent<Renderer>().material.SetFloat("_DayNightCycle", Mathf.Clamp01(_dayNightCycle));
		_sunsetOffset = Mathf.Clamp01(_sunsetOffset);
		base.GetComponent<Renderer>().material.SetFloat("_SunsetOffset", Mathf.Clamp01(_sunsetOffset));
		_sunsetVisibility = Mathf.Clamp01(_sunsetVisibility);
		base.GetComponent<Renderer>().material.SetFloat("_SunsetVisibility", _sunsetVisibility);
		base.GetComponent<Renderer>().material.SetColor("_HorizonColor", _horizonColor);
		base.GetComponent<Renderer>().material.SetColor("_DaySkyColor", _daySkyColor);
		base.GetComponent<Renderer>().material.SetColor("_SunSetColor", _sunsetColor);
	}
}

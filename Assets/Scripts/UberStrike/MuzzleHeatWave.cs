using System;
using UnityEngine;

public class MuzzleHeatWave : BaseWeaponEffect
{
	[SerializeField]
	private float _startSize;

	[SerializeField]
	private float _maxSize = 0.05f;

	[SerializeField]
	private float _duration = 0.25f;

	[SerializeField]
	private float _distortion = 64f;

	private Transform _transform;

	private Renderer _renderer;

	private float _elapsedTime;

	private float _normalizedTime;

	private float _s;

	private void Awake()
	{
		_transform = base.transform;
		_renderer = base.GetComponent<Renderer>();
		if (_renderer == null)
		{
			throw new Exception("No Renderer attached to HeatWave script on GameObject " + base.gameObject.name);
		}
	}

	private void Start()
	{
		_renderer.enabled = false;
		base.enabled = false;
	}

	private void Update()
	{
		if ((bool)_transform && (bool)_renderer)
		{
			_elapsedTime += Time.deltaTime;
			_normalizedTime = _elapsedTime / _duration;
			_s = Mathf.Lerp(_startSize, _maxSize, _normalizedTime);
			_renderer.material.SetFloat("_BumpAmt", (1f - _normalizedTime) * _distortion);
			_transform.localScale = new Vector3(_s, _s, _s);
			_transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - _transform.position);
			if (_elapsedTime > _duration)
			{
				_transform.localScale = new Vector3(0f, 0f, 0f);
				_renderer.enabled = false;
				base.enabled = false;
			}
		}
	}

	public override void OnShoot()
	{
		if (SystemInfo.supportsImageEffects)
		{
			_elapsedTime = 0f;
			_transform.rotation = Quaternion.FromToRotation(Vector3.up, Camera.main.transform.position - _transform.position);
			_renderer.enabled = true;
			base.enabled = true;
		}
	}

	public override void OnPostShoot()
	{
	}

	public override void Hide()
	{
		if (!_renderer)
		{
			_renderer = base.GetComponent<Renderer>();
		}
		if ((bool)_renderer)
		{
			_renderer.enabled = false;
		}
	}

	public override void OnHits(RaycastHit[] hits)
	{
	}
}

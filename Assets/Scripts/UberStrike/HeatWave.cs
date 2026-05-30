using System;
using UnityEngine;

public class HeatWave : MonoBehaviour
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

	private void Update()
	{
		if ((bool)_transform && (bool)_renderer)
		{
			_elapsedTime += Time.deltaTime;
			_normalizedTime = _elapsedTime / _duration;
			_s = Mathf.Lerp(_startSize, _maxSize, _normalizedTime);
			if ((bool)_renderer.material)
			{
				_renderer.material.SetFloat("_BumpAmt", (1f - _normalizedTime) * _distortion);
			}
			_transform.localScale = new Vector3(_s, _s, _s);
			if ((bool)Camera.main)
			{
				_transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - _transform.position);
			}
			if (_elapsedTime > _duration && (bool)base.gameObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}

using UnityEngine;

public class MoveTrailrendererObject : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private float _duration = 0.1f;

	private float _locationOnPath;

	private bool _move;

	private float _timeToArrive = 1f;

	private float _alpha = 1f;

	public void MoveTrail(Vector3 destination, Vector3 muzzlePosition, float distance)
	{
		if (_lineRenderer != null)
		{
			_alpha = 1f;
			_move = true;
			_lineRenderer.SetPosition(0, muzzlePosition);
			_lineRenderer.SetPosition(1, destination);
			_timeToArrive = Time.time + _duration;
		}
	}

	private void Update()
	{
		if (_move)
		{
			_locationOnPath = 1f - (_timeToArrive - Time.time);
			_alpha = Mathf.Lerp(_alpha, 0f, _locationOnPath);
			Color color = _lineRenderer.material.GetColor("_TintColor");
			color.a = _alpha;
			_lineRenderer.materials[0].SetColor("_TintColor", color);
			if (Time.time >= _timeToArrive)
			{
				_move = false;
				Object.Destroy(base.gameObject);
			}
		}
	}
}

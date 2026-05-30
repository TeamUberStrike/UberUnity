using UnityEngine;

public class CameraCollisionDetector
{
	private bool _lHitResult;

	private bool _rHitResult;

	private RaycastHit _lRaycastInfo;

	private RaycastHit _rRaycastInfo;

	private float _collidedDistance;

	public float Offset;

	public int LayerMask;

	public float Distance
	{
		get
		{
			return _collidedDistance;
		}
	}

	public bool Detect(Vector3 from, Vector3 to, Vector3 right)
	{
		_collidedDistance = Vector3.Distance(from, to);
		if ((Time.frameCount & 1) == 0)
		{
			to -= right * Offset;
			_lHitResult = Physics.Linecast(from, to, out _lRaycastInfo, LayerMask);
		}
		else
		{
			to += right * Offset;
			_rHitResult = Physics.Linecast(from, to, out _rRaycastInfo, LayerMask);
		}
		if (_lHitResult)
		{
			float num = Vector3.Distance(_lRaycastInfo.point, from);
			if (num < _collidedDistance)
			{
				_collidedDistance = num;
			}
		}
		if (_rHitResult)
		{
			float num2 = Vector3.Distance(_rRaycastInfo.point, from);
			if (num2 < _collidedDistance)
			{
				_collidedDistance = num2;
			}
		}
		return _lHitResult || _rHitResult;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		if (_lHitResult)
		{
			Gizmos.DrawWireSphere(_lRaycastInfo.point, 0.1f);
		}
		if (_rHitResult)
		{
			Gizmos.DrawWireSphere(_rRaycastInfo.point, 0.1f);
		}
	}
}

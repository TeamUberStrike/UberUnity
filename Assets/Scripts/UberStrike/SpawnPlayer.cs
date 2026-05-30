using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnPlayer : MonoBehaviour
{
	[SerializeField]
	private float _amplitude = 20f;

	[SerializeField]
	private float _velocity = 0.1f;

	[SerializeField]
	private Vector3 _startPosition;

	private Vector3 _currentPosition;

	[SerializeField]
	private Transform _spawnTo;

	private Transform _transform;

	private float _nextTeleport;

	private void Awake()
	{
		_transform = base.transform;
		_startPosition = (_currentPosition = _transform.localPosition);
	}

	private void Update()
	{
		_currentPosition.y = _startPosition.y + Mathf.Sin(Time.time * _velocity) * _amplitude;
		_transform.localPosition = _currentPosition;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player" && _nextTeleport < Time.time)
		{
			_nextTeleport = Time.time + 5f;
			GameState.Current.Player.SpawnPlayerAt(_spawnTo.position, _spawnTo.rotation);
		}
	}
}

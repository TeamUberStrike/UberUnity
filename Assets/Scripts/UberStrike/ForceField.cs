using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ForceField : MonoBehaviour
{
	[SerializeField]
	private Vector3 _direction;

	[SerializeField]
	private int _force = 1000;

	private float gizmofactor = 0.0055f;

	private void Awake()
	{
		base.GetComponent<Collider>().isTrigger = true;
		base.gameObject.layer = 2;
	}

	private const float Modifier = 0.035f;  // Original: LevelEnviroment.Modifier

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Player")
		{
			// Original: _currentVelocity = (_direction.normalized * _force) * 0.035 (ForceType.Exclusive)
			// Measured: force=1935, dir=(0.16,23,-5.23) → scaled vel ≈ (0.45, 66, -15) ✓
			Vector3 finalVelocity = _direction.normalized * _force * Modifier;
			collider.gameObject.SendMessage("PowerUp", finalVelocity, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(base.transform.localPosition, 0.2f);
		Vector3 normalized = _direction.normalized;
		normalized.y *= 0.6f;
		Gizmos.DrawLine(base.transform.localPosition, base.transform.localPosition + normalized * Mathf.Log(_force) * _force * gizmofactor);
	}
}

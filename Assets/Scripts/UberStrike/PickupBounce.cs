using UnityEngine;

public class PickupBounce : MonoBehaviour
{
	private float origPosY;

	private float startOffset;

	private void Awake()
	{
		origPosY = base.transform.position.y;
		startOffset = Random.value * 3f;
	}

	private void FixedUpdate()
	{
		base.transform.Rotate(new Vector3(0f, 2f, 0f));
		base.transform.position = new Vector3(base.transform.position.x, origPosY + Mathf.Sin((startOffset + Time.realtimeSinceStartup) * 4f) * 0.08f, base.transform.position.z);
	}
}

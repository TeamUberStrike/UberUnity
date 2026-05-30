using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	private Transform player;

	private Vector3 lastPosition;

	private void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player")
		{
			lastPosition = base.transform.position;
			player = c.transform;
		}
	}

	private void OnTriggerExit(Collider c)
	{
		if (c.tag == "Player")
		{
			player = null;
		}
	}

	private void LateUpdate()
	{
		if ((bool)player)
		{
			player.localPosition += base.transform.position - lastPosition;
			lastPosition = base.transform.position;
		}
	}
}

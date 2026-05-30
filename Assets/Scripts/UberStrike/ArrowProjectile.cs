using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	public void Destroy(int timeDelay)
	{
		Object.Destroy(base.gameObject, timeDelay);
	}

	public void SetParent(Transform parent)
	{
		base.transform.parent = parent;
	}
}

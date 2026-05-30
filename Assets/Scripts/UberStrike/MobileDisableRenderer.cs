using UnityEngine;

public class MobileDisableRenderer : MonoBehaviour
{
	private void OnEnable()
	{
		if (ApplicationDataManager.IsMobile)
		{
			base.GetComponent<Renderer>().enabled = false;
		}
	}
}

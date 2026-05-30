using UnityEngine;

public class MobileShadowReceiver : MonoBehaviour
{
	public void OnWillRenderObject()
	{
		Camera camera = null;
		for (int i = 0; i < Camera.allCameras.Length; i++)
		{
			if (Camera.allCameras[i].name == "Shadow Camera")
			{
				camera = Camera.allCameras[i];
				break;
			}
		}
		if (camera != null)
		{
			Matrix4x4 matrix = camera.projectionMatrix * camera.worldToCameraMatrix * base.GetComponent<Renderer>().localToWorldMatrix;
			base.GetComponent<Renderer>().material.SetMatrix("_LocalToShadowMatrix", matrix);
		}
	}
}

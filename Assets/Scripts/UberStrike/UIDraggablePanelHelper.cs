using UnityEngine;

public static class UIDraggablePanelHelper
{
	public static void SpringToSelection(this UIDraggablePanel dragPanel, GameObject selectedObject, float springStrength)
	{
		dragPanel.SpringToPosition(selectedObject.transform.position, springStrength);
	}

	public static void SpringToSelection(this UIDraggablePanel dragPanel, Vector3 selectedPosition, float springStrength)
	{
		dragPanel.SpringToPosition(selectedPosition, springStrength);
	}

	private static void SpringToPosition(this UIDraggablePanel dragPanel, Vector3 positionToSpring, float springStrength)
	{
		Vector4 clipRange = dragPanel.panel.clipRange;
		Transform cachedTransform = dragPanel.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		dragPanel.currentMomentum = Vector3.zero;
		Vector3 vector = cachedTransform.InverseTransformPoint(positionToSpring);
		Vector3 vector2 = cachedTransform.InverseTransformPoint(localPosition);
		Vector3 vector3 = vector - vector2;
		if (dragPanel.scale.x == 0f)
		{
			vector3.x = 0f;
		}
		if (dragPanel.scale.y == 0f)
		{
			vector3.y = 0f;
		}
		if (dragPanel.scale.z == 0f)
		{
			vector3.z = 0f;
		}
		SpringPanel.Begin(dragPanel.gameObject, cachedTransform.localPosition - vector3, springStrength);
	}
}

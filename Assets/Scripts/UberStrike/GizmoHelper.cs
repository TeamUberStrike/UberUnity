using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GizmoHelper : AutoMonoBehaviour<GizmoHelper>
{
	public enum GizmoType
	{
		Cube = 0,
		Sphere = 1,
		WiredSphere = 2
	}

	[Serializable]
	public class Gizmo
	{
		public GizmoType Type;

		public Vector3 Position;

		public float Size;

		public Color Color;
	}

	[SerializeField]
	private List<Gizmo> gizmos = new List<Gizmo>();

	public Gizmo CollisionTest = new Gizmo
	{
		Color = Color.red,
		Type = GizmoType.Sphere
	};

	public void AddGizmo(Vector3 position, [Optional] Color color, GizmoType type = GizmoType.Sphere, float size = 0.1f)
	{
		Debug.Log("AddGizmo " + position);
		gizmos.Add(new Gizmo
		{
			Position = position,
			Type = type,
			Size = size,
			Color = color
		});
	}

	private void OnDrawGizmos()
	{
		if (CollisionTest.Size > 0f)
		{
			Gizmos.color = CollisionTest.Color;
			Gizmos.DrawSphere(CollisionTest.Position, CollisionTest.Size);
		}
		foreach (Gizmo gizmo in gizmos)
		{
			Gizmos.color = gizmo.Color;
			switch (gizmo.Type)
			{
			case GizmoType.Cube:
				Gizmos.DrawCube(gizmo.Position, Vector3.one * gizmo.Size);
				break;
			case GizmoType.Sphere:
				Gizmos.DrawSphere(gizmo.Position, gizmo.Size);
				break;
			default:
				Gizmos.DrawSphere(gizmo.Position, gizmo.Size);
				break;
			}
		}
		Gizmos.color = Color.white;
	}
}

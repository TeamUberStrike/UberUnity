using System;
using System.Collections;
using UnityEngine;

public class UnityRuntime : AutoMonoBehaviour<UnityRuntime>
{
	[SerializeField]
	private bool showInvocationList;

	public event Action OnGui;

	public event Action OnUpdate;

	public event Action OnLateUpdate;

	public event Action OnFixedUpdate;

	public event Action OnDrawGizmo;

	public event Action<bool> OnAppFocus;

	private void FixedUpdate()
	{
		if (this.OnFixedUpdate != null)
		{
			this.OnFixedUpdate();
		}
	}

	private void Update()
	{
		if (this.OnUpdate != null)
		{
			this.OnUpdate();
		}
	}

	private void LateUpdate()
	{
		if (this.OnLateUpdate != null)
		{
			this.OnLateUpdate();
		}
	}

	private void OnGUI()
	{
		if (this.OnGui != null)
		{
			this.OnGui();
		}
		if (!showInvocationList)
		{
			return;
		}
		GUILayout.BeginArea(new Rect(10f, 100f, 400f, Screen.height - 200));
		if (this.OnUpdate != null)
		{
			Delegate[] invocationList = this.OnUpdate.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				GUILayout.Label("Update: " + obj.Method.DeclaringType.Name + "." + obj.Method.Name);
			}
		}
		if (this.OnFixedUpdate != null)
		{
			Delegate[] invocationList2 = this.OnFixedUpdate.GetInvocationList();
			foreach (Delegate obj2 in invocationList2)
			{
				GUILayout.Label("FixedUpdate: " + obj2.Method.DeclaringType.Name + "." + obj2.Method.Name);
			}
		}
		if (this.OnAppFocus != null)
		{
			Delegate[] invocationList3 = this.OnAppFocus.GetInvocationList();
			foreach (Delegate obj3 in invocationList3)
			{
				GUILayout.Label("OnApplicationFocus: " + obj3.Method.DeclaringType.Name + "." + obj3.Method.Name);
			}
		}
		GUILayout.EndArea();
	}

	private void OnApplicationFocus(bool focus)
	{
		if (this.OnAppFocus != null)
		{
			this.OnAppFocus(focus);
		}
	}

	public static Coroutine StartRoutine(IEnumerator routine)
	{
		if (AutoMonoBehaviour<UnityRuntime>.IsRunning)
		{
			return AutoMonoBehaviour<UnityRuntime>.Instance.StartCoroutine(routine);
		}
		return null;
	}
}

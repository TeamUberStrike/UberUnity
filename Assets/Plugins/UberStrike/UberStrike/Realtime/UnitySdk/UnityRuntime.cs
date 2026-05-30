using System;
using UnityEngine;

namespace UberStrike.Realtime.UnitySdk
{
	internal class UnityRuntime : MonoBehaviour
	{
		[SerializeField]
		private bool showInvocationList;

		private static UnityRuntime instance;

		private Action onFixedUpdate;

		private Action onUpdate;

		private Action onShutdown;

		public static UnityRuntime Instance
		{
			get
			{
				if (instance == null)
				{
					GameObject gameObject = GameObject.Find("AutoMonoBehaviours");
					if (gameObject == null)
					{
						gameObject = new GameObject("AutoMonoBehaviours");
					}
					instance = gameObject.AddComponent<UnityRuntime>();
				}
				return instance;
			}
		}

		public event Action OnFixedUpdate
		{
			add
			{
				onFixedUpdate = (Action)Delegate.Combine(onFixedUpdate, value);
			}
			remove
			{
				onFixedUpdate = (Action)Delegate.Remove(onFixedUpdate, value);
			}
		}

		public event Action OnUpdate
		{
			add
			{
				onUpdate = (Action)Delegate.Combine(onUpdate, value);
			}
			remove
			{
				onUpdate = (Action)Delegate.Remove(onUpdate, value);
			}
		}

		public event Action OnShutdown
		{
			add
			{
				onShutdown = (Action)Delegate.Combine(onShutdown, value);
			}
			remove
			{
				onShutdown = (Action)Delegate.Remove(onShutdown, value);
			}
		}

		private void OnGUI()
		{
			if (!showInvocationList)
			{
				return;
			}
			GUILayout.BeginArea(new Rect(10f, 100f, 400f, Screen.height - 200));
			if (onUpdate != null)
			{
				Delegate[] invocationList = onUpdate.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
					GUILayout.Label("Update: " + obj.Method.DeclaringType.Name + "." + obj.Method.Name);
				}
			}
			if (onFixedUpdate != null)
			{
				Delegate[] invocationList2 = onFixedUpdate.GetInvocationList();
				foreach (Delegate obj2 in invocationList2)
				{
					GUILayout.Label("FixedUpdate: " + obj2.Method.DeclaringType.Name + "." + obj2.Method.Name);
				}
			}
			if (onShutdown != null)
			{
				Delegate[] invocationList3 = onShutdown.GetInvocationList();
				foreach (Delegate obj3 in invocationList3)
				{
					GUILayout.Label("OnApplicationQuit: " + obj3.Method.DeclaringType.Name + "." + obj3.Method.Name);
				}
			}
			GUILayout.EndArea();
		}

		private void Update()
		{
			if (onUpdate != null)
			{
				onUpdate();
			}
		}

		private void FixedUpdate()
		{
			if (onFixedUpdate != null)
			{
				onFixedUpdate();
			}
		}

		private void OnApplicationQuit()
		{
			if (onShutdown != null)
			{
				onShutdown();
			}
		}
	}
}

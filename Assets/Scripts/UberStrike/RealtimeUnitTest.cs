using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UberStrike.Realtime.Client;
using UnityEngine;

public class RealtimeUnitTest : AutoMonoBehaviour<RealtimeUnitTest>
{
	private class RemoteCall
	{
		public object target { get; private set; }

		public MethodInfo method { get; private set; }

		public object[] arguments { get; private set; }

		public string debug { get; private set; }

		public RemoteCall(object target, MethodInfo method)
		{
			this.target = target;
			this.method = method;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(method.Name).Append("(");
			List<object> list = new List<object>();
			ParameterInfo[] parameters = method.GetParameters();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				list.Add(CreateArgument(parameterInfo.ParameterType));
				stringBuilder.Append(parameterInfo.ParameterType.Name).Append(":").Append(CreateArgument(parameterInfo.ParameterType))
					.Append(" ");
			}
			stringBuilder.Append(")");
			arguments = list.ToArray();
			debug = stringBuilder.ToString();
		}

		private static object CreateArgument(Type t)
		{
			if (t.IsGenericType)
			{
				return Activator.CreateInstance(t);
			}
			if (t == typeof(string))
			{
				return "asdf";
			}
			if (t.IsClass)
			{
				return null;
			}
			return Activator.CreateInstance(t);
		}
	}

	private Dictionary<string, List<RemoteCall>> methodCalls;

	private int index;

	private Vector2 scroll;

	private RealtimeUnitTest()
	{
		methodCalls = new Dictionary<string, List<RemoteCall>>();
	}

	private void OnGUI()
	{
		string[] array = methodCalls.KeyArray();
		GUILayout.BeginArea(new Rect(0f, 100f, Screen.width, Screen.height - 100));
		index = GUILayout.SelectionGrid(index, array, array.Length);
		if (index < array.Length)
		{
			scroll = GUILayout.BeginScrollView(scroll);
			List<RemoteCall> list = methodCalls[array[index]];
			foreach (RemoteCall item in list)
			{
				if (GUILayout.Button(item.debug))
				{
					item.method.Invoke(item.target, item.arguments);
				}
			}
			GUILayout.EndScrollView();
		}
		GUILayout.EndArea();
	}

	public void Add(IOperationSender target)
	{
		List<RemoteCall> value;
		if (!methodCalls.TryGetValue(target.GetType().Name, out value))
		{
			value = new List<RemoteCall>();
			methodCalls[target.GetType().Name] = value;
		}
		MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			if (methodInfo.Name.StartsWith("Send"))
			{
				value.Add(new RemoteCall(target, methodInfo));
			}
		}
	}

	public void Add(IEventDispatcher target)
	{
		List<RemoteCall> value;
		if (!methodCalls.TryGetValue(target.GetType().Name, out value))
		{
			value = new List<RemoteCall>();
			methodCalls[target.GetType().Name] = value;
		}
		MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			if (methodInfo.Name.StartsWith("On"))
			{
				value.Add(new RemoteCall(target, methodInfo));
			}
		}
	}
}

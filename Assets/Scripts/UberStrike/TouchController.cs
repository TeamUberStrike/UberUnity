using System.Collections.Generic;
using UnityEngine;

public class TouchController : Singleton<TouchController>
{
	public float GUIAlpha = 1f;

	private List<TouchBaseControl> _controls;

	private TouchController()
	{
		_controls = new List<TouchBaseControl>();
		AutoMonoBehaviour<UnityRuntime>.Instance.OnUpdate += OnUpdate;
		AutoMonoBehaviour<UnityRuntime>.Instance.OnGui += OnGui;
	}

	private void OnUpdate()
	{
		foreach (TouchBaseControl control in _controls)
		{
			if (control.Enabled)
			{
				control.FirstUpdate();
				Touch[] touches = Input.touches;
				foreach (Touch touch in touches)
				{
					control.UpdateTouches(touch);
				}
				control.FinalUpdate();
			}
		}
	}

	private void OnGui()
	{
		foreach (TouchBaseControl control in _controls)
		{
			if (control.Enabled)
			{
				control.Draw();
			}
		}
	}

	public void AddControl(TouchBaseControl control)
	{
		_controls.Add(control);
	}

	public void RemoveControl(TouchBaseControl control)
	{
		_controls.Remove(control);
	}
}

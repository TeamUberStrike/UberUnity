using UnityEngine;

public abstract class PanelGuiBase : MonoBehaviour, IPanelGui
{
	public bool IsEnabled
	{
		get
		{
			return base.enabled;
		}
	}

	public virtual void Show()
	{
		base.enabled = true;
	}

	public virtual void Hide()
	{
		base.enabled = false;
	}
}

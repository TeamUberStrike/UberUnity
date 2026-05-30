using UnityEngine;

public class MecanimEventData : MonoBehaviour
{
	public MecanimEventDataEntry[] data;

	public Object animatorController;

	public Animator animator;

	private void Start()
	{
		if (animator == null)
		{
			Debug.LogWarning("Do not find animator component.");
			base.enabled = false;
		}
		else if (animatorController == null)
		{
			Debug.LogWarning("Please assgin animator in editor. Add emitter at runtime is not currently supported.");
			base.enabled = false;
		}
		else
		{
			MecanimEventManager.SetEventDataSource(this);
		}
	}

	private void Update()
	{
		foreach (MecanimEvent @event in MecanimEventManager.GetEvents(animatorController.GetInstanceID(), animator))
		{
			if (@event.paramType != MecanimEventParamTypes.None)
			{
				SendMessage(@event.functionName, @event.parameter, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				SendMessage(@event.functionName, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}

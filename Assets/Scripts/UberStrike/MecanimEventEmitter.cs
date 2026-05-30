using System.Collections.Generic;
using UnityEngine;

public class MecanimEventEmitter : MonoBehaviour
{
	public Object animatorController;

	public Animator animator;

	public MecanimEventEmitTypes emitType;

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
	}

	private void Update()
	{
		ICollection<MecanimEvent> events = MecanimEventManager.GetEvents(animatorController.GetInstanceID(), animator);
		foreach (MecanimEvent item in events)
		{
			MecanimEvent.SetCurrentContext(item);
			switch (emitType)
			{
			case MecanimEventEmitTypes.Upwards:
				if (item.paramType != MecanimEventParamTypes.None)
				{
					SendMessageUpwards(item.functionName, item.parameter, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					SendMessageUpwards(item.functionName, SendMessageOptions.DontRequireReceiver);
				}
				break;
			case MecanimEventEmitTypes.Broadcast:
				if (item.paramType != MecanimEventParamTypes.None)
				{
					BroadcastMessage(item.functionName, item.parameter, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					BroadcastMessage(item.functionName, SendMessageOptions.DontRequireReceiver);
				}
				break;
			default:
				if (item.paramType != MecanimEventParamTypes.None)
				{
					SendMessage(item.functionName, item.parameter, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					SendMessage(item.functionName, SendMessageOptions.DontRequireReceiver);
				}
				break;
			}
		}
	}
}

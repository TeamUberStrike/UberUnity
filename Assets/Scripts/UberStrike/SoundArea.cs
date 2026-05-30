using UnityEngine;

public class SoundArea : MonoBehaviour
{
	[SerializeField]
	private FootStepSoundType _footStep;

	private void OnTriggerEnter(Collider other)
	{
		SetFootStep(other);
	}

	private void OnTriggerStay(Collider other)
	{
		SetFootStep(other);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Avatar")
		{
			CharacterTrigger component = other.GetComponent<CharacterTrigger>();
			if ((bool)component && component.Character.Avatar != null && (bool)component.Character.Avatar.Decorator && GameState.Current.Map != null)
			{
				component.Character.Avatar.Decorator.CurrentFootStep = GameState.Current.Map.DefaultFootStep;
			}
		}
	}

	private void SetFootStep(Collider other)
	{
		if (other.tag == "Avatar")
		{
			CharacterTrigger component = other.GetComponent<CharacterTrigger>();
			if ((bool)component && component.Character.Avatar != null && (bool)component.Character.Avatar.Decorator)
			{
				component.Character.Avatar.Decorator.CurrentFootStep = _footStep;
			}
		}
	}
}

using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationSynchronizer : MonoBehaviour
{
	private AnimationState animationState;

	private void Start()
	{
		animationState = base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name];
	}

	private void LateUpdate()
	{
		if (GameState.Current.IsMultiplayer)
		{
			animationState.time = GameState.Current.GameTime;
		}
		else
		{
			animationState.time = Time.timeSinceLevelLoad;
		}
	}
}

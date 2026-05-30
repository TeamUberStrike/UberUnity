using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathArea : MonoBehaviour
{
	private void Awake()
	{
		if ((bool)base.GetComponent<Collider>())
		{
			base.GetComponent<Collider>().isTrigger = true;
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player")
		{
			GameState.Current.Actions.KillPlayer();
		}
	}
}

using UnityEngine;

public class PlayerDamageFeedback : MonoBehaviour
{
	private Material damageSplat;

	public Color[] DamageColors;

	public float Factor;

	private int colorIndex;

	private void Awake()
	{
		damageSplat = base.GetComponent<Renderer>().material;
	}

	public void RandomizeDamageFeedbackcolor()
	{
		colorIndex = Random.Range(0, 5);
	}

	public void ShowDamageFeedback(float damage)
	{
		DamageColors[colorIndex].a = damage * Factor;
		damageSplat.color = DamageColors[colorIndex];
	}
}

using UnityEngine;

public class AnimateTextureUV : MonoBehaviour
{
	public int uvAnimationTileX = 1;

	public int uvAnimationTileY = 1;

	public int framesPerSecond = 10;

	private void Update()
	{
		int num = Mathf.RoundToInt(Time.time * (float)framesPerSecond);
		num %= uvAnimationTileX * uvAnimationTileY;
		Vector2 scale = new Vector2(1f / (float)uvAnimationTileX, 1f / (float)uvAnimationTileY);
		int num2 = num % uvAnimationTileX;
		int num3 = num / uvAnimationTileX;
		Vector2 offset = new Vector2((float)num2 * scale.x, 1f - scale.y - (float)num3 * scale.y);
		if ((bool)base.GetComponent<Renderer>())
		{
			base.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
			base.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
		}
	}
}

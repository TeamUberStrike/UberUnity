using UnityEngine;

[AddComponentMenu("Image Effects/UnderWater")]
[ExecuteInEditMode]
public class UnderWaterEffect : ImageEffectBase
{
	public Texture textureRamp;

	public float fadeDistance = 10f;

	public Vector2 center = new Vector2(0.5f, 0.5f);

	public Vector2 radius = new Vector2(0.5f, 0.5f);

	public float maxAngle = 7f;

	private float effectWeight;

	public float Weight
	{
		set
		{
			effectWeight = value;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if ((bool)Camera.main)
		{
			Camera.main.depthTextureMode |= DepthTextureMode.Depth;
		}
		float num = base.GetComponent<Camera>().farClipPlane - base.GetComponent<Camera>().nearClipPlane;
		float value = fadeDistance / num;
		float angle = Mathf.Cos(Time.time) * maxAngle;
		base.material.SetTexture("_RampTex", textureRamp);
		base.material.SetFloat("_FadeDistance", value);
		base.material.SetFloat("_EffectWeight", effectWeight);
//		Graphics.Blit(base.material, source, destination, angle, center, radius);
	}

	public void Update()
	{
	}
}

using UnityEngine;

public class ZoomInView : MonoBehaviour
{
	[SerializeField]
	private UITexture zoomReticle;

	[SerializeField]
	private UISprite leftBg;

	[SerializeField]
	private UISprite rightBg;

	public bool isShown;

	public float PADDING = 2f;

	private void UpdateReticleSize()
	{
		UIRoot uIRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		float pixelSizeAdjustment = uIRoot.pixelSizeAdjustment;
		Vector3 localScale = leftBg.cachedTransform.localScale;
		localScale.x = 2f * ((float)Screen.width * 0.5f * pixelSizeAdjustment - zoomReticle.cachedTransform.localScale.x * 0.5f + PADDING * pixelSizeAdjustment);
		leftBg.cachedTransform.localScale = localScale;
		rightBg.cachedTransform.localScale = localScale;
	}

	public void Show(bool show)
	{
		zoomReticle.gameObject.SetActive(show);
		leftBg.gameObject.SetActive(show);
		rightBg.gameObject.SetActive(show);
		isShown = show;
	}

	private void LateUpdate()
	{
		if (isShown)
		{
			UpdateReticleSize();
		}
	}
}

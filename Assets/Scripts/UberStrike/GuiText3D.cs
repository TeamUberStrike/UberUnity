using System.Collections;
using UnityEngine;

public class GuiText3D : MonoBehaviour
{
	public Font mFont;

	public string mText;

	public Camera mCamera;

	public Transform mTarget;

	public float mMaxDistance = 20f;

	public float mLifeTime = 5f;

	public Color mColor = Color.black;

	public bool mFadeOut = true;

	public Vector3 mFadeDirection = Vector2.up;

	private UnityEngine.UI.Text _guiText;

	private Transform _transform;

	private Material _material;

	private Vector3 _viewportPosition;

	private float time;

	private Vector3 fadeDir = Vector3.zero;

	private Color startColor;

	private Color finalColor;

	private void Awake()
	{
		_transform = base.transform;
	}

	private void Start()
	{
		_guiText = base.gameObject.AddComponent(typeof(UnityEngine.UI.Text)) as UnityEngine.UI.Text;
		_guiText.alignment = TextAnchor.MiddleCenter;
//		_guiText.anchor = TextAnchor.MiddleCenter;
		if (mCamera == null || mTarget == null || mFont == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_guiText.font = mFont;
		_guiText.text = mText;
		_guiText.material = mFont.material;
		_material = _guiText.material;
		startColor = _material.color;
		finalColor = _material.color;
		if (mFadeOut)
		{
			finalColor.a = 0f;
		}
	}

	private void LateUpdate()
	{
		if (mCamera != null && mTarget != null && (mLifeTime < 0f || mLifeTime > time))
		{
			time += Time.deltaTime;
			_viewportPosition = mCamera.WorldToViewportPoint(mTarget.localPosition);
			if (mFadeOut && mLifeTime > 0f)
			{
				_material.color = Color.Lerp(startColor, finalColor, time / mLifeTime);
			}
			else
			{
				float t = Mathf.Clamp01(_viewportPosition.z / mMaxDistance);
				_material.color = Color.Lerp(startColor, finalColor, t);
			}
			fadeDir += Time.deltaTime * mFadeDirection;
			_transform.localPosition = _viewportPosition + fadeDir;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator startShowGuiText(float mLifeTime)
	{
		float time = 0f;
		Vector3 fadeDir = Vector3.zero;
		Color startColor = _material.color;
		Color finalColor = _material.color;
		if (mFadeOut)
		{
			finalColor.a = 0f;
		}
		while (mCamera != null && mTarget != null && (mLifeTime < 0f || mLifeTime > time))
		{
			time += Time.deltaTime;
			_viewportPosition = mCamera.WorldToViewportPoint(mTarget.localPosition);
			if (mFadeOut && mLifeTime > 0f)
			{
				_material.color = Color.Lerp(startColor, finalColor, time / mLifeTime);
			}
			else
			{
				float dist = Mathf.Clamp01(_viewportPosition.z / mMaxDistance);
				_material.color = Color.Lerp(startColor, finalColor, dist);
			}
			fadeDir += Time.deltaTime * mFadeDirection;
			_transform.localPosition = _viewportPosition + fadeDir;
			yield return new WaitForEndOfFrame();
		}
		Object.Destroy(base.gameObject);
	}
}

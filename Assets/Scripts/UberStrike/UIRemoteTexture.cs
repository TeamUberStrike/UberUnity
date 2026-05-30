using UnityEngine;

[AddComponentMenu("NGUI/CMune Extensions/Texture Remote")]
[ExecuteInEditMode]
public class UIRemoteTexture : MonoBehaviour
{
	private enum DownloadState
	{
		None = 0,
		Downloading = 1,
		Downloaded = 2,
		Error = 3
	}

	[SerializeField]
	private string url;

	[SerializeField]
	private UITexture uiTexture;

	[SerializeField]
	private UISprite loadingSpinning;

	[SerializeField]
	private UISprite defaultImage;

	private float rotationSpeed = 300f;

	private DownloadState _state;

	public string Url
	{
		get
		{
			return url;
		}
		set
		{
			if (!(url == value))
			{
				url = value;
				if (!string.IsNullOrEmpty(url))
				{
					State = DownloadState.Downloading;
				}
			}
		}
	}

	private DownloadState State
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			TryEnableAndSetScale(uiTexture, value == DownloadState.Downloaded);
			TryEnableAndSetScale(loadingSpinning, value == DownloadState.None || value == DownloadState.Downloading);
			TryEnableAndSetScale(defaultImage, value == DownloadState.None || value == DownloadState.Downloading || value == DownloadState.Error);
		}
	}

	public void ShowDefault()
	{
		State = DownloadState.Error;
	}

	private void Start()
	{
		if (uiTexture == null)
		{
			GameObject gameObject = new GameObject("LoadedTexture");
			gameObject.layer = base.gameObject.layer;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			uiTexture = gameObject.AddComponent<UITexture>();
			uiTexture.depth = 0;
			uiTexture.enabled = false;
		}
		if (loadingSpinning == null)
		{
			GameObject gameObject2 = new GameObject("LoadingSpinning");
			gameObject2.layer = base.gameObject.layer;
			gameObject2.transform.parent = base.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localScale = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			loadingSpinning = gameObject2.AddComponent<UISprite>();
			loadingSpinning.depth = 2;
		}
		if (defaultImage == null)
		{
			GameObject gameObject3 = new GameObject("DefaultImage");
			gameObject3.layer = base.gameObject.layer;
			gameObject3.transform.parent = base.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localScale = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			defaultImage = gameObject3.AddComponent<UISprite>();
			defaultImage.depth = 1;
		}
		if (!string.IsNullOrEmpty(url) && uiTexture.mainTexture == null)
		{
			State = (Application.isPlaying ? DownloadState.Downloading : DownloadState.None);
		}
	}

	private void TryEnableAndSetScale(UITexture texture, bool enabled)
	{
		if (!(texture == null))
		{
			texture.enabled = enabled;
			if (enabled && texture.transform.localScale == Vector3.zero && texture.mainTexture != null)
			{
				texture.transform.localScale = new Vector3(texture.mainTexture.width, texture.mainTexture.height, 1f);
			}
		}
	}

	private void TryEnableAndSetScale(UISprite sprite, bool enabled)
	{
		if (sprite == null)
		{
			return;
		}
		sprite.enabled = enabled;
		if (enabled)
		{
			UIAtlas.Sprite atlasSprite = sprite.GetAtlasSprite();
			if (sprite.transform.localScale == Vector3.zero && atlasSprite != null)
			{
				sprite.transform.localScale = new Vector3(atlasSprite.outer.width, atlasSprite.outer.height, 1f);
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying || State != DownloadState.Downloading)
		{
			return;
		}
		if (loadingSpinning != null && loadingSpinning.enabled)
		{
			loadingSpinning.transform.localRotation = Quaternion.Euler(0f, 0f, (0f - Time.time) * rotationSpeed + Time.time * rotationSpeed % 30f);
		}
		TextureLoader.Holder holder = AutoMonoBehaviour<TextureLoader>.Instance.Load(url);
		if (holder.State != TextureLoader.State.Downloading)
		{
			if (holder.State == TextureLoader.State.Ok)
			{
				uiTexture.mainTexture = holder.Texture;
				State = DownloadState.Downloaded;
			}
			else
			{
				State = DownloadState.Error;
			}
		}
	}
}

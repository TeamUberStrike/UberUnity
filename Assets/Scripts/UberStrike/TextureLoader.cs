using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureLoader : AutoMonoBehaviour<TextureLoader>
{
	public enum State
	{
		Downloading = 0,
		Ok = 1,
		Error = 2,
		Timeout = 3
	}

	public class Holder
	{
		public string Url;

		public Texture2D Texture;

		public State State;
	}

	private readonly float TIMEOUT = 30f;

	public Dictionary<string, Holder> cache = new Dictionary<string, Holder>();

	public List<Holder> pending = new List<Holder>();

	private Holder nullHolder = new Holder
	{
		State = State.Ok
	};

	protected override void Start()
	{
		base.Start();
		nullHolder.Texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
		for (int i = 0; i < 5; i++)
		{
			StartCoroutine(WorkerCrt());
		}
	}

	private IEnumerator WorkerCrt()
	{
		while (true)
		{
			if (pending.Count == 0)
			{
				yield return 0;
				continue;
			}
			Holder item = pending[0];
			pending.RemoveAt(0);
			WWW www = new WWW(item.Url);
			float start = Time.time;
			while (!www.isDone && Time.time - start <= TIMEOUT)
			{
				yield return 0;
			}
			if (!www.isDone)
			{
				item.State = State.Timeout;
				Debug.Log("Failed to download texture " + item.Url + ". Timeout.");
				www.Dispose();
			}
			else if (!string.IsNullOrEmpty(www.error))
			{
				item.State = State.Error;
				Debug.Log("Failed to download texture " + item.Url + ". " + www.error);
			}
			else
			{
				www.LoadImageIntoTexture(item.Texture);
				item.State = State.Ok;
			}
		}
	}

	public Texture2D LoadImage(string url, Texture2D placeholder = null)
	{
		return Load(url, placeholder).Texture;
	}

	public Holder Load(string url, Texture2D placeholder = null)
	{
		if (string.IsNullOrEmpty(url))
		{
			return nullHolder;
		}
		Holder value = null;
		if (cache.TryGetValue(url, out value))
		{
			return value;
		}
		Holder holder = new Holder();
		holder.Url = url;
		holder.Texture = ((!(placeholder == null)) ? (Object.Instantiate(placeholder) as Texture2D) : new Texture2D(1, 1, TextureFormat.RGB24, false));
		value = holder;
		cache[url] = value;
		pending.Add(value);
		return value;
	}

	public State GetState(string url)
	{
		return cache[url].State;
	}

	public State GetState(Texture2D tex)
	{
		return new List<Holder>(cache.Values).Find((Holder el) => el.Texture == tex).State;
	}
}

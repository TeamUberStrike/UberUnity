using System.Collections.Generic;
using UnityEngine;

public static class MaterialUtil
{
	private class MaterialCache
	{
		public Dictionary<string, Color> Colors = new Dictionary<string, Color>();

		public Dictionary<string, float> Floats = new Dictionary<string, float>();

		public Dictionary<string, Vector2> TextureOffset = new Dictionary<string, Vector2>();

		public Dictionary<string, Texture> Texture = new Dictionary<string, Texture>();
	}

	private static Dictionary<Material, MaterialCache> _cache = new Dictionary<Material, MaterialCache>();

	public static void SetFloat(Material m, string propertyName, float value)
	{
		if ((bool)m && m.HasProperty(propertyName))
		{
			MaterialCache value2;
			if (!_cache.TryGetValue(m, out value2))
			{
				value2 = new MaterialCache();
				_cache[m] = value2;
			}
			if (!value2.Floats.ContainsKey(propertyName))
			{
				value2.Floats[propertyName] = m.GetFloat(propertyName);
			}
			m.SetFloat(propertyName, value);
		}
		else
		{
			Debug.LogError(string.Format("Property<float> '{0}' not found in Material {1}", propertyName, (!m) ? "NULL" : m.name));
		}
	}

	public static void SetColor(Material m, string propertyName, Color value)
	{
		if ((bool)m && m.HasProperty(propertyName))
		{
			MaterialCache value2;
			if (!_cache.TryGetValue(m, out value2))
			{
				value2 = new MaterialCache();
				_cache[m] = value2;
			}
			if (!value2.Colors.ContainsKey(propertyName))
			{
				value2.Colors[propertyName] = m.GetColor(propertyName);
			}
			m.SetColor(propertyName, value);
		}
		else
		{
			Debug.LogError(string.Format("Property<Color> '{0}' not found in Material {1}", propertyName, (!m) ? "NULL" : m.name));
		}
	}

	public static void SetTextureOffset(Material m, string propertyName, Vector2 value)
	{
		if ((bool)m && m.HasProperty(propertyName))
		{
			MaterialCache value2;
			if (!_cache.TryGetValue(m, out value2))
			{
				value2 = new MaterialCache();
				_cache[m] = value2;
			}
			if (!value2.TextureOffset.ContainsKey(propertyName))
			{
				value2.TextureOffset[propertyName] = m.GetTextureOffset(propertyName);
			}
			m.SetTextureOffset(propertyName, value);
		}
		else
		{
			Debug.LogError(string.Format("Property<Vector2> '{0}' not found in Material {1}", propertyName, (!m) ? "NULL" : m.name));
		}
	}

	public static void SetTexture(Material m, string propertyName, Texture value)
	{
		if ((bool)m && m.HasProperty(propertyName))
		{
			MaterialCache value2;
			if (!_cache.TryGetValue(m, out value2))
			{
				value2 = new MaterialCache();
				_cache[m] = value2;
			}
			if (!value2.Texture.ContainsKey(propertyName))
			{
				value2.Texture[propertyName] = m.GetTexture(propertyName);
			}
			m.SetTexture(propertyName, value);
		}
		else
		{
			Debug.LogError(string.Format("Property<Texture> '{0}' not found in Material {1}", propertyName, (!m) ? "NULL" : m.name));
		}
	}

	public static void Reset(Material m)
	{
		MaterialCache value;
		if (!_cache.TryGetValue(m, out value))
		{
			return;
		}
		foreach (KeyValuePair<string, Color> color in value.Colors)
		{
			m.SetColor(color.Key, color.Value);
		}
		foreach (KeyValuePair<string, float> @float in value.Floats)
		{
			m.SetFloat(@float.Key, @float.Value);
		}
		foreach (KeyValuePair<string, Vector2> item in value.TextureOffset)
		{
			m.SetTextureOffset(item.Key, item.Value);
		}
	}
}

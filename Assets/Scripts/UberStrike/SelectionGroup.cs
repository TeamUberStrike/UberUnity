using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionGroup<T> where T : IComparable
{
	private class Pair
	{
		public T Item;

		public GUIContent Content;
	}

	private List<Pair> _data = new List<Pair>();

	public int Index { get; private set; }

	public T Current { get; private set; }

	public int Length
	{
		get
		{
			return _data.Count;
		}
	}

	public GUIContent[] GuiContent { get; private set; }

	public T[] Items { get; private set; }

	public event Action<T> OnSelectionChange;

	public SelectionGroup()
	{
		GuiContent = new GUIContent[0];
	}

	public void SetIndex(int index)
	{
		Index = index;
		if (index >= 0 && index < _data.Count)
		{
			Current = _data[index].Item;
		}
		else
		{
			Current = default(T);
		}
		if (this.OnSelectionChange != null)
		{
			this.OnSelectionChange(Current);
		}
	}

	public void Select(T item)
	{
		Index = _data.FindIndex((Pair i) => i.Item.CompareTo(item) == 0);
		Current = item;
		if (this.OnSelectionChange != null)
		{
			this.OnSelectionChange(Current);
		}
	}

	public void Add(T item, GUIContent content)
	{
		_data.Add(new Pair
		{
			Item = item,
			Content = content
		});
		GuiContent = _data.ConvertAll((Pair p) => p.Content).ToArray();
		Items = _data.ConvertAll((Pair p) => p.Item).ToArray();
	}
}

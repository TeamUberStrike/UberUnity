using System;
using System.Collections.Generic;
using UnityEngine;

public class Property<T>
{
	private class Act<TT>
	{
		public MonoBehaviour Mb;

		public bool HasMb;

		public Action<TT> Changed;

		public Action<TT, TT> ChangedWithPrev;
	}

	private List<Act<T>> Callbacks = new List<Act<T>>();

	private T currentValue;

	public virtual T Value
	{
		get
		{
			return currentValue;
		}
		set
		{
			T oldValue = currentValue;
			currentValue = value;
			FireEvent(oldValue);
		}
	}

	public Property()
	{
	}

	public Property(T defaultValue)
	{
		currentValue = defaultValue;
	}

	public void AddEvent(Action<T> onChanged, MonoBehaviour mb)
	{
		Callbacks.Add(new Act<T>
		{
			Mb = mb,
			HasMb = (mb != null),
			Changed = onChanged
		});
	}

	public void AddEvent(Action<T, T> onChanged, MonoBehaviour mb)
	{
		Callbacks.Add(new Act<T>
		{
			Mb = mb,
			HasMb = (mb != null),
			ChangedWithPrev = onChanged
		});
	}

	public void AddEventAndFire(Action<T> onChanged, MonoBehaviour mb)
	{
		AddEvent(onChanged, mb);
		onChanged(currentValue);
	}

	public void AddEventAndFire(Action<T, T> onChanged, MonoBehaviour mb)
	{
		AddEvent(onChanged, mb);
		onChanged(currentValue, currentValue);
	}

	public void RemoveEvent(Action<T> onChanged)
	{
		Callbacks.RemoveAll((Act<T> el) => el.Changed == onChanged);
	}

	public void RemoveEvent(Action<T, T> onChanged)
	{
		Callbacks.RemoveAll((Act<T> el) => el.ChangedWithPrev == onChanged);
	}

	public void RemoveEvent(MonoBehaviour mb)
	{
		Callbacks.RemoveAll((Act<T> el) => el.Mb == mb);
	}

	public void Fire()
	{
		FireEvent(currentValue);
	}

	public void Fire(T newValue)
	{
		Value = newValue;
	}

	public void FireEvent(T oldValue)
	{
		Callbacks.RemoveAll(delegate(Act<T> el)
		{
			try
			{
				if (el.HasMb && el.Mb == null)
				{
					return true;
				}
				if (!el.HasMb || (el.Mb.gameObject.activeInHierarchy && el.Mb.enabled))
				{
					if (el.Changed != null)
					{
						el.Changed(currentValue);
					}
					if (el.ChangedWithPrev != null)
					{
						el.ChangedWithPrev(currentValue, oldValue);
					}
				}
				return false;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return false;
			}
		});
	}

	public override string ToString()
	{
		return currentValue.ToString();
	}

	public static implicit operator T(Property<T> property)
	{
		return property.Value;
	}
}

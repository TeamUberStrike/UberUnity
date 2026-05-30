using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : StateMachine<int>
{
}
public class StateMachine<T> where T : struct, IConvertible
{
	public readonly EventHandler Events = new EventHandler();

	private Dictionary<T, IState> registeredStates;

	private Stack<T> stateStack;

	public T CurrentStateId
	{
		get
		{
			return (stateStack.Count <= 0) ? default(T) : stateStack.Peek();
		}
	}

	private IState CurrentState
	{
		get
		{
			return GetState(CurrentStateId);
		}
	}

	public event Action<T> OnChanged;

	public StateMachine()
	{
		registeredStates = new Dictionary<T, IState>();
		stateStack = new Stack<T>();
	}

	public void SetState(T stateId)
	{
		if (ContainsState(stateId))
		{
			if (!stateId.Equals(CurrentStateId))
			{
				PopAllStates();
				stateStack.Push(stateId);
				GetState(stateId).OnEnter();
				if (this.OnChanged != null)
				{
					this.OnChanged(stateId);
				}
			}
			return;
		}
		throw new Exception("Unsupported state of type: " + stateId);
	}

	public void PushState(T stateId)
	{
		if (ContainsState(stateId))
		{
			if (!stateStack.Contains(stateId))
			{
				stateStack.Push(stateId);
				GetState(stateId).OnEnter();
				if (this.OnChanged != null)
				{
					this.OnChanged(stateId);
				}
			}
		}
		else
		{
			Debug.LogWarning("Unsupported state of type: " + stateId);
		}
	}

	public void PopState(bool resume = true)
	{
		if (stateStack.Count != 0)
		{
			CurrentState.OnExit();
			stateStack.Pop();
			if (resume && stateStack.Count != 0)
			{
				CurrentState.OnResume();
			}
			if (this.OnChanged != null && stateStack.Count > 0)
			{
				this.OnChanged(stateStack.Peek());
			}
		}
	}

	public void Reset()
	{
		PopAllStates();
		stateStack.Clear();
		registeredStates.Clear();
		Events.Clear();
		if (this.OnChanged != null)
		{
			this.OnChanged(default(T));
		}
	}

	public void PopAllStates()
	{
		while (stateStack.Count > 0)
		{
			PopState(false);
		}
		if (this.OnChanged != null)
		{
			this.OnChanged(default(T));
		}
	}

	public void RegisterState(T stateId, IState state)
	{
		if (!registeredStates.ContainsKey(stateId))
		{
			registeredStates.Add(stateId, state);
			return;
		}
		throw new Exception(string.Concat("StateMachine::RegisterState - state [", stateId, "] already exists in the current registry"));
	}

	public bool ContainsState(T stateId)
	{
		return registeredStates.ContainsKey(stateId);
	}

	public void Update()
	{
		if (stateStack.Count > 0)
		{
			CurrentState.OnUpdate();
		}
	}

	public IState GetState(T stateId)
	{
		IState value;
		registeredStates.TryGetValue(stateId, out value);
		return value;
	}
}

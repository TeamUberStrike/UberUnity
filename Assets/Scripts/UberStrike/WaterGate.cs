using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WaterGate : SecretDoor
{
	private enum DoorState
	{
		Closed = 0,
		Opening = 1,
		Open = 2,
		Closing = 3
	}

	[Serializable]
	public class DoorElement
	{
		[HideInInspector]
		public Vector3 ClosedPosition;

		[HideInInspector]
		public Quaternion ClosedRotation;

		public GameObject Element;

		public Vector3 OpenPosition;
	}

	[SerializeField]
	private float _maxTime = 1f;

	[SerializeField]
	private DoorElement[] _elements;

	private DoorState _state;

	private float _currentTime;

	private float _timeToClose;

	private int _doorID;

	public int DoorID
	{
		get
		{
			return _doorID;
		}
	}

	private void Awake()
	{
		_state = DoorState.Closed;
		DoorElement[] elements = _elements;
		foreach (DoorElement doorElement in elements)
		{
			doorElement.ClosedPosition = doorElement.Element.transform.localPosition;
		}
		_doorID = base.transform.position.GetHashCode();
	}

	public override void Open()
	{
		GameState.Current.Actions.OpenDoor(DoorID);
		OpenDoor();
	}

	private void OpenDoor()
	{
		switch (_state)
		{
		case DoorState.Closed:
			_state = DoorState.Opening;
			_currentTime = 0f;
			break;
		case DoorState.Closing:
			_state = DoorState.Opening;
			_currentTime = _maxTime - _currentTime;
			break;
		case DoorState.Open:
			_timeToClose = Time.time + 2f;
			break;
		}
		if ((bool)base.GetComponent<AudioSource>())
		{
			base.GetComponent<AudioSource>().Play();
		}
	}

	private void OnEnable()
	{
		EventHandler.Global.AddListener<GameEvents.DoorOpened>(OnDoorOpenedEvent);
	}

	private void OnDisable()
	{
		EventHandler.Global.RemoveListener<GameEvents.DoorOpened>(OnDoorOpenedEvent);
	}

	private void OnDoorOpenedEvent(GameEvents.DoorOpened ev)
	{
		if (DoorID == ev.DoorID)
		{
			OpenDoor();
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player")
		{
			Open();
		}
	}

	private void OnTriggerStay(Collider c)
	{
		if (c.tag == "Player")
		{
			_timeToClose = Time.time + 2f;
		}
	}

	private void Update()
	{
		if (_state == DoorState.Opening)
		{
			_currentTime += Time.deltaTime;
			DoorElement[] elements = _elements;
			foreach (DoorElement doorElement in elements)
			{
				doorElement.Element.transform.localPosition = Vector3.Lerp(doorElement.ClosedPosition, doorElement.OpenPosition, _currentTime / _maxTime);
			}
			if (_currentTime >= _maxTime)
			{
				_state = DoorState.Open;
				_timeToClose = Time.time + 2f;
				if ((bool)base.GetComponent<AudioSource>())
				{
					base.GetComponent<AudioSource>().Stop();
				}
			}
		}
		else if (_state == DoorState.Open)
		{
			if (_timeToClose < Time.time)
			{
				_state = DoorState.Closing;
				_currentTime = 0f;
				if ((bool)base.GetComponent<AudioSource>())
				{
					base.GetComponent<AudioSource>().Play();
				}
			}
		}
		else
		{
			if (_state != DoorState.Closing)
			{
				return;
			}
			_currentTime += Time.deltaTime;
			DoorElement[] elements2 = _elements;
			foreach (DoorElement doorElement2 in elements2)
			{
				doorElement2.Element.transform.localPosition = Vector3.Lerp(doorElement2.OpenPosition, doorElement2.ClosedPosition, _currentTime / _maxTime);
			}
			if (_currentTime >= _maxTime)
			{
				_state = DoorState.Closed;
				if ((bool)base.GetComponent<AudioSource>())
				{
					base.GetComponent<AudioSource>().Stop();
				}
			}
		}
	}
}

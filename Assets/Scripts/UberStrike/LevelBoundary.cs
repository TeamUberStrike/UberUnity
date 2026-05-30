using System.Collections;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LevelBoundary : MonoBehaviour
{
	private static float _checkTime;

	private static LevelBoundary _currentLevelBoundary;

	private void Awake()
	{
		if ((bool)base.GetComponent<Renderer>())
		{
			base.GetComponent<Renderer>().enabled = false;
		}
		StartCoroutine(StartCheckingPlayerInBounds(base.GetComponent<Collider>()));
		base.GetComponent<Collider>().isTrigger = true;
	}

	private void OnDisable()
	{
		_checkTime = 0f;
		_currentLevelBoundary = null;
	}

	private void OnTriggerExit(Collider c)
	{
		if (c.tag == "Player" && GameState.Current.HasJoinedGame)
		{
			if (_currentLevelBoundary == this)
			{
				_currentLevelBoundary = null;
			}
			StartCoroutine(StartCheckingPlayer());
		}
	}

	private IEnumerator StartCheckingPlayer()
	{
		if (_checkTime == 0f)
		{
			_checkTime = Time.time + 0.5f;
			while (_checkTime > Time.time)
			{
				yield return new WaitForEndOfFrame();
			}
			if (_currentLevelBoundary == null)
			{
				GameState.Current.Actions.KillPlayer();
			}
			_checkTime = 0f;
		}
		else
		{
			_checkTime = Time.time + 1f;
		}
	}

	private IEnumerator StartCheckingPlayerInBounds(Collider c)
	{
		while (true)
		{
			if (!c.bounds.Contains(GameState.Current.PlayerData.Position))
			{
				GameState.Current.Actions.KillPlayer();
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player" && GameState.Current.HasJoinedGame)
		{
			_currentLevelBoundary = this;
		}
	}

	private string PrintHierarchy(Transform t)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(t.name);
		Transform parent = t.parent;
		while ((bool)parent)
		{
			stringBuilder.Insert(0, parent.name + "/");
			parent = parent.parent;
		}
		return stringBuilder.ToString();
	}

	private string PrintVector(Vector3 v)
	{
		return string.Format("({0:N6},{1:N6},{2:N6})", v.x, v.y, v.z);
	}
}

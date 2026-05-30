using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Activate Advanced")]
public class UIButtonActivateExtended : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _targets;

	[SerializeField]
	private bool _state;

	[SerializeField]
	private bool _switch;

	private void OnClick()
	{
		if (_targets.Length == 0)
		{
			return;
		}
		GameObject[] targets = _targets;
		foreach (GameObject gameObject in targets)
		{
			if (gameObject != null)
			{
				NGUITools.SetActive(gameObject, (!_switch) ? _state : (!gameObject.activeInHierarchy));
			}
		}
	}
}

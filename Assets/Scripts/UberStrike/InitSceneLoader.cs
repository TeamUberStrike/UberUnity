using UnityEngine;

public class InitSceneLoader : MonoBehaviour
{
	private void Awake()
	{
		if (!GlobalSceneLoader.IsInitialised)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("InitScene");
		}
	}
}

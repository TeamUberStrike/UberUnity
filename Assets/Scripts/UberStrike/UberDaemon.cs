using System.Diagnostics;
using UnityEngine;

public class UberDaemon : MonoBehaviour
{
	public static UberDaemon Instance;

	private void Awake()
	{
		Instance = this;
	}

	public string GetMagicHash(string authToken)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		string fileName = "bash";
		processStartInfo.Arguments = "uberdaemon.sh " + authToken;
		if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			fileName = "uberdaemon.exe";
			processStartInfo.Arguments = authToken;
		}
		processStartInfo.RedirectStandardError = true;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.UseShellExecute = false;
		processStartInfo.FileName = fileName;
		processStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
		processStartInfo.CreateNoWindow = true;
		Process process = Process.Start(processStartInfo);
		return process.StandardOutput.ReadToEnd().Trim();
	}
}

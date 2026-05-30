using System;
using System.Diagnostics;
using System.IO;

namespace Steamworks
{
	public class DllCheck
	{
		public static bool Test()
		{
			bool flag = true;
			return CheckSteamAPIDLL();
		}

		private static bool CheckSteamAPIDLL()
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			string text;
			int num;
			if (IntPtr.Size == 4)
			{
				text = Path.Combine(currentDirectory, "steam_api.dll");
				num = 187584;
			}
			else
			{
				text = Path.Combine(currentDirectory, "steam_api64.dll");
				num = 208296;
			}
			if (File.Exists(text))
			{
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.Length != num)
				{
					return false;
				}
				if (FileVersionInfo.GetVersionInfo(text).FileVersion != "02.59.51.43")
				{
					return false;
				}
			}
			return true;
		}
	}
}

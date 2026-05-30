using System.Text;
using UberStrike.Realtime.UnitySdk;
using UnityEngine;

public class DebugLogMessages : IDebugPage
{
	public class ConsoleDebug
	{
		private LimitedQueue<string> _queue = new LimitedQueue<string>(300);

		private string _debugOut = string.Empty;

		public string DebugOut
		{
			get
			{
				return _debugOut;
			}
		}

		public void Log(int level, string s)
		{
			_queue.Enqueue(s);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in _queue)
			{
				stringBuilder.AppendLine(item);
			}
			_debugOut = stringBuilder.ToString();
		}

		public string ToHTML()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<h3>DEBUG LOG</h3>");
			foreach (string item in _queue)
			{
				stringBuilder.AppendLine(item + "<br/>");
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in _queue)
			{
				stringBuilder.AppendLine(item);
			}
			return stringBuilder.ToString();
		}
	}

	public static readonly ConsoleDebug Console = new ConsoleDebug();

	public string Title
	{
		get
		{
			return "Logs";
		}
	}

	public void Draw()
	{
		GUILayout.TextArea(Console.DebugOut);
	}

	public static void Log(int type, string msg)
	{
		Console.Log(type, msg);
	}
}

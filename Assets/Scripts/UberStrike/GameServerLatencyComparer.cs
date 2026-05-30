using System.Collections.Generic;
using Cmune.Core.Models;

public class GameServerLatencyComparer : IComparer<PhotonServer>
{
	public int Compare(PhotonServer a, PhotonServer b)
	{
		return StaticCompare(a, b);
	}

	public static int StaticCompare(PhotonServer a, PhotonServer b)
	{
		int num = 1;
		int num2 = ((a.Data.State != PhotonServerLoad.Status.Alive) ? 1000 : a.Latency);
		int num3 = ((b.Data.State != PhotonServerLoad.Status.Alive) ? 1000 : b.Latency);
		if (a.Latency == b.Latency)
		{
			return string.Compare(b.Name, a.Name);
		}
		return (num2 <= num3) ? (num * -1) : num;
	}
}

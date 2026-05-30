public class PlayerLeadAudio
{
	public enum LeadState
	{
		Tied = 0,
		Me = 1,
		Others = 2
	}

	private int lastKillsLeftPlayed;

	public LeadState CurrentLead { get; private set; }

	public void Reset()
	{
		CurrentLead = LeadState.Tied;
		lastKillsLeftPlayed = 0;
	}

	public void UpdateLeadStatus(int myKills, int otherKills, bool playAudio = true)
	{
		LeadState currentLead = CurrentLead;
		if (myKills > otherKills)
		{
			CurrentLead = LeadState.Me;
		}
		else if (otherKills == myKills)
		{
			CurrentLead = LeadState.Tied;
		}
		else
		{
			CurrentLead = LeadState.Others;
		}
		if (currentLead != CurrentLead && playAudio)
		{
			switch (CurrentLead)
			{
			case LeadState.Me:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.TakenLead, 500uL, 1f, 1f);
				break;
			case LeadState.Tied:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.TiedLead, 500uL, 1f, 1f);
				break;
			case LeadState.Others:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.LostLead, 500uL, 1f, 1f);
				break;
			}
		}
	}

	public void PlayKillsLeftAudio(int killsLeft)
	{
		if (lastKillsLeftPlayed != killsLeft)
		{
			switch (killsLeft)
			{
			case 1:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.KillLeft1, 2000uL, 1f, 1f);
				break;
			case 2:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.KillLeft2, 2000uL, 1f, 1f);
				break;
			case 3:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.KillLeft3, 2000uL, 1f, 1f);
				break;
			case 4:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.KillLeft4, 2000uL, 1f, 1f);
				break;
			case 5:
				AutoMonoBehaviour<SfxManager>.Instance.Play2dAudioClip(GameAudio.KillLeft5, 2000uL, 1f, 1f);
				break;
			}
			lastKillsLeftPlayed = killsLeft;
		}
	}
}

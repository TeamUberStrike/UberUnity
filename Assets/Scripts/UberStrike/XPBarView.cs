using System.Collections;
using UnityEngine;

public class XPBarView : MonoBehaviour
{
	[SerializeField]
	private UILabel currentLevel;

	[SerializeField]
	private UILabel nextLevel;

	[SerializeField]
	private UISprite bgr;

	[SerializeField]
	private UISprite bar;

	[SerializeField]
	private float animageSpeed = 2f;

	private float cachedXP = -1f;

	private IEnumerator Animate(float percentage01)
	{
		percentage01 = Mathf.Clamp01(percentage01);
		Transform tr = bar.transform;
		float fullWidth = bgr.transform.localScale.x;
		while (Mathf.Abs(tr.localScale.x / fullWidth - percentage01) > 0.01f)
		{
			Vector3 scale = tr.localScale;
			scale.x = Mathf.MoveTowards(scale.x, fullWidth * percentage01, Time.deltaTime * animageSpeed * fullWidth);
			tr.localScale = scale;
			yield return 0;
		}
	}

	private void Update()
	{
		int playerExperience = PlayerDataManager.PlayerExperience;
		if ((float)playerExperience != cachedXP)
		{
			cachedXP = playerExperience;
			int levelForXp = XpPointsUtil.GetLevelForXp(playerExperience);
			currentLevel.text = "Lvl " + levelForXp;
			nextLevel.text = "Lvl " + Mathf.Clamp(levelForXp + 1, 1, XpPointsUtil.MaxPlayerLevel);
			int minXp;
			int maxXp;
			XpPointsUtil.GetXpRangeForLevel(levelForXp, out minXp, out maxXp);
			StopAllCoroutines();
			StartCoroutine(Animate((float)(playerExperience - minXp) / (float)(maxXp - minXp)));
		}
	}
}

using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Examples/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
	public int charsPerSecond = 40;

	public bool loop;

	private UILabel mLabel;

	private string mText;

	private int mOffset;

	private float mNextChar;

	private void Update()
	{
		if (mLabel == null)
		{
			mLabel = GetComponent<UILabel>();
			mLabel.supportEncoding = false;
			mLabel.symbolStyle = UIFont.SymbolStyle.None;
			mText = mLabel.font.WrapText(mLabel.text, (float)mLabel.lineWidth / mLabel.cachedTransform.localScale.x, mLabel.maxLineCount, false, UIFont.SymbolStyle.None);
		}
		if (mOffset < mText.Length)
		{
			if (mNextChar <= Time.time)
			{
				charsPerSecond = Mathf.Max(1, charsPerSecond);
				float num = 1f / (float)charsPerSecond;
				char c = mText[mOffset];
				if (c == '.' || c == '\n' || c == '!' || c == '?')
				{
					num *= 4f;
				}
				mNextChar = Time.time + num;
				mLabel.text = mText.Substring(0, ++mOffset);
			}
		}
		else if (loop)
		{
			mOffset = 0;
			mText = mLabel.font.WrapText(mLabel.text, (float)mLabel.lineWidth / mLabel.cachedTransform.localScale.x, mLabel.maxLineCount, false, UIFont.SymbolStyle.None);
		}
		else
		{
			Object.Destroy(this);
		}
	}
}

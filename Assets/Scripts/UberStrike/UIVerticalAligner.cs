using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/CMune Extensions/Vertical Aligner")]
[ExecuteInEditMode]
public class UIVerticalAligner : MonoBehaviour
{
	public enum Direction
	{
		TopToBottom = 0,
		BottomToTop = 1
	}

	public Direction direction;

	public float padding;

	public bool sorted = true;

	public bool hideInactive = true;

	[SerializeField]
	private bool repositionNow;

	private bool mStarted;

	public void Reposition()
	{
		repositionNow = true;
	}

	private void Start()
	{
		mStarted = true;
		Reposition();
	}

	private void LateUpdate()
	{
		if (repositionNow)
		{
			repositionNow = false;
			DoReposition();
		}
	}

	private void DoReposition()
	{
		if (!mStarted)
		{
			repositionNow = true;
			return;
		}
		List<Transform> list = new List<Transform>();
		foreach (Transform item in base.transform)
		{
			if (item != null && (bool)item.gameObject && (!hideInactive || NGUITools.GetActive(item.gameObject)))
			{
				list.Add(item);
			}
		}
		if (sorted)
		{
			list.Sort((Transform el1, Transform el2) => el1.name.CompareTo(el2.name));
		}
		if (direction == Direction.BottomToTop)
		{
			list.Reverse();
		}
		float num = 0f;
		foreach (Transform item2 in list)
		{
			UIWidget component = item2.GetComponent<UIWidget>();
			if (component != null)
			{
				SetPivot(component, direction == Direction.TopToBottom);
			}
			item2.localPosition = item2.localPosition.SetY(num);
			float num2 = NGUIMath.CalculateRelativeWidgetBounds(item2).size.y * item2.localScale.y + padding;
			num = ((direction != Direction.TopToBottom) ? (num + num2) : (num - num2));
		}
	}

	private void SetPivot(UIWidget widget, bool topToBottom)
	{
		if (widget.pivot == UIWidget.Pivot.TopLeft || widget.pivot == UIWidget.Pivot.Left || widget.pivot == UIWidget.Pivot.BottomLeft)
		{
			widget.pivot = ((!topToBottom) ? UIWidget.Pivot.BottomLeft : UIWidget.Pivot.TopLeft);
		}
		else if (widget.pivot == UIWidget.Pivot.Top || widget.pivot == UIWidget.Pivot.Center || widget.pivot == UIWidget.Pivot.Bottom)
		{
			widget.pivot = ((!topToBottom) ? UIWidget.Pivot.Right : UIWidget.Pivot.Left);
		}
		else if (widget.pivot == UIWidget.Pivot.TopRight || widget.pivot == UIWidget.Pivot.Right || widget.pivot == UIWidget.Pivot.BottomRight)
		{
			widget.pivot = ((!topToBottom) ? UIWidget.Pivot.BottomRight : UIWidget.Pivot.TopRight);
		}
	}
}

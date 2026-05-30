using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/CMune Extensions/Horizontal Aligner")]
public class UIHorizontalAligner : MonoBehaviour
{
	public enum Direction
	{
		LeftToRight = 0,
		RightToLeft = 1
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
		if (direction == Direction.RightToLeft)
		{
			list.Reverse();
		}
		float num = 0f;
		foreach (Transform item2 in list)
		{
			UIWidget component = item2.GetComponent<UIWidget>();
			if (component != null)
			{
				SetPivot(component, direction == Direction.LeftToRight);
			}
			item2.localPosition = item2.localPosition.SetX(num);
			float num2 = NGUIMath.CalculateRelativeWidgetBounds(item2).size.x * item2.localScale.x + padding;
			num = ((direction != Direction.LeftToRight) ? (num - num2) : (num + num2));
		}
	}

	private void SetPivot(UIWidget widget, bool leftToRight)
	{
		if (widget.pivot == UIWidget.Pivot.TopLeft || widget.pivot == UIWidget.Pivot.Top || widget.pivot == UIWidget.Pivot.TopRight)
		{
			widget.pivot = ((!leftToRight) ? UIWidget.Pivot.TopRight : UIWidget.Pivot.TopLeft);
		}
		else if (widget.pivot == UIWidget.Pivot.Left || widget.pivot == UIWidget.Pivot.Center || widget.pivot == UIWidget.Pivot.Right)
		{
			widget.pivot = ((!leftToRight) ? UIWidget.Pivot.Right : UIWidget.Pivot.Left);
		}
		else if (widget.pivot == UIWidget.Pivot.BottomLeft || widget.pivot == UIWidget.Pivot.Bottom || widget.pivot == UIWidget.Pivot.BottomRight)
		{
			widget.pivot = ((!leftToRight) ? UIWidget.Pivot.BottomRight : UIWidget.Pivot.BottomLeft);
		}
	}
}

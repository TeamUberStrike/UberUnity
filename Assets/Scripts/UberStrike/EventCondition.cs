using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventCondition
{
	public List<EventConditionEntry> conditions = new List<EventConditionEntry>();

	public bool Test(Animator animator)
	{
		if (conditions.Count == 0)
		{
			return true;
		}
		foreach (EventConditionEntry condition in conditions)
		{
			if (string.IsNullOrEmpty(condition.conditionParam))
			{
				continue;
			}
			switch (condition.conditionParamType)
			{
			case EventConditionParamTypes.Int:
			{
				int integer = animator.GetInteger(condition.conditionParam);
				switch (condition.conditionMode)
				{
				case EventConditionModes.Equal:
					if (integer != condition.intValue)
					{
						return false;
					}
					break;
				case EventConditionModes.NotEqual:
					if (integer == condition.intValue)
					{
						return false;
					}
					break;
				case EventConditionModes.GreaterThan:
					if (integer <= condition.intValue)
					{
						return false;
					}
					break;
				case EventConditionModes.LessThan:
					if (integer >= condition.intValue)
					{
						return false;
					}
					break;
				case EventConditionModes.GreaterEqualThan:
					if (integer < condition.intValue)
					{
						return false;
					}
					break;
				case EventConditionModes.LessEqualThan:
					if (integer > condition.intValue)
					{
						return false;
					}
					break;
				}
				break;
			}
			case EventConditionParamTypes.Float:
			{
				float num = animator.GetFloat(condition.conditionParam);
				switch (condition.conditionMode)
				{
				case EventConditionModes.GreaterThan:
					if (num <= condition.floatValue)
					{
						return false;
					}
					break;
				case EventConditionModes.LessThan:
					if (num >= condition.floatValue)
					{
						return false;
					}
					break;
				}
				break;
			}
			case EventConditionParamTypes.Boolean:
			{
				bool flag = animator.GetBool(condition.conditionParam);
				if (flag != condition.boolValue)
				{
					return false;
				}
				break;
			}
			}
		}
		return true;
	}
}

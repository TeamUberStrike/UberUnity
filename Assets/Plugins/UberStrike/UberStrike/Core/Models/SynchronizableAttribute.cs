using System;

namespace UberStrike.Core.Models
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class SynchronizableAttribute : Attribute
	{
	}
}

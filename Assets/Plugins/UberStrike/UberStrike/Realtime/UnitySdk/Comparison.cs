using System.Collections;

namespace UberStrike.Realtime.UnitySdk
{
	public static class Comparison
	{
		public static bool IsEqual(object a, object b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a is ICollection && b is ICollection)
			{
				return IsSequenceEqual(a as ICollection, b as ICollection);
			}
			return a.Equals(b);
		}

		private static bool IsSequenceEqual(ICollection a1, ICollection a2)
		{
			if (a1 != null && a2 != null)
			{
				bool flag = true;
				IEnumerator enumerator = a1.GetEnumerator();
				IEnumerator enumerator2 = a2.GetEnumerator();
				while (flag && enumerator.MoveNext() && enumerator2.MoveNext())
				{
					flag = ((!(enumerator.Current is ICollection) || !(enumerator2.Current is ICollection)) ? (enumerator.Current != null && enumerator.Current.Equals(enumerator2.Current)) : IsSequenceEqual(enumerator.Current as ICollection, enumerator2.Current as ICollection));
				}
				return flag;
			}
			return false;
		}
	}
}

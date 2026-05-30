using System;
using System.Collections.Generic;

namespace UberStrike.Core.Models
{
	[Serializable]
	public class DamageEvent
	{
		public Dictionary<byte, byte> Damage { get; set; }

		public byte BodyPartFlag { get; set; }

		public int DamageEffectFlag { get; set; }

		public float DamgeEffectValue { get; set; }

		public int Count
		{
			get
			{
				return (Damage != null) ? Damage.Count : 0;
			}
		}

		public void Clear()
		{
			if (Damage == null)
			{
				Damage = new Dictionary<byte, byte>();
			}
			BodyPartFlag = 0;
			Damage.Clear();
		}

		public void AddDamage(byte angle, short damage, byte bodyPart, int damageEffectFlag, float damageEffectValue)
		{
			if (Damage == null)
			{
				Damage = new Dictionary<byte, byte>();
			}
			if (Damage.ContainsKey(angle))
			{
				Dictionary<byte, byte> damage2;
				Dictionary<byte, byte> dictionary = (damage2 = Damage);
				byte key2;
				byte key = (key2 = angle);
				key2 = damage2[key2];
				dictionary[key] = (byte)(key2 + (byte)damage);
			}
			else
			{
				Damage[angle] = (byte)damage;
			}
			BodyPartFlag |= bodyPart;
			DamageEffectFlag = damageEffectFlag;
			DamgeEffectValue = damageEffectValue;
		}
	}
}

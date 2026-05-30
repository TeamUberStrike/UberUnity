using System;
using System.Collections;
using UberStrike.Core.Types;
using UnityEngine;

public class HUDAmmoBar : MonoBehaviour
{
	[SerializeField]
	private UIPanel panel;

	[SerializeField]
	private UISprite bar;

	[SerializeField]
	private UISprite bgr;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel text;

	[SerializeField]
	private float animateSpeed = 200f;

	[SerializeField]
	private float PulseSpeed = 20f;

	[SerializeField]
	private float PulseScale = 7f;

	private float oldValue = -1f;

	private float baseWidth;

	private float baseScale;

	private void OnEnable()
	{
		GameState.Current.PlayerData.ActiveWeapon.Fire();
	}

	private void Start()
	{
		baseWidth = bgr.transform.localScale.x;
		baseScale = text.transform.localScale.x;
		GameState.Current.PlayerData.ActiveWeapon.AddEventAndFire(delegate(WeaponSlot el)
		{
			if (el != null)
			{
				WeaponSlot currentWeapon = Singleton<WeaponController>.Instance.GetCurrentWeapon();
				oldValue = AmmoDepot.AmmoOfClass(currentWeapon.View.ItemClass);
				OnChanged();
			}
		}, this);
		GameState.Current.PlayerData.Ammo.AddEvent((Action<int>)delegate
		{
			OnChanged();
		}, (MonoBehaviour)this);
	}

	private void OnChanged()
	{
		WeaponSlot currentWeapon = Singleton<WeaponController>.Instance.GetCurrentWeapon();
		bool flag = currentWeapon != null && currentWeapon.View.ItemClass != UberstrikeItemClass.WeaponMelee;
		panel.alpha = (flag ? 1 : 0);
		if (flag)
		{
			int num = AmmoDepot.AmmoOfClass(currentWeapon.View.ItemClass);
			int maxValue = AmmoDepot.MaxAmmoOfClass(currentWeapon.View.ItemClass);
			StopAllCoroutines();
			if ((float)num != oldValue)
			{
				StartCoroutine(PulseCrt((float)num >= oldValue));
			}
			StartCoroutine(AnimateCrt(num, maxValue));
		}
	}

	private IEnumerator AnimateCrt(int value, int maxValue)
	{
		panel.alpha = 1f;
		do
		{
			oldValue = Mathf.MoveTowards(oldValue, value, Time.deltaTime * animateSpeed);
			bgr.transform.localScale = bgr.transform.localScale.SetX(Mathf.Max(maxValue, oldValue) / (float)maxValue * baseWidth);
			bar.transform.localScale = bgr.transform.localScale.SetX(oldValue / (float)maxValue * baseWidth);
			text.text = Mathf.FloorToInt(oldValue).ToString();
			yield return 0;
		}
		while ((float)value != oldValue);
	}

	private IEnumerator PulseCrt(bool up)
	{
		float time = 0f;
		while (true)
		{
			time = Mathf.Min(time + Time.deltaTime * PulseSpeed, (float)Math.PI);
			float pulse = Mathf.Sin(time) * PulseScale;
			text.transform.localScale = (Vector3.one * (baseScale + pulse * (float)(up ? 1 : (-1)))).SetZ(1f);
			if (time >= (float)Math.PI)
			{
				break;
			}
			yield return 0;
		}
	}
}

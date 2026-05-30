using System.Collections.Generic;
using UberStrike.Core.Types;
using UnityEngine;

public class QuickItemSfxController : Singleton<QuickItemSfxController>
{
	private Dictionary<QuickItemLogic, QuickItemSfx> _effects;

	private Dictionary<int, QuickItemSfx> _curShownEffects;

	private int _sfxId;

	private int NextSfxId
	{
		get
		{
			return ++_sfxId;
		}
	}

	private QuickItemSfxController()
	{
		_effects = new Dictionary<QuickItemLogic, QuickItemSfx>();
		_curShownEffects = new Dictionary<int, QuickItemSfx>();
	}

	public void RegisterQuickItemEffect(QuickItemLogic behaviour, QuickItemSfx effect)
	{
		if ((bool)effect)
		{
			_effects[behaviour] = effect;
		}
		else
		{
			Debug.LogError("QuickItemSfx is null: " + behaviour);
		}
	}

	public void ShowThirdPersonEffect(CharacterConfig player, QuickItemLogic effectType, int robotLifeTime, int scrapsLifeTime, bool isInstant = false)
	{
		robotLifeTime = ((robotLifeTime <= 0) ? 5000 : robotLifeTime);
		scrapsLifeTime = ((scrapsLifeTime <= 0) ? 3000 : scrapsLifeTime);
		QuickItemSfx value;
		if (_effects.TryGetValue(effectType, out value))
		{
			QuickItemSfx quickItemSfx = Object.Instantiate(value) as QuickItemSfx;
			quickItemSfx.ID = NextSfxId;
			if ((bool)quickItemSfx)
			{
				_curShownEffects.Add(quickItemSfx.ID, quickItemSfx);
				quickItemSfx.transform.parent = player.transform;
				quickItemSfx.transform.localRotation = Quaternion.AngleAxis(-45f, Vector3.up);
				quickItemSfx.transform.localPosition = new Vector3(0f, 0.2f, 0f);
				quickItemSfx.Play(robotLifeTime, scrapsLifeTime, isInstant);
				LayerUtil.SetLayerRecursively(quickItemSfx.transform, UberstrikeLayer.IgnoreRaycast);
			}
		}
		else
		{
			Debug.LogError("Failed to get effect: " + effectType);
		}
	}

	public void RemoveEffect(int id)
	{
		QuickItemSfx value;
		if (_curShownEffects.TryGetValue(id, out value))
		{
			_curShownEffects.Remove(id);
		}
	}

	public void DestroytSfxFromPlayer(byte playerNumber)
	{
		foreach (KeyValuePair<int, QuickItemSfx> curShownEffect in _curShownEffects)
		{
			if ((curShownEffect.Key & 0xFF) == playerNumber)
			{
				curShownEffect.Value.Destroy();
				_curShownEffects.Remove(curShownEffect.Key);
				break;
			}
		}
	}

	private static int CreateGlobalSfxID(byte playerNumber, int sfxId)
	{
		return (sfxId << 8) + playerNumber;
	}
}

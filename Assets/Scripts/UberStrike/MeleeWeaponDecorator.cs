using System.Collections;
using UnityEngine;

public sealed class MeleeWeaponDecorator : BaseWeaponDecorator
{
	[SerializeField]
	private Animation _animation;

	[SerializeField]
	private AnimationClip[] _shootAnimClips;

	[SerializeField]
	private AudioClip[] _impactSounds;

	[SerializeField]
	private AudioClip _equipSound;

	protected override void Awake()
	{
		base.Awake();
		base.IsMelee = true;
	}

	public override void ShowShootEffect(RaycastHit[] hits)
	{
		base.ShowShootEffect(hits);
		if (base.EnableShootAnimation && (bool)_animation && _shootAnimClips.Length > 0)
		{
			int num = Random.Range(0, _shootAnimClips.Length);
			_animation.clip = _shootAnimClips[num];
			_animation.Rewind();
			_animation.Play();
		}
	}

	public override void PlayHitSound()
	{
	}

	public override void PlayEquipSound()
	{
		if ((bool)_mainAudioSource && (bool)_equipSound)
		{
			_mainAudioSource.volume = ((!ApplicationDataManager.ApplicationOptions.AudioEnabled) ? 0f : ApplicationDataManager.ApplicationOptions.AudioEffectsVolume);
			_mainAudioSource.PlayOneShot(_equipSound);
		}
	}

	protected override void EmitImpactSound(string impactType, Vector3 position)
	{
		if (_impactSounds != null && _impactSounds.Length > 0)
		{
			int num = Random.Range(0, _impactSounds.Length);
			AudioClip audioClip = _impactSounds[num];
			if ((bool)audioClip)
			{
				AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(audioClip, position, 1f);
			}
			else
			{
				Debug.LogError("Missing impact sound for melee weapon!");
			}
		}
		else
		{
			Debug.LogError("Melee impact sound is not signed!");
		}
	}

	protected override void ShowImpactEffects(RaycastHit hit, Vector3 direction, Vector3 muzzlePosition, float distance, bool playSound)
	{
		StartCoroutine(StartShowImpactEffects(hit, direction, muzzlePosition, distance, playSound));
	}

	private IEnumerator StartShowImpactEffects(RaycastHit hit, Vector3 direction, Vector3 muzzlePosition, float distance, bool playSound)
	{
		yield return new WaitForSeconds(0.2f);
		EmitImpactParticles(hit, direction, muzzlePosition, distance, playSound);
	}
}

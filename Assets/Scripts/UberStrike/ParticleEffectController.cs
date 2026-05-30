using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{
	[Serializable]
	private class ParticleConfiguration
	{
		[HideInInspector]
		public string Name = "Effect";

		public ParticleConfigurationType Type;

		public ParticleConfigurationPerWeapon Configuration;

		public ParticleConfiguration(string name, ParticleConfigurationType type, ParticleConfigurationPerWeapon configuration)
		{
			Name = name;
			Type = type;
			Configuration = configuration;
		}
	}

	[SerializeField]
	private ParticleConfiguration[] _allWeaponData;

	[SerializeField]
	private ParticleSystem _pickupParticleEmitter;

	[SerializeField]
	private HeatWave _heatWavePrefab;

	[SerializeField]
	private ParticleSystem _heatWave;

	[SerializeField]
	private ParticleSystem _jumpEffect;

	private Dictionary<ParticleConfigurationType, ParticleConfigurationPerWeapon> _allConfigurations;

	private static Dictionary<Vector3, float> _effects = new Dictionary<Vector3, float>();

	private static float _nextCleanup;

	private ExplosionController _explosionParticleSystem;

	public static ParticleEffectController Instance { get; private set; }

	public static bool Exists
	{
		get
		{
			return Instance != null;
		}
	}

	private void Awake()
	{
		Instance = this;
		_explosionParticleSystem = new ExplosionController();
		_allConfigurations = new Dictionary<ParticleConfigurationType, ParticleConfigurationPerWeapon>();
		ParticleConfiguration[] allWeaponData = _allWeaponData;
		foreach (ParticleConfiguration particleConfiguration in allWeaponData)
		{
			_allConfigurations[particleConfiguration.Type] = particleConfiguration.Configuration;
		}
		Singleton<ExplosionManager>.Instance.HeatWavePrefab = _heatWavePrefab;
	}

	public static void ShowJumpEffect(Vector3 pos, Vector2 normal)
	{
		if ((bool)Instance)
		{
			Instance._jumpEffect.transform.position = pos;
			Instance._jumpEffect.Emit(5);
		}
	}

	public static void ShowPickUpEffect(Vector3 pos, int count)
	{
		if ((bool)Instance)
		{
			Instance._pickupParticleEmitter.transform.position = pos;
			Instance._pickupParticleEmitter.Emit(count);
		}
	}

	public static void ShowHeatwaveEffect(Vector3 pos)
	{
		if ((bool)Instance && (bool)Instance._heatWave && !ApplicationDataManager.IsMobile)
		{
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
			{
				position = pos,
				velocity = Vector3.zero,
				startSize = 1f,
				startLifetime = 1f,
				startColor = Color.white
			};
			Instance._heatWave.Emit(emitParams, 1);
		}
	}

	public static void ShowHitEffect(ParticleConfigurationType effectType, SurfaceEffectType surface, Vector3 direction, Vector3 hitPoint, Vector3 hitNormal, Vector3 muzzlePosition, float distance, ref MoveTrailrendererObject trailRenderer, Transform parent)
	{
		ShowHitEffect(effectType, surface, direction, hitPoint, hitNormal, muzzlePosition, distance, ref trailRenderer, parent, 0);
	}

	public static void ShowHitEffect(ParticleConfigurationType effectType, SurfaceEffectType surface, Vector3 direction, Vector3 hitPoint, Vector3 hitNormal, Vector3 muzzlePosition, float distance, ref MoveTrailrendererObject trailRenderer, Transform parent, int damage)
	{
		if (Exists)
		{
			ParticleConfigurationPerWeapon particleConfigurationPerWeapon = Instance._allConfigurations[effectType];
			if (!(particleConfigurationPerWeapon != null))
			{
				return;
			}
			ShowTrailEffect(particleConfigurationPerWeapon, trailRenderer, parent, hitPoint, muzzlePosition, distance, direction);
			switch (surface)
			{
			case SurfaceEffectType.WoodEffect:
				if (CheckVisibility(hitPoint))
				{
					ParticleEmissionSystem.HitMaterialParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.WoodEffect);
					ParticleEmissionSystem.FireParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.FireParticleConfigurationForInstantHit);
				}
				break;
			case SurfaceEffectType.StoneEffect:
				if (CheckVisibility(hitPoint))
				{
					ParticleEmissionSystem.HitMaterialParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.StoneEffect);
					ParticleEmissionSystem.FireParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.FireParticleConfigurationForInstantHit);
				}
				break;
			case SurfaceEffectType.MetalEffect:
				if (CheckVisibility(hitPoint))
				{
					ParticleEmissionSystem.HitMaterialParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.MetalEffect);
					ParticleEmissionSystem.FireParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.FireParticleConfigurationForInstantHit);
				}
				break;
			case SurfaceEffectType.WaterEffect:
				if (CheckVisibility(hitPoint))
				{
					ParticleEmissionSystem.WaterCircleParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.WaterCircleEffect);
				}
				break;
			case SurfaceEffectType.GrassEffect:
				if (CheckVisibility(hitPoint))
				{
					ParticleEmissionSystem.HitMaterialParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.GrassEffect);
					ParticleEmissionSystem.FireParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.FireParticleConfigurationForInstantHit);
				}
				break;
			case SurfaceEffectType.SandEffect:
				if (CheckVisibility(hitPoint))
				{
					ParticleEmissionSystem.HitMaterialParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.SandEffect);
					ParticleEmissionSystem.FireParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.FireParticleConfigurationForInstantHit);
				}
				break;
			case SurfaceEffectType.Splat:
				if (CheckVisibility(hitPoint))
				{
					ParticleEmissionSystem.HitMaterialRotatingParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.Splat);
				}
				break;
			case SurfaceEffectType.Default:
				if (CheckVisibility(hitPoint))
				{
					ParticleEmissionSystem.FireParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.FireParticleConfigurationForInstantHit);
				}
				break;
			}
		}
		else
		{
			Debug.LogError("ParticleEffectController is not attached to a gameobject in scene!");
		}
	}

	private static void ShowTrailEffect(ParticleConfigurationPerWeapon effect, MoveTrailrendererObject trailRenderer, Transform parent, Vector3 hitPoint, Vector3 muzzlePosition, float distance, Vector3 direction)
	{
		if (effect.WeaponImpactEffectConfiguration.UseTrailrendererForTrail)
		{
			if (effect.WeaponImpactEffectConfiguration.TrailrendererTrailPrefab != null)
			{
				if (trailRenderer == null)
				{
					trailRenderer = UnityEngine.Object.Instantiate(effect.WeaponImpactEffectConfiguration.TrailrendererTrailPrefab, muzzlePosition, Quaternion.identity) as MoveTrailrendererObject;
					trailRenderer.gameObject.transform.parent = parent;
				}
				trailRenderer.MoveTrail(hitPoint, muzzlePosition, distance);
			}
		}
		else
		{
			ParticleEmissionSystem.TrailParticles(hitPoint, direction, effect.WeaponImpactEffectConfiguration.TrailParticleConfigurationForInstantHit, muzzlePosition, distance);
		}
	}

	public static void ShowExplosionEffect(ParticleConfigurationType effectType, SurfaceEffectType surface, Vector3 hitPoint, Vector3 hitNormal)
	{
		if (!Exists || !CheckVisibility(hitPoint))
		{
			return;
		}
		ParticleConfigurationPerWeapon particleConfigurationPerWeapon = Instance._allConfigurations[effectType];
		bool flag = false;
		if (particleConfigurationPerWeapon != null)
		{
			switch (surface)
			{
			case SurfaceEffectType.WoodEffect:
				ParticleEmissionSystem.HitMateriaHalfSphericParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.WoodEffect);
				break;
			case SurfaceEffectType.WaterEffect:
				ParticleEmissionSystem.WaterCircleParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.WaterCircleEffect);
				ParticleEmissionSystem.WaterSplashParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.WaterExtraSplashEffect);
				break;
			case SurfaceEffectType.StoneEffect:
				ParticleEmissionSystem.HitMateriaHalfSphericParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.StoneEffect);
				break;
			case SurfaceEffectType.MetalEffect:
				ParticleEmissionSystem.HitMateriaHalfSphericParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.MetalEffect);
				break;
			case SurfaceEffectType.GrassEffect:
				ParticleEmissionSystem.HitMateriaHalfSphericParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.GrassEffect);
				break;
			case SurfaceEffectType.SandEffect:
				ParticleEmissionSystem.HitMateriaHalfSphericParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.SandEffect);
				break;
			case SurfaceEffectType.Splat:
				ParticleEmissionSystem.HitMateriaFullSphericParticles(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.Splat);
				break;
			}
			bool flag2 = QualitySettings.GetQualityLevel() > 0;
			if (flag2)
			{
				Instance._explosionParticleSystem.EmitDust(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.ExplosionParameterSet.DustParameters);
				Instance._explosionParticleSystem.EmitSmoke(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.ExplosionParameterSet.SmokeParameters);
			}
			if (flag2 || flag)
			{
				Instance._explosionParticleSystem.EmitTrail(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.ExplosionParameterSet.TrailParameters);
			}
			Instance._explosionParticleSystem.EmitBlast(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.ExplosionParameterSet.BlastParameters);
			Instance._explosionParticleSystem.EmitRing(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.ExplosionParameterSet.RingParameters);
			Instance._explosionParticleSystem.EmitSpark(hitPoint, hitNormal, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.ExplosionParameterSet.SparkParameters);
		}
	}

	private static void WaterRipplesEffect(ParticleConfigurationPerWeapon effect, Vector3 hitPoint, Vector3 direction, Vector3 muzzlePosition, float distance)
	{
		float num = Math.Abs(muzzlePosition.y) * distance / (Math.Abs(hitPoint.y) + Math.Abs(muzzlePosition.y));
		Vector3 vector = direction * num + muzzlePosition;
		if (CanPlayEffectAt(vector) && CheckVisibility(vector))
		{
			ParticleEmissionSystem.WaterSplashParticles(vector, Vector3.up, effect.WeaponImpactEffectConfiguration.SurfaceParameterSet.WaterExtraSplashEffect);
			ParticleEmissionSystem.WaterCircleParticles(vector, Vector3.up, effect.WeaponImpactEffectConfiguration.SurfaceParameterSet.WaterCircleEffect);
		}
	}

	private static Vector3 PositionRaster(Vector3 v)
	{
		return new Vector3(Mathf.RoundToInt(v[0]), Mathf.RoundToInt(v[1]), Mathf.RoundToInt(v[2]));
	}

	private static bool CanPlayEffectAt(Vector3 v)
	{
		if (_nextCleanup < Time.time)
		{
			_nextCleanup = Time.time + 30f;
			_effects.Clear();
		}
		Vector3 key = PositionRaster(v);
		float value;
		if (!_effects.TryGetValue(key, out value) || value < Time.time)
		{
			_effects[key] = Time.time + 1f;
			return true;
		}
		return false;
	}

	public static void ProjectileWaterRipplesEffect(ParticleConfigurationType effectType, Vector3 hitPosition)
	{
		if (Exists && GameState.Current.Map != null)
		{
			ParticleConfigurationPerWeapon particleConfigurationPerWeapon = Instance._allConfigurations[effectType];
			if (particleConfigurationPerWeapon != null)
			{
				ParticleEmissionSystem.WaterSplashParticles(hitPosition, Vector3.up, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.WaterExtraSplashEffect);
				ParticleEmissionSystem.WaterCircleParticles(hitPosition, Vector3.up, particleConfigurationPerWeapon.WeaponImpactEffectConfiguration.SurfaceParameterSet.WaterCircleEffect);
			}
		}
	}

	private static bool CheckVisibility(Vector3 hitPoint)
	{
		return true;
	}
}

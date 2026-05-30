using UnityEngine;

public class ExplosionManager : Singleton<ExplosionManager>
{
	public HeatWave HeatWavePrefab { get; set; }

	private ExplosionManager()
	{
	}

	public void PlayExplosionSound(Vector3 point, AudioClip clip)
	{
		if (GameState.Current.Map != null && GameState.Current.Map.HasWaterPlane && GameState.Current.Map.WaterPlaneHeight > point.y)
		{
			clip = ((Random.Range(0, 2) != 0) ? GameAudio.UnderwaterExplosion2 : GameAudio.UnderwaterExplosion1);
		}
		if (clip != null)
		{
			AutoMonoBehaviour<SfxManager>.Instance.Play3dAudioClip(clip, point, 1f);
		}
	}

	public void ShowExplosionEffect(Vector3 point, Vector3 normal, string tag, ParticleConfigurationType effectType)
	{
		switch (tag)
		{
		case "Wood":
			ParticleEffectController.ShowExplosionEffect(effectType, SurfaceEffectType.WoodEffect, point, normal);
			break;
		case "Stone":
			ParticleEffectController.ShowExplosionEffect(effectType, SurfaceEffectType.StoneEffect, point, normal);
			break;
		case "Metal":
			ParticleEffectController.ShowExplosionEffect(effectType, SurfaceEffectType.MetalEffect, point, normal);
			break;
		case "Sand":
			ParticleEffectController.ShowExplosionEffect(effectType, SurfaceEffectType.SandEffect, point, normal);
			break;
		case "Grass":
			ParticleEffectController.ShowExplosionEffect(effectType, SurfaceEffectType.GrassEffect, point, normal);
			break;
		case "Avatar":
			ParticleEffectController.ShowExplosionEffect(effectType, SurfaceEffectType.Splat, point, normal);
			break;
		case "Water":
			ParticleEffectController.ShowExplosionEffect(effectType, SurfaceEffectType.WaterEffect, point, normal);
			break;
		default:
			ParticleEffectController.ShowExplosionEffect(effectType, SurfaceEffectType.Default, point, normal);
			break;
		}
	}
}

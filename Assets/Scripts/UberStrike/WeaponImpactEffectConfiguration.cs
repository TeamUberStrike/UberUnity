using System;

[Serializable]
public class WeaponImpactEffectConfiguration
{
	public ExplosionParameterSet ExplosionParameterSet;

	public FireParticleConfiguration FireParticleConfigurationForInstantHit;

	public TrailParticleConfiguration TrailParticleConfigurationForInstantHit;

	public SurfaceParameters SurfaceParameterSet;

	public MoveTrailrendererObject TrailrendererTrailPrefab;

	public bool UseTrailrendererForTrail;
}

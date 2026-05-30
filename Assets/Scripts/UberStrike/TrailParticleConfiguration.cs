using System;
using UnityEngine;

[Serializable]
public class TrailParticleConfiguration
{
	public float ParticleMinSize;

	public float ParticleMaxSize;

	public float ParticleMinLiveTime;

	public float ParticleMaxLiveTime;

	public Color ParticleColor;

	public ParticleSystem ParticleSystem;
}

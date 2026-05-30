using System;
using UnityEngine;

[Serializable]
public class ParticleConfiguration
{
	public float ParticleMinSize;

	public float ParticleMaxSize;

	public int ParticleCount;

	public float ParticleMinSpeed;

	public float ParticleMaxSpeed;

	public float ParticleMinLiveTime;

	public float ParticleMaxLiveTime;

	public float ParticleMinZVelocity;

	public float ParticleMaxZVelocity;

	public Color ParticleColor;

	public ParticleSystem ParticleSystem;
}

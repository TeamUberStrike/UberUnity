using System;
using UnityEngine;

[Serializable]
public class FireParticleConfiguration
{
	public float ParticleMinSize;

	public float ParticleMaxSize;

	public int ParticleCount;

	public float ParticleMinLiveTime;

	public float ParticleMaxLiveTime;

	public Color ParticleColor;

	public ParticleSystem ParticleSystem;
}

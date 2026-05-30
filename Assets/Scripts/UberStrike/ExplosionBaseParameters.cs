using System;
using UnityEngine;

[Serializable]
public class ExplosionBaseParameters
{
	public int ParticleCount;

	public float MinLifeTime;

	public float MaxLifeTime;

	public float MinSize;

	public float MaxSize;

	public ParticleSystem ParticleSystem;
}

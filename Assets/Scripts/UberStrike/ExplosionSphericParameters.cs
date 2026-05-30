using System;
using UnityEngine;

[Serializable]
public class ExplosionSphericParameters
{
	public int ParticleCount;

	public float MinLifeTime;

	public float MaxLifeTime;

	public float MinSize;

	public float MaxSize;

	public float Speed;

	public ParticleSystem ParticleSystem;
}

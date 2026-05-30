using System;
using UnityEngine;

[Serializable]
public class ExplosionDustParameters
{
	public int ParticleCount;

	public float MinStartPositionSize;

	public float MaxStartPositionSize;

	public float MinLifeTime;

	public float MaxLifeTime;

	public float MinSize;

	public float MaxSize;

	public ParticleSystem ParticleSystem;
}

using UnityEngine;

public class ExplosionController
{
	private static void EmitParticle(ParticleSystem particleSystem, Vector3 position, Vector3 velocity, float size, float lifetime, Color color)
	{
		ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
		{
			position = position,
			velocity = velocity,
			startSize = size,
			startLifetime = lifetime,
			startColor = color
		};
		particleSystem.Emit(emitParams, 1);
	}

	public void EmitBlast(Vector3 hitPoint, Vector3 hitNormal, ExplosionBaseParameters parameters)
	{
		Vector3 zero = Vector3.zero;
		if (parameters.ParticleSystem != null)
		{
			for (int i = 0; i < parameters.ParticleCount; i++)
			{
				float size = Random.Range(parameters.MinSize, parameters.MaxSize);
				float energy = Random.Range(parameters.MinLifeTime, parameters.MaxLifeTime);
				EmitParticle(parameters.ParticleSystem, hitPoint, zero, size, energy, Color.red);
			}
		}
	}

	public void EmitDust(Vector3 hitPoint, Vector3 hitNormal, ExplosionDustParameters parameters)
	{
		Vector3 zero = Vector3.zero;
		if (parameters.ParticleSystem != null)
		{
			for (int i = 0; i < parameters.ParticleCount; i++)
			{
				zero = Random.insideUnitSphere * 0.2f;
				hitPoint += Random.insideUnitSphere * Random.Range(parameters.MinStartPositionSize, parameters.MinStartPositionSize);
				float size = Random.Range(parameters.MinSize, parameters.MaxSize);
				float energy = Random.Range(parameters.MinLifeTime, parameters.MaxLifeTime);
				EmitParticle(parameters.ParticleSystem, hitPoint, zero, size, energy, Color.red);
			}
		}
	}

	public void EmitRing(Vector3 hitPoint, Vector3 hitNormal, ExplosionRingParameters parameters)
	{
		Vector3 zero = Vector3.zero;
		float startSize = parameters.StartSize;
		float energy = Random.Range(parameters.MinLifeTime, parameters.MaxLifeTime);
		if (parameters.ParticleSystem != null)
		{
			EmitParticle(parameters.ParticleSystem, hitPoint, zero, startSize, energy, Color.red);
		}
	}

	public void EmitSmoke(Vector3 hitPoint, Vector3 hitNormal, ExplosionBaseParameters parameters)
	{
		Vector3 zero = Vector3.zero;
		if (parameters.ParticleSystem != null)
		{
			for (int i = 0; i < parameters.ParticleCount; i++)
			{
				float size = Random.Range(parameters.MinSize, parameters.MaxSize);
				float energy = Random.Range(parameters.MinLifeTime, parameters.MaxLifeTime);
				zero = Random.insideUnitSphere * 0.3f;
				EmitParticle(parameters.ParticleSystem, hitPoint, zero, size, energy, Color.red);
			}
		}
	}

	public void EmitSpark(Vector3 hitPoint, Vector3 hitNormal, ExplosionSphericParameters parameters)
	{
		Vector3 zero = Vector3.zero;
		if (parameters.ParticleSystem != null)
		{
			for (int i = 0; i < parameters.ParticleCount; i++)
			{
				float size = Random.Range(parameters.MinSize, parameters.MaxSize);
				float energy = Random.Range(parameters.MinLifeTime, parameters.MaxLifeTime);
				zero = Random.insideUnitSphere * parameters.Speed;
				EmitParticle(parameters.ParticleSystem, hitPoint, zero, size, energy, Color.red);
			}
		}
	}

	public void EmitTrail(Vector3 hitPoint, Vector3 hitNormal, ExplosionSphericParameters parameters)
	{
		Vector3 zero = Vector3.zero;
		if (parameters.ParticleSystem != null)
		{
			for (int i = 0; i < parameters.ParticleCount; i++)
			{
				float size = Random.Range(parameters.MinSize, parameters.MaxSize);
				float energy = Random.Range(parameters.MinLifeTime, parameters.MaxLifeTime);
				zero = Random.insideUnitSphere * parameters.Speed;
				EmitParticle(parameters.ParticleSystem, hitPoint, zero, size, energy, Color.red);
			}
		}
	}
}

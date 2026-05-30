using UnityEngine;

public class GrenadeProjectile : Projectile
{
	public bool Sticky { get; set; }

	protected override void Start()
	{
		base.Start();
		if (base.Detonator != null)
		{
			base.Detonator.Direction = Vector3.zero;
		}
		base.Rigidbody.useGravity = true;
		base.Rigidbody.AddRelativeTorque(Random.insideUnitSphere.normalized * 10f);
	}

	protected override void OnTriggerEnter(Collider c)
	{
		if (!base.IsProjectileExploded)
		{
			if (LayerUtil.IsLayerInMask(UberstrikeLayerMasks.GrenadeCollisionMask, c.gameObject.layer))
			{
				Singleton<ProjectileManager>.Instance.RemoveProjectile(ID);
				GameState.Current.Actions.RemoveProjectile(ID, true);
			}
			PlayBounceSound(c.transform.position);
		}
	}

	protected override void OnCollisionEnter(Collision c)
	{
		if (base.IsProjectileExploded)
		{
			return;
		}
		if (LayerUtil.IsLayerInMask(UberstrikeLayerMasks.GrenadeCollisionMask, c.gameObject.layer))
		{
			Singleton<ProjectileManager>.Instance.RemoveProjectile(ID);
			GameState.Current.Actions.RemoveProjectile(ID, true);
		}
		else if (Sticky)
		{
			base.Rigidbody.isKinematic = true;
			base.GetComponent<Collider>().isTrigger = true;
			if (c.contacts.Length > 0)
			{
				base.transform.position = c.contacts[0].point + c.contacts[0].normal * base.GetComponent<Collider>().bounds.extents.sqrMagnitude;
			}
		}
		PlayBounceSound(c.transform.position);
	}
}

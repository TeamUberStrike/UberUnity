using System.Collections;
using System.Collections.Generic;
using UberStrike.Core.Models;
using UnityEngine;

public class AvatarDecoratorConfig : MonoBehaviour
{
	[SerializeField]
	private AvatarBone[] _avatarBones;

	private Color _skinColor;

	private float _hangTime = 0.5f;

	private float _hangTimeDownwardForce = 4f;

	private void Awake()
	{
		_skinColor = Color.magenta;
		AvatarBone[] avatarBones = _avatarBones;
		foreach (AvatarBone avatarBone in avatarBones)
		{
			avatarBone.Collider = avatarBone.Transform.GetComponent<Collider>();
			avatarBone.Rigidbody = avatarBone.Transform.GetComponent<Rigidbody>();
			avatarBone.OriginalPosition = avatarBone.Transform.localPosition;
			avatarBone.OriginalRotation = avatarBone.Transform.localRotation;
		}
	}

	private void SetGravity(bool enabled)
	{
		AvatarBone[] avatarBones = _avatarBones;
		foreach (AvatarBone avatarBone in avatarBones)
		{
			if (avatarBone != null && (bool)avatarBone.Rigidbody)
			{
				avatarBone.Rigidbody.useGravity = enabled;
			}
		}
	}

	public void ApplyDamageToRagdoll(DamageInfo damageInfo)
	{
		GameObject gameObject = null;
		switch (damageInfo.BodyPart)
		{
		case BodyPart.Head:
			gameObject = GetBone(BoneIndex.Head).gameObject;
			break;
		case BodyPart.Body:
			gameObject = GetBone(BoneIndex.Spine).gameObject;
			break;
		case BodyPart.Nuts:
			gameObject = GetBone(BoneIndex.Hips).gameObject;
			break;
		}
		if (gameObject != null)
		{
			RagdollBodyPart component = gameObject.GetComponent<RagdollBodyPart>();
			if (component != null)
			{
				StartCoroutine(Die(component, damageInfo));
			}
			else
			{
				Debug.LogError(gameObject.name + " doesn't contain a RagdollBodyPart component.");
			}
		}
		else
		{
			Debug.LogError(string.Concat("Bone GameObject ", damageInfo.BodyPart, " was not found."));
		}
	}

	private IEnumerator Die(RagdollBodyPart ragdollBodyPart, DamageInfo damageInfo)
	{
		SetGravity(false);
		if (damageInfo.IsExplosion)
		{
			damageInfo.Force *= (float)damageInfo.Damage;
			damageInfo.UpwardsForceMultiplier = 10f;
		}
		ragdollBodyPart.ApplyDamage(damageInfo);
		float bTime = 0f;
		while (bTime < _hangTime)
		{
			bTime += Time.deltaTime;
			ragdollBodyPart.GetComponent<Rigidbody>().AddForce(Vector3.down * _hangTimeDownwardForce);
			yield return new WaitForEndOfFrame();
		}
		SetGravity(true);
	}

	public Color GetSkinColor()
	{
		return _skinColor;
	}

	public void SetSkinColor(Color skinColor)
	{
		SkinnedMeshRenderer componentInChildren = GetComponentInChildren<SkinnedMeshRenderer>();
		if (!(componentInChildren != null))
		{
			return;
		}
		_skinColor = skinColor;
		Material[] materials = componentInChildren.materials;
		foreach (Material material in materials)
		{
			if (material.name.Contains("Skin"))
			{
				material.color = skinColor;
			}
		}
	}

	public Transform GetBone(BoneIndex bone)
	{
		AvatarBone[] avatarBones = _avatarBones;
		foreach (AvatarBone avatarBone in avatarBones)
		{
			if (avatarBone.Bone == bone)
			{
				return avatarBone.Transform;
			}
		}
		return base.transform;
	}

	public void SetBones(List<AvatarBone> bones)
	{
		_avatarBones = bones.ToArray();
	}

	public static void CopyBones(AvatarDecoratorConfig srcAvatar, AvatarDecoratorConfig dstAvatar)
	{
		AvatarBone[] avatarBones = srcAvatar._avatarBones;
		foreach (AvatarBone avatarBone in avatarBones)
		{
			Transform bone = dstAvatar.GetBone(avatarBone.Bone);
			if ((bool)bone)
			{
				bone.position = avatarBone.Transform.position;
				bone.rotation = avatarBone.Transform.rotation;
			}
		}
	}
}

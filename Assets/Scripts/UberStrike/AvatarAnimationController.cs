using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AvatarAnimationController : MonoBehaviour
{
	public enum AnimationLayer
	{
		Base = 0,
		Weapons = 1,
		Shop = 2,
		Dance = 3
	}

	private class ControlFields
	{
		public static readonly int SpeedZ = Animator.StringToHash("SpeedZ");

		public static readonly int SpeedX = Animator.StringToHash("SpeedX");

		public static readonly int IsSquatting = Animator.StringToHash("IsSquatting");

		public static readonly int IsPaused = Animator.StringToHash("IsPaused");

		public static readonly int WalkingSpeed = Animator.StringToHash("WalkingSpeed");

		public static readonly int TurnAround = Animator.StringToHash("TurnAround");

		public static readonly int IsSwimming = Animator.StringToHash("IsSwimming");

		public static readonly int IsWalking = Animator.StringToHash("IsWalking");

		public static readonly int IsJumping = Animator.StringToHash("IsJumping");

		public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

		public static readonly int Direction = Animator.StringToHash("Direction");

		public static readonly int IsShooting = Animator.StringToHash("IsShooting");

		public static readonly int WeaponClass = Animator.StringToHash("WeaponClass");

		public static readonly int WeaponSwitch = Animator.StringToHash("WeaponSwitch");

		public static readonly int IsTurningLeft = Animator.StringToHash("IsTurningLeft");

		public static readonly int IsTurningRight = Animator.StringToHash("IsTurningRight");

		public static readonly int Random = Animator.StringToHash("Random");

		public static readonly int GearType = Animator.StringToHash("GearType");

		public static readonly int IsDance = Animator.StringToHash("IsDance");
	}

	private class AnimationStates
	{
		public static readonly int Shooting = Animator.StringToHash("Weapons.Shooting");

		public static readonly int Jump = Animator.StringToHash("Base.Jumping.Jump");

		public static readonly int Idle = Animator.StringToHash("Base.Idle");
	}

	private const float IK_FADE_IN_SPEED = 10f;

	private const float IK_FADE_OUT_SPEED = 15f;

	private const int TURN_THRESHOLD = 45;

	private Transform _AnchorChest;

	private Transform _IKAnchor;

	private Transform _IKLeftHand;

	private Transform _IKRightHand;

	private float _IKWeight;

	private float _LookAtWeight;

	private ICharacterState state;

	private int gearTrigger;

	private bool jumpTrigger;

	private bool shootTrigger;

	private bool weaponSwitch;

	private float nextRandomUpdate;

	private float turnAround;

	private int animationLayerMask = 6;

	public Animator Animator { get; private set; }

	private void Awake()
	{
		Animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		_AnchorChest = base.transform.Find("Hips/Spine/Chest/Anchor_Chest");
		_IKAnchor = base.transform.Find("IK_Anchor");
		if ((bool)_IKAnchor)
		{
			_IKRightHand = _IKAnchor.transform.Find("IK_Hand_R");
			_IKLeftHand = _IKAnchor.transform.Find("IK_Hand_R/IK_Hand_L");
		}
	}

	public void SetCharacter(ICharacterState state)
	{
		this.state = state;
	}

	public void Jump()
	{
		jumpTrigger = true;
	}

	public void Shoot()
	{
		shootTrigger = true;
	}

	public bool IsLayerEnabled(AnimationLayer layer)
	{
		return (animationLayerMask & (1 << (int)layer)) != 0;
	}

	public void EnableLayer(AnimationLayer layer, bool enable)
	{
		if (enable)
		{
			animationLayerMask |= 1 << (int)layer;
		}
		else
		{
			animationLayerMask &= ~(1 << (int)layer);
		}
	}

	private void Update()
	{
		Animator.SetInteger(ControlFields.GearType, gearTrigger);
		if (state != null)
		{
			float value = Vector3.Magnitude(new Vector3(state.Velocity.x, 0f, state.Velocity.z));
			bool value2 = false;
			bool value3 = false;
			if (Mathf.DeltaAngle(state.HorizontalRotation.eulerAngles.y, turnAround) > 45f)
			{
				value2 = true;
				turnAround = state.HorizontalRotation.eulerAngles.y;
			}
			else if (Mathf.DeltaAngle(state.HorizontalRotation.eulerAngles.y, turnAround) < -45f)
			{
				value3 = true;
				turnAround = state.HorizontalRotation.eulerAngles.y;
			}
			Vector3 vector = Quaternion.Inverse(state.HorizontalRotation) * state.Velocity;
			if (state.KeyState != KeyState.Still)
			{
				Vector3 zero = Vector3.zero;
				float value4 = 0f;
				if ((state.KeyState & KeyState.Forward) != KeyState.Still)
				{
					zero.z += 1f;
				}
				if ((state.KeyState & KeyState.Backward) != KeyState.Still)
				{
					zero.z -= 1f;
				}
				if ((state.KeyState & KeyState.Left) != KeyState.Still)
				{
					zero.x += 1f;
				}
				if ((state.KeyState & KeyState.Right) != KeyState.Still)
				{
					zero.x -= 1f;
				}
				zero.Normalize();
				if (zero.magnitude > 0f)
				{
					value4 = Quaternion.LookRotation(zero).eulerAngles.y;
				}
				Animator.SetFloat(ControlFields.Direction, value4, 0.2f, Time.fixedDeltaTime);
			}
			Animator.SetFloat(ControlFields.WalkingSpeed, value);
			Animator.SetFloat(ControlFields.SpeedZ, vector.z);
			Animator.SetFloat(ControlFields.SpeedX, vector.x);
			Animator.SetFloat(ControlFields.TurnAround, turnAround);
			Animator.SetBool(ControlFields.IsShooting, state.Player.IsFiring || shootTrigger);
			Animator.SetBool(ControlFields.IsGrounded, (state.MovementState & MoveStates.Grounded) != 0);
			Animator.SetBool(ControlFields.IsJumping, jumpTrigger);
			Animator.SetBool(ControlFields.IsPaused, state.Player.Is(PlayerStates.Paused));
			Animator.SetBool(ControlFields.IsSquatting, state.Is(MoveStates.Ducked));
			Animator.SetBool(ControlFields.IsWalking, (state.KeyState & KeyState.Walking) != 0);
			Animator.SetBool(ControlFields.IsSwimming, (state.MovementState & (MoveStates.Swimming | MoveStates.Diving)) != 0);
			Animator.SetBool(ControlFields.IsTurningLeft, value2);
			Animator.SetBool(ControlFields.IsTurningRight, value3);
			float num = state.VerticalRotation;
			if (num > 180f)
			{
				num -= 360f;
			}
			num = Mathf.Clamp(num, -70f, 70f);
			Vector3 localEulerAngles = _IKAnchor.transform.localEulerAngles;
			localEulerAngles.x = num;
			_IKAnchor.transform.localEulerAngles = localEulerAngles;
		}
		EnableLayer(AnimationLayer.Shop, !GameState.Current.IsMultiplayer);
		if (!GameState.Current.IsMultiplayer && !Animator.GetCurrentAnimatorStateInfo(2).IsTag("ShopIdle"))
		{
			EnableLayer(AnimationLayer.Weapons, false);
		}
		else
		{
			EnableLayer(AnimationLayer.Weapons, true);
		}
		UpdateLayerWeight(AnimationLayer.Weapons, true);
		UpdateLayerWeight(AnimationLayer.Shop);
		shootTrigger = false;
		jumpTrigger = false;
		gearTrigger = 0;
		Animator.SetBool(ControlFields.WeaponSwitch, weaponSwitch);
		if (weaponSwitch)
		{
			weaponSwitch = false;
		}
	}

	private void OnAnimatorIK()
	{
		if ((bool)_AnchorChest && (bool)_IKAnchor)
		{
			_IKAnchor.transform.position = _AnchorChest.transform.position;
		}
		if ((bool)_IKLeftHand && (bool)_IKRightHand)
		{
			bool flag = Animator.GetCurrentAnimatorStateInfo(1).IsTag("IK");
			bool flag2 = Animator.GetCurrentAnimatorStateInfo(1).IsTag("Melee");
			bool flag3 = IsLayerEnabled(AnimationLayer.Weapons);
			float layerWeight = Animator.GetLayerWeight(1);
			if (flag3 && (flag || flag2))
			{
				_LookAtWeight = Mathf.Lerp(_LookAtWeight, 1f, Time.deltaTime * 10f);
			}
			else
			{
				_LookAtWeight = Mathf.Lerp(_LookAtWeight, 0f, Time.deltaTime * 15f);
			}
			Vector3 position = _IKLeftHand.transform.position;
			position.y += 0.2f;
			Animator.SetLookAtPosition(position);
			Animator.SetLookAtWeight(layerWeight * _LookAtWeight);
			if (flag3 && flag)
			{
				_IKWeight = Mathf.Lerp(_IKWeight, 1f, Time.deltaTime * 10f);
			}
			else
			{
				_IKWeight = Mathf.Lerp(_IKWeight, 0f, Time.deltaTime * 15f);
			}
			float weight = layerWeight * _IKWeight;
			SetIK(AvatarIKGoal.LeftHand, _IKLeftHand.transform, weight);
			SetIK(AvatarIKGoal.RightHand, _IKRightHand.transform, weight);
		}
	}

	private void SetIK(AvatarIKGoal goal, Transform goalTransform, float weight)
	{
		Animator.SetIKPositionWeight(goal, weight);
		Animator.SetIKRotationWeight(goal, weight);
		Animator.SetIKPosition(goal, goalTransform.position);
		Animator.SetIKRotation(goal, goalTransform.rotation);
	}

	private void UpdateLayerWeight(AnimationLayer layer, bool smooth = false)
	{
		float num = (IsLayerEnabled(layer) ? 1 : 0);
		if (smooth)
		{
			float weight = Mathf.Lerp(Animator.GetLayerWeight((int)layer), num, Time.deltaTime * 7.5f);
			Animator.SetLayerWeight((int)layer, weight);
		}
		else
		{
			Animator.SetLayerWeight((int)layer, num);
		}
	}

	public void TriggerGearAnimation(UberstrikeItemClass itemClass)
	{
		ChangeWeaponType((UberstrikeItemClass)0);
		switch (itemClass)
		{
		case UberstrikeItemClass.GearHead:
		case UberstrikeItemClass.GearFace:
			gearTrigger = 1;
			break;
		case UberstrikeItemClass.GearGloves:
			gearTrigger = 2;
			break;
		case UberstrikeItemClass.GearUpperBody:
		case UberstrikeItemClass.GearHolo:
			gearTrigger = 3;
			break;
		case UberstrikeItemClass.GearLowerBody:
			gearTrigger = 4;
			break;
		case UberstrikeItemClass.GearBoots:
			gearTrigger = 5;
			break;
		case UberstrikeItemClass.QuickUseGeneral:
		case UberstrikeItemClass.QuickUseGrenade:
		case UberstrikeItemClass.QuickUseMine:
		case UberstrikeItemClass.FunctionalGeneral:
		case UberstrikeItemClass.SpecialGeneral:
			break;
		}
	}

	public void ChangeWeaponType(UberstrikeItemClass itemClass)
	{
		if (Animator != null)
		{
			weaponSwitch = true;
			switch (itemClass)
			{
			case UberstrikeItemClass.WeaponMelee:
				Animator.SetInteger(ControlFields.WeaponClass, 1);
				break;
			case UberstrikeItemClass.WeaponSniperRifle:
				Animator.SetInteger(ControlFields.WeaponClass, 2);
				break;
			case UberstrikeItemClass.WeaponMachinegun:
			case UberstrikeItemClass.WeaponCannon:
			case UberstrikeItemClass.WeaponSplattergun:
			case UberstrikeItemClass.WeaponLauncher:
				Animator.SetInteger(ControlFields.WeaponClass, 3);
				break;
			case UberstrikeItemClass.WeaponShotgun:
				Animator.SetInteger(ControlFields.WeaponClass, 4);
				break;
			default:
				Animator.SetInteger(ControlFields.WeaponClass, 0);
				break;
			}
		}
	}
}

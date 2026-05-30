using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CharacterTrigger : MonoBehaviour
{
	[SerializeField]
	private AvatarHudInformation _hud;

	[SerializeField]
	private CharacterConfig _config;

	public AvatarHudInformation HudInfo
	{
		get
		{
			if (_hud == null && _config != null && _config.Avatar != null)
			{
				return _config.Avatar.Decorator.HudInformation;
			}
			return _hud;
		}
	}

	public CharacterConfig Character
	{
		get
		{
			return _config;
		}
	}
}

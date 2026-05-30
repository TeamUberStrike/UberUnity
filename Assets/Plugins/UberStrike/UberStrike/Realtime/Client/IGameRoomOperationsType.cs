namespace UberStrike.Realtime.Client
{
	public enum IGameRoomOperationsType
	{
		JoinGame = 1,
		JoinAsSpectator = 2,
		PowerUpRespawnTimes = 3,
		PowerUpPicked = 4,
		IncreaseHealthAndArmor = 5,
		OpenDoor = 6,
		SpawnPositions = 7,
		RespawnRequest = 8,
		DirectHitDamage = 9,
		ExplosionDamage = 10,
		DirectDamage = 11,
		DirectDeath = 12,
		Jump = 13,
		UpdatePositionAndRotation = 14,
		KickPlayer = 15,
		IsFiring = 16,
		IsReadyForNextMatch = 17,
		IsPaused = 18,
		IsInSniperMode = 19,
		SingleBulletFire = 20,
		SwitchWeapon = 21,
		SwitchTeam = 22,
		ChangeGear = 23,
		EmitProjectile = 24,
		EmitQuickItem = 25,
		RemoveProjectile = 26,
		HitFeedback = 27,
		ActivateQuickItem = 28,
		ChatMessage = 29
	}
}

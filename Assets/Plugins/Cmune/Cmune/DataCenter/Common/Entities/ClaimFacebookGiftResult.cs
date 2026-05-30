namespace Cmune.DataCenter.Common.Entities
{
	public enum ClaimFacebookGiftResult
	{
		ErrorUnknown = 0,
		ErrorCouldNotFindRequest = 1,
		ErrorRequestHasInvalidData = 2,
		ErrorCouldNotDeleteRequest = 3,
		ErrorCouldNotGenerateItemId = 4,
		AlreadyOwnedPermanently = 5,
		RentalTimeProlonged = 6,
		NewItemAttributed = 7,
		ErrorWhileSavingItemChanges = 8,
		ErrorClaimerIsNotReceiver = 9
	}
}

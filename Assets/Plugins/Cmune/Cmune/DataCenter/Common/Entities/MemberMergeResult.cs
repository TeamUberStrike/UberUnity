namespace Cmune.DataCenter.Common.Entities
{
	public enum MemberMergeResult
	{
		Ok = 0,
		CmidNotFound = 1,
		CmidAlreadyLinkedToEsns = 3,
		EsnsAlreadyLinkedToCmid = 4,
		InvalidData = 5
	}
}

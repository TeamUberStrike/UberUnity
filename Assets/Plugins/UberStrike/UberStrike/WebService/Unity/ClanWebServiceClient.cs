using System;
using System.Collections.Generic;
using System.IO;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Serialization;
using UnityEngine;

namespace UberStrike.WebService.Unity
{
	public static class ClanWebServiceClient
	{
		public static Coroutine GetOwnClan(string authToken, int groupId, Action<ClanView> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, authToken);
				Int32Proxy.Serialize(memoryStream, groupId);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "GetOwnClan", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ClanViewProxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine UpdateMemberPosition(MemberPositionUpdateView updateMemberPositionData, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				MemberPositionUpdateViewProxy.Serialize(memoryStream, updateMemberPositionData);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "UpdateMemberPosition", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine InviteMemberToJoinAGroup(int clanId, string authToken, int inviteeCmid, string message, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, clanId);
				StringProxy.Serialize(memoryStream, authToken);
				Int32Proxy.Serialize(memoryStream, inviteeCmid);
				StringProxy.Serialize(memoryStream, message);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "InviteMemberToJoinAGroup", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine AcceptClanInvitation(int clanInvitationId, string authToken, Action<ClanRequestAcceptView> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, clanInvitationId);
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "AcceptClanInvitation", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ClanRequestAcceptViewProxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine DeclineClanInvitation(int clanInvitationId, string authToken, Action<ClanRequestDeclineView> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, clanInvitationId);
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "DeclineClanInvitation", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ClanRequestDeclineViewProxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine KickMemberFromClan(int groupId, string authToken, int cmidToKick, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, groupId);
				StringProxy.Serialize(memoryStream, authToken);
				Int32Proxy.Serialize(memoryStream, cmidToKick);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "KickMemberFromClan", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine DisbandGroup(int groupId, string authToken, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, groupId);
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "DisbandGroup", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine LeaveAClan(int groupId, string authToken, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, groupId);
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "LeaveAClan", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine GetMyClanId(string authToken, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "GetMyClanId", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine CancelInvitation(int groupInvitationId, string authToken, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, groupInvitationId);
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "CancelInvitation", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine GetAllGroupInvitations(string authToken, Action<List<GroupInvitationView>> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "GetAllGroupInvitations", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ListProxy<GroupInvitationView>.Deserialize(new MemoryStream(data), GroupInvitationViewProxy.Deserialize));
					}
				}, handler));
			}
		}

		public static Coroutine GetPendingGroupInvitations(int groupId, string authToken, Action<List<GroupInvitationView>> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, groupId);
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "GetPendingGroupInvitations", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ListProxy<GroupInvitationView>.Deserialize(new MemoryStream(data), GroupInvitationViewProxy.Deserialize));
					}
				}, handler));
			}
		}

		public static Coroutine CreateClan(GroupCreationView createClanData, Action<ClanCreationReturnView> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				GroupCreationViewProxy.Serialize(memoryStream, createClanData);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "CreateClan", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ClanCreationReturnViewProxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine TransferOwnership(int groupId, string authToken, int newLeaderCmid, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, groupId);
				StringProxy.Serialize(memoryStream, authToken);
				Int32Proxy.Serialize(memoryStream, newLeaderCmid);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "TransferOwnership", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine CanOwnAClan(string authToken, Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, authToken);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "CanOwnAClan", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine test(Action<int> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IClanWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ClanWebServiceContract.svc", "test", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(Int32Proxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}
	}
}

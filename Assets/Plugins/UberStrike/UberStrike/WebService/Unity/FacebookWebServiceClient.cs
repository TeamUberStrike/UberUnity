using System;
using System.Collections.Generic;
using System.IO;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Serialization;
using UnityEngine;

namespace UberStrike.WebService.Unity
{
	public static class FacebookWebServiceClient
	{
		public static Coroutine ClaimFacebookGift(string authToken, string facebookRequestObjectId, Action<ClaimFacebookGiftView> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, authToken);
				StringProxy.Serialize(memoryStream, facebookRequestObjectId);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IFacebookWebServiceContract", "UberStrike.DataCenter.WebService.CWS.FacebookWebServiceContract.svc", "ClaimFacebookGift", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ClaimFacebookGiftViewProxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine AttachFacebookAccountToCmuneAccount(string authToken, string facebookId, Action<MemberOperationResult> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, authToken);
				StringProxy.Serialize(memoryStream, facebookId);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IFacebookWebServiceContract", "UberStrike.DataCenter.WebService.CWS.FacebookWebServiceContract.svc", "AttachFacebookAccountToCmuneAccount", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(EnumProxy<MemberOperationResult>.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine CheckFacebookSession(string cmuneAuthToken, string facebookIDString, Action<bool> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, cmuneAuthToken);
				StringProxy.Serialize(memoryStream, facebookIDString);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IFacebookWebServiceContract", "UberStrike.DataCenter.WebService.CWS.FacebookWebServiceContract.svc", "CheckFacebookSession", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(BooleanProxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine GetFacebookFriendsList(List<string> facebookIds, Action<List<PublicProfileView>> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ListProxy<string>.Serialize(memoryStream, facebookIds, StringProxy.Serialize);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IFacebookWebServiceContract", "UberStrike.DataCenter.WebService.CWS.FacebookWebServiceContract.svc", "GetFacebookFriendsList", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ListProxy<PublicProfileView>.Deserialize(new MemoryStream(data), PublicProfileViewProxy.Deserialize));
					}
				}, handler));
			}
		}
	}
}

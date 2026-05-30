using System;
using System.Collections.Generic;
using System.IO;
using Cmune.DataCenter.Common.Entities;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Serialization;
using UberStrike.Core.Types;
using UberStrike.DataCenter.Common.Entities;
using UnityEngine;

namespace UberStrike.WebService.Unity
{
	public static class ApplicationWebServiceClient
	{
		public static Coroutine AuthenticateApplication(string clientVersion, ChannelType channel, string publicKey, Action<AuthenticateApplicationView> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, clientVersion);
				EnumProxy<ChannelType>.Serialize(memoryStream, channel);
				StringProxy.Serialize(memoryStream, publicKey);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IApplicationWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ApplicationWebServiceContract.svc", "AuthenticateApplication", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(AuthenticateApplicationViewProxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine GetMaps(string clientVersion, DefinitionType clientType, Action<List<MapView>> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, clientVersion);
				EnumProxy<DefinitionType>.Serialize(memoryStream, clientType);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IApplicationWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ApplicationWebServiceContract.svc", "GetMaps", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ListProxy<MapView>.Deserialize(new MemoryStream(data), MapViewProxy.Deserialize));
					}
				}, handler));
			}
		}

		public static Coroutine GetConfigurationData(string clientVersion, Action<ApplicationConfigurationView> callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, clientVersion);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IApplicationWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ApplicationWebServiceContract.svc", "GetConfigurationData", memoryStream.ToArray(), delegate(byte[] data)
				{
					if (callback != null)
					{
						callback(ApplicationConfigurationViewProxy.Deserialize(new MemoryStream(data)));
					}
				}, handler));
			}
		}

		public static Coroutine SetMatchScore(string clientVersion, MatchStats scoringView, string serverAuthentication, Action callback, Action<Exception> handler)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StringProxy.Serialize(memoryStream, clientVersion);
				MatchStatsProxy.Serialize(memoryStream, scoringView);
				StringProxy.Serialize(memoryStream, serverAuthentication);
				return MonoInstance.Mono.StartCoroutine(SoapClient.MakeRequest("IApplicationWebServiceContract", "UberStrike.DataCenter.WebService.CWS.ApplicationWebServiceContract.svc", "SetMatchScore", memoryStream.ToArray(), delegate
				{
					if (callback != null)
					{
						callback();
					}
				}, handler));
			}
		}
	}
}

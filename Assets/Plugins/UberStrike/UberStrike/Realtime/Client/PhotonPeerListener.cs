using System;
using System.Runtime.Serialization;
using ExitGames.Client.Photon;
using UnityEngine;

namespace UberStrike.Realtime.Client
{
	public sealed class PhotonPeerListener : IPhotonPeerListener
	{
		private Action<byte, byte[]> eventDispatcher;

		private Action onConnect;

		private Action<StatusCode> onDisconnect;

		private Action<string> onError;

		public event Action<byte, byte[]> EventDispatcher
		{
			add
			{
				eventDispatcher = (Action<byte, byte[]>)Delegate.Combine(eventDispatcher, value);
			}
			remove
			{
				eventDispatcher = (Action<byte, byte[]>)Delegate.Remove(eventDispatcher, value);
			}
		}

		public event Action OnConnect
		{
			add
			{
				onConnect = (Action)Delegate.Combine(onConnect, value);
			}
			remove
			{
				onConnect = (Action)Delegate.Remove(onConnect, value);
			}
		}

		public event Action<StatusCode> OnDisconnect
		{
			add
			{
				onDisconnect = (Action<StatusCode>)Delegate.Combine(onDisconnect, value);
			}
			remove
			{
				onDisconnect = (Action<StatusCode>)Delegate.Remove(onDisconnect, value);
			}
		}

		public event Action<string> OnError
		{
			add
			{
				onError = (Action<string>)Delegate.Combine(onError, value);
			}
			remove
			{
				onError = (Action<string>)Delegate.Remove(onError, value);
			}
		}

		internal void ClearEvents()
		{
			eventDispatcher = null;
			onConnect = null;
			onDisconnect = null;
			onError = null;
		}

		public void OnEvent(EventData eventData)
		{
			try
			{
				if (eventDispatcher != null)
				{
					eventDispatcher(eventData.Code, (byte[])eventData.Parameters[0]);
				}
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				Debug.LogError("OnEvent failed: " + eventData.ToStringFull() + "\n" + ex2.ToString());
				throw ex2;
			}
		}

		public void OnOperationResponse(OperationResponse operationResponse)
		{
			if (operationResponse.ReturnCode > 0)
			{
				Debug.LogError("OnOperationResponse: " + operationResponse.DebugMessage);
				if (onError != null)
				{
					onError(operationResponse.DebugMessage);
				}
			}
			else
			{
				Debug.Log("OnOperationResponse: " + operationResponse.OperationCode);
			}
		}

		public void OnStatusChanged(StatusCode statusCode)
		{
			Debug.Log("PeerStatusCallback " + statusCode);
			switch (statusCode)
			{
			case StatusCode.Connect:
				if (onConnect != null)
				{
					onConnect();
				}
				break;
			case StatusCode.ExceptionOnConnect:
			case StatusCode.Disconnect:
			case StatusCode.Exception:
			case StatusCode.TimeoutDisconnect:
			case StatusCode.DisconnectByServer:
			case StatusCode.DisconnectByServerUserLimit:
			case StatusCode.DisconnectByServerLogic:
				if (onDisconnect != null)
				{
					onDisconnect(statusCode);
				}
				break;
			case StatusCode.SendError:
				Debug.LogWarning("Operation sent without connection to server");
				break;
			default:
				Debug.LogWarning("Unhandled OnStatusChanged " + statusCode);
				break;
			}
		}

		public void DebugReturn(DebugLevel level, string message)
		{
			Debug.Log("DebugReturn " + message);
		}
	}
}

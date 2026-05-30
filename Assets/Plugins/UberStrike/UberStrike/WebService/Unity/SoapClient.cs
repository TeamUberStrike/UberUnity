using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace UberStrike.WebService.Unity
{
	internal static class SoapClient
	{
		private static int _requestId;

		private static void LogRequest(int id, float time, int sizeBytes, string interfaceName, string serviceName, string methodName)
		{
			if (Configuration.RequestLogger != null)
			{
				string text = ((float)sizeBytes / 1000f).ToString();
				Configuration.RequestLogger(string.Format("[REQ] ID:{0} Time:{1:N2} Size:{2:N2}Kb Service:{3} Interface:{4} Method:{5}", id, time, text, serviceName, interfaceName, methodName));
			}
		}

		private static void LogResponse(int id, float time, string message, float duration, int sizeBytes)
		{
			if (Configuration.RequestLogger != null)
			{
				string text = ((float)sizeBytes / 1000f).ToString();
				Configuration.RequestLogger(string.Format("[RSP] ID:{0} Time:{1:N2} Size:{2:N2}Kb Duration:{3:N2}s Status:{4}", id, time, text, duration, message));
			}
		}

		public static IEnumerator MakeRequest(string interfaceName, string serviceName, string methodName, byte[] data, Action<byte[]> requestCallback, Action<Exception> exceptionHandler)
		{
			int requestId = _requestId++;
			string postData = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><" + methodName + " xmlns=\"http://tempuri.org/\"><data>" + Convert.ToBase64String(data) + "</data></" + methodName + "></s:Body></s:Envelope>";
			byte[] byteArray = Encoding.UTF8.GetBytes(postData);
			Dictionary<string, string> headers = new Dictionary<string, string>
			{
				{
					"SOAPAction",
					"\"http://tempuri.org/" + interfaceName + "/" + methodName + "\""
				},
				{ "Content-type", "text/xml; charset=utf-8" }
			};
			XmlDocument doc = new XmlDocument();
			float startTime = Time.realtimeSinceStartup;
			LogRequest(requestId, startTime, data.Length, interfaceName, serviceName, methodName);
			yield return new WaitForEndOfFrame();
			if (WebServiceStatistics.IsEnabled)
			{
				WebServiceStatistics.RecordWebServiceBegin(methodName, byteArray.Length);
			}
			byte[] returnData = null;
			UnityWebRequest request = new UnityWebRequest(Configuration.WebserviceBaseUrl + serviceName, "POST");
			request.uploadHandler = new UploadHandlerRaw(byteArray);
			request.downloadHandler = new DownloadHandlerBuffer();
			foreach (var header in headers)
			{
				request.SetRequestHeader(header.Key, header.Value);
			}
			yield return request.SendWebRequest();
			if (WebServiceStatistics.IsEnabled)
			{
				int responseSize = (request.downloadHandler.data != null) ? request.downloadHandler.data.Length : 0;
				WebServiceStatistics.RecordWebServiceEnd(methodName, responseSize, request.result == UnityWebRequest.Result.Success);
			}
			try
			{
				if (Configuration.SimulateWebservicesFail)
				{
					throw new Exception("Simulated Webservice fail when calling " + interfaceName + "/" + methodName);
				}
				if (request.result == UnityWebRequest.Result.Success)
				{
					string responseText = request.downloadHandler.text;
					if (!string.IsNullOrEmpty(responseText))
					{
						try
						{
							doc.LoadXml(responseText);
							XmlNodeList result = doc.GetElementsByTagName(methodName + "Result");
							if (result.Count <= 0)
							{
								LogResponse(requestId, Time.realtimeSinceStartup, responseText, Time.time - startTime, 0);
								throw new Exception("Request to " + Configuration.WebserviceBaseUrl + serviceName + " failed with content" + responseText);
							}
							returnData = Convert.FromBase64String(result[0].InnerXml);
							if (returnData.Length == 0)
							{
								LogResponse(requestId, Time.realtimeSinceStartup, responseText, Time.time - startTime, 0);
								throw new Exception("Request to " + Configuration.WebserviceBaseUrl + serviceName + " failed with content" + responseText);
							}
							int respSize = (request.downloadHandler.data != null) ? request.downloadHandler.data.Length : 0;
							LogResponse(requestId, Time.realtimeSinceStartup, "OK", Time.realtimeSinceStartup - startTime, respSize);
						}
						catch
						{
							LogResponse(requestId, Time.time, responseText, Time.realtimeSinceStartup - startTime, 0);
							throw new Exception("Error reading XML return for method call " + interfaceName + "/" + methodName + ":" + responseText);
						}
					}
					if (requestCallback != null)
					{
						requestCallback(returnData);
					}
					request.Dispose();
					yield break;
				}
				LogResponse(requestId, Time.realtimeSinceStartup, request.error, Time.time - startTime, 0);
				throw new Exception(request.error + "\nURL: " + Configuration.WebserviceBaseUrl + "\nService: " + serviceName + "\nMethod: " + methodName);
			}
			catch (Exception ex)
			{
				if (exceptionHandler != null)
				{
					exceptionHandler(ex);
				}
				else
				{
					Debug.LogError("SoapClient Unhandled Exception: " + ex.Message + "\n" + ex.StackTrace);
				}
			}
			request.Dispose();
		}
	}
}

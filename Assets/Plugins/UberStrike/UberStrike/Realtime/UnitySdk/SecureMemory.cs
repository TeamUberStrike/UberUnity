using System;
using System.IO;
using UberStrike.Core.Serialization;

namespace UberStrike.Realtime.UnitySdk
{
	public class SecureMemory<T>
	{
		private const string pp = "h&dk2Ks901HenM";

		private const string iv = "huSj39Dl)2kJ4nat";

		private byte[] _encryptedData;

		private T _cachedValue;

		private bool _useAOTCompatibleMode;

		public SecureMemory(T value, bool monitorMemory = true, bool useAOTCompatibleMode = false)
		{
			_useAOTCompatibleMode = useAOTCompatibleMode;
			if (_useAOTCompatibleMode)
			{
				_cachedValue = value;
				return;
			}
			WriteData(value);
			if (monitorMemory)
			{
				SecureMemoryMonitor.Instance.AddToMonitor += ValidateData;
			}
		}

		public void ReleaseData(SecureMemory<T> instance)
		{
			if (!_useAOTCompatibleMode && instance != null)
			{
				SecureMemoryMonitor.Instance.AddToMonitor -= instance.ValidateData;
			}
		}

		public void SimulateMemoryHack(T value)
		{
			_cachedValue = value;
		}

		public void WriteData(T value)
		{
			try
			{
				_cachedValue = value;
				if (!_useAOTCompatibleMode)
				{
					_encryptedData = Cryptography.RijndaelEncrypt(Serialize(value), "h&dk2Ks901HenM", "huSj39Dl)2kJ4nat");
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("SecureMemory failed encrypting Data: {0}", ex.Message), ex.InnerException);
			}
		}

		public void ValidateData()
		{
			if (!Comparison.IsEqual(_cachedValue, DecryptValue()))
			{
				throw new Exception("Failed to validate data due to a corrupted memory");
			}
		}

		public object ReadObject(bool secure)
		{
			return ReadData(secure);
		}

		public T ReadData(bool secure)
		{
			if (secure)
			{
				_cachedValue = DecryptValue();
			}
			return _cachedValue;
		}

		private T DecryptValue()
		{
			if (_useAOTCompatibleMode)
			{
				return _cachedValue;
			}
			try
			{
				byte[] array = Cryptography.RijndaelDecrypt(_encryptedData, "h&dk2Ks901HenM", "huSj39Dl)2kJ4nat");
				if (array != null)
				{
					object obj = Deserialize(array);
					if (obj != null)
					{
						return (T)obj;
					}
					throw new Exception("SecureMemory failed decrypting Data becauase RealtimeSerialization.ToObject returned NULL");
				}
				throw new Exception("SecureMemory failed decrypting Data becauase CmuneSecurity.Decrypt returned NULL");
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("SecureMemory failed decrypting Data: {0}", ex.Message), ex.InnerException);
			}
		}

		private byte[] Serialize(T obj)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle == typeof(int))
				{
					Int32Proxy.Serialize(memoryStream, (int)(object)obj);
				}
				else if (typeFromHandle == typeof(float))
				{
					SingleProxy.Serialize(memoryStream, (float)(object)obj);
				}
				else if (typeFromHandle == typeof(string))
				{
					StringProxy.Serialize(memoryStream, (string)(object)obj);
				}
				return memoryStream.ToArray();
			}
		}

		private T Deserialize(byte[] bytes)
		{
			using (MemoryStream bytes2 = new MemoryStream(bytes))
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle == typeof(int))
				{
					return (T)(object)Int32Proxy.Deserialize(bytes2);
				}
				if (typeFromHandle == typeof(float))
				{
					return (T)(object)SingleProxy.Deserialize(bytes2);
				}
				if (typeFromHandle == typeof(string))
				{
					return (T)(object)StringProxy.Deserialize(bytes2);
				}
				return default(T);
			}
		}
	}
}

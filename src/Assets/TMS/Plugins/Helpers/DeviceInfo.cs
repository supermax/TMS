#region Usings

using System;
using System.Globalization;
using TMS.Common.Extensions;
#if UNITY || UNITY3D || UNITY_3D
using UnityEngine;
#else
using System.Diagnostics;
#endif

#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	///     Device Info Helper
	/// </summary>
	public static class DeviceInfo
	{
#if UNITY || UNITY3D || UNITY_3D
		private static string _deviceId;
		private static string _vendorDeviceId;

#if UNITY_ANDROID
		/// <summary>
		/// Gets the android device identifier.
		/// </summary>
		/// <returns></returns>
		public static string GetAndroidDeviceId()
		{
			using (var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (var currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					using (var contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver"))
					{
						using (var secure = new AndroidJavaClass("android.provider.Settings$Secure"))
						{
							var deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");
							return deviceId;
						}                           
					}                       
				}
			}
		}

		/// <summary>
		/// Gets the android advertising identifier.
		/// </summary>
		/// <param name="advertisingId">The advertising identifier.</param>
		/// <returns></returns>
		public static bool GetAdvertisingIdentifier(out string advertisingId)
		{
			using (var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (var currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					using (var client = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient"))
					{
						using (var adInfo = client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity))
						{
							advertisingId = adInfo.Call<string>("getId");
							var limitAdvertising = adInfo.Call<bool>("isLimitAdTrackingEnabled");
							return limitAdvertising;
						}
					}
				}
			}
		}
#elif UNITY_IPHONE || UNITY_IOS
		/// <summary>
		/// Gets the android advertising identifier.
		/// </summary>
		/// <param name="advertisingId">The advertising identifier.</param>
		/// <returns></returns>
		public static bool GetAdvertisingIdentifier(out string advertisingId)
		{
			advertisingId = UnityEngine.iOS.Device.advertisingIdentifier;
			return UnityEngine.iOS.Device.advertisingTrackingEnabled;
		}
#endif

		/// <summary>
		///     Android id for android devices, advertising identifier (IDFA) for iOS devices, SystemInfo.deviceUniqueIdentifier for other devices
		/// </summary>
		/// <returns>DID</returns>
		public static string GetDeviceId()
		{
			if (!_deviceId.IsNullOrEmpty()) return _deviceId;

			if (Application.isEditor)
			{
				_deviceId = SystemInfo.deviceUniqueIdentifier;
				return _deviceId;
			}

			try
			{
#if UNITY_ANDROID
				_deviceId = GetAndroidDeviceId();
#elif UNITY_IPHONE
				try
				{
					_deviceId = UnityEngine.iOS.Device.advertisingTrackingEnabled ? 
											UnityEngine.iOS.Device.advertisingIdentifier : 
											UnityEngine.iOS.Device.vendorIdentifier;
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					if (_deviceId.IsNullOrEmpty())
					{
						_deviceId = UnityEngine.iOS.Device.vendorIdentifier;
					}
				}
#else
				_deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			if (!_deviceId.IsNullOrEmpty()) return _deviceId;

			_deviceId = SystemInfo.deviceUniqueIdentifier;
			Debug.LogWarning(string.Format("DID: {0} (fallback: SystemInfo.deviceUniqueIdentifier)", _deviceId));
			return _deviceId;
		}

		/// <summary>
		/// Gets the vendor device identifier.
		/// </summary>
		/// <returns></returns>
		public static string GetVendorDeviceId()
		{
			if (!_vendorDeviceId.IsNullOrEmpty()) return _vendorDeviceId;

			if (Application.isEditor)
			{
				_vendorDeviceId = SystemInfo.deviceUniqueIdentifier;
				return _vendorDeviceId;
			}

			try
			{
#if UNITY_ANDROID
				_vendorDeviceId = GetAndroidDeviceId();
#elif UNITY_IPHONE
				_vendorDeviceId = UnityEngine.iOS.Device.vendorIdentifier;
#else
				_vendorDeviceId = SystemInfo.deviceUniqueIdentifier;
#endif
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			if (!_vendorDeviceId.IsNullOrEmpty()) return _vendorDeviceId;

			_vendorDeviceId = SystemInfo.deviceUniqueIdentifier;
			Debug.LogWarning(string.Format("VDID: {0} (fallback: SystemInfo.deviceUniqueIdentifier)", _vendorDeviceId));
			return _vendorDeviceId;
		}

		// TODO
		public static string GetPersistentDataPath()
		{
			return Application.persistentDataPath;
		}
#endif

		/// <summary>
		/// Gets the current time zone.
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentTimeZone()
		{
			var tz = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalSeconds.ToString(CultureInfo.InvariantCulture);
			return tz;
		}

		/// <summary>
		/// Gets the current platform ID.
		/// </summary>
		/// <returns></returns>
		public static int GetCurrentPlatform()
		{
#if UNITY_IPHONE
			return 1;
#elif UNITY_ANDROID
			return 2;
#elif SILVERLIGHT
			return 3;
#else
			return 0;
#endif
		}
	}
}
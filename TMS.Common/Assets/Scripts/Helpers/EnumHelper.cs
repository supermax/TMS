#define UNITY3D

using System;
using TMS.Common.Extensions;

namespace TMS.Common.Helpers
{
	public static class EnumHelper
	{
		public static T[] GetEnumPartialArray<T>(this T src, int firstEnumAsInt, int lastEnumAsInt) 
		{
			if (firstEnumAsInt > lastEnumAsInt)
				return null;

			var enumValues = (T[])Enum.GetValues(typeof(T));
			var enumRange = lastEnumAsInt - firstEnumAsInt + 1;

			if (firstEnumAsInt < 0 || firstEnumAsInt > enumValues.Length)
				return null;
			if (lastEnumAsInt < 0 || lastEnumAsInt > enumValues.Length)
				return null;

			var enumArr = new T[enumRange];

			for (int i = firstEnumAsInt, j = 0; i <= lastEnumAsInt; i++, j++)
			{
				enumArr[j] = enumValues[i];
			}

			return enumArr;
		}

		/// <summary>
		/// Converts the specified string value to enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		/// <exception cref="OperationCanceledException"></exception>
		public static T Convert<T>(string value)
		{
			ArgumentValidator.AssertNotNullOrEmpty(value, "value");

			var enumType = typeof(T);
#if !UNITY_WSA
			if (!enumType.IsEnum)
			{
				throw new OperationCanceledException(string.Format("The \"{0}\" is not \"{1}\"!", enumType, typeof(Enum)));
			}
#else
			// TODO check if "enumType" is "Enum"
#endif

			var convertedValue = Enum.Parse(enumType, value, true);
			var method = (T)convertedValue;
			return method;
		}

		/// <summary>
		/// Safely converts given string to enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static T SafeConvert<T>(string value)
		{
			try
			{
				var res = Convert<T>(value);
				return res;
			}
			catch (Exception e)
			{
#if UNITY3D || UNITY_3D
				UnityEngine.Debug.LogException(e);
#else
				System.Diagnostics.Debug.WriteLine(e);				
#endif
				return default(T);
			}
		}
	}
}
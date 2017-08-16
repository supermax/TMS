using System;

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
	}
}
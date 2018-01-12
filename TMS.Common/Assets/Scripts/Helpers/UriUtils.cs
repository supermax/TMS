using System;
using System.Collections.Generic;

namespace diwip.Infr.Client.Common.Slots.Utils
{
	internal static class UriUtils
	{
		internal static Uri StripQueryParameters(Uri uri)
		{
			return (new UriBuilder(uri)
				{
					Query = string.Empty
				}).Uri;
		}

		internal static bool TryParseQuery(string uri, out IDictionary<string, string> data)
		{
			data = new Dictionary<string, string>();
			if (string.IsNullOrEmpty(uri))
			{
				return false;
			}
			return TryParseQuery(new Uri(uri), out data);
		}

		internal static bool TryParseQuery(Uri uri, out IDictionary<string, string> data)
		{
			data = new Dictionary<string, string>();
			string query = uri.Query;
			if (string.IsNullOrEmpty(query))
			{
				return false;
			}
			if (query.StartsWith("?"))
			{
				query = query.Substring(1);
			}
			string[] strArrays = query.Split(new[] {'&'});
			for (int i = 0; i < strArrays.Length; i++)
			{
				string str = strArrays[i];
				string[] strArrays1 = str.Split(new [] {'='});
				if (strArrays1.Length == 2)
				{
					data.Add(strArrays1[0], strArrays1[1]);
				}
			}
			return true;
		}
	}
}
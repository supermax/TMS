using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Common.Extensions
{
    public static class UriExtensions
    {
        private static readonly char[] QuerySignArray = "?".ToCharArray();
        private static readonly char[] QuerySplitterArray = "&".ToCharArray();
        private static readonly char[] QueryPartSplitterArray = "=".ToCharArray();

        public static IDictionary<string, string> GetQueryAsDictionary(this Uri src)
        {
            ArgumentValidator.AssertNotNull(src, "src");
            if (src.Query.IsNullOrEmpty()) return null;

            var query = src.Query.TrimStart(QuerySignArray);
            if (query.IsNullOrEmpty()) return null;

            var parts = query.Split(QuerySplitterArray);
            var dic = new Dictionary<string, string>();
            foreach (var part in parts)
            {
                var ary = part.Split(QueryPartSplitterArray);
                if (ary.IsNullOrEmpty() || ary.Length < 2) continue;

                dic[ary[0]] = ary[1];
            }
            return dic.Count > 0 ? dic : null;
        }

        public static string ConvertToUriQueryString(this IDictionary<string, string> src)
        {
            ArgumentValidator.AssertNotNull(src, "src");

            var str = new StringBuilder();
            str.Append(QuerySignArray);

            var count = 0;
            foreach (var item in src)
            {
                str.Append(item.Key).Append(QueryPartSplitterArray).Append(item.Value);
                if (count < src.Count - 1) str.Append(QuerySplitterArray);
                count++;
            }

            return str.ToString();
        }

        public static Uri SetQueryValue(this Uri src, string key, string value, bool replace = true)
        {
            ArgumentValidator.AssertNotNull(src, "src");

            var dic = src.GetQueryAsDictionary() ?? new Dictionary<string, string>();
            if (dic.ContainsKey(key) && !replace) return src;

            dic[key] = value;

            var query = dic.ConvertToUriQueryString();
            var address = src.OriginalString.Replace(src.Query, query);

            src = new Uri(address);
            return src;
        }
    }
}

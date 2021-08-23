using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Selenium.Humanize.Models
{
    public static class Helper
    {
        public static string GetEmbeddedResourceFile(string filename, string assembly = "Selenium.Humanize.Resources.")
        {
            var a = System.Reflection.Assembly.GetExecutingAssembly();
            using (var s = a.GetManifestResourceStream(assembly + filename))
            using (var r = new System.IO.StreamReader(s))
            {
                string result = r.ReadToEnd();
                return result;
            }
        }

        public static object ExecuteJs(this IWebDriver driver, string script)
        {
            return ((IJavaScriptExecutor)driver).ExecuteScript(script);
        }

        public static float NextFloat(this Random random, float min, float max)
        {
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }

        public static T Choice<T>(this Random random, params T[] source)
        {
            T result = default(T);
            int cnt = 0;
            foreach (T item in source)
            {
                cnt++;
                if (random.Next(cnt) == 0)
                {
                    result = item;
                }
            }
            return result;
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>
            (this IEnumerable<TFirst> source, IEnumerable<TSecond> second, IEnumerable<TThird> third, Func<TFirst, TSecond, TThird, TResult> selector)
        {
            using (IEnumerator<TFirst> iterator1 = source.GetEnumerator())
            using (IEnumerator<TSecond> iterator2 = second.GetEnumerator())
            using (IEnumerator<TThird> iterator3 = third.GetEnumerator())
            {
                while (iterator1.MoveNext() && iterator2.MoveNext()
                    && iterator3.MoveNext())
                {
                    yield return selector(iterator1.Current, iterator2.Current,
                        iterator3.Current);
                }
            }
        }

        public static double GetUnixTimestamp()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static bool NextBool(this Random random, int percentage = 50)
        {
            return random.NextDouble() < percentage / 100.0;
        }

        public static char NextChar(this Random random)
        {
            return (char)random.Next('a', 'z');
        }

        public static T Next<T>(this Random random, IEnumerable<T> enumerable)
        {
            int index = random.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

        public static bool CompareUri(Uri uri1, Uri uri2)
        {
            return Uri.Compare(uri1, uri2, UriComponents.Host | UriComponents.PathAndQuery, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool CompareHost(Uri uri1, Uri uri2)
        {
            return uri1.GetBaseDomain().Equals(uri2.GetBaseDomain());
        }

        public static string GetBaseDomain(this Uri uri)
        {
            if (uri.HostNameType == UriHostNameType.Dns)
                return GetBaseDomain(uri.DnsSafeHost);

            return uri.Host;
        }

        public static string GetBaseDomain(string domainName)
        {
            var tokens = domainName.Split('.');

            // only split 3 segments like www.west-wind.com
            if (tokens == null || tokens.Length != 3)
                return domainName;

            var tok = new List<string>(tokens);
            var remove = tokens.Length - 2;
            tok.RemoveRange(0, remove);

            return tok[0] + "." + tok[1]; ;
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}

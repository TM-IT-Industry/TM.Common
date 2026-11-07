using System;
using System.Collections.Generic;
using System.Text;

namespace TM.Common.Utils
{
    public class Url
    {
        public static string Combine(string baseUrl, string relativePath)
        {
            var baseUri = new Uri(baseUrl, UriKind.Absolute);
            var combinedUri = new Uri(baseUri, relativePath);
            return combinedUri.ToString();
        }
    }
}

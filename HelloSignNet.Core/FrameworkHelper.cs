using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace HelloSignNet.Core
{
    public static class FrameworkHelper
    {
        public static byte[] GetAsciiBytes(this string s)
        {
            var retval = new byte[s.Length];
            for (int ix = 0; ix < s.Length; ++ix)
            {
                char ch = s[ix];
                if (ch <= 0x7f) retval[ix] = (byte) ch;
                else retval[ix] = (byte) '?';
            }
            return retval;
        }

        public static HttpClient CreateBasicAuthHttpClient(string username, string password)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(string.Format("{0}:{1}", username, password).GetAsciiBytes()));

            return client;
        }

        public static MultipartFormDataContent AddStringContent(this MultipartFormDataContent formDataContent,
            string name, string content)
        {
            formDataContent.Add(CreateStringContent(name, content));
            return formDataContent;
        }

        public static MultipartFormDataContent AddFileStreamContent(this MultipartFormDataContent formDataContent,
            string filePath, string filename)
        {
            var fs = new FileStream(filePath, FileMode.Open);
            formDataContent.Add(CreateFileStreamContent(fs, filename));
            return formDataContent;
        }

        private static StringContent CreateStringContent(string name, string content)
        {
            var res = new StringContent(content);
            res.Headers.Clear();
            res.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"" + name + "\""
            };
            return res;
        }

        private static StreamContent CreateFileStreamContent(Stream stream, string filename)
        {
            var res = new StreamContent(stream);
            res.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file[0]\"",
                FileName = "\"" + filename + "\""
            };
            res.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return res;
        }

        public class UnderscoreMappingResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                // Find the camelCase position to insert an underscore
                return
                    Regex.Replace(propertyName, @"([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])",
                        "$1$3_$2$4").ToLower();
            }
        }
    }
}
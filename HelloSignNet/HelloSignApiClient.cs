using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HelloSignNet
{
    public class HelloSignConfig
    {
        public string ApiKey { get; set; }
        public string BaseUri { get; set; }
        public string SignatureRequestUri { get; set; }
        public string SendSignatureRequestUri { get; set; }

        public HelloSignConfig()
        {
            // elided
        }

        public HelloSignConfig(string apiKey, string baseUri = "https://api.hellosign.com/v3/")
        {
            ApiKey = apiKey;
            BaseUri = baseUri;
            SignatureRequestUri = BaseUri + "signature_request";
            SendSignatureRequestUri = BaseUri + "signature_request/send";
        }
    }

    public class HelloSignApiClient
    {
        private readonly HttpClient _client;
        private readonly HelloSignConfig _config;

        public HelloSignApiClient(string apiKey)
            : this(new HelloSignConfig(apiKey))
        {
        }

        public HelloSignApiClient(HelloSignConfig config)
        {
            _config = config;
            _client = FrameworkHelper.CreateBasicAuthHttpClient(_config.ApiKey, "");
        }

        public HelloSignApiClient(HttpClient httpClient, HelloSignConfig config)
        {
            _client = httpClient;
            _config = config;
        }

        public Task<SignatureRequestResponse> SendSignatureRequest(SignatureRequestData request, int milisecondsTimeout = 100000)
        {
            var formData = CreateFormData(request);
            return _client.PostAsync(_config.SendSignatureRequestUri, formData).ContinueWith(t =>
            {
                t.Result.EnsureSuccessStatusCode();

                var serializer = new JsonSerializer { ContractResolver = new FrameworkHelper.UnderscoreMappingResolver() };

                using (var streamReader = new StreamReader(t.Result.Content.ReadAsStreamAsync().Result))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var response = serializer.Deserialize<SignatureRequestResponse>(jsonReader);
                    return response;
                }
            });
        }

        private MultipartFormDataContent CreateFormData(SignatureRequestData request)
        {
            var formData = new MultipartFormDataContent();

            if (!string.IsNullOrEmpty(request.Title))
                formData.AddStringContent("title", request.Title);
            if (!string.IsNullOrEmpty(request.Subject))
                formData.AddStringContent("subject", request.Subject);
            if (!string.IsNullOrEmpty(request.Message))
                formData.AddStringContent("message", request.Message);

            for (var i = 0; i < request.Signers.Count; i++)
            {
                formData.AddStringContent(string.Format("signers[{0}][name]", i), request.Signers[i].Name);
                formData.AddStringContent(string.Format("signers[{0}][email_address]", i), request.Signers[i].EmailAddress);
                if (!string.IsNullOrEmpty(request.Signers[i].Order))
                    formData.AddStringContent(string.Format("signers[{0}][order]", i), request.Signers[i].Order);
                if (!string.IsNullOrEmpty(request.Signers[i].Pin))
                    formData.AddStringContent(string.Format("signers[{0}][pin]", i), request.Signers[i].Pin);
            }

            /*
            for (var i = 0; i < request.Files.Count; i++)
            {
                formData.AddFileStreamContent(request.Files[i].FullName, request.Files[i].Name);
            }

            for (var i = 0; i < request.FileUrls.Count; i++)
            {
                formData.AddStringContent(string.Format("file_url[{0}]", i), request.FileUrls[i]);
            }

            formData.AddStringContent("test_mode", request.TestMode.ToString(CultureInfo.InvariantCulture));
            */
            return formData;
        }
    }
}

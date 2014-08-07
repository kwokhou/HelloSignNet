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
        public const int DefaultApiTimeoutMiliseconds = 100000;
        public string ApiKey { get; set; }
        public string BaseUri { get; set; }
        public string GetSignatureRequestUri { get; set; }
        public string SendSignatureRequestUri { get; set; }
        public string RemindSignatureRequestUri { get; set; }

        public HelloSignConfig()
        {
            // elided
        }

        public HelloSignConfig(string apiKey, string baseUri = "https://api.hellosign.com/v3/")
        {
            ApiKey = apiKey;
            BaseUri = baseUri;
            GetSignatureRequestUri = BaseUri + "signature_request";
            SendSignatureRequestUri = BaseUri + "signature_request/send";
            RemindSignatureRequestUri = BaseUri + "signature_request/remind";
        }
    }

    public class HelloSignApiClient
    {
        private readonly HttpClient _client;
        private JsonSerializer _serializer;
        public HelloSignConfig Config { get; set; }

        private JsonSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                {
                    _serializer = new JsonSerializer { ContractResolver = new FrameworkHelper.UnderscoreMappingResolver() };
                }
                return _serializer;
            }
        }

        public HelloSignApiClient(HelloSignConfig config)
        {
            Config = config;
            _client = FrameworkHelper.CreateBasicAuthHttpClient(Config.ApiKey, "");
        }

        public HelloSignApiClient(string apiKey)
            : this(new HelloSignConfig(apiKey))
        {
            // elided
        }

        public HelloSignApiClient(HttpClient httpClient, HelloSignConfig config)
        {
            _client = httpClient;
            Config = config;
        }

        public Task<SignatureRequestResponse> GetSignatureRequest(string signatureRequestId, int milisecondsTimeout = HelloSignConfig.DefaultApiTimeoutMiliseconds)
        {
            return _client.GetAsync(Config.GetSignatureRequestUri + "/" + signatureRequestId)
                .ContinueWith(t =>
                {
                    t.Result.EnsureSuccessStatusCode();

                    using (var sr = new StreamReader(t.Result.Content.ReadAsStreamAsync().Result))
                    using (var jtr = new JsonTextReader(sr))
                    {
                        var response = Serializer.Deserialize<SignatureRequestResponse>(jtr);
                        return response;
                    }
                });
        }

        public Task<SignatureRequestResponse> SendSignatureRequest(SendSignatureRequestData request, int milisecondsTimeout = HelloSignConfig.DefaultApiTimeoutMiliseconds)
        {
            var formData = CreateFormData(request);

            return _client.PostAsync(Config.SendSignatureRequestUri, formData).ContinueWith(t =>
                    {
                        t.Result.EnsureSuccessStatusCode();

                        using (var sr = new StreamReader(t.Result.Content.ReadAsStreamAsync().Result))
                        using (var jtr = new JsonTextReader(sr))
                        {
                            var response = Serializer.Deserialize<SignatureRequestResponse>(jtr);
                            return response;
                        }
                    });
        }

        public Task<SignatureRequestResponse> RemindSignatureRequest(RemindSignatureRequestData request, int milisecondsTimeout = HelloSignConfig.DefaultApiTimeoutMiliseconds)
        {
            var formData = CreateFormData(request);

            return _client.PostAsync(Config.RemindSignatureRequestUri + "/" + request.SignatureRequestId, formData).ContinueWith(t =>
                    {
                        t.Result.EnsureSuccessStatusCode();

                        using (var sr = new StreamReader(t.Result.Content.ReadAsStreamAsync().Result))
                        using (var jtr = new JsonTextReader(sr))
                        {
                            var response = Serializer.Deserialize<SignatureRequestResponse>(jtr);
                            return response;
                        }
                    });
        }

        private MultipartFormDataContent CreateFormData(RemindSignatureRequestData request)
        {
            var formData = new MultipartFormDataContent();
            if (!string.IsNullOrEmpty(request.EmailAddress))
                formData.AddStringContent("email_address", request.EmailAddress);
            return formData;
        }

        private MultipartFormDataContent CreateFormData(SendSignatureRequestData request)
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

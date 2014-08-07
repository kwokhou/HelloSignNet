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

namespace HelloSignNet.Core
{
    public class HelloSignApiClient
    {
        private readonly HttpClient _client;
        private JsonSerializer _serializer;
        public HelloSignConfig Config { get; set; }

        private JsonSerializer Serializer
        {
            get
            {
                return _serializer ?? (_serializer = new JsonSerializer { ContractResolver = new FrameworkHelper.UnderscoreMappingResolver() });
            }
        }

        public IFileStorage FileStorage { get; set; }

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

        public Task<string> DownloadSignatureRequestDocuments(DownloadSignatureRequestData request, string outputPath, int milisecondsTimeout = HelloSignConfig.DefaultApiTimeoutMiliseconds)
        {
            if (FileStorage == null)
                throw new IOException("Undefined FileStore in HelloSign.Config");

            var getUrl = Config.GetSignatureRequestFilesUri + "/" + request.SignatureRequestId;

            if (!string.IsNullOrEmpty(request.FileType))
                getUrl = getUrl + "?file_type=" + request.FileType;

            return _client.GetAsync(getUrl, HttpCompletionOption.ResponseHeadersRead).ContinueWith(t =>
                {
                    var response = t.Result;
                    var contentDisposition = response.Content.Headers.ContentDisposition;
                    var filename = contentDisposition.FileName.Trim('"');

                    response.EnsureSuccessStatusCode();
                    response.Content.ReadAsStreamAsync().ContinueWith(a => FileStorage.SaveFileAsync(a.Result, outputPath, filename));

                    return filename;
                });
        }

        public Task<bool> CancelSignatureRequest(string signatureRequestId, int milisecondsTimeout = HelloSignConfig.DefaultApiTimeoutMiliseconds)
        {
            return _client.PostAsync(Config.CancelSignatureRequestUri + "/" + signatureRequestId, null).ContinueWith(t =>
                {
                    var response = t.Result;
                    response.EnsureSuccessStatusCode();

                    return response.IsSuccessStatusCode;
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HelloSignNet.Core
{
    public class HelloSignClient
    {
        private readonly HttpClient _httpClient;
        private JsonSerializer _serializer;

        public HelloSignClient(HelloSignConfig config)
        {
            Config = config;
            _httpClient = FrameworkHelper.CreateBasicAuthHttpClient(Config.ApiKey, "");
            _httpClient.Timeout = new TimeSpan(0, 0, 0, 0, config.ApiTimeoutMiliseconds);
        }

        public HelloSignClient(string apiKey)
            : this(new HelloSignConfig(apiKey))
        {
            // elided
        }

        public HelloSignClient(HttpClient httpClient, HelloSignConfig config = null)
        {
            _httpClient = httpClient;
            Config = config ?? new HelloSignConfig("UNKNOWN-HELLOSIGN-API-KEY");
        }

        public HelloSignConfig Config { get; set; }

        private JsonSerializer Serializer
        {
            get
            {
                return _serializer ??
                       (_serializer =
                           new JsonSerializer {ContractResolver = new FrameworkHelper.UnderscoreMappingResolver()});
            }
        }

        public IFileStorage FileStorage { get; set; }

        public Task<HSSignatureRequestResponse> GetSignatureRequest(string signatureRequestId)
        {
            return _httpClient.GetAsync(Config.GetSignatureRequestUri + "/" + signatureRequestId)
                .ContinueWith(t =>
                {
                    using (var sr = new StreamReader(t.Result.Content.ReadAsStreamAsync().Result))
                    using (var jtr = new JsonTextReader(sr))
                    {
                        var response = Serializer.Deserialize<HSSignatureRequestResponse>(jtr);
                        return response;
                    }
                });
        }

        public Task<HSSignatureRequestResponse> SendSignatureRequest(HSSendSignatureRequestData request)
        {
            if (!request.IsValid)
                throw new ArgumentException("Invalid Signature Request Data");

            var formData = CreateFormData(request);

            return _httpClient.PostAsync(Config.SendSignatureRequestUri, formData).ContinueWith(t =>
            {
                using (var sr = new StreamReader(t.Result.Content.ReadAsStreamAsync().Result))
                using (var jtr = new JsonTextReader(sr))
                {
                    var response = Serializer.Deserialize<HSSignatureRequestResponse>(jtr);
                    return response;
                }
            });
        }

        public Task<HSSignatureRequestResponse> RemindSignatureRequest(HSRemindSignatureRequestData request)
        {
            var formData = CreateFormData(request);

            return
                _httpClient.PostAsync(Config.RemindSignatureRequestUri + "/" + request.SignatureRequestId, formData)
                    .ContinueWith(t =>
                    {
                        using (var sr = new StreamReader(t.Result.Content.ReadAsStreamAsync().Result))
                        using (var jtr = new JsonTextReader(sr))
                        {
                            var response = Serializer.Deserialize<HSSignatureRequestResponse>(jtr);
                            return response;
                        }
                    });
        }

        public Task<string> DownloadSignatureRequestDocuments(HSDownloadSignatureRequestData request, string outputPath)
        {
            if (FileStorage == null)
                throw new IOException("Undefined FileStore in HelloSign.Config");

            string getUrl = Config.GetSignatureRequestFilesUri + "/" + request.SignatureRequestId;

            if (!string.IsNullOrEmpty(request.FileType))
                getUrl = getUrl + "?file_type=" + request.FileType;

            return _httpClient.GetAsync(getUrl, HttpCompletionOption.ResponseHeadersRead).ContinueWith(t =>
            {
                HttpResponseMessage response = t.Result;
                ContentDispositionHeaderValue contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition.FileName.Trim('"');

                response.Content.ReadAsStreamAsync()
                    .ContinueWith(a => FileStorage.SaveFileAsync(a.Result, outputPath, filename));

                return filename;
            });
        }

        public Task<bool> CancelSignatureRequest(string signatureRequestId)
        {
            return
                _httpClient.PostAsync(Config.CancelSignatureRequestUri + "/" + signatureRequestId, null).ContinueWith(t =>
                {
                    var response = t.Result;
                    return response.IsSuccessStatusCode;
                });
        }

        private MultipartFormDataContent CreateFormData(HSRemindSignatureRequestData request)
        {
            var formData = new MultipartFormDataContent();
            if (!string.IsNullOrEmpty(request.EmailAddress))
                formData.AddStringContent("email_address", request.EmailAddress);
            return formData;
        }

        private MultipartFormDataContent CreateFormData(HSSendSignatureRequestData request)
        {
            var formData = new MultipartFormDataContent();

            if (!string.IsNullOrEmpty(request.Title))
                formData.AddStringContent("title", request.Title);
            if (!string.IsNullOrEmpty(request.Subject))
                formData.AddStringContent("subject", request.Subject);
            if (!string.IsNullOrEmpty(request.Message))
                formData.AddStringContent("message", request.Message);

            for (int i = 0; i < request.Signers.Count; i++)
            {
                formData.AddStringContent(string.Format("signers[{0}][name]", i), request.Signers[i].Name);
                formData.AddStringContent(string.Format("signers[{0}][email_address]", i),
                    request.Signers[i].EmailAddress);
                if (!string.IsNullOrEmpty(request.Signers[i].Order))
                    formData.AddStringContent(string.Format("signers[{0}][order]", i), request.Signers[i].Order);
                if (!string.IsNullOrEmpty(request.Signers[i].Pin))
                    formData.AddStringContent(string.Format("signers[{0}][pin]", i), request.Signers[i].Pin);
            }

            for (var i = 0; request.Files != null && i < request.Files.Count; i++)
            {
                formData.AddFileStreamContent(request.Files[i].FullName, request.Files[i].Name);
            }

            for (var i = 0; request.FileUrls != null && i < request.FileUrls.Count; i++)
            {
                formData.AddStringContent(string.Format("file_url[{0}]", i), request.FileUrls[i]);
            }

            formData.AddStringContent("test_mode", request.TestMode.ToString(CultureInfo.InvariantCulture));

            return formData;
        }

        public Task<HSAccountResponse> GetAccount()
        {
            return _httpClient.GetAsync(Config.GetAcountUri)
                .ContinueWith(t =>
                {
                    using (var sr = new StreamReader(t.Result.Content.ReadAsStreamAsync().Result))
                    using (var jtr = new JsonTextReader(sr))
                    {
                        var response = Serializer.Deserialize<HSAccountResponse>(jtr);
                        return response;
                    }
                });
        }
    }
}
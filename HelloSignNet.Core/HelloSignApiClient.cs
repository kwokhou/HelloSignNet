﻿using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HelloSignNet.Core
{
    public class HelloSignApiClient
    {
        private readonly HttpClient _client;
        private JsonSerializer _serializer;

        public HelloSignApiClient(HelloSignConfig config)
        {
            Config = config;
            _client = FrameworkHelper.CreateBasicAuthHttpClient(Config.ApiKey, "");
            _client.Timeout = new TimeSpan(0, 0, 0, 0, config.ApiTimeoutMiliseconds);
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

        public Task<SignatureRequestResponse> GetSignatureRequest(string signatureRequestId)
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

        public Task<SignatureRequestResponse> SendSignatureRequest(SendSignatureRequestData request)
        {
            MultipartFormDataContent formData = CreateFormData(request);

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

        public Task<SignatureRequestResponse> RemindSignatureRequest(RemindSignatureRequestData request)
        {
            MultipartFormDataContent formData = CreateFormData(request);

            return
                _client.PostAsync(Config.RemindSignatureRequestUri + "/" + request.SignatureRequestId, formData)
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

        public Task<string> DownloadSignatureRequestDocuments(DownloadSignatureRequestData request, string outputPath)
        {
            if (FileStorage == null)
                throw new IOException("Undefined FileStore in HelloSign.Config");

            string getUrl = Config.GetSignatureRequestFilesUri + "/" + request.SignatureRequestId;

            if (!string.IsNullOrEmpty(request.FileType))
                getUrl = getUrl + "?file_type=" + request.FileType;

            return _client.GetAsync(getUrl, HttpCompletionOption.ResponseHeadersRead).ContinueWith(t =>
            {
                HttpResponseMessage response = t.Result;
                ContentDispositionHeaderValue contentDisposition = response.Content.Headers.ContentDisposition;
                string filename = contentDisposition.FileName.Trim('"');

                response.EnsureSuccessStatusCode();
                response.Content.ReadAsStreamAsync()
                    .ContinueWith(a => FileStorage.SaveFileAsync(a.Result, outputPath, filename));

                return filename;
            });
        }

        public Task<bool> CancelSignatureRequest(string signatureRequestId)
        {
            return
                _client.PostAsync(Config.CancelSignatureRequestUri + "/" + signatureRequestId, null).ContinueWith(t =>
                {
                    HttpResponseMessage response = t.Result;
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
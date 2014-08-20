using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using HelloSignNet.Core;

namespace HelloSignNet.Tests
{
    public class SigniningRequestTests
    {
        [Fact]
        public void SampleTest1()
        {
            Assert.Equal(4, 2 + 2);
        }

        private HelloSignApiClient _helloClient;

        private const string TestApiKey = "RAMDOM API";
        private const string TestBaseUri = "RAMDOM URI";

        public SigniningRequestTests()
        {
            var config = new HelloSignConfig(TestApiKey, TestBaseUri) ;
            _helloClient = new HelloSignApiClient(config) { FileStorage = new WindowsFileStorage() };
        }

        public void SendSignatureRequestTests()
        {
            //var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK);
            //fakeResponse.Content = new StringContent(););
            //var httpClient = new HttpClient(new FakeHttpHandler()
            //{
            //    Response = =
            //});
        }

        [Fact]
        public void GetAccount_Success_Response_Test()
        {
            var json = File.ReadAllText("TestData\\GetAccount-OK.json");
            var fakeHandler = new FakeHttpHandler(HttpStatusCode.OK, json);

            using (var httpClient = new HttpClient(fakeHandler))
            {
                var apiClient = new HelloSignApiClient(httpClient);
                var t = apiClient.GetAccount();

                var expected = new HSAccount
                {
                    AccountId = "abcXYZ",
                    EmailAddress = "me@hellosign.com",
                    IsPaidHS = true,
                    IsPaidHF = false,
                    CallbackUrl = null,
                    RoleCode = null,
                    Quotas = new HSQuotas
                    {
                        ApiSignatureRequest = 1250,
                        DocumentsLeft = null,
                        TemplatesLeft = null
                    }
                };

                Assert.Equal(expected, t.Result.Account);
            }
        }

        [Fact]
        public void Get_Unauthorized_Error_Test()
        {
            var json = File.ReadAllText("TestData\\Error.json");
            var fakeHandler = new FakeHttpHandler(HttpStatusCode.Unauthorized, json);

            using (var httpClient = new HttpClient(fakeHandler))
            {
                var apiClient = new HelloSignApiClient(httpClient);
                var t = apiClient.GetAccount();

                var expected = new HSError
                {
                    ErrorMsg = "Unauthorized user.",
                    ErrorName = "unauthorized"
                };

                Assert.Equal(expected, t.Result.Error);
            }
        }

        [Fact]
        public void Get_Warning_Response_Test()
        {
            var json = File.ReadAllText("TestData\\Warning.json");
            var fakeHandler = new FakeHttpHandler(HttpStatusCode.OK, json);

            using (var httpClient = new HttpClient(fakeHandler))
            {
                var apiClient = new HelloSignApiClient(httpClient);
                var t = apiClient.GetAccount();

                var warnings = new List<HSWarning>();
                var expected = new HSWarning
                {
                    WarningMsg = "This SignatureRequest will be placed on hold until the user confirms their email address.",
                    WarningName = "unconfirmed"
                };
                warnings.Add(expected);

                Assert.Equal(warnings[0], t.Result.Warnings[0]);
            }
        }

        [Fact]
        public void GetSignatureRequestTest()
        {
            var t = _helloClient.GetSignatureRequest("3d82cca3e3bb28a116f69508548a0497811215f1");
            Assert.Equal("3d82cca3e3bb28a116f69508548a0497811215f1",t.Result.SignatureRequest.SignatureRequestId);
        }

        [Fact]
        public void SendSignatureRequestTest()
        {
            var signatureRequest = new HSSendSignatureRequestData();

            var t = _helloClient.SendSignatureRequest(signatureRequest);

            Assert.Equal("1001221cdb7c474cfaef5cf0bf0eacb0639caa34", t.Result.SignatureRequest.SignatureRequestId);
        }

        [Fact]
        public void RemindSignatureRequestTest()
        {
            var remindSignatureRequestData = new HSRemindSignatureRequestData
            {
                SignatureRequestId = "c406d5a99cce33ee0026ade5a939183a833c195b",
                EmailAddress = "andy@estorm.com"
            };

            var t = _helloClient.RemindSignatureRequest(remindSignatureRequestData);

            Assert.Equal("c406d5a99cce33ee0026ade5a939183a833c195b", t.Result.SignatureRequest.SignatureRequestId);
        }

        [Fact]
        public void CancelSignatureRequestTest()
        {
            var t = _helloClient.CancelSignatureRequest("c406d5a99cce33ee0026ade5a939183a833c195b");
            Assert.True(t.Result);
        }

        [Fact]
        public void DownloadSignatureRequestDocumentsTest()
        {
            var downloadSignatureRequestData = new HSDownloadSignatureRequestData { SignatureRequestId = "ab0f0999743c14fc3a5fa0d830f7220899a1ab58", FileType = "pdf" };

            var t = _helloClient.DownloadSignatureRequestDocuments(downloadSignatureRequestData, @"c:\temp");

            t.Wait();

            Assert.True(File.Exists(@"c:\temp\"+ t.Result));
        }
    }

    public class WindowsFileStorage : IFileStorage
    {
        public string LoadFileAsync(string filename)
        {
            throw new System.NotImplementedException();
        }

        public void SaveFileAsync(Stream stream, string filepath, string filename)
        {
            var fullpath = Path.Combine(filepath, filename);

            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }

            using (var fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fs);
                fs.Flush();
            }
        }
    }

    public class FakeHttpHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpHandler(HttpStatusCode responseStatusCode, string responseContent)
        {
            _response = new HttpResponseMessage(responseStatusCode) {Content = new StringContent(responseContent)};
        }

        public FakeHttpHandler(HttpStatusCode responseStatusCode, Stream responseContent)
        {
            _response = new HttpResponseMessage(responseStatusCode) { Content = new StreamContent(responseContent) };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var res = new TaskCompletionSource<HttpResponseMessage>();
            res.SetResult(_response);
            return res.Task;
        }
    }
}

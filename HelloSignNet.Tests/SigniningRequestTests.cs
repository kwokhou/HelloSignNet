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
        public void Get_Account_Response_Test()
        {
            using (var httpClient = FakeClientWithJsonResponse("TestData\\GetAccount-OK.json"))
            {
                var apiClient = new HelloSignClient(httpClient);
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
            using (var httpClient = FakeClientWithJsonResponse("TestData\\Error.json"))
            {
                var apiClient = new HelloSignClient(httpClient);
                var t = apiClient.GetAccount();

                var expected = new HSError
                {
                    ErrorMsg = "Unauthorized user.",
                    ErrorName = "unauthorized"
                };

                Assert.False(t.Result.IsSuccess);
                Assert.True(t.Result.HasError);
                Assert.Equal(expected, t.Result.Error);
            }
        }

        [Fact]
        public void Get_Warning_Response_Test()
        {
            using (var httpClient = FakeClientWithJsonResponse("TestData\\Warning.json"))
            {
                var apiClient = new HelloSignClient(httpClient);
                var t = apiClient.GetAccount();

                var warnings = new List<HSWarning>();
                var expected = new HSWarning
                {
                    WarningMsg = "This SignatureRequest will be placed on hold until the user confirms their email address.",
                    WarningName = "unconfirmed"
                };
                warnings.Add(expected);


                Assert.False(t.Result.IsSuccess);
                Assert.True(t.Result.HasWarnings);
                Assert.Equal(warnings[0], t.Result.Warnings[0]);
            }
        }

        [Fact]
        public void Get_SignatureRequest_Response_Test()
        {
            using (var httpClient = FakeClientWithJsonResponse("TestData\\GetSignatureRequest.json"))
            {
                var apiClient = new HelloSignClient(httpClient);
                var t = apiClient.GetSignatureRequest("fa5c8a0b0f492d768749333ad6fcc214c111e967");

                Assert.Equal("fa5c8a0b0f492d768749333ad6fcc214c111e967", t.Result.SignatureRequest.SignatureRequestId);
            }
        }

        [Fact]
        public void Send_SignatureRequest_Files_Response_Test()
        {
            var requestData = new HSSendSignatureRequestData
            {
                Title = "NDA for Project X",
                Subject = "NDA We Talk about",
                Message = "Bla Bla Bla",
                Signers = new List<HSSigner> {new HSSigner {Name = "John", EmailAddress = "john@example.com"}},
                Files = new List<FileInfo> {new FileInfo("TestData\\pdf-sample.pdf")}
            };

            using (var httpClient = FakeClientWithJsonResponse("TestData\\SignatureRequest.json"))
            {
                var apiClient = new HelloSignClient(httpClient);
                var t = apiClient.SendSignatureRequest(requestData);

                Assert.Equal("a9f4825edef25f47e7b", t.Result.SignatureRequest.SignatureRequestId);
            }
        }

        [Fact]
        public void Send_SignatureRequest_FileUrls_Response_Test()
        {
            var requestData = new HSSendSignatureRequestData
            {
                Title = "NDA for Project X",
                Subject = "NDA We Talk about",
                Message = "Bla Bla Bla",
                Signers = new List<HSSigner> { new HSSigner { Name = "John", EmailAddress = "john@example.com" } },
                FileUrls = new List<string> { "http://www.hollywood-arts.org/wp-content/uploads/2014/05/pdf-sample.pdf" }
            };

            using (var httpClient = FakeClientWithJsonResponse("TestData\\SignatureRequest.json"))
            {
                var apiClient = new HelloSignClient(httpClient);
                var t = apiClient.SendSignatureRequest(requestData);

                Assert.Equal("a9f4825edef25f47e7b", t.Result.SignatureRequest.SignatureRequestId);
            }
        }

        public static HttpClient FakeClientWithJsonResponse(string jsonFilepath)
        {
            if (!string.IsNullOrEmpty(jsonFilepath))
            {
                var json = File.ReadAllText(jsonFilepath);
                var fakeHandler = new FakeHttpHandler(json);
                return new HttpClient(fakeHandler);
            }
            else
            {
                var fakeHandler = new FakeHttpHandler(null);
                return new HttpClient(fakeHandler);
            }
        }

        [Fact]
        public void RemindSignatureRequestTest()
        {
            var requestData = new HSRemindSignatureRequestData();

            using (var httpClient = FakeClientWithJsonResponse("TestData\\SignatureRequest-Remind.json"))
            {
                var apiClient = new HelloSignClient(httpClient);
                var t = apiClient.RemindSignatureRequest(requestData);

                Assert.Equal("2f9781e1a8e2045224d808c153c2e1d3df6f8f2f", t.Result.SignatureRequest.SignatureRequestId);
            }
        }

        [Fact]
        public void CancelSignatureRequestTest()
        {
            using (var httpClient = FakeClientWithJsonResponse(null))
            {
                var apiClient = new HelloSignClient(httpClient);
                var t = apiClient.CancelSignatureRequest("abcdeng");

                Assert.Equal(true, t.Result);
            }
        }

        //[Fact]
        //public void DownloadSignatureRequestDocumentsTest()
        //{
        //    var downloadSignatureRequestData = new HSDownloadSignatureRequestData { SignatureRequestId = "ab0f0999743c14fc3a5fa0d830f7220899a1ab58", FileType = "pdf" };

        //    var t = _helloClient.DownloadSignatureRequestDocuments(downloadSignatureRequestData, @"c:\temp");

        //    t.Wait();

        //    Assert.True(File.Exists(@"c:\temp\" + t.Result));
        //}
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

        public FakeHttpHandler(string responseContent)
        {
            _response = new HttpResponseMessage() { Content = new StringContent(responseContent ?? "") };
        }

        public FakeHttpHandler(HttpStatusCode responseStatusCode, Stream responseContent)
        {
            _response = new HttpResponseMessage() { Content = new StreamContent(responseContent) };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var res = new TaskCompletionSource<HttpResponseMessage>();
            res.SetResult(_response);
            return res.Task;
        }
    }
}

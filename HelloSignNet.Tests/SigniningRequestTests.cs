using System;
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
        public void GetAccount()
        {
            using (var fs = new FileStream("TestData\\GetAccount-OK.json", FileMode.Open, FileAccess.Read))
            {
                var fakeHandler = new FakeHttpHandler(HttpStatusCode.OK, fs);

                using (var httpClient = new HttpClient(fakeHandler))
                {
                    var apiClient = new HelloSignApiClient(httpClient);
                    var t = apiClient.GetAccount();

                    Assert.Equal("abcXYZ", t.Result.Account.AccountId);
                }
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
            var signatureRequest = new SendSignatureRequestData();

            var t = _helloClient.SendSignatureRequest(signatureRequest);

            Assert.Equal("1001221cdb7c474cfaef5cf0bf0eacb0639caa34", t.Result.SignatureRequest.SignatureRequestId);
        }

        [Fact]
        public void RemindSignatureRequestTest()
        {
            var remindSignatureRequestData = new RemindSignatureRequestData
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
            var downloadSignatureRequestData = new DownloadSignatureRequestData { SignatureRequestId = "ab0f0999743c14fc3a5fa0d830f7220899a1ab58", FileType = "pdf" };

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


using System;
using System.Collections.Generic;
using System.IO;
using HelloSignNet.Core;
using Xunit;

namespace HelloSignNet.Tests
{
    public class ObjectCreationTests 
    {
        [Fact]
        public void Can_Create_HelloSignClient_with_Api_Key()
        {
            var apiCient = new HelloSignClient("Example-of-Api-Key");
            Assert.NotNull(apiCient.Config);
        }

        [Fact]
        public void Can_Create_HelloSignClient_with_Config()
        {
            var exampleConfig = new HelloSignConfig("Example-of-Api-Key", "http://path/to/api/endpoing", 123456);
            var apiClient = new HelloSignClient(exampleConfig);

            Assert.NotNull(apiClient.Config);
        }

        [Fact]
        public void Can_verify_Valid_SendSignatureRequestData_FileUrls()
        {
            var data = new HSSendSignatureRequestData
            {
                Signers = new List<HSSigner> {new HSSigner {Name = "John", EmailAddress = "john@example.com"}},
                FileUrls = new List<string> {"http://example.com/pdf"}
            };

            Assert.True(data.IsValid);
        }

        [Fact]
        public void Can_verify_Valid_SendSignatureRequestData_Files()
        { 
            var data = new HSSendSignatureRequestData
            {
                Signers = new List<HSSigner> {new HSSigner {Name = "John", EmailAddress = "john@example.com"}},
                Files = new List<FileInfo> { new FileInfo("dummyfile.pdf") },
            };
            Assert.True(data.IsValid);
        }

        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_Files()
        {
            var data = new HSSendSignatureRequestData
            {
                Files = new List<FileInfo> { new FileInfo("dummyfile.pdf") },
            };
            Assert.False(data.IsValid);
        }

        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_with_empty_Files()
        {
            var data = new HSSendSignatureRequestData
            {
                Signers = new List<HSSigner> {new HSSigner {Name = "John", EmailAddress = "john@example.com"}},
                Files = new List<FileInfo>() {new FileInfo("TestFile.xml")}
            };
            Assert.Equal(true, data.IsValid);
        }

        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_FileUrls()
        {
            var data = new HSSendSignatureRequestData
            {
                FileUrls = new List<string> {"http://example.com/pdf"},
            };
            Assert.False(data.IsValid);
        }

        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_empty_FileUrls()
        {
            var data = new HSSendSignatureRequestData
            {
                FileUrls = new List<string>()
            };
            Assert.False(data.IsValid);
        }


        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_empty_Files_and_FileUrls()
        {
            var data = new HSSendSignatureRequestData
            {
                Files = new List<FileInfo>(),
                FileUrls = new List<string>()
            };

            Assert.False(data.IsValid);
        }

        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_non_empty_Files_and_FileUrls()
        {
            var data = new HSSendSignatureRequestData
            {
                Files = new List<FileInfo> { new FileInfo("dummyfile.pdf") },
                FileUrls = new List<string> {"http://example.com/pdf"},
            };
            Assert.Equal(false, data.IsValid);
        }


        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_FileUrls_and_Files()
        {
            var data = new HSSendSignatureRequestData
            {
                Signers = new List<HSSigner> {new HSSigner {Name = "John", EmailAddress = "john@example.com"}},
                Files = new List<FileInfo> { new FileInfo("dummyfile.pdf") },
                FileUrls = new List<string> {"http://example.com/pdf"},
            };
            Assert.Equal(false, data.IsValid);
        }

        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_Signer_info_name()
        {
            var data = new HSSendSignatureRequestData
            {
                Signers = new List<HSSigner> {new HSSigner {Name = "John" }},
                FileUrls = new List<string> {"http://example.com/pdf"},
            };
            Assert.Equal(false, data.IsValid);
        }

        [Fact]
        public void Can_verify_invalid_SendSignatureRequestData_Signer_info_email()
        {
            var data = new HSSendSignatureRequestData
            {
                Signers = new List<HSSigner> {new HSSigner {EmailAddress = "John@example.com" }},
                FileUrls = new List<string> {"http://example.com/pdf"},
            };
            Assert.Equal(false, data.IsValid);
        }
    }
}

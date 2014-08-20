using System.Collections.Generic;
using System.IO;

namespace HelloSignNet.Core
{
    public interface IFileStorage
    {
        void SaveFileAsync(Stream stream, string filepath, string filename);
        string LoadFileAsync(string filename);
    }

    public class SendSignatureRequestData
    {
        public SendSignatureRequestData()
        {
            Signers = new List<Signer>();
            CcEmailAddresses = new List<string>();
            //Files = new List<FileInfo>();
            FileUrls = new List<string>();
            TestMode = 0;
        }

        public string Title { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string SigningRedirectUrl { get; set; }
        public List<Signer> Signers { get; set; }
        public List<string> CcEmailAddresses { get; set; }
        //public List<FileInfo> Files { get; set; }
        public List<string> FileUrls { get; set; }
        public int TestMode { get; set; } // true=1, false=0
    }

    public class RemindSignatureRequestData
    {
        public string SignatureRequestId { get; set; }
        public string EmailAddress { get; set; }
    }

    public class DownloadSignatureRequestData
    {
        public string SignatureRequestId { get; set; }
        public string FileType { get; set; }
    }

    public class SignatureRequest
    {
        public string SignatureRequestId { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool TestMode { get; set; }
        public bool IsComplete { get; set; }
        public bool HasError { get; set; }
        public List<string> CustomFields { get; set; }
        public List<ResponseData> ResponseData { get; set; }
        public string SigningUrl { get; set; }
        public string SigningRedirectUrl { get; set; }
        public string FinalCopyUri { get; set; }
        public string FilesUrl { get; set; }
        public string DetailsUrl { get; set; }
        public string RequesterEmailAddress { get; set; }
        public List<Signature> Signatures { get; set; }
        public List<string> CcEmailAddresses { get; set; }
    }

    public class SignatureRequestResponse
    {
        public SignatureRequest SignatureRequest { get; set; }
    }

    public class Signature
    {
        public string SignatureId { get; set; }
        public string SignerEmailAddress { get; set; }
        public string SignerName { get; set; }
        public string Order { get; set; }
        public string StatusCode { get; set; }
        public string SignedAt { get; set; }
        public string LastViewedAt { get; set; }
        public string LastRemindedAt { get; set; }
        public bool HasPin { get; set; }
    }

    public class Signer
    {
        public Signer(string name, string emailAddress)
        {
            Name = name;
            EmailAddress = emailAddress;
        }

        public string Name { get; private set; }
        public string EmailAddress { get; private set; }
        public string Order { get; set; } // integer in string format
        public string Pin { get; set; }
    }

    public class ResponseData
    {
        public string ApiId { get; set; }
        public string Name { get; set; }
        public string SignatureId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
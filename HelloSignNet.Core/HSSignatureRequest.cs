using System.Collections.Generic;

namespace HelloSignNet.Core
{
    public class HSSignatureRequest
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
        public List<HSResponseData> ResponseData { get; set; }
        public string SigningUrl { get; set; }
        public string SigningRedirectUrl { get; set; }
        public string FinalCopyUri { get; set; }
        public string FilesUrl { get; set; }
        public string DetailsUrl { get; set; }
        public string RequesterEmailAddress { get; set; }
        public List<HSSignature> Signatures { get; set; }
        public List<string> CcEmailAddresses { get; set; }
    }

    public class HSSignatureRequestResponse : HSBaseResponse
    {
        public HSSignatureRequest SignatureRequest { get; set; }
    }
}

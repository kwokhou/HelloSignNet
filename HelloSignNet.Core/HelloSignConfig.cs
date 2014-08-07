namespace HelloSignNet.Core
{
    public class HelloSignConfig
    {
        public const int DefaultApiTimeoutMiliseconds = 10000; // 10 seconds

        public string ApiKey { get; set; }
        public string BaseUri { get; set; }

        public string GetSignatureRequestUri { get; set; }
        public string GetSignatureRequestListUri { get; set; }
        public string GetSignatureRequestFilesUri { get; set; }

        public string SendSignatureRequestUri { get; set; }
        public string SendWithTemplateSignatureRequestUri { get; set; }
        public string SendEmbeddedSignatureRequestUri { get; set; }
        public string SendEmbeddedWithTemplateSignatureRequestUri { get; set; }

        public string RemindSignatureRequestUri { get; set; }
        public string CancelSignatureRequestUri { get; set; }

        public HelloSignConfig()
        {
            // elided
        }

        public HelloSignConfig(string apiKey, string baseUri = "https://api.hellosign.com/v3/")
        {
            ApiKey = apiKey;
            BaseUri = baseUri;

            GetSignatureRequestUri = BaseUri + "signature_request";
            GetSignatureRequestListUri = BaseUri + "signature_request/list";
            GetSignatureRequestFilesUri = BaseUri + "signature_request/files";

            SendSignatureRequestUri = BaseUri + "signature_request/send";
            SendWithTemplateSignatureRequestUri = BaseUri + "signature_request/send_with_template";
            SendEmbeddedSignatureRequestUri = BaseUri + "signature_request/create_embedded";
            SendEmbeddedWithTemplateSignatureRequestUri = BaseUri + "signature_request/create_embedded_with_template";

            RemindSignatureRequestUri = BaseUri + "signature_request/remind";
            CancelSignatureRequestUri = BaseUri + "signature_request/cancel";
        }
    }
}
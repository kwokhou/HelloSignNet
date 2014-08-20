namespace HelloSignNet.Core
{
    public class HelloSignConfig
    {
        public const string DefaultBaseUri = "https://api.hellosign.com/v3/";
        public const int DefaultApiTimeoutMiliseconds = 60000; // 60 seconds
        public readonly string AddMemberToTeamUri;
        public readonly string AddUserToTemplateUri;

        public readonly string ApiKey;
        public readonly int ApiTimeoutMiliseconds;
        public readonly string BaseUri;
        public readonly string CancelSignatureRequestUri;

        public readonly string CreateAccountUri;
        public readonly string CreateEmbeddedUnclaimedDraftUri;
        public readonly string CreateTeamUri;
        public readonly string CreateUnclaimedDraftUri;
        public readonly string DeleteTeamUri;
        public readonly string GetAcountUri;
        public readonly string GetEmbeddedSigningObject;
        public readonly string GetSignatureRequestFilesUri;
        public readonly string GetSignatureRequestListUri;
        public readonly string GetSignatureRequestUri;
        public readonly string GetTeamUri;
        public readonly string GetTemplateListUri;
        public readonly string GetTemplateUri;
        public readonly string RemindSignatureRequestUri;
        public readonly string RemoveMemberFromTeamUri;
        public readonly string RemoveUserFromTemplateUri;
        public readonly string SendEmbeddedSignatureRequestUri;
        public readonly string SendEmbeddedWithTemplateSignatureRequestUri;
        public readonly string SendSignatureRequestUri;
        public readonly string SendWithTemplateSignatureRequestUri;
        public readonly string UpdateAccountUri;

        public readonly string UpdateTeamUri;
        public readonly string VerifyAccountUri;

        public HelloSignConfig(string apiKey, string baseUri = DefaultBaseUri,
            int apiTimeoutMiliseconds = DefaultApiTimeoutMiliseconds)
        {
            ApiKey = apiKey;
            ApiTimeoutMiliseconds = apiTimeoutMiliseconds;
            BaseUri = baseUri;

            GetAcountUri = BaseUri + "account";
            UpdateAccountUri = BaseUri + "account";
            CreateAccountUri = BaseUri + "account/create";
            VerifyAccountUri = BaseUri + "account/verify";

            GetSignatureRequestUri = BaseUri + "signature_request";
            GetSignatureRequestListUri = BaseUri + "signature_request/list";
            GetSignatureRequestFilesUri = BaseUri + "signature_request/files";
            SendSignatureRequestUri = BaseUri + "signature_request/send";
            SendWithTemplateSignatureRequestUri = BaseUri + "signature_request/send_with_template";

            SendEmbeddedSignatureRequestUri = BaseUri + "signature_request/create_embedded";
            SendEmbeddedWithTemplateSignatureRequestUri = BaseUri + "signature_request/create_embedded_with_template";
            RemindSignatureRequestUri = BaseUri + "signature_request/remind";
            CancelSignatureRequestUri = BaseUri + "signature_request/cancel";

            GetTemplateUri = BaseUri + "template";
            GetTemplateListUri = BaseUri + "template/list";
            AddUserToTemplateUri = BaseUri + "template/add_user";
            RemoveUserFromTemplateUri = BaseUri + "template/remove_user";

            GetTeamUri = BaseUri + "team";
            CreateTeamUri = BaseUri + "team/create";
            UpdateTeamUri = BaseUri + "team";
            DeleteTeamUri = BaseUri + "team/destroy";
            AddMemberToTeamUri = BaseUri + "team/add_member";
            RemoveMemberFromTeamUri = BaseUri + "team/remove_member";

            CreateUnclaimedDraftUri = BaseUri + "unclaimed_draft/create";
            CreateEmbeddedUnclaimedDraftUri = BaseUri + "unclaimed_draft/create_embedded";

            GetEmbeddedSigningObject = BaseUri + "embedded/sign_url";
        }
    }
}
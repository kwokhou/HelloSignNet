using System;
using Newtonsoft.Json;

namespace HelloSignNet.Core
{
    public class HSAccount : IEquatable<HSAccount>
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }
        [JsonProperty("is_paid_hs")]
        public bool IsPaidHS { get; set; }
        [JsonProperty("is_paid_hf")]
        public bool IsPaidHF { get; set; }
        public HSQuotas Quotas { get; set; }
        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }
        [JsonProperty("role_code")]
        public string RoleCode { get; set; }

        public bool Equals(HSAccount other)
        {
            var res = (
                AccountId == other.AccountId &&
                EmailAddress == other.EmailAddress &&
                IsPaidHS == other.IsPaidHS &&
                IsPaidHF == other.IsPaidHF &&
                CallbackUrl == other.CallbackUrl &&
                RoleCode == other.RoleCode && Quotas.Equals(other.Quotas));

            return res;
        }
    }

    public class HSQuotas : IEquatable<HSQuotas>
    {
        [JsonProperty("api_signature_requests_left")]
        public int? ApiSignatureRequest { get; set; }
        [JsonProperty("documents_left")]
        public int? DocumentsLeft { get; set; }
        [JsonProperty("templates_left")]
        public int? TemplatesLeft { get; set; }

        public bool Equals(HSQuotas other)
        {
            var res = (ApiSignatureRequest == other.ApiSignatureRequest &&
                DocumentsLeft == other.DocumentsLeft &&
                TemplatesLeft == other.TemplatesLeft);
            return res;
        }
    }

    public class HSAccountResponse : HSBaseResponse 
    {
        public HSAccount Account { get; set; }
    }
}
namespace HelloSignNet.Core
{
    public class HSSignature
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
}
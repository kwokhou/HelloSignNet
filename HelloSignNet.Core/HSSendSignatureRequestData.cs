using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HelloSignNet.Core
{
    public class HSSendSignatureRequestData
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string SigningRedirectUrl { get; set; }
        public List<HSSigner> Signers { get; set; }
        public List<string> CcEmailAddresses { get; set; }
        public List<FileInfo> Files { get; set; }
        public List<string> FileUrls { get; set; }
        public int TestMode { get; set; } // true=1, false=0

        public bool IsValid 
        {
            get
            {
                if (Files == null && FileUrls == null)
                {
                    return false;
                }

                if (Files != null && FileUrls == null)
                {
                    if (!Files.Any())
                        return true;
                }

                if (Files == null && FileUrls != null)
                {
                    if (!FileUrls.Any())
                        return false;
                }

                if (Signers == null || Signers.Count == 0)
                    return false;

                if (Signers.Any(s => string.IsNullOrEmpty(s.Name) || string.IsNullOrEmpty(s.EmailAddress)))
                    return false;

                return true;
            }
        }
    }
}

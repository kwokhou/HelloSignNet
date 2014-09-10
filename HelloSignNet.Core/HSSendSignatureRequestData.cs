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
        public int TestMode { get; set; } // true=1, false=0, default=0
        public int UseTextTags { get; set; } // true=1, false=0, default=0
        public int HideTextTags { get; set; } // true=1, false=0, default=0

        public bool IsValid 
        {
            get
            {
                if (Files == null && FileUrls == null)
                {
                    return false;
                }

                // API does not accept both files param in a request
                if (Files != null && FileUrls != null && Files.Any() && FileUrls.Any())
                    return false;

                if (Files != null && FileUrls == null)
                {
                    if (!Files.Any())
                        return false;
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

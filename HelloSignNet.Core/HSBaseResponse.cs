using System.Collections.Generic;

namespace HelloSignNet.Core
{
    public abstract class HSBaseResponse
    {
        public HSError Error { get; set;}
        public List<HSWarning> Warnings { get; set; }
        
        public bool IsSuccess
        {
            get
            {
                return Error == null && (Warnings == null || Warnings.Count == 0);
            }
        }

        public bool HasError
        {
            get { return Error != null; }
        }

        public bool HasWarnings
        {
            get { return Warnings != null && Warnings.Count > 0; }
        }
    }
}
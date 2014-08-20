using System;
using Newtonsoft.Json;

namespace HelloSignNet.Core
{
    public class HSWarning : IEquatable<HSWarning>
    {
        [JsonProperty("warning_msg")]
        public string WarningMsg { get; set; }
        [JsonProperty("warning_name")]
        public string WarningName { get; set; }
        public bool Equals(HSWarning other)
        {
            var res = (WarningMsg == other.WarningMsg && WarningName == other.WarningName);
            return res;
        }
    }
}
using System;
using Newtonsoft.Json;

namespace HelloSignNet.Core
{
    public class HSError : IEquatable<HSError>
    {
        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }
        [JsonProperty("error_name")]
        public string ErrorName { get; set; }

        public bool Equals(HSError other)
        {
            var res = (ErrorMsg == other.ErrorMsg && ErrorName == other.ErrorName);
            return res;
        }
    }
}
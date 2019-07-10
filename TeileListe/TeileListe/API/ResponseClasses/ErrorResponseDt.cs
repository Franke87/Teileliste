using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeileListe.API.ResponseClasses
{
    [DataContract]
    public class ErrorResponseDt
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "data")]
        public List<string> Data { get; set; }

        [DataMember(Name = "messages")]
        public List<string> Messages { get; set; }
    }
}

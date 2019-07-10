using System.Runtime.Serialization;

namespace TeileListe.API.PostClasses
{
    [DataContract]
    public class AddMessungDto
    {
        [DataMember(Name = "product")]
        public string ProduktId { get; set; }

        [DataMember(Name = "photo")]
        public string ImageBase64 { get; set; }

        [DataMember(Name = "weight_real")]
        public decimal Gewicht { get; set; }
    }

    [DataContract]
    public class ResponseMessungDto
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "data")]
        public ResponseDataMessungDto Data { get; set; }
    }

    [DataContract]
    public class ResponseDataMessungDto
    {
        [DataMember(Name = "product")]
        public ResponseProduktMessungDto Produkt { get; set; }
    }

    [DataContract]
    public class ResponseProduktMessungDto
    {
        [DataMember(Name = "id")]
        public decimal ProduktId { get; set; }

        [DataMember(Name = "url")]
        public string ProduktUrl { get; set; }
    }
}

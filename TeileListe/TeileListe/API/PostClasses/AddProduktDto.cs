using System.Runtime.Serialization;

namespace TeileListe.API.PostClasses
{
    [DataContract]
    public class AddProduktDto
    {
        [DataMember(Name = "category")]
        public string Kategorie { get; set; }

        [DataMember(Name = "manufacturer")]
        public string Hersteller { get; set; }

        [DataMember(Name = "name")]
        public string Beschreibung { get; set; }

        [DataMember(Name = "size")]
        public string Groesse { get; set; }

        [DataMember(Name = "year")]
        public string Jahr { get; set; }

        [DataMember(Name = "weight_claimed")]
        public decimal GewichtHersteller { get; set; }

        [DataMember(Name = "weight_real")]
        public decimal Gewicht { get; set; }

        [DataMember(Name = "comment")]
        public string Kommentar { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "photo")]
        public string ImageBase64 { get; set; }
    }
}

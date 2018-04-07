using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeileListe.API.ResponseClasses
{
    [DataContract]
    internal class ResponseProduktListeDto
    {
        [DataMember(Name = "data")]
        public ResponseProduktDataDto Data { get; set; }
    }

    [DataContract]
    internal class ResponseProduktDataDto
    {
        [DataMember(Name = "products")]
        public List<ResponseProduktBaseDto> Produkte { get; set; }
    }

    [DataContract]
    internal class ResponseProduktManufacturerDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    internal class ResponseProduktEinzelnDto
    {
        [DataMember(Name = "data")]
        public ResponseProduktBaseDto Data { get; set; }
    }

    [DataContract]
    internal class ResponseProduktBaseDto
    {
        [DataMember(Name = "product")]
        public ResponseProduktObjectDto Produkt { get; set; }
    }

    [DataContract]
    internal class ResponseProduktObjectDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "category")]
        public ResponseProduktKategorieDto Kategorie { get; set; }

        [DataMember(Name = "manufacturer")]
        public ResponseProduktManufacturerDto Manufacturer { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "size")]
        public string Groesse { get; set; }

        [DataMember(Name = "year")]
        public string Jahr { get; set; }

        [DataMember(Name = "weight_average")]
        public decimal Gewicht { get; set; }
    }

    [DataContract]
    internal class ResponseProduktKategorieDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}

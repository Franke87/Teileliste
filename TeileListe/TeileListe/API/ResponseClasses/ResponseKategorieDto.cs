using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeileListe.API.ResponseClasses
{
    [DataContract]
    public class ResponseKategorieDto
    {
        [DataMember(Name = "id")]
        public string KategorieId { get; set; }

        [DataMember(Name = "parent_id")]
        public string ParentId { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "can_contain_products")]
        public bool EnthaeltProdukte { get; set; }

        [DataMember(Name = "counter_products")]
        public double AnzahlProdukte { get; set; }

        [DataMember(Name = "counter_images")]
        public double AnzahlBilder { get; set; }

        [DataMember(Name = "subcategories")]
        public List<ResponseKategorieDto> Unterkategorien { get; set; }
    }

    [DataContract]
    public class ResponseKategorieBaseDto
    {
        [DataMember(Name = "data")]
        public List<ResponseKategorieDto> KategorienListe { get; set; }
    }
}
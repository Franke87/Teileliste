namespace TeileListe.Common.Dto
{
    public class RestteilDto
    {
        public string Guid { get; set; }
        public string Komponente { get; set; }
        public string Hersteller { get; set; }
        public string Beschreibung { get; set; }
        public string Groesse { get; set; }
        public string Jahr { get; set; }
        public string DatenbankId { get; set; }
        public string DatenbankLink { get; set; }
        public int Preis { get; set; }
        public int Gewicht { get; set; }
    }
}

namespace TeileListe.MessungHochladen.Dto
{
    public class ProduktHochladenDto
    {
        public string Kategorie { get; set; }
        public string Hersteller { get; set; }
        public string Beschreibung { get; set; }
        public string Groesse { get; set; }
        public string Jahr { get; set; }
        public decimal GewichtHersteller { get; set; }
        public decimal Gewicht { get; set; }
        public string Kommentar { get; set; }
        public string Link { get; set; }
        public string ImageBase64 { get; set; }
    }
}

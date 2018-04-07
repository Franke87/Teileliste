using TeileListe.MessungHochladen.Dto;

namespace TeileListe.API.Helper
{
    internal class UploadApiEventArgs
    {
        internal string ApiToken { get; set; }
        internal string Datenbank { get; set; }
        internal MessungHochladenDto Messung { get; set; }
        internal ProduktHochladenDto Produkt { get; set; }
    }
}

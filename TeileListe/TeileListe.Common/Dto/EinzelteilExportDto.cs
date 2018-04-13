using System.Collections.Generic;

namespace TeileListe.Common.Dto
{
    public class EinzelteilExportDto : KomponenteDto
    {
        public List<DateiDto> DokumentenListe { get; set; }
    }
}

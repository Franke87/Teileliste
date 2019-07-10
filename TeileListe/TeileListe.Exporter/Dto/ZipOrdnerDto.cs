using System.Collections.Generic;

namespace TeileListe.Exporter.Dto
{
    internal class ZipOrdnerDto
    {
        internal string ParentGuid { get; set; }
        internal string FolderName { get; set; }
        internal List<ZipDateiDto> FileList { get; set; }
    }
}

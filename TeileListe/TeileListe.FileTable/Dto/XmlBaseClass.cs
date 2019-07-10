using System.Collections.Generic;

namespace TeileListe.Table.Dto
{
    public class XmlBaseClass<T>
    {
        public string Version { get; set; }
        public List<T> Daten { get; set; }
    }
}

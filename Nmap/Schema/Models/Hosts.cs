using System.Xml.Serialization;

namespace NMap.Schema.Models
{
    [XmlType("hosts", AnonymousType = true)]
    [XmlRoot("hosts")]
    public class Hosts
    {
        [XmlAttribute("up")]
        public int Up { get; set; }

        [XmlAttribute("down")]
        public int Down { get; set; }

        [XmlAttribute("total")]
        public int Total { get; set; }
    }
}
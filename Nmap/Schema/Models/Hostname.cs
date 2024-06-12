using System.Xml.Serialization;
using NMap.Schema.Enums;

namespace NMap.Schema.Models
{
    [XmlType("hostname", AnonymousType = true)]
    [XmlRoot("hostname")]
    public class Hostname
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public HostNameType Type { get; set; }
    }
}
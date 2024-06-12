using System.Xml.Serialization;

namespace NMap.Schema.Models
{
    [XmlType("debugging", AnonymousType = true)]
    [XmlRoot("debugging")]
    public class Debugging
    {
        [XmlAttribute("level")]
        public int Level { get; set; }
    }
}
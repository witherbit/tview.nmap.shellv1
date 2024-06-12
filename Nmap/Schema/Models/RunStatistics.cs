using System.Xml.Serialization;
using NMap.Schema.Enums;

namespace NMap.Schema.Models
{
    [XmlType("runstats", AnonymousType = true)]
    [XmlRoot("runstats")]
    public class RunStatistics
    {
        [XmlElement("finished")]
        public Finished Finished { get; set; }

        [XmlElement("hosts")]
        public Hosts Hosts { get; set; }
    }
}
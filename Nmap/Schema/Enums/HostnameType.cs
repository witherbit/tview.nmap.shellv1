using System.Xml.Serialization;

namespace NMap.Schema.Enums
{
    [XmlType("HostnameType")]
    public enum HostNameType
    {

        [XmlEnum("user")]
        User,

        [XmlEnum("PTR")]
        PTR,
    }
}
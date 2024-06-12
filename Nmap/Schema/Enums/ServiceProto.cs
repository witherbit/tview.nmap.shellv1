using System.Xml.Serialization;

namespace NMap.Schema.Enums
{
    [XmlType("ServiceProto")]
    public enum ServiceProto
    {
        [XmlEnum("rpc")]
        Rpc,
    }
}

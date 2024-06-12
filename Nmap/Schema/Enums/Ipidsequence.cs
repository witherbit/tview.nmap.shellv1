﻿using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace NMap.Schema.Enums
{
    [GeneratedCode("XmlSchemaClassGenerator", "2.0.0.0")]
    [XmlType("ipidsequence", AnonymousType = true)]
    [XmlRoot("ipidsequence")]
    public class Ipidsequence
    {

        /// <summary>
        /// <para xml:lang="de">Ruft einen Wert ab, der diese Entität eindeutig identifiziert, oder legt diesen fest.</para>
        /// <para xml:lang="en">Gets or sets a value uniquely identifying this entity.</para>
        /// </summary>
        
        [Key()]
        public long Id { get; set; }

        [XmlAttribute("class", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Class { get; set; }

        [XmlAttribute("values", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Values { get; set; }
    }
}
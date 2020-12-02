using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace SysInfo_Fiskalizator.Model
{
    /// <summary>
    /// Nacini placanja
    /// </summary>
    [GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [SerializableAttribute]
    [XmlTypeAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    public enum NacinPlacanjaType
    {
        /// <remarks>G - gotovina</remarks>
        G,
        /// <remarks>K - kartice</remarks>
        K,
        /// <remarks>C - cek</remarks>
        C,
        /// <remarks>T - transakcijski racun</remarks>
        T,
        /// <remarks>O - ostalo</remarks>
        O,
    }
}

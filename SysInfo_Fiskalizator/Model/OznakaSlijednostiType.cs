using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace SysInfo_Fiskalizator.Model
{
    /// <summary>
    /// Oznaka koja govori kako je dodijeljen broj racuna
    /// </summary>
    [GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [SerializableAttribute]
    [XmlTypeAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    public enum OznakaSlijednostiType
    {
        ///<remarks>N - na nivou naplatnog uredjaja</remarks>
        N,
        ///<remarks>P - na nivou poslovnog prostora</remarks>
        P,
    }
}

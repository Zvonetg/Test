using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SysInfo_Fiskalizator.Model
{
    [GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [SerializableAttribute]
    [DebuggerStepThroughAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    [XmlRootAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = true)]
    public class GreskaType : EntityBaseType<GreskaType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string sifraGreskeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string porukaGreskeField;

        [XmlElementAttribute(Order = 0)]
        public string SifraGreske
        {
            get
            {
                return sifraGreskeField;
            }
            set
            {
                sifraGreskeField = value;
            }
        }

        [XmlElementAttribute(Order = 1)]
        public string PorukaGreske
        {
            get
            {
                return porukaGreskeField;
            }
            set
            {
                porukaGreskeField = value;
            }
        }
    }
}
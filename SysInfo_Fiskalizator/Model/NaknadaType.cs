using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SysInfo_Fiskalizator.Model
{
    [GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [SerializableAttribute]
    [DebuggerStepThroughAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    [XmlRootAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = true)]
    public class NaknadaType : EntityBaseType<NaknadaType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string nazivNaknadeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string iznosNaknadeField;

        [XmlElementAttribute(Order = 0)]
        public string NazivN
        {
            get
            {
                return nazivNaknadeField;
            }
            set
            {
                nazivNaknadeField = value;
            }
        }

        [XmlElementAttribute(Order = 1)]
        public string IznosN
        {
            get
            {
                return iznosNaknadeField;
            }
            set
            {
                iznosNaknadeField = value;
            }
        }
    }
}
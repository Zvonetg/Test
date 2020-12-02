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
    public class PorezType : EntityBaseType<PorezType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private decimal stopaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private decimal osnovicaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string iznosField;

        [XmlElementAttribute(Order = 0)]
        public decimal Stopa
        {
            get
            {
                return stopaField;
            }
            set
            {
                stopaField = value;
            }
        }

        [XmlElementAttribute(Order = 1)]
        public decimal Osnovica
        {
            get
            {
                return osnovicaField;
            }
            set
            {
                osnovicaField = value;
            }
        }

        [XmlElementAttribute(Order = 2)]
        public string Iznos
        {
            get
            {
                return iznosField;
            }
            set
            {
                iznosField = value;
            }
        }
    }
}
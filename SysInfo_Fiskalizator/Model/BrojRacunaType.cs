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
    public class BrojRacunaType : EntityBaseType<BrojRacunaType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string brojOznakeRacunaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string oznakaPoslovnogProstoraField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string oznakaNaplatnogUredajaField;

        [XmlElementAttribute(Order = 0)]
        public string BrOznRac
        {
            get
            {
                return brojOznakeRacunaField;
            }
            set
            {
                brojOznakeRacunaField = value;
            }
        }

        [XmlElementAttribute(Order = 1)]
        public string OznPosPr
        {
            get
            {
                return oznakaPoslovnogProstoraField;
            }
            set
            {
                oznakaPoslovnogProstoraField = value;
            }
        }

        [XmlElementAttribute(Order = 2)]
        public string OznNapUr
        {
            get
            {
                return oznakaNaplatnogUredajaField;
            }
            set
            {
                oznakaNaplatnogUredajaField = value;
            }
        }
    }
}
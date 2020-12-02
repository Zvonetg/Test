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
    public class ZaglavljeType : EntityBaseType<ZaglavljeType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string idPorukeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string datumVrijemeField;

        [XmlElementAttribute(Order = 0)]
        public string IdPoruke
        {
            get
            {
                return idPorukeField;
            }
            set
            {
                idPorukeField = value;
            }
        }

        [XmlElementAttribute(Order = 1)]
        public string DatumVrijeme
        {
            get
            {
                return datumVrijemeField;
            }
            set
            {
                datumVrijemeField = value;
            }
        }
    }
}
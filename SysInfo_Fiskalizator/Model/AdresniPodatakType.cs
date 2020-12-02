using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;
using System.ComponentModel;

namespace SysInfo_Fiskalizator.Model
{
    [GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [SerializableAttribute]
    [DebuggerStepThroughAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    [XmlRootAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = true)]
    public class AdresniPodatakType : EntityBaseType<AdresniPodatakType>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        private object itemField;

        [XmlElementAttribute("Adresa", typeof(AdresaType), Order = 0)]
        [XmlElementAttribute("OstaliTipoviPP", typeof(string), Order = 0)]
        public object Item
        {
            get
            {
                return itemField;
            }
            set
            {
                itemField = value;
            }
        }
    }
}
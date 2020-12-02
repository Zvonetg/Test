using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SysInfo_Fiskalizator.Model
{
    [GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [SerializableAttribute]
    [DebuggerStepThroughAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    [XmlRootAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = true)]
    public class PdvType : EntityBaseType<PdvType>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<PorezType> poreziField;

        public PdvType()
        {
            poreziField = new List<PorezType>();
        }

        [XmlElementAttribute("Porez", Order = 0)]
        public List<PorezType> Porez
        {
            get
            {
                return poreziField;
            }
            set
            {
                poreziField = value;
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.CodeDom.Compiler;
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
    public class PorezNaPotrosnjuType : EntityBaseType<PorezNaPotrosnjuType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<PorezType> porezField;

        public PorezNaPotrosnjuType()
        {
            porezField = new List<PorezType>();
        }

        [XmlElementAttribute("Porez", Order = 0)]
        public List<PorezType> Porez
        {
            get
            {
                return porezField;
            }
            set
            {
                porezField = value;
            }
        }
    }
}
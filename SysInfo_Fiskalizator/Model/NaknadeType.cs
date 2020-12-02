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
    public class NaknadeType : EntityBaseType<NaknadeType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<NaknadaType> naknadeField;

        public NaknadeType()
        {
            naknadeField = new List<NaknadaType>();
        }

        [XmlElementAttribute("Naknada", Order = 0)]
        public List<NaknadaType> Naknada
        {
            get
            {
                return naknadeField;
            }
            set
            {
                naknadeField = value;
            }
        }
    }
}
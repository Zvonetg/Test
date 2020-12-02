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
    public class GreskeType : EntityBaseType<GreskeType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<GreskaType> greskeField;

        public GreskeType()
        {
            greskeField = new List<GreskaType>();
        }

        [XmlElementAttribute("Greska", Order = 0)]
        public List<GreskaType> Greska
        {
            get
            {
                return greskeField;
            }
            set
            {
                greskeField = value;
            }
        }
    }
}
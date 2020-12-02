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
    public class AdresaType : EntityBaseType<AdresaType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string ulicaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string kucniBrojField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string kucniBrojDodatakField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string brojPosteField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string naseljeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string opcinaField;

        [XmlElementAttribute(Order = 0)]
        public string Ulica
        {
            get
            {
                return ulicaField;
            }
            set
            {
                ulicaField = value;
            }
        }

        [XmlElementAttribute(Order = 1)]
        public string KucniBroj
        {
            get
            {
                return kucniBrojField;
            }
            set
            {
                kucniBrojField = value;
            }
        }

        [XmlElementAttribute(Order = 2)]
        public string KucniBrojDodatak
        {
            get
            {
                return kucniBrojDodatakField;
            }
            set
            {
                kucniBrojDodatakField = value;
            }
        }

        [XmlElementAttribute(Order = 3)]
        public string BrojPoste
        {
            get
            {
                return brojPosteField;
            }
            set
            {
                brojPosteField = value;
            }
        }

        [XmlElementAttribute(Order = 4)]
        public string Naselje
        {
            get
            {
                return naseljeField;
            }
            set
            {
                naseljeField = value;
            }
        }

        [XmlElementAttribute(Order = 5)]
        public string Opcina
        {
            get
            {
                return opcinaField;
            }
            set
            {
                opcinaField = value;
            }
        }
    }
}
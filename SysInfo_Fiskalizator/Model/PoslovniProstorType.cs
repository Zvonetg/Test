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
    public class PoslovniProstorType : EntityBaseType<PoslovniProstorType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string oibField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string oznakaPoslovnogProstoraField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private AdresniPodatakType adresniPodatakField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string radnoVrijemeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string datumPocetkaPrimjeneField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private OznakaZatvaranjaType oznakaZatvaranjaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool oznakaZatvaranjaFieldSpecified;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string specijalnaNamjenaField;

        public PoslovniProstorType()
        {
            adresniPodatakField = new AdresniPodatakType();
        }

        [XmlElementAttribute(Order = 0)]
        public string Oib
        {
            get
            {
                return oibField;
            }
            set
            {
                oibField = value;
            }
        }

        [XmlElementAttribute(Order = 1)]
        public string OznPoslProstora
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
        public AdresniPodatakType AdresniPodatak
        {
            get
            {
                return adresniPodatakField;
            }
            set
            {
                adresniPodatakField = value;
            }
        }

        [XmlElementAttribute(Order = 3)]
        public string RadnoVrijeme
        {
            get
            {
                return radnoVrijemeField;
            }
            set
            {
                radnoVrijemeField = value;
            }
        }

        [XmlElementAttribute(Order = 4)]
        public string DatumPocetkaPrimjene
        {
            get
            {
                return datumPocetkaPrimjeneField;
            }
            set
            {
                datumPocetkaPrimjeneField = value;
            }
        }

        [XmlElementAttribute(Order = 5)]
        public OznakaZatvaranjaType OznakaZatvaranja
        {
            get
            {
                return oznakaZatvaranjaField;
            }
            set
            {
                oznakaZatvaranjaField = value;
            }
        }

        [XmlIgnoreAttribute]
        public bool OznakaZatvaranjaSpecified
        {
            get
            {
                return oznakaZatvaranjaFieldSpecified;
            }
            set
            {
                oznakaZatvaranjaFieldSpecified = value;
            }
        }

        [XmlElementAttribute(Order = 6)]
        public string SpecNamj
        {
            get
            {
                return specijalnaNamjenaField;
            }
            set
            {
                specijalnaNamjenaField = value;
            }
        }
    }
}
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
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
    public class RacunType : EntityBaseType<RacunType>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string oibField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool uSustavuPdvaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string datatumVrijemeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private OznakaSlijednostiType oznakaSlijednostiField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private BrojRacunaType brojRacunaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<PorezType> pdvField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<PorezType> poreziNaPotrosnjuField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<PorezOstaloType> ostaliPoreziField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string iznosOslobodenPdvaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool iznosOslobodenPdvaFieldSpecified;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string iznosMarzaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool iznosMarzaFieldSpecified;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string iznosNePodlijezeOporezivanjuField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool iznosNePodlijezeOporezivanjuFieldSpecified;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<NaknadaType> naknadeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string iznosUkupnoField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private NacinPlacanjaType nacinPlacanjaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string oibOperateraField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string zastitniKodField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool naknadnaDostavaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string paragonBrojRacunaField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string specijalnaNamjenaField;

        /// <summary>
        /// RacunType class constructor
        /// </summary>
        public RacunType()
        {
            naknadeField = new List<NaknadaType>();
            ostaliPoreziField = new List<PorezOstaloType>();
            poreziNaPotrosnjuField = new List<PorezType>();
            pdvField = new List<PorezType>();
            brojRacunaField = new BrojRacunaType();
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
        public bool USustPdv
        {
            get
            {
                return uSustavuPdvaField;
            }
            set
            {
                uSustavuPdvaField = value;
            }
        }

        [XmlElementAttribute(Order = 2)]
        public string DatVrijeme
        {
            get
            {
                return datatumVrijemeField;
            }
            set
            {
                datatumVrijemeField = value;
            }
        }

        [XmlElementAttribute(Order = 3)]
        public OznakaSlijednostiType OznSlijed
        {
            get
            {
                return this.oznakaSlijednostiField;
            }
            set
            {
                this.oznakaSlijednostiField = value;
            }
        }

        [XmlElementAttribute(Order = 4)]
        public BrojRacunaType BrRac
        {
            get
            {
                return brojRacunaField;
            }
            set
            {
                brojRacunaField = value;
            }
        }

        [XmlArrayAttribute(Order = 5)]
        [XmlArrayItemAttribute("Porez", IsNullable = false)]
        public List<PorezType> Pdv
        {
            get
            {
                return pdvField;
            }
            set
            {
                pdvField = value;
            }
        }

        [XmlArrayAttribute(Order = 6)]
        [XmlArrayItemAttribute("Porez", IsNullable = false)]
        public List<PorezType> Pnp
        {
            get
            {
                return poreziNaPotrosnjuField;
            }
            set
            {
                poreziNaPotrosnjuField = value;
            }
        }

        [XmlArrayAttribute(Order = 7)]
        [XmlArrayItemAttribute("Porez", IsNullable = false)]
        public List<PorezOstaloType> OstaliPor
        {
            get
            {
                return this.ostaliPoreziField;
            }
            set
            {
                this.ostaliPoreziField = value;
            }
        }

        [XmlElementAttribute(Order = 8)]
        public string IznosOslobPdv
        {
            get
            {
                return iznosOslobodenPdvaField;
            }
            set
            {
                iznosOslobodenPdvaField = value;
            }
        }

        [XmlIgnoreAttribute]
        public bool IznosOslobPdvSpecified
        {
            get
            {
                return iznosOslobodenPdvaFieldSpecified;
            }
            set
            {
                iznosOslobodenPdvaFieldSpecified = value;
            }
        }

        [XmlElementAttribute(Order = 9)]
        public string IznosMarza
        {
            get
            {
                return iznosMarzaField;
            }
            set
            {
                iznosMarzaField = value;
            }
        }

        [XmlIgnoreAttribute]
        public bool IznosMarzaSpecified
        {
            get
            {
                return iznosMarzaFieldSpecified;
            }
            set
            {
                iznosMarzaFieldSpecified = value;
            }
        }

        [XmlElementAttribute(Order = 10)]
        public string IznosNePodlOpor
        {
            get
            {
                return iznosNePodlijezeOporezivanjuField;
            }
            set
            {
                iznosNePodlijezeOporezivanjuField = value;
            }
        }

        [XmlIgnoreAttribute]
        public bool IznosNePodlOporSpecified
        {
            get
            {
                return iznosNePodlijezeOporezivanjuFieldSpecified;
            }
            set
            {
                iznosNePodlijezeOporezivanjuFieldSpecified = value;
            }
        }

        [XmlArrayAttribute(Order = 11)]
        [XmlArrayItemAttribute("Naknada", IsNullable = false)]
        public List<NaknadaType> Naknade
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

        [XmlElementAttribute(Order = 12)]
        public string IznosUkupno
        {
            get
            {
                return iznosUkupnoField;
            }
            set
            {
                iznosUkupnoField = value;
            }
        }

        [XmlElementAttribute(Order = 13)]
        public NacinPlacanjaType NacinPlac
        {
            get
            {
                return nacinPlacanjaField;
            }
            set
            {
                nacinPlacanjaField = value;
            }
        }

        [XmlElementAttribute(Order = 14)]
        public string OibOper
        {
            get
            {
                return oibOperateraField;
            }
            set
            {
                oibOperateraField = value;
            }
        }

        [XmlElementAttribute(Order = 15)]
        public string ZastKod
        {
            get
            {
                return zastitniKodField;
            }
            set
            {
                zastitniKodField = value;
            }
        }

        [XmlElementAttribute(Order = 16)]
        public bool NakDost
        {
            get
            {
                return naknadnaDostavaField;
            }
            set
            {
                naknadnaDostavaField = value;
            }
        }

        [XmlElementAttribute(Order = 17)]
        public string ParagonBrRac
        {
            get
            {
                return paragonBrojRacunaField;
            }
            set
            {
                paragonBrojRacunaField = value;
            }
        }

        /// <summary>
        /// Potrebno je dostaviti jedan od podataka u nastavku:
        ///	- OIB pravne ili fizicke osobe koja je proizvela programsko rjesenje ili
        ///	- OIB pravne ili fizicke osobe koja odrzava programsko rjesenje ili
        ///	- OIB pravne ili fizicke osobe prodavatelja u slucaju da se koristi rjesenje od stranog proizvodaca – bez lokalnog partnera
        /// </summary>
        [XmlElementAttribute(Order = 18)]
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
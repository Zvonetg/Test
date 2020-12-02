using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SysInfo_Fiskalizator.Model
{
    [GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [SerializableAttribute]
    [DebuggerStepThroughAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    [XmlRootAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = false)]
    public class RacunOdgovor : EntityBaseType<RacunOdgovor>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        private ZaglavljeOdgovorType zaglavljeOdgovoraField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string jirField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<GreskaType> greskeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string idField;

        public RacunOdgovor()
        {
            greskeField = new List<GreskaType>();
            zaglavljeOdgovoraField = new ZaglavljeOdgovorType();
        }

        [XmlElementAttribute(Order = 0)]
        public ZaglavljeOdgovorType Zaglavlje
        {
            get
            {
                return zaglavljeOdgovoraField;
            }
            set
            {
                zaglavljeOdgovoraField = value;
            }
        }

        /// <summary>
        /// Jedinstveni identifikator racuna.
        /// </summary>
        [XmlElementAttribute(Order = 1)]
        public string Jir
        {
            get
            {
                return jirField;
            }
            set
            {
                jirField = value;
            }
        }

        [XmlArrayAttribute(Order = 2)]
        [XmlArrayItemAttribute("Greska", IsNullable = false)]
        public List<GreskaType> Greske
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

        /// <summary>
        /// Atribut za potrebe digitalnog potpisa, u njega se stavlja referentni na koji se referencira digitalni potpis.
        /// </summary>
        [XmlAttributeAttribute]
        public string Id
        {
            get
            {
                return idField;
            }
            set
            {
                idField = value;
            }
        }
    }
}
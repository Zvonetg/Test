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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    [XmlRootAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = false)]
    public class PoslovniProstorOdgovor : EntityBaseType<PoslovniProstorOdgovor>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private ZaglavljeOdgovorType zaglavljeOdgovoraField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<GreskaType> greskeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string idField;

        public PoslovniProstorOdgovor()
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

        [XmlArrayAttribute(Order = 1)]
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
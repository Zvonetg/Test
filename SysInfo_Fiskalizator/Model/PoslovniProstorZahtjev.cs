using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;
using SysInfo_Fiskalizator.Model.Auxilliary;

namespace SysInfo_Fiskalizator.Model
{
    [GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [SerializableAttribute]
    [DebuggerStepThroughAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    [XmlRootAttribute(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = false)]
    public class PoslovniProstorZahtjev : EntityBaseType<PoslovniProstorZahtjev>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private ZaglavljeType zaglavljeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private PoslovniProstorType poslovniProstorField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string idField;

        public PoslovniProstorZahtjev()
        {
            poslovniProstorField = new PoslovniProstorType();
            zaglavljeField = new ZaglavljeType();
        }

        /// <summary>Generira novi inicijalizirani PoslovniProstorZahtjev</summary>
        /// <param name="workspace">Poslovni prostor</param>
        /// <returns>Inicijalizirani PoslovniProstorZahtjev</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        public static PoslovniProstorZahtjev GetInitialized(PoslovniProstorType workspace)
        {
            #region Input parameter testing

            if (workspace == null)
            {
                throw new ArgumentNullException("workspace", "Method: PoslovniProstorZahtjev.GetInitialized Parameter: workspace");
            }

            #endregion

            return new PoslovniProstorZahtjev
            {
                Id = "signXmlId",
                PoslovniProstor = workspace,
                Zaglavlje = new ZaglavljeType
                {
                    DatumVrijeme = Helper.FormatDateTimeToCisDateTimeFormat(DateTime.Now, false),
                    IdPoruke = Helper.GenerateGuidString()
                }
            };
        }

        [XmlElementAttribute(Order = 0)]
        public ZaglavljeType Zaglavlje
        {
            get
            {
                return zaglavljeField;
            }
            set
            {
                zaglavljeField = value;
            }
        }

        [XmlElementAttribute(Order = 1)]
        public PoslovniProstorType PoslovniProstor
        {
            get
            {
                return poslovniProstorField;
            }
            set
            {
                poslovniProstorField = value;
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

        /// <summary>Serializira objekt</summary>
        /// <returns>XML dokument</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="XmlException">Greška tijekom parsiranja dokumenta</exception>
        internal XmlDocument SerializeToXmlDocument()
        {
            return Helper.ParseStringToXmlDocument(Serialize());
        }
    }
}
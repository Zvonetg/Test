using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
using SysInfo_Fiskalizator.Model.Auxilliary;
using System.Collections.Generic;

namespace SysInfo_Fiskalizator.Model
{
    [GeneratedCode("System.Xml", "4.0.30319.233")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
    [XmlRoot(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = false)]
    public class ProvjeraOdgovor : EntityBaseType<ProvjeraOdgovor>
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private ZaglavljeType zaglavljeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private RacunType racunField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private List<GreskaType> greskeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string idField;

        /// <summary>
        /// ProvjeraOdgovor class constructor
        /// </summary>
        public ProvjeraOdgovor()
        {
            zaglavljeField = new ZaglavljeType();
            racunField = new RacunType();
            greskeField = new List<GreskaType>();
        }

        /// <summary>Generira novi inicijalizirani ProvjeraOdgovor</summary>
        /// <param name="invoice">Račun</param>
        /// <returns>Inicijalizirani ProvjeraOdgovor</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        public static ProvjeraOdgovor GetInitialized(RacunType invoice)
        {
            #region Input parameter testing

            if (invoice == null)
            {
                throw new ArgumentNullException("invoice", "Method: ProvjeraOdgovor.GetInitialized Parameter: invoice");
            }

            #endregion

            return new ProvjeraOdgovor
            {
                Id = "signXmlId",
                Racun = invoice,
                Zaglavlje = new ZaglavljeType()
                {
                    DatumVrijeme = (!invoice.NakDost ? invoice.DatVrijeme : Helper.FormatDateTimeToCisDateTimeFormat(DateTime.Now, false)),
                    IdPoruke = Helper.GenerateGuidString()
                }
            };
        }

        [XmlElement(Order = 0)]
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

        [XmlElement(Order = 1)]
        public RacunType Racun
        {
            get
            {
                return racunField;
            }
            set
            {
                racunField = value;
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
        [XmlAttribute]
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

        /// <summary>Serializira objekt uz izbacivanje praznih elemenata</summary>
        /// <returns>XML dokument</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="XmlException">Greška tijekom parsiranja dokumenta</exception>
        internal XmlDocument SerializeToXmlDocument()
        {
            return Helper.ParseStringToXmlDocument(Serialize().Replace("<tns:Pdv />", "")
                                                              .Replace("<tns:Pnp />", "")
                                                              .Replace("<tns:OstaliPor />", "")
                                                              .Replace("<tns:Naknade />", ""));
        }
    }
}
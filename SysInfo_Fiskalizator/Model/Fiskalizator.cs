using System;
using System.Xml;
using SysInfo_Fiskalizator.Model.Auxilliary;
using System.Net;

namespace SysInfo_Fiskalizator.Model
{
    sealed public class Fiskalizator
    {
        public delegate void EventHandler(string message);
        public event EventHandler NewLogEntryNeeded;

        #region ---PROPERTIES & FIELDS---

        private readonly SoapMessenger _soapMessenger;

        /// <summary>Vrsta izuzetka vraćenog od web servisa kod zadnje komunikacija. Ukoliko nije bilo izuzetka, vrijednost je null.</summary>
        public WebExceptionStatus? CisExceptionStatus
        {
            get { return _soapMessenger.CisExceptionStatus; }
        }

        /// <summary>XML dokument sa odgovorom web servisa koji sadrži grešku kod zadnje komunikacije. Ukoliko nije bilo greške, vrijednost je null.</summary>
        public XmlDocument CisErrorReply
        {
            get { return _soapMessenger.CisErrorReply; }
        }

        /// <summary>Datumi i vremena slanja zahtjeva i zaprimanja odgovora zadnje uspješne poruke.</summary>
        public DateTime[] LastMessageTimeStamps
        {
            get { return _soapMessenger.lastMessageTimeStamps; }
        }

        #endregion

        #region ---CONSTRUCTORS---

        /// <summary>Konstruktor</summary>
        /// <param name="cisUrl">URL CIS web servisa s kojim se komunicira</param>
        /// <param name="xmlSavePath">Lokacija za pohranu XML poruka na disk. Poruke se ne pohranjuju ukoliko je vrijednost null ili prazna </param>
        /// <param name="timeOut">Vrijednost TimeOut-a WebRequest-a (u milisekundama) kod komunikacije sa CIS web servisom. Standardna vrijednost je 100.000 ms (100 s)</param>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentOutOfRangeException">Vrijednost parametra nije ispravna</exception>
        public Fiskalizator(string cisUrl, string xmlSavePath, int timeOut = 100000)
        {
            _soapMessenger = new SoapMessenger(cisUrl, xmlSavePath, timeOut);
            _soapMessenger.NewLogEntryNeeded += _soapMessenger_NewLogEntryNeeded;
        }

        #endregion

        #region ---METHODS---

        private void _soapMessenger_NewLogEntryNeeded(string message)
        {
            if (NewLogEntryNeeded != null)
            {
                NewLogEntryNeeded(message);
            }
        }

        #region ---CIS COMMUNICATION---

        /// <summary>Šalje Echo poruku u CIS</summary>
        /// <param name="echoContent">Tekst poruke koja se šalje</param>
        /// <returns>XML poruka vraćena od CIS-a</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        public XmlDocument SendEcho(string echoContent)
        {
            return _soapMessenger.SendEcho(echoContent);
        }

        /// <summary>Šalje PoslovniProstor poruku u CIS</summary>
        /// <param name="workspace">Objekt tipa PoslovniProstorType koji sadrži informacije o poslovnom prostoru</param>
        /// <param name="certificateName">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <returns>XML poruka vraćena od CIS-a</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije pronađen</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        public XmlDocument SendWorkspace(PoslovniProstorType workspace, string certificateName)
        {
            return _soapMessenger.SendWorkspace(workspace, certificateName);
        }
        /// <summary>Šalje PoslovniProstor poruku u CIS</summary>
        /// <param name="workspace">Objekt tipa PoslovniProstorType koji sadrži informacije o poslovnom prostoru</param>
        /// <param name="certificateFilenameAndPath">Naziv i puna adresa .pfx datoteke.</param>
        /// <param name="certificatePassword">Lozinka za pristup .pfx datoteci.</param>
        /// <returns>XML poruka vraćena od CIS-a</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="FileNotFoundException">Datoteka ne postoji</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije moguće dohvatiti</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        public XmlDocument SendWorkspace(PoslovniProstorType workspace, string certificateFilenameAndPath, string certificatePassword)
        {
            return _soapMessenger.SendWorkspace(workspace, certificateFilenameAndPath, certificatePassword);
        }

        /// <summary>Šalje Račun poruku u CIS. Ukoliko zaštitni kod izdavatelja (ZKI) nije upisan, generira se prije slanja</summary>
        /// <param name="invoice">Objekt tipa RacunType koji sadrži informacije o računu</param>
        /// <param name="certificateName">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <returns>XML poruka vraćena od CIS-a</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije pronađen</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        public XmlDocument SendInvoice(RacunType invoice, string certificateName)
        {
            if (string.IsNullOrEmpty(invoice.ZastKod))
            {
                invoice.ZastKod = Helper.GenerateSafetyCode(certificateName, invoice.Oib, invoice.DatVrijeme, invoice.BrRac.BrOznRac, invoice.BrRac.OznPosPr, invoice.BrRac.OznNapUr, invoice.IznosUkupno);
            }

            return _soapMessenger.SendInvoice(invoice, certificateName);
        }
        /// <summary>Šalje Račun poruku u CIS. Ukoliko zaštitni kod izdavatelja (ZKI) nije upisan, generira se prije slanja</summary>
        /// <param name="invoice">Objekt tipa RacunType koji sadrži informacije o računu</param>
        /// <param name="certificateFilenameAndPath">Naziv i puna adresa .pfx datoteke.</param>
        /// <param name="certificatePassword">Lozinka za pristup .pfx datoteci.</param>
        /// <returns>XML poruka vraćena od CIS-a</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="FileNotFoundException">Datoteka ne postoji</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije moguće dohvatiti</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        public XmlDocument SendInvoice(RacunType invoice, string certificateFilenameAndPath, string certificatePassword)
        {
            if (string.IsNullOrEmpty(invoice.ZastKod))
            {
                invoice.ZastKod = Helper.GenerateSafetyCode(certificateFilenameAndPath, certificatePassword, invoice.Oib, invoice.DatVrijeme, invoice.BrRac.BrOznRac, invoice.BrRac.OznPosPr, invoice.BrRac.OznNapUr, invoice.IznosUkupno);
            }

            return _soapMessenger.SendInvoice(invoice, certificateFilenameAndPath, certificatePassword);
        }

        /// <summary>Šalje Provjera poruku u test CIS. Ukoliko zaštitni kod izdavatelja (ZKI) nije upisan, generira se prije slanja</summary>
        /// <param name="invoice">Objekt tipa RacunType koji sadrži informacije o računu koji se testira</param>
        /// <param name="certificateName">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <returns>XML poruka vraćena od testnog CIS-a</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije pronađen</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        public XmlDocument SendCheck(RacunType invoice, string certificateName)
        {
            if (string.IsNullOrEmpty(invoice.ZastKod))
            {
                invoice.ZastKod = Helper.GenerateSafetyCode(certificateName, invoice.Oib, invoice.DatVrijeme, invoice.BrRac.BrOznRac, invoice.BrRac.OznPosPr, invoice.BrRac.OznNapUr, invoice.IznosUkupno);
            }

            return _soapMessenger.SendCheck(invoice, certificateName);
        }
        /// <summary>Šalje Provjera poruku u testni CIS. Ukoliko zaštitni kod izdavatelja (ZKI) nije upisan, generira se prije slanja</summary>
        /// <param name="invoice">Objekt tipa RacunType koji sadrži informacije o računu koji se testira</param>
        /// <param name="certificateFilenameAndPath">Naziv i puna adresa .pfx datoteke.</param>
        /// <param name="certificatePassword">Lozinka za pristup .pfx datoteci.</param>
        /// <returns>XML poruka vraćena od testnog CIS-a</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="FileNotFoundException">Datoteka ne postoji</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije moguće dohvatiti</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        public XmlDocument SendCheck(RacunType invoice, string certificateFilenameAndPath, string certificatePassword)
        {
            if (string.IsNullOrEmpty(invoice.ZastKod))
            {
                invoice.ZastKod = Helper.GenerateSafetyCode(certificateFilenameAndPath, certificatePassword, invoice.Oib, invoice.DatVrijeme, invoice.BrRac.BrOznRac, invoice.BrRac.OznPosPr, invoice.BrRac.OznNapUr, invoice.IznosUkupno);
            }

            return _soapMessenger.SendCheck(invoice, certificateFilenameAndPath, certificatePassword);
        }

        #endregion

        #region ---XML OPERATIONS---

        /// <summary>Dodaje namespace-e soap i tns XML dokumentu u SOAP omotnici</summary>
        /// <param name="xmlDocument">XML dokument</param>
        /// <returns>XmlNamespaceManager namespace-a dodanih dokumentu</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        private static XmlNamespaceManager AppendNamespaceToXmlDocumentWithSoapEnvelope(XmlDocument xmlDocument)
        {
            #region Input parameter testing

            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument", "Method: Fiskalizator.AppendNamespaceToXmlDocumentWithSOAPEnvelope Parameter: xmlDocument");
            }

            #endregion

            var xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            xmlNamespaceManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            xmlNamespaceManager.AddNamespace("tns", "http://www.apis-it.hr/fin/2012/types/f73");
            return xmlNamespaceManager;
        }

        #endregion

        #region ---STATIC---

        /// <summary>Generira zaštitni kod izdavatelja (ZKI) na temelju ulaznih parametara računa i naziva certifikata. </summary>
        /// <param name="certificateName">Naziv certifikata koji se koristi (npr. "FISKAL 1")</param>
        /// <param name="companyTaxId">OIB izdavatelja</param>
        /// <param name="invoiceIssueDateTime">Datum i vrijeme izdavanja</param>
        /// <param name="invoiceIdNumber">Redni broj računa</param>
        /// <param name="workspaceId">Oznaka poslovnog prostora</param>
        /// <param name="cashRegisterId">Oznaka naplatnog uređaja</param>
        /// <param name="invoiceTotal">Ukupni iznos računa</param>
        /// <returns>Zaštitni kod izdavatelja u propisanom formatu</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije pronađen</exception>
        public static string GenerateSafetyCode(string certificateName, string companyTaxId, string invoiceIssueDateTime, string invoiceIdNumber, string workspaceId, string cashRegisterId, string invoiceTotal)
        {
            return Helper.GenerateSafetyCode(certificateName, companyTaxId, invoiceIssueDateTime, invoiceIdNumber, workspaceId, cashRegisterId, invoiceTotal);
        }
        /// <summary>Generira zaštitni kod izdavatelja (ZKI) na temelju ulaznih parametara računa i datoteke s certifikatom (i pripadne lozinke pristupa).</summary>
        /// <param name="certificateFilenameAndPath">Naziv i puna adresa .pfx datoteke.</param>
        /// <param name="certificatePassword">Lozinka za pristup .pfx datoteci.</param>
        /// <param name="companyTaxId">OIB izdavatelja</param>
        /// <param name="invoiceIssueDateTime">Datum i vrijeme izdavanja</param>
        /// <param name="invoiceIdNumber">Redni broj računa</param>
        /// <param name="workspaceId">Oznaka poslovnog prostora</param>
        /// <param name="cashRegisterId">Oznaka naplatnog uređaja</param>
        /// <param name="invoiceTotal">Ukupni iznos računa</param>
        /// <returns>Zaštitni kod izdavatelja u propisanom formatu</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="FileNotFoundException">Datoteka ne postoji</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije pronađen</exception>
        public static string GenerateSafetyCode(string certificateFilenameAndPath, string certificatePassword, string companyTaxId, string invoiceIssueDateTime, string invoiceIdNumber, string workspaceId, string cashRegisterId, string invoiceTotal)
        {
            return Helper.GenerateSafetyCode(certificateFilenameAndPath, certificatePassword, companyTaxId, invoiceIssueDateTime, invoiceIdNumber, workspaceId, cashRegisterId, invoiceTotal);
        }

        /// <summary>Dohvaća jedinstveni identifikator računa (JIR) iz XML dokumenta tipa RacunOdgovor</summary>
        /// <param name="xmlInvoiceReply">XML dokument koji sadrži RacunOdgovor</param>
        /// <returns>Formatirani jedinstveni identifikator računa (JIR)</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="XmlException">Jedinstveni identifikator računa (JIR) nije pronađen u dokumentu</exception>
        public static string GetInvoiceUniqueIdentifier(XmlDocument xmlInvoiceReply)
        {
            #region Input parameter testing

            if (xmlInvoiceReply == null)
            {
                throw new ArgumentNullException("xmlInvoiceReply", "Method: Fiskalizator.GetInvoiceUniqueIdentifier Parameter: xmlInvoiceReply");
            }
            if (string.IsNullOrEmpty(xmlInvoiceReply.InnerXml) || xmlInvoiceReply.DocumentElement == null)
            {
                throw new ArgumentException("Method: Fiskalizator.GetInvoiceUniqueIdentifier Parameter: xmlInvoiceReply Error: InnerXml is null or empty, or DocumentElement is null");
            }

            #endregion

            var namespaceManager = AppendNamespaceToXmlDocumentWithSoapEnvelope(xmlInvoiceReply);

            var node = xmlInvoiceReply.DocumentElement.SelectSingleNode("soap:Body/tns:RacunOdgovor/tns:Jir", namespaceManager);

            if (node != null)
            {
                return node.InnerText;
            }

            throw new XmlException("Method: Fiskalizator.GetInvoiceUniqueIdentifier Error: Invoice unique identifier was not found");
        }

        /// <summary>Dohvaća šifru i opis greške iz XML dokumenta tipa [T]Odgovor</summary>
        /// <param name="xmlReply">XML dokument koji sadrži [T]Odgovor</param>
        /// <param name="messageType">Tip poruke ([T]) sadržane u XML dokumentu</param>
        /// <returns>[0]Šifru greške po CIS tehničkoj specifikaciji [1]Opis greške po CIS tehničkoj specifikaciji</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="XmlException">XML dokument ne sadrži greške</exception>
        public static string[] GetErrorCodeAndMessage(XmlDocument xmlReply, MessageType messageType)
        {
            #region Input parameter testing

            if (xmlReply == null)
            {
                throw new ArgumentNullException("xmlReply", "Method: Fiskalizator.GetErrorCodeAndMessage Parameter: xmlReply");
            }
            if (string.IsNullOrEmpty(xmlReply.InnerXml) || xmlReply.DocumentElement == null)
            {
                throw new ArgumentException("Method: Fiskalizator.GetErrorCodeAndMessage Parameter: xmlReply Error: InnerXml is null or empty, or DocumentElement is null");
            }
            if (messageType == MessageType.Other)
            {
                throw new ArgumentNullException("messageType", "Method: Fiskalizator.GetErrorCodeAndMessage Parameter: messageType");
            }

            #endregion

            var namespaceManager = AppendNamespaceToXmlDocumentWithSoapEnvelope(xmlReply);
            var root = xmlReply.DocumentElement;

            var errorCodeNode = root.SelectSingleNode(String.Format("soap:Body/tns:{0}/tns:Greske/tns:Greska/tns:SifraGreske", Helper.GetEnumDescription(messageType)), namespaceManager);
            var errorCodeMessage = root.SelectSingleNode(String.Format("soap:Body/tns:{0}/tns:Greske/tns:Greska/tns:PorukaGreske", Helper.GetEnumDescription(messageType)), namespaceManager);

            if (errorCodeNode != null)
            {
                return new[] { errorCodeNode.InnerText, (errorCodeMessage != null ? errorCodeMessage.InnerText : "") };
            }

            throw new XmlException("Method: Fiskalizator.GetErrorCodeAndMessage Error: No error was not found in XmlDocument");
        }

        /// <summary>Dohvaća datum i vrijeme obrade na CIS servisu iz XML dokumenta tipa [T]Odgovor</summary>
        /// <param name="xmlReply">XML dokument koji sadrži [T]Odgovor</param>
        /// <param name="messageType">Tip poruke ([T]) sadržane u XML dokumentu</param>
        /// <returns>Datum i vrijeme obrade u CIS formatu</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="XmlException">XML dokument ne sadrži datum i vrijeme obrade</exception>
        public static string GetMessageTimestamp(XmlDocument xmlReply, MessageType messageType)
        {
            #region Input parameter testing

            if (xmlReply == null)
            {
                throw new ArgumentNullException("xmlReply", "Method: Fiskalizator.GetErrorCodeAndMessage Parameter: xmlReply");
            }
            if (string.IsNullOrEmpty(xmlReply.InnerXml) || xmlReply.DocumentElement == null)
            {
                throw new ArgumentException("Method: Fiskalizator.GetErrorCodeAndMessage Parameter: xmlReply Error: InnerXml is null or empty, or DocumentElement is null");
            }
            if (messageType == MessageType.Other)
            {
                throw new ArgumentNullException("messageType", "Method: Fiskalizator.GetErrorCodeAndMessage Parameter: messageType");
            }

            #endregion

            var namespaceManager = AppendNamespaceToXmlDocumentWithSoapEnvelope(xmlReply);
            var root = xmlReply.DocumentElement;

            var timestampNode = root.SelectSingleNode(String.Format("soap:Body/tns:{0}/tns:Zaglavlje/tns:DatumVrijeme", Helper.GetEnumDescription(messageType)), namespaceManager);

            if (timestampNode != null)
            {
                return timestampNode.InnerText;
            }

            throw new XmlException("Method: Fiskalizator.GetMessageTimestamp Error: No timestamp was not found in XmlDocument");
        }

        #endregion

        #endregion
    }
}
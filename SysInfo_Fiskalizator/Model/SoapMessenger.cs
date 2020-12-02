using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Xml;
using SysInfo_Fiskalizator.Model.Auxilliary;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SysInfo_Fiskalizator.Model
{
    sealed internal class SoapMessenger
    {
        internal delegate void EventHandler(string message);
        internal event EventHandler NewLogEntryNeeded;

        #region ---PROPERTIES & FIELDS---

        /// <summary>URL CIS web servisa</summary>
        private readonly string _cisUrl;

        /// <summary>Lokacija za pohranu XML poruka</summary>
        private readonly string _xmlSavePath;

        /// <summary>GUID zadnje poslane/primljene poruke.</summary>
        private string lastMessageId;

        /// <summary>Datumi i vremena slanja zahtjeva i zaprimanja odgovora zadnje uspješne poruke.</summary>
        internal DateTime[] lastMessageTimeStamps;

        /// <summary>Vrijednost TimeOut-a WebRequest-a (u milisekundama) kod komunikacije sa CIS web servisom</summary>
        private readonly int _timeOut;

        /// <summary>Vrsta izuzetka vraćenog od web servisa kod zadnje komunikacija. Ukoliko nije bilo izuzetka, vrijednost je null.</summary>
        internal WebExceptionStatus? CisExceptionStatus { get; set; }

        /// <summary>XML dokument sa odgovorom web servisa koji sadrži grešku kod zadnje komunikacije. Ukoliko nije bilo greške, vrijednost je null.</summary>
        internal XmlDocument CisErrorReply { get; set; }

        #endregion

        #region ---CONSTRUCTORS---

        /// <summary>Konstruktor</summary>
        /// <param name="cisUrl">URL CIS web servisa s kojim se komunicira</param>
        /// <param name="xmlSavePath">Lokacija za pohranu XML poruka na disk. Poruke se ne pohranjuju ukoliko je vrijednost null ili prazna </param>
        /// <param name="timeOut">Vrijednost TimeOut-a WebRequest-a (u milisekundama) kod komunikacije sa CIS web servisom. Ne koristi se ako je vrijednost 0</param>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentOutOfRangeException">Vrijednost parametra nije ispravna</exception>
        internal SoapMessenger(string cisUrl, string xmlSavePath, int timeOut)
        {
            if (string.IsNullOrEmpty(cisUrl))
            {
                throw new ArgumentNullException("cisUrl", "Method: SoapMessenger.CONSTRUCTOR Parameter: cisUrl");
            }
            if (timeOut < 0)
            {
                throw new ArgumentOutOfRangeException("timeOut", timeOut, "Method: SoapMessenger.CONSTRUCTOR Parameter: timeOut");
            }

            _cisUrl = cisUrl;
            _timeOut = timeOut;
            _xmlSavePath = xmlSavePath;
        }

        #endregion

        #region ---ECHO---

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
        internal XmlDocument SendEcho(string echoContent)
        {
            XmlDocument echoRequest =
                Helper.ParseStringToXmlDocument(
                    String.Format(
                        @"<tns:EchoRequest xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.apisit.hr/fin/2012/types/f73/FiskalizacijaSchema.xsd"">{0}</tns:EchoRequest>",
                        echoContent));
            AppendSoapEnvelopeToXmlDocument(ref echoRequest);

            return SendSoapMessage(echoRequest);
        }

        #endregion

        #region ---WORKSPACE---

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
        internal XmlDocument SendWorkspace(PoslovniProstorType workspace, string certificateName)
        {
            PoslovniProstorZahtjev request = PoslovniProstorZahtjev.GetInitialized(workspace);
            lastMessageId = request.Zaglavlje.IdPoruke;
            return SignEnvelopAndSendRequest(certificateName, request.SerializeToXmlDocument());
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
        internal XmlDocument SendWorkspace(PoslovniProstorType workspace, string certificateFilenameAndPath, string certificatePassword)
        {
            PoslovniProstorZahtjev request = PoslovniProstorZahtjev.GetInitialized(workspace);
            lastMessageId = request.Zaglavlje.IdPoruke;
            return SignEnvelopAndSendRequest(certificateFilenameAndPath, certificatePassword, request.SerializeToXmlDocument());
        }

        #endregion

        #region ---INVOICE---

        /// <summary>Šalje Račun poruku u CIS</summary>
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
        internal XmlDocument SendInvoice(RacunType invoice, string certificateName)
        {
            RacunZahtjev request = RacunZahtjev.GetInitialized(invoice);
            lastMessageId = request.Zaglavlje.IdPoruke;
            return SignEnvelopAndSendRequest(certificateName, request.SerializeToXmlDocument());
        }
        /// <summary>Šalje Račun poruku u CIS</summary>
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
        internal XmlDocument SendInvoice(RacunType invoice, string certificateFilenameAndPath, string certificatePassword)
        {
            RacunZahtjev request = RacunZahtjev.GetInitialized(invoice);
            lastMessageId = request.Zaglavlje.IdPoruke;
            return SignEnvelopAndSendRequest(certificateFilenameAndPath, certificatePassword, request.SerializeToXmlDocument());
        }

        #endregion

        #region ---CHECK---

        /// <summary>Šalje Provjera poruku u test CIS</summary>
        /// <param name="invoice">Objekt tipa RacunType koji sadrži informacije o računu koji se testira</param>
        /// <param name="certificateName">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <returns>XML poruka vraćena od test CIS-a</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije pronađen</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        internal XmlDocument SendCheck(RacunType invoice, string certificateName)
        {
            ProvjeraZahtjev request = ProvjeraZahtjev.GetInitialized(invoice);
            lastMessageId = request.Zaglavlje.IdPoruke;
            return SignEnvelopAndSendRequest(certificateName, request.SerializeToXmlDocument());
        }
        /// <summary>Šalje Provjera poruku u test CIS</summary>
        /// <param name="invoice">Objekt tipa RacunType koji sadrži informacije o računu koji se testira</param>
        /// <param name="certificateFilenameAndPath">Naziv i puna adresa .pfx datoteke.</param>
        /// <param name="certificatePassword">Lozinka za pristup .pfx datoteci.</param>
        /// <returns>XML poruka vraćena od test CIS-a</returns>
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
        internal XmlDocument SendCheck(RacunType invoice, string certificateFilenameAndPath, string certificatePassword)
        {
            ProvjeraZahtjev request = ProvjeraZahtjev.GetInitialized(invoice);
            lastMessageId = request.Zaglavlje.IdPoruke;
            return SignEnvelopAndSendRequest(certificateFilenameAndPath, certificatePassword, request.SerializeToXmlDocument());
        }

        #endregion

        #region ---COMMON---

        /// <summary>
        /// Šalje poruku CIS web servisu
        /// </summary>
        /// <param name="soapMessage">Potpisana omotana XML poruka</param>
        /// <returns>Odgovor CIS servisa. Ukoliko odgovor nije primljen ili je primljena greška, vrijednost je null</returns>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        internal XmlDocument SendSoapMessage(XmlDocument soapMessage)
        {
            lastMessageTimeStamps = new DateTime[2];

            if (!string.IsNullOrEmpty(_xmlSavePath))
            {
                if (NewLogEntryNeeded != null)
                {
                    NewLogEntryNeeded(String.Format(@"Writing OUTPUT XML: {0}OUT_{1}.xml", _xmlSavePath, lastMessageId));
                }
                soapMessage.Save(String.Format(@"{0}OUT_{1}.xml", _xmlSavePath, lastMessageId));
            }

            CisExceptionStatus = null;
            CisErrorReply = null;

            #region Input parameter testing

            if (soapMessage == null)
            {
                throw new ArgumentNullException("soapMessage", "Method: SoapMessenger.SendSoapMessage Parameter: soapMessage");
            }
            if (string.IsNullOrEmpty(soapMessage.InnerXml) || soapMessage.DocumentElement == null)
            {
                throw new ArgumentException("Method: SoapMessenger.SendSoapMessage Parameter: soapMessage Error: InnerXml is null or empty, or DocumentElement is null");
            }

            #endregion

            var cisUri = new Uri(_cisUrl);
            if (cisUri == null)
            {
                throw new UriFormatException("Method: SoapMessenger.SendSoapMessage Error: Could not find web service. Check URL");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var cisRequest = WebRequest.Create(cisUri) as HttpWebRequest;
            if (cisRequest == null)
            {
                throw new NotSupportedException("Method: SoapMessenger.SendSoapMessage Error: Could not establish connection to web service");
            }

            cisRequest.Timeout = _timeOut;
            cisRequest.ContentType = "text/xml";
            cisRequest.Method = "POST";

            var innerXml = UTF8Encoding.UTF8.GetBytes(soapMessage.InnerXml);
            cisRequest.ProtocolVersion = HttpVersion.Version11;
            cisRequest.ContentLength = innerXml.Length;

            try
            {
                lastMessageTimeStamps[0] = DateTime.Now;
                using (var requestStream = cisRequest.GetRequestStream())
                {
                    requestStream.Write(innerXml, 0, innerXml.Length);
                }

                var cisResponse = cisRequest.GetResponse() as HttpWebResponse;
                if (cisResponse != null)
                {
                    var soapReply = new XmlDocument { PreserveWhitespace = true };
                    using (var cisResponseStream = cisResponse.GetResponseStream())
                    {
                        using (var cisResponseReader = new StreamReader(cisResponseStream, Encoding.GetEncoding("utf-8")))
                        {
                            soapReply.LoadXml(cisResponseReader.ReadToEnd());
                        }
                    }
                    if (!string.IsNullOrEmpty(_xmlSavePath))
                    {
                        if (NewLogEntryNeeded != null)
                        {
                            NewLogEntryNeeded(String.Format(@"Writing INPUT XML: {0}IN_{1}.xml", _xmlSavePath, lastMessageId));
                        }
                        soapReply.Save(String.Format(@"{0}IN_{1}.xml", _xmlSavePath, lastMessageId));
                    }
                    lastMessageTimeStamps[1] = DateTime.Now;
                    if (NewLogEntryNeeded != null)
                    {
                        NewLogEntryNeeded("SOAPMessenger returning XML to service.");
                    }
                    return soapReply;
                }
            }
            catch (WebException ex)
            {
                CisExceptionStatus = ex.Status;

                if (CisExceptionStatus == WebExceptionStatus.Timeout)
                {
                    throw new ServerException("Method: SoapMessenger.SendSoapMessage Error: Message sent successfuly, but the server did not respond before Timeout period elapsed");
                }

                if (ex.Response != null)
                {
                    using (var cisResponseStream = ex.Response.GetResponseStream())
                    {
                        using (var cisResponseReader = new StreamReader(cisResponseStream))
                        {
                            CisErrorReply = new XmlDocument();
                            CisErrorReply.Load(cisResponseReader);
                            if (!string.IsNullOrEmpty(_xmlSavePath))
                            {
                                CisErrorReply.Save(String.Format(@"{0}IN_{1}.xml", _xmlSavePath, lastMessageId));
                            }
                        }

                        throw new ServerException("Method: SoapMessenger.SendSoapMessage Error: Message sent successfuly, but CIS returned error response", ex);
                    }
                }

                throw new ServerException("Method: SoapMessenger.SendSoapMessage Error: Message sent successfuly, but CIS returned error without response", ex);
            }
            catch (Exception ex)
            {
                throw new ServerException("Method: SoapMessenger.SendSoapMessage Error: Error occured while sending SOAP message", ex);
            }

            throw new ServerException("Method: SoapMessenger.SendSoapMessage Error: Message sent successfuly, but CIS returned empty response");
        }

        /// <summary>Potpisuje, umata u SOAP omotnicu i šalje XML dokument</summary>
        /// <param name="certificate">Certifikat</param>
        /// <param name="request">XML dokument koji sadrži nepotpisani neomotani zahtjev za CIS servis</param>
        /// <returns>Odgovor CIS servisa</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        private XmlDocument SignEnvelopAndSendRequest(X509Certificate2 certificate, XmlDocument request)
        {
            Helper.SignXmlDocument(certificate, request);
            AppendSoapEnvelopeToXmlDocument(ref request);

            return SendSoapMessage(request);
        }
        /// <summary>Potpisuje, umata u SOAP omotnicu i šalje XML dokument</summary>
        /// <param name="certificateName">Naziv certifikata koji se koristi (npr. "FISKAL 1")</param>
        /// <param name="request">XML dokument koji sadrži nepotpisani neomotani zahtjev za CIS servis</param>
        /// <returns>Odgovor CIS servisa</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije pronađen</exception>
        /// <exception cref="UriFormatException">Nije moguće ostvariti vezu prema CIS web servisu</exception>
        /// <exception cref="NotSupportedException">Web servis odbija vezu</exception>
        /// <exception cref="System.Security.SecurityException">Nedovoljna prava pristupa web servisu</exception>
        /// <exception cref="ServerException">Greška prilikom komunikacije ili tijekom dijaloga s web servisom</exception>
        private XmlDocument SignEnvelopAndSendRequest(string certificateName, XmlDocument request)
        {
            return SignEnvelopAndSendRequest(Helper.GetCertificate(certificateName), request);
        }
        /// <summary>Potpisuje, umata u SOAP omotnicu i šalje XML dokument</summary>
        /// <param name="certificateFilenameAndPath">Naziv i puna adresa .pfx datoteke.</param>
        /// <param name="certificatePassword">Lozinka za pristup .pfx datoteci.</param>
        /// <param name="request">XML dokument koji sadrži nepotpisani neomotani zahtjev za CIS servis</param>
        /// <returns>Odgovor CIS servisa</returns>
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
        private XmlDocument SignEnvelopAndSendRequest(string certificateFilenameAndPath, string certificatePassword, XmlDocument request)
        {
            return SignEnvelopAndSendRequest(Helper.GetCertificate(certificateFilenameAndPath, certificatePassword), request);
        }

        #endregion

        #region ---HELPER---

        /// <summary>Dodaje SOAP omotnicu XML dokumentu</summary>
        /// <param name="xmlDocument">XML dokument</param>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        internal static void AppendSoapEnvelopeToXmlDocument(ref XmlDocument xmlDocument)
        {
            #region Input parameter testing

            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument", "Method: SOAPMessenger.AppendSoapEnvelopeToXmlDocument Parameter: xmlDocument");
            }
            else if (string.IsNullOrEmpty(xmlDocument.InnerXml) || xmlDocument.DocumentElement == null)
            {
                throw new ArgumentException("Method: SOAPMessenger.AppendSoapEnvelopeToXmlDocument Parameter: xmlDocument Error: InnerXml is null or empty, or DocumentElement is null");
            }

            #endregion

            var sb = new StringBuilder();
            sb.AppendFormat(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchemainstance""><soapenv:Body><{0}", xmlDocument.DocumentElement.Name);
            for (var i = 0; i < xmlDocument.DocumentElement.Attributes.Count; i++)
            {
                sb.AppendFormat(@" {0}=""{1}""", xmlDocument.DocumentElement.Attributes[i].Name, xmlDocument.DocumentElement.Attributes[i].Value);
            }
            sb.AppendFormat(">{0}</{1}></soapenv:Body></soapenv:Envelope>", xmlDocument.DocumentElement.InnerXml, xmlDocument.DocumentElement.Name);

            sb.Replace(@"<tns:Zaglavlje xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73"">", "<tns:Zaglavlje>");
            sb.Replace(@"<tns:Racun xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73"">", "<tns:Racun>");

            try
            {
                xmlDocument.LoadXml(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new XmlException("Method: SOAPMessenger.AppendSoapEnvelopeToXmlDocument Error: An error occured during the text parsing. See inner exception for details", ex);
            }
        }
        
        #endregion
    }
}
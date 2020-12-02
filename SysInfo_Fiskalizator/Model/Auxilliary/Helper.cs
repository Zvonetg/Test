using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.ComponentModel;

namespace SysInfo_Fiskalizator.Model.Auxilliary
{
    public static class Helper
    {
        #region ---GENERATING---

        /// <summary>Generira novi GUID</summary>
        /// <returns>Tekstualni zapis GUID-a</returns>
        internal static string GenerateGuidString()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Dohvaća opis (Description) danog enuma. Ukoliko enum nema opis, vraća njegov ekstualni zapis.
        /// </summary>
        /// <param name="value">Enum</param>
        /// <returns>Opis enuma</returns>
        internal static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[]) fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        #endregion

        #region ---FORMATING---

        /// <summary>Pretvara ulazni DateTime u string prema formatu CIS poslužitelja (dd.MM.yyyyTHH:mm:ss). Ukoliko je zastavica withoutTime postavljena, ne prikazuje se vrijeme.</summary>
        /// <param name="dateTime">Ulazni DateTime</param>
        /// <param name="withoutTime">Zastavica: True - ne prikazuje se vrijeme; False - prikazuje se vrijeme</param>
        /// <returns>Formatirani tekstualni zapis ulaza</returns>
        internal static string FormatDateTimeToCisDateTimeFormat(DateTime dateTime, bool withoutTime)
        {
            return String.Format("{0:dd.MM.yyyy}T{0:HH:mm:ss}", dateTime);
        }

        #endregion

        #region ---SECURITY---

        /// <summary>Izračunava MD5 sažetak</summary>
        /// <param name="subject">Subjekt čiji sažetak se traži</param>
        /// <returns>MD5 sažetak</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        private static string ComputeHash(byte[] subject)
        {
            #region Input parameter testing

            if (subject == null)
            {
                throw new ArgumentNullException("subject", "Method: Helper.ComputeHash Parameter: subject");
            }

            #endregion

            var result = MD5.Create().ComputeHash(subject);

            var sb = new StringBuilder();
            for (var i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>Generira zaštitni kod izdavatelja (ZKI) na temelju ulaznih parametara računa i danog certifikata.</summary>
        /// <param name="certificate">Certifikat kojim se potpisuje</param>
        /// <param name="companyTaxId">OIB izdavatelja</param>
        /// <param name="invoiceIssueDateTime">Datum i vrijeme izdavanja</param>
        /// <param name="invoiceIdNumber">Redni broj računa</param>
        /// <param name="workspaceId">Oznaka poslovnog prostora</param>
        /// <param name="cashRegisterId">Oznaka naplatnog uređaja</param>
        /// <param name="invoiceTotal">Ukupni iznos računa</param>
        /// <returns>Zaštitni kod izdavatelja u propisanom formatu</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        internal static string GenerateSafetyCode(X509Certificate2 certificate, string companyTaxId, string invoiceIssueDateTime, string invoiceIdNumber, string workspaceId, string cashRegisterId, string invoiceTotal)
        {
            #region Input parameter testing

            if (certificate == null)
            {
                throw new ArgumentNullException("certificate", "Method: Helper.GenerateSafetyCode Parameter: certificate");
            }
            if (string.IsNullOrEmpty(companyTaxId))
            {
                throw new ArgumentNullException("companyTaxId", "Method: Helper.GenerateSafetyCode Parameter: companyTaxId");
            }
            if (string.IsNullOrEmpty(invoiceIssueDateTime))
            {
                throw new ArgumentNullException("invoiceIssueDateTime", "Method: Helper.GenerateSafetyCode Parameter: invoiceIssueDateTime");
            }
            if (string.IsNullOrEmpty(invoiceIdNumber))
            {
                throw new ArgumentNullException("invoiceIdNumber", "Method: Helper.GenerateSafetyCode Parameter: invoiceIdNumber");
            }
            if (string.IsNullOrEmpty(workspaceId))
            {
                throw new ArgumentNullException("workspaceId", "Method: Helper.GenerateSafetyCode Parameter: workspaceId");
            }
            if (string.IsNullOrEmpty(cashRegisterId))
            {
                throw new ArgumentNullException("cashRegisterId", "Method: Helper.GenerateSafetyCode Parameter: cashRegisterId");
            }

            #endregion

            var data = SignPlainText(certificate, String.Format("{0}{1}{2}{3}{4}{5}", companyTaxId, invoiceIssueDateTime, invoiceIdNumber, workspaceId, cashRegisterId, invoiceTotal.Replace(',', '.')));

            return (data != null ? ComputeHash(data) : "");
        }
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
        internal static string GenerateSafetyCode(string certificateName, string companyTaxId, string invoiceIssueDateTime, string invoiceIdNumber, string workspaceId, string cashRegisterId, string invoiceTotal)
        {
            #region Input parameter testing

            if (string.IsNullOrEmpty(certificateName))
            {
                throw new ArgumentNullException("certificateName", "Method: Helper.GenerateSafetyCode Parameter: certificateName");
            }

            #endregion

            return GenerateSafetyCode(GetCertificate(certificateName), companyTaxId, invoiceIssueDateTime, invoiceIdNumber, workspaceId, cashRegisterId, invoiceTotal);
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
        internal static string GenerateSafetyCode(string certificateFilenameAndPath, string certificatePassword, string companyTaxId, string invoiceIssueDateTime, string invoiceIdNumber, string workspaceId, string cashRegisterId, string invoiceTotal)
        {
            #region Input parameter testing

            if (string.IsNullOrEmpty(certificateFilenameAndPath))
            {
                throw new ArgumentNullException("certificateFilenameWithPath", "Method: Helper.GenerateSafetyCode Parameter: certificateFilenameWithPath");
            }
            if (string.IsNullOrEmpty(certificatePassword))
            {
                throw new ArgumentNullException("certificatePassword", "Method: Helper.GenerateSafetyCode Parameter: certificatePassword");
            }

            #endregion

            return GenerateSafetyCode(GetCertificate(certificateFilenameAndPath, certificatePassword), companyTaxId, invoiceIssueDateTime, invoiceIdNumber, workspaceId, cashRegisterId, invoiceTotal);
        }

        /// <summary>Dohvaća X509 certifikat iz Certificate Store-a (StoreLocation.CurrentUser, StoreName.My) po nazivu.</summary>
        /// <param name="certificateName">Naziv certifikata koji se koristi (npr. "FISKAL 1").</param>
        /// <returns>Vraća dohvaćeni certifikat.</returns>
        /// <exception cref="KeyNotFoundException">Certifikat nije pronađen</exception>
        internal static X509Certificate2 GetCertificate(string certificateName)
        {
            var certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certificateStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            foreach (var item in certificateStore.Certificates)
            {
                if (item.Subject.StartsWith(String.Format("CN={0}", certificateName)))
                {
                    return item;
                }
            }

            throw new KeyNotFoundException("Method: Helper.GetCertificate Error: Certificate with the requested name could not be found in the certificate store of the current user");
        }
        /// <summary>Dohvaća X509 certifikat iz .pfx datoteke.</summary>
        /// <param name="certificateFilenameAndPath">Naziv i puna adresa .pfx datoteke.</param>
        /// <param name="certificatePassword">Lozinka za pristup .pfx datoteci.</param>
        /// <returns>Vraća dohvaćeni certifikat.</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="FileNotFoundException">Datoteka ne postoji</exception>
        /// <exception cref="KeyNotFoundException">Certifikat nije moguće dohvatiti</exception>
        internal static X509Certificate2 GetCertificate(string certificateFilenameAndPath, string certificatePassword)
        {
            #region Input parameter testing

            if (string.IsNullOrEmpty(certificateFilenameAndPath))
            {
                throw new ArgumentNullException("certificateFilenameAndFullPath", "Method: Helper.GetCertificate Parameter: certificateFilenameAndFullPath");
            }
            if (string.IsNullOrEmpty(certificatePassword))
            {
                throw new ArgumentNullException("certificatePassword", "Method: Helper.GetCertificate Parameter: certificatePassword");
            }
            if (!new FileInfo(certificateFilenameAndPath).Exists)
            {
                throw new FileNotFoundException("Method: Helper.GetCertificate Error: File could not be found");
            }

            #endregion

            try
            {
                return new X509Certificate2(certificateFilenameAndPath, certificatePassword);
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException("Method: Helper.GetCertificate Error: An error occured during the certificate retrieval. See inner exception for details", ex);
            }
        }

        /// <summary>Potpisuje plain tekst danim certifikatom</summary>
        /// <param name="certificate">Certifikat kojim se potpisuje</param>
        /// <param name="plainText">Plain tekst koji se potpisuje</param>
        /// <returns>Potpisani tekst</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        internal static byte[] SignPlainText(X509Certificate2 certificate, string plainText)
        {
            #region Input parameter testing

            if (certificate == null)
            {
                throw new ArgumentNullException("certificate", "Method: Helper.SignPlainText Parameter: certificate");
            }
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText", "Method: Helper.SignPlainText Parameter: plainText");
            }

            #endregion

            try
            {
                var data = Encoding.ASCII.GetBytes(plainText);
                return ((RSACryptoServiceProvider)certificate.PrivateKey).SignData(data, new SHA1CryptoServiceProvider());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Method: Helper.SignPlainText Error: An error occured during the signing process. See inner exception for details", ex);
            }
        }

        /// <summary>Potpisuje XML dokument sanim certifikatom</summary>
        /// <param name="certificate">Certifikat kojim se potpisuje</param>
        /// <param name="xmlDocument">XML dokument koji se potpisuje</param>
        /// <returns>Potpisani XMl dokument</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="ArgumentException">Elementi ulaznog parametra nisu zadani</exception>
        /// <exception cref="ApplicationException">Greška u procesu potpisivanja</exception>
        public static XmlDocument SignXmlDocument(X509Certificate2 certificate, XmlDocument xmlDocument)
        {
            #region Input parameter testing

            if (certificate == null)
            {
                throw new ArgumentNullException("certificate", "Method: Helper.SignXmlDocument Parameter: certificate");
            }
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument", "Method: Helper.SignXmlDocument Parameter: xmlDocument");
            }
            if (string.IsNullOrEmpty(xmlDocument.InnerXml) || xmlDocument.DocumentElement == null)
            {
                throw new ArgumentException("Method: Helper.SignXmlDocument Parameter: xmlDocument Error: InnerXml is null or empty, or DocumentElement is null");
            }

            #endregion

            try
            {
                var signedXml = new SignedXml(xmlDocument);
                signedXml.SigningKey = certificate.PrivateKey;
                signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

                var keyInfoData = new KeyInfoX509Data();
                keyInfoData.AddCertificate(certificate);
                keyInfoData.AddIssuerSerial(certificate.Issuer, certificate.GetSerialNumberString());

                var keyInfo = new KeyInfo();
                keyInfo.AddClause(keyInfoData);

                signedXml.KeyInfo = keyInfo;

                var reference = new Reference("");
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform(false));
                reference.AddTransform(new XmlDsigExcC14NTransform(false));
                reference.Uri = "#signXmlId";
                signedXml.AddReference(reference);
                signedXml.ComputeSignature();

                var xmlElement = signedXml.GetXml();
                xmlDocument.DocumentElement.AppendChild(xmlElement);
                return xmlDocument;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Method: Helper.SignXmlDocument Error: An error occured during the signing process. See inner exception for details", ex);
            }
        }

        #endregion

        #region ---XML OPERATIONS---

        /// <summary>Parsira tekst u XmlDocument</summary>
        /// <param name="text">Tekst koji se parsira</param>
        /// <returns>XmlDocument</returns>
        /// <exception cref="ArgumentNullException">Ulazni parametar nije zadan</exception>
        /// <exception cref="XmlException">Greška prilikom parsiranja</exception>
        internal static XmlDocument ParseStringToXmlDocument(string text)
        {
            #region Input parameter testing

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text", "Method: Helper.ParseStringToXmlDocument Parameter: text");
            }

            #endregion

            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                return xmlDocument;
            }
            catch (Exception ex)
            {
                throw new XmlException("Method: Helper.ParseStringToXmlDocument Error: An error occured during the text parsing. See inner exception for details", ex);
            }
        }

        #endregion
    }
}

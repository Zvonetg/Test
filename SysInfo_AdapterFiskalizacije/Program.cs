using System;
using System.Linq;
using SysInfo_Fiskalizator.Model;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Remoting;
using SysInfo_Fiskalizator.Util;
using System.Xml.Linq;

namespace SysInfo_AdapterFiskalizacije
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings(false);

            Util.ParseArguments(args, settings);

            if (settings.Operation == OperationType.UNKNOWN)
            {
                WriteToFile(settings, new[] { "FALSE", "Operation unknown" });
                return;
            }

            var fiskalizator = new Fiskalizator(Properties.Settings.Default.CIS_URLTEST, null, settings.TimeOut);

            try
            {
                File.Delete(settings.OutputFile);
            }
            catch (DirectoryNotFoundException) { }
            catch
            {
                return;
            }

            try
            {
                switch (settings.Operation)
                {
                    case OperationType.ECHO:
                        SendEcho(fiskalizator, Util.ReadFromFile(settings.InputFile), settings);
                        break;

                    case OperationType.WORKSPACE:
                        SendWorkspace(fiskalizator, settings);
                        break;

                    case OperationType.INVOICE:
                        SendInvoice(fiskalizator, settings);
                        break;

                    case OperationType.CHECK:
                        SendCheck(fiskalizator, settings);
                        break;

                    case OperationType.SAFETYCODE:
                        GenerateSafetyCode(settings);
                        break;
                }
            }
            catch (MyExitException) { }
            catch (Exception ex)
            {
                WriteToFile(settings, new[] { "FALSE", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : "") });
            }
        }
        
        private static void SendEcho(Fiskalizator fiskalizator, string testString, Settings settings)
        {
            try
            {
                var replyMessage = fiskalizator.SendEcho(testString).InnerText.Trim(new[] { '\r', '\n' });
                WriteToFile(settings, (replyMessage == testString ? new[] { "TRUE" }
                                                                  : new[] { "FALSE", String.Format("Received string ({0}) is different than sent string ({1})", replyMessage, testString) }));
            }
            catch (Exception ex)
            {
                try
                {
                    WriteToFile(settings, new [] { "FALSE", ex.Message });
                }
                catch { }
            }
        }

        private static void SendWorkspace(Fiskalizator fiskalizator, Settings settings)
        {
            try
            {
                var parameters = new List<string>(Util.ReadFromFile(settings.InputFile).Split('|'));

                if (parameters.Count != 17)
                {
                    WriteToFile(settings, new[] { "FALSE", "Input file parameter count missmatch." });
                    throw new MyExitException();
                }

                PoslovniProstorType workspace;
                string[] certificateParams;
                Util.ParseWorkspaceInput(parameters, out workspace, out certificateParams);

                if (certificateParams == null || !(certificateParams.Length == 1 || certificateParams.Length == 2))
                {
                    WriteToFile(settings, new[] { "FALSE", "Certificate definition error" });
                    throw new MyExitException();
                }

                XmlDocument reply;
                try
                {
                    reply = (certificateParams.Length == 1 ? fiskalizator.SendWorkspace(workspace, certificateParams[0])
                                                           : fiskalizator.SendWorkspace(workspace, certificateParams[0], certificateParams[1]));
                }
                catch (ServerException sex)
                {
                    reply = fiskalizator.CisErrorReply;
                    var lines = new List<string> { "FALSE" };
                    if (fiskalizator.CisExceptionStatus != null)
                    {
                        lines.Add("Server exception: " + fiskalizator.CisExceptionStatus);
                    }
                    try
                    {
                        var error = Fiskalizator.GetErrorCodeAndMessage(reply, MessageType.Workspace_From_CIS);
                        lines.Add("Server error: " + error[0] + ' ' + error[1]);
                    }
                    catch { }
                    lines.Add(String.Format("{0} {1}", sex.Message, (sex.InnerException != null ? sex.InnerException.Message : "")));
                    WriteToFile(settings, lines.ToArray());
                    throw new MyExitException();
                }

                WriteToFile(settings, new[] { "TRUE", Fiskalizator.GetMessageTimestamp(reply, MessageType.Workspace_From_CIS) });
            }
            catch (MyExitException)
            {
                throw new MyExitException();
            }
            catch (Exception ex)
            {
                WriteToFile(settings, new[] { "FALSE", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : "") });
            }
        }

        private static void SendInvoice(Fiskalizator fiskalizator, Settings settings)
        {
            try
            {
                var parameters = new List<string>(Util.ReadFromFile(settings.InputFile).Split('|'));

                if (parameters.Count != 23)
                {
                    WriteToFile(settings, new[] { "FALSE", "Input file parameter count missmatch." });
                    throw new MyExitException();
                }

                RacunType invoice;
                string[] certificateParams;
                Util.ParseInvoiceInput(parameters, out invoice, out certificateParams);

                if (certificateParams == null || !(certificateParams.Length == 1 || certificateParams.Length == 2))
                {
                    WriteToFile(settings, new[] { "FALSE", "Certificate definition error" });
                    throw new MyExitException();
                }

                XmlDocument reply;
                try
                {
                    reply = (certificateParams.Length == 1 ? fiskalizator.SendInvoice(invoice, certificateParams[0])
                                                           : fiskalizator.SendInvoice(invoice, certificateParams[0], certificateParams[1]));


                    var tekstGresaka = (string)null;
                    var doc = XDocument.Load(new XmlNodeReader(reply));
                    var greskeNode = doc.Elements().First()
                                        .Elements().First()
                                        .Elements().First()
                                        .Elements().SingleOrDefault(x => x.Name.ToString().EndsWith("Greske"));

                    if (greskeNode != null)
                    {
                        tekstGresaka = greskeNode.Elements()
                                                 .Select(x => string.Format("{0} {1}", x.Elements().Single(y => y.Name.ToString().EndsWith("SifraGreske")).Value,
                                                                                       x.Elements().Single(y => y.Name.ToString().EndsWith("PorukaGreske")).Value))
                                                 .Aggregate((x, y) => x + Environment.NewLine + y);
                    }
                    WriteToFile(settings, (tekstGresaka != null ? new[] { "FALSE", invoice.ZastKod, tekstGresaka }
                                                                : new[] { "TRUE", invoice.ZastKod, Fiskalizator.GetInvoiceUniqueIdentifier(reply) }));
                }
                catch (ServerException sex)
                {
                    reply = fiskalizator.CisErrorReply;
                    var lines = new List<string> { "FALSE", invoice.ZastKod };
                    if (fiskalizator.CisExceptionStatus != null)
                    {
                        lines.Add("Server exception: " + fiskalizator.CisExceptionStatus);
                    }
                    try
                    {
                        var error = Fiskalizator.GetErrorCodeAndMessage(reply, MessageType.Invoice_From_CIS);
                        lines.Add("Server error: " + error[0] + ' ' + error[1]);
                    }
                    catch { }
                    lines.Add(String.Format("{0} {1}", sex.Message, (sex.InnerException != null ? sex.InnerException.Message : "")));
                    WriteToFile(settings, lines.ToArray());
                    throw new MyExitException();
                }
            }
            catch (MyExitException)
            {
                throw new MyExitException();
            }
            catch (Exception ex)
            {
                WriteToFile(settings, new[] { "FALSE", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : "") });
            }
        }

        private static void SendCheck(Fiskalizator fiskalizator, Settings settings)
        {
            try
            {
                var parameters = new List<string>(Util.ReadFromFile(settings.InputFile).Split('|'));

                if (parameters.Count != 23)
                {
                    WriteToFile(settings, new[] { "FALSE", "Input file parameter count missmatch." });
                    throw new MyExitException();
                }

                RacunType invoice;
                string[] certificateParams;
                Util.ParseInvoiceInput(parameters, out invoice, out certificateParams);

                if (certificateParams == null || !(certificateParams.Length == 1 || certificateParams.Length == 2))
                {
                    WriteToFile(settings, new[] { "FALSE", "Certificate definition error" });
                    throw new MyExitException();
                }

                XmlDocument reply;
                try
                {
                    reply = (certificateParams.Length == 1 ? fiskalizator.SendCheck(invoice, certificateParams[0])
                                                           : fiskalizator.SendCheck(invoice, certificateParams[0], certificateParams[1]));
                    var tekstGresaka = (string)null;
                    var doc = XDocument.Load(new XmlNodeReader(reply));
                    var greskeNode = doc.Elements().First()
                                        .Elements().First()
                                        .Elements().First()
                                        .Elements().SingleOrDefault(x => x.Name.ToString().EndsWith("Greske"));

                    if (greskeNode != null)
                    {
                        tekstGresaka = greskeNode.Elements()
                                                 .Select(x => string.Format("{0} {1}", x.Elements().Single(y => y.Name.ToString().EndsWith("SifraGreske")).Value,
                                                                                       x.Elements().Single(y => y.Name.ToString().EndsWith("PorukaGreske")).Value))
                                                 .Aggregate((x, y) => x + Environment.NewLine + y);
                    }
                    WriteToFile(settings, (tekstGresaka != null ? new[] { "FALSE", tekstGresaka } 
                                                                : new[] { "TRUE" }));
                }
                catch (ServerException sex)
                {
                    reply = fiskalizator.CisErrorReply;
                    var lines = new List<string> { "FALSE", invoice.ZastKod };
                    if (fiskalizator.CisExceptionStatus != null)
                    {
                        lines.Add("Server exception: " + fiskalizator.CisExceptionStatus);
                    }
                    try
                    {
                        var error = Fiskalizator.GetErrorCodeAndMessage(reply, MessageType.Invoice_From_CIS);
                        lines.Add("Server error(s): " + error.Aggregate((x, y) => x + " " + y));
                    }
                    catch { }
                    lines.Add(String.Format("{0} {1}", sex.Message, (sex.InnerException != null ? sex.InnerException.Message : "")));
                    WriteToFile(settings, lines.ToArray());
                    throw new MyExitException();
                }
            }
            catch (MyExitException)
            {
                throw new MyExitException();
            }
            catch (Exception ex)
            {
                WriteToFile(settings, new[] { "FALSE", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : "") });
            }
        }
        
        private static void WriteToFile(Settings settings, string[] data)
        {
            try
            {
                using (TextWriter outputWriter = new StreamWriter(settings.OutputFile))
                {
                    foreach (var line in data)
                    {
                        outputWriter.WriteLine(line);
                    }
                    outputWriter.Flush();
                }
            }
            catch { }
        }

        private static void GenerateSafetyCode(Settings settings)
        {
            var parameters = new List<string>(Util.ReadFromFile(settings.InputFile).Split('|'));

            if (parameters.Count != 10)
            {
                WriteToFile(settings, new[] { "FALSE", "Input file parameter count missmatch." });
                throw new MyExitException();
            }

            RacunType invoice;
            string[] certificateParams;
            Util.ParseSafetyKeyInput(parameters, out invoice, out certificateParams);

            if (certificateParams == null || !(certificateParams.Length == 1 || certificateParams.Length == 2))
            {
                WriteToFile(settings, new[] { "FALSE", "Certificate definition error" });
                throw new MyExitException();
            }

            WriteToFile(settings, new[] { "TRUE", (certificateParams.Length == 1 ? Fiskalizator.GenerateSafetyCode(certificateParams[0], invoice.Oib, invoice.DatVrijeme, invoice.BrRac.BrOznRac, invoice.BrRac.OznPosPr, invoice.BrRac.OznNapUr, invoice.IznosUkupno)
                                                                                 : Fiskalizator.GenerateSafetyCode(certificateParams[0], certificateParams[1], invoice.Oib, invoice.DatVrijeme, invoice.BrRac.BrOznRac, invoice.BrRac.OznPosPr, invoice.BrRac.OznNapUr, invoice.IznosUkupno)) });
        }
    }
}
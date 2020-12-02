using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.IO;
using System.Xml;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml.Linq;
using SysInfo_Fiskalizator.Model;
using SysInfo_Fiskalizator.Util;

namespace SysInfo_ServisFiskalizacije
{
    public partial class SysInfo_ServisFiskalizacije : ServiceBase
    {
        private readonly Timer _timer;
        private Settings _settings;
        private DateTime _operationStartTimestamp;

        public SysInfo_ServisFiskalizacije()
        {
            _timer = new Timer(100);
            _timer.Elapsed += Timer_Elapsed;

            InitializeComponent();

            if (!EventLog.SourceExists("SysInfo_Servis_fiskalizacije"))
            {
                EventLog.CreateEventSource("SysInfo_Servis_fiskalizacije", "SysInfo_Servis_fiskalizacije_Log");
            }
            eventLog.Source = "SysInfo_Servis_fiskalizacije";
            eventLog.Log = "SysInfo_Servis_fiskalizacije_Log";

            ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timer.Stop();
                _settings = new Settings(true);
                _operationStartTimestamp = DateTime.Now;

                List<string> inputParameters = null;

                try
                {
                    inputParameters = new List<string>(Util.ReadFromFile(_settings.InputFile).Split('|'));
                    File.Delete(_settings.InputFile);
                }
                catch
                {
                    throw new MyExitException();
                }

                Util.ParseArguments(inputParameters[0].Split(';'), _settings);
                inputParameters.RemoveAt(0);

                if (_settings.Operation == OperationType.UNKNOWN)
                {
                    WriteToFile(new[] {"FALSE", "Operation unknown"});
                    return;
                }

                var fiskalizator = new Fiskalizator(_settings.CisWebServiceUrl, _settings.XMLOutputFolder, _settings.TimeOut);
                fiskalizator.NewLogEntryNeeded += fiskalizator_NewLogEntryNeeded;

                try
                {
                    File.Delete(_settings.OutputFile);
                }
                catch (DirectoryNotFoundException)
                {
                }
                catch
                {
                    throw new MyExitException();
                }

                try
                {
                    switch (_settings.Operation)
                    {
                        case OperationType.ECHO:
                            SendEcho(fiskalizator, inputParameters[0]);
                            break;

                        case OperationType.WORKSPACE:
                            SendWorkspace(fiskalizator, inputParameters);
                            break;

                        case OperationType.INVOICE:
                            SendInvoice(fiskalizator, inputParameters);
                            break;

                        case OperationType.CHECK:
                            SendCheck(fiskalizator, inputParameters);
                            break;

                        case OperationType.SAFETYCODE:
                            GenerateSafetyCode(inputParameters);
                            break;
                    }
                }
                catch (MyExitException) { }
            }
            catch (MyExitException) { }
            catch (Exception ex)
            {
                if (_settings.WriteDebugToLog)
                {
                    WriteToLog("General exception in service loop. See next entry for exception details.");
                    WriteToLog(GenerateExceptionMessages(ex, 714));
                }
                WriteToFile(new[] {"FALSE", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : "")});
            }
            finally
            {
                _timer.Start();
            }
        }

        private void fiskalizator_NewLogEntryNeeded(string message)
        {
            if (_settings.WriteDebugToLog)
            {
                WriteToLog(message);
            }
        }

        protected override void OnStart(string[] args)
        {
            WriteToLog("Service started");
            _timer.Start();
        }

        protected override void OnPause()
        {
            _timer.Stop();
            WriteToLog("Service paused");
        }

        protected override void OnContinue()
        {
            _timer.Start();
            WriteToLog("Servicee resumed");
        }
        
        protected override void OnStop()
        {
            _timer.Stop();
            WriteToLog("Service stopped");
        }

        protected override void OnShutdown()
        {
            _timer.Stop();
            WriteToLog("Service shutdown");
        }

        private void WriteToLog(string message)
        {
            try
            {
                eventLog.WriteEntry(String.Format("[{0:dd.MM.yyyy HH:mm:ss}] {1}", DateTime.Now, message), EventLogEntryType.Information);
            }
            catch { }
        }

        private string GenerateExceptionMessages(Exception ex, int lineNumber)
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.AppendFormat("[{0:dd.MM.yyyy HH:mm:ss}] Greška servisa: [Linija {1}]", DateTime.Now, lineNumber);
            errorBuilder.AppendLine();
            while (ex != null)
            {
                errorBuilder.AppendFormat("Exception: {0} Message: {1}", ex.GetType(), ex.Message);
                errorBuilder.AppendLine();
                ex = ex.InnerException;
            }

            return errorBuilder.ToString();
        }

        private void SendEcho(Fiskalizator fiskalizator, string testString)
        {
            try
            {
                var replyMessage = fiskalizator.SendEcho(testString).InnerText.Trim(new[] { '\r', '\n' });
                WriteToFile((replyMessage == testString ? new[] { "TRUE", (fiskalizator.LastMessageTimeStamps != null ? String.Format("Komunikacija: {0} - {1} ({2})", fiskalizator.LastMessageTimeStamps[0], fiskalizator.LastMessageTimeStamps[1], fiskalizator.LastMessageTimeStamps[1] - fiskalizator.LastMessageTimeStamps[0]) : "") }
                                                        : new[] { "FALSE", String.Format("Received string ({0}) is different than sent string ({1})", replyMessage, testString) }));
            }
            catch (Exception ex)
            {
                try
                {
                    WriteToFile(new[] { "FALSE", ex.Message });
                }
                catch (Exception wex)
                {
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Unable to write to file. See next two log entries for details. [Line 271]");
                        WriteToLog(GenerateExceptionMessages(ex, 263));
                        WriteToLog(GenerateExceptionMessages(wex, 269));
                    }
                }
            }
        }
        
        private void SendWorkspace(Fiskalizator fiskalizator, List<string> inputParameters)
        {
            try
            {
                if (inputParameters.Count != 17)
                {
                    WriteToFile(new[] { "FALSE", "Input file parameter count missmatch." });
                    throw new MyExitException();
                }

                PoslovniProstorType workspace;
                string[] certificateParams;
                Util.ParseWorkspaceInput(inputParameters, out workspace, out certificateParams);

                if (certificateParams == null || !(certificateParams.Length == 1 || certificateParams.Length == 2))
                {
                    WriteToFile(new[] { "FALSE", "Certificate definition error" });
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
                    catch (Exception ex)
                    {
                        if (_settings.WriteDebugToLog)
                        {
                            WriteToLog(GenerateExceptionMessages(sex, 416));
                            WriteToLog(GenerateExceptionMessages(ex, 432));
                        }
                    }
                    lines.Add(String.Format("{0} {1}", sex.Message, (sex.InnerException != null ? sex.InnerException.Message : "")));
                    WriteToFile(lines.ToArray());
                    throw new MyExitException();
                }

                WriteToFile(new[] { "TRUE", Fiskalizator.GetMessageTimestamp(reply, MessageType.Workspace_From_CIS), (fiskalizator.LastMessageTimeStamps != null ? String.Format("Komunikacija: {0} - {1} ({2})", fiskalizator.LastMessageTimeStamps[0], fiskalizator.LastMessageTimeStamps[1], fiskalizator.LastMessageTimeStamps[1] - fiskalizator.LastMessageTimeStamps[0]) : "") });
            }
            catch (MyExitException)
            {
                throw new MyExitException();
            }
            catch (Exception ex)
            {
                WriteToFile(new[] { "FALSE", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : "") });
            }
        }

        private void SendInvoice(Fiskalizator fiskalizator, List<string> inputParameters)
        {
            try
            {
                if (inputParameters.Count != 23)
                {
                    WriteToFile(new[] { "FALSE", "Input file parameter count missmatch." });
                    throw new MyExitException();
                }

                RacunType invoice;
                string[] certificateParams;
                Util.ParseInvoiceInput(inputParameters, out invoice, out certificateParams);

                if (certificateParams == null || !(certificateParams.Length == 1 || certificateParams.Length == 2))
                {
                    WriteToFile(new[] { "FALSE", "Certificate definition error" });
                    throw new MyExitException();
                }

                XmlDocument reply;
                try
                {
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Prepared to send invoice.");
                    }
                    reply = (certificateParams.Length == 1 ? fiskalizator.SendInvoice(invoice, certificateParams[0])
                                                           : fiskalizator.SendInvoice(invoice, certificateParams[0], certificateParams[1]));
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Invoice sent and reply recieved.");
                    }
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Sucessful reply - starting to write output.out [Line 707]");
                    }
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
                    WriteToFile((tekstGresaka != null ? new[] { "FALSE", invoice.ZastKod, tekstGresaka, (fiskalizator.LastMessageTimeStamps != null ? String.Format("Komunikacija: {0} - {1} ({2})", fiskalizator.LastMessageTimeStamps[0], fiskalizator.LastMessageTimeStamps[1], fiskalizator.LastMessageTimeStamps[1] - fiskalizator.LastMessageTimeStamps[0]) : "") }
                                                      : new[] { "TRUE", invoice.ZastKod, Fiskalizator.GetInvoiceUniqueIdentifier(reply), (fiskalizator.LastMessageTimeStamps != null ? String.Format("Komunikacija: {0} - {1} ({2})", fiskalizator.LastMessageTimeStamps[0], fiskalizator.LastMessageTimeStamps[1], fiskalizator.LastMessageTimeStamps[1] - fiskalizator.LastMessageTimeStamps[0]) : "") }));
                }
                catch (ServerException sex)
                {
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Recieved ServerException - message parsing started [Line 681]");
                    }

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
                    catch (Exception ex)
                    {
                        if (_settings.WriteDebugToLog)
                        {
                            WriteToLog(GenerateExceptionMessages(sex, 670));
                            WriteToLog(GenerateExceptionMessages(ex, 683));
                        }
                    }
                    lines.Add(String.Format("{0} {1}", sex.Message, (sex.InnerException != null ? sex.InnerException.Message : "")));
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("ServerException sucessfuly parsed - starting to write output.out [Line 702]");
                    }
                    WriteToFile(lines.ToArray());
                    throw new MyExitException();
                }
            }
            catch (MyExitException)
            {
                throw new MyExitException();
            }
            catch (Exception ex)
            {
                if (_settings.WriteDebugToLog)
                {
                    WriteToLog("Exception in SendInvoice. See next entry for exception details.");
                    WriteToLog(GenerateExceptionMessages(ex, 714));
                }
                WriteToFile(new[] { "FALSE", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : "") });
            }
        }

        private void SendCheck(Fiskalizator fiskalizator, List<string> inputParameters)
        {
            try
            {
                if (inputParameters.Count != 23)
                {
                    WriteToFile(new[] { "FALSE", "Input file parameter count missmatch." });
                    throw new MyExitException();
                }

                RacunType invoice;
                string[] certificateParams;
                Util.ParseInvoiceInput(inputParameters, out invoice, out certificateParams);

                if (certificateParams == null || !(certificateParams.Length == 1 || certificateParams.Length == 2))
                {
                    WriteToFile(new[] { "FALSE", "Certificate definition error" });
                    throw new MyExitException();
                }

                XmlDocument reply;
                try
                {
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Prepared to send invoice.");
                    }
                    reply = (certificateParams.Length == 1 ? fiskalizator.SendCheck(invoice, certificateParams[0])
                                                           : fiskalizator.SendCheck(invoice, certificateParams[0], certificateParams[1]));
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Invoice sent and reply recieved.");
                    }
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Sucessful reply - starting to write output.out [Line 707]");
                    }
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
                    WriteToFile((tekstGresaka != null ? new[] { "FALSE", tekstGresaka, (fiskalizator.LastMessageTimeStamps != null ? String.Format("Komunikacija: {0} - {1} ({2})", fiskalizator.LastMessageTimeStamps[0], fiskalizator.LastMessageTimeStamps[1], fiskalizator.LastMessageTimeStamps[1] - fiskalizator.LastMessageTimeStamps[0]) : "") }
                                                      : new[] { "TRUE", (fiskalizator.LastMessageTimeStamps != null ? String.Format("Komunikacija: {0} - {1} ({2})", fiskalizator.LastMessageTimeStamps[0], fiskalizator.LastMessageTimeStamps[1], fiskalizator.LastMessageTimeStamps[1] - fiskalizator.LastMessageTimeStamps[0]) : "") }));
                }
                catch (ServerException sex)
                {
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("Recieved ServerException - message parsing started [Line 681]");
                    }

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
                    catch (Exception ex)
                    {
                        if (_settings.WriteDebugToLog)
                        {
                            WriteToLog(GenerateExceptionMessages(sex, 670));
                            WriteToLog(GenerateExceptionMessages(ex, 683));
                        }
                    }
                    lines.Add(String.Format("{0} {1}", sex.Message, (sex.InnerException != null ? sex.InnerException.Message : "")));
                    if (_settings.WriteDebugToLog)
                    {
                        WriteToLog("ServerException sucessfuly parsed - starting to write output.out [Line 702]");
                    }
                    WriteToFile(lines.ToArray());
                    throw new MyExitException();
                }
            }
            catch (MyExitException)
            {
                throw new MyExitException();
            }
            catch (Exception ex)
            {
                if (_settings.WriteDebugToLog)
                {
                    WriteToLog("Exception in SendInvoice. See next entry for exception details.");
                    WriteToLog(GenerateExceptionMessages(ex, 714));
                }
                WriteToFile(new[] { "FALSE", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : "") });
            }
        }

        private void WriteToFile(string[] data)
        {
            try
            {
                if (_settings.WriteDebugToLog)
                {
                    WriteToLog("WriteToFile Initiated [Line 167]");
                }
                var messageBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    messageBuilder.AppendLine(data[i]);
                }
                DateTime _operationEndTimestamp = DateTime.Now;
                messageBuilder.AppendLine(String.Format("Rad: {0} - {1} ({2})", _operationStartTimestamp, _operationEndTimestamp, _operationEndTimestamp - _operationStartTimestamp));
                WriteToLog(messageBuilder.ToString());

                if (_settings.WriteDebugToLog)
                {
                    WriteToLog("Started writting procedure [Line 175]");
                }
                using (TextWriter outputWriter = new StreamWriter(_settings.OutputFile))
                {
                    foreach (var line in data)
                    {
                        outputWriter.WriteLine(line);
                    }
                    outputWriter.Flush();
                }
                if (_settings.WriteDebugToLog)
                {
                    WriteToLog("Writting procedure completed sucessfully [Line 184]");
                }
            }
            catch (Exception ex)
            {
                if (_settings.WriteDebugToLog)
                {
                    WriteToLog("Exception on WriteToFile [Line 186]. See next entry for details.");
                    WriteToLog(GenerateExceptionMessages(ex, 186));
                }
            }
        }

        private void GenerateSafetyCode(List<string> inputParameters)
        {
            if (inputParameters.Count != 10)
            {
                WriteToFile(new[] { "FALSE", "Input file parameter count missmatch." });
                throw new MyExitException();
            }

            RacunType invoice;
            string[] certificateParams;
            Util.ParseSafetyKeyInput(inputParameters, out invoice, out certificateParams);

            if (certificateParams == null || !(certificateParams.Length == 1 || certificateParams.Length == 2))
            {
                WriteToFile(new[] { "FALSE", "Certificate definition error" });
                throw new MyExitException();
            }

            WriteToFile(new[] { "TRUE", (certificateParams.Length == 1 ? Fiskalizator.GenerateSafetyCode(certificateParams[0], invoice.Oib, invoice.DatVrijeme, invoice.BrRac.BrOznRac, invoice.BrRac.OznPosPr, invoice.BrRac.OznNapUr, invoice.IznosUkupno)
                                                                       : Fiskalizator.GenerateSafetyCode(certificateParams[0], certificateParams[1], invoice.Oib, invoice.DatVrijeme, invoice.BrRac.BrOznRac, invoice.BrRac.OznPosPr, invoice.BrRac.OznNapUr, invoice.IznosUkupno)) });
        }
    }
}
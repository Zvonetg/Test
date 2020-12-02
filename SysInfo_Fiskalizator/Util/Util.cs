using System.Collections.Generic;
using System.IO;
using SysInfo_Fiskalizator.Model;

namespace SysInfo_Fiskalizator.Util
{
    public static class Util
    {
        public static void ParseSafetyKeyInput(List<string> parameters, out RacunType invoice, out string[] certificateParams)
        {
            invoice = new RacunType();
            invoice.BrRac = new BrojRacunaType();
            certificateParams = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                switch (i + 1)
                {
                    case 1:
                        invoice.Oib = parameters[i];
                        break;

                    case 2:
                        invoice.DatVrijeme = parameters[i];
                        break;

                    case 3:
                        invoice.BrRac.BrOznRac = parameters[i];
                        break;

                    case 4:
                        invoice.BrRac.OznPosPr = parameters[i];
                        break;

                    case 5:
                        invoice.BrRac.OznNapUr = parameters[i];
                        break;

                    case 6:
                        invoice.IznosUkupno = parameters[i];
                        break;

                    case 7:
                        certificateParams = (parameters[i] == "1" ? new string[1] : new string[2]);
                        break;

                    case 8:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 1)
                        {
                            certificateParams[0] = parameters[i];
                        }
                        break;

                    case 9:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 2)
                        {
                            certificateParams[0] = parameters[i];
                        }
                        break;

                    case 10:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 2)
                        {
                            certificateParams[1] = parameters[i];
                        }
                        break;
                }
            }
        }

        public static void ParseWorkspaceInput(List<string> parameters, out PoslovniProstorType workspace, out string[] certificateParams)
        {
            workspace = new PoslovniProstorType { AdresniPodatak = new AdresniPodatakType() };
            certificateParams = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                switch (i + 1)
                {
                    case 1:
                        workspace.Oib = parameters[i];
                        break;

                    case 2:
                        workspace.OznPoslProstora = parameters[i];
                        break;

                    case 3:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            workspace.AdresniPodatak.Item = new AdresaType();
                            (workspace.AdresniPodatak.Item as AdresaType).Ulica = parameters[i];
                        }
                        break;

                    case 4:
                        if (!string.IsNullOrEmpty(parameters[i]) && workspace.AdresniPodatak.Item != null)
                        {
                            (workspace.AdresniPodatak.Item as AdresaType).KucniBroj = parameters[i];
                        }
                        break;

                    case 5:
                        if (!string.IsNullOrEmpty(parameters[i]) && workspace.AdresniPodatak.Item != null)
                        {
                            (workspace.AdresniPodatak.Item as AdresaType).KucniBrojDodatak = parameters[i];
                        }
                        break;

                    case 6:
                        if (!string.IsNullOrEmpty(parameters[i]) && workspace.AdresniPodatak.Item != null)
                        {
                            (workspace.AdresniPodatak.Item as AdresaType).BrojPoste = parameters[i];
                        }
                        break;

                    case 7:
                        if (!string.IsNullOrEmpty(parameters[i]) && workspace.AdresniPodatak.Item != null)
                        {
                            (workspace.AdresniPodatak.Item as AdresaType).Naselje = parameters[i];
                        }
                        break;

                    case 8:
                        if (!string.IsNullOrEmpty(parameters[i]) && workspace.AdresniPodatak.Item != null)
                        {
                            (workspace.AdresniPodatak.Item as AdresaType).Opcina = parameters[i];
                        }
                        break;

                    case 9:
                        if (!string.IsNullOrEmpty(parameters[i]) && workspace.AdresniPodatak.Item == null)
                        {
                            workspace.AdresniPodatak.Item = parameters[i];
                        }
                        break;

                    case 10:
                        workspace.RadnoVrijeme = parameters[i];
                        break;

                    case 11:
                        workspace.DatumPocetkaPrimjene = parameters[i];
                        break;

                    case 12:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            workspace.OznakaZatvaranja = OznakaZatvaranjaType.Z;
                            workspace.OznakaZatvaranjaSpecified = true;
                        }
                        break;

                    case 13:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            workspace.SpecNamj = parameters[i];
                        }
                        break;

                    case 14:
                        certificateParams = (parameters[i] == "1" ? new string[1] : new string[2]);
                        break;

                    case 15:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 1)
                        {
                            certificateParams[0] = parameters[i];
                        }
                        break;

                    case 16:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 2)
                        {
                            certificateParams[0] = parameters[i];
                        }
                        break;

                    case 17:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 2)
                        {
                            certificateParams[1] = parameters[i];
                        }
                        break;
                }
            }
        }

        public static void ParseInvoiceInput(List<string> parameters, out RacunType invoice, out string[] certificateParams)
        {
            invoice = new RacunType();
            invoice.BrRac = new BrojRacunaType();
            certificateParams = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                switch (i + 1)
                {
                    case 1:
                        invoice.Oib = parameters[i];
                        break;

                    case 2:
                        invoice.USustPdv = (parameters[i] == "1");
                        break;

                    case 3:
                        invoice.DatVrijeme = parameters[i];
                        break;

                    case 4:
                        invoice.OznSlijed = (parameters[i] == "N" ? OznakaSlijednostiType.N : OznakaSlijednostiType.P);
                        break;

                    case 5:
                        invoice.BrRac.BrOznRac = parameters[i];
                        break;

                    case 6:
                        invoice.BrRac.OznPosPr = parameters[i];
                        break;

                    case 7:
                        invoice.BrRac.OznNapUr = parameters[i];
                        break;

                    case 8:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            invoice.Pdv = new List<PorezType>();
                            string[] fragments = parameters[i].Split(';');
                            for (int j = 0; j + 2 < fragments.Length; j += 3)
                            {
                                PorezType taxTupple = new PorezType();
                                taxTupple.Stopa = Settings.CustomParseDecimal(fragments[j]);
                                taxTupple.Osnovica = Settings.CustomParseDecimal(fragments[j + 1]);
                                taxTupple.Iznos = fragments[j + 2];
                                invoice.Pdv.Add(taxTupple);
                            }
                        }
                        break;

                    case 9:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            invoice.Pnp = new List<PorezType>();
                            string[] fragments = parameters[i].Split(';');
                            for (int j = 0; j + 2 < fragments.Length; j += 3)
                            {
                                PorezType taxTupple = new PorezType();
                                taxTupple.Stopa = Settings.CustomParseDecimal(fragments[j]);
                                taxTupple.Osnovica = Settings.CustomParseDecimal(fragments[j + 1]);
                                taxTupple.Iznos = fragments[j + 2];
                                invoice.Pnp.Add(taxTupple);
                            }
                        }
                        break;

                    case 10:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            invoice.OstaliPor = new List<PorezOstaloType>();
                            string[] fragments = parameters[i].Split(';');
                            for (int j = 0; j + 3 < fragments.Length; j += 4)
                            {
                                PorezOstaloType taxTupple = new PorezOstaloType();
                                taxTupple.Naziv = fragments[j];
                                taxTupple.Stopa = Settings.CustomParseDecimal(fragments[j + 1]);
                                taxTupple.Osnovica = Settings.CustomParseDecimal(fragments[j + 2]);
                                taxTupple.Iznos = fragments[j + 3];
                                invoice.OstaliPor.Add(taxTupple);
                            }
                        }
                        break;

                    case 11:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            invoice.IznosOslobPdvSpecified = true;
                            invoice.IznosOslobPdv = parameters[i];
                        }
                        break;

                    case 12:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            invoice.IznosMarzaSpecified = true;
                            invoice.IznosMarza = parameters[i];
                        }
                        break;

                    case 13:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            invoice.IznosNePodlOporSpecified = true;
                            invoice.IznosNePodlOpor = parameters[i];
                        }
                        break;

                    case 14:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            invoice.Naknade = new List<NaknadaType>();
                            string[] fragments = parameters[i].Split(';');
                            for (int j = 0; j + 1 < fragments.Length; j += 2)
                            {
                                NaknadaType costTupple = new NaknadaType();
                                costTupple.NazivN = fragments[j];
                                costTupple.IznosN = fragments[j + 1];
                                invoice.Naknade.Add(costTupple);
                            }
                        }
                        break;

                    case 15:
                        invoice.IznosUkupno = parameters[i];
                        break;

                    case 16:
                        switch (parameters[i])
                        {
                            case "G":
                                invoice.NacinPlac = NacinPlacanjaType.G;
                                break;

                            case "K":
                                invoice.NacinPlac = NacinPlacanjaType.K;
                                break;

                            case "C":
                                invoice.NacinPlac = NacinPlacanjaType.C;
                                break;

                            case "T":
                                invoice.NacinPlac = NacinPlacanjaType.T;
                                break;

                            case "O":
                                invoice.NacinPlac = NacinPlacanjaType.O;
                                break;
                        }
                        break;

                    case 17:
                        invoice.OibOper = parameters[i];
                        break;

                    case 18:
                        invoice.NakDost = (parameters[i] == "1");
                        break;

                    case 19:
                        if (!string.IsNullOrEmpty(parameters[i]))
                        {
                            invoice.ParagonBrRac = parameters[i];
                        }
                        break;

                    case 20:
                        certificateParams = (parameters[i] == "1" ? new string[1] : new string[2]);
                        break;

                    case 21:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 1)
                        {
                            certificateParams[0] = parameters[i];
                        }
                        break;

                    case 22:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 2)
                        {
                            certificateParams[0] = parameters[i];
                        }
                        break;

                    case 23:
                        if (!string.IsNullOrEmpty(parameters[i]) && certificateParams.Length == 2)
                        {
                            certificateParams[1] = parameters[i];
                        }
                        break;
                }
            }
        }

        public static void ParseArguments(string[] args, Settings _settings)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "ECHO":
                        _settings.Operation = OperationType.ECHO;
                        break;

                    case "WORKSPACE":
                        _settings.Operation = OperationType.WORKSPACE;
                        break;

                    case "INVOICE":
                        _settings.Operation = OperationType.INVOICE;
                        break;

                    case "CHECK":
                        _settings.Operation = OperationType.CHECK;
                        break;

                    case "SAFETYCODE":
                        _settings.Operation = OperationType.SAFETYCODE;
                        break;

                    case "/T":
                        i++;
                        int.TryParse(args[i], out _settings.TimeOut);
                        break;

                    case "/O":
                        i++;
                        _settings.OutputFile = args[i];
                        break;

                    case "/S":
                        i++;
                        _settings.CisWebServiceUrl = args[i];
                        break;

                    case "/L":
                        _settings.WriteDebugToLog = true;
                        break;

                    case "/IO":
                        i++;
                        _settings.XMLOutputFolder = args[i];
                        break;

                    case "/I":
                        i++;
                        _settings.InputFile = args[i];
                        break;

                    default:
                        using (TextWriter outputWriter = new StreamWriter(_settings.OutputFile))
                        {
                            outputWriter.WriteLine("FALSE");
                            outputWriter.Write("Argument mismatch.");
                            outputWriter.Flush();
                        }
                        return;
                }
            }
        }

        public static string ReadFromFile(string inputFile)
        {
            string input;
            using (TextReader inputReader = new StreamReader(inputFile))
            {
                input = inputReader.ReadToEnd().Trim(new[] { '\r', '\n', ' ' });
            }
            return input;
        }
    }
}
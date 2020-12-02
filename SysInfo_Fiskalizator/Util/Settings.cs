namespace SysInfo_Fiskalizator.Util
{
    public class Settings
    {
        public OperationType Operation;
        public string CisWebServiceUrl;
        public int TimeOut;
        public string InputFile;
        public string OutputFile;
        public string XMLOutputFolder;
        public bool WriteDebugToLog;

        public Settings(bool callFromService)
        {
            if (callFromService)
            {
                Operation = OperationType.UNKNOWN;
                CisWebServiceUrl = @"https://cis.porezna-uprava.hr:8449/FiskalizacijaService";
                TimeOut = 100000;
                InputFile = @"C:\input.in";
                OutputFile = @"C:\output.out";
                XMLOutputFolder = null;
                WriteDebugToLog = false;
            }
            else
            {
                Operation = OperationType.UNKNOWN;
                TimeOut = 100000;
                InputFile = @".\input.txt";
                OutputFile = @".\output.txt";
            }
        }

        /// <summary>
        /// Parsira string sa zapisom decimalnog broja koji ne koristi separatore za tisuće i koji koristi decimalnu točku
        /// </summary>
        /// <param name="decimalText">Tekstualni zapis broja (NAPOMENA: ne parsira dobro cijele brojeve koji imaju separatore tisuća)</param>
        /// <returns>Decimalni zapis broja</returns>
        public static decimal CustomParseDecimal(string decimalText)
        {
            decimal result = 0M;

            //Zamjena svih zareza točkom
            decimalText = decimalText.Replace(',', '.');

            //Uklanjanje svih točaka osim zadnje (koja se smatra decimalnom)
            if (decimalText.Contains("."))
            {
                while (decimalText.IndexOf('.') != decimalText.LastIndexOf('.'))
                {
                    decimalText = decimalText.Remove(decimalText.IndexOf('.'), 1);
                }
            }

            //Cijeli dio broja se sigurno može parsirati kao long
            result += long.Parse(decimalText.Split('.')[0]);

            //Parsiranje decimalnog dijela
            //Počinje se sa prvom decimalom - koeficijent 0.1
            decimal coeficient = (result < 0 ? -1 : 1) * 0.1M;
            string decimalPart = decimalText.Split('.')[1];
            for (int i = 0; i < decimalPart.Length; i++, coeficient /= 10)
            {
                result += (decimalPart[i] - '0') * coeficient;
            }

            return result;
        }
    }
}
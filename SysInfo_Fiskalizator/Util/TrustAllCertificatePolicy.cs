﻿using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SysInfo_Fiskalizator.Util
{
    public class TrustAllCertificatePolicy : ICertificatePolicy
    {
        public TrustAllCertificatePolicy() { }

        public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem)
        {
            return true;
        }
    }
}
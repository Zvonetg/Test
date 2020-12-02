using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace SysInfo_ServisFiskalizacije
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] { new SysInfo_ServisFiskalizacije() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
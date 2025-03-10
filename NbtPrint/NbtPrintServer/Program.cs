﻿
using System.ServiceProcess;

namespace NbtPrintServer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {

            bool debug = false;

            if( debug )
            {
                Service1 myService = new Service1();
                myService.OnDebug();
                System.Threading.Thread.Sleep( System.Threading.Timeout.Infinite );
            }
            else
            {

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Service1()
                };
                ServiceBase.Run( ServicesToRun );
            }

        }
    }
}

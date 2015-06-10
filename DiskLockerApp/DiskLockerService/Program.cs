using DiskLockerService.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiskLockerService
{
    static class Program
    {
        static void Main( string[] args )
        {
            try
            {
                if ( System.Environment.UserInteractive )
                {
#if DEBUG
                ( new Service() ).RunAsConsole();
                Thread.Sleep( Timeout.Infinite );
#else
                    string parameter = string.Concat( args );

                    switch ( parameter )
                    {
                        case "--install":
                            ManagedInstallerClass.InstallHelper( new string[] { Assembly.GetExecutingAssembly().Location } );
                            break;
                        case "--uninstall":
                            ManagedInstallerClass.InstallHelper( new string[] { "/u", Assembly.GetExecutingAssembly().Location } );
                            break;
                    }
#endif
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] 
            { 
                new Service() 
            };
                    ServiceBase.Run( ServicesToRun );
                }
            }
            catch(Exception e)
            {
                Service.HandleException( e );
            }
        }
    }
}

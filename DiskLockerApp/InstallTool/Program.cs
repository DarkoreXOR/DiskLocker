using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class Program
    {


        static void Main( string[] args )
        {
            string parameter = string.Concat( args );

            switch ( parameter )
            {
                case "--install":
                    ( new Installer() ).Install();
                    break;
                case "--uninstall":
                    ( new Installer() ).Uninstall();
                    break;
            }

           
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerService.Data
{
    public static class DataUtils
    {
        private static string GetExeFile()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        public static string GetExeFilePath()
        {
            return Path.GetDirectoryName( GetExeFile() ) + Path.DirectorySeparatorChar;
        }

        private static string GetDatabaseFile()
        {
            return String.Format( "{0}data.dat", GetExeFilePath() );
        }

        public static string GetConnectionString()
        {
            return String.Format( "Data Source={0};Version=3;", GetDatabaseFile() );
        }


    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    public class Installer
    {
        Version win7version = new Version( 6, 1 );
        Version win8version = new Version( 6, 2 );
        Version win81version = new Version( 6, 3 );

        public enum WinVersion
        {
            Win7,
            Win8,
            Win81
        }

        public Installer()
        {
            this.Initialize();
        }

        private void Initialize()
        {

        }

        private bool IsCurrentVersion( Version version )
        {
            if ( Environment.OSVersion.Platform == PlatformID.Win32NT )
            {
                if ( version.Major == Environment.OSVersion.Version.Major &&
                    version.Minor == Environment.OSVersion.Version.Minor )
                {
                    return true;
                }
            }
            return false;
        }

        private string GetExeFile()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        public string GetExeFilePath()
        {
            return Path.GetDirectoryName( GetExeFile() ) + Path.DirectorySeparatorChar;
        }

        public string GetRelativeInfPath( WinVersion ver, bool x64 )
        {
            if ( ver == WinVersion.Win7 )
            {
                if ( x64 )
                {
                    return String.Format( "drivers{0}w7{0}x64{0}fsprot.inf", Path.DirectorySeparatorChar );
                }
                else
                {
                    return String.Format( "drivers{0}w7{0}x86{0}fsprot.inf", Path.DirectorySeparatorChar );
                }
            }

            if ( ver == WinVersion.Win8 )
            {
                if ( x64 )
                {
                    return String.Format( "drivers{0}w8{0}x64{0}fsprot.inf", Path.DirectorySeparatorChar );
                }
                else
                {
                    return String.Format( "drivers{0}w8{0}x86{0}fsprot.inf", Path.DirectorySeparatorChar );
                }
            }

            if ( ver == WinVersion.Win81 )
            {
                if ( x64 )
                {
                    return String.Format( "drivers{0}w81{0}x64{0}fsprot.inf", Path.DirectorySeparatorChar );
                }
                else
                {
                    return String.Format( "drivers{0}w81{0}x86{0}fsprot.inf", Path.DirectorySeparatorChar );
                }
            }

            return "";
        }

        [DllImport( "Setupapi.dll", EntryPoint = "InstallHinfSectionW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode )]
        public static extern void InstallHinfSection(
            [In] IntPtr hwnd,
            [In] IntPtr ModuleHandle,
            [In, MarshalAs( UnmanagedType.LPWStr )] string CmdLineBuffer,
            int nCmdShow );

        public void InstallInf( string path )
        {
            //Process.Start( String.Format( "RUNDLL32.EXE", "SETUPAPI.DLL,InstallHinfSection DefaultInstall 132 {0}", path ) );
            InstallHinfSection( IntPtr.Zero, IntPtr.Zero, path, 0 );
        }

        public void Install()
        {
            bool x64 = Environment.Is64BitOperatingSystem;

            StringBuilder path = new StringBuilder();

            Console.WriteLine(Environment.OSVersion.ToString());

            path.AppendFormat( "{0}", this.GetExeFilePath() );

            if ( this.IsCurrentVersion( win7version ) )
            {
                Console.WriteLine( "win7" );
                path.AppendFormat( this.GetRelativeInfPath( WinVersion.Win7, x64 ) );
            }
            else if ( this.IsCurrentVersion( win8version ) )
            {
                Console.WriteLine( "win8" );
                path.AppendFormat( this.GetRelativeInfPath( WinVersion.Win8, x64 ) );
            }
            else if ( this.IsCurrentVersion( win81version ) )
            {
                Console.WriteLine( "win81" );
                path.AppendFormat( this.GetRelativeInfPath( WinVersion.Win81, x64 ) );
            }

            Console.WriteLine( path.ToString() );

            //Process.Start( String.Format( "{0}certmgr.exe", this.GetExeFilePath() ), "-add fsprot.cer -s -r localMachine ROOT" ).WaitForExit();
            //Process.Start( String.Format( "{0}certmgr.exe", this.GetExeFilePath() ), "-add fsprot.cer -s -r localMachine TRUSTEDPUBLISHER" ).WaitForExit();

            this.InstallInf( string.Format( "{0} {1}", "DefaultInstall 132", path.ToString() ) );
            Process.Start( "sc.exe", "start fsprot" );

            Process.Start( String.Format( "{0}disklockerservice.exe", this.GetExeFilePath() ), "--install" ).WaitForExit();
            Process.Start( "sc.exe", "start DiskLockerService" ).WaitForExit();


        }

        public void Uninstall()
        {
            Process.Start( "sc.exe", "stop DiskLockerService" ).WaitForExit();
            Process.Start( String.Format( "{0}disklockerservice.exe", this.GetExeFilePath() ), "--uninstall" ).WaitForExit();

            Process.Start( "sc.exe", "stop fsprot" ).WaitForExit();
            Process.Start( "sc.exe", "delete fsprot" ).WaitForExit();
        }

    }
}

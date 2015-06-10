using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tests
{
    public class DevIoCore
    {
        [DllImport( "devio.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool StartService();

        [DllImport( "devio.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool StopService();

        [DllImport( "devio.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool AddPathForProtection(
            [MarshalAs( UnmanagedType.LPWStr )] string FullPath,
            [MarshalAs( UnmanagedType.Bool )] bool WeakProtection,
            [MarshalAs( UnmanagedType.U4 )] out UInt32 UniqueKey );

        [DllImport( "devio.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool RemovePathFromProtection(
            [MarshalAs( UnmanagedType.U4 )] UInt32 UniqueKey,
            [MarshalAs( UnmanagedType.Bool )] out bool RemoveResult );

        [DllImport( "devio.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool RemoveAllPathsFromProtection();

        [DllImport( "devio.dll",
            CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.StdCall )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool SetUnloadAccess(
            [MarshalAs( UnmanagedType.Bool )] bool CanUnload );

        public static bool IoStartService()
        {
            return StartService();
        }

        public static bool IoStopService()
        {
            return StopService();
        }

        public static bool IoAddPathForProtection( string FullPath, bool WeakProtection, out UInt32 UniqueKey )
        {
            return AddPathForProtection( FullPath, WeakProtection, out UniqueKey );
        }

        public static bool IoRemovePathFromProtection( UInt32 UniqueKey, out bool RemoveResult )
        {
            return RemovePathFromProtection( UniqueKey, out RemoveResult );
        }

        public static bool IoRemoveAllPathsFromProtection()
        {
            return RemoveAllPathsFromProtection();
        }

        public static bool IoSetUnloadAccess( bool CanUnload )
        {
            return SetUnloadAccess( CanUnload );
        }

        [DllImport( "kernel32.dll" )]
        static extern uint QueryDosDevice( string lpDeviceName,
            StringBuilder lpTargetPath,
            int ucchMax );

        public static bool GetPhysicalName( string LogicalName, out string PhysicalName )
        {
            StringBuilder physicalName = new StringBuilder();
            LogicalName = LogicalName.Replace( Path.DirectorySeparatorChar.ToString(), string.Empty );
            bool result = QueryDosDevice( LogicalName, physicalName, 260 ) > 0;
            PhysicalName = physicalName.ToString();
            return result;
        }

        public static bool LogicalPathToPhysical( string LogicalPath, out string PhysicalPath )
        {
            try
            {
                string physicalDriveName;
                string logicalDriveName = Path.GetPathRoot( LogicalPath );

                if ( GetPhysicalName( logicalDriveName, out physicalDriveName ) )
                {
                    physicalDriveName += Path.DirectorySeparatorChar;
                    PhysicalPath = LogicalPath.Replace( logicalDriveName, physicalDriveName );
                    return true;
                }

                PhysicalPath = string.Empty;
                return false;
            }
            catch ( Exception )
            {
            }

            PhysicalPath = string.Empty;
            return false;
        }
    }
}

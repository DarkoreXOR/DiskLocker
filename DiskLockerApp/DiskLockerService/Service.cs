using DiskLockerIpc;
using DiskLockerIpc.Shared;
using DiskLockerIpc.Shared.Answer;
using DiskLockerIpc.Shared.Request;
using DiskLockerService.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerService
{
    public partial class Service : ServiceBase
    {
        private ServerSideChannel channel;
        private Dictionary<uint, string> paths;
        private List<string> sessions;

        public Service()
        {
            InitializeComponent();
            this.channel = new ServerSideChannel( "DiskLockerIpcChannel" );
            this.channel.OnMessageReceiveAndWaitAnswer = this.OnMessageReceiveAndWaitAnswer;
            this.paths = new Dictionary<uint, string>();
            this.sessions = new List<string>();
        }

        private object OnMessageReceiveAndWaitAnswer( uint messageId, object message )
        {
            try
            {
                MessageCodes messageCode = ( MessageCodes )messageId;

                object result = null;

                switch ( messageCode )
                {
                    case MessageCodes.AddPathForProtectionCode:
                        result = this.OnAddPathForProtection( message );
                        break;

                    case MessageCodes.RemovePathFromProtectionCode:
                        result = this.OnRemovePathFromProtection( message );
                        break;

                    case MessageCodes.GetProtectedPathsCode:
                        result = this.OnGetProtectedPaths( message );
                        break;

                    case MessageCodes.HasPasswordsCode:
                        result = this.OnHasPasswords( message );
                        break;

                    case MessageCodes.AddPasswordCode:
                        result = this.OnAddPassword( message );
                        break;

                    case MessageCodes.DeletePasswordCode:
                        result = this.OnDeletePassword( message );
                        break;

                    case MessageCodes.ChangePasswordCode:
                        result = this.OnChangePassword( message );
                        break;

                    case MessageCodes.CreateAuthSessionKeyCode:
                        result = this.OnCreateAuthSessionKey( message );
                        break;
                }
                return result;
            }
            catch ( Exception e )
            {
                HandleException( e );
            }

            return null;
        }

        private bool CheckSession( string sessionKey )
        {
            if ( sessionKey == null || sessionKey.Length <= 0 )
            {
                return false;
            }

            return this.sessions.Contains( sessionKey );
        }

        private bool IsPasswordCorrect( string password )
        {
            if ( password == null || password.Length <= 0 )
            {
                return false;
            }

            return true;
        }

        private void DeleteSession( string sessionKey )
        {
            this.sessions.Remove( sessionKey );
        }

        private object OnCreateAuthSessionKey( object message )
        {
            var packet = ( CreateAuthSessionKeyMessage )message;
            var ret = new CreateAuthSessionKeyResult();

            PasswordManager passwordManager = new PasswordManager();

            string password = HashManager.Sha256( packet.Password );

            bool result = passwordManager.CheckPassword( password );

            ret.SessionKey = Guid.NewGuid().ToString();
            this.sessions.Add( ret.SessionKey );

            ret.Value = result;
            return ret;
        }

        private object OnChangePassword( object message )
        {
            var packet = ( ChangePasswordMessage )message;
            var ret = new OperationResult();

            if ( !this.CheckSession( packet.SessionKey ) )
            {
                ret.Value = false;
                return ret;
            }

            if ( !this.IsPasswordCorrect( packet.OldPassword ) )
            {
                ret.Value = false;
                return ret;
            }

            if ( !this.IsPasswordCorrect( packet.NewPassword ) )
            {
                ret.Value = false;
                return ret;
            }

            PasswordManager passwordManager = new PasswordManager();

            string oldPassword = HashManager.Sha256( packet.OldPassword );

            bool result = passwordManager.CheckPassword( oldPassword );

            if ( result )
            {
                string newPassword = HashManager.Sha256( packet.NewPassword );
                result = passwordManager.ChangePassword( oldPassword, newPassword );
            }

            ret.Value = result;
            return ret;
        }

        private object OnDeletePassword( object message )
        {
            var packet = ( DeletePasswordMessage )message;
            var ret = new OperationResult();

            if ( !this.CheckSession( packet.SessionKey ) )
            {
                ret.Value = false;
                return ret;
            }

            PasswordManager passwordManager = new PasswordManager();

            string password = HashManager.Sha256( packet.Password );

            bool result = passwordManager.CheckPassword( password );

            if ( result )
            {
                result = passwordManager.DeletePassword( password );
                this.DeleteSession( packet.SessionKey );
            }

            ret.Value = result;
            return ret;
        }

        private object OnAddPassword( object message )
        {
            var packet = ( AddPasswordMessage )message;
            var ret = new OperationResult();

            PasswordManager passwordManager = new PasswordManager();

            bool hasPasswords = passwordManager.HasPasswords();

            if ( !this.CheckSession( packet.SessionKey ) && hasPasswords )
            {
                ret.Value = false;
                return ret;
            }

            if ( !this.IsPasswordCorrect( packet.NewPassword ) )
            {
                ret.Value = false;
                return ret;
            }

            string newPassword = HashManager.Sha256( packet.NewPassword );

            passwordManager.InsertPassword( newPassword );

            ret.Value = true;
            return ret;
        }

        private object OnHasPasswords( object message )
        {
            var packet = ( CreateAuthSessionKeyMessage )message;
            var ret = new OperationResult();

            PasswordManager passwordManager = new PasswordManager();

            ret.Value = passwordManager.HasPasswords();
            return ret;
        }

        private object OnAddPathForProtection( object message )
        {
            var packet = ( AddPathForProtectionMessage )message;
            var ret = new OperationResult();

            if ( !this.CheckSession( packet.SessionKey ) )
            {
                ret.Value = false;
                return ret;
            }

            PathManager pathManager = new PathManager();

            if ( pathManager.ExistsPath( packet.Path ) )
            {
                ret.Value = false;
                return ret;
            }

            bool result = true;

            result = pathManager.InsertPath( packet.Path, packet.WeakProtection );

            /* TODO: check correct path */

            uint uniqueKey = 0;

            if ( result )
            {
                result = this.SafeAddPathForProtection( packet.Path, packet.WeakProtection, out uniqueKey );
            }

            if ( result )
            {
                paths.Add( uniqueKey, packet.Path );
            }

            ret.Value = result;
            return ret;
        }

        private string GetPathByUniqueKey( uint uniqueKey )
        {
            var linqPath = from p in this.paths
                           where p.Key == uniqueKey
                           select p.Value;

            if ( linqPath.Count() <= 0 )
            {
                return null;
            }

            return linqPath.FirstOrDefault();
        }

        private object OnRemovePathFromProtection( object message )
        {
            var packet = ( RemovePathFromProtectionMessage )message;
            var ret = new OperationResult();

            if ( !this.CheckSession( packet.SessionKey ) )
            {
                ret.Value = false;
                return ret;
            }

            bool removeResult;

            bool result = DevIoCore.IoRemovePathFromProtection( packet.UniqueKey, out removeResult );

            if ( result && removeResult )
            {
                string path = this.GetPathByUniqueKey( packet.UniqueKey );

                if ( path != null )
                {
                    PathManager pathManager = new PathManager();
                    result = pathManager.DeletePath( path );
                }
                else
                {
                    ret.Value = false;
                    return ret;
                }
            }

            if ( result && removeResult )
            {
                result = this.paths.Remove( packet.UniqueKey );
            }

            ret.Value = result && removeResult;
            return ret;
        }

        private object OnGetProtectedPaths( object message )
        {
            var packet = ( GetProtectedPathsMessage )message;
            var ret = new GetProtectedPathsResult();

            if ( !this.CheckSession( packet.SessionKey ) )
            {
                ret.Value = false;
                return ret;
            }

            ret.Paths = new Dictionary<uint, string>();

            foreach ( var path in this.paths )
            {
                ret.Paths.Add( path.Key, path.Value );
            }

            ret.Value = true;
            return ret;
        }

        private bool SafeAddPathForProtection( string path, bool weakProtection, out uint uniqueKey )
        {
            string physicalPath;

            bool result = DevIoCore.LogicalPathToPhysical( path, out physicalPath );

            if ( !result )
            {
                uniqueKey = 0;
                return false;
            }

            return DevIoCore.IoAddPathForProtection( physicalPath, weakProtection, out uniqueKey );
        }

        private bool ReloadPaths()
        {
            PathManager pathManager = new PathManager();

            var list = pathManager.GetAllPaths();

            bool result = DevIoCore.IoRemoveAllPathsFromProtection();

            this.paths.Clear();
            this.sessions.Clear();

            if ( result )
            {
                foreach ( var path in list )
                {
                    uint uniqueKey;

                    result = this.SafeAddPathForProtection( path.Key, path.Value, out uniqueKey );

                    if ( result )
                    {
                        this.paths.Add( uniqueKey, path.Key );
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return result;
        }

        protected override void OnStart( string[] args )
        {
            try
            {
                DevIoCore.IoStartService();

                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                this.channel.Open();

                this.ReloadPaths();

            }
            catch ( Exception e )
            {
                HandleException( e );
            }

            /* todo protect driver file */
        }

        protected override void OnStop()
        {
            this.channel.Close();
        }


        public void RunAsConsole()
        {
            this.OnStart( null );
        }

        public static void HandleException( Exception e )
        {
            string path = String.Format( "{0}service.output-{1}.log", DataUtils.GetExeFilePath(), DateTime.Now.ToString( "yyyy-MM-dd" ) );

            using ( var fs = new FileStream( path, FileMode.Create ) )
            {
                using ( var sw = new StreamWriter( fs ) )
                {
                    sw.WriteLine( e.Message );
                    sw.WriteLine( e.StackTrace );
                }
            }
        }

        public static void UnhandledExceptionHandler( object sender, UnhandledExceptionEventArgs args )
        {
            HandleException( ( Exception )args.ExceptionObject );
        }

    }
}


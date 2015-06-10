using DiskLockerIpc;
using DiskLockerIpc.Shared;
using DiskLockerIpc.Shared.Answer;
using DiskLockerIpc.Shared.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerApp
{
    public class IpcManager
    {
        public bool AddPathForProtection( string sessionKey, string Path, bool WeakProtection )
        {
            ClientSideChannel channel = new ClientSideChannel( "DiskLockerIpcChannel" );

            var message = new AddPathForProtectionMessage();

            message.SessionKey = sessionKey;
            message.Path = Path;
            message.WeakProtection = WeakProtection;

            var result = ( OperationResult )channel.SendMessageAndWaitAnswer( ( uint )MessageCodes.AddPathForProtectionCode, message );

            return result.Value;
        }

        public bool RemovePathFromProtection( string sessionKey, uint UniqueKey )
        {
            ClientSideChannel channel = new ClientSideChannel( "DiskLockerIpcChannel" );

            var message = new RemovePathFromProtectionMessage();

            message.SessionKey = sessionKey;
            message.UniqueKey = UniqueKey;

            var result = ( OperationResult )channel.SendMessageAndWaitAnswer( ( uint )MessageCodes.RemovePathFromProtectionCode, message );

            return result.Value;
        }

        public GetProtectedPathsResult GetProtectedPaths(string sessionKey)
        {
            ClientSideChannel channel = new ClientSideChannel( "DiskLockerIpcChannel" );

            var message = new GetProtectedPathsMessage();

            message.SessionKey = sessionKey;

            var result = ( GetProtectedPathsResult )channel.SendMessageAndWaitAnswer( ( uint )MessageCodes.GetProtectedPathsCode, message );

            return result;
        }

        public bool HasPasswords()
        {
            ClientSideChannel channel = new ClientSideChannel( "DiskLockerIpcChannel" );

            var result = ( OperationResult )channel.SendMessageAndWaitAnswer( ( uint )MessageCodes.HasPasswordsCode, null );

            return result.Value;
        }

        public bool AddPassword(string sessionKey, string newPassword)
        {
            ClientSideChannel channel = new ClientSideChannel( "DiskLockerIpcChannel" );

            var message = new AddPasswordMessage();

            message.SessionKey = sessionKey;
            message.NewPassword = newPassword;

            var result = ( OperationResult )channel.SendMessageAndWaitAnswer( ( uint )MessageCodes.AddPasswordCode, message );

            return result.Value;
        }

        public bool DeletePassword( string sessionKey, string password )
        {
            ClientSideChannel channel = new ClientSideChannel( "DiskLockerIpcChannel" );

            var message = new DeletePasswordMessage();

            message.SessionKey = sessionKey;
            message.Password = password;

            var result = ( OperationResult )channel.SendMessageAndWaitAnswer( ( uint )MessageCodes.DeletePasswordCode, message );

            return result.Value;
        }

        public bool ChangePassword( string sessionKey, string oldPassword, string newPassword )
        {
            ClientSideChannel channel = new ClientSideChannel( "DiskLockerIpcChannel" );

            var message = new ChangePasswordMessage();

            message.SessionKey = sessionKey;
            message.OldPassword = oldPassword;
            message.NewPassword = newPassword;

            var result = ( OperationResult )channel.SendMessageAndWaitAnswer( ( uint )MessageCodes.ChangePasswordCode, message );

            return result.Value;
        }

        public bool CreateAuthSessionKey( string password, out string sessionKey  )
        {
            ClientSideChannel channel = new ClientSideChannel( "DiskLockerIpcChannel" );

            var message = new CreateAuthSessionKeyMessage();

            message.Password = password;

            var result = ( CreateAuthSessionKeyResult )channel.SendMessageAndWaitAnswer( ( uint )MessageCodes.CreateAuthSessionKeyCode, message );

            if ( result.Value )
            {
                sessionKey = result.SessionKey;
            }
            else
            {
                sessionKey = string.Empty;
            }

            return result.Value;
        }


    }
}

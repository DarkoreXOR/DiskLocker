using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Shared
{
    public enum MessageCodes : uint
    {
        AddPathForProtectionCode,
        RemovePathFromProtectionCode,
        GetProtectedPathsCode,
        HasPasswordsCode,
        AddPasswordCode,
        DeletePasswordCode,
        ChangePasswordCode,
        CreateAuthSessionKeyCode
    }
}

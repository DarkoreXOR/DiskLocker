using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Shared.Request
{
    [Serializable]
    public class ChangePasswordMessage
    {
        public string SessionKey { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Shared.Request
{
    [Serializable]
    public class AddPathForProtectionMessage
    {
        public string SessionKey { get; set; }
        public string Path { get; set; }
        public bool WeakProtection { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Shared.Request
{
    [Serializable]
    public class DeletePasswordMessage
    {
        public string SessionKey { get; set; }

        public string Password { get; set; }
    }
}

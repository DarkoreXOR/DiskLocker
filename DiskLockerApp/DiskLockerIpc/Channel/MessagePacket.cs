using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Channel
{
    [Serializable]
    public class MessagePacket
    {
        public uint messageId { get; set; }

        public String message { get; set; }
    }
}

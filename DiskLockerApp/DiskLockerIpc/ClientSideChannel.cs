using DiskLockerIpc.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc
{
    public class ClientSideChannel
    {
        private string channelAddress;
        private OutChannel outChannel;

        public ClientSideChannel( string channelAddress )
        {
            this.channelAddress = "net.pipe://localhost/" + channelAddress;
            this.InitializeChannel();
        }

        public void InitializeChannel()
        {
            this.outChannel = new OutChannel( this.channelAddress );
        }

        public Object SendMessageAndWaitAnswer( uint messageId, Object message )
        {
            return this.outChannel.SendMessageAndWaitAnswer( messageId, message );
        }
    }
}

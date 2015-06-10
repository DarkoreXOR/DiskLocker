using DiskLockerIpc.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc
{
    public class ServerSideChannel
    {
        private string channelAddress;

        private InChannel inChannel;

        public ServerSideChannel( string channelAddress )
        {
            this.channelAddress = "net.pipe://localhost/" + channelAddress;
            this.InitializeChannel();
            this.OnMessageReceiveAndWaitAnswer = null;
        }

        private void InitializeChannel()
        {
            this.inChannel = new InChannel( this.channelAddress );
        }

        public void Open()
        {
            this.inChannel.Open();
        }

        public void Close()
        {
            this.inChannel.Close();
        }

        public ChannelHandler.MessageReceiveAndWaitAnswerEvent OnMessageReceiveAndWaitAnswer
        {
            get
            {
                return this.inChannel.OnMessageReceiveAndWaitAnserEvent;
            }

            set
            {
                this.inChannel.OnMessageReceiveAndWaitAnserEvent += value;
            }
        }

    }
}

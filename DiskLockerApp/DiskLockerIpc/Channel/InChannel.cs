using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Channel
{
    public class InChannel
    {
        private string channelAddress;
        private ServiceHost serviceHost;
        private NetNamedPipeBinding binding;
        private ChannelHandler channelHandler;

        public InChannel( string channelAddress )
        {
            this.channelAddress = channelAddress;
            this.InitializeChannel();
            this.OnMessageReceiveAndWaitAnserEvent = null;
        }

        public void InitializeChannel()
        {
            this.channelHandler = new ChannelHandler();
            this.channelHandler.OnMessageReceiveAndWaitAnserEvent = this.OnMessageReceiveAndWaitAnserEvent;

            this.serviceHost = new ServiceHost( this.channelHandler );

            this.binding = new NetNamedPipeBinding( NetNamedPipeSecurityMode.None );
            this.binding.MaxBufferSize = 2147483647;
            this.binding.MaxReceivedMessageSize = 2147483647;

            this.serviceHost.AddServiceEndpoint( typeof( IChannel ), this.binding, this.channelAddress );
        }

        public void Open()
        {
            this.serviceHost.Open();
        }

        public void Close()
        {
            this.serviceHost.Close();
        }

        public ChannelHandler.MessageReceiveAndWaitAnswerEvent OnMessageReceiveAndWaitAnserEvent
        {
            get
            {
                return this.channelHandler.OnMessageReceiveAndWaitAnserEvent;
            }

            set
            {
                this.channelHandler.OnMessageReceiveAndWaitAnserEvent += value;
            }
        }

    }
}

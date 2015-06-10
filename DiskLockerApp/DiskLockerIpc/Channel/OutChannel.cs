using DiskLockerIpc.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Channel
{
    public class OutChannel
    {
        private string channelAddress;
        private NetNamedPipeBinding binding;
        private EndpointAddress ep;
        private IChannel channel;

        public OutChannel( string channelAddress )
        {
            this.channelAddress = channelAddress;
            this.InitializeChannel();
        }

        public void InitializeChannel()
        {
            this.binding = new NetNamedPipeBinding( NetNamedPipeSecurityMode.None );
            this.binding.MaxBufferSize = 2147483647;
            this.binding.MaxReceivedMessageSize = 2147483647;
            this.ep = new EndpointAddress( this.channelAddress );
            this.CreateChannel();
        }

        public void CreateChannel()
        {
            this.channel = ChannelFactory<IChannel>.CreateChannel( binding, ep );
        }

        public Object SendMessageAndWaitAnswer( uint messageId, Object message )
        {
            MessagePacket packet = new MessagePacket();
            String serializedMessage = "";

            packet.messageId = messageId;

            if ( message != null )
            {
                packet.message = SerializationHelper.Serialize( message );
            }

            serializedMessage = SerializationHelper.Serialize( packet );

            String serializedResult = ( String )this.channel.SendMessageAndWaitAnswer( serializedMessage );

            return SerializationHelper.Deserialize<Object>( serializedResult );
        }
    }
}

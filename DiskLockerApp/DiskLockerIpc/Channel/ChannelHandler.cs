using DiskLockerIpc.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Channel
{
    [ServiceBehavior( InstanceContextMode = InstanceContextMode.Single )]
    public class ChannelHandler : IChannel
    {
        public delegate Object MessageReceiveAndWaitAnswerEvent( uint messageId, Object message );

        public MessageReceiveAndWaitAnswerEvent OnMessageReceiveAndWaitAnserEvent { get; set; }

        public ChannelHandler()
        {
            OnMessageReceiveAndWaitAnserEvent = null;
        }

        public Object SendMessageAndWaitAnswer( Object message )
        {
            if ( OnMessageReceiveAndWaitAnserEvent != null )
            {
                String serializedMessage = ( String )message;

                MessagePacket packet = SerializationHelper.Deserialize<MessagePacket>( serializedMessage );

                Object messageObject = null;

                if ( packet.message != null )
                {
                    messageObject = SerializationHelper.Deserialize<Object>( packet.message );
                }

                Object result = OnMessageReceiveAndWaitAnserEvent( packet.messageId, messageObject );

                return SerializationHelper.Serialize( result );
            }

            return null;
        }
    }
}

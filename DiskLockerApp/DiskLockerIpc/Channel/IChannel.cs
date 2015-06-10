using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DiskLockerIpc.Channel
{
    [ServiceContract]
    public interface IChannel
    {
        [OperationContract( IsOneWay = false )]
        Object SendMessageAndWaitAnswer( Object message );
    }
}

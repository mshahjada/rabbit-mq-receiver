using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RMQ.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {

            //ReceiverWorker.Receive();
            //ReceiverExchange.Receive();
            //ReceiverRouting.Receive(args);
            RpcServer.Receive();
        }
    }
}

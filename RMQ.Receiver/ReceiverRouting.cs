using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RMQ.Receiver
{
    public static class ReceiverRouting
    {
        public static void Receive(string[] args)
        {
            using (var connection = ConnectionBuilder.ConnectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("log.direct", type: ExchangeType.Direct);

                var dqueue = channel.QueueDeclare().QueueName;


                foreach(var item in args)
                {
                    channel.QueueBind(queue: dqueue, exchange: "log.direct", routingKey: item);
                }

                Console.WriteLine("--------------Waiting ------------------");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    if(ea.RoutingKey == "warning")
                    {
                        Console.WriteLine("Received {0}:  {1}", ea.RoutingKey, message);
                        Console.WriteLine("Analysing--------");
                        Thread.Sleep(10000);
                        Console.WriteLine("Finished");
                    }
                    else
                    {
                        Console.WriteLine("Received {0}:  {1}", ea.RoutingKey, message);
                    }
                    

                    //int dots = message.Split('.').Length - 1;

                    // Note: it is possible to access the channel via
                    //       ((EventingBasicConsumer)sender).Model here
                    //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: dqueue,
                                 autoAck: true,
                                 consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}

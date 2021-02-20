using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMQ.Receiver
{
    public static class ReceiverExchange
    {
        public static void Receive()
        {
            using (var connection = ConnectionBuilder.ConnectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("log", type: ExchangeType.Fanout);

                var dqueue = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: dqueue, exchange: "log", routingKey: "");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received {0}", message);

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

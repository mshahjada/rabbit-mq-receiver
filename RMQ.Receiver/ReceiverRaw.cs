using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMQ.Receiver
{
    public static class ReceiverRaw
    {
        public static void Receive()
        {
            using (var connection = ConnectionBuilder.ConnectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                            "first_q",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);


                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Message Arrived: {0}", message);
                };

                channel.BasicConsume(queue: "first_q",
                            autoAck: true,
                            consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}

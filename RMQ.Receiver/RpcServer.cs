using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMQ.Receiver
{
    public static class RpcServer
    {
        private static string Q_Name = "rpc_q";
        public static void Receive()
        {
            using (var connection = ConnectionBuilder.ConnectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: Q_Name,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine("Waiting------");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received => {0}", message);

                    string result = "n/a";

                    try
                    {
                        result = CheckPalindrome(message) ? "Yes" : "No";
                        throw new Exception("froce");
                    }
                    catch(Exception e)
                    {
                        result = e.Message;
                    }
                    finally
                    {
                        channel.BasicPublish(
                                "",
                                ea.BasicProperties.ReplyTo,
                                ea.BasicProperties,
                                Encoding.UTF8.GetBytes(result));
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }

                    
                };

                channel.BasicConsume(queue: Q_Name,
                                 autoAck: false,
                                 consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();

            }
        }
    
        private static bool CheckPalindrome(string val)
        {
            char[] charArr = val.ToCharArray();
            Array.Reverse(charArr);
            return val == new string(charArr);
        }
    }
}

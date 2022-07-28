using System;
using System.Text;
using System.Text.RegularExpressions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace ServiceB 
{
    public class Program
    {
        private const string QUEUE_NAME = "wongaQueue";
        private const string HOST_NAME = "localhost";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = HOST_NAME };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(QUEUE_NAME, false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (m, a) =>
            {
                var body = a.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    Console.WriteLine($"Hello {GetMessage.GetName(message)}, I am your father!");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Invalid message: {message}");
                }
            };
            channel.BasicConsume(QUEUE_NAME, true, consumer);
            Console.ReadLine();
        }
    }
}


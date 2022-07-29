using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace ServiceA
{
    /// <summary>
    /// Used to publish to RabbitMQ Queue
    /// </summary>
    public class Publisher
    {
        private readonly string queueName;
        private readonly ConnectionFactory connectionFactory = null;
        private const string HOST_NAME = "localhost";

        /// <summary>
        /// Constructor for QueueuPublisher object
        /// </summary>
        /// <param name="queueName">The name of the Queue that object should publish to</param>
        public Publisher(string queueName)
        {
            this.queueName = queueName;
            connectionFactory = new ConnectionFactory() { HostName = HOST_NAME };
        }

        /// <summary>
        /// Publish a message to a queue
        /// </summary>
        /// <param name="message">The message to be published</param>
        public void PublishMessage(string message)
        {
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queueName, false, false, false, null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("", queueName, null, body);
        }
    }
}

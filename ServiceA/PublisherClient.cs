using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ServiceA
{
#nullable enable

    /// <summary>
    /// Used to publish to wongaQueue
    /// </summary>
    public class PublisherClient : IDisposable
    {
        public bool IsConnected { get; private set; }

        private IConnection? _connection;
        private IModel? _channel;
        readonly SemaphoreSlim _semaphor;

        private readonly string queueName;
        private const string HOST_NAME = "localhost";

        /// <summary>
        /// Constructor for Publisher 
        /// </summary>
        /// <param name="queueName">The name of the Queue that object should publish to</param>
        public PublisherClient(string queueName)
        {
            this.queueName = queueName;
            _semaphor = new SemaphoreSlim(1, 1);
            IsConnected = false;
        }

        /// <summary>
        /// Publish a message to wongaQueue
        /// </summary>
        /// <param name="message">The message to be published</param>
        public async Task PublishMessage(string message)
        {
            try
            {
                await _semaphor.WaitAsync();
                var factory = new ConnectionFactory() { HostName = HOST_NAME };

                _connection = factory.CreateConnection();

                _connection.ConnectionShutdown += ConnectionShutDown;

                _channel = _connection.CreateModel();
                _channel.ModelShutdown += ModelShutDown;

                _channel.QueueDeclare(queueName, false, false, false, null);

                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish("", queueName, null, body);
            }

            catch (Exception ex)
            {

            }
            finally
            {
                _semaphor.Release();
            }

        }

        public void Dispose()
        {
            Disconnect();
        }

        private void Disconnect()
        {
            IsConnected = false;

            if (_channel != null)
                _channel.ModelShutdown -= ModelShutDown;

            if (_connection != null)
                _connection.ConnectionShutdown -= ConnectionShutDown;

            _channel?.Dispose();
            _connection?.Close();
        }

        private void ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Disconnect();
        }

        private void ModelShutDown(object sender, ShutdownEventArgs e)
        {
            Disconnect();
        }
    }
}

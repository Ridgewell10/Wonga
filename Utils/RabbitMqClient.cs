using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Security.Authentication;
using System.Text;
using Utils;

namespace ServiceA
{
    public class RabbitMqClient : IDisposable
    {
        private readonly bool _bodyIsZipped;
        private readonly ILogger _logger;
        private IConnection? _connection;
        private Action<object>? _callBack;
        private IModel? _channel;
        private EventingBasicConsumer? _consumer;
        private ConnectionFactory? _connectionFactory;
        private readonly string _name;
        private string? _queueName;

        public RabbitMqClient(string host, string userName, string password, string name, ILogger logger,
        string? virtualHost = null, int? port = null, bool useSsl = true, string? certPath = null, bool bodyIsZipped = true)
        {
            _name = name;
            _bodyIsZipped = bodyIsZipped;
            _logger = logger;
            CreateConnectionFactory(userName, password, host, virtualHost, port, useSsl, certPath);
        }

        private void CreateConnectionFactory(string userName, string password, string host, string? virtualHost, int? port, bool useSsl, string? certPath)
        {
            _logger.LogTrace("Starting MQ Feed Host:{host}, Port:{port}, VHost:{virtualHost}", host, port, virtualHost);
            _connectionFactory = new ConnectionFactory
            {
                HostName = host,
                UserName = userName,
                Password = password,
                VirtualHost = virtualHost ?? ConnectionFactory.DefaultVHost,
                Port = port ?? AmqpTcpEndpoint.UseDefaultPort,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(45),
            };

            if (useSsl)
            {
                _connectionFactory.Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = host,
                    Version = SslProtocols.Tls12,
                    CertPath = certPath
                };
            }
        }

        public void LogOut()
        {
            if (_connection == null)
            {
                return;
            }

            _logger.LogTrace("{Name}: Disconnecting from queue {queue}", _name, _queueName);

            Reset();
        }

        private void Reset()
        {

            _queueName = null;

            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch
            {
                //silent Fail
            }

            _channel?.Dispose();
            _connection?.Dispose();
            if (_consumer != null)
            {
                _consumer.Received -= OnDataReceived;
            }

            _consumer = null;
            _channel = null;
            _connection = null;
        }

        public void SubscribeToQueue(Action<object> callback, string queueName)
        {
            if (_queueName != null)
            {
                throw new Exception($"Client is already subscribed to queue {_queueName}");
            }

            try
            {
                _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
                _callBack = callback ?? throw new ArgumentNullException(nameof(callback));

                _logger.LogTrace("{Name}: Subscribing to queue {QueueName}", _name, queueName);
                _connection = _connectionFactory!.CreateConnection();

                _channel = _connection.CreateModel();
                _channel.BasicQos(0, 50, false);

                _consumer = new EventingBasicConsumer(_channel);
                _consumer.Received += OnDataReceived;

                _channel.BasicConsume(queueName, false, _consumer);
            }
            catch
            {
                Reset();
                throw;
            }
        }

        private void OnDataReceived(object sender, BasicDeliverEventArgs ea)
        {
            if (ea!.Body.Length > 0)
            {

                var isZipped = _bodyIsZipped || ea.BasicProperties.ContentEncoding == "gzip";
                var body = ea.Body;

                var feedData = isZipped
                    ? body.Decompress()
                    : Encoding.UTF8.GetString(body.ToArray());

                _callBack!.Invoke(feedData);
            }

            _channel!.BasicAck(ea.DeliveryTag, false);
        }

        public void Dispose()
        {
            LogOut();
        }

    }
}

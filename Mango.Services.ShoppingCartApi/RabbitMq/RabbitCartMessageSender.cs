using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Mango.Services.ShoppingCartApi.RabbitMq
{
    public class RabbitCartMessageSender : IRabbitCartMessageSender
    {
        private readonly string hostName;
        private readonly string userName;
        private readonly string password;

        private IConnection connection;

        public RabbitCartMessageSender(string hostName, string userName, string password)
        {
            this.hostName = hostName;
            this.userName = userName;
            this.password = password;
        }

        public void SendMessage(BaseMessage message, string queueName)
        {
            // -- (1) -- Create RabbitMQ connection first: -------------
            if (ConnectionExists()) //<-Create only one connection and re-use it because creation connection is expensive;
            {
                // -- (2) -- Create sending channel:

                using IModel channel = this.connection.CreateModel(); //<= Model is actually channel;

                // -- (3) -- Declare queue:

                channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);

                // -- (4) -- Now prepare the message to be sent to the queue:

                string json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                // -- (5) -- Now publish message to the channel: ------------

                channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);

            }
        }

        private void CreateConnection()
        {
            try
            {
                ConnectionFactory connFactory = new ConnectionFactory
                {
                    HostName = this.hostName,
                    UserName = this.userName,
                    Password = this.password
                };
                this.connection = connFactory.CreateConnection();
            }
            catch (Exception error)
            {
                // Log error here;
            }
        }

        private bool ConnectionExists()
        {
            if (this.connection != null)
            {
                return true;
            }
            else
            {
                this.CreateConnection();
                return this.connection != null;
            }
        }
    }
}

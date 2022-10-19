using Mango.MessageBus;
using Mango.PaymentProcessor;
using Mango.PaymentProcessor.Models;
using Mango.Services.OrderApi.RabbitMq;
using Mango.Services.PaymentApi.Messages;
using Microsoft.AspNetCore.SignalR.Protocol;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Mango.Services.PaymentApi.Messaging
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IRabbitPaymentMessageSender messageSender;
        private readonly IPaymentProcessor paymentProcessor;
        private readonly IModel channel;
        private readonly IConnection connection;

        public RabbitMqConsumer(IRabbitPaymentMessageSender messageSender, IPaymentProcessor paymentProcessor)
        {
            this.messageSender = messageSender;
            this.paymentProcessor = paymentProcessor;

            // - (1) - Create RabbitMq connection Factory: -------
            ConnectionFactory connFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            // - (2) - Create connection from the factory: -----
            this.connection = connFactory.CreateConnection();

            // - (3) - Create channel to call RabbitMQ: -------
            this.channel = this.connection.CreateModel();

            // -- (4) -- Declare queue to read from: ----------------------

            this.channel.QueueDeclare(queue: "orderpaymentprocesstopic", false, false, false, arguments: null);

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested(); //< - if cancellation is needed, throws OperationCancelledException.

            // - (1) - Create event consumer for reading from the queue:
            EventingBasicConsumer consumer = new EventingBasicConsumer(this.channel);

            // - (2) - Create "Received" message handler for reading received messages from the queue: -----
            consumer.Received += (channel, eventArgs) =>
            {
                string messageBody = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                // (3) -- Deserialize body string into type which was sent to Service Bus by sender app:
                PaymentRequestMessage headerDto = JsonConvert.DeserializeObject<PaymentRequestMessage>(messageBody);

                // (4) - Handle received from RabbitMQ message: ------------------------
                this.HandleMessage(headerDto).GetAwaiter().GetResult();

                // - (5) - Now signal back to RabbitMQ that the message is received and handled: -----

                this.channel.BasicAck(eventArgs.DeliveryTag, false); //<- This will discard message on the queue side;
            };

            // - (6) - Now initiate the call to the queue to consume the message:
            this.channel.BasicConsume("checkoutmessagetopic", false, consumer);

            // - (7) - Now return "completed task" signal for the async pattern:
            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentRequestMessage headerDto)
        {
            PaymentDto dto = new PaymentDto()
            {
                CardNumber = headerDto.CardNumber,
                CVV = headerDto.CVV,
                ExpiryMonthYear = headerDto.ExpiryMonthYear,
                Name = headerDto.Name,
                OrderId = headerDto.OrderId,
                OrderTotal = headerDto.OrderTotal
            };
            bool isPaid = this.paymentProcessor.ProcessPayment(dto); // <= This is where credit card is charged and payment is done;

            // (3) -- Now since credit card is successfully charged, raise another event with SB under different
            //     -- topic and have differenct subscriptions handle it by whoever needs to know that payment is completed.
            UpdatePaymentResultMessage result = new()
            {
                Status = isPaid,
                OrderId = dto.OrderId,
                Email = headerDto.Email
            };
            try
            {
                //this.messageSender.SendMessage(result, "orderpaymentprocesstopic");
            }
            catch
            {
                throw;
            }
        }
    }
}

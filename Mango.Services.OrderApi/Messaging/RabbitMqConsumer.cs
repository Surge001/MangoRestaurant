using Mango.MessageBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.RabbitMq;
using Mango.Services.OrderApi.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Mango.Services.OrderApi.Messaging
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly OrderRepository orderRepo;
        private readonly IRabbitOrderMessageSender messageSender;
        private readonly IModel channel;
        private readonly IConnection connection;

        public RabbitMqConsumer(OrderRepository orderRepo, IRabbitOrderMessageSender messageSender)
        {
            this.orderRepo = orderRepo;
            this.messageSender = messageSender;

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

            this.channel.QueueDeclare(queue: "checkoutmessagetopic", false, false, false, arguments: null);

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
                CheckoutHeaderDto headerDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(messageBody);

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

        private async Task HandleMessage(CheckoutHeaderDto headerDto)
        { 
            // (1) -- Now received strongly typed SBus message is to be processed as desired:
            OrderHeader order = new OrderHeader()
            {
                UserId = headerDto.UserId,
                FirstName = headerDto.FirstName,
                LastName = headerDto.LastName,
                CardNumber = headerDto.CardNumber,
                CouponCode = headerDto.CouponCode,
                CVV = headerDto.CVV,
                DiscountTotal = headerDto.DiscountTotal,
                Email = headerDto.Email,
                ExpiryMonthYear = headerDto.ExpiryMonthYear,
                OrderHeaderId = headerDto.CartHeaderId,
                OrderTime = DateTime.UtcNow,
                OrderTotal = headerDto.OrderTotal,
                PaymentStatus = false,
                PhoneNumber = headerDto.PhoneNumber,
                PickupDateTime = headerDto.PickupDateTime,
                OrderDetails = new List<OrderDetails>()
            };

            foreach (CartDetailsDto orderDetail in headerDto.CartDetails)
            {
                OrderDetails details = new()
                {
                    Count = orderDetail.Count,
                    ProductName = orderDetail.Product.Name,
                    ProductId = orderDetail.Product.ProductId,
                    Price = orderDetail.Product.Price
                };
                order.CartTotalItems += details.Count;
                order.OrderDetails.Add(details);
            }
            await this.orderRepo.AddOrder(order);

            // Now that we saved data into the database, send a message to SB for others to handle async:
            PaymentRequestMessage sbRequestMessage = new()
            {
                Name = order.FirstName + " " + order.LastName,
                CardNumber = headerDto.CardNumber,
                CVV = order.CVV,
                ExpiryMonthYear = order.ExpiryMonthYear,
                OrderId = order.OrderHeaderId,
                OrderTotal = order.OrderTotal,
                Email = order.Email
            };
            try
            {
                this.messageSender.SendMessage(sbRequestMessage, "orderpaymentprocesstopic");
            }
            catch
            {
                throw;
            }
        }
    }
}

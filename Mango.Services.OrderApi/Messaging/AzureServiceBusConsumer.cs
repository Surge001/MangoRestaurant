using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderApi.Messages;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.OrderApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository repository;
        private readonly IConfiguration configuration;
        private readonly IMessageBus messageBus;

        #region Fields for or checkout Message receiving and sending back to SB: =========================

        private readonly string serviceBusConnectionString;
        private readonly string subscriptionName;
        private readonly string messageTopic;
        private readonly string orderPaymentProcessTopic;

        #endregion

        #region Fields for Payment result status update message recieving: ===================================

        private readonly string orderUpdatePaymentResultTopic;

        #endregion

        private ServiceBusProcessor checkoutProcessor;
        private ServiceBusProcessor paymentProcessor;

        public AzureServiceBusConsumer(OrderRepository repository, IConfiguration configuration, IMessageBus messageBus)
        {
            this.repository = repository;
            this.configuration = configuration;
            this.messageBus = messageBus;
            //<= For checkout Message receiving and sending back to SB: =========================
            this.serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            this.subscriptionName = configuration.GetValue<string>("SubscriptionName");
            this.messageTopic = configuration.GetValue<string>("CheckoutMessageTopicName");
            this.orderPaymentProcessTopic = configuration.GetValue<string>("OrderPaymentProcessTopic");

            //<= For Payment result status update message recieving: ===================================
            this.orderUpdatePaymentResultTopic = configuration.GetValue<string>("OrderUpdatePaymentResultTopic");



            // -(1) -- Create Service Bus Client to listen to messages:
            var client = new ServiceBusClient(this.serviceBusConnectionString);

            //-(2) -- Create service bus processor: ----------
            this.checkoutProcessor = client.CreateProcessor(this.messageTopic, this.subscriptionName); //<-for checkout messages;
            this.paymentProcessor = client.CreateProcessor(this.orderUpdatePaymentResultTopic, this.subscriptionName); //<- for payment messages;
        }

        public async Task Start()
        {
            // <== Checkout Processor listening start: ===============
            this.checkoutProcessor.ProcessMessageAsync += this.OnCheckoutMessageReceived;
            this.checkoutProcessor.ProcessErrorAsync += this.ErrorHandler;
            await this.checkoutProcessor.StartProcessingAsync();

            // <== Payment Processor listening start: ================
            this.paymentProcessor.ProcessMessageAsync += this.OnPaymentMessageReceived;
            this.paymentProcessor.ProcessErrorAsync += this.ErrorHandler;
            await this.paymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            // <== Checkout Processor listening start: ===============
            this.checkoutProcessor.ProcessMessageAsync -= this.OnCheckoutMessageReceived;
            this.checkoutProcessor.ProcessErrorAsync -= this.ErrorHandler;
            await this.checkoutProcessor.StopProcessingAsync();
            await this.checkoutProcessor.DisposeAsync();

            // <== Payment Processor listening start: ================
            this.paymentProcessor.ProcessMessageAsync -= this.OnPaymentMessageReceived;
            this.paymentProcessor.ProcessErrorAsync -= this.ErrorHandler;
            await this.paymentProcessor.StopProcessingAsync();
            await this.paymentProcessor.DisposeAsync();
        }

        public async Task OnPaymentMessageReceived(ProcessMessageEventArgs args)
        {
            ServiceBusReceivedMessage message = args.Message;
            string body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage messageDto = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            await this.repository.UpdateOrderPaymentStatus(messageDto.OrderId, messageDto.Status);
            await args.CompleteMessageAsync(args.Message);

        }

        public async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            // (1) -- Get Message from Service Bus: --------------
            ServiceBusReceivedMessage message = args.Message; //<= This is where the actual Bus message is found to be deserialized;

            // (2) -- Get message Body as string from Service Bus Message: ----------
            string body = Encoding.UTF8.GetString(message.Body);

            // (3) -- Deserialize body string into type which was sent to Service Bus by sender app:
            CheckoutHeaderDto headerDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            // (4) -- Now received strongly typed SBus message is to be processed as desired:
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
            await this.repository.AddOrder(order);

            // Now that we saved data into the database, send a message to SB for others to handle async:
            PaymentRequestMessage sbRequestMessage = new()
            {
                Name = order.FirstName + " " + order.LastName,
                CardNumber = headerDto.CardNumber,
                CVV = order.CVV,
                ExpiryMonthYear = order.ExpiryMonthYear,
                OrderId = order.OrderHeaderId,
                OrderTotal = order.OrderTotal
            };
            try
            {
                await messageBus.PublishMessage(sbRequestMessage, this.orderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch
            {
                throw;
            }

        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}

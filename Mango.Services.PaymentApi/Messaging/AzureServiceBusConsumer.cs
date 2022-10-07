using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.PaymentProcessor;
using Mango.PaymentProcessor.Models;
using Mango.Services.PaymentApi.Messages;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.PaymentApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration configuration;
        private readonly IMessageBus messageBus;
        private readonly IPaymentProcessor paymentProcessor;
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionName;
        private readonly string orderPaymentProcessTopic;
        private ServiceBusProcessor sbProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IMessageBus messageBus, IPaymentProcessor paymentProcessor)
        {
            this.configuration = configuration;
            this.messageBus = messageBus;
            this.paymentProcessor = paymentProcessor;
            this.serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            this.orderPaymentProcessTopic = configuration.GetValue<string>("OrderPaymentProcessTopic");
            this.subscriptionName = configuration.GetValue<string>("SubscriptionName");

            // -(1) -- Create Service Bus Client to listen to messages:
            var client = new ServiceBusClient(this.serviceBusConnectionString);

            //-(2) -- Create service bus processor: ----------
            this.sbProcessor = client.CreateProcessor(this.orderPaymentProcessTopic, this.subscriptionName);

        }

        public async Task Start()
        {
            this.sbProcessor.ProcessMessageAsync += this.OnPaymentMessageReceived;
            this.sbProcessor.ProcessErrorAsync += this.ErrorHandler;
            await this.sbProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            this.sbProcessor.ProcessMessageAsync -= this.OnPaymentMessageReceived;
            this.sbProcessor.ProcessErrorAsync -= this.ErrorHandler;
            await this.sbProcessor.StopProcessingAsync();
            await this.sbProcessor.DisposeAsync();
        }

        public async Task OnPaymentMessageReceived(ProcessMessageEventArgs args)
        {
            try
            {
                // (1) -- Get Message from Service Bus: --------------
                ServiceBusReceivedMessage message = args.Message; //<= This is where the actual Bus message is found to be deserialized;

                // (2) -- Get message Body as string from Service Bus Message: ----------
                string body = Encoding.UTF8.GetString(message.Body);

                PaymentRequestMessage sbMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);
                if (sbMessage != null)
                {
                    PaymentDto dto = new PaymentDto()
                    {
                        CardNumber = sbMessage.CardNumber,
                        CVV = sbMessage.CVV,
                        ExpiryMonthYear = sbMessage.ExpiryMonthYear,
                        Name = sbMessage.Name,
                        OrderId = sbMessage.OrderId,
                        OrderTotal = sbMessage.OrderTotal
                    };
                    bool isPaid = this.paymentProcessor.ProcessPayment(dto); // <= This is where credit card is charged and payment is done;

                    if (isPaid)
                    {
                        // (3) -- Now since credit card is successfully charged, raise another event with SB under different
                        //     -- topic and have differenct subscriptions handle it by whoever needs to know that payment is completed.
                    }
                    else
                    {
                        //Todo: handle unsuccessful payment whichever way you wish;
                    }
                }
            }
            catch(Exception error)
            {
                Console.WriteLine(error.Message + Environment.NewLine + error.StackTrace);
            }
        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}

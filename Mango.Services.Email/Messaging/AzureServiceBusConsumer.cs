using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Models;
using Mango.Services.Email.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly EmailRepository repository;
        private readonly IConfiguration configuration;

        #region Fields for payment completed Message from SB: =========================

        private readonly string serviceBusConnectionString;
        private readonly string subscriptionName;
        private readonly string messageTopic;

        #endregion

        
        private ServiceBusProcessor paymentReceivedProcessor;

        public AzureServiceBusConsumer(EmailRepository repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
            //<= For checkout Message receiving and sending back to SB: =========================
            this.serviceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
            this.subscriptionName = configuration.GetValue<string>("EmailSubscriptionName");
            this.messageTopic = configuration.GetValue<string>("OrderUpdatePaymentResultTopic");




            // -(1) -- Create Service Bus Client to listen to messages:
            var client = new ServiceBusClient(this.serviceBusConnectionString);

            //-(2) -- Create service bus processor: ----------
            this.paymentReceivedProcessor = client.CreateProcessor(this.messageTopic, this.subscriptionName); //<-for payment status messages;
        }

        public async Task Start()
        {
            // <== Checkout Processor listening start: ===============
            this.paymentReceivedProcessor.ProcessMessageAsync += this.OnPaymentMessageReceived;
            this.paymentReceivedProcessor.ProcessErrorAsync += this.ErrorHandler;
            await this.paymentReceivedProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            // <== Checkout Processor listening start: ===============
            this.paymentReceivedProcessor.ProcessMessageAsync -= this.OnPaymentMessageReceived;
            this.paymentReceivedProcessor.ProcessErrorAsync -= this.ErrorHandler;
            await this.paymentReceivedProcessor.StopProcessingAsync();
            await this.paymentReceivedProcessor.DisposeAsync();
        }

        public async Task OnPaymentMessageReceived(ProcessMessageEventArgs args)
        {
            ServiceBusReceivedMessage message = args.Message;
            string body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage messageDto = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            await this.repository.SendAndLogEmail(messageDto);
            await args.CompleteMessageAsync(args.Message);

        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}

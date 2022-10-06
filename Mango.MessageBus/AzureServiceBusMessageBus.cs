using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        #region Private Fields

        private string connectionString = "Endpoint=sb://sivanchenkov.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xUi8aoNv0AVubQKh7y3HGqldXQy4lbF74JGN27wV19s=";
        #endregion


        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            // --  (1) -- Create sender client to talk to Azure Service Bus:  ----
            await using var senderClient = new ServiceBusClient(this.connectionString);

            // -- (2) -- Create sender by referencing clinet: ---------
            ServiceBusSender sender = senderClient.CreateSender(topicName);

            // -- (3) -- Serialize passed-in message class to JSON string: -----
            string jsonMessage = JsonConvert.SerializeObject(message);

            // -- (4) -- Create standard "Message" object for talking to Service Bus consistently: ----
            ServiceBusMessage sentMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            // -- (5) -- Actually send message via calling sender client class, created above: ---
            await sender.SendMessageAsync(sentMessage);

            // -- (6) -- Close sender client connection to Azure Service Bus:
            await senderClient.DisposeAsync();

        }
    }
}

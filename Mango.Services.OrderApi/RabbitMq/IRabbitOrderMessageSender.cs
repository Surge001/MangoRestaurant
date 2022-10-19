using Mango.MessageBus;

namespace Mango.Services.OrderApi.RabbitMq
{
    public interface IRabbitOrderMessageSender
    {
        void SendMessage(BaseMessage message, string queueName);
    }
}

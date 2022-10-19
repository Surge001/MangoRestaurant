using Mango.MessageBus;

namespace Mango.Services.OrderApi.RabbitMq
{
    public interface IRabbitPaymentMessageSender
    {
        void SendMessage(BaseMessage message, string queueName);
    }
}

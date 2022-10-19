using Mango.MessageBus;

namespace Mango.Services.ShoppingCartApi.RabbitMq
{
    public interface IRabbitCartMessageSender
    {
        void SendMessage(BaseMessage message, string queueName);
    }
}

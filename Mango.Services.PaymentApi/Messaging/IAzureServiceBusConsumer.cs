namespace Mango.Services.PaymentApi.Messaging
{

    /// <summary>
    /// Handles receiving SB message when payment card needs to be charged,
    /// AND sends another message to SB to say that credit card was successfully
    /// charged to have anyone listen to it.
    /// </summary>
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}

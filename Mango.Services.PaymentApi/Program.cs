using Mango.MessageBus;
using Mango.PaymentProcessor;
using Mango.Services.OrderApi.RabbitMq;
using Mango.Services.PaymentApi.Extensions;
using Mango.Services.PaymentApi.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRabbitPaymentMessageSender>(x => new RabbitPaymentMessageSender("localhost", "guest", "guest"));
builder.Services.AddHostedService<RabbitMqConsumer>();

builder.Services.AddSingleton<IPaymentProcessor, PaymentProcessor>();

builder.Services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAzureServiceBusConsumer();

app.Run();

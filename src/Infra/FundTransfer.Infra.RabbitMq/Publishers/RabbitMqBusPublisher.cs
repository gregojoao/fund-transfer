using FundTransfer.Domain.Bus.Publishers;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;
using System.Text;

namespace FundTransfer.Infra.RabbitMq.Publishers
{
    public class RabbitMqBusPublisher : IBusPublisher
    {
        private readonly IConfiguration _configuration;

        public RabbitMqBusPublisher(IConfiguration configuration) =>
            _configuration = configuration;

        public async Task SendAsync(string message)
        {
            var factory = new ConnectionFactory() { HostName = GetHostName() };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: GetTransferQueueName(), durable: true, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            try
            {
                Log.Debug($"Posting message to queue({GetTransferQueueName}): {message}");
                await channel.BasicPublishAsync(exchange: "", routingKey: GetRoutingKeyName(), body: body);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                throw;
            }
        }

        private string GetRoutingKeyName() =>
            _configuration["RabbitMq:RoutingKey"];

        private string GetTransferQueueName() =>
            _configuration["RabbitMq:TransferQueue"];

        private string GetHostName() =>
            _configuration["RabbitMq:Url"];
    }
}
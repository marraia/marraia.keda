using Azure.Messaging.ServiceBus;

namespace KedaExemploSend
{
    public class Worker : BackgroundService
    {
        private const string queueName = "kedafila";
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger,
                        IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = _configuration.GetSection("ServiceBus:Connection").Value;

            await using (var client = new ServiceBusClient(connectionString))
            {
                var sender = client.CreateSender(queueName);

                for (int i = 0; i < 1000; i++)
                {
                    var messageBus = new ServiceBusMessage($"mensagem numero {i}");

                    await sender
                            .SendMessageAsync(messageBus)
                            .ConfigureAwait(false);

                    _logger.LogInformation($"mensagem numero {i} enviada");
                }
            }
        }
    }
}
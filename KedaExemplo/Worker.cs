using Azure.Messaging.ServiceBus;

namespace KedaExemplo
{
    public class Worker : BackgroundService
    {
        public const string QueueName = "kedafila";
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger,
                      IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = _configuration.GetSection("ServiceBus:Connection").Value;
            var client = new ServiceBusClient(connectionString);

            var processor = client.CreateProcessor(QueueName);

            processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            await processor
                    .StartProcessingAsync(stoppingToken)
                    .ConfigureAwait(false);
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            var message = arg.Message.Body.ToString();

            _logger.LogInformation($"Mensagem recebida => {message}");

            return Task.CompletedTask;
        }
    }
}
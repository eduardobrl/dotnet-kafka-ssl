using ProcessadorPedidos.Application.Ouvintes;

namespace ProcessadorPedidos.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOuvintePedidos _ouvinte;

    public Worker(ILogger<Worker> logger, IOuvintePedidos ouvinte)
    {
        _logger = logger;
        _ouvinte = ouvinte;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var ouvintetask = _ouvinte.Iniciar(stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}

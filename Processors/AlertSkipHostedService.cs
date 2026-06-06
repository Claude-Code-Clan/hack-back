namespace XakUjin2026
{
    public class AlertSkipHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        public AlertSkipHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Проверяем просроченные алерты раз в минуту.
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var alertSkipService = scope.ServiceProvider.GetRequiredService<AlertSkipService>();
                alertSkipService.SkipExpiredAlerts();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using XakUjin2026.DB;

namespace XakUjin2026
{
    public class AlertSkipService
    {
        private static readonly TimeSpan AlertLifetime = TimeSpan.FromMinutes(30);

        private readonly ApplicationDbContext _context;

        public AlertSkipService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SkipExpiredAlerts()
        {
            var threshold = DateTime.UtcNow - AlertLifetime;

            _context.Alerts
                .Where(a => a.Status == AlertStatus.Warning && a.CreatedAt <= threshold)
                .ExecuteUpdate(s => s.SetProperty(a => a.Status, AlertStatus.Skip));
        }
    }
}

using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class AuditService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly MetricsServiceClient _metricsServiceClient;

        public AuditService(ApplicationDbContext dbContext, MetricsServiceClient metricsServiceClient)
        {
            _dbContext = dbContext;
            _metricsServiceClient = metricsServiceClient;
        }

        public void RecordAction(UserAction action)
        {
            // Додаємо дію користувача в базу
            _dbContext.UserActions.Add(action);
            _dbContext.SaveChanges();

            // Створюємо базову метрику для дії
            var metric = new ActionResourceMetric
            {
                ActionId = action.Id,
                CpuTime = 0,
                RamUsageMb = 0,
                ResponseTimeMs = 0,
                CollectedAt = DateTime.Now
            };

            // Надсилаємо метрику через MetricsServiceClient
            _metricsServiceClient.SendMetric(metric);
        }

        public UserAction[] GetActionByUserId(int userId)
        {
            return _dbContext.UserActions
                .Where(a => a.UserId == userId)
                .ToArray();
        }

        public UserAction[] GetActionByTimeRange(DateTime start, DateTime end)
        {
            return _dbContext.UserActions
                .Where(a => a.CreatedAt >= start && a.CreatedAt <= end)
                .ToArray();
        }
    }
}

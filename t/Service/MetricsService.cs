using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class MetricsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuditServiceClient _auditClient;

        public MetricsService(ApplicationDbContext dbContext, AuditServiceClient auditClient)
        {
            _dbContext = dbContext;
            _auditClient = auditClient;
        }

        public void RecordMetric(ActionResourceMetric metric)
        {
            _dbContext.Metrics.Add(metric);
            _dbContext.SaveChanges();
            _auditClient.RecordAction(metric.ActionId);
        }

        public ActionResourceMetric[] GetMetricsByActionId(int actionId)
        {
            return _dbContext.Metrics.Where(m => m.ActionId == actionId).ToArray();
        }

        public IEnumerable<ActionResourceMetric> GetMetricsByTimeRange(DateTime start, DateTime end)
        {
            return _dbContext.Metrics
                .Include(m => m.UserAction)
                .Where(m => m.CollectedAt >= start && m.CollectedAt <= end)
                .ToList();
        }
    }
}

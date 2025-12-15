using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/metrics")]
    public class MetricController : ControllerBase
    {
        private readonly MetricsService _metricService;
        private readonly ILogger<MetricController> _logger;

        public MetricController(MetricsService metricService, ILogger<MetricController> logger)
        {
            _metricService = metricService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult AddMetric([FromBody] ActionResourceMetric metric)
        {
            _metricService.RecordMetric(metric);
            return Ok();
        }

        [HttpGet("action/{actionId}")]
        public IActionResult GetMetricByActionId(int actionId)
        {
            // Поточний процес
            var process = Process.GetCurrentProcess();

            // Вимір CPU time на старті
            var cpuStart = process.TotalProcessorTime;

            // Вимір загального часу виконання
            var stopwatch = Stopwatch.StartNew();


            var metrics = _metricService.GetMetricsByActionId(actionId);

            stopwatch.Stop();

            // Вимір CPU time після виконання
            var cpuEnd = process.TotalProcessorTime;
            var cpuTimeUsed = cpuEnd - cpuStart;

            var memoryUsedMb = process.WorkingSet64 / (1024.0 * 1024.0);

            string logFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metrics_log.txt");
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Загальний час: {stopwatch.ElapsedMilliseconds} ms, " +
                                $"CPU time: {cpuTimeUsed.TotalMilliseconds} ms, " +
                                $"Пам'ять: {memoryUsedMb:F2} MB, " +
                                $"Кількість записів: {metrics.Count()}";

            System.IO.File.AppendAllText(logFilePath, logMessage + Environment.NewLine);

            return Ok(metrics);
        }

        [HttpGet("time-range")]
        public IActionResult GetMetricsByTimeRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            // Поточний процес
            var process = Process.GetCurrentProcess();

            // Вимір CPU time на старті
            var cpuStart = process.TotalProcessorTime;

            // Вимір загального часу виконання
            var stopwatch = Stopwatch.StartNew();


            var metrics = _metricService.GetMetricsByTimeRange(start, end);

            stopwatch.Stop();

            // Вимір CPU time після виконання
            var cpuEnd = process.TotalProcessorTime;
            var cpuTimeUsed = cpuEnd - cpuStart;

            var memoryUsedMb = process.WorkingSet64 / (1024.0 * 1024.0);

            Console.WriteLine($"Загальний час: {stopwatch.ElapsedMilliseconds} ms,\n" +
                                $"CPU time: {cpuTimeUsed.TotalMilliseconds} ms, " +
                                $"Пам'ять: {memoryUsedMb:F2} MB, " +
                                $"Кількість записів: {metrics.Count()}");

            return Ok(metrics);
        }
    }
}

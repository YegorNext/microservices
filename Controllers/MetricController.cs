using System;
using System.Diagnostics;
using System.Text.Json;
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
            var process = Process.GetCurrentProcess();

            // Вимір CPU time на старті
            var cpuStart = process.TotalProcessorTime;

            // Вимір загального часу виконання
            var stopwatch = Stopwatch.StartNew();

            // Отримуємо метрики з бази
            var metrics = _metricService.GetMetricsByTimeRange(start, end);

            stopwatch.Stop();

            // Вимір CPU time після виконання
            var cpuEnd = process.TotalProcessorTime;
            var cpuTimeUsed = cpuEnd - cpuStart;

            var memoryUsedMb = process.WorkingSet64 / (1024.0 * 1024.0);

            // Формуємо об'єкт для повернення
            var result = new
            {
                Performance = new
                {
                    TotalElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    CpuTimeMilliseconds = cpuTimeUsed.TotalMilliseconds,
                    MemoryUsedMb = Math.Round(memoryUsedMb, 2),
                    RecordsCount = metrics.Count()
                },
                Data = metrics
            };

            // Вивід у консоль (для локального логування)
            Console.WriteLine("=== Performance Metrics ===");
            Console.WriteLine($"Total time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"CPU time: {cpuTimeUsed.TotalMilliseconds} ms");
            Console.WriteLine($"Memory used: {memoryUsedMb:F2} MB");
            Console.WriteLine($"Records count: {metrics.Count()}");
            Console.WriteLine("===========================");

            // Повернення JSON
            return new JsonResult(result, new JsonSerializerOptions
            {
                WriteIndented = true, // для читабельного форматування
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles // уникаємо циклів
            });
        }
    }
}

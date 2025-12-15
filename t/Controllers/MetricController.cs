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
        public async Task<IActionResult> GetMetricsByTimeRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var process = Process.GetCurrentProcess();
            int processorCount = Environment.ProcessorCount;

            var cpuStart = process.TotalProcessorTime;

            var stopwatch = Stopwatch.StartNew();

            double systemCpuLoadStart = ReadSystemCpuLoad();

            for (int i = 0; i < 17; i++)
            {
                var metrics = _metricService.GetMetricsByTimeRange(start, end);
            }

            stopwatch.Stop();
            
            var cpuEnd = process.TotalProcessorTime;
            double cpuTimeUsedMs = (cpuEnd - cpuStart).TotalMilliseconds;

            double cpuLoadPercent = (cpuTimeUsedMs / (stopwatch.Elapsed.TotalMilliseconds * processorCount)) * 100;

            var totalMemoryMb = process.WorkingSet64 / (1024.0 * 1024.0);

            double systemCpuLoadEnd = ReadSystemCpuLoad();
            double systemCpuLoadPercent = (systemCpuLoadEnd - systemCpuLoadStart) * 100;

            var result = new
            {
                Performance = new
                {
                    TotalElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    AppCpuLoadPercent = Math.Round(cpuLoadPercent, 2),
                    TotalMemoryUsedMb = Math.Round(totalMemoryMb, 2),
                    SystemCpuLoadPercent = Math.Round(systemCpuLoadPercent, 2)
                }
            };

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return new JsonResult(result, new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });
        }

        // РњРµС‚РѕРґ РґР»СЏ С‡С‚РµРЅРёСЏ РѕР±С‰РµР№ Р·Р°РіСЂСѓР·РєРё CPU РІ Linux
        private double ReadSystemCpuLoad()
        {
            try
            {
                var lines = System.IO.File.ReadAllLines("/proc/stat");
                var cpuLine = lines.FirstOrDefault(l => l.StartsWith("cpu "));
                if (cpuLine == null) return 0;

                var parts = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                double user = double.Parse(parts[1]);
                double nice = double.Parse(parts[2]);
                double system = double.Parse(parts[3]);
                double idle = double.Parse(parts[4]);
                double iowait = double.Parse(parts[5]);
                double irq = double.Parse(parts[6]);
                double softirq = double.Parse(parts[7]);
                double steal = double.Parse(parts[8]);

                double totalIdle = idle + iowait;
                double totalNonIdle = user + nice + system + irq + softirq + steal;
                double total = totalIdle + totalNonIdle;

                return totalNonIdle / total;
            }
            catch
            {
                return 0;
            }       
         }    

      
      }
    
}

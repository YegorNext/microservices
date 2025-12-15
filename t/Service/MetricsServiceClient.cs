using System.Text.Json;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class MetricsServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public MetricsServiceClient(HttpClient httpClient, string endpoint)
        {
            _httpClient = httpClient;
            _endpoint = endpoint;
        }

        public void SendMetric(ActionResourceMetric metric)
        {
            var json = JsonSerializer.Serialize(metric);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.PostAsync(_endpoint, content).GetAwaiter().GetResult();
        }
    }
}

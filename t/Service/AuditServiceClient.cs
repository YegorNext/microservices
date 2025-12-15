using System.Text.Json;
using System.Text;

namespace WebApplication1.Service
{
    public class AuditServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public AuditServiceClient(HttpClient httpClient, string endpoint)
        {
            _httpClient = httpClient;
            _endpoint = endpoint;
        }

        public void RecordAction(int actionId)
        {
        
            var content = new StringContent(
                JsonSerializer.Serialize(new { actionId }),
                Encoding.UTF8,
                "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, _endpoint)
            {
                Content = content
            };

            using var response = _httpClient.Send(request); 
            response.EnsureSuccessStatusCode(); 
        }
    }
}

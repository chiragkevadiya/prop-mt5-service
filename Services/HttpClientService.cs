using PropMT5Service.ApiResponse;
using System.Net.Http;
using System.Threading.Tasks;

namespace PropMT5Service.Services
{
    public interface IHttpClientService
    {
        Task<ResponseHttpMessage> PostAsync(string baseUrl, string url);
    }

    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _client;

        public HttpClientService()
        {
            _client = new HttpClient();
        }

        public async Task<ResponseHttpMessage> PostAsync(string baseUrl, string url)
        {
            var response = await _client.PostAsync($"{baseUrl}/{url}", null);
            var content = await response.Content.ReadAsStringAsync();
            return new ResponseHttpMessage
            {
                Success = response.IsSuccessStatusCode,
                Message = content
            };
        }
    }
}


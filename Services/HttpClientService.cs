using PropMT5ConnectionService.ApiResponse;
using System.Net.Http;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Services
{
    public interface IHttpClientService
    {
        //Task<ResponseHttpMessage> GetAsync(string BaseAddress, string getUrl);
        //Task<ResponseHttpMessage> PostAsync(string BaseAddress, string postUrl, object modelPass);
        //Task<ResponseHttpMessage> PostAsync(string BaseAddress, string getUrl);
        //Task<ResponseHttpMessage> PostAsync(string url, object data, string apiKeyName = null, string apiKeyValue = null);
        //Task<BaseResponse> GetBrokerLicenseAsync(string Url);

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


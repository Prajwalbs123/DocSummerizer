using DocQuery.Contracts;
using DocQuery.Model;

namespace DocQuery.Services
{
    public class ApiCallService(HttpClient client) : IApiCallService
    {
        private readonly HttpClient httpClient = client;
        //upload controller
        public async Task<string> GetSummaryAsync(MultipartFormDataContent content)
        {
            var response = await httpClient.PostAsync(@"Upload/Summary", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostFileAsync(MultipartFormDataContent content)
        {
            var response = await httpClient.PostAsync(@"Upload/save", content);
            return await response.Content.ReadAsStringAsync();
        }

        //query controller
        public async Task<HttpResponseMessage> PostQueryAsync(HttpContent content)
        {
            return await httpClient.PostAsync("Query", content);
        }

        public async Task<HttpResponseMessage> GetFileListAsync()
        {
            return await httpClient.GetAsync("Query");
        }
    }
}

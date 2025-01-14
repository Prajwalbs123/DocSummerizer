using fnf.DocQuery.WebUI.Contracts;

namespace fnf.DocQuery.WebUI.Services
{
    public class FeatureDataService(HttpClient httpClient, ILogger<FeatureDataService> logger) : IFeatureDataService
    {
        public bool IsUploadFeatureEnabled { get; set; } = true;
        private readonly ILogger<FeatureDataService> logger = logger;

        private readonly HttpClient httpClient = httpClient;
        public async Task GetFeatureStatusAsync()
        {
            logger.LogInformation("Updating features");
            try
            {
                var response = await httpClient.GetAsync("Upload");
                if (response.IsSuccessStatusCode)
                {
                    IsUploadFeatureEnabled = true;
                }
                else
                {
                    IsUploadFeatureEnabled = false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error: {ex.Message}");
            }

        }
    }
}

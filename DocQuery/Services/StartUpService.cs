using System.Text.Json;
using DocQuery.Contracts;
using DocQuery.Model;

namespace DocQuery.Services
{
    /// <summary>
    /// Used to pull The files stored in Index (azure search)
    /// </summary>
    public class StartUpService(ILogger<StartUpService> _logger, IApiCallService apiCallService) : IStartUpService
    {
        private readonly ILogger<StartUpService> _logger = _logger;
        private readonly IApiCallService _apiCallService = apiCallService;
        public bool _hasInitialized { get; set; } = false;

        /// <summary>
        /// Initialization of SharedFileList of SharedDataModel
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            _logger.LogInformation("Initialization, populating sharedFileList");
            try
            {
                if (!_hasInitialized)
                {
                    SharedDataModel.SharedFileList.Clear();

                    var list = await _apiCallService.GetFileListAsync();
                    if (list.IsSuccessStatusCode)
                    {
                        var stringFile = await list.Content.ReadAsStringAsync();
                        Dictionary<string,string> FileList = JsonSerializer.Deserialize<Dictionary<string,string>>(stringFile)!;

                        foreach (var item in FileList)
                        {
                            SharedDataModel.SharedFileList.Add(item.Key,item.Value);
                        }
                    }

                    _hasInitialized = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }


        }
    }
}

namespace fnf.DocQuery.WebUI.Contracts
{
    public interface IFeatureDataService
    {
        bool IsUploadFeatureEnabled { get; set; }
        Task GetFeatureStatusAsync();
    }
}
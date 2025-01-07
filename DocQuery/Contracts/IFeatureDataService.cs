namespace DocQuery.Contracts
{
    public interface IFeatureDataService
    {
        bool IsUploadFeatureEnabled { get; set; }
        Task GetFeatureStatusAsync();
    }
}
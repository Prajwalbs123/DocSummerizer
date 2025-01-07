namespace DocQuery.Contracts
{
    public interface IStartUpService
    {
        bool _hasInitialized { get; set; }

        Task Initialize();
    }
}
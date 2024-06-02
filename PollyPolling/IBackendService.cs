namespace PollyPolling;

public interface IBackendService
{
    Task<int> Create();
    Task<Status> GetStatus(int id);
}

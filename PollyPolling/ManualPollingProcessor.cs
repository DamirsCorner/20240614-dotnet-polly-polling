namespace PollyPolling;

public class ManualPollingProcessor(IBackendService backendService) : IProcessor
{
    public static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    public async Task<bool> Process()
    {
        var id = await backendService.Create();
        var endTime = DateTime.UtcNow + Timeout;
        while (DateTime.UtcNow < endTime)
        {
            var status = await backendService.GetStatus(id);
            if (status != Status.Pending)
            {
                return status == Status.Success;
            }
            await Task.Delay(PollingInterval);
        }
        throw new TimeoutException();
    }
}

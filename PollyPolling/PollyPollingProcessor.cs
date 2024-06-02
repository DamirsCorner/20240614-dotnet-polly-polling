using Polly;
using Polly.Retry;

namespace PollyPolling;

public class PollyPollingProcessor(IBackendService backendService) : IProcessor
{
    public static readonly ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
        .AddRetry(
            new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().HandleResult(Status.Pending),
                Delay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 10,
            }
        )
        .Build();

    public async Task<bool> Process()
    {
        var id = await backendService.Create();
        var result = await pipeline.ExecuteAsync(async _ => await backendService.GetStatus(id));
        if (result == Status.Pending)
        {
            throw new TimeoutException();
        }
        return result == Status.Success;
    }
}

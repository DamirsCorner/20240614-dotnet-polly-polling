namespace PollyPolling;

public class PollyPollingProcessorTests : ProcessorTestsBase
{
    protected override IProcessor CreateProcessor(IBackendService backendService)
    {
        return new PollyPollingProcessor(backendService);
    }
}

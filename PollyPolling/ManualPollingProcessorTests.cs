namespace PollyPolling;

public class ManualPollingProcessorTests : ProcessorTestsBase
{
    protected override IProcessor CreateProcessor(IBackendService backendService)
    {
        return new ManualPollingProcessor(backendService);
    }
}

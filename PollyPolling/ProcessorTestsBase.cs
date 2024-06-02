using FluentAssertions;
using Moq;

namespace PollyPolling;

public abstract class ProcessorTestsBase
{
    protected abstract IProcessor CreateProcessor(IBackendService backendService);

    [TestCase(Status.Success)]
    [TestCase(Status.Failure)]
    public async Task ReturnsResultIfStatusResolvesImmediately(Status status)
    {
        var backendServiceMock = new Mock<IBackendService>();
        var id = 42;
        backendServiceMock.Setup(x => x.Create()).ReturnsAsync(id);
        backendServiceMock.Setup(x => x.GetStatus(id)).ReturnsAsync(status);
        var processor = CreateProcessor(backendServiceMock.Object);

        var result = await processor.Process();

        result.Should().Be(status == Status.Success);
    }

    [TestCase(Status.Success)]
    [TestCase(Status.Failure)]
    public async Task ReturnsResultIfStatusResolvesBeforeTimeout(Status status)
    {
        var backendServiceMock = new Mock<IBackendService>();
        var id = 42;
        backendServiceMock.Setup(x => x.Create()).ReturnsAsync(id);
        backendServiceMock
            .SetupSequence(x => x.GetStatus(id))
            .ReturnsAsync(Status.Pending)
            .ReturnsAsync(Status.Pending)
            .ReturnsAsync(Status.Pending)
            .ReturnsAsync(status);

        var processor = CreateProcessor(backendServiceMock.Object);

        var result = await processor.Process();

        result.Should().Be(status == Status.Success);
    }

    [Test]
    public async Task ThrowsIfStatusResolvesAfterTimeout()
    {
        var backendServiceMock = new Mock<IBackendService>();
        var id = 42;
        backendServiceMock.Setup(x => x.Create()).ReturnsAsync(id);
        backendServiceMock.Setup(x => x.GetStatus(id)).ReturnsAsync(Status.Pending);

        var processor = CreateProcessor(backendServiceMock.Object);

        var action = processor.Process;

        await action.Should().ThrowAsync<TimeoutException>();
    }
}

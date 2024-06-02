
namespace PollyPolling;

public interface IProcessor
{
    Task<bool> Process();
}
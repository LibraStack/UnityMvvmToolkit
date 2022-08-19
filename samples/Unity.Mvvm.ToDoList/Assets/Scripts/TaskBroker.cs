using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

public class TaskBroker : IDisposable
{
    private readonly Channel<string> _channel;

    private readonly IDisposable _connection;
    private readonly IConnectableUniTaskAsyncEnumerable<string> _multicastSource;

    public TaskBroker()
    {
        _channel = Channel.CreateSingleConsumerUnbounded<string>();
        _multicastSource = _channel.Reader.ReadAllAsync().Publish();
        _connection = _multicastSource.Connect();
    }

    public void Publish(string value)
    {
        _channel.Writer.TryWrite(value);
    }

    public IUniTaskAsyncEnumerable<string> Subscribe()
    {
        return _multicastSource;
    }

    public void Dispose()
    {
        _channel.Writer.TryComplete();
        _connection.Dispose();
    }
}
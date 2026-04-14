using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Toarnbeike.TestLibrary.Logging;

public sealed class TestLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<TestLogEntry> _entries = new();

    public IReadOnlyCollection<TestLogEntry> Entries => _entries;

    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger<object>(categoryName, _entries);
    }

    public LogAssertionScope Assert()
    {
        return new LogAssertionScope(_entries.ToArray());
    }

    public void Clear()
    {
        while (_entries.TryDequeue(out _))
        { }
    }

    public void Dispose()
    {
        // nothing to dispose.
    }
}
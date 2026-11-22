using Microsoft.Extensions.Logging;

namespace ObserverMagazine.Web.Tests.Services;

public sealed record LogEntry(LogLevel Level, string Message, Exception? Exception);

public sealed class ListLogger<T> : ILogger<T>
{
    public List<LogEntry> Entries { get; } = [];

    public IReadOnlyList<LogEntry> At(LogLevel level) => Entries.FindAll(e => e.Level == level);

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter) =>
        Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
}

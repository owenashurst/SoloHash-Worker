using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace SoloHash.Worker;

public class CustomConsoleFormatter() : ConsoleFormatter("custom")
{
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        var logMessage = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (!string.IsNullOrEmpty(logMessage))
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            textWriter.WriteLine($"[{timestamp}] {logMessage}");
        }
    }
}
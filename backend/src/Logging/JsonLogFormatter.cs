using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace FitHub.Common.Logging;

/// <summary>
/// Выводит JSON с полями:
/// Timestamp, Level (int), StringLevel (string), Application, Message + остальные свойства из enrichers.
/// </summary>
internal sealed class JsonLogFormatter : ITextFormatter
{
    private static readonly JsonValueFormatter ValueFormatter = new JsonValueFormatter();

    private readonly string application;

    /// <summary>
    /// Форматтер в json для логов
    /// </summary>
    /// <param name="application">Имя приложения, от имени которого будем писать логи</param>
    internal JsonLogFormatter(string application)
    {
        this.application = application;
    }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        FormatContent(logEvent, output, application);
    }

    private static void FormatContent(LogEvent logEvent, TextWriter output, string application)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(output);

        output.Write('{');

        WritePropertyAndValue(output, "timestamp", logEvent.Timestamp.ToString("o"));
        output.Write(",");

        WritePropertyAndValue(output, "application", application);
        output.Write(",");

        WritePropertyAndValue(output, "level", ((int)logEvent.Level).ToString());
        output.Write(",");

        WritePropertyAndValue(output, "stringLevel", logEvent.Level.ToString());
        output.Write(",");

        WritePropertyAndValue(output, "renderedMessage", logEvent.RenderMessage());
        output.Write(",");

        WritePropertyAndValue(output, "messageTemplate", logEvent.MessageTemplate.Text);

        if (logEvent.TraceId is not null)
        {
            output.Write(",");
            WritePropertyAndValue(output, "traceId", logEvent.TraceId.Value.ToString());
        }

        if (logEvent.SpanId is not null)
        {
            output.Write(",");
            WritePropertyAndValue(output, "spanId", logEvent.SpanId.Value.ToString());
        }

        if (logEvent.Exception is not null)
        {
            output.Write(",");
            WritePropertyAndValue(output, "exception", logEvent.Exception.ToString());
        }

        WriteProperties(output, logEvent.Properties);

        output.Write('}');
        output.WriteLine();
    }

    private static void WritePropertyAndValue(TextWriter output, string propertyKey, string propertyValue)
    {
        JsonValueFormatter.WriteQuotedJsonString(propertyKey, output);
        output.Write(":");
        JsonValueFormatter.WriteQuotedJsonString(propertyValue, output);
    }

    private static void WriteProperties(TextWriter output, IReadOnlyDictionary<string, LogEventPropertyValue> properties)
    {
        if (!properties.Any())
        {
            return;
        }

        output.Write(",\"fields\":{");

        var precedingDelimiter = "";
        foreach (var property in properties)
        {
            output.Write(precedingDelimiter);
            precedingDelimiter = ",";

            var camelCasePropertyKey = Char.ToLowerInvariant(property.Key[0]) + property.Key[1..];
            JsonValueFormatter.WriteQuotedJsonString(camelCasePropertyKey, output);
            output.Write(':');
            ValueFormatter.Format(property.Value, output);
        }

        output.Write('}');
    }
}

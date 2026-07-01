using System.Diagnostics;

namespace Webhooks.Processing.OpenTelemetry;

internal sealed class DiagnosticConfig
{
    internal static readonly ActivitySource Source = new("webhooks-processing");
}
using System.Diagnostics;

namespace Webhooks.Api.OpenTelemetry;

internal sealed class DiagnosticConfig
{
    internal static readonly ActivitySource Source = new("webhooks-api");
}
namespace Api.Trace;

public sealed class CorrelationIdMiddleware : IMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly ILogger<CorrelationIdMiddleware> _log;

    public CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> log) => _log = log;

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        var correlationId = ctx.Request.Headers.TryGetValue(HeaderName, out var v) && !string.IsNullOrWhiteSpace(v)
            ? v.ToString()
            : Guid.NewGuid().ToString("N");

        ctx.Items[HeaderName] = correlationId;
        ctx.Response.Headers[HeaderName] = correlationId;

        using (_log.BeginScope(new Dictionary<string, object> { [HeaderName] = correlationId }))
        {
            await next(ctx);
        }
    }
}

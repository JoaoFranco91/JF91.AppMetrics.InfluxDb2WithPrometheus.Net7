namespace JF91.AppMetrics.InfluxDb2WithPrometheus.Middleware;

using Extensions;
using App.Metrics;
using App.Metrics.Counter;
using Microsoft.AspNetCore.Http;

public class RequestsCounterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetrics _metrics;

    public RequestsCounterMiddleware
    (
        RequestDelegate next,
        IMetrics metrics
    )
    {
        _next = next;
        _metrics = metrics;
    }

    public async Task InvokeAsync
    (
        HttpContext context
    )
    {
        try
        {
            await _next(context);

            var tags = new MetricTags
            (
                new[]
                {
                    "method",
                    "path",
                    "user"
                },
                new[]
                {
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.User.GetEmail() ?? context.User.GetName() ?? context.User.GetUsername() ?? "Anonymous"
                }
            );

            _metrics.Measure.Counter.Increment
            (
                new CounterOptions
                {
                    Name = "http_requests_count",
                    Context = Environment.GetEnvironmentVariable("APPLICATION_NAME"),
                    MeasurementUnit = Unit.Calls,
                    Tags = tags
                }
            );
        }
        catch (Exception ex)
        {
        }
    }
}
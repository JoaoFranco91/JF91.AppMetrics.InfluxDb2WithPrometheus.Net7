namespace JF91.AppMetrics.InfluxDb2WithPrometheus.Middleware;

using Extensions;
using App.Metrics;
using App.Metrics.Apdex;
using Microsoft.AspNetCore.Http;

public class ResponsesApdexMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetrics _metrics;

    public ResponsesApdexMiddleware
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
            var tags = new MetricTags
            (
                new[]
                {
                    "method",
                    "path",
                    "user",
                    "request_id"
                },
                new[]
                {
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.User.GetEmail() ?? context.User.GetName() ?? context.User.GetUsername() ?? "Anonymous",
                    context.TraceIdentifier
                }
            );

            var apdex = new ApdexOptions
            {
                Name = "apdex",
                Context = Environment.GetEnvironmentVariable("APPLICATION_NAME"),
                ApdexTSeconds = 0.5, // Adjust based on your SLA
                Tags = tags
            };

            using (_metrics.Measure.Apdex.Track(apdex))
            {
                await _next(context);
            }
        }
        catch (Exception ex)
        {
        }
    }
}
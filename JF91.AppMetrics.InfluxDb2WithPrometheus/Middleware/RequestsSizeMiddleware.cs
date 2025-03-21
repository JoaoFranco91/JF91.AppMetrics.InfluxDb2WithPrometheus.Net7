namespace JF91.AppMetrics.InfluxDb2WithPrometheus.Middleware;

using Extensions;
using App.Metrics;
using App.Metrics.Histogram;
using Microsoft.AspNetCore.Http;

public class RequestsSizeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetrics _metrics;

    public RequestsSizeMiddleware
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
            var httpMethod = context.Request.Method.ToUpperInvariant();

            if (httpMethod == "POST" || httpMethod == "PUT")
            {
                if (context.Request.Headers != null && context.Request.Headers.ContainsKey("Content-Length"))
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
                            context.User.GetEmail() ??
                            context.User.GetName() ?? context.User.GetUsername() ?? "Anonymous",
                            context.TraceIdentifier
                        }
                    );

                    var postAndPutRequestSize = new HistogramOptions
                    {
                        Name = "http_requests_size",
                        Context = Environment.GetEnvironmentVariable("APPLICATION_NAME"),
                        MeasurementUnit = Unit.Bytes,
                        Tags = tags
                    };

                    _metrics.Measure.Histogram.Update
                    (
                        postAndPutRequestSize,
                        long.Parse(context.Request.Headers["Content-Length"].First())
                    );
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
        }
    }
}
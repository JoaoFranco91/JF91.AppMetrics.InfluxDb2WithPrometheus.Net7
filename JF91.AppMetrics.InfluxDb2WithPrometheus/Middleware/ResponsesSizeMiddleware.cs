namespace JF91.AppMetrics.InfluxDb2WithPrometheus.Middleware;

using Extensions;
using App.Metrics;
using App.Metrics.Histogram;
using Microsoft.AspNetCore.Http;

public class ResponsesSizeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetrics _metrics;

    public ResponsesSizeMiddleware
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

            if (httpMethod == "GET")
            {
                using (var buffer = new MemoryStream())
                {
                    var response = context.Response;

                    var bodyStream = response.Body;
                    response.Body = buffer;

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
                            context.User.GetEmail() ??
                            context.User.GetName() ?? context.User.GetUsername() ?? "Anonymous"
                        }
                    );

                    var getRequestSize = new HistogramOptions
                    {
                        Name = "http_responses_size",
                        Context = Environment.GetEnvironmentVariable("APPLICATION_NAME"),
                        MeasurementUnit = Unit.Bytes,
                        Tags = tags
                    };

                    _metrics.Measure.Histogram.Update
                    (
                        getRequestSize,
                        response.ContentLength ?? buffer.Length
                    );

                    buffer.Position = 0;
                    await buffer.CopyToAsync(bodyStream);
                }
            }
        }
        catch (Exception ex)
        {
        }
    }
}
namespace JF91.AppMetrics.InfluxDb2WithPrometheus.Middleware;

using Microsoft.AspNetCore.Builder;

public static class MetricsMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestsCounterMiddleware
    (
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<RequestsCounterMiddleware>();
    }

    public static IApplicationBuilder UseRequestsDurationMiddleware
    (
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<RequestsDurationMiddleware>();
    }

    public static IApplicationBuilder UseResponsesSizeMiddleware
    (
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<ResponsesSizeMiddleware>();
    }

    public static IApplicationBuilder UseResponsesApdexMiddleware
    (
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<ResponsesApdexMiddleware>();
    }

    public static IApplicationBuilder UseHttpStatusCodesCounterMiddleware
    (
        this IApplicationBuilder builder
    )
    {
        return builder.UseMiddleware<HttpStatusCodesCounterMiddleware>();
    }
}
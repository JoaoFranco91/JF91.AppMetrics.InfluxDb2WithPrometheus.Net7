namespace JF91.AppMetrics.InfluxDb2WithPrometheus.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

public static class MetricsExtensions
{
    public static IServiceCollection AddMetricsServices
    (
        this IServiceCollection services
    )
    {
        services.AddAppMetricsCollectors()
            .AddMvc()
            .AddMetrics();

        return services;
    }
}
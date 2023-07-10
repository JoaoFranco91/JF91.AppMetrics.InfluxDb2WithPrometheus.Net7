namespace JF91.AppMetrics.InfluxDb2WithPrometheus.Services;

using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;

public static class PrometheusExtensions
{
    public static IWebHostBuilder AddPrometheusAppMetrics
    (
        this IWebHostBuilder host
    )
    {
        host.UseMetrics
        (
            options =>
            {
                options.EndpointOptions = endpointsOptions =>
                {
                    endpointsOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                    endpointsOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                    endpointsOptions.EnvironmentInfoEndpointEnabled = false;
                };
            }
        );

        return host;
    }
}
namespace JF91.AppMetrics.InfluxDb2WithPrometheus.Services;

using Reporter;
using App.Metrics.AspNetCore;
using App.Metrics.Extensions.Configuration;
using App.Metrics.Formatters.InfluxDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

public static class InfluxDb2Extensions
{
    public static Action<InfluxDb2Options> Action { get; }
    
    public static IWebHostBuilder AddInfluxDb2AppMetrics
    (
        this IWebHostBuilder host,
        IConfiguration config,
        Action<InfluxDb2Options> options = null
    )
    {
        var influxOptions = new MetricsReportingInfluxDb2Options();
        config.GetSection(nameof(MetricsReportingInfluxDb2Options)).Bind(influxOptions);
        influxOptions.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();

        if (options != null)
        {
            options(influxOptions.InfluxDb2);
        }

        var metrics = App.Metrics.AppMetrics.CreateDefaultBuilder()
            .Configuration.ReadFrom(config)
            .Report.ToInfluxDb2(influxOptions)
            .Build();

        host.ConfigureMetrics(metrics)
            .UseMetrics();

        return host;
    }
}
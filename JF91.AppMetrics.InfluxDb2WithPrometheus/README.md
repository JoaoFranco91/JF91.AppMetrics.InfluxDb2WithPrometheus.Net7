I've created this package so you can easily integrate App.Metrics into you ASP WEB API to send collected metrics to an InfluxDB2 Server with Prometheus Text Formatter (/metrics-text). 

The setup is very straightforward, just install the Nuget Package, add the config section to your appsettings.json and inject the services into your program.cs. 

Please follow the instructions carefully.

#### Install Nuget:
```
dotnet add package JF91.AppMetrics.InfluxDb2WithPrometheus --version 1.0.2
```

<br>

#### 1 - Add this section to your ```appsettings.json``` and modify it to your needs:

```
"MetricsOptions": {
    "DefaultContextLabel": "MyCustomContext",
    "Enabled": true
},
    "MetricsWebTrackingOptions": {
    "ApdexTrackingEnabled": true,
    "ApdexTSeconds": 0.1,
    "IgnoredHttpStatusCodes": [ 404 ],
    "IgnoredRoutesRegexPatterns": [],
    "OAuth2TrackingEnabled": true
},
    "MetricEndpointsOptions": {
    "MetricsEndpointEnabled": true,
    "MetricsTextEndpointEnabled": true,
    "PingEndpointEnabled": true,
    "EnvironmentInfoEndpointEnabled": true
},
    "MetricsReportingInfluxDb2Options": {
    "InfluxDb2": {
    "BaseUri": "http://127.0.0.1:8086",
    "Organization": "metrics",
    "Bucket": "metrics",
    "Token": "changeme"
},
    "HttpPolicy": {
    "BackoffPeriod": "0:0:30",
    "FailuresBeforeBackoff": 5,
    "Timeout": "0:0:40"
},
    "ReportInterval": "0:0:1"
}
```

<br>

#### 2 - Add this Environment Variable to your ```launchSettings.json```:
```
"APPLICATION_NAME": "MyKickassApi"
```

<br>

#### 3 - Add this before ```builder.Build()```:
```
builder.Services.AddMetricsServices();
builder.WebHost
    .AddInfluxDb2AppMetrics(builder.Configuration)
    .AddPrometheusAppMetrics();
```

<br>

#### 4 - Add this after ```builder.Build()```:
```
app.UseRequestsCounterMiddleware();
app.UseRequestsDurationMiddleware();
app.UseResponsesSizeMiddleware();
app.UseResponsesApdexMiddleware();
app.UseMetricsAllMiddleware();
app.UseMetricsAllEndpoints();
```

<br>

#### 5 - Add this before ```app.Run()```:
```
app.UseHttpStatusCodesCounterMiddleware();
```

---


### How to add a new metrics middleware:

1 - Create an interceptor middleware;

2 - Inject ```IMetrics```;

```
public class MyNewMetricMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetrics _metrics;

    public MyNewMetricMiddleware(RequestDelegate next, IMetrics metrics)
    {
        _next = next;
        _metrics = metrics;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            var tags = new MetricTags
            (
                new[] { "Tag1-Key", "Tag2-Key" },
                new[] { "Tag1-Value, "Tag2-Value }
            );

            _metrics.Measure.Counter.Increment
            (
                new CounterOptions
                {
                    Name = "my-new-metric",
                    Context = "my-new-context",
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
```

<br>

3 - Add middleware to app pipeline:
```
builder.UseMiddleware<MyNewMetricMiddleware>();
```

---

### How to add a new metrics in controller:
1 - Inject ```IMetrics``` into controller:

```
public WeatherForecastController(ILogger<WeatherForecastController> logger, IMetrics metrics)
{
    _logger = logger;
    _metrics = metrics;
}
```

<br>

2 - Use ```IMetrics```:
```
[HttpGet(Name = "GetWeatherForecast")]
public IEnumerable<WeatherForecast> Get()
{
    var tags = new MetricTags
    (
        new[] { "Tag1-Key", "Tag2-Key" },
        new[] { "Tag1-Value, "Tag2-Value }
    );
    
    _metrics.Measure.Counter.Increment
    (
        new CounterOptions
        {
            Name = "my-new-metric",
            Context = "my-new-context",
            MeasurementUnit = Unit.Calls,
            Tags = tags
        }
    );

    return Enumerable.Range(1, 5).Select
        (
            index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }
        )
        .ToArray();
}
```
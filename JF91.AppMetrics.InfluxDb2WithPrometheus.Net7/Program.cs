using JF91.AppMetrics.InfluxDb2WithPrometheus.Middleware;
using JF91.AppMetrics.InfluxDb2WithPrometheus.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMetricsServices();
builder.WebHost
    .AddInfluxDb2AppMetrics(builder.Configuration)
    .AddPrometheusAppMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRequestsCounterMiddleware();
app.UseRequestsDurationMiddleware();
app.UseResponsesSizeMiddleware();
app.UseResponsesApdexMiddleware();
app.UseMetricsAllMiddleware();
app.UseMetricsAllEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHttpStatusCodesCounterMiddleware();

app.Run();
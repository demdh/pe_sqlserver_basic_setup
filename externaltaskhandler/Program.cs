using AtlasEngine;
using AtlasEngine.ApiClient;
using AtlasEngine.ExternalTasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
        {
            builder.AddJsonFile("appsettings.json");
        })
    .ConfigureLogging((context, builder) =>
        {
            builder.AddConfiguration(context.Configuration.GetSection("Logging"));
            builder.AddDebug();
            builder.AddConsole();
        })
    .ConfigureServices((hostContext, services) =>
        {
            services.AddTransient<SampleExternalTaskHandler>();

            services.AddHostedService<ApplicationLifecycleMonitor>();
            services.AddOptions<ApiClientSettings>().Bind(hostContext.Configuration.GetSection("ProcessEngine:ApiClientSettings"));
        })
    .UseExternalTaskWorkers(opt => { })
    .RunConsoleAsync();

[ExternalTaskHandler(topic: "SampleExternalTask")]
class SampleExternalTaskHandler : IExternalTaskHandler<SampleExternalTaskHandler.Payload, SampleExternalTaskHandler.Response>
{
    private readonly ILogger<SampleExternalTaskHandler> logger;

    public SampleExternalTaskHandler(ILogger<SampleExternalTaskHandler> logger)
    {
        this.logger = logger;
    }

    public Task<Response> HandleAsync(Payload payload, ExternalTask task)
    {
        logger.LogInformation($"Handling external task {task.Topic} (FlowNodeId {task.FlowNodeInstanceId})");
        logger.LogTrace($"External task data: '{payload.Data}' (FlowNodeId {task.FlowNodeInstanceId})");

        return Task.FromResult(new Response());
    }

    public record Payload(string Data);

    public record Response();  
}

class ApplicationLifecycleMonitor : IHostedService
{
    private readonly ILogger logger;

    public ApplicationLifecycleMonitor(ILogger<ApplicationLifecycleMonitor> logger)
    {
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("External task handling started...");
        logger.LogInformation("Press CTRL+C to exit");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("External task handling was stopped.");

        return Task.CompletedTask;
    }
}
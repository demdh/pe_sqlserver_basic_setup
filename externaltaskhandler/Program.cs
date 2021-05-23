using AtlasEngine;
using AtlasEngine.ExternalTasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var logger = LoggerFactory
    .Create(builder => builder.AddConfiguration(configuration.GetSection("Logging")).AddConsole())
    .CreateLogger("externaltaskhandler");

var externalTaskClient = ClientFactory.CreateExternalTaskClient(new Uri(configuration.GetValue<string>("ProcessEngineUri")));

logger.LogInformation("Start external tasks handling...");

var worker = externalTaskClient.SubscribeToExternalTaskTopic(
    "SampleExternalTask",
    s => s.UseHandlerMethod((ExternalTaskPayload payload, ExternalTask task) =>
    {
        logger.LogTrace($@"Handling external task {task.Topic} for process instance {task.ProcessInstanceId}{Environment.NewLine}
                        External task data: '{payload.Data}'");
        return Task.FromResult(new ExternalTaskResponse());
    }));

logger.LogInformation("External task handling started.");
logger.LogInformation("Press CTRL+C to exit");

await worker.StartAsync();

record ExternalTaskPayload(string Data);

record ExternalTaskResponse();
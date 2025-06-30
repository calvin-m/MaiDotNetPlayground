using Azure.Identity;
using MaiDotNetPlayground.DeepDiveOnLinq;
using MaiDotNetPlayground.ProofOfConcept.FtpClients;
using MaiDotNetPlayground.WritingAsyncAwaitFromScratch;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


var defaultAzureCredential = new DefaultAzureCredential(); // Azure.Identity NuGet package

var host = new HostBuilder()
    .ConfigureAppConfiguration((_, builder) =>
    {
        var endpoint = Environment.GetEnvironmentVariable("AppConfig");
        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            builder.AddAzureAppConfiguration(options =>   // Microsoft.Azure.AppConfiguration.Functions.Worker NuGet package
            {
                options.Connect(new Uri(endpoint), defaultAzureCredential);
                options.ConfigureKeyVault(kvOptions => { kvOptions.SetCredential(defaultAzureCredential); });  // Microsoft.Extensions.Configuration.AzureAppConfiguration NuGet package
            });
        }
    })
    .ConfigureFunctionsWebApplication()  // Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore
    .ConfigureServices((hostContext, services) =>
    {
        //var aiOptions = new ApplicationInsightsServiceOptions
        //{
        //    EnableAdaptiveSampling = false,
        //};
        //services.AddApplicationInsightsTelemetryWorkerService(aiOptions);
        //services.ConfigureFunctionsApplicationInsights();

        //services.AddAzureClients(clientBuilder =>
        //{
        //    var serviceBusConnectionString = hostContext.Configuration.GetValue<string>("ServiceBusConnectionString");
        //    var serviceBusMaxRetries = hostContext.Configuration.GetValue<int>("ServiceBusMaxRetries");
        //    clientBuilder.AddServiceBusClient(serviceBusConnectionString).ConfigureOptions(options =>
        //    {
        //        //options.TransportType = ServiceBusTransportType.AmqpWebSockets;
        //        options.RetryOptions.MaxRetries = serviceBusMaxRetries;
        //    });

        //    var blobServiceConnectionString = hostContext.Configuration.GetValue<string>("BlobStorageConnectionString");
        //    clientBuilder.AddBlobServiceClient(blobServiceConnectionString);

        //    clientBuilder.UseCredential(defaultAzureCredential);

        //    //RegisterServiceBusSenders(hostContext, clientBuilder);
        //});

        //services
        //    .AddCommonServices()
        //    .AddEihOptions()
        //    .AddAuthServices()
        //    .AddDomainSpecificServices();
    })
    .Build();

await host.RunAsync();
return;

/* Console App Entrypoint
// For adding appsetttings.json, need to add the following 3 NuGet packages:
// 1. Microsoft.Extensions.Configuration.Json
// 2. Microsoft.Extensions.Configuration
// 3. Microsoft.Extensions.Configuration.Abstractions
var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);
IConfiguration config = builder.Build();

SshNetFtpClient ftpClient = new(config);
await ftpClient.ListWorkingDirectory();

Console.ReadLine();
*/

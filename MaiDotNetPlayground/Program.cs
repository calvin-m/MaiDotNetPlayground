using MaiDotNetPlayground.DeepDiveOnLinq;
using MaiDotNetPlayground.ProofOfConcept.FtpClients;
using MaiDotNetPlayground.WritingAsyncAwaitFromScratch;
using Microsoft.Extensions.Configuration;

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

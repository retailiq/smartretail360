using SmartRetail360.WorkerBootstrap.Hosting;
using Microsoft.Extensions.Hosting;

var host = await WorkerHostFactory.CreateAsync<EmailConsumerWorker>(args);
await host.RunAsync();
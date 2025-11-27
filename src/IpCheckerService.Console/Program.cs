using IpCheckerService.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

Console.WriteLine("IP Checker Service - Console Mode");
Console.WriteLine("Press Ctrl+C to stop...\n");

await host.RunAsync();

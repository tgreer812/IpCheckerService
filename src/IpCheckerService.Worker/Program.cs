using IpCheckerService.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Enable running as a Windows Service
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "IpCheckerService";
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

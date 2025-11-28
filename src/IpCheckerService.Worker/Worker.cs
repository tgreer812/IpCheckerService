using System.Xml.Linq;
using IpCheckerService.Worker.ReportingEngines;

namespace IpCheckerService.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private TimeSpan _interval = TimeSpan.FromHours(24);
    private string _deviceName = "DEFAULTNAME";
    private readonly List<IReportingEngine> _reportingEngines = new();

    private static readonly Dictionary<string, Type> ReportingEngineTypes = new()
    {
        { "HttpReportingEngine", typeof(HttpReportingEngine) },
        { "EmailReportingEngine", typeof(EmailReportingEngine) }
    };

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        string configPath = Path.Combine(AppContext.BaseDirectory, "IpCheckerServiceConfigV2.xml");
        if (File.Exists(configPath))
        {
            try
            {
                XElement config = XElement.Load(configPath);
                _interval = GetIntervalFromConfig(config.Element("HeartbeatInterval"));
                _deviceName = config.Element("DeviceName")?.Value ?? _deviceName;

                InitializeReportingEngines(config.Element("ReportingEngines"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading configuration");
            }
        }
        else
        {
            _logger.LogWarning("Configuration file not found at {ConfigPath}", configPath);
        }
    }

    private void InitializeReportingEngines(XElement? reportingEnginesConfig)
    {
        if (reportingEnginesConfig == null) return;

        foreach (var engineConfig in reportingEnginesConfig.Elements("ReportEngine"))
        {
            string? engineType = engineConfig.Attribute("type")?.Value;
            if (string.IsNullOrEmpty(engineType))
            {
                _logger.LogWarning("ReportEngine element missing 'type' attribute, skipping");
                continue;
            }

            if (ReportingEngineTypes.TryGetValue(engineType, out Type? type))
            {
                try
                {
                    var engine = (IReportingEngine)Activator.CreateInstance(type)!;
                    engine.Initialize(engineConfig, _deviceName, _logger);
                    _reportingEngines.Add(engine);
                    _logger.LogInformation("Initialized reporting engine: {EngineType}", engineType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize {EngineType}", engineType);
                }
            }
            else
            {
                _logger.LogWarning("Unknown reporting engine type: {EngineType}", engineType);
            }
        }
    }

    private TimeSpan GetIntervalFromConfig(XElement? intervalElement)
    {
        if (intervalElement == null) return _interval;

        int days = int.Parse(intervalElement.Element("Days")?.Value ?? "0");
        int hours = int.Parse(intervalElement.Element("Hours")?.Value ?? "0");
        int minutes = int.Parse(intervalElement.Element("Minutes")?.Value ?? "0");
        int seconds = int.Parse(intervalElement.Element("Seconds")?.Value ?? "0");
        int milliseconds = int.Parse(intervalElement.Element("Milliseconds")?.Value ?? "0");

        return new TimeSpan(days, hours, minutes, seconds, milliseconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("IP Checker Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckInAsync();
            _logger.LogInformation("Sleeping for {Interval}...", _interval);
            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("IP Checker Service stopped");
    }

    private async Task CheckInAsync()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://api.ipify.org");
            var ipAddress = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Current IP Address: {IpAddress}", ipAddress);

            foreach (var engine in _reportingEngines)
            {
                try
                {
                    await engine.ReportAsync(ipAddress);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reporting engine failed");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during check-in");
        }
    }
}

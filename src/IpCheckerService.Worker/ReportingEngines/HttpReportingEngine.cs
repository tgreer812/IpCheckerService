using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace IpCheckerService.Worker.ReportingEngines;

public class HttpReportingEngine : IReportingEngine
{
    private string _endpointUrl = string.Empty;
    private string _deviceName = string.Empty;
    private ILogger _logger = null!;

    public void Initialize(XElement config, string deviceName, ILogger logger)
    {
        _deviceName = deviceName;
        _logger = logger;
        _endpointUrl = config.Element("EndpointUrl")?.Value
            ?? throw new ArgumentException("EndpointUrl is required for HttpReportingEngine");
    }

    public async Task ReportAsync(string ipAddress)
    {
        using var client = new HttpClient();
        
        var data = new
        {
            Name = _deviceName,
            IPv4 = ipAddress
        };

        string jsonData = JsonSerializer.Serialize(data);
        _logger.LogInformation("Sending IP Address to {EndpointUrl}", _endpointUrl);
        
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var postResponse = await client.PostAsync(_endpointUrl, content);

        if (postResponse.IsSuccessStatusCode)
        {
            _logger.LogInformation("HTTP check-in successful");
        }
        else
        {
            _logger.LogError("HTTP check-in failed: {ReasonPhrase}", postResponse.ReasonPhrase);
        }
    }
}

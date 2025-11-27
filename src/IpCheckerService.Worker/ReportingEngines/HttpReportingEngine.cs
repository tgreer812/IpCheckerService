using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace IpCheckerService.Worker.ReportingEngines;

public class HttpReportingEngine : IReportingEngine
{
    private string _endpointUrl = string.Empty;
    private string _deviceName = string.Empty;
    private Action<string> _log = _ => { };

    public void Initialize(XElement config, string deviceName, Action<string> log)
    {
        _deviceName = deviceName;
        _log = log;
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
        _log($"[INFO] Sending IP Address to {_endpointUrl}");
        
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var postResponse = await client.PostAsync(_endpointUrl, content);

        if (postResponse.IsSuccessStatusCode)
        {
            _log("[INFO] HTTP check-in successful.");
        }
        else
        {
            _log($"[ERROR] HTTP check-in failed: {postResponse.ReasonPhrase}");
        }
    }
}

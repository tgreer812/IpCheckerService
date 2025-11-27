using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace IpCheckerService.Worker.ReportingEngines;

public interface IReportingEngine
{
    void Initialize(XElement config, string deviceName, ILogger logger);
    Task ReportAsync(string ipAddress);
}

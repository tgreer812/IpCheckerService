using System.Xml.Linq;

namespace IpCheckerService.Worker.ReportingEngines;

public interface IReportingEngine
{
    void Initialize(XElement config, string deviceName, Action<string> log);
    Task ReportAsync(string ipAddress);
}

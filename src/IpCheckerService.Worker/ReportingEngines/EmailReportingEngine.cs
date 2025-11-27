using System.Xml.Linq;

namespace IpCheckerService.Worker.ReportingEngines;

public class EmailReportingEngine : IReportingEngine

    public void Initialize(XElement config, string deviceName, Action<string> log)
    {
        
    }

    public Task ReportAsync(string ipAddress)
    {
        throw new NotImplementedException();
    }
}

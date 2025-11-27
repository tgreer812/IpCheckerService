using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IpCheckerService.ReportingEngines
{
    public class EmailReportingEngine : IReportingEngine
    {
        public void Initialize(XElement config, string deviceName, Action<string> log)
        {
            throw new NotImplementedException();
        }

        public Task ReportAsync(string ipAddress)
        {
            throw new NotImplementedException();
        }
    }
}

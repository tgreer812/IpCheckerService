using System.Threading.Tasks;
using System.Xml.Linq;

namespace IpCheckerService.ReportingEngines
{
    public interface IReportingEngine
    {
        void Initialize(XElement config, string deviceName, System.Action<string> log);
        Task ReportAsync(string ipAddress);
    }
}

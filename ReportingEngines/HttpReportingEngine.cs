using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace IpCheckerService.ReportingEngines
{
    public class HttpReportingEngine : IReportingEngine
    {
        private string _endpointUrl;
        private string _deviceName;
        private Action<string> _log;

        public void Initialize(XElement config, string deviceName, Action<string> log)
        {
            _deviceName = deviceName;
            _log = log;
            _endpointUrl = config.Element("EndpointUrl")?.Value 
                ?? throw new ArgumentException("EndpointUrl is required for HttpReportingEngine");
        }

        public async Task ReportAsync(string ipAddress)
        {
            using (HttpClient client = new HttpClient())
            {
                var data = new
                {
                    Name = _deviceName,
                    IPv4 = ipAddress
                };

                string jsonData = JsonConvert.SerializeObject(data);
                _log($"[INFO] Sending IP Address to {_endpointUrl}");
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
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
    }
}

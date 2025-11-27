using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using System.Threading;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using IpCheckerService.ReportingEngines;

namespace IpCheckerService
{
    /// <summary>
    /// Service that checks in with an HTTP endpoint at a given interval to report the current IP address.
    /// </summary>
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer _timer;
        private double _interval = 24 * 60 * 60 * 1000; // 24 hours in milliseconds (default)
        private string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "IpCheckerService.log"; // Default log file path
        private string _name = "DEFAULTNAME"; // Default name
        private static readonly object logLock = new object();
        private List<IReportingEngine> _reportingEngines = new List<IReportingEngine>();

        private static readonly Dictionary<string, Type> ReportingEngineTypes = new Dictionary<string, Type>
        {
            { "HttpReportingEngine", typeof(HttpReportingEngine) }
            // Add more engine types here as they are created
        };

        public Service1()
        {
            this.ServiceName = "IpCheckerService";
            LoadConfiguration();
        }

        /// <summary>
        /// Load configuration settings from the configuration file.
        /// </summary>
        private void LoadConfiguration()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "IpCheckerServiceConfigV2.xml";
            if (File.Exists(configPath))
            {
                try
                {
                    XElement config = XElement.Load(configPath);
                    _interval = GetIntervalFromConfig(config.Element("HeartbeatInterval"));
                    logFilePath = config.Element("LogFilePath")?.Value ?? logFilePath;
                    _name = config.Element("DeviceName")?.Value ?? _name;

                    InitializeReportingEngines(config.Element("ReportingEngines"));
                }
                catch (Exception ex)
                {
                    Log($"[ERROR] loading configuration: {ex.Message}");
                }
            }
        }

        private void InitializeReportingEngines(XElement reportingEnginesConfig)
        {
            if (reportingEnginesConfig == null) return;

            foreach (var engineConfig in reportingEnginesConfig.Elements("ReportEngine"))
            {
                string engineType = engineConfig.Attribute("type")?.Value;
                if (string.IsNullOrEmpty(engineType))
                {
                    Log("[WARN] ReportEngine element missing 'type' attribute, skipping.");
                    continue;
                }

                if (ReportingEngineTypes.TryGetValue(engineType, out Type type))
                {
                    try
                    {
                        var engine = (IReportingEngine)Activator.CreateInstance(type);
                        engine.Initialize(engineConfig, _name, Log);
                        _reportingEngines.Add(engine);
                        Log($"[INFO] Initialized reporting engine: {engineType}");
                    }
                    catch (Exception ex)
                    {
                        Log($"[ERROR] Failed to initialize {engineType}: {ex.Message}");
                    }
                }
                else
                {
                    Log($"[WARN] Unknown reporting engine type: {engineType}");
                }
            }
        }

        private double GetIntervalFromConfig(XElement intervalElement)
        {
            if (intervalElement == null) return _interval;

            int days = int.Parse(intervalElement.Element("Days")?.Value ?? "0");
            int hours = int.Parse(intervalElement.Element("Hours")?.Value ?? "0");
            int minutes = int.Parse(intervalElement.Element("Minutes")?.Value ?? "0");
            int seconds = int.Parse(intervalElement.Element("Seconds")?.Value ?? "0");
            int milliseconds = int.Parse(intervalElement.Element("Milliseconds")?.Value ?? "0");

            TimeSpan interval = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            return interval.TotalMilliseconds;
        }

        protected override void OnStart(string[] args)
        {
            _timer = new System.Timers.Timer
            {
                Interval = _interval,
                AutoReset = true
            };
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
            Log("[INFO] Service started.");
        }

        /// <summary>
        /// Run the check-in process when the timer elapses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(async () => await CheckInAsync());
            Log($"[INFO] Sleeping for {_interval} milliseconds...");
        }

        private async Task CheckInAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync("https://api.ipify.org");
                    var ipAddress = await response.Content.ReadAsStringAsync();
                    Log($"[INFO] Current IP Address: {ipAddress}");

                    foreach (var engine in _reportingEngines)
                    {
                        try
                        {
                            await engine.ReportAsync(ipAddress);
                        }
                        catch (Exception ex)
                        {
                            Log($"[ERROR] Reporting engine failed: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"[ERROR] during check-in: {ex.Message}");
            }
        }


        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
            Log("[INFO] Service stopped.");
        }

        public void DebugRun(string[] args)
        {
            OnStart(args);
            Thread.Sleep(10000);
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
            OnStop();
        }

        /// <summary>
        /// Logs a message to the log file.
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            lock (logLock)
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
        }
    }
}

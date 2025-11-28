using System.Xml.Linq;
using IpCheckerService.Worker.ReportingEngines;
using Microsoft.Extensions.Logging;

namespace IpCheckerService.Tests;

/// <summary>
/// Integration test for email functionality.
/// Requires a config file at: src/IpCheckerService.Tests/bin/Debug/net8.0/EmailTestConfig.xml
/// 
/// Config format:
/// <?xml version="1.0" encoding="utf-8"?>
/// <ReportEngine type="EmailReportingEngine">
///     <EmailAddress>your-recipient@example.com</EmailAddress>
///     <SmtpHost>smtp.gmail.com</SmtpHost>
///     <SmtpPort>587</SmtpPort>
///     <SmtpUsername>your-email@gmail.com</SmtpUsername>
///     <SmtpPassword>your-app-password</SmtpPassword>
///     <UseSsl>true</UseSsl>
/// </ReportEngine>
/// </summary>
[TestClass]
public class EmailReportingEngineIntegrationTests
{
    private const string ConfigFileName = "EmailTestConfig.xml";
    private ILogger _logger = null!;

    [TestInitialize]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        _logger = loggerFactory.CreateLogger<EmailReportingEngineIntegrationTests>();
    }

    [TestMethod]
    public async Task SendEmail_WithRealSmtpServer_ShouldSucceed()
    {
        // Arrange
        string configPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
        
        if (!File.Exists(configPath))
        {
            Assert.Inconclusive($"Config file not found at: {configPath}. Create it to run this test.");
        }

        XElement config = XElement.Load(configPath);
        var engine = new EmailReportingEngine();
        engine.Initialize(config, "IntegrationTestDevice", _logger);

        // Act
        await engine.ReportAsync("192.168.1.100");

        // Assert - if no exception thrown, test passes
        _logger.LogInformation("Email sent successfully in integration test");
    }
}

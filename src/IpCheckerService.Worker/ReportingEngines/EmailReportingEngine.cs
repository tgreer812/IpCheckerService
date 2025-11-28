using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace IpCheckerService.Worker.ReportingEngines;

public class EmailReportingEngine : IReportingEngine
{
    private ILogger _logger = null!;
    private IEmailClient _emailClient = null!;
    private string _recipientEmail = string.Empty;
    private string _deviceName = string.Empty;

    public void Initialize(XElement config, string deviceName, ILogger logger)
    {
        _logger = logger;
        _deviceName = deviceName;

        // Parse recipient email
        _recipientEmail = config.Element("EmailAddress")?.Value
            ?? throw new ArgumentException("EmailAddress is required for EmailReportingEngine");

        // Parse SMTP settings
        string smtpHost = config.Element("SmtpHost")?.Value
            ?? throw new ArgumentException("SmtpHost is required for EmailReportingEngine");
        
        int smtpPort = int.Parse(config.Element("SmtpPort")?.Value 
            ?? throw new ArgumentException("SmtpPort is required for EmailReportingEngine"));
        
        string smtpUsername = config.Element("SmtpUsername")?.Value
            ?? throw new ArgumentException("SmtpUsername is required for EmailReportingEngine");
        
        string smtpPassword = config.Element("SmtpPassword")?.Value
            ?? throw new ArgumentException("SmtpPassword is required for EmailReportingEngine");
        
        bool useSsl = bool.Parse(config.Element("UseSsl")?.Value ?? "true");

        // Create email client
        _emailClient = new SmtpEmailClient(smtpHost, smtpPort, smtpUsername, smtpPassword, useSsl, logger);
        
        _logger.LogInformation("EmailReportingEngine initialized for {RecipientEmail}", _recipientEmail);
    }

    public async Task ReportAsync(string ipAddress)
    {
        string subject = $"IP Address Report - {_deviceName}";
        string body = $"Device: {_deviceName}\n" +
                      $"IP Address: {ipAddress}\n" +
                      $"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

        _logger.LogInformation("Sending IP report email to {RecipientEmail}", _recipientEmail);
        
        try
        {
            await _emailClient.SendEmailAsync(_recipientEmail, subject, body);
            _logger.LogInformation("Email report sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email report");
            throw;
        }
    }
}

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace IpCheckerService.Worker.ReportingEngines;

public class SmtpEmailClient : IEmailClient
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _username;
    private readonly string _password;
    private readonly bool _useSsl;
    private readonly ILogger _logger;

    public SmtpEmailClient(string smtpHost, int smtpPort, string username, string password, bool useSsl, ILogger logger)
    {
        _smtpHost = smtpHost;
        _smtpPort = smtpPort;
        _username = username;
        _password = password;
        _useSsl = useSsl;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_username),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(to);

        using var client = new SmtpClient(_smtpHost, _smtpPort)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = _useSsl
        };

        try
        {
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}

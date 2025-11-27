namespace IpCheckerService.Worker.ReportingEngines;

public interface IEmailClient
{
    Task SendEmailAsync(string to, string subject, string body);
}

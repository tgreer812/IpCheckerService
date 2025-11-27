using System.Xml.Linq;
using IpCheckerService.Worker.ReportingEngines;
using Microsoft.Extensions.Logging;
using Moq;

namespace IpCheckerService.Tests;

[TestClass]
public class EmailReportingEngineTests
{
    private Mock<ILogger> _mockLogger = null!;
    private const string TestDeviceName = "TestDevice";

    [TestInitialize]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger>();
    }

    [TestMethod]
    public void EmailReportingEngine_CanCreateInstance()
    {
        EmailReportingEngine engine = new EmailReportingEngine();
        Assert.IsNotNull(engine);
    }

    [TestMethod]
    public void Initialize_WithValidConfig_ShouldParseSuccessfully()
    {
        // Arrange
        var config = XElement.Parse(@"
            <ReportEngine type='EmailReportingEngine'>
                <EmailAddress>recipient@example.com</EmailAddress>
                <SmtpHost>smtp.gmail.com</SmtpHost>
                <SmtpPort>587</SmtpPort>
                <SmtpUsername>sender@example.com</SmtpUsername>
                <SmtpPassword>password123</SmtpPassword>
                <UseSsl>true</UseSsl>
            </ReportEngine>");

        var engine = new EmailReportingEngine();

        // Act
        engine.Initialize(config, TestDeviceName, _mockLogger.Object);

        // Assert - no exception means success
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("EmailReportingEngine initialized")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Initialize_WithMissingEmailAddress_ShouldThrowException()
    {
        // Arrange
        var config = XElement.Parse(@"
            <ReportEngine type='EmailReportingEngine'>
                <SmtpHost>smtp.gmail.com</SmtpHost>
                <SmtpPort>587</SmtpPort>
                <SmtpUsername>sender@example.com</SmtpUsername>
                <SmtpPassword>password123</SmtpPassword>
            </ReportEngine>");

        var engine = new EmailReportingEngine();

        // Act
        engine.Initialize(config, TestDeviceName, _mockLogger.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Initialize_WithMissingSmtpHost_ShouldThrowException()
    {
        // Arrange
        var config = XElement.Parse(@"
            <ReportEngine type='EmailReportingEngine'>
                <EmailAddress>recipient@example.com</EmailAddress>
                <SmtpPort>587</SmtpPort>
                <SmtpUsername>sender@example.com</SmtpUsername>
                <SmtpPassword>password123</SmtpPassword>
            </ReportEngine>");

        var engine = new EmailReportingEngine();

        // Act
        engine.Initialize(config, TestDeviceName, _mockLogger.Object);
    }
}

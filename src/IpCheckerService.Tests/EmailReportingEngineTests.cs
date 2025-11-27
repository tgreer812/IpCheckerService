using IpCheckerService.Worker.ReportingEngines;

namespace IpCheckerService.Tests;

[TestClass]
public class EmailReportingEngineTests
{
    [TestMethod]
    public void EmailReportingEngine_CanCreateInstance()
    {
        EmailReportingEngine engine = new EmailReportingEngine();
        Assert.IsNotNull(engine);
    }

    [TestMethod]
    public void Initialize_ShouldParseConfig()
    {
        // TODO: Implement test
    }

    [TestMethod]
    public async Task ReportAsync_ShouldSendEmail()
    {
        // TODO: Implement test
        await Task.CompletedTask;
    }
}

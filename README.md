# IpCheckerService
A .NET 8 Windows Service that periodically reports the current IP address of the device using configurable reporting engines.

## Features
- **Multiple Reporting Engines**: Support for HTTP endpoints, email, and extensible custom reporting mechanisms
- **Modern .NET 8**: Built on the latest .NET framework with Worker Service architecture
- **Flexible Configuration**: XML-based configuration for heartbeat intervals and reporting engine settings
- **Structured Logging**: Built-in logging using `Microsoft.Extensions.Logging`

## Project Structure
```
src/
├── IpCheckerService.Worker/       # Main service application
│   ├── ReportingEngines/          # Reporting engine implementations
│   │   ├── IReportingEngine.cs
│   │   ├── HttpReportingEngine.cs
│   │   └── EmailReportingEngine.cs
│   └── Worker.cs                  # Background service logic
└── IpCheckerService.Tests/        # Unit tests
```

## Installation Instructions

### Step 1: Build the Service
1. Navigate to the `src` directory:
   ```powershell
   cd src
   ```
2. Build the project:
   ```powershell
   dotnet build -c Release
   ```

### Step 2: Install the Service
1. Open PowerShell as Administrator.
2. Navigate to the build output directory (e.g., `src\IpCheckerService.Worker\bin\Release\net8.0\`).
3. Install the service using the `sc` command:
   ```powershell
   sc.exe create IpCheckerService binPath="C:\path\to\IpCheckerService.Worker.exe"
   ```

### Step 3: Start the Service
1. Start the service:
   ```powershell
   sc.exe start IpCheckerService
   ```

## Configuration Instructions

### Configuration File
The service uses `IpCheckerServiceConfigV2.xml` located in the same directory as the executable.

### Example Configuration
```xml
<?xml version="1.0" encoding="utf-8"?>
<Configuration>
    <HeartbeatInterval>
        <Days>1</Days>
        <Hours>0</Hours>
        <Minutes>0</Minutes>
        <Seconds>0</Seconds>
        <Milliseconds>0</Milliseconds>
    </HeartbeatInterval>
    <DeviceName>YourDeviceName</DeviceName>
    <ReportingEngines>
        <ReportEngine type="HttpReportingEngine">
            <EndpointUrl>http://your-custom-endpoint:9999</EndpointUrl>
        </ReportEngine>
        <ReportEngine type="EmailReportingEngine">
            <EmailAddress>your-email@example.com</EmailAddress>
        </ReportEngine>
    </ReportingEngines>
</Configuration>
```

### Customization
1. **DeviceName**: Set a unique name to identify this device in reports
2. **HeartbeatInterval**: Configure how often the service checks and reports the IP address
3. **ReportingEngines**: Add or remove reporting engines as needed
   - **HttpReportingEngine**: Sends IP via HTTP POST to a specified endpoint
   - **EmailReportingEngine**: Sends IP via email (implementation in progress)

### Restart the Service
After modifying the configuration file, restart the service:
```powershell
sc.exe stop IpCheckerService
sc.exe start IpCheckerService
```

## Development

### Running Locally
To run the service as a console application (for testing):
```powershell
cd src/IpCheckerService.Worker
dotnet run
```

### Running Tests
```powershell
cd src
dotnet test
```

## Extending with Custom Reporting Engines
To add a new reporting engine:

1. Create a class implementing `IReportingEngine` in the `ReportingEngines` folder
2. Implement the `Initialize` and `ReportAsync` methods
3. Register the type in `Worker.cs` in the `ReportingEngineTypes` dictionary
4. Add configuration in `IpCheckerServiceConfigV2.xml`


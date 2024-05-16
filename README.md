# IpCheckerService
A service that periodically alerts an endpoint with the current IP address of the device running the service.

## Installation Instructions

### Step 1: Build the Service
1. Open the `IpCheckerService` project in Visual Studio.
2. Build the project to generate the service executable. The output should be in the `bin\Debug` or `bin\Release` directory.

### Step 2: Install the Service
1. Open Command Prompt as Administrator.
2. Navigate to the directory containing your service executable. For example:
   ```cmd
   cd C:\path\to\your\executable
   ```
3. Install the service using the `sc` command:
    ```cmd
    sc create IpCheckerService binPath= "C:\path\to\your\executable\IpCheckerService.exe"
    ```

### Step 3: Start the Service
1. Open Command Prompt as Administrator.
2. Start the service using the `sc` command:
    ```cmd
    sc start IpCheckerService
    ```

## Configuration Instructions

### Step 1: Create the Configuration File
1. In the same directory as your service executable, create a file named IpCheckerServiceConfig.xml.
2. Add the following content to the IpCheckerServiceConfig.xml file:
    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <Configuration>
        <EndpointUrl>http://your-custom-endpoint:9999</EndpointUrl>
        <HeartbeatInterval>
            <Days>1</Days>
            <Hours>0</Hours>
            <Minutes>0</Minutes>
            <Seconds>0</Seconds>
            <Milliseconds>0</Milliseconds>
        </HeartbeatInterval>
        <LogFilePath>C:\Path\To\Your\Log\IpCheckerService.log</LogFilePath>
    </Configuration>
    ```

### Step 2: Customize the Configuration
1. Replace http://your-custom-endpoint:9999 with the actual URL of the endpoint you want to alert with the current IP address.
2. Adjust the values of <Days>, <Hours>, <Minutes>, <Seconds>, and <Milliseconds> to set the desired interval for the service to check in with the endpoint.
3. Set the <LogFilePath> to the desired location for the log file.

### Step 3: Restart the Service
1. After modifying the configuration file, restart the service to apply the changes:
    ```cmd
    sc stop IpCheckerService
    sc start IpCheckerService
    ```


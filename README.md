# BoxwriterInterop
A mapping between the Boxwriter API and the new OPCUA API allowing customers to quickly migrate to our new product.

[![.NET](https://github.com/ITWDiagraph/BoxwriterInterop/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/ITWDiagraph/BoxwriterInterop/actions/workflows/dotnet.yml)

### Configuration
Add printer names and addresses to the appsettings.json file.

## Functionality
### Locate
Making a UDP broadcast Request on port 2200 with the data
>{Locate BoxWriter}

The application will respond with
>{Locate BoxWriter, **_Address_**, **_Port_**, **_Machine Name_**}
- **_Address_**: IP address of the machine
- **_Port_**: Port it will respond on
- **_Machine Name_**: Machine name

### GetTasks
This request is used to determine the available tasks or messages that the printer can start

Making a TCP Request on the port *default 2202* with the data
> {Get tasks, **_PrinterName_**}

The application will respond with
> {Get tasks, **_PrinterName_**, **_TaskName1_**, **_TaskName2_**, ... }
- **_PrinterName_**: Name of the printer that the command is targeting
- **_TaskName_**: Name of a task that is available on this printer




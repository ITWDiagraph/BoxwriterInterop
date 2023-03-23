# BoxwriterInterop
A mapping between the Boxwriter API and the new OPCUA API allowing customers to quickly migrate to our new product.

[![.NET](https://github.com/ITWDiagraph/BoxwriterInterop/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/ITWDiagraph/BoxwriterInterop/actions/workflows/dotnet.yml)

### Configuration
Add printer names and addresses to the appsettings.json file.

## Functionality
### Locate
Making a UDP broadcast Request on port 2200 with the data

    {Locate BoxWriter}

The application will respond with

    {Locate BoxWriter, **_Address_**, **_Port_**, **_Machine Name_**}

- **_Address_**: IP address of the machine
- **_Port_**: Port it will respond on
- **_Machine Name_**: Machine name

### Get Tasks
Determines the available tasks or messages that the printer can start

Making a TCP Request on the port *default 2202* with the data

    {Get tasks, **_PrinterName_**}

The application will respond with
    
    {Get tasks, **_PrinterName_**, **_TaskName1_**, **_TaskName2_**, ...}
    
- **_PrinterName_**: Name of the printer that the command is targeting
- **_TaskName_**: Name of a task that is available on this printer

### Load Task
Prints a message

Making a TCP Request on the port *default 2202* with the data

    {Load task, **_PrinterName_**, **_TaskName_**}

The application will respond with

    {Load task, **_PrinterName_**, **_Result_**}

- **_PrinterName_**: Name of the printer that the command is targeting
- **_Result_**: Result of the command (1 for success, 0 for failure)

### Start Task
Resumes printing the current message

Making a TCP Request on the port *default 2202* with the data

    {Start task, **_PrinterName_**}

The application will respond with

    {Start task, **_PrinterName_**, **_Result_**}
    
- **_PrinterName_**: Name of the printer that the command is targeting
- **_Result_**: Result of the command (1 for success, 0 for failure)

### Idle Task
This command pauses the current message

Making a TCP Request on the port *default 2202* with the data

    {Idle task, **_PrinterName_**}

The application will respond with

    {Idle task, **_PrinterName_**, **_Result_**}
    
- **_PrinterName_**: Name of the printer that the command is targeting
- **_Result_**: Result of the command (1 for success, 0 for failure)

### Resume Task
Resumes the current message

Making a TCP Request on the port *default 2202* with the data

    {Resume task, **_PrinterName_**}

The application will respond with

    {Resume task, **_PrinterName_**, **_Result_**}
    
- **_PrinterName_**: Name of the printer that the command is targeting
- **_Result_**: Result of the command (1 for success, 0 for failure)

### Add Line
Saves a printer to the configuration file

Making a TCP Request on the port *default 2202* with the data

    {Add line, **_PrinterName_**, **IPAddress**}

The application will respond with
> {Add line, **_PrinterName_**, **_Result_**}
- **_PrinterName_**: Name of a printer that is configured
- **_Result_**: Result of the command (1 for success, 0 for failure)

### Get Lines
Gets all the saved printers

Making a TCP Request on the port *default 2202* with the data

    {Get lines}

The application will respond with

    {Get lines, **_PrinterName1_**, **_PrinterName2_**, ...}
    
- **_PrinterName_**: Name of a printer that is saved

### Get User Elements
Gets the variable data available for the current message. It returns no data if there are no variables in the message.

Making a TCP Request on the port *default 2202* with the data

    {Get user elements, **_PrinterName_**}

The application will respond with

    {Get user elements, **_PrinterName_**, **_Prompt1_**, **_Data1_**, **_Prompt2_**, **_Data2_**...}
    
- **_PrinterName_**: Name of the printer
- **_Prompt_**: The variable prompt
- **_Data_**: The variable data

### Set User Elements
Gets the variable data available for the current message. It returns no data if there are no variables in the message.

Making a TCP Request on the port *default 2202* with the data

    {Set user elements, **_PrinterName_**, **_Prompt1_**, **_Data1_**, **_Prompt2_**, **_Data2_**...}

The application will respond with

    {Set user elements, **_PrinterName_**, **_Result_**}
    
- **_PrinterName_**: Name of the printer
- **_Prompt_**: The variable prompt
- **_Data_**: The variable data
- **_Result_**: Result of the command (1 for success, 0 for failure)





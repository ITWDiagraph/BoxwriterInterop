# BoxwriterInterop
A mapping between the Boxwriter API and the new OPCUA API allowing customers to quickly migrate to our new product.

[![.NET](https://github.com/ITWDiagraph/BoxwriterInterop/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/ITWDiagraph/BoxwriterInterop/actions/workflows/dotnet.yml)

### Configuration
Use the Add Line command to add printer names and addresses to the PrinterConnections.json file.

# Functionality
## Locate
Making a UDP broadcast Request on port 2200 with the data

    {Locate BoxWriter}

The application will respond with

    {Locate BoxWriter, Address, Port, Machine Name}

- **Address**: IP address of the machine
- **Port**: Port it will respond on
- **Machine Name**: Machine name

## Get Tasks
Determines the available tasks that the printer can start

Making a TCP Request on the port *default 2202* with the data

    {Get tasks, PrinterName}

The application will respond with
    
    {Get tasks, PrinterName, TaskName1, TaskName2, ...}
    
- **PrinterName**: Name of the printer that the command is targeting
- **TaskName**: Name of a task that is on the printer

There will be no tasks returned if there are no tasks on the printer

### Load Task
Prints a message

Making a TCP Request on the port *default 2202* with the data

    {Load task, PrinterName, TaskName}

The application will respond with

    {Load task, PrinterName, Result}

- **PrinterName**: Name of the printer that the command is targeting
- **Result**: Result of the command (1 for success, 0 for failure)

## Start Task
Resumes printing the current message

Making a TCP Request on the port *default 2202* with the data

    {Start task, PrinterName}

The application will respond with

    {Start task, PrinterName, Result}
    
- **PrinterName**: Name of the printer that the command is targeting
- **Result**: Result of the command (1 for success, 0 for failure)

## Idle Task
This command pauses the current message

Making a TCP Request on the port *default 2202* with the data

    {Idle task, PrinterName}

The application will respond with

    {Idle task, PrinterName, Result}
    
- **PrinterName**: Name of the printer that the command is targeting
- **Result**: Result of the command (1 for success, 0 for failure)

## Resume Task
Resumes the current message

Making a TCP Request on the port *default 2202* with the data

    {Resume task, PrinterName}

The application will respond with

    {Resume task, PrinterName, Result}
    
- **PrinterName**: Name of the printer that the command is targeting
- **Result**: Result of the command (1 for success, 0 for failure)

## Add Line
Saves a printer to the configuration file

Making a TCP Request on the port *default 2202* with the data

    {Add line, PrinterName, IPAddress}

The application will respond with
 
    {Add line, PrinterName, Result}
    
- **PrinterName**: Name of a printer that is configured
- **Result**: Result of the command (1 for success, 0 for failure)

## Get Lines
Gets all the saved printers

Making a TCP Request on the port *default 2202* with the data

    {Get lines}

The application will respond with

    {Get lines, PrinterName1, PrinterName2, ...}
    
- **PrinterName**: Name of a printer that is saved

## Get User Elements
Gets the variable data available for the current message. It returns no data if there are no variables in the message.

Making a TCP Request on the port *default 2202* with the data

    {Get user elements, PrinterName}

The application will respond with

    {Get user elements, PrinterName, Prompt1, Data1, Prompt2, Data2, ...}
    
- **PrinterName**: Name of the printer
- **Prompt**: The variable prompt
- **Data**: The variable data

## Set User Elements
Gets the variable data available for the current message. It returns no data if there are no variables in the message.

Making a TCP Request on the port *default 2202* with the data

    {Set user elements, PrinterName, Prompt1, Data1, Prompt2, Data2, ...}

The application will respond with

    {Set user elements, PrinterName, Result}
    
- **PrinterName**: Name of the printer
- **Prompt**: The variable prompt
- **Data**: The variable data
- **Result**: Result of the command (1 for success, 0 for failure)





# Crestron Logging Demo 

Crestron Program that showcases the different Sinks, Enrichers and Extensions of the Serilog.CrestronLogger

At startup the program will write logs to Console and to the Error Log.  The logs are being [enriched](../src/Serilog.Crestron/Enrichers/README.md) with the Slot No/Room Id Information and the Program Name.
Through [extensions](../src/Serilog.Crestron/Global%20Extensions/README.md) the logs also carry the information on the Line Number, the Class and the File where the log was created.

You can browse through all the different themes for the Crestron Console on SSH (themes wont work Crestron Console via Toolbox) by typing the following Command on the console:
```csharp
BrowseThemes
```

You can also change the Error Level that is being Logged into the console by issuing the following command in the Console
```csharp
SetErrorLevel {LevelNo}
```
Where LevelNo is one of these numbers
* 1 Verbose
* 2 Debug
* 3 Information
* 4 Warning
* 5 Error
* 6 Fatal

At any particular time you can generate sample logs with various Error Levels that write to the [Crestron System Console](../src/Serilog.Crestron/Sinks/CrestronConsole/README.md) or the [Crestron Error Log](../src/Serilog.Crestron/Sinks/CrestronErrorLog/README.md).  Use these commands:
```csharp
RunConsoleExamples
```
or
```csharp
RunErrorLogExamples
```

Notice that the properties of the Logs going to the Crestron Error Log (Template and Error Level) are being controlled 
through a [XML configuration file](https://github.com/serilog/serilog-settings-appsettings) (App.config). This file is being watched for changes. When the file changes, the configuration is read again, and a new set of sample log entries are created.  

This is useful when running programs in VC4 where there is no console and you still need a way to change
the error level  at run time.
The file is saved in the RunningPrograms folder under the specific program instance you created for the program.
In VC4 that location is `/opt/crestron/virtualcontrol/RunningPrograms/{ProgramId}/App`.

To try it, open the **App.config** file while SSHing into VC4 using a file editor:
```bash
cd /opt/crestron/virtualcontrol/RunningPrograms/{ProgramId}/App
sudo nano App.config 
```
The sample XML File will look like this:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <add key="serilog:using:CrestronErrorLog" value="Serilog.Crestron" />
        <add key="serilog:write-to:CrestronErrorLog"  />
        <add key="serilog:write-to:CrestronErrorLog.outputTemplate" value="XML LOG: [{Level:u3} {SlotNo}-{FileName}-{MemberName}-{LineNumber}] {Message:lj}" />
        <add key="serilog:minimum-level" value="Verbose"/>
    </appSettings>
</configuration>
```

Modify the line `<add key="serilog:minimum-level" value="Verbose"/>` by changing the value Verbose to Fatal `<add key="serilog:minimum-level" value="Fatal"/>`

By watching the XML files for changes using [FileSystemWatcher](https://learn.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=net-7.0), you can just Write Out and Exit the editor, and issue the command `journalctl -xef -u virtualcontrol` and you will see a new set of log entries with the new error level.

You can also create configuration files using [JSON strings](https://github.com/serilog/serilog-settings-configuration), by using [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/) and the [AddJsonFile](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.jsonconfigurationextensions.addjsonfile?view=dotnet-plat-ext-7.0#microsoft-extensions-configuration-jsonconfigurationextensions-addjsonfile(microsoft-extensions-configuration-iconfigurationbuilder-system-action((microsoft-extensions-configuration-json-jsonconfigurationsource)) method. This supports
Hot Reload instead of FileSystemWatcher. Follow the same process as above but this time open the file `ErrorLog.json` and change the line `"MinimumLevel": "Fatal",` to `"MinimumLevel": "Verbose",`. On a 4-series processor, you can execute the custom console command `RunErrorLogExamples` to add new logs entries with the new level

Finally there is a third logger. This one records the logs to a File in the User Directory.  It uses [Serilog.Sinks.File](https://github.com/serilog/serilog-sinks-file) and reads
the maximum log level from an JSON File with Hot Reload.  It also has a rolling 
interval of one day. The common scenario would be to debug applications in VC4 (where there is no console) by changing the error level and monitoring the file using the following shell command:
```bash
tail {LogName} -F
```
This will track any changes added to the tail of the file in a user friendly location and away from the populated VirtualControl log.



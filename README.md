# Serilog Crestron Repository
A collection of sinks, enrichers, and extensions used to create structured logging on Crestron Automation Platform for Audio And Video integration.

It provides the following sinks:
* [CrestronSystemConsole](./src/Serilog.Crestron/Sinks/CrestronConsole/README.md). Writes Logs to Crestron Console.
* [CrestronErrorLog](./src/Serilog.Crestron/Sinks/CrestronErrorLog/README.md). Writes Logs to Crestron Permanent Error Log.

The Following Enrichers
* [SlotNo](./src/Serilog.Crestron/Enrichers/README.md). Injects a property containing the Slot Id or the Room Id of the running application.
* [ProgramName](./src/Serilog.Crestron/Enrichers/README.md). Injects a property containing the Program Name of the running application.

The Following Extensions:

* [FromHere](./src/Serilog.Crestron/Global%20Extensions/README.md). Extension that provide three [ForContext](https://github.com/serilog/serilog/wiki/Writing-Log-Events#correlation) Properties:
  * FileName: Log event property that contains the running code internal File Name.
  * MemberName: Log event property that contains the running code Member Name (Executing Method)
  * LineNumber: Log event property that contains the running code internal Line Number.

Logger configuration can easily be achieved by using Serilog Configuration packages:
* [Serilog.Settings.AppSettings](https://github.com/serilog/serilog-settings-appsettings) for XML Configurations
* [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) for JSON Configurations

These packages were designed for regular .NET applications (Microsoft Console, WinForms, etc), so a [couple of adjustments](./assets/FileConfigurations.md) are needed to work under Crestron.

### Getting started

There is a SIMPL# Pro project under the [sample Directory](./sample) that you can reference.  You can also download the compiled
project ([CrestronLoggingDemo.cpz](./sample/CrestronLoggingDemo.cpz)) and push into a 4-series processor or VC4.

### Contributing

See [Contributing.md](./CONTRIBUTING.md)

### Build Notes

The MSBUILD on this project has been converted to SDK, to simplify package maintenance.
This project borrows the code for its CrestronConsole from [Serilog.Sinks.Console](https://github.com/serilog/serilog-sinks-console)

_Copyright &copy; All Contributers - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
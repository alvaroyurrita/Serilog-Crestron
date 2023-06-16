# Serilog.Sinks.CrestronErrorLog

A Serilog sink that writes log events to the Error log of a Crestron 4-series Control Appliances and Crestron Control Servers such as a CP4(N) or VC4.

This repository has a sample ControlSystem Project ([CrestronLoggingDemo.cpz](../../../../sample/CrestronLoggingDemo.cpz)) that can be pushed to a Crestron 4-series Control Appliances and Crestron Control Servers such as a CP4(N) or VC4.

### Getting started

To use the Crestron Error Log sink, first install the NuGet package:
```shell
dotnet add package Serilog.Crestron
```
Then enable the sink using `WriteTo.CrestronErrorLog()`:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.CrestronErrorLog()
    .CreateLogger();

Log.Information("Hello world");
```
When connected using toolbox text console tool enter `ERRLOG PLOGALL` and you will see
```
6. Notice: SimplSharpPro[App01] # 2023-04-01 11:24:36 # [INF] Hello, world!
```
or when using VC4 connect via ssh, enter `journalctl -xef -u virtualcontrol` and you will see an entry like this on:
```
Apr 03 22:05:33 VC4 SimplSharpPro[50389]: [INF] Hello, world!
```

### Error Log Levels and Crestron Error Logs

There is no one to one relation between SeriLog Error Levels and Crestron's Error Log Levels.  This Logger follows this relations:

| SeriLog Error Level | Crestron ErrorLog Error Level |
|---------------------|-------------------------------|
| Verbose             | OK                            |
| Debug               | OK                            |
| Information         | Notice                        |
| Warning             | Warning                       |
| Error               | Error                         |
| Fatal               | Error[^1]                     |

[^1]: Crestron Error Log Supports a Fatal Error Level, but it is not exposed in the Crestron.SimplSharp.ErrorLog class

All SeriLog Log Events with an exception code will be logged into Crestron's ErrorLog as Exception which displays with and Error and an Exception message.
Notice that Crestron Exceptions are generated with a \n as new line, which does not translate well in Crestron Console.
```
  8. Error: SimplSharpPro[App01] # 2023-04-01 11:24:36 # Exception:[ERR] Error Level 5. Example: Exception Happened - System.DivideByZeroException: Attempted to divide by zero.
```

### Output templates

The format of events to the console can be modified using the `outputTemplate` configuration parameter:

```csharp
    .WriteTo.CrestronErrorLog(
        outputTemplate: "[{Level:u3}] {Message:lj}")
```

Notice that other [built-in properties](https://github.com/serilog/serilog/wiki/Formatting-Output) like `Timestamp` could be used, but Crestron already Time Stamps its Error Log entries, and therefore it is not necessary. Properties from events, including those attached using [enrichers](https://github.com/serilog/serilog/wiki/Enrichment), can also appear in the output template.

### Configuration Files

Logger configuration can easily be achieved by using Serilog Configuration packages:
* [Serilog.Settings.AppSettings](https://github.com/serilog/serilog-settings-appsettings) for XML Configurations
* [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) for JSON Configurations

These packages were designed for regular .NET applications (Microsoft Console, WinForms, etc), so a [couple of adjustments](../../../../assets/FileConfigurations.md) are needed to work under Crestron.


### Performance

Console logging is synchronous and this can cause bottlenecks in some deployment scenarios. For high-volume console logging, consider using [_Serilog.Sinks.Async_](https://github.com/serilog/serilog-sinks-async) to move console writes to a background thread:

```csharp
// dotnet add package serilog.sinks.async

Log.Logger = new LoggerConfiguration()
    .WriteTo.Async(wt => wt.CrestronErrorLogSink())
    .CreateLogger();
```

### Contributing

See [Contributing.md](../../../../CONTRIBUTING.md)

_Copyright &copy; All Contributers - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._

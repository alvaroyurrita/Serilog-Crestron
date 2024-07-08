# Serilog Crestron

## About

A collection of sinks, enrichers, and extensions used to create structured logging on Crestron Automation Platform for
Audio And Video integration. Used on Crestron 4-Series Control Appliances and Control Servers.

## Get Started

To use the different sinks, enrichers and extensions, first install the NuGet package:

```
dotnet add package Serilog.Crestron
```

### Crestron Console Sink

To enable sinks that write to a Crestron Console use:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.CrestronConsole()
    .CreateLogger();

Log.Information("Hello world");
```

When connected via ssh or using toolbox text console tool, you'll see the output:

```
[12:50:51 INF] Hello, world!
```

### Crestron Error Log Sink

To enable sinks that write to a the Crestron Error Log use:

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

or when using VC4 connect via ssh, enter `JOURNALCTL -xef -u VirtualControl` and you will see an entry like this on:

```
Apr 03 22:05:33 VC4 SimplSharpPro[50389]: [INF] Hello, world!
```

### Enrichers

The enrichers supported are:

* **SlotNo:** Provides the number of the slot the program is running on for Processors, or the Room Instance Id for
  Virtual Control
* **ProgramName:** Provides the Name of the Running Program. The assembly name in Processors , or the Program name in
  Virtual Control

Add `Enrich.WithSlotNo()` and/or `Enrich.WithProgramName()` to the `LoggerConfiguration()`

```csharp
_crestronErrorLog = new LoggerConfiguration()
    .Enrich.WithSlotNo()
    .Enrich.WithProgramName()
    .WriteTo.CrestronErrorLog(
        outputTemplate: "[{Level:u3} {SlotNo}-{ProgramName}] {Message:lj}")
    .CreateLogger();  
```

### Extensions

An extension to ILogger that adds the following properties to the running log:

* **FileName:** Name of the File where the log originated
* **MemberName:** Name of the containing Method where the log originated
* **LineNumber:** Line Number where the log originated

Add this extension add it to the Global Logger or instance logger

```csharp
 Log.Logger.FromHere().Verbose("{FileName}-{MemberName}-{LineNumber} Something to log");
 _myLog.FromHere().Verbose("{FileName}-{MemberName}-{LineNumber} Something to log");
```

_Copyright &copy; All Contributers - Provided under
the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
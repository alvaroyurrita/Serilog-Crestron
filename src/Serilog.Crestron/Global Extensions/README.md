# Serilog.Extensions.Crestron

An extension to ILogger that adds the following properties to the running log:

* FileName: Name of the File where the log originated
* MemberName: Name of the containing Method where the log originated
* LineNumber: Line Number where the log originated

### Getting started

To use the Crestron Extensions, first install the NuGet package:

```shell
dotnet add package Serilog.Crestron
```

Add this extension add it to the Global Logger or instance logger

```csharp
 Log.Logger.FromHere().Verbose("{FileName}-{MemberName}-{LineNumber} Something to log");
 _myLog.FromHere().Verbose("{FileName}-{MemberName}-{LineNumber} Something to log");
```

Or add a template that references those properties:

```csharp
    Log.Logger = new LoggerConfiguration()
        .WriteTo.CrestronErrorLog(
            outputTemplate: "[{Level:u3} {FileName}-{MemberName}-{LineNumber}] {Message:lj}")
        .CreateLogger();  
```

Note:  When adding to the template, you must ensure that the extension is used, otherwise the properties will be empty
and Serilog will throw an internal exception to its self log.

### Contributing

See [Contributing.md](../../../CONTRIBUTING.md)

_Copyright &copy; All Contributers - Provided under
the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._

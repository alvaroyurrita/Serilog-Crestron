# Serilog.Enrichers.Crestron

Enrichers that Enrich Serilogs Properties with Crestron Related Information.

The enrichers supported are:

* SlotNo: Provides the number of the slot the program is running on for Processors, or the Room Instance Id for Virtual
  Control
* ProgramName: Provides the Name of the Running Program. The assembly name in Processors , or the Program name in
  Virtual Control

### Getting started

To use the Crestron Enrichers, first install the NuGet package:

```shell
dotnet add package Serilog.Crestron
```

Add `Enrich.WithSlotNo()` and/or `Enrich.WithProgramName()` to the `LoggerConfiguration()` of your choice.

```csharp
_crestronErrorLog = new LoggerConfiguration()
    .Enrich.WithSlotNo()
    .Enrich.WithProgramName()
    .WriteTo.CrestronErrorLog(
        outputTemplate: "[{Level:u3} {SlotNo}-{ProgramName}] {Message:lj}")
    .CreateLogger();  
```

### Contributing

See [Contributing.md](../../../CONTRIBUTING.md)

_Copyright &copy; All Contributers - Provided under
the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._

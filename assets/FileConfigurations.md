# Using File Configurations

It is possible to configure the loggers by using Configuration Files.  A couple of things need to be addressed since Serilog is running on a Crestron Platform, and your program will be running as a Library inside Crestron run time program, instead of an application.

### XML `<appSettings>` configuration

To use the console sink with the [Serilog.Settings.AppSettings](https://github.com/serilog/serilog-settings-appsettings) package, first install that package if you haven't already done so:

```shell
dotnet add package Serilog.Settings.AppSettings
```

Instead of configuring the logger in code, call `ReadFrom.AppSettings()`:

```csharp
var log = new LoggerConfiguration()
    .ReadFrom.AppSettings("App.config")
    .CreateLogger();
```
Notice that the name of the file is necessary for the settings to work. AppSetting uses internally the [ConfigurationManager class](https://learn.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager?view=dotnet-plat-ext-7.0).
When the project is an Executable File (Windows Console, WinForms, etc), the App.config settings is compiled with the final program, and ConfigurationManager is capable of parsing the XML file without having to pass the file name. Because compiling Crestron SIMPL# programs are done using a .Net library, ConfigurationManager is not capable of loading the configuration without a file name. Any file name can be used not just App.config.  Be sure to set the **Copy To Output Directory** property of the file to **Copy always**

In your application's `App.config` file, specify the console sink assembly under the `<appSettings>` node:

```xml
<configuration>
    <appSettings>
      <add key="serilog:using:CrestronErrorLog" value="Serilog.Sinks.Crestron" />
      <add key="serilog:write-to:CrestronErrorLog" />
      <add key="serilog:write-to:CrestronErrorLog.outputTemplate" value="[{Level:u3} {SlotNo}-{FileName}-{MemberName}-{LineNumber}] {Message:lj}" />
      <add key="serilog:minimum-level" value="Verbose"/>
    </appSettings>
</configuration>
```


### JSON `appsettings.json` configuration

To use the console sink with [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json), use the [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) package. First install that packages if you have not already done so.


```shell
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Serilog.Settings.Configuration
```
Note that it is necessary to use the specified Microsoft Extension to support Json configuration loading under .Net Framework 4.7

Instead of configuring the sink directly in code, call `ReadFrom.Configuration()`:

```csharp
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
```

A sample json configuration will look like this: 
```json
{
    "Serilog": {
        "Using":  [ "Serilog.Crestron" ],
        "MinimumLevel": "Fatal",
        "Enrich": [ "WithSlotNo", "WithProgramName" ],
        "WriteTo": [
            {
                "Name": "CrestronErrorLog",
                "Args": {
                    "outputTemplate": "JSON LOG: [{Level:u3} {SlotNo}-{FileName}-{MemberName}-{LineNumber}] {Message:lj}",
                }
            }
        ]
    }
}
```
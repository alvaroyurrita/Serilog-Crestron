using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace CrestronLoggingDemo
{
    public partial class ControlSystem
    {
        private Logger _errorLogFromJson;
        private void SetJsonConfiguredLog()
        {
            //Setting Up Error Log with JSON Configuration.
            //Execute RunErrorLogExamples in the console to generate a new set of errors
            //No need to watch for file changes, since Microsoft.Extensions.Configuration.Json supports Hot Reload
            //https://swimburger.net/blog/dotnet/changing-serilog-minimum-level-without-application-restart-on-dotnet-framework-and-core
            var errorLogJsonConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("ErrorLog.json", false, true)
                .Build();
            _errorLogFromJson = new LoggerConfiguration()
                .ReadFrom.Configuration(errorLogJsonConfig)
                .CreateLogger();
        }
    }
}
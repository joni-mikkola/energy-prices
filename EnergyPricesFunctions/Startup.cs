using EnergyPricesBL;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(EnergyPricesFunctions.Startup))]
namespace EnergyPricesFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..");
            var environmentName = builder.GetContext().EnvironmentName;
            builder.ConfigurationBuilder
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            var connectionString = configuration["KeyvaultConnection"];
            builder.Services.AddSingleton(new KeyvaultAccessor(connectionString));
        }
    }
}
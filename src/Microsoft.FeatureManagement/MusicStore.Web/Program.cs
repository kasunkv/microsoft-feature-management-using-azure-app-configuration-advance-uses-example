using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MusicStore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) => {
                    var settings = config.Build();

                    config.AddAzureAppConfiguration(options =>
                    {
                        options
                            .Connect(settings["ConnectionStrings:AppConfiguration"])
                            .UseFeatureFlags(featureFlagOptions =>
                            {
                                // Reduce the cache expiry time to 5 seconds
                                featureFlagOptions.CacheExpirationTime = TimeSpan.FromSeconds(5);

                                // Set the REGION_NAME available in Azure App Service
                                featureFlagOptions.Label = Environment.GetEnvironmentVariable("REGION_NAME").Trim().ToLowerInvariant().Replace(' ', '_');
                            });
                    });
                });
    }
}

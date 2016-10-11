using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Modules;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orchard.Environment.Extensions;
using Orchard.Hosting.Web.Routing;

namespace Microsoft.AspNetCore.Mvc.Modules.Hosting
{
    public static class ApplicationBuilderExtensions
    {
        public static ModularApplicationBuilder UseMvcModules(this ModularApplicationBuilder modularApp)
        {
            modularApp.Configure(app =>
            {
                var extensionManager = app.ApplicationServices.GetRequiredService<IExtensionManager>();
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Default");

                // Route the request to the correct tenant specific pipeline
                app.UseMiddleware<OrchardRouterMiddleware>();

                // Load controllers
                var applicationPartManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();

                using (logger.BeginScope("Loading extensions"))
                {
                    Parallel.ForEach(extensionManager.AvailableFeatures(), feature =>
                    {
                        try
                        {
                            var extensionEntry = extensionManager.LoadExtension(feature.Extension);
                            applicationPartManager.ApplicationParts.Add(new AssemblyPart(extensionEntry.Assembly));
                        }
                        catch (Exception e)
                        {
                            logger.LogCritical("Could not load an extension", feature.Extension, e);
                        }
                    });
                }
            });

            return modularApp;
        }
    }
}
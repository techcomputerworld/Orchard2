using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Orchard.Environment.Extensions;

namespace Microsoft.AspNetCore.Modules.Hosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseModules(this IApplicationBuilder app, Action<ModularApplicationBuilder> modules)
        {
            // Ensure the shell tenants are loaded when a request comes in
            // and replaces the current service provider for the tenant's one.
            app.UseMiddleware<OrchardContainerMiddleware>();

            var modularApplicationBuilder = new ModularApplicationBuilder(app);
            modules(modularApplicationBuilder);

            return app;
        }

        public static ModularApplicationBuilder UseStaticFilesModules(this ModularApplicationBuilder modularApp)
        {
            modularApp.Configure(app =>
            {
                var extensionManager = app.ApplicationServices.GetRequiredService<IExtensionManager>();
                var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();

                // TODO: configure the location and parameters (max-age) per module.
                foreach (var extension in extensionManager.AvailableExtensions())
                {
                    var contentPath = Path.Combine(env.ContentRootPath, extension.Location, extension.Id, "Content");
                    if (Directory.Exists(contentPath))
                    {
                        app.UseStaticFiles(new StaticFileOptions()
                        {
                            RequestPath = "/" + extension.Id,
                            FileProvider = new PhysicalFileProvider(contentPath)
                        });
                    }
                }
            });

            return modularApp;
        }
    }
}
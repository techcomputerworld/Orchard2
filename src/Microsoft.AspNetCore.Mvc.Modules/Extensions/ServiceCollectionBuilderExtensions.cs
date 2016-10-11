using System.Collections.Generic;
using Microsoft.AspNetCore.Modules;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Orchard.Environment.Extensions;
using Orchard.Hosting.Mvc.Filters;
using Orchard.Hosting.Mvc.Razor;
using Orchard.Hosting.Routing;

namespace Microsoft.AspNetCore.Mvc.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static ModularServiceCollection AddMvcModules(this ModularServiceCollection moduleServices)
        {
            moduleServices.Configure(services =>
            {
                services.AddMvcCore(options =>
                {
                    options.Filters.Add(typeof(AutoValidateAntiforgeryTokenAuthorizationFilter));
                    
                    // mvcSetupAction?.Invoke(options);
                })
                .AddViews()
                .AddViewLocalization()
                .AddRazorViewEngine()
                .AddJsonFormatters();

                services.AddTransient<IFilterProvider, DependencyFilterProvider>();
                services.AddTransient<IApplicationModelProvider, ModuleAreaRouteConstraintApplicationModelProvider>();

                services.Configure<RazorViewEngineOptions>(configureOptions: options =>
                {
                    var expander = new ModuleViewLocationExpander();
                    options.ViewLocationExpanders.Add(expander);

                    var extensionLibraryService = services.BuildServiceProvider().GetService<IExtensionLibraryService>();
                    ((List<MetadataReference>)options.AdditionalCompilationReferences).AddRange(extensionLibraryService.MetadataReferences());
                });


            });

            return moduleServices;
        }
    }
}
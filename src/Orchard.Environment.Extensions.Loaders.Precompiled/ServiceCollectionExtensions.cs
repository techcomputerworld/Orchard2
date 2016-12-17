using System;
using System.Collections.Generic;


namespace Orchard.Environment.Extensions.Loaders.Precompiled
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPrecompiledExtensionLoader(this IServiceCollection services)
        {
            services.Add<IExtensionLoader, PrecompiledExtensionLoader>();

            return services;
        }
    }
}
using System;
using System.Collections.Generic;


namespace Orchard.Environment.Extensions.Loaders.Dynamic
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicExtensionLoader(this IServiceCollection services)
        {
            services.Add<IExtensionLoader, DynamicExtensionLoader>();

            return services;
        }
    }
}
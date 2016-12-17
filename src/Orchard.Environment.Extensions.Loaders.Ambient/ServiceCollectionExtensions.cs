using System;
using System.Collections.Generic;


namespace Orchard.Environment.Extensions.Loaders.Ambient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmbientExtensionLoader(this IServiceCollection services)
        {
            services.Add<IExtensionLoader, AmbientExtensionLoader>();

            return services;
        }
    }
}
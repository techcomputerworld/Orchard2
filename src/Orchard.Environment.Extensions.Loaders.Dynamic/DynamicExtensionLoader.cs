using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Orchard.Environment.Extensions.Loaders.Dynamic
{
    public class DynamicExtensionLoader : IExtensionLoader
    {
        //public static string Configuration => _configuration.Value;
        //private static readonly Lazy<string> _configuration = new Lazy<string>(GetConfiguration);

        public DynamicExtensionLoader(
            ILogger<DynamicExtensionLoader> logger)
        {
            L = logger;
        }

        private readonly ILogger L;

        public ExtensionEntry Load(IExtensionInfo extensionInfo)
        {
            Assembly assembly;

            if (TryLoadAssembly(extensionInfo, out assembly))
            {
                return new ExtensionEntry
                {
                    ExtensionInfo = extensionInfo,
                    Assembly = assembly,
                    ExportedTypes = assembly.ExportedTypes
                };
            }

            return null;
        }

        public bool TryLoadAssembly(IExtensionInfo extensionInfo, out Assembly assembly)
        {
            if (TryLoadDynamicAssembly(extensionInfo, out assembly))
            {
                LogLoadedInformation(extensionInfo, assembly);
            }

            assembly = null;

            return assembly != null;
        }

        public bool TryLoadDynamicAssembly(IExtensionInfo extensionInfo, out Assembly assembly)
        {
            // TODO : Support Dynamic Assemblies


            assembly = null;
            return false;
        }

        private void LogLoadedInformation(IExtensionInfo extensionInfo, Assembly assembly)
        {
            if (L.IsEnabled(LogLevel.Information))
            {
                L.LogInformation("Loaded referenced dynamic extension \"{0}\": assembly name=\"{1}\"", extensionInfo.Id, assembly.FullName);
            }
        }

        //private bool IsDynamicContext(ProjectContext context)
        //{
        //    if (context == null)
        //    {
        //        return false;
        //    }

        //    var compilationOptions = context.ResolveCompilationOptions(Configuration);
        //    // change Any with Count or Length? remove Linq Dependency
        //    return CompilerUtility.GetCompilationSources(context, compilationOptions).Any();
        //}

        //private static string GetConfiguration()
        //{
        //    var defines = DependencyContext.Default.CompilationOptions.Defines;
        //    return defines?.Contains(CompilerUtility.ReleaseConfiguration, StringComparer.OrdinalIgnoreCase) == true
        //        ? CompilerUtility.ReleaseConfiguration : CompilerUtility.DefaultConfiguration;
        //}
    }
}
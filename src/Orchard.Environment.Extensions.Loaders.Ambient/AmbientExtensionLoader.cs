using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Orchard.Environment.Extensions.Loaders.Ambient
{
    public class AmbientExtensionLoader : IExtensionLoader
    {
        private static HashSet<string> ApplicationAssemblyNames => 
            _applicationAssemblyNames.Value;

        private static readonly Lazy<HashSet<string>> _applicationAssemblyNames = 
            new Lazy<HashSet<string>>(GetApplicationAssemblyNames);

        public AmbientExtensionLoader(
            ILogger<AmbientExtensionLoader> logger)
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
            if (IsAmbientExtension(extensionInfo))
            {
                assembly = Assembly.Load(new AssemblyName(extensionInfo.Id));

                if (L.IsEnabled(LogLevel.Information))
                {
                    L.LogInformation("Loaded referenced ambient extension \"{0}\": assembly name=\"{1}\"", extensionInfo.Id, assembly.FullName);
                }

                return true;
            }

            assembly = null;

            return false;
        }

        private bool IsAmbientExtension(IExtensionInfo extensionInfo)
        {
            return ApplicationAssemblyNames.Contains(extensionInfo.Id);
        }
        
        private static HashSet<string> GetApplicationAssemblyNames()
        {
            return new HashSet<string>(DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.RuntimeAssemblyGroups)
                .SelectMany(assetGroup => assetGroup.AssetPaths)
                .Select(path => Path.GetFileNameWithoutExtension(path)));
        }
    }
}
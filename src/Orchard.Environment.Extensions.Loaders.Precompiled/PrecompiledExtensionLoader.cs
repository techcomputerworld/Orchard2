using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Orchard.Environment.Extensions.Loaders.Precompiled
{
    public class PrecompiledExtensionLoader : IExtensionLoader
    {
        // TODO : Use Lazy Concurrent Dictionary
        private readonly ConcurrentDictionary<string, Lazy<Assembly>> _loadedAssemblies = 
            new ConcurrentDictionary<string, Lazy<Assembly>>();

        private readonly ConcurrentDictionary<string, string> _compileOnlyAssemblies = 
            new ConcurrentDictionary<string, string>();

        public PrecompiledExtensionLoader(
            ILogger<PrecompiledExtensionLoader> logger)
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
            if (IsAssemblyLoaded(extensionInfo.Id))
            {
                assembly = GetLoadedAssembly(extensionInfo.Id);

                LogLoadedInformation(extensionInfo, assembly);

                return true;
            }

            if (TryLoadPrecompiledAssembly(extensionInfo, out assembly))
            {
                LogLoadedInformation(extensionInfo, assembly);
            }

            return assembly != null;
        }

        private bool IsAssemblyLoaded(string assemblyName)
        {
            return _loadedAssemblies.ContainsKey(assemblyName);
        }

        private Assembly GetLoadedAssembly(string assemblyName)
        {
            return _loadedAssemblies[assemblyName].Value;
        }

        private void LogLoadedInformation(IExtensionInfo extensionInfo, Assembly assembly)
        {
            if (L.IsEnabled(LogLevel.Information))
            {
                L.LogInformation("Loaded referenced ambient extension \"{0}\": assembly name=\"{1}\"", extensionInfo.Id, assembly.FullName);
            }
        }

        private bool TryLoadPrecompiledAssembly(IExtensionInfo extensionInfo, out Assembly assembly)
        {
            // TODO : Support Precompiled Assemblies
            LoadPrecompiledModule(extensionInfo);

            assembly = null;
            return false;
        }

        internal void LoadPrecompiledModule(IExtensionInfo extensionInfo)
        {
            //var fileInfo = extensionInfo.ExtensionFileInfo;
            //var assemblyFolderPath = Path.Combine(fileInfo.PhysicalPath, Constants.BinDirectoryName);
            //var assemblyPath = Path.Combine(assemblyFolderPath, CompilerUtility.GetAssemblyFileName(extensionInfo.Id));

            //// default runtime assemblies: "bin/{assembly}.dll"
            //var runtimeAssemblies = Directory.GetFiles(assemblyFolderPath,
            //    "*" + FileNameSuffixes.DotNet.DynamicLib, SearchOption.TopDirectoryOnly);

            //foreach (var asset in runtimeAssemblies)
            //{
            //    var assetName = Path.GetFileNameWithoutExtension(asset);

            //    if (!IsAmbientAssembly(assetName))
            //    {
            //        var assetFileName = Path.GetFileName(asset);
            //        var assetResolvedPath = ResolveAssemblyPath(assemblyFolderPath, assetFileName);
            //        LoadFromAssemblyPath(assetResolvedPath);

            //        PopulateBinaryFolder(assemblyFolderPath, assetResolvedPath);
            //        PopulateProbingFolder(assetResolvedPath);
            //    }
            //}

            //// compile only assemblies: "bin/refs/{assembly}.dll"
            //if (Directory.Exists(Path.Combine(assemblyFolderPath, CompilerUtility.RefsDirectoryName)))
            //{
            //    var compilationAssemblies = Directory.GetFiles(
            //        Path.Combine(assemblyFolderPath, CompilerUtility.RefsDirectoryName),
            //        "*" + FileNameSuffixes.DotNet.DynamicLib, SearchOption.TopDirectoryOnly);

            //    foreach (var asset in compilationAssemblies)
            //    {
            //        var assetFileName = Path.GetFileName(asset);
            //        var assetResolvedPath = ResolveAssemblyPath(assemblyFolderPath, assetFileName, CompilerUtility.RefsDirectoryName);

            //        _compileOnlyAssemblies[assetFileName] = assetResolvedPath;
            //        PopulateBinaryFolder(assemblyFolderPath, assetResolvedPath, CompilerUtility.RefsDirectoryName);
            //        PopulateProbingFolder(assetResolvedPath, CompilerUtility.RefsDirectoryName);
            //    }
            //}

            //// specific runtime assemblies: "bin/runtimes/{rid}/lib/{tfm}/{assembly}.dll"
            //if (Directory.Exists(Path.Combine(assemblyFolderPath, PackagingConstants.Folders.Runtimes)))
            //{
            //    var runtimeIds = GetRuntimeIdentifiers();

            //    var runtimeTargets = Directory.GetFiles(
            //        Path.Combine(assemblyFolderPath, PackagingConstants.Folders.Runtimes),
            //        "*" + FileNameSuffixes.DotNet.DynamicLib, SearchOption.AllDirectories);

            //    foreach (var asset in runtimeTargets)
            //    {
            //        var assetName = Path.GetFileNameWithoutExtension(asset);

            //        if (!IsAmbientAssembly(assetName))
            //        {
            //            var tfmPath = Paths.GetParentFolderPath(asset);
            //            var libPath = Paths.GetParentFolderPath(tfmPath);
            //            var lib = Paths.GetFolderName(libPath);

            //            if (String.Equals(lib, PackagingConstants.Folders.Lib, StringComparison.OrdinalIgnoreCase))
            //            {
            //                var tfm = Paths.GetFolderName(tfmPath);
            //                var runtime = Paths.GetParentFolderName(libPath);

            //                var relativeFolderPath = Path.Combine(PackagingConstants.Folders.Runtimes, runtime, lib, tfm);

            //                if (runtimeIds.Contains(runtime))
            //                {
            //                    LoadFromAssemblyPath(asset);
            //                }

            //                PopulateProbingFolder(asset, relativeFolderPath);
            //            }
            //        }
            //    }
            //}

            //// resource assemblies: "bin/{locale?}/{assembly}.resources.dll"
            //var resourceAssemblies = Directory.GetFiles(assemblyFolderPath, "*.resources"
            //    + FileNameSuffixes.DotNet.DynamicLib, SearchOption.AllDirectories);

            //var assemblyFolderName = Paths.GetFolderName(assemblyFolderPath);

            //foreach (var asset in resourceAssemblies)
            //{
            //    var assetFileName = Path.GetFileName(asset);
            //    var locale = Paths.GetParentFolderName(asset).Replace(assemblyFolderName, String.Empty);
            //    var assetResolvedPath = ResolveAssemblyPath(assemblyFolderPath, assetFileName, locale);

            //    PopulateBinaryFolder(assemblyFolderPath, assetResolvedPath, locale);
            //    PopulateProbingFolder(assetResolvedPath, locale);
            //    PopulateRuntimeFolder(assetResolvedPath, locale);
            //}
        }
    }
}
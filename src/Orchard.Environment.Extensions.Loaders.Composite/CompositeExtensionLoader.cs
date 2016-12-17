using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Orchard.Environment.Extensions.Loaders.Composite
{
    internal class CompositeExtensionLoader : IExtensionLoader
    {
        private readonly IExtensionLoader[] _extensionLoaders;
        public CompositeExtensionLoader(params IExtensionLoader[] extensionLoaders)
        {
            _extensionLoaders = extensionLoaders ?? new IExtensionLoader[0];
        }

        public CompositeExtensionLoader(IEnumerable<IExtensionLoader> extensionLoaders)
        {
            if (extensionLoaders == null)
            {
                throw new ArgumentNullException(nameof(extensionLoaders));
            }
            _extensionLoaders = extensionLoaders.ToArray();
        }

        public ExtensionEntry Load(IExtensionInfo extensionInfo)
        {
            foreach (var loader in _extensionLoaders)
            {
                var entry = loader.Load(extensionInfo);
                if (entry != null)
                {
                    return entry;
                }
            }

            return new ExtensionEntry { ExtensionInfo = extensionInfo, IsError = true };
        }
    }
}

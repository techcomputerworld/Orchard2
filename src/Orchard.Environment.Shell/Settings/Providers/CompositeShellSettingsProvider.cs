using System;
using System.Collections.Generic;
using System.Linq;

namespace Orchard.Environment.Shell.Settings.Providers
{
    public class CompositeShellSettingsProvider : IShellSettingsProvider
    {
        private readonly IShellSettingsProvider[] _providers;
        public CompositeShellSettingsProvider(params IShellSettingsProvider[] providers)
        {
            _providers = providers ?? new IShellSettingsProvider[0];
        }

        public CompositeShellSettingsProvider(IEnumerable<IShellSettingsProvider> providers)
        {
            if (providers == null)
            {
                throw new ArgumentNullException(nameof(providers));
            }
            _providers = providers.ToArray();
        }

        public IEnumerable<ShellSettings> LoadSettings()
        {
            var settings = new List<ShellSettings>();

            foreach (var provider in _providers)
            {
                settings.AddRange(provider.LoadSettings());
            }

            return settings;
        }

        public void SaveSettings(ShellSettings settings)
        {
            foreach (var provider in _providers)
            {
                provider.SaveSettings(settings);
            }
        }
    }
}

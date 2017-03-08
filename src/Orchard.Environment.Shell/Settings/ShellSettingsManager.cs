using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Orchard.Environment.Shell.Settings.Providers;

namespace Orchard.Environment.Shell.Settings
{
    public class ShellSettingsManager : IShellSettingsManager
    {
        private readonly IShellSettingsProvider _provider;
        private readonly ILogger _logger;

        public ShellSettingsManager(IEnumerable<IShellSettingsProvider> providers,
            ILogger<ShellSettingsManager> logger)
        {
            _provider = new CompositeShellSettingsProvider(providers);
            _logger = logger;
        }

        public IEnumerable<ShellSettings> LoadSettings()
        {
            return _provider.LoadSettings();
        }

        public void SaveSettings(ShellSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Saving ShellSettings for tenant '{0}'", settings.Name);
            }

            _provider.SaveSettings(settings);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Saved ShellSettings for tenant '{0}'", settings.Name);
            }
        }
    }
}
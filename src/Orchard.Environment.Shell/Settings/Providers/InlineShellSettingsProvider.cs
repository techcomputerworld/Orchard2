using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Orchard.Environment.Shell.Settings.Providers
{
    public class InlineShellSettingsProvider : IShellSettingsProvider
    {
        private readonly IDictionary<string, ShellSettings> _shellSettings;
        public InlineShellSettingsProvider(IOptions<TenantManifestOptions> options)
        {
            _shellSettings = options.Value.ShellSettings.ToDictionary(x => x.Name, y => y);
        }

        public IEnumerable<ShellSettings> LoadSettings()
        {
            return _shellSettings.Values;
        }

        public void SaveSettings(ShellSettings settings)
        {
            if (_shellSettings.ContainsKey(settings.Name))
            {
                _shellSettings[settings.Name] = settings;
            }
        }
    }
}

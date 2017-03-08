using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchard.Environment.Shell.Settings.Providers
{
    public interface IShellSettingsProvider
    {
        IEnumerable<ShellSettings> LoadSettings();
        void SaveSettings(ShellSettings settings);
    }

    public class PhysicalDocumentShellSettingsProvider : IShellSettingsProvider
    {
        public IEnumerable<ShellSettings> LoadSettings()
        {
            throw new NotImplementedException();
        }

        public void SaveSettings(ShellSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}

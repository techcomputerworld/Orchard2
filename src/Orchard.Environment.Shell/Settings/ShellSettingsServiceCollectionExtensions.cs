using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Orchard.Environment.Shell.Models;
using Orchard.Environment.Shell.Settings.Providers;

namespace Orchard.Environment.Shell
{
    public static class ShellSettingsServiceCollectionExtensions
    {
        public static ModularServiceCollection AddTenant(this ModularServiceCollection modules,
            string name)
        {
            return modules.AddTenant(name, cfg => { });
        }

        public static ModularServiceCollection AddTenant(this ModularServiceCollection modules,
            string name,
            Action<TenantBuilder> configure)
        {
            modules.Configure(services =>
            {
                services.TryAddScoped<IShellSettingsProvider, InlineShellSettingsProvider>();

                services.TryAddEnumerable(
                    ServiceDescriptor.Transient<IConfigureOptions<TenantManifestOptions>, TenantManifestOptionsSetup>());

                services.WithTenant(builder => {
                    builder.WithName(name);
                    configure(builder);
                    return builder.Build();
                });
            });

            return modules;
        }

        internal static IServiceCollection WithTenant(this IServiceCollection modules,
            Func<TenantBuilder, ShellSettings> tenantBuilder)
        {
            var manifest = tenantBuilder(new TenantBuilder());

            modules.Configure<TenantManifestOptions>(options =>
            {
                options.ShellSettings.Add(manifest);
            });

            return modules;
        }
    }

    public class TenantManifestOptionsSetup : ConfigureOptions<TenantManifestOptions>
    {
        public TenantManifestOptionsSetup()
            : base(options => { })
        {
        }
    }

    public class TenantBuilder {
        private string _name;
        private TenantState _state = TenantState.Uninitialized;

        public TenantBuilder WithName(string tenantName) {
            _name = tenantName;

            return this;
        }

        public TenantBuilder WithState(TenantState state) {
            _state = state;

            return this;
        }

        public ShellSettings Build() {
            return new ShellSettings
            {
                Name = _name,
                State = _state
            };
        }
    }

    public class TenantManifestOptions
    {
        public IList<ShellSettings> ShellSettings { get; set; }
            = new List<ShellSettings>();
    }
}
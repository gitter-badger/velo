using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.Settings
{
    internal sealed class SettingsResolver<TSettings> : DependencyResolver
    {
        private readonly string _path;

        public SettingsResolver(string path): base(Typeof<TSettings>.Raw)
        {
            _path = path;
        }

        protected override object ResolveInstance(Type contract, IDependencyScope scope)
        {
            var configuration = scope.GetRequiredService<IConfiguration>();
            return configuration.Get<TSettings>(_path);
        }
    }
}
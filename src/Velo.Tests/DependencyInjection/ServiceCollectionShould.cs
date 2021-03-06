using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.Extensions.DependencyInjection;
using Velo.Extensions.DependencyInjection.CQRS;
using Velo.Extensions.DependencyInjection.Serialization;
using Velo.Logging;
using Velo.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Velo.DependencyInjection
{
    public class ServiceCollectionShould : TestClass
    {
        public ServiceCollectionShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AddServicesToDependencies()
        {
            var provider = new DependencyCollection()
                .AddServiceCollection(new ServiceCollection()
                    .AddSingleton<JConverter>()
                    .AddEmitter())
                .BuildProvider();

            provider.GetRequiredService<JConverter>();
            provider.GetRequiredService<Emitter>();
        }
        
        [Fact]
        public void AddDependencyProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddDependencyProvider(dependencies => dependencies
                    .AddEmitter()
                    .AddLogging())
                .BuildServiceProvider();

            var dependencyProvider = serviceProvider.GetRequiredService<DependencyProvider>();
            dependencyProvider.GetRequiredService<Emitter>();
        }
        
        [Fact]
        public void CreateDependencyProvider()
        {
            var provider = new ServiceCollection()
                .BuildDependencyProvider();

            provider.Should().BeOfType<DependencyProvider>();
        }

        [Fact]
        public void ContainsDependencies()
        {
            var provider = new ServiceCollection()
                .AddJsonConverter()
                .BuildDependencyProvider();

            provider.GetRequiredService<JConverter>();
            provider.GetRequiredService<IConvertersCollection>();
        }
    }
}
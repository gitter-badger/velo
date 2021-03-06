using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Velo.Benchmark.DependencyInjection
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class DependencyBuildBenchmark
    {
        [BenchmarkCategory("Mixed")]
        [Benchmark]
        public string Mixed_Velo()
        {
            var builder = DependencyBuilders.ForVelo_Mixed();
            var container = builder.BuildProvider();

            return container.ToString();
        }

        [BenchmarkCategory("Mixed"), Benchmark(Baseline = true)]
        public string Mixed_Core()
        {
            var builder = DependencyBuilders.ForCore_Mixed();
            var container = builder.BuildServiceProvider();

            return container.ToString();
        }

        [BenchmarkCategory("Singleton")]
        [Benchmark]
        public string Autofac()
        {
            var builder = DependencyBuilders.ForAutofac();
            var container = builder.Build();

            return container.ToString();
        }

        [BenchmarkCategory("Singleton")]
        [Benchmark]
        public string Castle()
        {
            var container = DependencyBuilders.ForCastle();
            return container.ToString();
        }

        [BenchmarkCategory("Singleton")]
        [Benchmark(Baseline = true)]
        public string Core()
        {
            var builder = DependencyBuilders.ForCore();
            var container = builder.BuildServiceProvider();

            return container.ToString();
        }

        [BenchmarkCategory("Singleton")]
        [Benchmark]
        public string LightInject()
        {
            var container = DependencyBuilders.ForLightInject();
            return container.ToString();
        }

        [BenchmarkCategory("Singleton")]
        [Benchmark]
        public string SimpleInject()
        {
            var container = DependencyBuilders.ForSimpleInject();
            return container.ToString();
        }

        [BenchmarkCategory("Singleton")]
        [Benchmark]
        public string Velo()
        {
            var builder = DependencyBuilders.ForVelo();
            var container = builder.BuildProvider();

            return container.ToString();
        }

        [BenchmarkCategory("Singleton")]
        [Benchmark]
        public string Unity()
        {
            var container = DependencyBuilders.ForUnity();
            return container.ToString();
        }
    }
}
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace DemoClassLibraryBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = DefaultConfig
                        .Instance
                        .WithOptions(ConfigOptions.DisableOptimizationsValidator);

            var _ = BenchmarkRunner.Run<DemoClassBenchmarks>(config);
        }
    }
}

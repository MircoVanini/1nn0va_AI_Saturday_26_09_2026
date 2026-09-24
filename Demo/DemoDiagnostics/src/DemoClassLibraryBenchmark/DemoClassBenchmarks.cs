using BenchmarkDotNet.Attributes;
using DemoClassLibrary;
using Microsoft.VSDiagnostics;
using System;
using System.Security.Cryptography;

namespace DemoClassLibraryBenchmark
{
    // For more information on the VS BenchmarkDotNet Diagnosers see https://learn.microsoft.com/visualstudio/profiling/profiling-with-benchmark-dotnet
    [CPUUsageDiagnoser]
    public class DemoClassBenchmarks
    {       
        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        public string BenchmarkCreateAndFindStringOnBuffer()
        {
            return new DemoClass().CreateAndFindStringOnBuffer();
        }
    }
}

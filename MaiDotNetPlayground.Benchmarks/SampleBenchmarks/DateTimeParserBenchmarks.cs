using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaiDotNetPlayground.Benchmarks.SampleBenchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class DateTimeParsersBenchmarks
    {
        private const string DATE_STRING = "2024-07-30T17:25:09Z";
        private static readonly MaiDateTimeParser _parser = new MaiDateTimeParser();

        [Benchmark(Baseline = true)]
        public void GetYearFromDateString()
        {
            var year = _parser.GetYearFromDateString(DATE_STRING);
        }
        [Benchmark]
        public void GetYearFromDateStringV2()
        {
            var year = _parser.GetYearFromDateStringV2(DATE_STRING);
        }
    }

    /* Output result:

  // * Summary *

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
Unknown processor
.NET SDK 9.0.300
  [Host]     : .NET 8.0.17 (8.0.1725.26602), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.17 (8.0.1725.26602), X64 RyuJIT AVX2


| Method                  | Mean      | Error    | StdDev    | Ratio | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|------------------------ |----------:|---------:|----------:|------:|--------:|-----:|-------:|----------:|------------:|
| GetYearFromDateStringV2 |  59.56 ns | 3.355 ns |  9.893 ns |  0.28 |    0.05 |    1 | 0.0126 |     160 B |          NA |
| GetYearFromDateString   | 212.96 ns | 7.207 ns | 21.251 ns |  1.01 |    0.14 |    2 |      - |         - |          NA |

// * Warnings *
MultimodalDistribution
  DateTimeParsersBenchmarks.GetYearFromDateStringV2: Default -> It seems that the distribution can have several modes (mValue = 2.91)

// * Legends *
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
  Rank        : Relative position of current benchmark mean among all benchmarks (Arabic style)
  Gen0        : GC Generation 0 collects per 1000 operations
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 ns        : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:02:30 (150.95 sec), executed benchmarks: 2

Global total time: 00:02:43 (163.23 sec), executed benchmarks: 2
// * Artifacts cleanup *
Artifacts cleanup is finished
    */
}

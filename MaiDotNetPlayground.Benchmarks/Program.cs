// See https://aka.ms/new-console-template for more information
using System.Reflection;
using BenchmarkDotNet.Running;
using MaiDotNetPlayground.Benchmarks.SampleBenchmarks;

Console.WriteLine("Running Benchmark.NET...");

BenchmarkRunner.Run<DateTimeParsersBenchmarks>();

/// Different ways to run benchmarks:
//BenchmarkRunner.Run<TimersBenchmarks>();

//BenchmarkRunner.Run(new []{typeof(TimersBenchmarks), typeof(DateTimeParsersBenchmarks)});

/*
BenchmarkSwitcher
    .FromAssembly(Assembly.GetExecutingAssembly())
    .Run(args);
*/
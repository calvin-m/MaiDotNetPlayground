# MaiDotNetPlayground.Benchmarks



# How to run Benchmark:
- Compile in release mode
- run Benchmark entrypoint in release mode (without debugger).

## Build and run in Release mode commandline
- dotnet build -c release
- dotnet /Users/calvinmai/_git_repos/MaiDotNetSolutionFromMac/Benchmarks/MaiBenchmarks/bin/Release/net8.0/MaiBenchmarks.dll
- (or dotnet run -c release)

## Build and Run in Debug mode:
- dotnet build
- (or dotnet build -c debug)
- dotnet run
- (or dotnet run [assembly_path])
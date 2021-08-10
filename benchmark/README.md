# Benchmarks

## Environment

``` ini
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1110 (21H1/May2021Update)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.302
  [Host]     : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 3.1.17 (CoreCLR 4.700.21.31506, CoreFX 4.700.21.31502), X64 RyuJIT
```

## Calling method x.Next() (system cryptographic: GetBytes + BitConverter.ToInt32)

|               Method |      Mean |     Error |    StdDev |
|--------------------- |----------:|----------:|----------:|
|  SystemCryptographic | 75.783 ns | 0.4999 ns | 0.4676 ns |
| BufferedCryptoRandom | 20.384 ns | 0.0449 ns | 0.0420 ns |
|         SystemRandom |  6.819 ns | 0.0084 ns | 0.0070 ns |

## Calling method x.NextBytes() with 16 byte array

|               Method |     Mean |    Error |   StdDev |
|--------------------- |---------:|---------:|---------:|
|  SystemCryptographic | 86.94 ns | 0.168 ns | 0.140 ns |
| BufferedCryptoRandom | 42.32 ns | 0.105 ns | 0.098 ns |
|         SystemRandom | 99.77 ns | 0.195 ns | 0.182 ns |

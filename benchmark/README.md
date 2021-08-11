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

|                              Method |      Mean |     Error |    StdDev |
|------------------------------------ |----------:|----------:|----------:|
|                        SystemRandom |  6.748 ns | 0.0438 ns | 0.0388 ns |
|                 SystemCryptographic | 75.527 ns | 0.8426 ns | 0.7469 ns |
|                BufferedCryptoRandom | 18.827 ns | 0.1294 ns | 0.1080 ns |
|       BufferedCryptoRandomMaxBuffer | 18.022 ns | 0.0396 ns | 0.0351 ns |
|          BufferedCryptoRandomUnsafe |  7.547 ns | 0.0852 ns | 0.0711 ns |
| BufferedCryptoRandomMaxBufferUnsafe |  6.773 ns | 0.0294 ns | 0.0275 ns |


## Calling method x.NextBytes() with 16 byte array

|                              Method |      Mean |    Error |   StdDev |
|------------------------------------ |----------:|---------:|---------:|
|                        SystemRandom | 101.40 ns | 0.184 ns | 0.163 ns |
|                 SystemCryptographic |  87.08 ns | 0.237 ns | 0.210 ns |
|                BufferedCryptoRandom |  75.91 ns | 0.424 ns | 0.397 ns |
|       BufferedCryptoRandomMaxBuffer |  70.46 ns | 0.251 ns | 0.235 ns |
|          BufferedCryptoRandomUnsafe |  62.94 ns | 0.273 ns | 0.242 ns |
| BufferedCryptoRandomMaxBufferUnsafe |  62.25 ns | 0.228 ns | 0.213 ns |

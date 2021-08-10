# Zanaptak.BufferedCryptoRandom

[![GitHub](https://img.shields.io/badge/-github-gray?logo=github)](https://github.com/zanaptak/BufferedCryptoRandom) [![NuGet](https://img.shields.io/nuget/v/Zanaptak.BufferedCryptoRandom?logo=nuget)](https://www.nuget.org/packages/Zanaptak.BufferedCryptoRandom)

A buffered and thread-safe wrapper around the cryptographic random number generator for .NET and Fable. Compatible with the System.Random interface, plus additional convenience methods.

Normally, making many calls to the cryptographic provider for small amounts of data can impact performance. This library will request and cache larger amounts of crypto data in an internal buffer, allowing it to serve multiple subsequent calls from cache until needing to refill.

## Usage

Add the [NuGet package](https://www.nuget.org/packages/Zanaptak.BufferedCryptoRandom) to your project:
```
dotnet add package Zanaptak.BufferedCryptoRandom
```

### C#
```cs
using Zanaptak.BufferedCryptoRandom;
var random = new BufferedCryptoRandom();
var randomValue = random.Next();
var diceRoll = random.Next(1, 7);
```

### F#
```fs
open Zanaptak.BufferedCryptoRandom
let random = BufferedCryptoRandom()
let randomValue = random.Next()
let diceRoll = random.Next(1, 7)
```

## API

See the [API documentation](https://github.com/zanaptak/BufferedCryptoRandom/blob/main/doc/api.md).

## Benchmarks

See the [benchmark project](https://github.com/zanaptak/BufferedCryptoRandom/tree/main/benchmark).


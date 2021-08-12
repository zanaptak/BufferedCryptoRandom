# Zanaptak.BufferedCryptoRandom

[![GitHub](https://img.shields.io/badge/-github-gray?logo=github)](https://github.com/zanaptak/BufferedCryptoRandom) [![NuGet](https://img.shields.io/nuget/v/Zanaptak.BufferedCryptoRandom?logo=nuget)](https://www.nuget.org/packages/Zanaptak.BufferedCryptoRandom)

A buffered and thread-safe wrapper around the cryptographic random number generator for [.NET](https://dotnet.microsoft.com/) and [Fable](https://fable.io/). Compatible with the System.Random interface, plus additional convenience methods.

Because the cryptographic provider can be slower than the standard RNG, this library improves performance by using an internal buffer to serve multiple requests, only refilling from the cryptographic provider when necessary.

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
var coinFlip = random.NextBoolean();
Console.WriteLine($"{randomValue}, {diceRoll}, {coinFlip}");
```

### F#
```fs
open Zanaptak.BufferedCryptoRandom
let random = BufferedCryptoRandom()
let randomValue = random.Next()
let diceRoll = random.Next(1, 7)
let coinFlip = random.NextBoolean()
printfn $"{randomValue}, {diceRoll}, {coinFlip}"
```

## Cryptographic provider

  - For .NET, [`Cryptography.RandomNumberGenerator`](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator?view=netstandard-2.0) is used.
  - For Fable, [`window.crypto.getRandomValues`](https://developer.mozilla.org/en-US/docs/Web/API/Crypto/getRandomValues) is used if available in the runtime environment. If not, then by default an exception is thrown, but the `fableAllowNonCrypto` option can be used to fall back to [`Math.random`](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/random) in that case.
      - When compiling a Node.js app with Fable, the `ZANAPTAK_NODEJS_CRYPTO` compilation symbol can be defined to enable use of [`crypto.randomFillSync`](https://nodejs.org/api/crypto.html#crypto_crypto_randomfillsync_buffer_offset_size) (e.g. for Fable 3: `dotnet fable --define ZANAPTAK_NODEJS_CRYPTO`). This mechanism is used to hide the Node.js import during the default compilation to avoid interfering with web bundlers while still allowing the same codebase to work in both environments.

## Thread-safety

By default, the library uses exclusive locking when accessing the internal buffer for thread safety. The `threadSafe` option can be set to false to disable this for additional performance at the expense of safety.

## API

See the [API documentation](https://github.com/zanaptak/BufferedCryptoRandom/blob/main/doc/api.md).

## Benchmarks

See the [benchmark project](https://github.com/zanaptak/BufferedCryptoRandom/tree/main/benchmark).


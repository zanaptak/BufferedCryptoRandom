# BufferedCryptoRandom

A buffered and thread-safe wrapper around the .NET cryptographic random number generator, compatible with the System.Random interface.

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
```

### F#
```fs
open Zanaptak.BufferedCryptoRandom
let random = BufferedCryptoRandom()
let randomValue = random.Next()
```

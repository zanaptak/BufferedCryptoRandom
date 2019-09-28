# BufferedCryptoRandom

A buffered and thread-safe wrapper around the .NET cryptographic random number generator, compatible with the Random interface.

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

## API

| Constructor | Description |
| :--- | :--- |
| `BufferedCryptoRandom()` | Creates an instance with a 256-byte buffer. |
| `BufferedCryptoRandom(Int32)` | Creates an instance with the specified buffer size in bytes (min 16, max 65536). |

| Method | Description |
| :--- | :--- |
| `Next()` | Returns a random 32-bit signed integer greater than or equal to 0 and less than Int32.MaxValue. |
| `Next(Int32)` | Returns a random 32-bit signed integer less than the specified maximum. |
| `Next(Int32, Int32)` | Returns a random 32-bit signed integer greater than or equal to the specified minimum and less than the specified maximum. |
| `NextBytes(Byte[])` | Sets all bytes in the specified array to random bytes. |
| `NextBytes(Byte[], Int32, Int32)` | Sets the specified bytes in the specified array to random bytes. |
| `NextDouble()` | Returns a random double greater than or equal to 0.0 and less than 1.0. |
| `NextBoolean()` | Returns a random boolean. |
| `NextInt8()` | Returns a random 8-bit signed integer greater than or equal to 0 and less than SByte.MaxValue. |
| `NextInt8(SByte)` | Returns a random 8-bit signed integer less than the specified maximum. |
| `NextInt8(SByte, SByte)` | Returns a random 8-bit signed integer greater than or equal to the specified minimum and less than the specified maximum. |
| `NextInt16()` | Returns a random 16-bit signed integer greater than or equal to 0 and less than Int16.MaxValue. |
| `NextInt16(Int16)` | Returns a random 16-bit signed integer less than the specified maximum. |
| `NextInt16(Int16, Int16)` | Returns a random 16-bit signed integer greater than or equal to the specified minimum and less than the specified maximum. |
| `NextInt32()` | Returns a random 32-bit signed integer greater than or equal to 0 and less than Int32.MaxValue. |
| `NextInt32(Int32)` | Returns a random 32-bit signed integer less than the specified maximum. |
| `NextInt32(Int32, Int32)` | Returns a random 32-bit signed integer greater than or equal to the specified minimum and less than the specified maximum. |
| `NextInt64()` | Returns a random 64-bit signed integer greater than or equal to 0 and less than Int64.MaxValue. |
| `NextInt64(Int64)` | Returns a random 64-bit signed integer less than the specified maximum. |
| `NextInt64(Int64, Int64)` | Returns a random 64-bit signed integer greater than or equal to the specified minimum and less than the specified maximum. |
| `NextUInt8()` | Returns a random 8-bit unsigned integer greater than or equal to 0 and less than or equal to Byte.MaxValue. |
| `NextUInt8(Byte)` | Returns a random 8-bit unsigned integer less than the specified maximum. |
| `NextUInt8(Byte, Byte)` | Returns a random 8-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum. |
| `NextUInt16()` | Returns a random 16-bit unsigned integer greater than or equal to 0 and less than or equal to UInt16.MaxValue. |
| `NextUInt16(UInt16)` | Returns a random 16-bit unsigned integer less than the specified maximum. |
| `NextUInt16(UInt16, UInt16)` | Returns a random 16-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum. |
| `NextUInt32()` | Returns a random 32-bit unsigned integer greater than or equal to 0 and less than or equal to UInt32.MaxValue. |
| `NextUInt32(UInt32)` | Returns a random 32-bit unsigned integer less than the specified maximum. |
| `NextUInt32(UInt32, UInt32)` | Returns a random 32-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum. |
| `NextUInt64()` | Returns a random 64-bit unsigned integer greater than or equal to 0 and less than or equal to UInt64.MaxValue. |
| `NextUInt64(UInt64)` | Returns a random 64-bit unsigned integer less than the specified maximum. |
| `NextUInt64(UInt64, UInt64)` | Returns a random 64-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum. |

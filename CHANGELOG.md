# Changelog - Zanaptak.BufferedCryptoRandom

[![GitHub](https://img.shields.io/badge/-github-gray?logo=github)](https://github.com/zanaptak/BufferedCryptoRandom) [![NuGet](https://img.shields.io/nuget/v/Zanaptak.BufferedCryptoRandom?logo=nuget)](https://www.nuget.org/packages/Zanaptak.BufferedCryptoRandom)

## x.x.x (unreleased)

- __Breaking change__: Remove static Global instance (shift responsibility to user)
- Allow argumentless data-type specific methods (`NextInt8()`, etc.) to return the max value
- Add Fable support
- Enable Source Link
- Enable deterministic build

## 0.3.1 (2019-09-29)

- Fix constructor IntelliSense

## 0.3.0 (2019-09-28)

- __Breaking change__: Remove `Create` method. Constructor with integer parameter now specifies custom buffer size instead of an ignored seed parameter.
- Add NextBytes overload for array range
- Add NextBoolean
- Change NextDouble calculation to use 53 random bits (max significant for IEEE floating point) instead of 32

## 0.2.0 (2019-09-15)

- Add custom buffer size option

## 0.1.0 (2019-08-28)

- Initial release

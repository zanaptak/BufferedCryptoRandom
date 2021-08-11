# Changelog - Zanaptak.BufferedCryptoRandom

[![GitHub](https://img.shields.io/badge/-github-gray?logo=github)](https://github.com/zanaptak/BufferedCryptoRandom) [![NuGet](https://img.shields.io/nuget/v/Zanaptak.BufferedCryptoRandom?logo=nuget)](https://www.nuget.org/packages/Zanaptak.BufferedCryptoRandom)

## 1.0.1 (2021-08-11)

- Update package description

## 1.0.0 (2021-08-11)

- __Breaking change__: Remove static Global instance (shift instance management to user)
- Add Fable support
- Add option to disable thread-safety
- Allow parameterless data-type specific methods (`NextInt8()`, etc.) to return the max value
- Enable Source Link
- Enable deterministic build

## 0.3.1 (2019-09-29)

- Fix constructor tooltip

## 0.3.0 (2019-09-28)

- __Breaking change__: Remove `Create` method. Constructor with integer parameter now specifies custom buffer size instead of an ignored seed parameter.
- Add NextBytes overload for array range
- Add NextBoolean
- Change NextDouble calculation to use 53 random bits (max significant for IEEE floating point) instead of 32

## 0.2.0 (2019-09-15)

- Add custom buffer size option

## 0.1.0 (2019-08-28)

- Initial release

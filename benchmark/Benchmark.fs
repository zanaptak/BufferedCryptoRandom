open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

open System
open System.Security.Cryptography
open Zanaptak.BufferedCryptoRandom

type NextBenchmark() =
    let systemRandom = System.Random( 1234567 )
    let bufferedCryptoRandom = BufferedCryptoRandom()
    let systemCryptographic = RandomNumberGenerator.Create()
    let systemCryptographic_Bytes = Array.create 4 0uy

    [< Benchmark >]
    member this.SystemCryptographic() =
        systemCryptographic.GetBytes systemCryptographic_Bytes
        BitConverter.ToInt32( systemCryptographic_Bytes , 0 )

    [< Benchmark >]
    member this.BufferedCryptoRandom() =
        bufferedCryptoRandom.Next()

    [< Benchmark >]
    member this.SystemRandom() =
        systemRandom.Next()

type NextBytesBenchmark() =
    let systemRandom = System.Random( 1234567 )
    let bufferedCryptoRandom = BufferedCryptoRandom()
    let systemCryptographic = RandomNumberGenerator.Create()

    let systemRandom_Bytes = Array.create 16 0uy
    let bufferedCryptoRandom_Bytes = Array.create 16 0uy
    let systemCryptographic_Bytes = Array.create 16 0uy

    [< Benchmark >]
    member this.SystemCryptographic() =
        systemCryptographic.GetBytes( systemCryptographic_Bytes )

    [< Benchmark >]
    member this.BufferedCryptoRandom() =
        bufferedCryptoRandom.NextBytes( bufferedCryptoRandom_Bytes )

    [< Benchmark >]
    member this.SystemRandom() =
        systemRandom.NextBytes( systemRandom_Bytes )

[<EntryPoint>]
let main argv =
    BenchmarkSwitcher
        .FromTypes(
            [|
                typeof< NextBenchmark >
                typeof< NextBytesBenchmark >
            |]
        )
        .RunAll()
        |> ignore
    0

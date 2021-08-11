open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

open System
open System.Security.Cryptography
open Zanaptak.BufferedCryptoRandom

type NextBenchmark() =
    let systemRandom = System.Random()
    let bufferedCryptoRandom = BufferedCryptoRandom()
    let bufferedCryptoRandomMax = BufferedCryptoRandom( Int32.MaxValue )
    let bufferedCryptoRandomUnsafe = BufferedCryptoRandom( threadSafe = false )
    let bufferedCryptoRandomMaxUnsafe = BufferedCryptoRandom( Int32.MaxValue , threadSafe = false )
    let systemCryptographic = RandomNumberGenerator.Create()
    let systemCryptographic_Bytes = Array.create 4 0uy

    [< Benchmark >]
    member this.SystemRandom() =
        systemRandom.Next()

    [< Benchmark >]
    member this.SystemCryptographic() =
        systemCryptographic.GetBytes systemCryptographic_Bytes
        BitConverter.ToInt32( systemCryptographic_Bytes , 0 ) &&& Int32.MaxValue

    [< Benchmark >]
    member this.BufferedCryptoRandom() =
        bufferedCryptoRandom.Next()

    [< Benchmark >]
    member this.BufferedCryptoRandomMaxBuffer() =
        bufferedCryptoRandomMax.Next()

    [< Benchmark >]
    member this.BufferedCryptoRandomUnsafe() =
        bufferedCryptoRandomUnsafe.Next()

    [< Benchmark >]
    member this.BufferedCryptoRandomMaxBufferUnsafe() =
        bufferedCryptoRandomMaxUnsafe.Next()

type NextBytesBenchmark() =
    let systemRandom = System.Random()
    let bufferedCryptoRandom = BufferedCryptoRandom()
    let bufferedCryptoRandomMax = BufferedCryptoRandom( Int32.MaxValue )
    let bufferedCryptoRandomUnsafe = BufferedCryptoRandom( threadSafe = false )
    let bufferedCryptoRandomMaxUnsafe = BufferedCryptoRandom( Int32.MaxValue , threadSafe = false )
    let systemCryptographic = RandomNumberGenerator.Create()

    let systemRandom_Bytes = Array.create 16 0uy
    let bufferedCryptoRandom_Bytes = Array.create 16 0uy
    let systemCryptographic_Bytes = Array.create 16 0uy

    [< Benchmark >]
    member this.SystemRandom() =
        systemRandom.NextBytes( systemRandom_Bytes )

    [< Benchmark >]
    member this.SystemCryptographic() =
        systemCryptographic.GetBytes( systemCryptographic_Bytes )

    [< Benchmark >]
    member this.BufferedCryptoRandom() =
        bufferedCryptoRandom.NextBytes( bufferedCryptoRandom_Bytes )

    [< Benchmark >]
    member this.BufferedCryptoRandomMaxBuffer() =
        bufferedCryptoRandomMax.NextBytes( bufferedCryptoRandom_Bytes )

    [< Benchmark >]
    member this.BufferedCryptoRandomUnsafe() =
        bufferedCryptoRandomUnsafe.NextBytes( bufferedCryptoRandom_Bytes )

    [< Benchmark >]
    member this.BufferedCryptoRandomMaxBufferUnsafe() =
        bufferedCryptoRandomMaxUnsafe.NextBytes( bufferedCryptoRandom_Bytes )

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

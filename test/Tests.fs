module Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Zanaptak.BufferedCryptoRandom

let mainTests =
    testList "Main" [
        testCase "Next(-3,3) in range" <| fun () ->
            let rng = BufferedCryptoRandom( fableAllowNonCrypto = true )
            let actual = Seq.init 999 ( fun _ -> rng.Next( -3 , 3 ) ) |> Seq.distinct |> Seq.toArray |> Array.sort
            let expected = [| -3 ; -2 ; -1 ; 0 ; 1 ; 2 |]
            Expect.equal actual expected ""
        testCase "diff output from two values" <| fun () ->
            // infinitesimal chance of collision
            let rng = BufferedCryptoRandom( fableAllowNonCrypto = true )
            let r1 = Array.init 22 ( fun _ -> rng.Next() )
            let r2 = Array.init 22 ( fun _ -> rng.Next() )
            Expect.notEqual r1 r2 ""
        testCase "NextBytes() non-zeros" <| fun () ->
            // infinitesimal chance of multiple zeros
            let bytes : byte array = Array.zeroCreate 10
            BufferedCryptoRandom( fableAllowNonCrypto = true ).NextBytes( bytes )
            let zeroCount = bytes |> Array.filter( fun x -> x = 0uy ) |> Array.length
            Expect.isTrue ( zeroCount < 5 ) ""
        testCase "NextBytes() offset leaves zeros" <| fun () ->
            let bytes : byte array = Array.zeroCreate 10
            BufferedCryptoRandom( fableAllowNonCrypto = true ).NextBytes( bytes , 3 , 4 )
            let zeroCount = bytes |> Array.filter( fun x -> x = 0uy ) |> Array.length
            Expect.isTrue ( zeroCount > 5 ) ""
    ]

let allTests = testList "All" [
    mainTests
]

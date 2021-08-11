namespace Zanaptak.BufferedCryptoRandom

open System
open System.Security.Cryptography
open System.Runtime.InteropServices

#if FABLE_COMPILER
module private FableSupport =
    // We don't reference Fable.Core, but this will work when compiled in Fable project
    open Fable.Core
    open Fable.Core.JsInterop

    #if ZANAPTAK_NODEJS_CRYPTO
    let nodeCrypto : obj = importDefault "crypto"
    let getFillFunction _ : bool * ( byte array -> unit ) =
        true , fun bytes -> nodeCrypto?randomFillSync bytes
    #else
    let [<Global>] window: obj = jsNative
    [<Emit("typeof $0 !== 'undefined'")>]
    let isNotTypeofUndefined (x: 'a) : bool = jsNative
    let getFillFunction allowFallback : bool * ( byte array -> unit ) =
        if isNotTypeofUndefined window && window?crypto && window?crypto?getRandomValues then
            true , fun bytes -> window?crypto?getRandomValues bytes
        elif allowFallback then
            let systemRng = System.Random()
            false , fun bytes -> systemRng.NextBytes bytes
        else
            raise( NotSupportedException "Crypto RNG not supported in current environment" )
    #endif
#endif

#nowarn "1182"

/// A buffered and thread-safe wrapper around the cryptographic random number generator for .NET and Fable.
type BufferedCryptoRandom
    /// <summary>Creates a BufferedCryptoRandom instance, optionally configured using the specified parameters.</summary>
    /// <param name='bufferByteCount'>Buffer size in bytes (min 16, max 65536). Default: 1024</param>
    /// <param name='threadSafe'>Enable exclusive locking when accessing the internal buffer for thread safety. Default: true</param>
    /// <param name='fableAllowNonCrypto'>In Fable app, if environment doesn't support crypto RNG, allow fallback to non-crypto RNG. Default: false (throws exception when no crypto support)</param>
    #if ! FABLE_COMPILER
    (
        [< Optional ; DefaultParameterValue( 1024 ) >] bufferByteCount : int
        , [< Optional ; DefaultParameterValue( true ) >] threadSafe : bool
        , [< Optional ; DefaultParameterValue( false ) >] fableAllowNonCrypto : bool
    ) =
    inherit Random()
    let cryptoRng = RandomNumberGenerator.Create()
    let isCrypto = true
    let fillFromCryptoRng : byte array -> unit = fun bytes -> cryptoRng.GetBytes bytes
    #else
    (
        ?bufferByteCount : int
        , ?threadSafe : bool
        , ?fableAllowNonCrypto : bool
    ) =
    let bufferByteCount = defaultArg bufferByteCount 1024
    let threadSafe = defaultArg threadSafe true
    let fableAllowNonCrypto = defaultArg fableAllowNonCrypto false
    let isCrypto , fillFromCryptoRng = FableSupport.getFillFunction fableAllowNonCrypto
    #endif

    let lockObj = obj()

    let bufferLength = max bufferByteCount 16 |> min 65536
    let byteBuffer : byte array = Array.zeroCreate bufferLength
    let mutable bufferPos = bufferLength

    let refillBufferUnsafe() =
        fillFromCryptoRng byteBuffer
        bufferPos <- 0

    let ensureBufferUnsafe numBytes =
        if bufferLength - bufferPos < numBytes then refillBufferUnsafe()

    let copyBytesFromBufferUnsafe bytes startIndex length =
        ensureBufferUnsafe length
        Array.Copy ( byteBuffer , bufferPos , bytes , startIndex , length )
        bufferPos <- bufferPos + length

    // Fill bytes directly from crypto provider when requested byte length is >= buffer size
    let copyBytesFromCryptoProvider ( bytes : byte array ) startIndex length =
        if length = bytes.Length then
            // Requested length is same as input array length, so we are filling whole input array directly
            fillFromCryptoRng( bytes )
        else
            // Filling portion of input array, so create temp array, fill from crypto provider, and copy it over the portion
            let tempBytes = Array.create length 0uy
            fillFromCryptoRng( tempBytes )
            Array.Copy ( tempBytes , 0 , bytes , startIndex , length )

    let nextBytesUnsafe ( bytes : byte array ) startIndex length =
        if length < bufferLength then
            copyBytesFromBufferUnsafe bytes startIndex length
        else
            copyBytesFromCryptoProvider bytes startIndex length

    let nextBytesSafe ( bytes : byte array ) startIndex length =
        if length < bufferLength then
            lock lockObj ( fun() -> copyBytesFromBufferUnsafe bytes startIndex length )
        else
            copyBytesFromCryptoProvider bytes startIndex length

    let nextUInt8Unsafe() : uint8 =
        ensureBufferUnsafe 1
        let valuePos = bufferPos
        bufferPos <- bufferPos + 1
        byteBuffer.[ valuePos ]

    let nextUInt16Unsafe() =
        ensureBufferUnsafe 2
        let valuePos = bufferPos
        bufferPos <- bufferPos + 2
        BitConverter.ToUInt16( byteBuffer, valuePos )

    let nextUInt32Unsafe() =
        ensureBufferUnsafe 4
        let valuePos = bufferPos
        bufferPos <- bufferPos + 4
        BitConverter.ToUInt32( byteBuffer , valuePos )

    let nextUInt64Unsafe() =
        ensureBufferUnsafe 8
        let valuePos = bufferPos
        bufferPos <- bufferPos + 8
        BitConverter.ToUInt64( byteBuffer , valuePos )

    let nextUInt8Safe() = lock lockObj nextUInt8Unsafe
    let nextUInt16Safe() = lock lockObj nextUInt16Unsafe
    let nextUInt32Safe() = lock lockObj nextUInt32Unsafe
    let nextUInt64Safe() = lock lockObj nextUInt64Unsafe

    let nextBytes , nextUInt8 , nextUInt16 , nextUInt32 , nextUInt64 =
        if threadSafe then
            nextBytesSafe , nextUInt8Safe , nextUInt16Safe , nextUInt32Safe , nextUInt64Safe
        else
            nextBytesUnsafe , nextUInt8Unsafe , nextUInt16Unsafe , nextUInt32Unsafe , nextUInt64Unsafe

    let [< Literal >] Int32Max = 2147483647u
    let [< Literal >] Int32Threshold = 2u

    // Special case for signed int32 that excludes max value, for compatibility with System.Random
    let rec nextInt32Exclusive () =
        let r = nextUInt32 ()
        if r >= Int32Threshold then r % Int32Max |> int32 else nextInt32Exclusive ()

    let [< Literal >] Int8Max = 127uy

    let [< Literal >] Int16Max = 32767us

    let [< Literal >] Int64Max = 9223372036854775807UL

    let nextInt8 () =
        nextUInt8 () &&& Int8Max |> int8

    let nextInt16 () =
        nextUInt16 () &&& Int16Max |> int16

    let nextInt32 () =
        nextUInt32 () &&& Int32Max |> int32

    let nextInt64 () =
        nextUInt64 () &&& Int64Max |> int64

    let nextBoundedUInt8 ( maxExclusive : uint8 ) : uint8 =
        if maxExclusive > 0uy then
            let threshold = ( uint8 -( int8 maxExclusive ) ) % maxExclusive
            let rec loop () =
                let r = nextUInt8 ()
                if r >= threshold then r % maxExclusive else loop ()
            loop ()
        else 0uy

    let nextBoundedInt8 ( maxExclusive : int8 ) =
        if maxExclusive >= 0y then
            nextBoundedUInt8 ( uint8 maxExclusive ) |> int8
        else
            raise ( ArgumentException( "maxExclusive cannot be less than 0" ) )

    let nextRangeUInt8 ( minInclusive : uint8 ) ( maxExclusive : uint8 ) =
        if maxExclusive > minInclusive then
            let bound = maxExclusive - minInclusive
            minInclusive + nextBoundedUInt8 bound
        elif minInclusive = maxExclusive then minInclusive
        else raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )

    let nextRangeInt8 ( minInclusive : int8 ) ( maxExclusive : int8 ) =
        if maxExclusive > minInclusive then
            let bound = uint8 ( maxExclusive - minInclusive ) // math works due to overflow
            minInclusive + ( nextBoundedUInt8 bound |> int8 )
        elif minInclusive = maxExclusive then minInclusive
        else raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )

    let nextBoundedUInt16 ( maxExclusive : uint16 ) =
        if maxExclusive > 0us then
            let threshold = ( uint16 -( int16 maxExclusive ) ) % maxExclusive
            let rec loop () =
                let r = nextUInt16 ()
                if r >= threshold then r % maxExclusive else loop ()
            loop ()
        else 0us

    let nextBoundedInt16 ( maxExclusive : int16 ) =
        if maxExclusive >= 0s then
            nextBoundedUInt16 ( uint16 maxExclusive ) |> int16
        else
            raise ( ArgumentException( "maxExclusive cannot be less than 0" ) )

    let nextRangeUInt16 ( minInclusive : uint16 ) ( maxExclusive : uint16 ) =
        if maxExclusive > minInclusive then
            let bound = maxExclusive - minInclusive
            minInclusive + nextBoundedUInt16 bound
        elif minInclusive = maxExclusive then minInclusive
        else raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )

    let nextRangeInt16 ( minInclusive : int16 ) ( maxExclusive : int16 ) =
        if maxExclusive > minInclusive then
            let bound = uint16 ( maxExclusive - minInclusive ) // math works due to overflow
            minInclusive + ( nextBoundedUInt16 bound |> int16 )
        elif minInclusive = maxExclusive then minInclusive
        else raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )

    let nextBoundedUInt32 ( maxExclusive : uint32 ) =
        if maxExclusive > 0u then
            let threshold = ( uint32 -( int32 maxExclusive ) ) % maxExclusive
            let rec loop () =
                let r = nextUInt32 ()
                if r >= threshold then r % maxExclusive else loop ()
            loop ()
        else 0u

    let nextBoundedInt32 ( maxExclusive : int32 ) =
        if maxExclusive >= 0 then
            nextBoundedUInt32 ( uint32 maxExclusive ) |> int32
        else
            raise ( ArgumentException( "maxExclusive cannot be less than 0" ) )

    let nextRangeUInt32 ( minInclusive : uint32 ) ( maxExclusive : uint32 ) =
        if maxExclusive > minInclusive then
            let bound = maxExclusive - minInclusive
            minInclusive + nextBoundedUInt32 bound
        elif minInclusive = maxExclusive then minInclusive
        else raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )

    let nextRangeInt32 ( minInclusive : int32 ) ( maxExclusive : int32 ) =
        if maxExclusive > minInclusive then
            let bound = uint32 ( maxExclusive - minInclusive ) // math works due to overflow
            minInclusive + ( nextBoundedUInt32 bound |> int32 )
        elif minInclusive = maxExclusive then minInclusive
        else raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )

    let nextBoundedUInt64 ( maxExclusive : uint64 ) =
        if maxExclusive > 0UL then
            let threshold = ( uint64 -( int64 maxExclusive ) ) % maxExclusive
            let rec loop () =
                let r = nextUInt64 ()
                if r >= threshold then r % maxExclusive else loop ()
            loop ()
        else 0UL

    let nextBoundedInt64 ( maxExclusive : int64 ) =
        if maxExclusive >= 0L then
            nextBoundedUInt64 ( uint64 maxExclusive ) |> int64
        else
            raise ( ArgumentException( "maxExclusive cannot be less than 0" ) )

    let nextRangeUInt64 ( minInclusive : uint64 ) ( maxExclusive : uint64 ) =
        if maxExclusive > minInclusive then
            let bound = maxExclusive - minInclusive
            minInclusive + nextBoundedUInt64 bound
        elif minInclusive = maxExclusive then minInclusive
        else raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )

    let nextRangeInt64 ( minInclusive : int64 ) ( maxExclusive : int64 ) =
        if maxExclusive > minInclusive then
            let bound = uint64 ( maxExclusive - minInclusive ) // math works due to overflow
            minInclusive + ( nextBoundedUInt64 bound |> int64 )
        elif minInclusive = maxExclusive then minInclusive
        else raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )

    /// Indicates whether this instance uses exclusive locking when accessing the internal buffer for thread safety.
    member this.IsThreadSafe = threadSafe

    /// Indicates whether this instance is using the cryptographic provider. Always true in .NET. In Fable, depends on environment and the fableAllowNonCrypto option.
    member this.IsCrypto = isCrypto

    /// Returns a random 32-bit signed integer greater than or equal to 0 and less than Int32.MaxValue.
    #if FABLE_COMPILER
    member
    #else
    override
    #endif
        this.Next() = nextInt32Exclusive ()

    /// Returns a random 32-bit signed integer less than the specified maximum.
    #if FABLE_COMPILER
    member
    #else
    override
    #endif
        this.Next( maxExclusive : int ) = nextBoundedInt32 maxExclusive

    /// Returns a random 32-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.
    #if FABLE_COMPILER
    member
    #else
    override
    #endif
        this.Next( minInclusive : int , maxExclusive : int ) = nextRangeInt32 minInclusive maxExclusive

    // http://prng.di.unimi.it/
    // A standard double (64-bit) floating-point number in IEEE floating point format has 52 bits of significand, plus an implicit bit at the left of the significand.
    // Thus, the representation can actually store numbers with 53 significant binary digits.
    // Because of this fact, in C99 a 64-bit unsigned integer x should be converted to a 64-bit double using the expression
    //    #include <stdint.h>
    //    (x >> 11) * (1. / (UINT64_C(1) << 53))
    // This conversion guarantees that all dyadic rationals of the form k / 2−53 will be equally likely.
    // Note that this conversion prefers the high bits of x (usually, a good idea), but you can alternatively use the lowest bits.

    /// Returns a random double greater than or equal to 0.0 and less than 1.0 (using 53 random bits).
    #if FABLE_COMPILER
    member
    #else
    override
    #endif
        this.NextDouble() =
            float ( nextUInt64 () >>> 11 ) * 0.000000000000000111022302462515654042363166809082031250000000
            // Result with UInt64.MaxValue is: 0.999999999999999888977697537484345957636833190917968750000000

    /// Sets all bytes in the specified array to random bytes.
    #if FABLE_COMPILER
    member
    #else
    override
    #endif
        this.NextBytes( bytes : byte array ) =
            if ( not ( isNull bytes ) ) && bytes.Length > 0 then
                nextBytes bytes 0 bytes.Length
            elif isNull bytes then
                raise ( ArgumentException( "byte array cannot be null" ) )
            else ()

    /// Sets the specified bytes in the specified array to random bytes.
    member this.NextBytes( bytes : byte array , startIndex , length ) =
        if ( not ( isNull bytes ) ) && startIndex >= 0 && length > 0 && ( startIndex + length ) <= bytes.Length then
            nextBytes bytes startIndex length
        elif isNull bytes then
            raise ( ArgumentException( "byte array cannot be null" ) )
        elif startIndex < 0 || startIndex >= bytes.Length then
            raise ( ArgumentOutOfRangeException( "start index must be within array bounds" ) )
        elif ( startIndex + length ) > bytes.Length then
            raise ( ArgumentException( "length parameter must not exceed number of elements from start index to end of array" ) )
        else ()

    /// Returns a random boolean.
    member this.NextBoolean() = nextUInt8 () > Int8Max

    /// Returns a random 8-bit signed integer greater than or equal to 0 and less than or equal to SByte.MaxValue.
    member this.NextInt8() = nextInt8 ()
    /// Returns a random 8-bit signed integer less than the specified maximum.
    member this.NextInt8( maxExclusive : int8 ) = nextBoundedInt8 maxExclusive
    /// Returns a random 8-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.
    member this.NextInt8( minInclusive : int8 , maxExclusive : int8 ) = nextRangeInt8 minInclusive maxExclusive

    /// Returns a random 8-bit unsigned integer greater than or equal to 0 and less than or equal to Byte.MaxValue.
    member this.NextUInt8() = nextUInt8 ()
    /// Returns a random 8-bit unsigned integer less than the specified maximum.
    member this.NextUInt8( maxExclusive : uint8 ) = nextBoundedUInt8 maxExclusive
    /// Returns a random 8-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum.
    member this.NextUInt8( minInclusive : uint8 , maxExclusive : uint8 ) = nextRangeUInt8 minInclusive maxExclusive

    /// Returns a random 16-bit signed integer greater than or equal to 0 and less than or equal to Int16.MaxValue.
    member this.NextInt16() = nextInt16 ()
    /// Returns a random 16-bit signed integer less than the specified maximum.
    member this.NextInt16( maxExclusive : int16 ) = nextBoundedInt16 maxExclusive
    /// Returns a random 16-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.
    member this.NextInt16( minInclusive : int16 , maxExclusive : int16 ) = nextRangeInt16 minInclusive maxExclusive

    /// Returns a random 16-bit unsigned integer greater than or equal to 0 and less than or equal to UInt16.MaxValue.
    member this.NextUInt16() = nextUInt16 ()
    /// Returns a random 16-bit unsigned integer less than the specified maximum.
    member this.NextUInt16( maxExclusive : uint16 ) = nextBoundedUInt16 maxExclusive
    /// Returns a random 16-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum.
    member this.NextUInt16( minInclusive : uint16 , maxExclusive : uint16 ) = nextRangeUInt16 minInclusive maxExclusive

    /// Returns a random 32-bit signed integer greater than or equal to 0 and less than or equal to Int32.MaxValue.
    member this.NextInt32() = nextInt32 ()
    /// Returns a random 32-bit signed integer less than the specified maximum.
    member this.NextInt32( maxExclusive : int32 ) = nextBoundedInt32 maxExclusive
    /// Returns a random 32-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.
    member this.NextInt32( minInclusive : int32 , maxExclusive : int32 ) = nextRangeInt32 minInclusive maxExclusive

    /// Returns a random 32-bit unsigned integer greater than or equal to 0 and less than or equal to UInt32.MaxValue.
    member this.NextUInt32() = nextUInt32 ()
    /// Returns a random 32-bit unsigned integer less than the specified maximum.
    member this.NextUInt32( maxExclusive : uint32 ) = nextBoundedUInt32 maxExclusive
    /// Returns a random 32-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum.
    member this.NextUInt32( minInclusive : uint32 , maxExclusive : uint32 ) = nextRangeUInt32 minInclusive maxExclusive

    /// Returns a random 64-bit signed integer greater than or equal to 0 and less than or equal to Int64.MaxValue.
    member this.NextInt64() = nextInt64 ()
    /// Returns a random 64-bit signed integer less than the specified maximum.
    member this.NextInt64( maxExclusive : int64 ) = nextBoundedInt64 maxExclusive
    /// Returns a random 64-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.
    member this.NextInt64( minInclusive : int64 , maxExclusive : int64 ) = nextRangeInt64 minInclusive maxExclusive

    /// Returns a random 64-bit unsigned integer greater than or equal to 0 and less than or equal to UInt64.MaxValue.
    member this.NextUInt64() = nextUInt64 ()
    /// Returns a random 64-bit unsigned integer less than the specified maximum.
    member this.NextUInt64( maxExclusive : uint64 ) = nextBoundedUInt64 maxExclusive
    /// Returns a random 64-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum.
    member this.NextUInt64( minInclusive : uint64 , maxExclusive : uint64 ) = nextRangeUInt64 minInclusive maxExclusive

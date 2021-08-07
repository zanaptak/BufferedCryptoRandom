namespace Zanaptak.BufferedCryptoRandom

open System
open System.Security.Cryptography

type BufferedCryptoRandom private ( dummy : unit , bufferByteCount : int ) =
    // Private primary constructor since it doesn't support xml doc.

    inherit Random ()

    let cryptoRng = RandomNumberGenerator.Create()

    static let [< Literal >] defaultBufferLength = 256

    let bufferLength = max bufferByteCount 16 |> min 65536
    let byteBuffer : byte array = Array.zeroCreate bufferLength
    let mutable bufferPos = bufferLength

    let refillBuffer() =
        cryptoRng.GetBytes byteBuffer
        bufferPos <- 0

    let ensureBuffer numBytes =
        if bufferLength - bufferPos < numBytes then refillBuffer()

    let nextBytes ( bytes : byte array ) startIndex length =
        if length < bufferLength then
            lock byteBuffer.SyncRoot ( fun () ->
                ensureBuffer length
                Buffer.BlockCopy ( byteBuffer , bufferPos , bytes , startIndex , length )
                bufferPos <- bufferPos + length
            )
        else
            // Requesting more bytes than in buffer; fill from cryptoRng.
            if length = bytes.Length then
                cryptoRng.GetBytes( bytes )
            else
                let tempBytes = Array.create length 0uy
                cryptoRng.GetBytes( tempBytes )
                Buffer.BlockCopy ( tempBytes , 0 , bytes , startIndex , length )

    let nextUInt8 () : uint8 =
        lock byteBuffer.SyncRoot ( fun () ->
            ensureBuffer 1
            let valuePos = bufferPos
            bufferPos <- bufferPos + 1
            byteBuffer.[ valuePos ]
        )

    let nextUInt16 () =
        lock byteBuffer.SyncRoot ( fun () ->
            ensureBuffer 2
            let valuePos = bufferPos
            bufferPos <- bufferPos + 2
            BitConverter.ToUInt16(byteBuffer, valuePos)
        )

    let nextUInt32 () =
        lock byteBuffer.SyncRoot ( fun () ->
            ensureBuffer 4
            let valuePos = bufferPos
            bufferPos <- bufferPos + 4
            BitConverter.ToUInt32(byteBuffer, valuePos)
        )

    let nextUInt64 () =
        lock byteBuffer.SyncRoot ( fun () ->
            ensureBuffer 8
            let valuePos = bufferPos
            bufferPos <- bufferPos + 8
            BitConverter.ToUInt64(byteBuffer, valuePos)
        )

    let [< Literal >] Int8Bound = 127uy
    let [< Literal >] Int8Threshold = 2uy

    let [< Literal >] Int16Bound = 32767us
    let [< Literal >] Int16Threshold = 2us

    let [< Literal >] Int32Bound = 2147483647u
    let [< Literal >] Int32Threshold = 2u

    let [< Literal >] Int64Bound = 9223372036854775807UL
    let [< Literal >] Int64Threshold = 2UL

    let rec nextInt8 () =
        let r = nextUInt8 ()
        if r >= Int8Threshold then r % Int8Bound |> int8 else nextInt8 ()

    let rec nextInt16 () =
        let r = nextUInt16 ()
        if r >= Int16Threshold then r % Int16Bound |> int16 else nextInt16 ()

    let rec nextInt32 () =
        let r = nextUInt32 ()
        if r >= Int32Threshold then r % Int32Bound |> int32 else nextInt32 ()

    let rec nextInt64 () =
        let r = nextUInt64 ()
        if r >= Int64Threshold then r % Int64Bound |> int64 else nextInt64 ()

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

    static let globalInstance = BufferedCryptoRandom()
    static member Global = globalInstance

    /// Creates an instance with the specified buffer size in bytes (min 16, max 65536).
    new ( bufferByteCount : int ) =  BufferedCryptoRandom ( () , bufferByteCount )
    /// Creates an instance with a 256-byte buffer.
    new() = BufferedCryptoRandom( defaultBufferLength )

    /// Returns a random 32-bit signed integer greater than or equal to 0 and less than Int32.MaxValue.
    override this.Next() = nextInt32 ()
    /// Returns a random 32-bit signed integer less than the specified maximum.
    override this.Next( maxExclusive : int ) = nextBoundedInt32 maxExclusive
    /// Returns a random 32-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.
    override this.Next( minInclusive : int , maxExclusive : int ) = nextRangeInt32 minInclusive maxExclusive

    //  http://prng.di.unimi.it/
    //  A standard double (64-bit) floating-point number in IEEE floating point format has 52 bits of significand, plus an implicit bit at the left of the significand. Thus, the representation can actually store numbers with 53 significant binary digits.
    //  Because of this fact, in C99 a 64-bit unsigned integer x should be converted to a 64-bit double using the expression
    //    #include <stdint.h>
    //    (x >> 11) * (1. / (UINT64_C(1) << 53))

    /// Returns a random double greater than or equal to 0.0 and less than 1.0.
    override this.NextDouble() =
        float ( nextUInt64 () >>> 11 ) * 0.00000000000000011102230246251565

    /// Sets all bytes in the specified array to random bytes.
    override this.NextBytes( bytes : byte array ) =
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
    member this.NextBoolean() = nextUInt8 () >= 128uy

    /// Returns a random 8-bit signed integer greater than or equal to 0 and less than SByte.MaxValue.
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

    /// Returns a random 16-bit signed integer greater than or equal to 0 and less than Int16.MaxValue.
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

    /// Returns a random 32-bit signed integer greater than or equal to 0 and less than Int32.MaxValue.
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

    /// Returns a random 64-bit signed integer greater than or equal to 0 and less than Int64.MaxValue.
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

namespace Zanaptak.BufferedCryptoRandom

open System
open System.Security.Cryptography

type BufferedCryptoRandom private ( bufferByteCount : uint16 ) =
  inherit Random ()

  let cryptoRng = RandomNumberGenerator.Create()

  static let [< Literal >] defaultBufferLength = 512us

  let bufferLength = max bufferByteCount 32us |> int
  let byteBuffer : byte array = Array.zeroCreate bufferLength
  let mutable bufferPos = bufferLength

  let refillBuffer() =
    cryptoRng.GetBytes byteBuffer
    bufferPos <- 0

  let ensureBuffer numBytes =
    if bufferLength - bufferPos < numBytes then refillBuffer()

  let nextBytes ( bytes : byte array ) =
    if bytes = null then raise ( ArgumentException( "byte array cannot be null" ) )
    if bytes.Length = 0 then ()
    elif bytes.Length >= bufferLength then cryptoRng.GetBytes( bytes )
    else
      lock byteBuffer.SyncRoot ( fun () ->
        ensureBuffer bytes.Length
        Buffer.BlockCopy ( byteBuffer , bufferPos , bytes , 0 , bytes.Length )
        bufferPos <- bufferPos + bytes.Length
      )

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

  let nextBoundedUInt8 ( bound : uint8 ) : uint8 =
    if bound = 0uy then 0uy
    else
      let threshold = ( uint8 -( int8 bound ) ) % bound
      let rec loop () =
        let r = nextUInt8 ()
        if r >= threshold then r % bound else loop ()
      loop ()

  let nextBoundedInt8 ( maxExc : int8 ) =
    if maxExc < 0y then raise ( ArgumentException( "maxExclusive cannot be less than 0" ) )
    else nextBoundedUInt8 ( uint8 maxExc ) |> int8

  let nextRangeUInt8 ( minInc : uint8 ) ( maxExc : uint8 ) =
    if minInc > maxExc then raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )
    elif minInc = maxExc then minInc
    else
      let bound = maxExc - minInc
      minInc + nextBoundedUInt8 bound

  let nextRangeInt8 ( minInc : int8 ) ( maxExc : int8 ) =
    if minInc > maxExc then raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )
    elif minInc = maxExc then minInc
    else
      let bound = uint8 ( maxExc - minInc ) // math works due to overflow
      minInc + ( nextBoundedUInt8 bound |> int8 )

  let nextBoundedUInt16 ( bound : uint16 ) =
    if bound = 0us then 0us
    else
      let threshold = ( uint16 -( int16 bound ) ) % bound
      let rec loop () =
        let r = nextUInt16 ()
        if r >= threshold then r % bound else loop ()
      loop ()

  let nextBoundedInt16 ( maxExc : int16 ) =
    if maxExc < 0s then raise ( ArgumentException( "maxExclusive cannot be less than 0" ) )
    else nextBoundedUInt16 ( uint16 maxExc ) |> int16

  let nextRangeUInt16 ( minInc : uint16 ) ( maxExc : uint16 ) =
    if minInc > maxExc then raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )
    elif minInc = maxExc then minInc
    else
      let bound = maxExc - minInc
      minInc + nextBoundedUInt16 bound

  let nextRangeInt16 ( minInc : int16 ) ( maxExc : int16 ) =
    if minInc > maxExc then raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )
    elif minInc = maxExc then minInc
    else
      let bound = uint16 ( maxExc - minInc ) // math works due to overflow
      minInc + ( nextBoundedUInt16 bound |> int16 )

  let nextBoundedUInt32 ( bound : uint32 ) =
    if bound = 0u then 0u
    else
      let threshold = ( uint32 -( int32 bound ) ) % bound
      let rec loop () =
        let r = nextUInt32 ()
        if r >= threshold then r % bound else loop ()
      loop ()

  let nextBoundedInt32 ( maxExc : int32 ) =
    if maxExc < 0 then raise ( ArgumentException( "maxExclusive cannot be less than 0" ) )
    else nextBoundedUInt32 ( uint32 maxExc ) |> int32

  let nextRangeUInt32 ( minInc : uint32 ) ( maxExc : uint32 ) =
    if minInc > maxExc then raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )
    elif minInc = maxExc then minInc
    else
      let bound = maxExc - minInc
      minInc + nextBoundedUInt32 bound

  let nextRangeInt32 ( minInc : int32 ) ( maxExc : int32 ) =
    if minInc > maxExc then raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )
    elif minInc = maxExc then minInc
    else
      let bound = uint32 ( maxExc - minInc ) // math works due to overflow
      minInc + ( nextBoundedUInt32 bound |> int32 )

  let nextBoundedUInt64 ( bound : uint64 ) =
    if bound = 0UL then 0UL
    else
      let threshold = ( uint64 -( int64 bound ) ) % bound
      let rec loop () =
        let r = nextUInt64 ()
        if r >= threshold then r % bound else loop ()
      loop ()

  let nextBoundedInt64 ( maxExc : int64 ) =
    if maxExc < 0L then raise ( ArgumentException( "maxExclusive cannot be less than 0" ) )
    else nextBoundedUInt64 ( uint64 maxExc ) |> int64

  let nextRangeUInt64 ( minInc : uint64 ) ( maxExc : uint64 ) =
    if minInc > maxExc then raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )
    elif minInc = maxExc then minInc
    else
      let bound = maxExc - minInc
      minInc + nextBoundedUInt64 bound

  let nextRangeInt64 ( minInc : int64 ) ( maxExc : int64 ) =
    if minInc > maxExc then raise ( ArgumentException( "minInclusive cannot be greater than maxExclusive" ) )
    elif minInc = maxExc then minInc
    else
      let bound = uint64 ( maxExc - minInc ) // math works due to overflow
      minInc + ( nextBoundedUInt64 bound |> int64 )

  let nextDouble () =
    let rec loop () =
      let r = nextUInt32 ()
      if r < UInt32.MaxValue then float r / float UInt32.MaxValue
      else loop ()
    loop ()

  static let globalInstance = BufferedCryptoRandom()
  static member Global = globalInstance

  /// Creates an instance with a custom buffer size. Minimum value of 32 will be used if provided value is lower.
  static member Create( bufferByteCount : uint16 ) = BufferedCryptoRandom( bufferByteCount )

  /// Creates an instance with a 512-byte buffer. Seed is ingored.
  new( seedIgnored : int ) = BufferedCryptoRandom()
  /// Creates an instance with a 512-byte buffer.
  new() = BufferedCryptoRandom( defaultBufferLength )

  override this.Next() = nextInt32 ()
  override this.Next( maxExclusive : int ) = nextBoundedInt32 maxExclusive
  override this.Next( minInclusive : int , maxExclusive : int ) = nextRangeInt32 minInclusive maxExclusive

  override this.NextBytes( bytes ) = nextBytes bytes
  override this.NextDouble() = nextDouble ()

  member this.NextInt8() = nextInt8 ()
  member this.NextInt8( maxExclusive : int8 ) = nextBoundedInt8 maxExclusive
  member this.NextInt8( minInclusive : int8 , maxExclusive : int8 ) = nextRangeInt8 minInclusive maxExclusive

  member this.NextUInt8() = nextUInt8 ()
  member this.NextUInt8( maxExclusive : uint8 ) = nextBoundedUInt8 maxExclusive
  member this.NextUInt8( minInclusive : uint8 , maxExclusive : uint8 ) = nextRangeUInt8 minInclusive maxExclusive

  member this.NextInt16() = nextInt16 ()
  member this.NextInt16( maxExclusive : int16 ) = nextBoundedInt16 maxExclusive
  member this.NextInt16( minInclusive : int16 , maxExclusive : int16 ) = nextRangeInt16 minInclusive maxExclusive

  member this.NextUInt16() = nextUInt16 ()
  member this.NextUInt16( maxExclusive : uint16 ) = nextBoundedUInt16 maxExclusive
  member this.NextUInt16( minInclusive : uint16 , maxExclusive : uint16 ) = nextRangeUInt16 minInclusive maxExclusive

  member this.NextInt32() = nextInt32 ()
  member this.NextInt32( maxExclusive : int32 ) = nextBoundedInt32 maxExclusive
  member this.NextInt32( minInclusive : int32 , maxExclusive : int32 ) = nextRangeInt32 minInclusive maxExclusive

  member this.NextUInt32() = nextUInt32 ()
  member this.NextUInt32( maxExclusive : uint32 ) = nextBoundedUInt32 maxExclusive
  member this.NextUInt32( minInclusive : uint32 , maxExclusive : uint32 ) = nextRangeUInt32 minInclusive maxExclusive

  member this.NextInt64() = nextInt64 ()
  member this.NextInt64( maxExclusive : int64 ) = nextBoundedInt64 maxExclusive
  member this.NextInt64( minInclusive : int64 , maxExclusive : int64 ) = nextRangeInt64 minInclusive maxExclusive

  member this.NextUInt64() = nextUInt64 ()
  member this.NextUInt64( maxExclusive : uint64 ) = nextBoundedUInt64 maxExclusive
  member this.NextUInt64( minInclusive : uint64 , maxExclusive : uint64 ) = nextRangeUInt64 minInclusive maxExclusive

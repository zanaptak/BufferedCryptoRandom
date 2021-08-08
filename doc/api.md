## BufferedCryptoRandom Type

Namespace: Zanaptak.BufferedCryptoRandom

Assembly: Zanaptak.BufferedCryptoRandom.dll

Base Type: <code><a href="https://docs.microsoft.com/dotnet/api/system.random">Random</a></code>

A buffered and thread-safe wrapper around the cryptographic random number generator for .NET and Fable.

### Constructors

Constructor | Description
:--- | :---
<code><span>BufferedCryptoRandom<span><span>(<span><span style="white-space:nowrap">?bufferByteCount</span>,&#32;<span style="white-space:nowrap">?allowBrowserFallback</span></span>)</span></span></span></code> | Creates a BufferedCryptoRandom instance, optionally configured using the specified parameters.<br /><br />Parameters<br /><br />**?bufferByteCount**: <code>int</code><br />: Buffer size in bytes (min 16, max 65536). Default: 256<br />**?allowBrowserFallback**: <code>bool</code><br />: In browser, allow fallback to non-crypto JS PRNG if cryptographic RNG not supported. Throws exception if no support and fallback disallowed. Default: false<br /><br />Returns: <code>BufferedCryptoRandom</code><br />


### Instance members

Instance member | Description
:--- | :---
<code><span>this.Next<span>()</span></span></code> | Returns a random 32-bit signed integer greater than or equal to 0 and less than Int32.MaxValue.<br /><br />Returns: <code>int</code><br />
<code><span>this.Next<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 32-bit signed integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>int</code><br /><br />Returns: <code>int</code><br />
<code><span>this.Next<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 32-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>int</code><br />**maxExclusive**: <code>int</code><br /><br />Returns: <code>int</code><br />
<code><span>this.NextDouble<span>()</span></span></code> | Returns a random double greater than or equal to 0.0 and less than 1.0 (using 53 random bits).<br /><br />Returns: <code>float</code><br />
<code><span>this.NextBytes<span><span>(<span>bytes</span>)</span></span></span></code> | Sets all bytes in the specified array to random bytes.<br /><br />Parameters<br /><br />**bytes**: <code><span>byte&#32;array</span></code><br />
<code><span>this.NextBytes<span><span>(<span>bytes,&#32;startIndex,&#32;length</span>)</span></span></span></code> | Sets the specified bytes in the specified array to random bytes.<br /><br />Parameters<br /><br />**bytes**: <code><span>byte&#32;array</span></code><br />**startIndex**: <code>int</code><br />**length**: <code>int</code><br />
<code><span>this.NextBoolean<span>()</span></span></code> | Returns a random boolean.<br /><br />Returns: <code>bool</code><br />
<code><span>this.NextInt8<span>()</span></span></code> | Returns a random 8-bit signed integer greater than or equal to 0 and less than or equal to SByte.MaxValue.<br /><br />Returns: <code>int8</code><br />
<code><span>this.NextInt8<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 8-bit signed integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>int8</code><br /><br />Returns: <code>int8</code><br />
<code><span>this.NextInt8<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 8-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>int8</code><br />**maxExclusive**: <code>int8</code><br /><br />Returns: <code>int8</code><br />
<code><span>this.NextInt16<span>()</span></span></code> | Returns a random 16-bit signed integer greater than or equal to 0 and less than or equal to Int16.MaxValue.<br /><br />Returns: <code>int16</code><br />
<code><span>this.NextInt16<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 16-bit signed integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>int16</code><br /><br />Returns: <code>int16</code><br />
<code><span>this.NextInt16<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 16-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>int16</code><br />**maxExclusive**: <code>int16</code><br /><br />Returns: <code>int16</code><br />
<code><span>this.NextInt32<span>()</span></span></code> | Returns a random 32-bit signed integer greater than or equal to 0 and less than or equal to Int32.MaxValue.<br /><br />Returns: <code>int32</code><br />
<code><span>this.NextInt32<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 32-bit signed integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>int32</code><br /><br />Returns: <code>int32</code><br />
<code><span>this.NextInt32<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 32-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>int32</code><br />**maxExclusive**: <code>int32</code><br /><br />Returns: <code>int32</code><br />
<code><span>this.NextInt64<span>()</span></span></code> | Returns a random 64-bit signed integer greater than or equal to 0 and less than or equal to Int64.MaxValue.<br /><br />Returns: <code>int64</code><br />
<code><span>this.NextInt64<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 64-bit signed integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>int64</code><br /><br />Returns: <code>int64</code><br />
<code><span>this.NextInt64<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 64-bit signed integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>int64</code><br />**maxExclusive**: <code>int64</code><br /><br />Returns: <code>int64</code><br />
<code><span>this.NextUInt8<span>()</span></span></code> | Returns a random 8-bit unsigned integer greater than or equal to 0 and less than or equal to Byte.MaxValue.<br /><br />Returns: <code>uint8</code><br />
<code><span>this.NextUInt8<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 8-bit unsigned integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>uint8</code><br /><br />Returns: <code>uint8</code><br />
<code><span>this.NextUInt8<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 8-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>uint8</code><br />**maxExclusive**: <code>uint8</code><br /><br />Returns: <code>uint8</code><br />
<code><span>this.NextUInt16<span>()</span></span></code> | Returns a random 16-bit unsigned integer greater than or equal to 0 and less than or equal to UInt16.MaxValue.<br /><br />Returns: <code>uint16</code><br />
<code><span>this.NextUInt16<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 16-bit unsigned integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>uint16</code><br /><br />Returns: <code>uint16</code><br />
<code><span>this.NextUInt16<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 16-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>uint16</code><br />**maxExclusive**: <code>uint16</code><br /><br />Returns: <code>uint16</code><br />
<code><span>this.NextUInt32<span>()</span></span></code> | Returns a random 32-bit unsigned integer greater than or equal to 0 and less than or equal to UInt32.MaxValue.<br /><br />Returns: <code>uint32</code><br />
<code><span>this.NextUInt32<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 32-bit unsigned integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>uint32</code><br /><br />Returns: <code>uint32</code><br />
<code><span>this.NextUInt32<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 32-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>uint32</code><br />**maxExclusive**: <code>uint32</code><br /><br />Returns: <code>uint32</code><br />
<code><span>this.NextUInt64<span>()</span></span></code> | Returns a random 64-bit unsigned integer greater than or equal to 0 and less than or equal to UInt64.MaxValue.<br /><br />Returns: <code>uint64</code><br />
<code><span>this.NextUInt64<span><span>(<span>maxExclusive</span>)</span></span></span></code> | Returns a random 64-bit unsigned integer less than the specified maximum.<br /><br />Parameters<br /><br />**maxExclusive**: <code>uint64</code><br /><br />Returns: <code>uint64</code><br />
<code><span>this.NextUInt64<span><span>(<span>minInclusive,&#32;maxExclusive</span>)</span></span></span></code> | Returns a random 64-bit unsigned integer greater than or equal to the specified minimum and less than the specified maximum.<br /><br />Parameters<br /><br />**minInclusive**: <code>uint64</code><br />**maxExclusive**: <code>uint64</code><br /><br />Returns: <code>uint64</code><br />

Note that `Next()` cannot return the max integer value, for consistency with the behavior of `System.Random.Next()`. However, the argumentless data-type specific versions (`NextInt8()`, etc.) can return the full positive range including the max value.

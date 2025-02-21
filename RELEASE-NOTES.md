# Release Notes

## Current (0.1.0)

This is the first release. Changes over base Lidgren.Network:
- **Dropped .NET Framework support.** Minimum required is now .NET Standard 2.1.
- Added support for `[ReadOnly]Span<byte>` in many places.
- Fix initialization exception if loopback is the only up network interface on the system.
- Added `NetBuffer.PeekStringSize()`
- Optimize `BitsToHoldUInt[64]` to use `lzcnt` intrinsics on .NET Core >3.1.
- Minor optimizations to `NetQueue` with unmanaged types.
- Detect endianness at runtime using `BitConverter.IsLittleEndian` instead of compile directive.
- Optimized many `NetBuffer` read/write operations to use modern APIs and otherwise.
- Removed public `SingleUIntUnion` type that solely used to cast float<->uint internally.
- Optimized MTU detection code to allocate less.
- Added extra statistics to `NetPeerStatistics` and `NetConnectionStatistics`.
- Statistics are now all `long` to avoid rollover.
- Compiled with `USE_RELEASE_STATISTICS` by default.
- Added better IPv6 support to various APIs.
- Can now connect over IPv6 link-locals. Connections get rerouted to correct adapter on response receive.
- Improvements to IPv6 Dual Stack support.
- Reduced allocations in encryption code.
- Log error if sending unreliable message above MTU.
- Add support to .NET 5 `Half` on `NetBuffer`.
- Fixed encryption stuff on .NET 5.0.1 & .NET 6
- Add `NetTime.SetNow()` to synchronize any internal engine clocks you may have with Lidgren's.
- `NetBuffer` internal buffer now expands exponentially instead of linearly (similar to `List<T>`). This avoids O(n^2) scenarios when adding lots of things to a `NetBuffer`.
- Removed allocations from packet send/receive on Windows and Linux by P/Invoking native socket APIs directly instead of allocation-heavy BCL `Socket`. (BSD and macOS not currently optimized here)
- You can now port-forward non-UDP protocols with UPnP.
- Improved network interface detection (used for broadcasting and such e.g. UPnP) to be more intelligent.
- Improved UPnP to select correct IGD if multiple devices on your network implement the relevant UPnP services.
- Improve debug logging for UPnP stuff.
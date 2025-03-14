using System;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void nsIWriteSegmentFunDelegate([MarshalAs(UnmanagedType.Interface)] nsIInputStream aInStream, IntPtr aClosure, string aFromSegment, uint aToOffset, uint aCount, out uint aWriteCount);

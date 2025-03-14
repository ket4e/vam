using System;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void nsIReadSegmentFunDelegate([MarshalAs(UnmanagedType.Interface)] nsIOutputStream aInStream, IntPtr aClosure, string aFromSegment, uint aCount, out uint aWriteCount);

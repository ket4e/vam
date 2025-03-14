using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Enc;

[return: MarshalAs(UnmanagedType.Bool)]
public delegate bool ENCODECLIENTPROC(int handle, [MarshalAs(UnmanagedType.Bool)] bool connect, [In][MarshalAs(UnmanagedType.LPStr)] string client, IntPtr headers, IntPtr user);

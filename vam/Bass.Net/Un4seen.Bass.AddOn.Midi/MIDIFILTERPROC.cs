using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Midi;

[return: MarshalAs(UnmanagedType.Bool)]
public delegate bool MIDIFILTERPROC(int handle, int track, BASS_MIDI_EVENT midievent, [In][MarshalAs(UnmanagedType.Bool)] bool seeking, IntPtr user);

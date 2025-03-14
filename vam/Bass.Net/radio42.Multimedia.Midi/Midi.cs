using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace radio42.Multimedia.Midi;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class Midi
{
	private Midi()
	{
	}

	[DllImport("winmm.dll")]
	private static extern int midiInGetErrorText(int errCode, StringBuilder errMsg, int sizeOfErrMsg);

	[DllImport("winmm.dll")]
	private static extern int midiOutGetErrorText(int errCode, StringBuilder message, int sizeOfMessage);

	public static string MIDI_GetErrorText(bool input, int errCode)
	{
		StringBuilder stringBuilder = new StringBuilder(128);
		if (input)
		{
			midiInGetErrorText(errCode, stringBuilder, stringBuilder.Capacity);
		}
		else
		{
			midiOutGetErrorText(errCode, stringBuilder, stringBuilder.Capacity);
		}
		return stringBuilder.ToString();
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInGetDevCaps(IntPtr deviceID, [In][Out] MIDI_INCAPS caps, int sizeOfMidiInCaps);

	public static MIDIError MIDI_InGetDevCaps(int deviceID, MIDI_INCAPS caps)
	{
		return midiInGetDevCaps(new IntPtr(deviceID), caps, Marshal.SizeOf(typeof(MIDI_INCAPS)));
	}

	[DllImport("winmm.dll")]
	private static extern int midiInGetNumDevs();

	public static int MIDI_InGetNumDevs()
	{
		return midiInGetNumDevs();
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiOutGetDevCaps(IntPtr deviceID, [In][Out] MIDI_OUTCAPS caps, int sizeOfMidiOutCaps);

	public static MIDIError MIDI_OutGetDevCaps(int deviceID, MIDI_OUTCAPS caps)
	{
		return midiOutGetDevCaps(new IntPtr(deviceID), caps, Marshal.SizeOf(typeof(MIDI_OUTCAPS)));
	}

	[DllImport("winmm.dll")]
	private static extern int midiOutGetNumDevs();

	public static int MIDI_OutGetNumDevs()
	{
		return midiOutGetNumDevs();
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInOpen([In][Out] ref IntPtr handle, IntPtr deviceID, MIDIINPROC proc, IntPtr user, MIDIFlags flags);

	public static MIDIError MIDI_InOpen(ref IntPtr handle, int deviceID, MIDIINPROC proc, IntPtr user, MIDIFlags flags)
	{
		flags = ((proc != null) ? (flags | MIDIFlags.MIDI_CALLBACK_FUNCTION) : MIDIFlags.MIDI_CALLBACK_NULL);
		return midiInOpen(ref handle, new IntPtr(deviceID), proc, user, flags);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInClose(IntPtr handle);

	public static MIDIError MIDI_InClose(IntPtr handle)
	{
		return midiInClose(handle);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiOutOpen([In][Out] ref IntPtr handle, IntPtr deviceID, MIDIOUTPROC proc, IntPtr user, MIDIFlags flags);

	public static MIDIError MIDI_OutOpen(ref IntPtr handle, int deviceID, MIDIOUTPROC proc, IntPtr user)
	{
		MIDIFlags flags = MIDIFlags.MIDI_CALLBACK_FUNCTION;
		if (proc == null)
		{
			flags = MIDIFlags.MIDI_CALLBACK_NULL;
		}
		return midiOutOpen(ref handle, new IntPtr(deviceID), proc, user, flags);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiOutClose(IntPtr handle);

	public static MIDIError MIDI_OutClose(IntPtr handle)
	{
		return midiOutClose(handle);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiConnect(IntPtr handleA, IntPtr handleB, IntPtr reserved);

	public static MIDIError MIDI_Connect(IntPtr handleA, IntPtr handleB)
	{
		return midiConnect(handleA, handleB, IntPtr.Zero);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiDisconnect(IntPtr handleA, IntPtr handleB, IntPtr reserved);

	public static MIDIError MIDI_Disconnect(IntPtr handleA, IntPtr handleB)
	{
		return midiDisconnect(handleA, handleB, IntPtr.Zero);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInStart(IntPtr handle);

	public static MIDIError MIDI_InStart(IntPtr handle)
	{
		return midiInStart(handle);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInStop(IntPtr handle);

	public static MIDIError MIDI_InStop(IntPtr handle)
	{
		return midiInStop(handle);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInReset(IntPtr handle);

	public static MIDIError MIDI_InReset(IntPtr handle)
	{
		return midiInReset(handle);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiOutReset(IntPtr handle);

	public static MIDIError MIDI_OutReset(IntPtr handle)
	{
		return midiOutReset(handle);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiOutShortMsg(IntPtr handle, int message);

	public static MIDIError MIDI_OutShortMsg(IntPtr handle, int message)
	{
		return midiOutShortMsg(handle, message);
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiOutLongMsg(IntPtr handle, IntPtr headerPtr, int sizeOfMidiHeader);

	public static MIDIError MIDI_OutLongMsg(IntPtr handle, IntPtr headerPtr)
	{
		return midiOutLongMsg(handle, headerPtr, Marshal.SizeOf(typeof(MIDIHDR)));
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInPrepareHeader(IntPtr handle, [In][Out] IntPtr headerPtr, int sizeOfMidiHeader);

	public static MIDIError MIDI_InPrepareHeader(IntPtr handle, IntPtr headerPtr)
	{
		return midiInPrepareHeader(handle, headerPtr, Marshal.SizeOf(typeof(MIDIHDR)));
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInUnprepareHeader(IntPtr handle, [In][Out] IntPtr headerPtr, int sizeOfMidiHeader);

	public static MIDIError MIDI_InUnprepareHeader(IntPtr handle, IntPtr headerPtr)
	{
		return midiInUnprepareHeader(handle, headerPtr, Marshal.SizeOf(typeof(MIDIHDR)));
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiInAddBuffer(IntPtr handle, IntPtr headerPtr, int sizeOfMidiHeader);

	public static MIDIError MIDI_InAddBuffer(IntPtr handle, IntPtr headerPtr)
	{
		return midiInAddBuffer(handle, headerPtr, Marshal.SizeOf(typeof(MIDIHDR)));
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiOutPrepareHeader(IntPtr handle, IntPtr headerPtr, int sizeOfMidiHeader);

	public static MIDIError MIDI_OutPrepareHeader(IntPtr handle, IntPtr headerPtr)
	{
		return midiOutPrepareHeader(handle, headerPtr, Marshal.SizeOf(typeof(MIDIHDR)));
	}

	[DllImport("winmm.dll")]
	private static extern MIDIError midiOutUnprepareHeader(IntPtr handle, IntPtr headerPtr, int sizeOfMidiHeader);

	public static MIDIError MIDI_OutUnprepareHeader(IntPtr handle, IntPtr headerPtr)
	{
		return midiOutUnprepareHeader(handle, headerPtr, Marshal.SizeOf(typeof(MIDIHDR)));
	}

	[DllImport("winmm.dll")]
	private static extern int midiInMessage(IntPtr handle, MIDIMessage msg, IntPtr param1, IntPtr param2);

	public static int MIDI_InMessage(IntPtr handle, MIDIMessage msg, IntPtr param1, IntPtr param2)
	{
		return midiInMessage(handle, msg, param1, param2);
	}

	[DllImport("winmm.dll")]
	private static extern int midiOutMessage(IntPtr handle, MIDIMessage msg, IntPtr param1, IntPtr param2);

	public static int MIDI_OutMessage(IntPtr handle, MIDIMessage msg, IntPtr param1, IntPtr param2)
	{
		return midiOutMessage(handle, msg, param1, param2);
	}

	public static int MIDI_Note2Frequency(int noteNumber)
	{
		return (int)Math.Round(13.75 * Math.Pow(2.0, (double)(noteNumber - 9) / 12.0));
	}
}

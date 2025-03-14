using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Hls;

[SuppressUnmanagedCodeSecurity]
public sealed class BassHls
{
	private static int _myModuleHandle;

	private const string _myModuleName = "basshls";

	private BassHls()
	{
	}

	[DllImport("basshls", EntryPoint = "BASS_HLS_StreamCreateFile")]
	private static extern int BASS_HLS_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

	public static int BASS_HLS_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_HLS_StreamCreateFileUnicode(mem: false, file, offset, length, flags);
	}

	[DllImport("basshls", EntryPoint = "BASS_HLS_StreamCreateFile")]
	private static extern int BASS_HLS_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

	public static int BASS_HLS_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
	{
		return BASS_HLS_StreamCreateFileMemory(mem: true, memory, offset, length, flags);
	}

	[DllImport("basshls", EntryPoint = "BASS_HLS_StreamCreateURL")]
	private static extern int BASS_HLS_StreamCreateURLAscii([In][MarshalAs(UnmanagedType.LPStr)] string url, BASSFlag flags, DOWNLOADPROC proc, IntPtr user);

	[DllImport("basshls", EntryPoint = "BASS_HLS_StreamCreateURL")]
	private static extern int BASS_HLS_StreamCreateURLUnicode([In][MarshalAs(UnmanagedType.LPWStr)] string url, BASSFlag flags, DOWNLOADPROC proc, IntPtr user);

	public static int BASS_HLS_StreamCreateURL(string url, BASSFlag flags, DOWNLOADPROC proc, IntPtr user)
	{
		flags |= BASSFlag.BASS_UNICODE;
		int num = BASS_HLS_StreamCreateURLUnicode(url, flags, proc, user);
		if (num == 0)
		{
			flags &= BASSFlag.BASS_SPEAKER_PAIR15 | BASSFlag.BASS_SAMPLE_OVER_DIST | BASSFlag.BASS_AC3_DOWNMIX_DOLBY | BASSFlag.BASS_SAMPLE_8BITS | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_SAMPLE_3D | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_MUTEMAX | BASSFlag.BASS_SAMPLE_VAM | BASSFlag.BASS_SAMPLE_FX | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_RECORD_PAUSE | BASSFlag.BASS_RECORD_ECHOCANCEL | BASSFlag.BASS_RECORD_AGC | BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_STREAM_RESTRATE | BASSFlag.BASS_STREAM_BLOCK | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_STATUS | BASSFlag.BASS_SPEAKER_LEFT | BASSFlag.BASS_SPEAKER_RIGHT | BASSFlag.BASS_ASYNCFILE | BASSFlag.BASS_WV_STEREO | BASSFlag.BASS_AC3_DYNAMIC_RANGE | BASSFlag.BASS_AAC_FRAME960;
			num = BASS_HLS_StreamCreateURLAscii(url, flags, proc, user);
		}
		return num;
	}

	public static bool LoadMe()
	{
		return Utils.LoadLib("basshls", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "basshls"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

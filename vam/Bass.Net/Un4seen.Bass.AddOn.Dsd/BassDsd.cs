using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Dsd;

[SuppressUnmanagedCodeSecurity]
public sealed class BassDsd
{
	public static string SupportedStreamExtensions = "*.dsf;*.dff;*.dsd";

	public static string SupportedStreamName = "Direct Stream Digital";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "bassdsd";

	private BassDsd()
	{
	}

	[DllImport("bassdsd", EntryPoint = "BASS_DSD_StreamCreateFile")]
	private static extern int BASS_DSD_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags, int freq);

	public static int BASS_DSD_StreamCreateFile(string file, long offset, long length, BASSFlag flags, int freq)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_DSD_StreamCreateFileUnicode(mem: false, file, offset, length, flags, freq);
	}

	[DllImport("bassdsd", EntryPoint = "BASS_DSD_StreamCreateFile")]
	private static extern int BASS_DSD_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags, int freq);

	public static int BASS_DSD_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags, int freq)
	{
		return BASS_DSD_StreamCreateFileMemory(mem: true, memory, offset, length, flags, freq);
	}

	[DllImport("bassdsd")]
	public static extern int BASS_DSD_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user, int freq);

	[DllImport("bassdsd", EntryPoint = "BASS_DSD_StreamCreateURL")]
	private static extern int BASS_DSD_StreamCreateURLAscii([In][MarshalAs(UnmanagedType.LPStr)] string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user, int freq);

	[DllImport("bassdsd", EntryPoint = "BASS_DSD_StreamCreateURL")]
	private static extern int BASS_DSD_StreamCreateURLUnicode([In][MarshalAs(UnmanagedType.LPWStr)] string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user, int freq);

	public static int BASS_DSD_StreamCreateURL(string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user, int freq)
	{
		flags |= BASSFlag.BASS_UNICODE;
		int num = BASS_DSD_StreamCreateURLUnicode(url, offset, flags, proc, user, freq);
		if (num == 0)
		{
			flags &= BASSFlag.BASS_SPEAKER_PAIR15 | BASSFlag.BASS_SAMPLE_OVER_DIST | BASSFlag.BASS_AC3_DOWNMIX_DOLBY | BASSFlag.BASS_SAMPLE_8BITS | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_SAMPLE_3D | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_MUTEMAX | BASSFlag.BASS_SAMPLE_VAM | BASSFlag.BASS_SAMPLE_FX | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_RECORD_PAUSE | BASSFlag.BASS_RECORD_ECHOCANCEL | BASSFlag.BASS_RECORD_AGC | BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_STREAM_RESTRATE | BASSFlag.BASS_STREAM_BLOCK | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_STATUS | BASSFlag.BASS_SPEAKER_LEFT | BASSFlag.BASS_SPEAKER_RIGHT | BASSFlag.BASS_ASYNCFILE | BASSFlag.BASS_WV_STEREO | BASSFlag.BASS_AC3_DYNAMIC_RANGE | BASSFlag.BASS_AAC_FRAME960;
			num = BASS_DSD_StreamCreateURLAscii(url, offset, flags, proc, user, freq);
		}
		return num;
	}

	public static bool LoadMe()
	{
		return Utils.LoadLib("bassdsd", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bassdsd"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

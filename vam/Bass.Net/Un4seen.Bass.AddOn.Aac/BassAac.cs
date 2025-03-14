using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Aac;

[SuppressUnmanagedCodeSecurity]
public sealed class BassAac
{
	public static string SupportedStreamExtensions = "*.aac;*.adts;*.mp4;*.m4a;*.m4b;*.m4p";

	public static string SupportedStreamName = "Advanced Audio Codec/MP4";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "bass_aac";

	private BassAac()
	{
	}

	[DllImport("bass_aac", EntryPoint = "BASS_AAC_StreamCreateFile")]
	private static extern int BASS_AAC_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

	public static int BASS_AAC_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_AAC_StreamCreateFileUnicode(mem: false, file, offset, length, flags);
	}

	[DllImport("bass_aac", EntryPoint = "BASS_AAC_StreamCreateFile")]
	private static extern int BASS_AAC_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

	public static int BASS_AAC_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
	{
		return BASS_AAC_StreamCreateFileMemory(mem: true, memory, offset, length, flags);
	}

	[DllImport("bass_aac")]
	public static extern int BASS_AAC_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);

	[DllImport("bass_aac", EntryPoint = "BASS_AAC_StreamCreateURL")]
	private static extern int BASS_AAC_StreamCreateURLAscii([In][MarshalAs(UnmanagedType.LPStr)] string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user);

	[DllImport("bass_aac", EntryPoint = "BASS_AAC_StreamCreateURL")]
	private static extern int BASS_AAC_StreamCreateURLUnicode([In][MarshalAs(UnmanagedType.LPWStr)] string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user);

	public static int BASS_AAC_StreamCreateURL(string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user)
	{
		flags |= BASSFlag.BASS_UNICODE;
		int num = BASS_AAC_StreamCreateURLUnicode(url, offset, flags, proc, user);
		if (num == 0)
		{
			flags &= BASSFlag.BASS_SPEAKER_PAIR15 | BASSFlag.BASS_SAMPLE_OVER_DIST | BASSFlag.BASS_AC3_DOWNMIX_DOLBY | BASSFlag.BASS_SAMPLE_8BITS | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_SAMPLE_3D | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_MUTEMAX | BASSFlag.BASS_SAMPLE_VAM | BASSFlag.BASS_SAMPLE_FX | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_RECORD_PAUSE | BASSFlag.BASS_RECORD_ECHOCANCEL | BASSFlag.BASS_RECORD_AGC | BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_STREAM_RESTRATE | BASSFlag.BASS_STREAM_BLOCK | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_STATUS | BASSFlag.BASS_SPEAKER_LEFT | BASSFlag.BASS_SPEAKER_RIGHT | BASSFlag.BASS_ASYNCFILE | BASSFlag.BASS_WV_STEREO | BASSFlag.BASS_AC3_DYNAMIC_RANGE | BASSFlag.BASS_AAC_FRAME960;
			num = BASS_AAC_StreamCreateURLAscii(url, offset, flags, proc, user);
		}
		return num;
	}

	[DllImport("bass_aac", EntryPoint = "BASS_MP4_StreamCreateFile")]
	private static extern int BASS_MP4_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

	public static int BASS_MP4_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_MP4_StreamCreateFileUnicode(mem: false, file, offset, length, flags);
	}

	[DllImport("bass_aac", EntryPoint = "BASS_MP4_StreamCreateFile")]
	private static extern int BASS_MP4_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

	public static int BASS_MP4_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
	{
		return BASS_MP4_StreamCreateFileMemory(mem: true, memory, offset, length, flags);
	}

	[DllImport("bass_aac")]
	public static extern int BASS_MP4_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);

	public static bool LoadMe()
	{
		return Utils.LoadLib("bass_aac", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bass_aac"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Alac;

[SuppressUnmanagedCodeSecurity]
public sealed class BassAlac
{
	public static string SupportedStreamExtensions = "*.m4a;*.aac;*.mp4;*.mov";

	public static string SupportedStreamName = "Apple Lossless Audio";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "bassalac";

	private BassAlac()
	{
	}

	[DllImport("bassalac", EntryPoint = "BASS_ALAC_StreamCreateFile")]
	private static extern int BASS_ALAC_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

	public static int BASS_ALAC_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_ALAC_StreamCreateFileUnicode(mem: false, file, offset, length, flags);
	}

	[DllImport("bassalac", EntryPoint = "BASS_ALAC_StreamCreateFile")]
	private static extern int BASS_ALAC_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

	public static int BASS_ALAC_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
	{
		return BASS_ALAC_StreamCreateFileMemory(mem: true, memory, offset, length, flags);
	}

	[DllImport("bassalac")]
	public static extern int BASS_ALAC_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);

	[DllImport("bassalac", EntryPoint = "BASS_ALAC_StreamCreateURL")]
	private static extern int BASS_ALAC_StreamCreateURLUnicode([In][MarshalAs(UnmanagedType.LPWStr)] string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user);

	public static int BASS_ALAC_StreamCreateURL(string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_ALAC_StreamCreateURLUnicode(url, offset, flags, proc, user);
	}

	public static bool LoadMe()
	{
		return Utils.LoadLib("bassalac", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bassalac"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

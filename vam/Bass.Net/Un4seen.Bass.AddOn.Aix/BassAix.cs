using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Aix;

[SuppressUnmanagedCodeSecurity]
public sealed class BassAix
{
	public static string SupportedStreamExtensions = "*.aix;*.aixp";

	public static string SupportedStreamName = "AIX Audio";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "bass_aix";

	private BassAix()
	{
	}

	[DllImport("bass_aix", EntryPoint = "BASS_AIX_StreamCreateFile")]
	private static extern int BASS_AIX_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

	public static int BASS_AIX_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_AIX_StreamCreateFileUnicode(mem: false, file, offset, length, flags);
	}

	[DllImport("bass_aix", EntryPoint = "BASS_AIX_StreamCreateFile")]
	private static extern int BASS_AIX_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

	public static int BASS_AIX_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
	{
		return BASS_AIX_StreamCreateFileMemory(mem: true, memory, offset, length, flags);
	}

	[DllImport("bass_aix")]
	public static extern int BASS_AIX_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);

	public static bool LoadMe()
	{
		return Utils.LoadLib("bass_aix", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bass_aix"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

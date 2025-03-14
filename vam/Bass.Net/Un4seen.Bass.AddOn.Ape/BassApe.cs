using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Ape;

[SuppressUnmanagedCodeSecurity]
public sealed class BassApe
{
	public static string SupportedStreamExtensions = "*.ape;*.apl";

	public static string SupportedStreamName = "Monkey's Audio";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "bass_ape";

	private BassApe()
	{
	}

	[DllImport("bass_ape", EntryPoint = "BASS_APE_StreamCreateFile")]
	private static extern int BASS_APE_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

	public static int BASS_APE_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_APE_StreamCreateFileUnicode(mem: false, file, offset, length, flags);
	}

	[DllImport("bass_ape", EntryPoint = "BASS_APE_StreamCreateFile")]
	private static extern int BASS_APE_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

	public static int BASS_APE_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
	{
		return BASS_APE_StreamCreateFileMemory(mem: true, memory, offset, length, flags);
	}

	[DllImport("bass_ape")]
	public static extern int BASS_APE_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);

	public static bool LoadMe()
	{
		return Utils.LoadLib("bass_ape", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bass_ape"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

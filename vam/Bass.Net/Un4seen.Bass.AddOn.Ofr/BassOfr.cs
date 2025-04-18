using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Ofr;

[SuppressUnmanagedCodeSecurity]
public sealed class BassOfr
{
	public static string SupportedStreamExtensions = "*.ofr;*.ofs";

	public static string SupportedStreamName = "OptimFROG Audio";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "bass_ofr";

	private BassOfr()
	{
	}

	[DllImport("bass_ofr", EntryPoint = "BASS_OFR_StreamCreateFile")]
	private static extern int BASS_OFR_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

	public static int BASS_OFR_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_OFR_StreamCreateFileUnicode(mem: false, file, offset, length, flags);
	}

	[DllImport("bass_ofr", EntryPoint = "BASS_OFR_StreamCreateFile")]
	private static extern int BASS_OFR_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

	public static int BASS_OFR_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
	{
		return BASS_OFR_StreamCreateFileMemory(mem: true, memory, offset, length, flags);
	}

	[DllImport("bass_ofr")]
	public static extern int BASS_OFR_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);

	public static bool LoadMe()
	{
		return Utils.LoadLib("bass_ofr", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bass_ofr"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.AddOn.EncFlac;

[SuppressUnmanagedCodeSecurity]
public sealed class BassEnc_Flac
{
	public const int BASSENCFLACVERSION = 516;

	private static int _myModuleHandle;

	private const string _myModuleName = "bassenc_flac";

	private BassEnc_Flac()
	{
	}

	[DllImport("bassenc_flac")]
	public static extern int BASS_Encode_FLAC_GetVersion();

	public static Version BASS_Encode_FLAC_GetVersion(int fieldcount)
	{
		if (fieldcount < 1)
		{
			fieldcount = 1;
		}
		if (fieldcount > 4)
		{
			fieldcount = 4;
		}
		int num = BASS_Encode_FLAC_GetVersion();
		Version result = new Version(2, 4);
		switch (fieldcount)
		{
		case 1:
			result = new Version((num >> 24) & 0xFF, 0);
			break;
		case 2:
			result = new Version((num >> 24) & 0xFF, (num >> 16) & 0xFF);
			break;
		case 3:
			result = new Version((num >> 24) & 0xFF, (num >> 16) & 0xFF, (num >> 8) & 0xFF);
			break;
		case 4:
			result = new Version((num >> 24) & 0xFF, (num >> 16) & 0xFF, (num >> 8) & 0xFF, num & 0xFF);
			break;
		}
		return result;
	}

	[DllImport("bassenc_flac", EntryPoint = "BASS_Encode_FLAC_Start")]
	private static extern int BASS_Encode_FLAC_StartUnicode(int chan, [In][MarshalAs(UnmanagedType.LPWStr)] string options, BASSEncode flags, ENCODEPROC proc, IntPtr user);

	public static int BASS_Encode_FLAC_Start(int handle, string options, BASSEncode flags, ENCODEPROC proc, IntPtr user)
	{
		flags |= BASSEncode.BASS_UNICODE;
		return BASS_Encode_FLAC_StartUnicode(handle, options, flags, proc, user);
	}

	[DllImport("bassenc_flac", EntryPoint = "BASS_Encode_FLAC_StartFile")]
	private static extern int BASS_Encode_FLAC_StartFileUnicode(int chan, [In][MarshalAs(UnmanagedType.LPWStr)] string options, BASSEncode flags, [In][MarshalAs(UnmanagedType.LPWStr)] string filename);

	public static int BASS_Encode_FLAC_StartFile(int handle, string options, BASSEncode flags, string filename)
	{
		flags |= BASSEncode.BASS_UNICODE;
		return BASS_Encode_FLAC_StartFileUnicode(handle, options, flags, filename);
	}

	public static bool LoadMe()
	{
		return Utils.LoadLib("bassenc_flac", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bassenc_flac"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

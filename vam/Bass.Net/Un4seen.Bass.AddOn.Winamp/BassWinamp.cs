using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Winamp;

[SuppressUnmanagedCodeSecurity]
public sealed class BassWinamp
{
	private static int _myModuleHandle;

	private const string _myModuleName = "bass_winamp";

	private BassWinamp()
	{
	}

	[DllImport("bass_winamp")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WINAMP_GetIsSeekable(int handle);

	[DllImport("bass_winamp")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WINAMP_GetUsesOutput(int handle);

	[DllImport("bass_winamp", EntryPoint = "BASS_WINAMP_GetFileInfo")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool BASS_WINAMP_GetFileInfoIntPtr([In][MarshalAs(UnmanagedType.LPStr)] string file, IntPtr title, ref int lenms);

	public static bool BASS_WINAMP_GetFileInfo(string file, ref string title, ref int lenms)
	{
		bool flag = false;
		title = new string('\0', 255);
		GCHandle gCHandle = GCHandle.Alloc(title, GCHandleType.Pinned);
		try
		{
			flag = BASS_WINAMP_GetFileInfoIntPtr(file, gCHandle.AddrOfPinnedObject(), ref lenms);
			if (flag)
			{
				title = Utils.IntPtrAsStringAnsi(gCHandle.AddrOfPinnedObject());
			}
			else
			{
				title = string.Empty;
			}
		}
		finally
		{
			gCHandle.Free();
		}
		return flag;
	}

	[DllImport("bass_winamp")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WINAMP_InfoDlg([In][MarshalAs(UnmanagedType.LPStr)] string file, IntPtr win);

	[DllImport("bass_winamp", EntryPoint = "BASS_WINAMP_GetName")]
	private static extern IntPtr BASS_WINAMP_GetNamePtr(int handle);

	public static string BASS_WINAMP_GetName(int handle)
	{
		IntPtr intPtr = BASS_WINAMP_GetNamePtr(handle);
		if (intPtr != IntPtr.Zero)
		{
			return Utils.IntPtrAsStringAnsi(intPtr);
		}
		return null;
	}

	[DllImport("bass_winamp")]
	public static extern IntPtr BASS_WINAMP_GetExtentions(int handle);

	public static string BASS_WINAMP_GetExtentionsFilter(int handle)
	{
		IntPtr intPtr = BASS_WINAMP_GetExtentions(handle);
		if (intPtr != IntPtr.Zero)
		{
			string[] array = Utils.IntPtrToArrayNullTermAnsi(intPtr);
			if (array != null && array.Length >= 0)
			{
				string text = "";
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					text = text + text2 + "|";
				}
				return text.Substring(0, text.Length - 1);
			}
			return null;
		}
		return null;
	}

	[DllImport("bass_winamp", EntryPoint = "BASS_WINAMP_FindPlugins")]
	private static extern IntPtr BASS_WINAMP_FindPluginsPtr([In][MarshalAs(UnmanagedType.LPStr)] string pluginpath, BASSWINAMPFindPlugin flags);

	public static string[] BASS_WINAMP_FindPlugins(string pluginpath, BASSWINAMPFindPlugin flags)
	{
		flags &= ~BASSWINAMPFindPlugin.BASS_WINAMP_FIND_COMMALIST;
		IntPtr intPtr = BASS_WINAMP_FindPluginsPtr(pluginpath, flags);
		if (intPtr != IntPtr.Zero)
		{
			return Utils.IntPtrToArrayNullTermAnsi(intPtr);
		}
		return null;
	}

	[DllImport("bass_winamp")]
	public static extern int BASS_WINAMP_LoadPlugin([In][MarshalAs(UnmanagedType.LPStr)] string file);

	[DllImport("bass_winamp")]
	public static extern void BASS_WINAMP_UnloadPlugin(int handle);

	[DllImport("bass_winamp")]
	public static extern int BASS_WINAMP_GetVersion(int handle);

	[DllImport("bass_winamp")]
	public static extern void BASS_WINAMP_ConfigPlugin(int handle, IntPtr win);

	[DllImport("bass_winamp")]
	public static extern void BASS_WINAMP_AboutPlugin(int handle, IntPtr win);

	[DllImport("bass_winamp", EntryPoint = "BASS_WINAMP_StreamCreate")]
	private static extern int BASS_WINAMP_StreamCreateAnsi([In][MarshalAs(UnmanagedType.LPStr)] string file, BASSFlag flags);

	public static int BASS_WINAMP_StreamCreate(string file, BASSFlag flags)
	{
		flags &= BASSFlag.BASS_SPEAKER_PAIR15 | BASSFlag.BASS_SAMPLE_OVER_DIST | BASSFlag.BASS_AC3_DOWNMIX_DOLBY | BASSFlag.BASS_SAMPLE_8BITS | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_SAMPLE_3D | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_MUTEMAX | BASSFlag.BASS_SAMPLE_VAM | BASSFlag.BASS_SAMPLE_FX | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_RECORD_PAUSE | BASSFlag.BASS_RECORD_ECHOCANCEL | BASSFlag.BASS_RECORD_AGC | BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_STREAM_RESTRATE | BASSFlag.BASS_STREAM_BLOCK | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_STATUS | BASSFlag.BASS_SPEAKER_LEFT | BASSFlag.BASS_SPEAKER_RIGHT | BASSFlag.BASS_ASYNCFILE | BASSFlag.BASS_WV_STEREO | BASSFlag.BASS_AC3_DYNAMIC_RANGE | BASSFlag.BASS_AAC_FRAME960;
		return BASS_WINAMP_StreamCreateAnsi(file, flags);
	}

	public static bool LoadMe()
	{
		return Utils.LoadLib("bass_winamp", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bass_winamp"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

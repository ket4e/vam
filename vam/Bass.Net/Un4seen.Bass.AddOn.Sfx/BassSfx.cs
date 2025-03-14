using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Sfx;

[SuppressUnmanagedCodeSecurity]
public sealed class BassSfx
{
	public const int BASSSFXVERSION = 516;

	private static int _myModuleHandle;

	private const string _myModuleName = "bass_sfx";

	private BassSfx()
	{
	}

	[DllImport("bass_sfx")]
	public static extern int BASS_SFX_GetVersion();

	public static Version BASS_SFX_GetVersion(int fieldcount)
	{
		if (fieldcount < 1)
		{
			fieldcount = 1;
		}
		if (fieldcount > 4)
		{
			fieldcount = 4;
		}
		int num = BASS_SFX_GetVersion();
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

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_Init(IntPtr hInstance, IntPtr hWnd);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_Free();

	[DllImport("bass_sfx", EntryPoint = "BASS_SFX_PluginCreateW")]
	public static extern int BASS_SFX_PluginCreate([In][MarshalAs(UnmanagedType.LPWStr)] string file, IntPtr hWnd, int width, int height, BASSSFXFlag flags);

	[DllImport("bass_sfx")]
	public static extern BASSSFXPlugin BASS_SFX_PluginGetType(int handle);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginSetStream(int handle, int channel);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginStart(int handle);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginStop(int handle);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginFree(int handle);

	[DllImport("bass_sfx")]
	private static extern IntPtr BASS_SFX_PluginGetNameW(int handle);

	public static string BASS_SFX_PluginGetName(int handle)
	{
		IntPtr intPtr = BASS_SFX_PluginGetNameW(handle);
		if (intPtr != IntPtr.Zero)
		{
			return Marshal.PtrToStringUni(intPtr);
		}
		return null;
	}

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginConfig(int handle);

	[DllImport("bass_sfx")]
	public static extern int BASS_SFX_PluginModuleGetCount(int handle);

	[DllImport("bass_sfx")]
	private static extern IntPtr BASS_SFX_PluginModuleGetNameW(int handle, int module);

	public static string BASS_SFX_PluginModuleGetName(int handle, int module)
	{
		IntPtr intPtr = BASS_SFX_PluginModuleGetNameW(handle, module);
		if (intPtr != IntPtr.Zero)
		{
			return Marshal.PtrToStringUni(intPtr);
		}
		return null;
	}

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginModuleSetActive(int handle, int module);

	[DllImport("bass_sfx")]
	public static extern int BASS_SFX_PluginModuleGetActive(int handle);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginRender(int handle, int channel, IntPtr hDC);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginClicked(int handle, int x, int y);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginResize(int handle, int width, int height);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_SFX_PluginResizeMove(int handle, int left, int top, int width, int height);

	[DllImport("bass_sfx")]
	public static extern BASSSFXFlag BASS_SFX_PluginFlags(int handle, BASSSFXFlag flags, BASSSFXFlag mask);

	[DllImport("bass_sfx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool BASS_SFX_WMP_GetPluginW(int index, [In][Out] ref BASS_SFX_PLUGININFO_INTERNAL info);

	public static bool BASS_SFX_WMP_GetPlugin(int index, BASS_SFX_PLUGININFO info)
	{
		bool num = BASS_SFX_WMP_GetPluginW(index, ref info._internal);
		if (num)
		{
			info.name = Marshal.PtrToStringUni(info._internal.name);
			info.clsid = Marshal.PtrToStringUni(info._internal.clsid);
		}
		return num;
	}

	public static BASS_SFX_PLUGININFO BASS_SFX_WMP_GetPlugin(int index)
	{
		BASS_SFX_PLUGININFO bASS_SFX_PLUGININFO = new BASS_SFX_PLUGININFO();
		if (BASS_SFX_WMP_GetPlugin(index, bASS_SFX_PLUGININFO))
		{
			return bASS_SFX_PLUGININFO;
		}
		return null;
	}

	public static int BASS_SFX_WMP_GetPluginCount()
	{
		BASS_SFX_PLUGININFO info = new BASS_SFX_PLUGININFO();
		int i;
		for (i = 0; BASS_SFX_WMP_GetPlugin(i, info); i++)
		{
		}
		BASS_SFX_GetVersion();
		return i;
	}

	[DllImport("bass_sfx")]
	public static extern BASSSFXError BASS_SFX_ErrorGetCode();

	public static bool LoadMe()
	{
		return Utils.LoadLib("bass_sfx", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bass_sfx"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

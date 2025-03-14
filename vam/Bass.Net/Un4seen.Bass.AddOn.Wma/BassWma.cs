using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Un4seen.Bass.AddOn.Wma;

[SuppressUnmanagedCodeSecurity]
public sealed class BassWma
{
	public static string SupportedStreamExtensions = "*.wma;*.wmv";

	public static string SupportedStreamName = "Windows Media Audio";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "basswma";

	public static bool BASS_WMA_IsDRMVersion => false;

	private BassWma()
	{
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_StreamCreateFile")]
	private static extern int BASS_WMA_StreamCreateFileIStream(int mem, IntPtr istream, long offset, long length, BASSFlag flags);

	public static int BASS_WMA_StreamCreateIStream(IntPtr istream, long offset, long length, BASSFlag flags)
	{
		return BASS_WMA_StreamCreateFileIStream(2, istream, offset, length, flags);
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_StreamCreateFile")]
	private static extern int BASS_WMA_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

	public static int BASS_WMA_StreamCreateFile(string file, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_WMA_StreamCreateFileUnicode(mem: false, file, offset, length, flags);
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_StreamCreateFile")]
	private static extern int BASS_WMA_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

	public static int BASS_WMA_StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
	{
		return BASS_WMA_StreamCreateFileMemory(mem: true, memory, offset, length, flags);
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_StreamCreateFileAuth")]
	private static extern int BASS_WMA_StreamCreateFileAuthIStream(int mem, IntPtr istream, long offset, long length, BASSFlag flags, [In][MarshalAs(UnmanagedType.LPWStr)] string user, [In][MarshalAs(UnmanagedType.LPWStr)] string pass);

	public static int BASS_WMA_StreamCreateIStreamAuth(IntPtr istream, long offset, long length, BASSFlag flags, string user, string pass)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_WMA_StreamCreateFileAuthIStream(2, istream, offset, length, flags, user, pass);
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_StreamCreateFileAuth")]
	private static extern int BASS_WMA_StreamCreateFileAuthUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In][MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags, [In][MarshalAs(UnmanagedType.LPWStr)] string user, [In][MarshalAs(UnmanagedType.LPWStr)] string pass);

	public static int BASS_WMA_StreamCreateFileAuth(string file, long offset, long length, BASSFlag flags, string user, string pass)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_WMA_StreamCreateFileAuthUnicode(mem: false, file, offset, length, flags, user, pass);
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_StreamCreateFileAuth")]
	private static extern int BASS_WMA_StreamCreateFileAuthMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags, [In][MarshalAs(UnmanagedType.LPWStr)] string user, [In][MarshalAs(UnmanagedType.LPWStr)] string pass);

	public static int BASS_WMA_StreamCreateFileAuth(IntPtr memory, long offset, long length, BASSFlag flags, string user, string pass)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_WMA_StreamCreateFileAuthMemory(mem: true, memory, offset, length, flags, user, pass);
	}

	[DllImport("basswma")]
	public static extern int BASS_WMA_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);

	public static int BASS_WMA_StreamCreateURL(string url, long offset, long length, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_WMA_StreamCreateFileUnicode(mem: false, url, offset, length, flags);
	}

	[DllImport("basswma")]
	public static extern IntPtr BASS_WMA_GetWMObject(int handle);

	[DllImport("basswma")]
	private static extern IntPtr BASS_WMA_GetTags([In][MarshalAs(UnmanagedType.LPWStr)] string file, BASSFlag flags);

	public static IntPtr BASS_WMA_GetTags(string file)
	{
		return BASS_WMA_GetTags(file, BASSFlag.BASS_UNICODE);
	}

	public static string[] BASS_WMA_GetTagsArray(string file)
	{
		return Utils.IntPtrToArrayNullTermUtf8(BASS_WMA_GetTags(file));
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_EncodeGetRates")]
	private static extern IntPtr BASS_WMA_EncodeGetRatesPtr(int freq, int chans, BASSWMAEncode flags);

	public static int[] BASS_WMA_EncodeGetRates(int freq, int chans, BASSWMAEncode flags)
	{
		return Utils.IntPtrToArrayNullTermInt32(BASS_WMA_EncodeGetRatesPtr(freq, chans, flags));
	}

	[DllImport("basswma")]
	public static extern int BASS_WMA_EncodeOpen(int freq, int chans, BASSWMAEncode flags, int bitrate, WMENCODEPROC proc, IntPtr user);

	[DllImport("basswma", EntryPoint = "BASS_WMA_EncodeOpenFile")]
	private static extern int BASS_WMA_EncodeOpenFileUnicode(int freq, int chans, BASSWMAEncode flags, int bitrate, [In][MarshalAs(UnmanagedType.LPWStr)] string file);

	public static int BASS_WMA_EncodeOpenFile(int freq, int chans, BASSWMAEncode flags, int bitrate, string file)
	{
		flags |= BASSWMAEncode.BASS_UNICODE;
		return BASS_WMA_EncodeOpenFileUnicode(freq, chans, flags, bitrate, file);
	}

	[DllImport("basswma")]
	public static extern int BASS_WMA_EncodeOpenNetwork(int freq, int chans, BASSWMAEncode flags, int bitrate, int port, int clients);

	[DllImport("basswma")]
	public static extern int BASS_WMA_EncodeOpenNetworkMulti(int freq, int chans, BASSWMAEncode flags, int[] bitrates, int port, int clients);

	[DllImport("basswma", EntryPoint = "BASS_WMA_EncodeOpenPublish")]
	private static extern int BASS_WMA_EncodeOpenPublishUnicode(int freq, int chans, BASSWMAEncode flags, int bitrate, [In][MarshalAs(UnmanagedType.LPWStr)] string url, [In][MarshalAs(UnmanagedType.LPWStr)] string user, [In][MarshalAs(UnmanagedType.LPWStr)] string pass);

	public static int BASS_WMA_EncodeOpenPublish(int freq, int chans, BASSWMAEncode flags, int bitrate, string url, string user, string pass)
	{
		flags |= BASSWMAEncode.BASS_UNICODE;
		return BASS_WMA_EncodeOpenPublishUnicode(freq, chans, flags, bitrate, url, user, pass);
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_EncodeOpenPublishMulti")]
	private static extern int BASS_WMA_EncodeOpenPublishMultiUnicode(int freq, int chans, BASSWMAEncode flags, int[] bitrates, [In][MarshalAs(UnmanagedType.LPWStr)] string url, [In][MarshalAs(UnmanagedType.LPWStr)] string user, [In][MarshalAs(UnmanagedType.LPWStr)] string pass);

	public static int BASS_WMA_EncodeOpenPublishMulti(int freq, int chans, BASSWMAEncode flags, int[] bitrates, string url, string user, string pass)
	{
		flags |= BASSWMAEncode.BASS_UNICODE;
		return BASS_WMA_EncodeOpenPublishMultiUnicode(freq, chans, flags, bitrates, url, user, pass);
	}

	[DllImport("basswma")]
	public static extern int BASS_WMA_EncodeGetPort(int handle);

	[DllImport("basswma")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WMA_EncodeSetNotify(int handle, CLIENTCONNECTPROC proc, IntPtr user);

	[DllImport("basswma")]
	public static extern int BASS_WMA_EncodeGetClients(int handle);

	[DllImport("basswma")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WMA_EncodeSetTag(int handle, IntPtr tag, IntPtr value, BASSWMATag type);

	[DllImport("basswma", EntryPoint = "BASS_WMA_EncodeSetTag")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool BASS_WMA_EncodeSetTagUnicode(int handle, [In][MarshalAs(UnmanagedType.LPWStr)] string tag, [In][MarshalAs(UnmanagedType.LPWStr)] string value, BASSWMATag type);

	public unsafe static bool BASS_WMA_EncodeSetTag(int handle, string tag, string value, BASSWMATag type)
	{
		if (handle == 0)
		{
			return false;
		}
		if (tag == null && value == null)
		{
			return BASS_WMA_EncodeSetTag(handle, IntPtr.Zero, IntPtr.Zero, type);
		}
		if (tag == null)
		{
			tag = string.Empty;
		}
		else if (value == null)
		{
			value = string.Empty;
		}
		if (type == BASSWMATag.BASS_WMA_TAG_ANSI)
		{
			type = BASSWMATag.BASS_WMA_TAG_UNICODE;
		}
		if (type == BASSWMATag.BASS_WMA_TAG_UNICODE)
		{
			return BASS_WMA_EncodeSetTagUnicode(handle, tag, value, type);
		}
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(tag);
		byte[] bytes2 = uTF8Encoding.GetBytes(value);
		bool result;
		fixed (byte* ptr = bytes)
		{
			fixed (byte* ptr2 = bytes2)
			{
				result = BASS_WMA_EncodeSetTag(handle, (IntPtr)ptr, (IntPtr)ptr2, type);
			}
		}
		return result;
	}

	[DllImport("basswma", EntryPoint = "BASS_WMA_EncodeSetTag")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool BASS_WMA_EncodeSetTagUnicode(int handle, [In][MarshalAs(UnmanagedType.LPWStr)] string tag, IntPtr value, BASSWMATag type);

	public static bool BASS_WMA_EncodeSetTag(int handle, string tag, IntPtr value, int length)
	{
		if (handle == 0)
		{
			return false;
		}
		if (tag == null && value == IntPtr.Zero)
		{
			return BASS_WMA_EncodeSetTag(handle, IntPtr.Zero, IntPtr.Zero, BASSWMATag.BASS_WMA_TAG_UNICODE);
		}
		if (tag == null)
		{
			tag = string.Empty;
		}
		BASSWMATag type = (BASSWMATag)(1 | Utils.MakeLong(256, length));
		return BASS_WMA_EncodeSetTagUnicode(handle, tag, value, type);
	}

	public static bool BASS_WMA_EncodeSetTag(int handle, string tag, string value)
	{
		if (handle == 0)
		{
			return false;
		}
		if (tag == null && value == null)
		{
			return BASS_WMA_EncodeSetTag(handle, IntPtr.Zero, IntPtr.Zero, BASSWMATag.BASS_WMA_TAG_UNICODE);
		}
		if (tag == null)
		{
			tag = string.Empty;
		}
		else if (value == null)
		{
			value = string.Empty;
		}
		return BASS_WMA_EncodeSetTagUnicode(handle, tag, value, BASSWMATag.BASS_WMA_TAG_UNICODE);
	}

	[DllImport("basswma")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WMA_EncodeWrite(int handle, IntPtr buffer, int length);

	[DllImport("basswma")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WMA_EncodeWrite(int handle, float[] buffer, int length);

	[DllImport("basswma")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WMA_EncodeWrite(int handle, int[] buffer, int length);

	[DllImport("basswma")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WMA_EncodeWrite(int handle, short[] buffer, int length);

	[DllImport("basswma")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_WMA_EncodeWrite(int handle, byte[] buffer, int length);

	[DllImport("basswma")]
	public static extern void BASS_WMA_EncodeClose(int handle);

	public static bool LoadMe()
	{
		return Utils.LoadLib("basswma", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "basswma"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

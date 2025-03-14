using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Un4seen.Bass.AddOn.Enc;

[SuppressUnmanagedCodeSecurity]
public sealed class BassEnc
{
	public const int BASSENCVERSION = 516;

	public static string BASS_ENCODE_TYPE_MP3 = "audio/mpeg";

	public static string BASS_ENCODE_TYPE_OGG = "application/ogg";

	public static string BASS_ENCODE_TYPE_AAC = "audio/aacp";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "bassenc";

	private BassEnc()
	{
	}

	[DllImport("bassenc")]
	public static extern int BASS_Encode_GetVersion();

	public static Version BASS_Encode_GetVersion(int fieldcount)
	{
		if (fieldcount < 1)
		{
			fieldcount = 1;
		}
		if (fieldcount > 4)
		{
			fieldcount = 4;
		}
		int num = BASS_Encode_GetVersion();
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

	[DllImport("bassenc", EntryPoint = "BASS_Encode_Start")]
	private static extern int BASS_Encode_StartUnicode(int chan, [In][MarshalAs(UnmanagedType.LPWStr)] string cmdline, BASSEncode flags, ENCODEPROC proc, IntPtr user);

	public static int BASS_Encode_Start(int handle, string cmdline, BASSEncode flags, ENCODEPROC proc, IntPtr user)
	{
		flags |= BASSEncode.BASS_UNICODE;
		return BASS_Encode_StartUnicode(handle, cmdline, flags, proc, user);
	}

	[DllImport("bassenc", EntryPoint = "BASS_Encode_StartLimit")]
	private static extern int BASS_Encode_StartUnicode(int chan, [In][MarshalAs(UnmanagedType.LPWStr)] string cmdline, BASSEncode flags, ENCODEPROC proc, IntPtr user, int limit);

	public static int BASS_Encode_Start(int handle, string cmdline, BASSEncode flags, ENCODEPROC proc, IntPtr user, int limit)
	{
		flags |= BASSEncode.BASS_UNICODE;
		return BASS_Encode_StartUnicode(handle, cmdline, flags, proc, user, limit);
	}

	[DllImport("bassenc", EntryPoint = "BASS_Encode_StartUser")]
	private static extern int BASS_Encode_StartUserUnicode(int chan, [In][MarshalAs(UnmanagedType.LPWStr)] string filename, BASSEncode flags, ENCODERPROC proc, IntPtr user);

	public static int BASS_Encode_StartUser(int handle, string filename, BASSEncode flags, ENCODERPROC proc, IntPtr user)
	{
		flags |= BASSEncode.BASS_UNICODE;
		return BASS_Encode_StartUserUnicode(handle, filename, flags, proc, user);
	}

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_AddChunk(int handle, [In][MarshalAs(UnmanagedType.LPStr)] string id, IntPtr buffer, int length);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_AddChunk(int handle, [In][MarshalAs(UnmanagedType.LPStr)] string id, byte[] buffer, int length);

	[DllImport("bassenc")]
	public static extern BASSActive BASS_Encode_IsActive(int handle);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_Stop(int handle);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_StopEx(int handle, [MarshalAs(UnmanagedType.Bool)] bool queue);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_SetPaused(int handle, [In][MarshalAs(UnmanagedType.Bool)] bool paused);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_Write(int handle, IntPtr buffer, int length);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_Write(int handle, float[] buffer, int length);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_Write(int handle, int[] buffer, int length);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_Write(int handle, short[] buffer, int length);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_Write(int handle, byte[] buffer, int length);

	[DllImport("bassenc")]
	public static extern long BASS_Encode_GetCount(int handle, BASSEncodeCount count);

	[DllImport("bassenc", EntryPoint = "BASS_Encode_GetACMFormat")]
	private static extern int BASS_Encode_GetACMFormatUnicode(int handle, IntPtr form, int fromlen, [In][MarshalAs(UnmanagedType.LPWStr)] string title, BASSACMFormat flags);

	public static int BASS_Encode_GetACMFormat(int handle, IntPtr form, int fromlen, string title, BASSACMFormat flags)
	{
		flags |= BASSACMFormat.BASS_UNICODE;
		return BASS_Encode_GetACMFormatUnicode(handle, form, fromlen, title, flags);
	}

	public unsafe static ACMFORMAT BASS_Encode_GetACMFormat(int handle, string title, BASSACMFormat flags, WAVEFormatTag format)
	{
		ACMFORMAT result = null;
		int num = BASS_Encode_GetACMFormatUnicode(0, IntPtr.Zero, 0, null, BASSACMFormat.BASS_ACM_NONE);
		fixed (byte* ptr = new byte[num])
		{
			if (BASS_Encode_GetACMFormat(handle, (IntPtr)ptr, num, title, (BASSACMFormat)Utils.MakeLong((int)flags, (int)format)) > 0)
			{
				result = new ACMFORMAT((IntPtr)ptr);
			}
		}
		return result;
	}

	public unsafe static bool BASS_Encode_GetACMFormat(int handle, ref ACMFORMAT codec, string title, BASSACMFormat flags)
	{
		int num = BASS_Encode_GetACMFormatUnicode(0, IntPtr.Zero, 0, null, BASSACMFormat.BASS_ACM_NONE);
		byte[] array = new byte[(num > codec.FormatLength) ? num : codec.FormatLength];
		bool result = false;
		byte[] array2 = new byte[Marshal.SizeOf(codec) + codec.waveformatex.cbSize];
		int num2 = Marshal.SizeOf(codec);
		IntPtr intPtr = Marshal.AllocHGlobal(num2);
		Marshal.StructureToPtr(codec, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array2, 0, num2);
		Marshal.FreeHGlobal(intPtr);
		for (int i = 0; i < codec.extension.Length; i++)
		{
			array2[18 + i] = codec.extension[i];
		}
		Array.Copy(array2, array, num2);
		fixed (byte* ptr = array)
		{
			if (BASS_Encode_GetACMFormat(handle, (IntPtr)ptr, num, title, flags) > 0)
			{
				codec = new ACMFORMAT((IntPtr)ptr);
				result = true;
			}
		}
		return result;
	}

	public unsafe static ACMFORMAT BASS_Encode_GetACMFormatSuggest(int handle, BASSACMFormat flags, WAVEFormatTag format)
	{
		ACMFORMAT result = null;
		int num = BASS_Encode_GetACMFormatUnicode(0, IntPtr.Zero, 0, null, BASSACMFormat.BASS_ACM_NONE);
		fixed (byte* ptr = new byte[num])
		{
			if (BASS_Encode_GetACMFormat(handle, (IntPtr)ptr, num, null, (BASSACMFormat)Utils.MakeLong((int)flags, (int)format)) > 0)
			{
				result = new ACMFORMAT((IntPtr)ptr);
			}
		}
		return result;
	}

	[DllImport("bassenc")]
	public static extern int BASS_Encode_StartACM(int handle, [In] ACMFORMAT form, BASSEncode flags, ENCODEPROC proc, IntPtr user);

	[DllImport("bassenc")]
	public static extern int BASS_Encode_StartACM(int handle, IntPtr form, BASSEncode flags, ENCODEPROC proc, IntPtr user);

	[DllImport("bassenc", EntryPoint = "BASS_Encode_StartACMFile")]
	private static extern int BASS_Encode_StartACMFileUnicode(int handle, IntPtr form, BASSEncode flags, [In][MarshalAs(UnmanagedType.LPWStr)] string filename);

	[DllImport("bassenc", EntryPoint = "BASS_Encode_StartACMFile")]
	private static extern int BASS_Encode_StartACMFileUnicode(int handle, [In] ACMFORMAT form, BASSEncode flags, [In][MarshalAs(UnmanagedType.LPWStr)] string file);

	public static int BASS_Encode_StartACMFile(int handle, IntPtr form, BASSEncode flags, string filename)
	{
		flags |= BASSEncode.BASS_UNICODE;
		return BASS_Encode_StartACMFileUnicode(handle, form, flags, filename);
	}

	public static int BASS_Encode_StartACMFile(int handle, ACMFORMAT form, BASSEncode flags, string filename)
	{
		flags |= BASSEncode.BASS_UNICODE;
		return BASS_Encode_StartACMFileUnicode(handle, form, flags, filename);
	}

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_SetNotify(int handle, ENCODENOTIFYPROC proc, IntPtr user);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_SetChannel(int handle, int channel);

	[DllImport("bassenc")]
	public static extern int BASS_Encode_GetChannel(int handle);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_CastInit(int handle, [In][MarshalAs(UnmanagedType.LPStr)] string server, [In][MarshalAs(UnmanagedType.LPStr)] string pass, [In][MarshalAs(UnmanagedType.LPStr)] string content, [In][MarshalAs(UnmanagedType.LPStr)] string name, [In][MarshalAs(UnmanagedType.LPStr)] string url, [In][MarshalAs(UnmanagedType.LPStr)] string genre, [In][MarshalAs(UnmanagedType.LPStr)] string desc, [In][MarshalAs(UnmanagedType.LPStr)] string headers, int bitrate, bool pub);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_CastSetTitle(int handle, [In][MarshalAs(UnmanagedType.LPStr)] string title, [In][MarshalAs(UnmanagedType.LPStr)] string url);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_CastSetTitle(int handle, byte[] title, byte[] url);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool BASS_Encode_CastSendMeta(int handle, BASSEncodeMetaDataType type, byte[] buffer, int length);

	public static bool BASS_Encode_CastSendMeta(int handle, BASSEncodeMetaDataType type, byte[] buffer)
	{
		return BASS_Encode_CastSendMeta(handle, type, buffer, buffer.Length);
	}

	public static bool BASS_Encode_CastSendMeta(int handle, BASSEncodeMetaDataType type, string xml)
	{
		if (string.IsNullOrEmpty(xml))
		{
			return false;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(xml);
		return BASS_Encode_CastSendMeta(handle, type, bytes, bytes.Length);
	}

	[DllImport("bassenc", EntryPoint = "BASS_Encode_CastGetStats")]
	private static extern IntPtr BASS_Encode_CastGetStatsPtr(int handle, BASSEncodeStats type, [In][MarshalAs(UnmanagedType.LPStr)] string pass);

	[DllImport("bassenc", EntryPoint = "BASS_Encode_CastGetStats")]
	private static extern IntPtr BASS_Encode_CastGetStatsPtr(int handle, BASSEncodeStats type, IntPtr pass);

	public static string BASS_Encode_CastGetStats(int handle, BASSEncodeStats type, string pass)
	{
		if (string.IsNullOrEmpty(pass))
		{
			IntPtr intPtr = BASS_Encode_CastGetStatsPtr(handle, type, IntPtr.Zero);
			if (intPtr != IntPtr.Zero)
			{
				return Utils.IntPtrAsStringAnsi(intPtr);
			}
			return null;
		}
		IntPtr intPtr2 = BASS_Encode_CastGetStatsPtr(handle, type, pass);
		if (intPtr2 != IntPtr.Zero)
		{
			return Utils.IntPtrAsStringAnsi(intPtr2);
		}
		return null;
	}

	[DllImport("bassenc")]
	public static extern int BASS_Encode_ServerInit(int handle, [In][MarshalAs(UnmanagedType.LPStr)] string port, int buffer, int burst, BASSEncodeServer flags, ENCODECLIENTPROC proc, IntPtr user);

	[DllImport("bassenc")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Encode_ServerKick(int handle, [In][MarshalAs(UnmanagedType.LPStr)] string client);

	public static bool LoadMe()
	{
		return Utils.LoadLib("bassenc", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bassenc"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

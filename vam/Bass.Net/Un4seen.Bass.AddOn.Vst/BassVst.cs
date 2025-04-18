using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using radio42.Multimedia.Midi;
using Un4seen.Bass.AddOn.Midi;

namespace Un4seen.Bass.AddOn.Vst;

[SuppressUnmanagedCodeSecurity]
public sealed class BassVst
{
	private static int _myModuleHandle;

	private const string _myModuleName = "bass_vst";

	private BassVst()
	{
	}

	[DllImport("bass_vst", EntryPoint = "BASS_VST_ChannelSetDSP")]
	private static extern int BASS_VST_ChannelSetDSPUnicode(int chan, [In][MarshalAs(UnmanagedType.LPWStr)] string dllFile, int flags, int priority);

	public static int BASS_VST_ChannelSetDSP(int chan, string dllFile, BASSVSTDsp flags, int priority)
	{
		flags |= BASSVSTDsp.BASS_UNICODE;
		return BASS_VST_ChannelSetDSPUnicode(chan, dllFile, (int)flags, priority);
	}

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_ChannelRemoveDSP(int chan, int vstHandle);

	[DllImport("bass_vst", EntryPoint = "BASS_VST_ChannelCreate")]
	private static extern int BASS_VST_ChannelCreateUnicode(int freq, int chans, [In][MarshalAs(UnmanagedType.LPWStr)] string dllFile, BASSFlag flags);

	public static int BASS_VST_ChannelCreate(int freq, int chans, string dllFile, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_VST_ChannelCreateUnicode(freq, chans, dllFile, flags);
	}

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_ChannelFree(int vstHandle);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_ProcessEvent(int vstHandle, int midiCh, BASSMIDIEvent eventtype, int param);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_ProcessEventRaw(int vstHandle, IntPtr msg, int length);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_ProcessEventRaw(int vstHandle, byte[] msg, int length);

	public static bool BASS_VST_ProcessEventRaw(int vstHandle, MidiShortMessage msg)
	{
		int num = 0;
		num |= msg.Data2;
		num |= msg.Data1 << 8;
		num |= msg.Status << 16;
		return BASS_VST_ProcessEventRaw(vstHandle, new IntPtr(num), 0);
	}

	public static bool BASS_VST_ProcessEventRaw(int vstHandle, MidiSysExMessage msg)
	{
		return BASS_VST_ProcessEventRaw(vstHandle, msg.Message, msg.MessageLength);
	}

	[DllImport("bass_vst")]
	public static extern int BASS_VST_GetParamCount(int vstHandle);

	[DllImport("bass_vst")]
	public static extern float BASS_VST_GetParam(int vstHandle, int paramIndex);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_SetParam(int vstHandle, int paramIndex, float newValue);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_GetParamInfo(int vstHandle, int paramIndex, [In][Out] BASS_VST_PARAM_INFO ret);

	public static BASS_VST_PARAM_INFO BASS_VST_GetParamInfo(int vstHandle, int paramIndex)
	{
		BASS_VST_PARAM_INFO bASS_VST_PARAM_INFO = new BASS_VST_PARAM_INFO();
		if (BASS_VST_GetParamInfo(vstHandle, paramIndex, bASS_VST_PARAM_INFO))
		{
			return bASS_VST_PARAM_INFO;
		}
		return null;
	}

	[DllImport("bass_vst")]
	private static extern IntPtr BASS_VST_GetChunk(int vstHandle, [MarshalAs(UnmanagedType.Bool)] bool isPreset, ref int length);

	public static byte[] BASS_VST_GetChunk(int vstHandle, bool isPreset)
	{
		int length = 0;
		IntPtr intPtr = BASS_VST_GetChunk(vstHandle, isPreset, ref length);
		if (intPtr != IntPtr.Zero && length > 0)
		{
			byte[] array = new byte[length];
			Marshal.Copy(intPtr, array, 0, length);
			return array;
		}
		return null;
	}

	[DllImport("bass_vst")]
	private static extern int BASS_VST_SetChunk(int vstHandle, [MarshalAs(UnmanagedType.Bool)] bool isPreset, byte[] chunk, int length);

	public static int BASS_VST_SetChunk(int vstHandle, [MarshalAs(UnmanagedType.Bool)] bool isPreset, byte[] chunk)
	{
		if (chunk == null || chunk.Length == 0)
		{
			return -1;
		}
		return BASS_VST_SetChunk(vstHandle, isPreset, chunk, chunk.Length);
	}

	[DllImport("bass_vst")]
	public static extern int BASS_VST_GetProgramCount(int vstHandle);

	[DllImport("bass_vst")]
	public static extern int BASS_VST_GetProgram(int vstHandle);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_SetProgram(int vstHandle, int programIndex);

	[DllImport("bass_vst", EntryPoint = "BASS_VST_GetProgramParam")]
	private static extern IntPtr BASS_VST_GetProgramParamPtr(int vstHandle, int programIndex, ref int length);

	public static float[] BASS_VST_GetProgramParam(int vstHandle, int programIndex)
	{
		int length = 0;
		IntPtr intPtr = BASS_VST_GetProgramParamPtr(vstHandle, programIndex, ref length);
		if (intPtr != IntPtr.Zero && length > 0)
		{
			float[] array = new float[length];
			Marshal.Copy(intPtr, array, 0, length);
			return array;
		}
		return null;
	}

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool BASS_VST_SetProgramParam(int vstHandle, int programIndex, float[] param, int length);

	public static bool BASS_VST_SetProgramParam(int vstHandle, int programIndex, float[] param)
	{
		if (param != null && param.Length != 0)
		{
			return BASS_VST_SetProgramParam(vstHandle, programIndex, param, param.Length);
		}
		return false;
	}

	[DllImport("bass_vst", EntryPoint = "BASS_VST_GetProgramName")]
	private static extern IntPtr BASS_VST_GetProgramNamePtr(int vstHandle, int programIndex);

	public static string BASS_VST_GetProgramName(int vstHandle, int programIndex)
	{
		IntPtr intPtr = BASS_VST_GetProgramNamePtr(vstHandle, programIndex);
		if (intPtr != IntPtr.Zero)
		{
			return Utils.IntPtrAsStringAnsi(intPtr);
		}
		return null;
	}

	public static string[] BASS_VST_GetProgramNames(int vstHandle)
	{
		int num = BASS_VST_GetProgramCount(vstHandle);
		if (num > 0)
		{
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = BASS_VST_GetProgramName(vstHandle, i);
			}
			return array;
		}
		return null;
	}

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_SetProgramName(int vstHandle, int programIndex, [In][MarshalAs(UnmanagedType.LPStr)] string name);

	public static void BASS_VST_SetParamRestoreDefaults(int vstHandle)
	{
		if (vstHandle != 0)
		{
			int num = BASS_VST_GetParamCount(vstHandle);
			BASS_VST_PARAM_INFO bASS_VST_PARAM_INFO = new BASS_VST_PARAM_INFO();
			for (int i = 0; i < num; i++)
			{
				BASS_VST_GetParamInfo(vstHandle, i, bASS_VST_PARAM_INFO);
				BASS_VST_SetParam(vstHandle, i, bASS_VST_PARAM_INFO.defaultValue);
			}
		}
	}

	public static void BASS_VST_SetParamCopyParams(int sourceVstHandle, int destinVstHandle)
	{
		if (sourceVstHandle == 0 || destinVstHandle == 0)
		{
			return;
		}
		BASS_VST_INFO bASS_VST_INFO = new BASS_VST_INFO();
		BASS_VST_INFO bASS_VST_INFO2 = new BASS_VST_INFO();
		if (BASS_VST_GetInfo(sourceVstHandle, bASS_VST_INFO) && BASS_VST_GetInfo(destinVstHandle, bASS_VST_INFO2) && bASS_VST_INFO.uniqueID == bASS_VST_INFO2.uniqueID && bASS_VST_INFO.effectName.Equals(bASS_VST_INFO2.effectName))
		{
			int num = BASS_VST_GetParamCount(sourceVstHandle);
			for (int i = 0; i < num; i++)
			{
				BASS_VST_SetParam(destinVstHandle, i, BASS_VST_GetParam(sourceVstHandle, i));
			}
		}
	}

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_Resume(int vstHandle);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_SetBypass(int vstHandle, [MarshalAs(UnmanagedType.Bool)] bool state);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_GetBypass(int vstHandle);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_GetInfo(int vstHandle, [In][Out] BASS_VST_INFO ret);

	public static BASS_VST_INFO BASS_VST_GetInfo(int vstHandle)
	{
		BASS_VST_INFO bASS_VST_INFO = new BASS_VST_INFO();
		if (BASS_VST_GetInfo(vstHandle, bASS_VST_INFO))
		{
			return bASS_VST_INFO;
		}
		return null;
	}

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_EmbedEditor(int vstHandle, IntPtr parentWindow);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_SetScope(int vstHandle, int scope);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_SetCallback(int vstHandle, VSTPROC proc, IntPtr user);

	[DllImport("bass_vst")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_VST_SetLanguage([In][MarshalAs(UnmanagedType.LPStr)] string lang);

	public static bool LoadMe()
	{
		return Utils.LoadLib("bass_vst", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bass_vst"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

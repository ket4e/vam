using System;
using System.Runtime.InteropServices;

namespace Un4seen.Bass.AddOn.Vst;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public sealed class BASS_VST_AEFFECT
{
	public int magic = 1450406992;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	public VST_AEFFECT_Dispatcher dispatcher;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	public VST_AEFFECT_Process process;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	public VST_AEFFECT_SetParameter setParameter;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	public VST_AEFFECT_GetParameter getParameter;

	public int numPrograms;

	public int numParams;

	public int numInputs;

	public int numOutputs;

	public BASSVSTAEffectFlags flags;

	public IntPtr resvd1 = IntPtr.Zero;

	public IntPtr resvd2 = IntPtr.Zero;

	public int initialDelay;

	public int realQualities;

	public int offQualities;

	public float ioRatio;

	public IntPtr obj;

	public IntPtr user;

	public int uniqueID;

	public int version;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	public VST_AEFFECT_ProcessReplacing processReplacing;

	[MarshalAs(UnmanagedType.FunctionPtr)]
	public VST_AEFFECT_ProcessDoubleProc processDoubleReplacing;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 56)]
	public string future = string.Empty;

	private IntPtr _aeffect = IntPtr.Zero;

	public static BASS_VST_AEFFECT FromIntPtr(IntPtr aeffect)
	{
		if (Environment.Version.Major >= 2 && aeffect != IntPtr.Zero)
		{
			BASS_VST_AEFFECT bASS_VST_AEFFECT = (BASS_VST_AEFFECT)Marshal.PtrToStructure(aeffect, typeof(BASS_VST_AEFFECT));
			if (bASS_VST_AEFFECT != null)
			{
				bASS_VST_AEFFECT._aeffect = aeffect;
			}
			return bASS_VST_AEFFECT;
		}
		return null;
	}

	public override string ToString()
	{
		return $"In={numInputs}, Out={numOutputs}";
	}

	public string GetCurrentProgramName()
	{
		string result = string.Empty;
		if (_aeffect == IntPtr.Zero || numPrograms == 0)
		{
			return result;
		}
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = Marshal.AllocCoTaskMem(25);
			dispatcher(_aeffect, BASSVSTDispatcherOpCodes.effGetProgramName, 0, 0, intPtr, 0f);
			result = Utils.IntPtrAsStringAnsi(intPtr);
		}
		catch
		{
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(intPtr);
			}
		}
		return result;
	}

	public string GetProgramName(int program)
	{
		string result = string.Empty;
		if (_aeffect == IntPtr.Zero || program >= numPrograms)
		{
			return result;
		}
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = Marshal.AllocCoTaskMem(25);
			int currentProgram = GetCurrentProgram();
			SetCurrentProgram(program);
			dispatcher(_aeffect, BASSVSTDispatcherOpCodes.effGetProgramName, 0, 0, intPtr, 0f);
			result = Utils.IntPtrAsStringAnsi(intPtr);
			SetCurrentProgram(currentProgram);
		}
		catch
		{
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(intPtr);
			}
		}
		return result;
	}

	public string[] GetProgramNames()
	{
		if (_aeffect == IntPtr.Zero || numPrograms == 0)
		{
			return null;
		}
		string[] array = new string[numPrograms];
		for (int i = 0; i < numPrograms; i++)
		{
			array[i] = GetProgramName(i);
		}
		return array;
	}

	public int GetCurrentProgram()
	{
		int result = 0;
		if (_aeffect == IntPtr.Zero || numPrograms == 0)
		{
			return result;
		}
		try
		{
			result = dispatcher(_aeffect, BASSVSTDispatcherOpCodes.effGetProgram, 0, 0, IntPtr.Zero, 0f);
		}
		catch
		{
		}
		return result;
	}

	public bool SetCurrentProgram(int program)
	{
		bool result = false;
		if (_aeffect == IntPtr.Zero || program >= numPrograms)
		{
			return result;
		}
		try
		{
			dispatcher(_aeffect, BASSVSTDispatcherOpCodes.effSetProgram, 0, program, IntPtr.Zero, 0f);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public bool SetCurrentProgramName(string name)
	{
		bool result = false;
		if (_aeffect == IntPtr.Zero)
		{
			return result;
		}
		if (name.Length > 24)
		{
			name = name.Substring(0, 24);
		}
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			intPtr = Marshal.StringToCoTaskMemAnsi(name);
			dispatcher(_aeffect, BASSVSTDispatcherOpCodes.effSetProgramName, 0, 0, intPtr, 0f);
			result = true;
		}
		catch
		{
			result = false;
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(intPtr);
			}
		}
		return result;
	}
}

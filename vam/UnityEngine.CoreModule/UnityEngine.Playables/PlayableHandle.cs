using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[UsedByNativeCode]
public struct PlayableHandle
{
	internal IntPtr m_Handle;

	internal int m_Version;

	public static PlayableHandle Null
	{
		get
		{
			PlayableHandle result = default(PlayableHandle);
			result.m_Version = 10;
			return result;
		}
	}

	internal T GetObject<T>() where T : class, IPlayableBehaviour
	{
		if (!IsValid())
		{
			return (T)null;
		}
		object scriptInstance = GetScriptInstance();
		if (scriptInstance == null)
		{
			return (T)null;
		}
		return (T)scriptInstance;
	}

	[VisibleToOtherModules]
	internal bool IsPlayableOfType<T>()
	{
		return GetPlayableType() == typeof(T);
	}

	internal Playable GetInput(int inputPort)
	{
		return new Playable(GetInputHandle(inputPort));
	}

	internal Playable GetOutput(int outputPort)
	{
		return new Playable(GetOutputHandle(outputPort));
	}

	internal bool SetInputWeight(int inputIndex, float weight)
	{
		if (CheckInputBounds(inputIndex))
		{
			SetInputWeightFromIndex(inputIndex, weight);
			return true;
		}
		return false;
	}

	internal float GetInputWeight(int inputIndex)
	{
		if (CheckInputBounds(inputIndex))
		{
			return GetInputWeightFromIndex(inputIndex);
		}
		return 0f;
	}

	internal void Destroy()
	{
		GetGraph().DestroyPlayable(new Playable(this));
	}

	public static bool operator ==(PlayableHandle x, PlayableHandle y)
	{
		return CompareVersion(x, y);
	}

	public static bool operator !=(PlayableHandle x, PlayableHandle y)
	{
		return !CompareVersion(x, y);
	}

	public override bool Equals(object p)
	{
		if (!(p is PlayableHandle))
		{
			return false;
		}
		return CompareVersion(this, (PlayableHandle)p);
	}

	public override int GetHashCode()
	{
		return m_Handle.GetHashCode() ^ m_Version.GetHashCode();
	}

	internal static bool CompareVersion(PlayableHandle lhs, PlayableHandle rhs)
	{
		return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
	}

	internal bool CheckInputBounds(int inputIndex)
	{
		return CheckInputBounds(inputIndex, acceptAny: false);
	}

	internal bool CheckInputBounds(int inputIndex, bool acceptAny)
	{
		if (inputIndex == -1 && acceptAny)
		{
			return true;
		}
		if (inputIndex < 0)
		{
			throw new IndexOutOfRangeException("Index must be greater than 0");
		}
		if (GetInputCount() <= inputIndex)
		{
			throw new IndexOutOfRangeException("inputIndex " + inputIndex + " is greater than the number of available inputs (" + GetInputCount() + ").");
		}
		return true;
	}

	[VisibleToOtherModules]
	internal bool IsValid()
	{
		return IsValid_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal Type GetPlayableType()
	{
		return GetPlayableType_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetScriptInstance(object scriptInstance)
	{
		SetScriptInstance_Injected(ref this, scriptInstance);
	}

	[VisibleToOtherModules]
	internal bool CanChangeInputs()
	{
		return CanChangeInputs_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal bool CanSetWeights()
	{
		return CanSetWeights_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal bool CanDestroy()
	{
		return CanDestroy_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal PlayState GetPlayState()
	{
		return GetPlayState_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void Play()
	{
		Play_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void Pause()
	{
		Pause_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal double GetSpeed()
	{
		return GetSpeed_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetSpeed(double value)
	{
		SetSpeed_Injected(ref this, value);
	}

	[VisibleToOtherModules]
	internal double GetTime()
	{
		return GetTime_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetTime(double value)
	{
		SetTime_Injected(ref this, value);
	}

	[VisibleToOtherModules]
	internal bool IsDone()
	{
		return IsDone_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetDone(bool value)
	{
		SetDone_Injected(ref this, value);
	}

	[VisibleToOtherModules]
	internal double GetDuration()
	{
		return GetDuration_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetDuration(double value)
	{
		SetDuration_Injected(ref this, value);
	}

	[VisibleToOtherModules]
	internal bool GetPropagateSetTime()
	{
		return GetPropagateSetTime_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetPropagateSetTime(bool value)
	{
		SetPropagateSetTime_Injected(ref this, value);
	}

	[VisibleToOtherModules]
	internal PlayableGraph GetGraph()
	{
		GetGraph_Injected(ref this, out var ret);
		return ret;
	}

	[VisibleToOtherModules]
	internal int GetInputCount()
	{
		return GetInputCount_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetInputCount(int value)
	{
		SetInputCount_Injected(ref this, value);
	}

	[VisibleToOtherModules]
	internal int GetOutputCount()
	{
		return GetOutputCount_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetOutputCount(int value)
	{
		SetOutputCount_Injected(ref this, value);
	}

	[VisibleToOtherModules]
	internal void SetInputWeight(PlayableHandle input, float weight)
	{
		SetInputWeight_Injected(ref this, ref input, weight);
	}

	[VisibleToOtherModules]
	internal void SetDelay(double delay)
	{
		SetDelay_Injected(ref this, delay);
	}

	[VisibleToOtherModules]
	internal double GetDelay()
	{
		return GetDelay_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal bool IsDelayed()
	{
		return IsDelayed_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal double GetPreviousTime()
	{
		return GetPreviousTime_Injected(ref this);
	}

	[VisibleToOtherModules]
	internal void SetLeadTime(float value)
	{
		SetLeadTime_Injected(ref this, value);
	}

	[VisibleToOtherModules]
	internal float GetLeadTime()
	{
		return GetLeadTime_Injected(ref this);
	}

	private object GetScriptInstance()
	{
		return GetScriptInstance_Injected(ref this);
	}

	private PlayableHandle GetInputHandle(int index)
	{
		GetInputHandle_Injected(ref this, index, out var ret);
		return ret;
	}

	private PlayableHandle GetOutputHandle(int index)
	{
		GetOutputHandle_Injected(ref this, index, out var ret);
		return ret;
	}

	private void SetInputWeightFromIndex(int index, float weight)
	{
		SetInputWeightFromIndex_Injected(ref this, index, weight);
	}

	private float GetInputWeightFromIndex(int index)
	{
		return GetInputWeightFromIndex_Injected(ref this, index);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValid_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Type GetPlayableType_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetScriptInstance_Injected(ref PlayableHandle _unity_self, object scriptInstance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CanChangeInputs_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CanSetWeights_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CanDestroy_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PlayState GetPlayState_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Play_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Pause_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetSpeed_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSpeed_Injected(ref PlayableHandle _unity_self, double value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetTime_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTime_Injected(ref PlayableHandle _unity_self, double value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsDone_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDone_Injected(ref PlayableHandle _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetDuration_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDuration_Injected(ref PlayableHandle _unity_self, double value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetPropagateSetTime_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPropagateSetTime_Injected(ref PlayableHandle _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGraph_Injected(ref PlayableHandle _unity_self, out PlayableGraph ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetInputCount_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetInputCount_Injected(ref PlayableHandle _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetOutputCount_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetOutputCount_Injected(ref PlayableHandle _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetInputWeight_Injected(ref PlayableHandle _unity_self, ref PlayableHandle input, float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDelay_Injected(ref PlayableHandle _unity_self, double delay);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetDelay_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsDelayed_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double GetPreviousTime_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLeadTime_Injected(ref PlayableHandle _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetLeadTime_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetScriptInstance_Injected(ref PlayableHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetInputHandle_Injected(ref PlayableHandle _unity_self, int index, out PlayableHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetOutputHandle_Injected(ref PlayableHandle _unity_self, int index, out PlayableHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetInputWeightFromIndex_Injected(ref PlayableHandle _unity_self, int index, float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetInputWeightFromIndex_Injected(ref PlayableHandle _unity_self, int index);
}

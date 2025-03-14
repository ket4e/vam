using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[UsedByNativeCode]
public struct PlayableOutputHandle
{
	internal IntPtr m_Handle;

	internal int m_Version;

	public static PlayableOutputHandle Null
	{
		get
		{
			PlayableOutputHandle result = default(PlayableOutputHandle);
			result.m_Version = int.MaxValue;
			return result;
		}
	}

	[VisibleToOtherModules]
	internal bool IsPlayableOutputOfType<T>()
	{
		return GetPlayableOutputType() == typeof(T);
	}

	public override int GetHashCode()
	{
		return m_Handle.GetHashCode() ^ m_Version.GetHashCode();
	}

	public static bool operator ==(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
	{
		return CompareVersion(lhs, rhs);
	}

	public static bool operator !=(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
	{
		return !CompareVersion(lhs, rhs);
	}

	public override bool Equals(object p)
	{
		return p is PlayableOutputHandle && CompareVersion(this, (PlayableOutputHandle)p);
	}

	internal static bool CompareVersion(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
	{
		return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
	}

	[VisibleToOtherModules]
	internal bool IsValid()
	{
		return IsValid_Injected(ref this);
	}

	internal Type GetPlayableOutputType()
	{
		return GetPlayableOutputType_Injected(ref this);
	}

	internal Object GetReferenceObject()
	{
		return GetReferenceObject_Injected(ref this);
	}

	internal void SetReferenceObject(Object target)
	{
		SetReferenceObject_Injected(ref this, target);
	}

	internal Object GetUserData()
	{
		return GetUserData_Injected(ref this);
	}

	internal void SetUserData([Writable] Object target)
	{
		SetUserData_Injected(ref this, target);
	}

	internal PlayableHandle GetSourcePlayable()
	{
		GetSourcePlayable_Injected(ref this, out var ret);
		return ret;
	}

	internal void SetSourcePlayable(PlayableHandle target)
	{
		SetSourcePlayable_Injected(ref this, ref target);
	}

	internal int GetSourceInputPort()
	{
		return GetSourceInputPort_Injected(ref this);
	}

	internal void SetSourceInputPort(int port)
	{
		SetSourceInputPort_Injected(ref this, port);
	}

	internal float GetWeight()
	{
		return GetWeight_Injected(ref this);
	}

	internal void SetWeight(float weight)
	{
		SetWeight_Injected(ref this, weight);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValid_Injected(ref PlayableOutputHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Type GetPlayableOutputType_Injected(ref PlayableOutputHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Object GetReferenceObject_Injected(ref PlayableOutputHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetReferenceObject_Injected(ref PlayableOutputHandle _unity_self, Object target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Object GetUserData_Injected(ref PlayableOutputHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetUserData_Injected(ref PlayableOutputHandle _unity_self, [Writable] Object target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSourcePlayable_Injected(ref PlayableOutputHandle _unity_self, out PlayableHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSourcePlayable_Injected(ref PlayableOutputHandle _unity_self, ref PlayableHandle target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSourceInputPort_Injected(ref PlayableOutputHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSourceInputPort_Injected(ref PlayableOutputHandle _unity_self, int port);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetWeight_Injected(ref PlayableOutputHandle _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetWeight_Injected(ref PlayableOutputHandle _unity_self, float weight);
}

using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling;

/// <summary>
///   <para>Records profiling data produced by a specific Sampler.</para>
/// </summary>
[UsedByNativeCode]
public sealed class Recorder
{
	internal IntPtr m_Ptr;

	internal static Recorder s_InvalidRecorder = new Recorder();

	/// <summary>
	///   <para>Returns true if Recorder is valid and can collect data. (Read Only)</para>
	/// </summary>
	public bool isValid => m_Ptr != IntPtr.Zero;

	/// <summary>
	///   <para>Enables recording.</para>
	/// </summary>
	[ThreadAndSerializationSafe]
	public extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Accumulated time of Begin/End pairs for the previous frame in nanoseconds. (Read Only)</para>
	/// </summary>
	[ThreadAndSerializationSafe]
	public extern long elapsedNanoseconds
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Number of time Begin/End pairs was called during the previous frame. (Read Only)</para>
	/// </summary>
	[ThreadAndSerializationSafe]
	public extern int sampleBlockCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal Recorder()
	{
	}

	~Recorder()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			DisposeNative();
		}
	}

	/// <summary>
	///   <para>Use this function to get a Recorder for the specific Profiler label.</para>
	/// </summary>
	/// <param name="samplerName">Sampler name.</param>
	/// <returns>
	///   <para>Recorder object for the specified Sampler.</para>
	/// </returns>
	public static Recorder Get(string samplerName)
	{
		Recorder @internal = GetInternal(samplerName);
		if (@internal == null)
		{
			return s_InvalidRecorder;
		}
		return @internal;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern Recorder GetInternal(string samplerName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void DisposeNative();
}

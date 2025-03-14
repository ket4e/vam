using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The Audio High Pass Filter passes high frequencies of an AudioSource, and cuts off signals with frequencies lower than the Cutoff Frequency.</para>
/// </summary>
[RequireComponent(typeof(AudioBehaviour))]
public sealed class AudioHighPassFilter : Behaviour
{
	/// <summary>
	///   <para>Highpass cutoff frequency in hz. 10.0 to 22000.0. Default = 5000.0.</para>
	/// </summary>
	public extern float cutoffFrequency
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Determines how much the filter's self-resonance isdampened.</para>
	/// </summary>
	public extern float highpassResonanceQ
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}
}

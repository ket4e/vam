using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The Audio Low Pass Filter passes low frequencies of an AudioSource or all sounds reaching an AudioListener, while removing frequencies higher than the Cutoff Frequency.</para>
/// </summary>
[RequireComponent(typeof(AudioBehaviour))]
public sealed class AudioLowPassFilter : Behaviour
{
	/// <summary>
	///   <para>Lowpass cutoff frequency in hz. 10.0 to 22000.0. Default = 5000.0.</para>
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
	///   <para>Returns or sets the current custom frequency cutoff curve.</para>
	/// </summary>
	public extern AnimationCurve customCutoffCurve
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Determines how much the filter's self-resonance is dampened.</para>
	/// </summary>
	public extern float lowpassResonanceQ
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}
}

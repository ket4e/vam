using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The Audio Reverb Filter takes an Audio Clip and distorts it to create a custom reverb effect.</para>
/// </summary>
[RequireComponent(typeof(AudioBehaviour))]
public sealed class AudioReverbFilter : Behaviour
{
	/// <summary>
	///   <para>Set/Get reverb preset properties.</para>
	/// </summary>
	public extern AudioReverbPreset reverbPreset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Mix level of dry signal in output in mB. Ranges from -10000.0 to 0.0. Default is 0.</para>
	/// </summary>
	public extern float dryLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Room effect level at low frequencies in mB. Ranges from -10000.0 to 0.0. Default is 0.0.</para>
	/// </summary>
	public extern float room
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Room effect high-frequency level re. low frequency level in mB. Ranges from -10000.0 to 0.0. Default is 0.0.</para>
	/// </summary>
	public extern float roomHF
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("roomRolloffFactor is no longer supported.")]
	public float roomRolloffFactor
	{
		get
		{
			return 10f;
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Reverberation decay time at low-frequencies in seconds. Ranges from 0.1 to 20.0. Default is 1.0.</para>
	/// </summary>
	public extern float decayTime
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Decay HF Ratio : High-frequency to low-frequency decay time ratio. Ranges from 0.1 to 2.0. Default is 0.5.</para>
	/// </summary>
	public extern float decayHFRatio
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Early reflections level relative to room effect in mB. Ranges from -10000.0 to 1000.0. Default is -10000.0.</para>
	/// </summary>
	public extern float reflectionsLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Late reverberation level relative to room effect in mB. Ranges from -10000.0 to 2000.0. Default is 0.0.</para>
	/// </summary>
	public extern float reflectionsDelay
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Late reverberation level relative to room effect in mB. Ranges from -10000.0 to 2000.0. Default is 0.0.</para>
	/// </summary>
	public extern float reverbLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Late reverberation delay time relative to first reflection in seconds. Ranges from 0.0 to 0.1. Default is 0.04.</para>
	/// </summary>
	public extern float reverbDelay
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Reverberation diffusion (echo density) in percent. Ranges from 0.0 to 100.0. Default is 100.0.</para>
	/// </summary>
	public extern float diffusion
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Reverberation density (modal density) in percent. Ranges from 0.0 to 100.0. Default is 100.0.</para>
	/// </summary>
	public extern float density
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Reference high frequency in Hz. Ranges from 20.0 to 20000.0. Default is 5000.0.</para>
	/// </summary>
	public extern float hfReference
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Room effect low-frequency level in mB. Ranges from -10000.0 to 0.0. Default is 0.0.</para>
	/// </summary>
	public extern float roomLF
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Reference low-frequency in Hz. Ranges from 20.0 to 1000.0. Default is 250.0.</para>
	/// </summary>
	public extern float lfReference
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}
}

using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Audio;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Allow recording the main output of the game or specific groups in the AudioMixer.</para>
/// </summary>
[NativeType(Header = "Modules/Audio/Public/ScriptBindings/AudioRenderer.bindings.h")]
public class AudioRenderer
{
	/// <summary>
	///   <para>Enters audio recording mode. After this Unity will output silence until AudioRenderer.Stop is called.</para>
	/// </summary>
	/// <returns>
	///   <para>True if the engine was switched into output recording mode. False if it is already recording.</para>
	/// </returns>
	public static bool Start()
	{
		return Internal_AudioRenderer_Start();
	}

	/// <summary>
	///   <para>Exits audio recording mode. After this audio output will be audible again.</para>
	/// </summary>
	/// <returns>
	///   <para>True if the engine was recording when this function was called.</para>
	/// </returns>
	public static bool Stop()
	{
		return Internal_AudioRenderer_Stop();
	}

	/// <summary>
	///   <para>Returns the number of samples available since the last time AudioRenderer.Render was called. This is dependent on the frame capture rate.</para>
	/// </summary>
	/// <returns>
	///   <para>Number of samples available since last recorded frame.</para>
	/// </returns>
	public static int GetSampleCountForCaptureFrame()
	{
		return Internal_AudioRenderer_GetSampleCountForCaptureFrame();
	}

	internal unsafe static bool AddMixerGroupSink(AudioMixerGroup mixerGroup, NativeArray<float> buffer, bool excludeFromMix)
	{
		return Internal_AudioRenderer_AddMixerGroupSink(mixerGroup, buffer.GetUnsafePtr(), buffer.Length, excludeFromMix);
	}

	public unsafe static bool Render(NativeArray<float> buffer)
	{
		return Internal_AudioRenderer_Render(buffer.GetUnsafePtr(), buffer.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool Internal_AudioRenderer_Start();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool Internal_AudioRenderer_Stop();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int Internal_AudioRenderer_GetSampleCountForCaptureFrame();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal unsafe static extern bool Internal_AudioRenderer_AddMixerGroupSink(AudioMixerGroup mixerGroup, void* ptr, int length, bool excludeFromMix);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal unsafe static extern bool Internal_AudioRenderer_Render(void* ptr, int length);
}

using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Representation of a listener in 3D space.</para>
/// </summary>
[RequireComponent(typeof(Transform))]
public sealed class AudioListener : AudioBehaviour
{
	internal AudioListenerExtension spatializerExtension = null;

	/// <summary>
	///   <para>Controls the game sound volume (0.0 to 1.0).</para>
	/// </summary>
	public static extern float volume
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The paused state of the audio system.</para>
	/// </summary>
	public static extern bool pause
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>This lets you set whether the Audio Listener should be updated in the fixed or dynamic update.</para>
	/// </summary>
	public extern AudioVelocityUpdateMode velocityUpdateMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetOutputDataHelper(float[] samples, int channel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GetSpectrumDataHelper(float[] samples, int channel, FFTWindow window);

	/// <summary>
	///   <para>Deprecated Version. Returns a block of the listener (master)'s output data.</para>
	/// </summary>
	/// <param name="numSamples"></param>
	/// <param name="channel"></param>
	[Obsolete("GetOutputData returning a float[] is deprecated, use GetOutputData and pass a pre allocated array instead.")]
	public static float[] GetOutputData(int numSamples, int channel)
	{
		float[] array = new float[numSamples];
		GetOutputDataHelper(array, channel);
		return array;
	}

	/// <summary>
	///   <para>Provides a block of the listener (master)'s output data.</para>
	/// </summary>
	/// <param name="samples">The array to populate with audio samples. Its length must be a power of 2.</param>
	/// <param name="channel">The channel to sample from.</param>
	public static void GetOutputData(float[] samples, int channel)
	{
		GetOutputDataHelper(samples, channel);
	}

	/// <summary>
	///   <para>Deprecated Version. Returns a block of the listener (master)'s spectrum data.</para>
	/// </summary>
	/// <param name="numSamples">Number of values (the length of the samples array). Must be a power of 2. Min = 64. Max = 8192.</param>
	/// <param name="channel">The channel to sample from.</param>
	/// <param name="window">The FFTWindow type to use when sampling.</param>
	[Obsolete("GetSpectrumData returning a float[] is deprecated, use GetOutputData and pass a pre allocated array instead.")]
	public static float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
	{
		float[] array = new float[numSamples];
		GetSpectrumDataHelper(array, channel, window);
		return array;
	}

	/// <summary>
	///   <para>Provides a block of the listener (master)'s spectrum data.</para>
	/// </summary>
	/// <param name="samples">The array to populate with audio samples. Its length must be a power of 2.</param>
	/// <param name="channel">The channel to sample from.</param>
	/// <param name="window">The FFTWindow type to use when sampling.</param>
	public static void GetSpectrumData(float[] samples, int channel, FFTWindow window)
	{
		GetSpectrumDataHelper(samples, channel, window);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern int GetNumExtensionProperties();

	internal int GetNumExtensionPropertiesForThisExtension(PropertyName extensionName)
	{
		return INTERNAL_CALL_GetNumExtensionPropertiesForThisExtension(this, ref extensionName);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int INTERNAL_CALL_GetNumExtensionPropertiesForThisExtension(AudioListener self, ref PropertyName extensionName);

	internal PropertyName ReadExtensionName(int listenerIndex)
	{
		INTERNAL_CALL_ReadExtensionName(this, listenerIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadExtensionName(AudioListener self, int listenerIndex, out PropertyName value);

	internal PropertyName ReadExtensionPropertyName(int listenerIndex)
	{
		INTERNAL_CALL_ReadExtensionPropertyName(this, listenerIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadExtensionPropertyName(AudioListener self, int listenerIndex, out PropertyName value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern float ReadExtensionPropertyValue(int listenerIndex);

	internal bool ReadExtensionProperty(PropertyName extensionName, PropertyName propertyName, ref float propertyValue)
	{
		return INTERNAL_CALL_ReadExtensionProperty(this, ref extensionName, ref propertyName, ref propertyValue);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_ReadExtensionProperty(AudioListener self, ref PropertyName extensionName, ref PropertyName propertyName, ref float propertyValue);

	internal void ClearExtensionProperties(PropertyName extensionName)
	{
		INTERNAL_CALL_ClearExtensionProperties(this, ref extensionName);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ClearExtensionProperties(AudioListener self, ref PropertyName extensionName);

	internal AudioListenerExtension AddExtension(Type extensionType)
	{
		if (spatializerExtension == null)
		{
			spatializerExtension = ScriptableObject.CreateInstance(extensionType) as AudioListenerExtension;
		}
		return spatializerExtension;
	}
}

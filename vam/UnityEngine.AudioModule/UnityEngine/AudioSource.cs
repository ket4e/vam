using System;
using System.Runtime.CompilerServices;
using UnityEngine.Audio;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A representation of audio sources in 3D.</para>
/// </summary>
[RequireComponent(typeof(Transform))]
public sealed class AudioSource : AudioBehaviour
{
	internal AudioSourceExtension spatializerExtension = null;

	internal AudioSourceExtension ambisonicExtension = null;

	/// <summary>
	///   <para>The volume of the audio source (0.0 to 1.0).</para>
	/// </summary>
	public extern float volume
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The pitch of the audio source.</para>
	/// </summary>
	public extern float pitch
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Playback position in seconds.</para>
	/// </summary>
	public extern float time
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Playback position in PCM samples.</para>
	/// </summary>
	[ThreadAndSerializationSafe]
	public extern int timeSamples
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The default AudioClip to play.</para>
	/// </summary>
	[ThreadAndSerializationSafe]
	public extern AudioClip clip
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The target group to which the AudioSource should route its signal.</para>
	/// </summary>
	public extern AudioMixerGroup outputAudioMixerGroup
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Is the clip playing right now (Read Only)?</para>
	/// </summary>
	public extern bool isPlaying
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>True if all sounds played by the AudioSource (main sound started by Play() or playOnAwake as well as one-shots) are culled by the audio system.</para>
	/// </summary>
	public extern bool isVirtual
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Is the audio clip looping?</para>
	/// </summary>
	public extern bool loop
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>This makes the audio source not take into account the volume of the audio listener.</para>
	/// </summary>
	public extern bool ignoreListenerVolume
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>If set to true, the audio source will automatically start playing on awake.</para>
	/// </summary>
	public extern bool playOnAwake
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Allows AudioSource to play even though AudioListener.pause is set to true. This is useful for the menu element sounds or background music in pause menus.</para>
	/// </summary>
	public extern bool ignoreListenerPause
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Whether the Audio Source should be updated in the fixed or dynamic update.</para>
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

	/// <summary>
	///   <para>Pans a playing sound in a stereo way (left or right). This only applies to sounds that are Mono or Stereo.</para>
	/// </summary>
	public extern float panStereo
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D.</para>
	/// </summary>
	public extern float spatialBlend
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	internal extern bool spatializeInternal
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Enables or disables spatialization.</para>
	/// </summary>
	public bool spatialize
	{
		get
		{
			return spatializeInternal;
		}
		set
		{
			if (spatializeInternal == value)
			{
				return;
			}
			spatializeInternal = value;
			if (value)
			{
				AudioSourceExtension audioSourceExtension = AudioExtensionManager.AddSpatializerExtension(this);
				if (audioSourceExtension != null && isPlaying)
				{
					AudioExtensionManager.GetReadyToPlay(audioSourceExtension);
				}
			}
		}
	}

	/// <summary>
	///   <para>Determines if the spatializer effect is inserted before or after the effect filters.</para>
	/// </summary>
	public extern bool spatializePostEffects
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The amount by which the signal from the AudioSource will be mixed into the global reverb associated with the Reverb Zones.</para>
	/// </summary>
	public extern float reverbZoneMix
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Bypass effects (Applied from filter components or global listener filters).</para>
	/// </summary>
	public extern bool bypassEffects
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>When set global effects on the AudioListener will not be applied to the audio signal generated by the AudioSource. Does not apply if the AudioSource is playing into a mixer group.</para>
	/// </summary>
	public extern bool bypassListenerEffects
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>When set doesn't route the signal from an AudioSource into the global reverb associated with reverb zones.</para>
	/// </summary>
	public extern bool bypassReverbZones
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Sets the Doppler scale for this AudioSource.</para>
	/// </summary>
	public extern float dopplerLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Sets the spread angle (in degrees) of a 3d stereo or multichannel sound in speaker space.</para>
	/// </summary>
	public extern float spread
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Sets the priority of the AudioSource.</para>
	/// </summary>
	public extern int priority
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Un- / Mutes the AudioSource. Mute sets the volume=0, Un-Mute restore the original volume.</para>
	/// </summary>
	public extern bool mute
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Within the Min distance the AudioSource will cease to grow louder in volume.</para>
	/// </summary>
	public extern float minDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>(Logarithmic rolloff) MaxDistance is the distance a sound stops attenuating at.</para>
	/// </summary>
	public extern float maxDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Sets/Gets how the AudioSource attenuates over distance.</para>
	/// </summary>
	public extern AudioRolloffMode rolloffMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
	public extern float minVolume
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
	public extern float maxVolume
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
	public extern float rolloffFactor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Plays the clip with an optional certain delay.</para>
	/// </summary>
	/// <param name="delay">Delay in number of samples, assuming a 44100Hz sample rate (meaning that Play(44100) will delay the playing by exactly 1 sec).</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Play([DefaultValue("0")] ulong delay);

	[ExcludeFromDocs]
	public void Play()
	{
		ulong delay = 0uL;
		Play(delay);
	}

	/// <summary>
	///   <para>Plays the clip with a delay specified in seconds. Users are advised to use this function instead of the old Play(delay) function that took a delay specified in samples relative to a reference rate of 44.1 kHz as an argument.</para>
	/// </summary>
	/// <param name="delay">Delay time specified in seconds.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void PlayDelayed(float delay);

	/// <summary>
	///   <para>Plays the clip at a specific time on the absolute time-line that AudioSettings.dspTime reads from.</para>
	/// </summary>
	/// <param name="time">Time in seconds on the absolute time-line that AudioSettings.dspTime refers to for when the sound should start playing.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void PlayScheduled(double time);

	/// <summary>
	///   <para>Changes the time at which a sound that has already been scheduled to play will start.</para>
	/// </summary>
	/// <param name="time">Time in seconds.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetScheduledStartTime(double time);

	/// <summary>
	///   <para>Changes the time at which a sound that has already been scheduled to play will end. Notice that depending on the timing not all rescheduling requests can be fulfilled.</para>
	/// </summary>
	/// <param name="time">Time in seconds.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetScheduledEndTime(double time);

	/// <summary>
	///   <para>Stops playing the clip.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Stop();

	/// <summary>
	///   <para>Pauses playing the clip.</para>
	/// </summary>
	public void Pause()
	{
		INTERNAL_CALL_Pause(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Pause(AudioSource self);

	/// <summary>
	///   <para>Unpause the paused playback of this AudioSource.</para>
	/// </summary>
	public void UnPause()
	{
		INTERNAL_CALL_UnPause(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UnPause(AudioSource self);

	/// <summary>
	///   <para>Plays an AudioClip, and scales the AudioSource volume by volumeScale.</para>
	/// </summary>
	/// <param name="clip">The clip being played.</param>
	/// <param name="volumeScale">The scale of the volume (0-1).</param>
	[ExcludeFromDocs]
	public void PlayOneShot(AudioClip clip)
	{
		float volumeScale = 1f;
		PlayOneShot(clip, volumeScale);
	}

	/// <summary>
	///   <para>Plays an AudioClip, and scales the AudioSource volume by volumeScale.</para>
	/// </summary>
	/// <param name="clip">The clip being played.</param>
	/// <param name="volumeScale">The scale of the volume (0-1).</param>
	public void PlayOneShot(AudioClip clip, [DefaultValue("1.0F")] float volumeScale)
	{
		if (clip != null && clip.ambisonic)
		{
			AudioSourceExtension audioSourceExtension = AudioExtensionManager.AddAmbisonicDecoderExtension(this);
			if (audioSourceExtension != null)
			{
				AudioExtensionManager.GetReadyToPlay(audioSourceExtension);
			}
		}
		PlayOneShotHelper(clip, volumeScale);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void PlayOneShotHelper(AudioClip clip, [DefaultValue("1.0F")] float volumeScale);

	[ExcludeFromDocs]
	private void PlayOneShotHelper(AudioClip clip)
	{
		float volumeScale = 1f;
		PlayOneShotHelper(clip, volumeScale);
	}

	/// <summary>
	///   <para>Plays an AudioClip at a given position in world space.</para>
	/// </summary>
	/// <param name="clip">Audio data to play.</param>
	/// <param name="position">Position in world space from which sound originates.</param>
	/// <param name="volume">Playback volume.</param>
	[ExcludeFromDocs]
	public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		float num = 1f;
		PlayClipAtPoint(clip, position, num);
	}

	/// <summary>
	///   <para>Plays an AudioClip at a given position in world space.</para>
	/// </summary>
	/// <param name="clip">Audio data to play.</param>
	/// <param name="position">Position in world space from which sound originates.</param>
	/// <param name="volume">Playback volume.</param>
	public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [DefaultValue("1.0F")] float volume)
	{
		GameObject gameObject = new GameObject("One shot audio");
		gameObject.transform.position = position;
		AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		audioSource.clip = clip;
		audioSource.spatialBlend = 1f;
		audioSource.volume = volume;
		audioSource.Play();
		Object.Destroy(gameObject, clip.length * ((!(Time.timeScale < 0.01f)) ? Time.timeScale : 0.01f));
	}

	/// <summary>
	///   <para>Set the custom curve for the given AudioSourceCurveType.</para>
	/// </summary>
	/// <param name="type">The curve type that should be set.</param>
	/// <param name="curve">The curve that should be applied to the given curve type.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetCustomCurve(AudioSourceCurveType type, AnimationCurve curve);

	/// <summary>
	///   <para>Get the current custom curve for the given AudioSourceCurveType.</para>
	/// </summary>
	/// <param name="type">The curve type to get.</param>
	/// <returns>
	///   <para>The custom AnimationCurve corresponding to the given curve type.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern AnimationCurve GetCustomCurve(AudioSourceCurveType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void GetOutputDataHelper(float[] samples, int channel);

	/// <summary>
	///   <para>Deprecated Version. Returns a block of the currently playing source's output data.</para>
	/// </summary>
	/// <param name="numSamples"></param>
	/// <param name="channel"></param>
	[Obsolete("GetOutputData return a float[] is deprecated, use GetOutputData passing a pre allocated array instead.")]
	public float[] GetOutputData(int numSamples, int channel)
	{
		float[] array = new float[numSamples];
		GetOutputDataHelper(array, channel);
		return array;
	}

	/// <summary>
	///   <para>Provides a block of the currently playing source's output data.</para>
	/// </summary>
	/// <param name="samples">The array to populate with audio samples. Its length must be a power of 2.</param>
	/// <param name="channel">The channel to sample from.</param>
	public void GetOutputData(float[] samples, int channel)
	{
		GetOutputDataHelper(samples, channel);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void GetSpectrumDataHelper(float[] samples, int channel, FFTWindow window);

	/// <summary>
	///   <para>Deprecated Version. Returns a block of the currently playing source's spectrum data.</para>
	/// </summary>
	/// <param name="numSamples">The number of samples to retrieve. Must be a power of 2.</param>
	/// <param name="channel">The channel to sample from.</param>
	/// <param name="window">The FFTWindow type to use when sampling.</param>
	[Obsolete("GetSpectrumData returning a float[] is deprecated, use GetSpectrumData passing a pre allocated array instead.")]
	public float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
	{
		float[] array = new float[numSamples];
		GetSpectrumDataHelper(array, channel, window);
		return array;
	}

	/// <summary>
	///   <para>Provides a block of the currently playing audio source's spectrum data.</para>
	/// </summary>
	/// <param name="samples">The array to populate with audio samples. Its length must be a power of 2.</param>
	/// <param name="channel">The channel to sample from.</param>
	/// <param name="window">The FFTWindow type to use when sampling.</param>
	public void GetSpectrumData(float[] samples, int channel, FFTWindow window)
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
	private static extern int INTERNAL_CALL_GetNumExtensionPropertiesForThisExtension(AudioSource self, ref PropertyName extensionName);

	internal PropertyName ReadExtensionName(int sourceIndex)
	{
		INTERNAL_CALL_ReadExtensionName(this, sourceIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadExtensionName(AudioSource self, int sourceIndex, out PropertyName value);

	internal PropertyName ReadExtensionPropertyName(int sourceIndex)
	{
		INTERNAL_CALL_ReadExtensionPropertyName(this, sourceIndex, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadExtensionPropertyName(AudioSource self, int sourceIndex, out PropertyName value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern float ReadExtensionPropertyValue(int sourceIndex);

	internal bool ReadExtensionProperty(PropertyName extensionName, PropertyName propertyName, ref float propertyValue)
	{
		return INTERNAL_CALL_ReadExtensionProperty(this, ref extensionName, ref propertyName, ref propertyValue);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_ReadExtensionProperty(AudioSource self, ref PropertyName extensionName, ref PropertyName propertyName, ref float propertyValue);

	internal void ClearExtensionProperties(PropertyName extensionName)
	{
		INTERNAL_CALL_ClearExtensionProperties(this, ref extensionName);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ClearExtensionProperties(AudioSource self, ref PropertyName extensionName);

	internal AudioSourceExtension AddSpatializerExtension(Type extensionType)
	{
		if (spatializerExtension == null)
		{
			spatializerExtension = ScriptableObject.CreateInstance(extensionType) as AudioSourceExtension;
		}
		return spatializerExtension;
	}

	internal AudioSourceExtension AddAmbisonicExtension(Type extensionType)
	{
		if (ambisonicExtension == null)
		{
			ambisonicExtension = ScriptableObject.CreateInstance(extensionType) as AudioSourceExtension;
		}
		return ambisonicExtension;
	}

	/// <summary>
	///   <para>Sets a user-defined parameter of a custom spatializer effect that is attached to an AudioSource.</para>
	/// </summary>
	/// <param name="index">Zero-based index of user-defined parameter to be set.</param>
	/// <param name="value">New value of the user-defined parameter.</param>
	/// <returns>
	///   <para>True, if the parameter could be set.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool SetSpatializerFloat(int index, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool GetSpatializerFloat(int index, out float value);

	/// <summary>
	///   <para>Sets a user-defined parameter of a custom ambisonic decoder effect that is attached to an AudioSource.</para>
	/// </summary>
	/// <param name="index">Zero-based index of user-defined parameter to be set.</param>
	/// <param name="value">New value of the user-defined parameter.</param>
	/// <returns>
	///   <para>True, if the parameter could be set.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool SetAmbisonicDecoderFloat(int index, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool GetAmbisonicDecoderFloat(int index, out float value);
}

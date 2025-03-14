using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Movie Textures are textures onto which movies are played back.</para>
/// </summary>
[ExcludeFromPreset]
[ExcludeFromObjectFactory]
public sealed class MovieTexture : Texture
{
	/// <summary>
	///   <para>Returns the AudioClip belonging to the MovieTexture.</para>
	/// </summary>
	public extern AudioClip audioClip
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Set this to true to make the movie loop.</para>
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
	///   <para>Returns whether the movie is playing or not.</para>
	/// </summary>
	public extern bool isPlaying
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>If the movie is downloading from a web site, this returns if enough data has been downloaded so playback should be able to start without interruptions.</para>
	/// </summary>
	public extern bool isReadyToPlay
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The time, in seconds, that the movie takes to play back completely.</para>
	/// </summary>
	public extern float duration
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	private MovieTexture()
	{
	}

	/// <summary>
	///   <para>Starts playing the movie.</para>
	/// </summary>
	public void Play()
	{
		INTERNAL_CALL_Play(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Play(MovieTexture self);

	/// <summary>
	///   <para>Stops playing the movie, and rewinds it to the beginning.</para>
	/// </summary>
	public void Stop()
	{
		INTERNAL_CALL_Stop(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Stop(MovieTexture self);

	/// <summary>
	///   <para>Pauses playing the movie.</para>
	/// </summary>
	public void Pause()
	{
		INTERNAL_CALL_Pause(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Pause(MovieTexture self);
}

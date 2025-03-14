using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Video;

/// <summary>
///   <para>A container for video data.</para>
/// </summary>
public sealed class VideoClip : Object
{
	/// <summary>
	///   <para>The video clip path in the project's assets. (Read Only).</para>
	/// </summary>
	public extern string originalPath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The length of the VideoClip in frames. (Read Only).</para>
	/// </summary>
	public extern ulong frameCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The frame rate of the clip in frames/second. (Read Only).</para>
	/// </summary>
	public extern double frameRate
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The length of the video clip in seconds. (Read Only).</para>
	/// </summary>
	public extern double length
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The width of the images in the video clip in pixels. (Read Only).</para>
	/// </summary>
	public extern uint width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The height of the images in the video clip in pixels. (Read Only).</para>
	/// </summary>
	public extern uint height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Numerator of the pixel aspect ratio (num:den). (Read Only).</para>
	/// </summary>
	public extern uint pixelAspectRatioNumerator
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Denominator of the pixel aspect ratio (num:den). (Read Only).</para>
	/// </summary>
	public extern uint pixelAspectRatioDenominator
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Number of audio tracks in the clip.</para>
	/// </summary>
	public extern ushort audioTrackCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	private VideoClip()
	{
	}

	/// <summary>
	///   <para>The number of channels in the audio track.  E.g. 2 for a stereo track.</para>
	/// </summary>
	/// <param name="audioTrackIdx">Index of the audio queried audio track.</param>
	/// <returns>
	///   <para>The number of channels.</para>
	/// </returns>
	public ushort GetAudioChannelCount(ushort audioTrackIdx)
	{
		return INTERNAL_CALL_GetAudioChannelCount(this, audioTrackIdx);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern ushort INTERNAL_CALL_GetAudioChannelCount(VideoClip self, ushort audioTrackIdx);

	/// <summary>
	///   <para>Get the audio track sampling rate in Hertz.</para>
	/// </summary>
	/// <param name="audioTrackIdx">Index of the audio queried audio track.</param>
	/// <returns>
	///   <para>The sampling rate in Hertz.</para>
	/// </returns>
	public uint GetAudioSampleRate(ushort audioTrackIdx)
	{
		return INTERNAL_CALL_GetAudioSampleRate(this, audioTrackIdx);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern uint INTERNAL_CALL_GetAudioSampleRate(VideoClip self, ushort audioTrackIdx);

	/// <summary>
	///   <para>Get the audio track language.  Can be unknown.</para>
	/// </summary>
	/// <param name="audioTrackIdx">Index of the audio queried audio track.</param>
	/// <returns>
	///   <para>The abbreviated name of the language.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern string GetAudioLanguage(ushort audioTrackIdx);
}

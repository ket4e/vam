using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The AnimationState gives full control over animation blending.</para>
/// </summary>
[UsedByNativeCode]
public sealed class AnimationState : TrackedReference
{
	/// <summary>
	///   <para>Enables / disables the animation.</para>
	/// </summary>
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
	///   <para>The weight of animation.</para>
	/// </summary>
	public extern float weight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Wrapping mode of the animation.</para>
	/// </summary>
	public extern WrapMode wrapMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The current time of the animation.</para>
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
	///   <para>The normalized time of the animation.</para>
	/// </summary>
	public extern float normalizedTime
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The playback speed of the animation. 1 is normal playback speed.</para>
	/// </summary>
	public extern float speed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The normalized playback speed.</para>
	/// </summary>
	public extern float normalizedSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The length of the animation clip in seconds.</para>
	/// </summary>
	public extern float length
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public extern int layer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The clip that is being played by this animation state.</para>
	/// </summary>
	public extern AnimationClip clip
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The name of the animation.</para>
	/// </summary>
	public extern string name
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Which blend mode should be used?</para>
	/// </summary>
	public extern AnimationBlendMode blendMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Adds a transform which should be animated. This allows you to reduce the number of animations you have to create.</para>
	/// </summary>
	/// <param name="mix">The transform to animate.</param>
	/// <param name="recursive">Whether to also animate all children of the specified transform.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void AddMixingTransform(Transform mix, [DefaultValue("true")] bool recursive);

	/// <summary>
	///   <para>Adds a transform which should be animated. This allows you to reduce the number of animations you have to create.</para>
	/// </summary>
	/// <param name="mix">The transform to animate.</param>
	/// <param name="recursive">Whether to also animate all children of the specified transform.</param>
	[ExcludeFromDocs]
	public void AddMixingTransform(Transform mix)
	{
		bool recursive = true;
		AddMixingTransform(mix, recursive);
	}

	/// <summary>
	///   <para>Removes a transform which should be animated.</para>
	/// </summary>
	/// <param name="mix"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RemoveMixingTransform(Transform mix);
}

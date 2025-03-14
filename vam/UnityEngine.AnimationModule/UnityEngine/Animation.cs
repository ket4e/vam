using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>The animation component is used to play back animations.</para>
/// </summary>
public sealed class Animation : Behaviour, IEnumerable
{
	private sealed class Enumerator : IEnumerator
	{
		private Animation m_Outer;

		private int m_CurrentIndex = -1;

		public object Current => m_Outer.GetStateAtIndex(m_CurrentIndex);

		internal Enumerator(Animation outer)
		{
			m_Outer = outer;
		}

		public bool MoveNext()
		{
			int stateCount = m_Outer.GetStateCount();
			m_CurrentIndex++;
			return m_CurrentIndex < stateCount;
		}

		public void Reset()
		{
			m_CurrentIndex = -1;
		}
	}

	/// <summary>
	///   <para>The default animation.</para>
	/// </summary>
	public extern AnimationClip clip
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Should the default animation clip (the Animation.clip property) automatically start playing on startup?</para>
	/// </summary>
	public extern bool playAutomatically
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>How should time beyond the playback range of the clip be treated?</para>
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
	///   <para>Are we playing any animations?</para>
	/// </summary>
	public extern bool isPlaying
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public AnimationState this[string name] => GetState(name);

	/// <summary>
	///   <para>When turned on, animations will be executed in the physics loop. This is only useful in conjunction with kinematic rigidbodies.</para>
	/// </summary>
	public extern bool animatePhysics
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>When turned on, Unity might stop animating if it thinks that the results of the animation won't be visible to the user.</para>
	/// </summary>
	[Obsolete("Use cullingType instead")]
	public extern bool animateOnlyIfVisible
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Controls culling of this Animation component.</para>
	/// </summary>
	public extern AnimationCullingType cullingType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>AABB of this Animation animation component in local space.</para>
	/// </summary>
	public Bounds localBounds
	{
		get
		{
			INTERNAL_get_localBounds(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_localBounds(ref value);
		}
	}

	/// <summary>
	///   <para>Stops all playing animations that were started with this Animation.</para>
	/// </summary>
	public void Stop()
	{
		INTERNAL_CALL_Stop(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Stop(Animation self);

	/// <summary>
	///   <para>Stops an animation named name.</para>
	/// </summary>
	/// <param name="name"></param>
	public void Stop(string name)
	{
		Internal_StopByName(name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_StopByName(string name);

	/// <summary>
	///   <para>Rewinds the animation named name.</para>
	/// </summary>
	/// <param name="name"></param>
	public void Rewind(string name)
	{
		Internal_RewindByName(name);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_RewindByName(string name);

	/// <summary>
	///   <para>Rewinds all animations.</para>
	/// </summary>
	public void Rewind()
	{
		INTERNAL_CALL_Rewind(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Rewind(Animation self);

	/// <summary>
	///   <para>Samples animations at the current state.</para>
	/// </summary>
	public void Sample()
	{
		INTERNAL_CALL_Sample(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Sample(Animation self);

	/// <summary>
	///   <para>Is the animation named name playing?</para>
	/// </summary>
	/// <param name="name"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool IsPlaying(string name);

	[ExcludeFromDocs]
	public bool Play()
	{
		PlayMode mode = PlayMode.StopSameLayer;
		return Play(mode);
	}

	/// <summary>
	///   <para>Plays an animation without any blending.</para>
	/// </summary>
	/// <param name="mode"></param>
	/// <param name="animation"></param>
	public bool Play([DefaultValue("PlayMode.StopSameLayer")] PlayMode mode)
	{
		return PlayDefaultAnimation(mode);
	}

	/// <summary>
	///   <para>Plays an animation without any blending.</para>
	/// </summary>
	/// <param name="mode"></param>
	/// <param name="animation"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool Play(string animation, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

	/// <summary>
	///   <para>Plays an animation without any blending.</para>
	/// </summary>
	/// <param name="mode"></param>
	/// <param name="animation"></param>
	[ExcludeFromDocs]
	public bool Play(string animation)
	{
		PlayMode mode = PlayMode.StopSameLayer;
		return Play(animation, mode);
	}

	/// <summary>
	///   <para>Fades the animation with name animation in over a period of time seconds and fades other animations out.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="fadeLength"></param>
	/// <param name="mode"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void CrossFade(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

	/// <summary>
	///   <para>Fades the animation with name animation in over a period of time seconds and fades other animations out.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="fadeLength"></param>
	/// <param name="mode"></param>
	[ExcludeFromDocs]
	public void CrossFade(string animation, float fadeLength)
	{
		PlayMode mode = PlayMode.StopSameLayer;
		CrossFade(animation, fadeLength, mode);
	}

	/// <summary>
	///   <para>Fades the animation with name animation in over a period of time seconds and fades other animations out.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="fadeLength"></param>
	/// <param name="mode"></param>
	[ExcludeFromDocs]
	public void CrossFade(string animation)
	{
		PlayMode mode = PlayMode.StopSameLayer;
		float fadeLength = 0.3f;
		CrossFade(animation, fadeLength, mode);
	}

	/// <summary>
	///   <para>Blends the animation named animation towards targetWeight over the next time seconds.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="targetWeight"></param>
	/// <param name="fadeLength"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Blend(string animation, [DefaultValue("1.0F")] float targetWeight, [DefaultValue("0.3F")] float fadeLength);

	/// <summary>
	///   <para>Blends the animation named animation towards targetWeight over the next time seconds.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="targetWeight"></param>
	/// <param name="fadeLength"></param>
	[ExcludeFromDocs]
	public void Blend(string animation, float targetWeight)
	{
		float fadeLength = 0.3f;
		Blend(animation, targetWeight, fadeLength);
	}

	/// <summary>
	///   <para>Blends the animation named animation towards targetWeight over the next time seconds.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="targetWeight"></param>
	/// <param name="fadeLength"></param>
	[ExcludeFromDocs]
	public void Blend(string animation)
	{
		float fadeLength = 0.3f;
		float targetWeight = 1f;
		Blend(animation, targetWeight, fadeLength);
	}

	/// <summary>
	///   <para>Cross fades an animation after previous animations has finished playing.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="fadeLength"></param>
	/// <param name="queue"></param>
	/// <param name="mode"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern AnimationState CrossFadeQueued(string animation, [DefaultValue("0.3F")] float fadeLength, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

	/// <summary>
	///   <para>Cross fades an animation after previous animations has finished playing.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="fadeLength"></param>
	/// <param name="queue"></param>
	/// <param name="mode"></param>
	[ExcludeFromDocs]
	public AnimationState CrossFadeQueued(string animation, float fadeLength, QueueMode queue)
	{
		PlayMode mode = PlayMode.StopSameLayer;
		return CrossFadeQueued(animation, fadeLength, queue, mode);
	}

	/// <summary>
	///   <para>Cross fades an animation after previous animations has finished playing.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="fadeLength"></param>
	/// <param name="queue"></param>
	/// <param name="mode"></param>
	[ExcludeFromDocs]
	public AnimationState CrossFadeQueued(string animation, float fadeLength)
	{
		PlayMode mode = PlayMode.StopSameLayer;
		QueueMode queue = QueueMode.CompleteOthers;
		return CrossFadeQueued(animation, fadeLength, queue, mode);
	}

	/// <summary>
	///   <para>Cross fades an animation after previous animations has finished playing.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="fadeLength"></param>
	/// <param name="queue"></param>
	/// <param name="mode"></param>
	[ExcludeFromDocs]
	public AnimationState CrossFadeQueued(string animation)
	{
		PlayMode mode = PlayMode.StopSameLayer;
		QueueMode queue = QueueMode.CompleteOthers;
		float fadeLength = 0.3f;
		return CrossFadeQueued(animation, fadeLength, queue, mode);
	}

	/// <summary>
	///   <para>Plays an animation after previous animations has finished playing.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="queue"></param>
	/// <param name="mode"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern AnimationState PlayQueued(string animation, [DefaultValue("QueueMode.CompleteOthers")] QueueMode queue, [DefaultValue("PlayMode.StopSameLayer")] PlayMode mode);

	/// <summary>
	///   <para>Plays an animation after previous animations has finished playing.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="queue"></param>
	/// <param name="mode"></param>
	[ExcludeFromDocs]
	public AnimationState PlayQueued(string animation, QueueMode queue)
	{
		PlayMode mode = PlayMode.StopSameLayer;
		return PlayQueued(animation, queue, mode);
	}

	/// <summary>
	///   <para>Plays an animation after previous animations has finished playing.</para>
	/// </summary>
	/// <param name="animation"></param>
	/// <param name="queue"></param>
	/// <param name="mode"></param>
	[ExcludeFromDocs]
	public AnimationState PlayQueued(string animation)
	{
		PlayMode mode = PlayMode.StopSameLayer;
		QueueMode queue = QueueMode.CompleteOthers;
		return PlayQueued(animation, queue, mode);
	}

	/// <summary>
	///   <para>Adds a clip to the animation with name newName.</para>
	/// </summary>
	/// <param name="clip"></param>
	/// <param name="newName"></param>
	public void AddClip(AnimationClip clip, string newName)
	{
		AddClip(clip, newName, int.MinValue, int.MaxValue);
	}

	/// <summary>
	///   <para>Adds clip to the only play between firstFrame and lastFrame. The new clip will also be added to the animation with name newName.</para>
	/// </summary>
	/// <param name="addLoopFrame">Should an extra frame be inserted at the end that matches the first frame? Turn this on if you are making a looping animation.</param>
	/// <param name="clip"></param>
	/// <param name="newName"></param>
	/// <param name="firstFrame"></param>
	/// <param name="lastFrame"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame, [DefaultValue("false")] bool addLoopFrame);

	/// <summary>
	///   <para>Adds clip to the only play between firstFrame and lastFrame. The new clip will also be added to the animation with name newName.</para>
	/// </summary>
	/// <param name="addLoopFrame">Should an extra frame be inserted at the end that matches the first frame? Turn this on if you are making a looping animation.</param>
	/// <param name="clip"></param>
	/// <param name="newName"></param>
	/// <param name="firstFrame"></param>
	/// <param name="lastFrame"></param>
	[ExcludeFromDocs]
	public void AddClip(AnimationClip clip, string newName, int firstFrame, int lastFrame)
	{
		bool addLoopFrame = false;
		AddClip(clip, newName, firstFrame, lastFrame, addLoopFrame);
	}

	/// <summary>
	///   <para>Remove clip from the animation list.</para>
	/// </summary>
	/// <param name="clip"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void RemoveClip(AnimationClip clip);

	/// <summary>
	///   <para>Remove clip from the animation list.</para>
	/// </summary>
	/// <param name="clipName"></param>
	public void RemoveClip(string clipName)
	{
		RemoveClip2(clipName);
	}

	/// <summary>
	///   <para>Get the number of clips currently assigned to this animation.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern int GetClipCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void RemoveClip2(string clipName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern bool PlayDefaultAnimation(PlayMode mode);

	[Obsolete("use PlayMode instead of AnimationPlayMode.")]
	public bool Play(AnimationPlayMode mode)
	{
		return PlayDefaultAnimation((PlayMode)mode);
	}

	[Obsolete("use PlayMode instead of AnimationPlayMode.")]
	public bool Play(string animation, AnimationPlayMode mode)
	{
		return Play(animation, (PlayMode)mode);
	}

	public void SyncLayer(int layer)
	{
		INTERNAL_CALL_SyncLayer(this, layer);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SyncLayer(Animation self, int layer);

	public IEnumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern AnimationState GetState(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern AnimationState GetStateAtIndex(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern int GetStateCount();

	public AnimationClip GetClip(string name)
	{
		AnimationState state = GetState(name);
		if ((bool)state)
		{
			return state.clip;
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_localBounds(out Bounds value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_localBounds(ref Bounds value);
}

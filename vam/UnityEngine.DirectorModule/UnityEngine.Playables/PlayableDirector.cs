using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables;

/// <summary>
///   <para>Instantiates a PlayableAsset and controls playback of Playable objects.</para>
/// </summary>
[NativeHeader("Modules/Director/PlayableDirector.h")]
[RequiredByNativeCode]
[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
public class PlayableDirector : Behaviour, IExposedPropertyTable
{
	/// <summary>
	///   <para>The current playing state of the component. (Read Only)</para>
	/// </summary>
	public PlayState state => GetPlayState();

	/// <summary>
	///   <para>Controls how the time is incremented when it goes beyond the duration of the playable.</para>
	/// </summary>
	public DirectorWrapMode extrapolationMode
	{
		get
		{
			return GetWrapMode();
		}
		set
		{
			SetWrapMode(value);
		}
	}

	/// <summary>
	///   <para>The PlayableAsset that is used to instantiate a playable for playback.</para>
	/// </summary>
	public PlayableAsset playableAsset
	{
		get
		{
			return Internal_GetPlayableAsset() as PlayableAsset;
		}
		set
		{
			SetPlayableAsset(value);
		}
	}

	/// <summary>
	///   <para>The PlayableGraph created by the PlayableDirector.</para>
	/// </summary>
	public PlayableGraph playableGraph => GetGraphHandle();

	/// <summary>
	///   <para>Whether the playable asset will start playing back as soon as the component awakes.</para>
	/// </summary>
	public bool playOnAwake
	{
		get
		{
			return GetPlayOnAwake();
		}
		set
		{
			SetPlayOnAwake(value);
		}
	}

	/// <summary>
	///   <para>Controls how time is incremented when playing back.</para>
	/// </summary>
	public extern DirectorUpdateMode timeUpdateMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The component's current time. This value is incremented according to the PlayableDirector.timeUpdateMode when it is playing. You can also change this value manually.</para>
	/// </summary>
	public extern double time
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The time at which the Playable should start when first played.</para>
	/// </summary>
	public extern double initialTime
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The duration of the Playable in seconds.</para>
	/// </summary>
	public extern double duration
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public event Action<PlayableDirector> played;

	public event Action<PlayableDirector> paused;

	public event Action<PlayableDirector> stopped;

	/// <summary>
	///   <para>Tells the PlayableDirector to evaluate it's PlayableGraph on the next update.</para>
	/// </summary>
	public void DeferredEvaluate()
	{
		EvaluateNextFrame();
	}

	/// <summary>
	///   <para>Instatiates a Playable using the provided PlayableAsset and starts playback.</para>
	/// </summary>
	/// <param name="asset">An asset to instantiate a playable from.</param>
	/// <param name="mode">What to do when the time passes the duration of the playable.</param>
	public void Play(PlayableAsset asset)
	{
		if (asset == null)
		{
			throw new ArgumentNullException("asset");
		}
		Play(asset, extrapolationMode);
	}

	/// <summary>
	///   <para>Instatiates a Playable using the provided PlayableAsset and starts playback.</para>
	/// </summary>
	/// <param name="asset">An asset to instantiate a playable from.</param>
	/// <param name="mode">What to do when the time passes the duration of the playable.</param>
	public void Play(PlayableAsset asset, DirectorWrapMode mode)
	{
		if (asset == null)
		{
			throw new ArgumentNullException("asset");
		}
		playableAsset = asset;
		extrapolationMode = mode;
		Play();
	}

	/// <summary>
	///   <para>Sets the binding of a reference object from a PlayableBinding.</para>
	/// </summary>
	/// <param name="key">The source object in the PlayableBinding.</param>
	/// <param name="value">The object to bind to the key.</param>
	public void SetGenericBinding(Object key, Object value)
	{
		Internal_SetGenericBinding(key, value);
	}

	/// <summary>
	///   <para>Evaluates the currently playing Playable at  the current time.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Evaluate();

	/// <summary>
	///   <para>Instatiates a Playable using the provided PlayableAsset and starts playback.</para>
	/// </summary>
	/// <param name="asset">An asset to instantiate a playable from.</param>
	/// <param name="mode">What to do when the time passes the duration of the playable.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Play();

	/// <summary>
	///   <para>Stops playback of the current Playable and destroys the corresponding graph.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Stop();

	/// <summary>
	///   <para>Pauses playback of the currently running playable.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Pause();

	/// <summary>
	///   <para>Resume playing a paused playable.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Resume();

	/// <summary>
	///   <para>Discards the existing PlayableGraph and creates a new instance.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void RebuildGraph();

	/// <summary>
	///   <para>Clears an exposed reference value.</para>
	/// </summary>
	/// <param name="id">Identifier of the ExposedReference.</param>
	public void ClearReferenceValue(PropertyName id)
	{
		ClearReferenceValue_Injected(ref id);
	}

	/// <summary>
	///   <para>Sets an ExposedReference value.</para>
	/// </summary>
	/// <param name="id">Identifier of the ExposedReference.</param>
	/// <param name="value">The object to bind to set the reference value to.</param>
	public void SetReferenceValue(PropertyName id, Object value)
	{
		SetReferenceValue_Injected(ref id, value);
	}

	public Object GetReferenceValue(PropertyName id, out bool idValid)
	{
		return GetReferenceValue_Injected(ref id, out idValid);
	}

	/// <summary>
	///   <para>Returns a binding to a reference object.</para>
	/// </summary>
	/// <param name="key">The object that acts as a key.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetBindingFor")]
	public extern Object GetGenericBinding(Object key);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void ProcessPendingGraphChanges();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("HasBinding")]
	internal extern bool HasGenericBinding(Object key);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern PlayState GetPlayState();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetWrapMode(DirectorWrapMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern DirectorWrapMode GetWrapMode();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void EvaluateNextFrame();

	private PlayableGraph GetGraphHandle()
	{
		GetGraphHandle_Injected(out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetPlayOnAwake(bool on);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool GetPlayOnAwake();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void Internal_SetGenericBinding(Object key, Object value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetPlayableAsset(ScriptableObject asset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern ScriptableObject Internal_GetPlayableAsset();

	[RequiredByNativeCode]
	private void SendOnPlayableDirectorPlay()
	{
		if (this.played != null)
		{
			this.played(this);
		}
	}

	[RequiredByNativeCode]
	private void SendOnPlayableDirectorPause()
	{
		if (this.paused != null)
		{
			this.paused(this);
		}
	}

	[RequiredByNativeCode]
	private void SendOnPlayableDirectorStop()
	{
		if (this.stopped != null)
		{
			this.stopped(this);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void ClearReferenceValue_Injected(ref PropertyName id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetReferenceValue_Injected(ref PropertyName id, Object value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Object GetReferenceValue_Injected(ref PropertyName id, out bool idValid);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetGraphHandle_Injected(out PlayableGraph ret);
}

using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace UnityEngine.Timeline;

[Serializable]
[IgnoreOnPlayableTrack]
public abstract class TrackAsset : PlayableAsset, IPropertyPreview, ISerializationCallbackReceiver
{
	protected internal enum Versions
	{
		Initial,
		RotationAsEuler
	}

	private static class TrackAssetUpgrade
	{
	}

	[SerializeField]
	[HideInInspector]
	private bool m_Locked;

	[SerializeField]
	[HideInInspector]
	private bool m_Muted;

	[SerializeField]
	[HideInInspector]
	private string m_CustomPlayableFullTypename = string.Empty;

	[SerializeField]
	[FormerlySerializedAs("m_animClip")]
	[HideInInspector]
	private AnimationClip m_AnimClip;

	[SerializeField]
	[HideInInspector]
	private PlayableAsset m_Parent;

	[SerializeField]
	[HideInInspector]
	private List<ScriptableObject> m_Children;

	[NonSerialized]
	private int m_ItemsHash;

	[NonSerialized]
	private TimelineClip[] m_ClipsCache;

	private DiscreteTime m_Start;

	private DiscreteTime m_End;

	private TimelineAsset.MediaType m_MediaType;

	private bool m_CacheSorted;

	private static TrackAsset[] s_EmptyCache = new TrackAsset[0];

	private IEnumerable<TrackAsset> m_ChildTrackCache;

	private static Dictionary<Type, TrackBindingTypeAttribute> s_TrackBindingTypeAttributeCache = new Dictionary<Type, TrackBindingTypeAttribute>();

	[SerializeField]
	[HideInInspector]
	protected internal List<TimelineClip> m_Clips = new List<TimelineClip>();

	protected internal const int k_LatestVersion = 1;

	[SerializeField]
	[HideInInspector]
	private int m_Version;

	public double start
	{
		get
		{
			UpdateDuration();
			return (double)m_Start;
		}
	}

	public double end
	{
		get
		{
			UpdateDuration();
			return (double)m_End;
		}
	}

	public sealed override double duration
	{
		get
		{
			UpdateDuration();
			return (double)(m_End - m_Start);
		}
	}

	public bool muted
	{
		get
		{
			return m_Muted;
		}
		set
		{
			m_Muted = value;
		}
	}

	public TimelineAsset timelineAsset
	{
		get
		{
			TrackAsset trackAsset = this;
			while (trackAsset != null)
			{
				if (trackAsset.parent == null)
				{
					return null;
				}
				TimelineAsset timelineAsset = trackAsset.parent as TimelineAsset;
				if (timelineAsset != null)
				{
					return timelineAsset;
				}
				trackAsset = trackAsset.parent as TrackAsset;
			}
			return null;
		}
	}

	public PlayableAsset parent
	{
		get
		{
			return m_Parent;
		}
		internal set
		{
			m_Parent = value;
		}
	}

	internal TimelineClip[] clips
	{
		get
		{
			if (m_Clips == null)
			{
				m_Clips = new List<TimelineClip>();
			}
			if (m_ClipsCache == null)
			{
				m_CacheSorted = false;
				m_ClipsCache = m_Clips.ToArray();
			}
			return m_ClipsCache;
		}
	}

	public virtual bool isEmpty => !hasClips;

	internal bool hasClips => m_Clips != null && m_Clips.Count != 0;

	public bool isSubTrack
	{
		get
		{
			TrackAsset trackAsset = parent as TrackAsset;
			return trackAsset != null && trackAsset.GetType() == GetType();
		}
	}

	public override IEnumerable<PlayableBinding> outputs
	{
		get
		{
			if (!s_TrackBindingTypeAttributeCache.TryGetValue(GetType(), out var attribute))
			{
				attribute = (TrackBindingTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(TrackBindingTypeAttribute));
				s_TrackBindingTypeAttributeCache.Add(GetType(), attribute);
			}
			Type trackBindingType = attribute?.type;
			yield return new PlayableBinding
			{
				sourceObject = this,
				streamName = base.name,
				streamType = DataStreamType.None,
				sourceBindingType = trackBindingType
			};
		}
	}

	internal string customPlayableTypename
	{
		get
		{
			return m_CustomPlayableFullTypename;
		}
		set
		{
			m_CustomPlayableFullTypename = value;
		}
	}

	internal AnimationClip animClip
	{
		get
		{
			return m_AnimClip;
		}
		set
		{
			m_AnimClip = value;
		}
	}

	internal List<ScriptableObject> subTracksObjects => m_Children;

	internal bool locked
	{
		get
		{
			return m_Locked;
		}
		set
		{
			m_Locked = value;
		}
	}

	internal TimelineAsset.MediaType mediaType => m_MediaType;

	internal virtual bool compilable => !muted && !isEmpty;

	internal static event Action<TimelineClip, GameObject, Playable> OnPlayableCreate;

	protected TrackAsset()
	{
		m_MediaType = GetMediaType(GetType());
	}

	public IEnumerable<TimelineClip> GetClips()
	{
		return clips;
	}

	public IEnumerable<TrackAsset> GetChildTracks()
	{
		UpdateChildTrackCache();
		return m_ChildTrackCache;
	}

	private void __internalAwake()
	{
		if (m_Clips == null)
		{
			m_Clips = new List<TimelineClip>();
		}
		m_ChildTrackCache = null;
		if (m_Children == null)
		{
			m_Children = new List<ScriptableObject>();
		}
		for (int num = m_Children.Count - 1; num >= 0; num--)
		{
		}
	}

	public virtual Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		return Playable.Create(graph, inputCount);
	}

	public sealed override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		return Playable.Null;
	}

	public TimelineClip CreateDefaultClip()
	{
		object[] customAttributes = GetType().GetCustomAttributes(typeof(TrackClipTypeAttribute), inherit: true);
		Type type = null;
		object[] array = customAttributes;
		foreach (object obj in array)
		{
			if (obj is TrackClipTypeAttribute trackClipTypeAttribute && typeof(IPlayableAsset).IsAssignableFrom(trackClipTypeAttribute.inspectedType) && typeof(ScriptableObject).IsAssignableFrom(trackClipTypeAttribute.inspectedType))
			{
				type = trackClipTypeAttribute.inspectedType;
				break;
			}
		}
		if (type == null)
		{
			Debug.LogWarning("Cannot create a default clip for type " + GetType());
			return null;
		}
		return CreateAndAddNewClipOfType(type);
	}

	public TimelineClip CreateClip<T>() where T : ScriptableObject, IPlayableAsset
	{
		Type typeFromHandle = typeof(T);
		if (ValidateClipType(typeFromHandle))
		{
			return CreateAndAddNewClipOfType(typeFromHandle);
		}
		throw new InvalidOperationException(string.Concat("Clips of type ", typeFromHandle, " are not permitted on tracks of type ", GetType()));
	}

	private TimelineClip CreateAndAddNewClipOfType(Type requestedType)
	{
		ScriptableObject scriptableObject = ScriptableObject.CreateInstance(requestedType);
		if (scriptableObject == null)
		{
			throw new InvalidOperationException("Could not create an instance of the ScriptableObject type " + requestedType.Name);
		}
		scriptableObject.name = requestedType.Name;
		TimelineCreateUtilities.SaveAssetIntoObject(scriptableObject, this);
		TimelineClip timelineClip = CreateNewClipContainerInternal();
		timelineClip.displayName = scriptableObject.name;
		timelineClip.asset = scriptableObject;
		try
		{
			OnCreateClipFromAsset(scriptableObject, timelineClip);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message, scriptableObject);
			return null;
		}
		AddClip(timelineClip);
		return timelineClip;
	}

	internal void AddClip(TimelineClip newClip)
	{
		if (!m_Clips.Contains(newClip))
		{
			m_Clips.Add(newClip);
			m_ClipsCache = null;
		}
	}

	internal Playable CreatePlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
	{
		UpdateDuration();
		return OnCreatePlayableGraph(graph, go, tree);
	}

	internal virtual Playable OnCreatePlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
	{
		if (tree == null)
		{
			throw new ArgumentException("IntervalTree argument cannot be null", "tree");
		}
		if (go == null)
		{
			throw new ArgumentException("GameObject argument cannot be null", "go");
		}
		Playable playable = CreateTrackMixer(graph, go, clips.Length);
		for (int i = 0; i < clips.Length; i++)
		{
			Playable playable2 = CreatePlayable(graph, go, clips[i]);
			if (playable2.IsValid())
			{
				playable2.SetDuration(clips[i].duration);
				RuntimeClip item = new RuntimeClip(clips[i], playable2, playable);
				tree.Add(item);
				graph.Connect(playable2, 0, playable, i);
				playable.SetInputWeight(i, 0f);
			}
		}
		return playable;
	}

	internal void SortClips()
	{
		TimelineClip[] array = clips;
		if (!m_CacheSorted)
		{
			Array.Sort(clips, (TimelineClip clip1, TimelineClip clip2) => clip1.start.CompareTo(clip2.start));
			m_CacheSorted = true;
		}
	}

	internal void ClearClipsInternal()
	{
		m_Clips = new List<TimelineClip>();
		m_ClipsCache = null;
	}

	internal void ClearSubTracksInternal()
	{
		m_Children = new List<ScriptableObject>();
		Invalidate();
	}

	internal void OnClipMove()
	{
		m_CacheSorted = false;
	}

	internal TimelineClip CreateClipFromAsset(Object asset)
	{
		if (asset == null)
		{
			return null;
		}
		TimelineAsset timelineAsset = asset as TimelineAsset;
		if (timelineAsset == null && !ValidateClipType(asset.GetType()))
		{
			throw new InvalidOperationException($"Cannot create a clip for track {GetType()} with clip type: {asset.GetType()}. Did you forget a ClipClass attribute?");
		}
		TimelineClip timelineClip = CreateNewClipContainerInternal();
		timelineClip.displayName = asset.name;
		timelineClip.asset = asset;
		try
		{
			OnCreateClipFromAsset(asset, timelineClip);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message, asset);
			RemoveClip(timelineClip);
			return null;
		}
		return timelineClip;
	}

	internal virtual void OnCreateClipFromAsset(Object asset, TimelineClip clip)
	{
		clip.asset = asset;
		clip.displayName = asset.name;
		if (asset is IPlayableAsset playableAsset)
		{
			double num = playableAsset.duration;
			if (num > 0.0 && !double.IsInfinity(num))
			{
				clip.duration = num;
			}
		}
	}

	internal TimelineClip CreateNewClipContainerInternal()
	{
		TimelineClip timelineClip = new TimelineClip(this);
		timelineClip.asset = null;
		double val = 0.0;
		for (int i = 0; i < m_Clips.Count - 1; i++)
		{
			double num = m_Clips[i].duration;
			if (double.IsInfinity(num))
			{
				num = TimelineClip.kDefaultClipDurationInSeconds;
			}
			val = Math.Max(val, m_Clips[i].start + num);
		}
		timelineClip.mixInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		timelineClip.mixOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
		timelineClip.start = val;
		timelineClip.duration = TimelineClip.kDefaultClipDurationInSeconds;
		timelineClip.displayName = "untitled";
		return timelineClip;
	}

	internal void AddChild(TrackAsset child)
	{
		if (!(child == null))
		{
			m_Children.Add(child);
			child.parent = this;
			Invalidate();
		}
	}

	internal bool AddChildAfter(TrackAsset child, TrackAsset other)
	{
		if (child == null)
		{
			return false;
		}
		int num = m_Children.IndexOf(other);
		if (num >= 0 && num != m_Children.Count - 1)
		{
			m_Children.Insert(num + 1, child);
		}
		else
		{
			m_Children.Add(child);
		}
		Invalidate();
		child.parent = this;
		return true;
	}

	internal bool RemoveSubTrack(TrackAsset child)
	{
		if (m_Children.Remove(child))
		{
			Invalidate();
			child.parent = null;
			return true;
		}
		return false;
	}

	internal void RemoveClip(TimelineClip clip)
	{
		m_Clips.Remove(clip);
		m_ClipsCache = null;
	}

	internal virtual void GetEvaluationTime(out double outStart, out double outDuration)
	{
		if (clips.Length == 0)
		{
			outStart = 0.0;
			outDuration = GetMarkerDuration();
			return;
		}
		outStart = double.MaxValue;
		double num = 0.0;
		for (int i = 0; i < clips.Length; i++)
		{
			outStart = Math.Min(clips[i].start, outStart);
			num = Math.Max(clips[i].start + clips[i].duration, num);
		}
		outStart = Math.Max(outStart, 0.0);
		outDuration = Math.Max(GetMarkerDuration(), num - outStart);
	}

	internal virtual void GetSequenceTime(out double outStart, out double outDuration)
	{
		GetEvaluationTime(out outStart, out outDuration);
	}

	public virtual void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		GameObject gameObjectBinding = GetGameObjectBinding(director);
		if (gameObjectBinding != null)
		{
			driver.PushActiveGameObject(gameObjectBinding);
		}
		if (animClip != null)
		{
			driver.AddFromClip(animClip);
		}
		TimelineClip[] array = clips;
		foreach (TimelineClip timelineClip in array)
		{
			if (timelineClip.curves != null && timelineClip.asset != null)
			{
				driver.AddObjectProperties(timelineClip.asset, timelineClip.curves);
			}
			if (timelineClip.asset is IPropertyPreview propertyPreview)
			{
				propertyPreview.GatherProperties(director, driver);
			}
		}
		foreach (TrackAsset childTrack in GetChildTracks())
		{
			if (childTrack != null)
			{
				childTrack.GatherProperties(director, driver);
			}
		}
		if (gameObjectBinding != null)
		{
			driver.PopActiveGameObject();
		}
	}

	internal GameObject GetGameObjectBinding(PlayableDirector director)
	{
		if (director == null)
		{
			return null;
		}
		Object genericBinding = director.GetGenericBinding(this);
		GameObject gameObject = genericBinding as GameObject;
		if (gameObject != null)
		{
			return gameObject;
		}
		Component component = genericBinding as Component;
		if (component != null)
		{
			return component.gameObject;
		}
		return null;
	}

	internal bool ValidateClipType(Type clipType)
	{
		object[] customAttributes = GetType().GetCustomAttributes(typeof(TrackClipTypeAttribute), inherit: true);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			TrackClipTypeAttribute trackClipTypeAttribute = (TrackClipTypeAttribute)customAttributes[i];
			if (trackClipTypeAttribute.inspectedType.IsAssignableFrom(clipType))
			{
				return true;
			}
		}
		return typeof(PlayableTrack).IsAssignableFrom(GetType()) && typeof(IPlayableAsset).IsAssignableFrom(clipType) && typeof(ScriptableObject).IsAssignableFrom(clipType);
	}

	protected internal virtual void UpdateDuration()
	{
		int h = ((m_AnimClip != null) ? ((int)(m_AnimClip.frameRate * m_AnimClip.length)) : 0);
		int num = HashUtility.CombineHash(GetClipsHash(), GetMarkerHash(), h);
		if (num != m_ItemsHash)
		{
			m_ItemsHash = num;
			GetSequenceTime(out var outStart, out var outDuration);
			m_Start = (DiscreteTime)outStart;
			m_End = (DiscreteTime)(outStart + outDuration);
			this.CalculateExtrapolationTimes();
		}
	}

	protected internal Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
	{
		if (clip.asset is IPlayableAsset playableAsset)
		{
			Playable playable = playableAsset.CreatePlayable(graph, go);
			if (playable.IsValid())
			{
				playable.SetAnimatedProperties(clip.curves);
				playable.SetSpeed(clip.timeScale);
				if (TrackAsset.OnPlayableCreate != null)
				{
					TrackAsset.OnPlayableCreate(clip, go, playable);
				}
			}
			return playable;
		}
		return Playable.Null;
	}

	internal void Invalidate()
	{
		m_ChildTrackCache = null;
		TimelineAsset timelineAsset = this.timelineAsset;
		if (timelineAsset != null)
		{
			timelineAsset.Invalidate();
		}
	}

	private void UpdateChildTrackCache()
	{
		if (m_ChildTrackCache != null)
		{
			return;
		}
		if (m_Children == null || m_Children.Count == 0)
		{
			m_ChildTrackCache = s_EmptyCache;
			return;
		}
		List<TrackAsset> list = new List<TrackAsset>(m_Children.Count);
		for (int i = 0; i < m_Children.Count; i++)
		{
			TrackAsset trackAsset = m_Children[i] as TrackAsset;
			if (trackAsset != null)
			{
				list.Add(trackAsset);
			}
		}
		m_ChildTrackCache = list;
	}

	protected internal virtual int Hash()
	{
		return clips.Length + (GetMarkerContainerHash() << 16);
	}

	private int GetClipsHash()
	{
		int num = 0;
		foreach (TimelineClip clip in m_Clips)
		{
			num = num.CombineHash(((ITimelineItem)clip).Hash());
		}
		return num;
	}

	private int GetMarkerContainerHash()
	{
		if (!(this is ITimelineMarkerContainer timelineMarkerContainer))
		{
			return 0;
		}
		TimelineMarker[] markers = timelineMarkerContainer.GetMarkers();
		return (markers != null) ? markers.Length : 0;
	}

	private int GetMarkerHash()
	{
		ITimelineMarkerContainer timelineMarkerContainer = this as ITimelineMarkerContainer;
		int num = 0;
		if (timelineMarkerContainer != null)
		{
			TimelineMarker[] markers = timelineMarkerContainer.GetMarkers();
			if (markers != null)
			{
				for (int i = 0; i < markers.Length; i++)
				{
					num = num.CombineHash(((ITimelineItem)markers[i]).Hash());
				}
			}
		}
		return num;
	}

	private double GetMarkerDuration()
	{
		ITimelineMarkerContainer timelineMarkerContainer = this as ITimelineMarkerContainer;
		double num = 0.0;
		if (timelineMarkerContainer != null)
		{
			TimelineMarker[] markers = timelineMarkerContainer.GetMarkers();
			if (markers != null)
			{
				for (int i = 0; i < markers.Length; i++)
				{
					num = Math.Max(num, markers[i].time);
				}
			}
		}
		return num;
	}

	private static TimelineAsset.MediaType GetMediaType(Type t)
	{
		object[] customAttributes = t.GetCustomAttributes(typeof(TrackMediaType), inherit: true);
		if (customAttributes.Length > 0 && customAttributes[0] is TrackMediaType)
		{
			return ((TrackMediaType)customAttributes[0]).m_MediaType;
		}
		return TimelineAsset.MediaType.Script;
	}

	protected virtual void OnBeforeTrackSerialize()
	{
	}

	protected virtual void OnAfterTrackDeserialize()
	{
	}

	protected internal virtual void OnUpgradeFromVersion(int oldVersion)
	{
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		m_Version = 1;
		for (int num = m_Children.Count - 1; num >= 0; num--)
		{
			TrackAsset trackAsset = m_Children[num] as TrackAsset;
			if (trackAsset != null && trackAsset.parent != this)
			{
				trackAsset.parent = this;
			}
		}
		OnBeforeTrackSerialize();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		m_ClipsCache = null;
		Invalidate();
		if (m_Version < 1)
		{
			UpgradeToLatestVersion();
			OnUpgradeFromVersion(m_Version);
		}
		OnAfterTrackDeserialize();
	}

	private void UpgradeToLatestVersion()
	{
	}
}

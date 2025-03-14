using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

[Serializable]
public class TimelineAsset : PlayableAsset, ISerializationCallbackReceiver, ITimelineClipAsset, IPropertyPreview
{
	public enum MediaType
	{
		Animation = 0,
		Audio = 1,
		Texture = 2,
		[Obsolete("Use Texture MediaType instead. (UnityUpgradable) -> UnityEngine.Timeline.TimelineAsset/MediaType.Texture", false)]
		Video = 2,
		Script = 3,
		Hybrid = 4,
		Group = 5
	}

	public enum DurationMode
	{
		BasedOnClips,
		FixedLength
	}

	[Serializable]
	public class EditorSettings
	{
		internal static readonly float kMinFps = (float)TimeUtility.kFrameRateEpsilon;

		internal static readonly float kMaxFps = 1000f;

		internal static readonly float kDefaultFps = 60f;

		[HideInInspector]
		[SerializeField]
		private float m_Framerate = kDefaultFps;

		public float fps
		{
			get
			{
				return m_Framerate;
			}
			set
			{
				m_Framerate = GetValidFramerate(value);
			}
		}
	}

	private enum Versions
	{
		Initial
	}

	private static class TimelineAssetUpgrade
	{
	}

	[HideInInspector]
	[SerializeField]
	private int m_NextId;

	[HideInInspector]
	[SerializeField]
	private List<ScriptableObject> m_Tracks;

	[HideInInspector]
	[SerializeField]
	private double m_FixedDuration;

	[NonSerialized]
	[HideInInspector]
	private TrackAsset[] m_CacheOutputTracks;

	[NonSerialized]
	[HideInInspector]
	private List<TrackAsset> m_CacheRootTracks;

	[NonSerialized]
	[HideInInspector]
	private List<TrackAsset> m_CacheFlattenedTracks;

	[HideInInspector]
	[SerializeField]
	private EditorSettings m_EditorSettings = new EditorSettings();

	[SerializeField]
	private DurationMode m_DurationMode;

	private const int k_LatestVersion = 0;

	[SerializeField]
	[HideInInspector]
	private int m_Version;

	public EditorSettings editorSettings => m_EditorSettings;

	public override double duration
	{
		get
		{
			if (m_DurationMode == DurationMode.BasedOnClips)
			{
				return CalculateDuration();
			}
			return m_FixedDuration;
		}
	}

	public double fixedDuration
	{
		get
		{
			DiscreteTime discreteTime = (DiscreteTime)m_FixedDuration;
			if (discreteTime <= 0)
			{
				return 0.0;
			}
			return (double)discreteTime.OneTickBefore();
		}
		set
		{
			m_FixedDuration = Math.Max(0.0, value);
		}
	}

	public DurationMode durationMode
	{
		get
		{
			return m_DurationMode;
		}
		set
		{
			m_DurationMode = value;
		}
	}

	public override IEnumerable<PlayableBinding> outputs
	{
		get
		{
			foreach (TrackAsset outputTracks in GetOutputTracks())
			{
				foreach (PlayableBinding output in outputTracks.outputs)
				{
					yield return output;
				}
			}
		}
	}

	public ClipCaps clipCaps
	{
		get
		{
			ClipCaps clipCaps = ClipCaps.All;
			foreach (TrackAsset rootTrack in GetRootTracks())
			{
				TimelineClip[] clips = rootTrack.clips;
				foreach (TimelineClip timelineClip in clips)
				{
					clipCaps &= timelineClip.clipCaps;
				}
			}
			return clipCaps;
		}
	}

	public int outputTrackCount
	{
		get
		{
			GetOutputTracks();
			return m_CacheOutputTracks.Length;
		}
	}

	public int rootTrackCount
	{
		get
		{
			UpdateRootTrackCache();
			return m_CacheRootTracks.Count;
		}
	}

	internal IEnumerable<TrackAsset> flattenedTracks
	{
		get
		{
			if (m_CacheFlattenedTracks == null)
			{
				m_CacheFlattenedTracks = new List<TrackAsset>(m_Tracks.Count * 2);
				UpdateRootTrackCache();
				m_CacheFlattenedTracks.AddRange(m_CacheRootTracks);
				for (int i = 0; i < m_CacheRootTracks.Count; i++)
				{
					AddSubTracksRecursive(m_CacheRootTracks[i], ref m_CacheFlattenedTracks);
				}
			}
			return m_CacheFlattenedTracks;
		}
	}

	internal List<ScriptableObject> trackObjects => m_Tracks;

	private void OnValidate()
	{
		editorSettings.fps = GetValidFramerate(editorSettings.fps);
	}

	private static float GetValidFramerate(float framerate)
	{
		return Mathf.Clamp(framerate, EditorSettings.kMinFps, EditorSettings.kMaxFps);
	}

	public TrackAsset GetRootTrack(int index)
	{
		UpdateRootTrackCache();
		return m_CacheRootTracks[index];
	}

	public IEnumerable<TrackAsset> GetRootTracks()
	{
		UpdateRootTrackCache();
		return m_CacheRootTracks;
	}

	public TrackAsset GetOutputTrack(int index)
	{
		UpdateOutputTrackCache();
		return m_CacheOutputTracks[index];
	}

	public IEnumerable<TrackAsset> GetOutputTracks()
	{
		UpdateOutputTrackCache();
		return m_CacheOutputTracks;
	}

	private void UpdateRootTrackCache()
	{
		if (m_CacheRootTracks != null)
		{
			return;
		}
		if (m_Tracks == null)
		{
			m_CacheRootTracks = new List<TrackAsset>();
			return;
		}
		m_CacheRootTracks = new List<TrackAsset>(m_Tracks.Count);
		for (int i = 0; i < m_Tracks.Count; i++)
		{
			TrackAsset trackAsset = m_Tracks[i] as TrackAsset;
			if (trackAsset != null)
			{
				m_CacheRootTracks.Add(trackAsset);
			}
		}
	}

	private void UpdateOutputTrackCache()
	{
		if (m_CacheOutputTracks != null)
		{
			return;
		}
		List<TrackAsset> list = new List<TrackAsset>();
		foreach (TrackAsset flattenedTrack in flattenedTracks)
		{
			if (flattenedTrack != null && flattenedTrack.mediaType != MediaType.Group && !flattenedTrack.isSubTrack)
			{
				list.Add(flattenedTrack);
			}
		}
		m_CacheOutputTracks = list.ToArray();
	}

	internal void AddTrackInternal(TrackAsset track)
	{
		m_Tracks.Add(track);
		track.parent = this;
		Invalidate();
	}

	internal void RemoveTrack(TrackAsset track)
	{
		m_Tracks.Remove(track);
		Invalidate();
		TrackAsset trackAsset = track.parent as TrackAsset;
		if (trackAsset != null)
		{
			trackAsset.RemoveSubTrack(track);
		}
	}

	internal int GenerateNewId()
	{
		m_NextId++;
		return GetInstanceID().GetHashCode().CombineHash(m_NextId.GetHashCode());
	}

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		bool autoRebalance = false;
		bool createOutputs = graph.GetPlayableCount() == 0;
		ScriptPlayable<TimelinePlayable> scriptPlayable = TimelinePlayable.Create(graph, GetOutputTracks(), go, autoRebalance, createOutputs);
		return (!scriptPlayable.IsValid()) ? Playable.Null : ((Playable)scriptPlayable);
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		m_Version = 0;
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		Invalidate();
		if (m_Version < 0)
		{
			UpgradeToLatestVersion();
		}
	}

	private void __internalAwake()
	{
		if (m_Tracks == null)
		{
			m_Tracks = new List<ScriptableObject>();
		}
		for (int num = m_Tracks.Count - 1; num >= 0; num--)
		{
			TrackAsset trackAsset = m_Tracks[num] as TrackAsset;
			if (trackAsset != null)
			{
				trackAsset.parent = this;
			}
		}
	}

	public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		IEnumerable<TrackAsset> outputTracks = GetOutputTracks();
		foreach (TrackAsset item in outputTracks)
		{
			item.GatherProperties(director, driver);
		}
	}

	internal void Invalidate()
	{
		m_CacheRootTracks = null;
		m_CacheOutputTracks = null;
		m_CacheFlattenedTracks = null;
	}

	private double CalculateDuration()
	{
		IEnumerable<TrackAsset> enumerable = flattenedTracks;
		DiscreteTime discreteTime = new DiscreteTime(0);
		foreach (TrackAsset item in enumerable)
		{
			if (!item.muted)
			{
				discreteTime = DiscreteTime.Max(discreteTime, (DiscreteTime)item.end);
			}
		}
		if (discreteTime <= 0)
		{
			return 0.0;
		}
		return (double)discreteTime.OneTickBefore();
	}

	private static void AddSubTracksRecursive(TrackAsset track, ref List<TrackAsset> allTracks)
	{
		if (track == null)
		{
			return;
		}
		allTracks.AddRange(track.GetChildTracks());
		foreach (TrackAsset childTrack in track.GetChildTracks())
		{
			AddSubTracksRecursive(childTrack, ref allTracks);
		}
	}

	public TrackAsset CreateTrack(Type type, TrackAsset parent, string name)
	{
		if (parent != null && parent.timelineAsset != this)
		{
			throw new InvalidOperationException("Addtrack cannot parent to a track not in the Timeline");
		}
		if (!typeof(TrackAsset).IsAssignableFrom(type))
		{
			throw new InvalidOperationException("Supplied type must be a track asset");
		}
		if (parent != null && !TimelineCreateUtilities.ValidateParentTrack(parent, type))
		{
			throw new InvalidOperationException("Cannot assign a child of type " + type.Name + "to a parent of type " + parent.GetType().Name);
		}
		PlayableAsset masterAsset = ((!(parent != null)) ? ((PlayableAsset)this) : ((PlayableAsset)parent));
		string text = name;
		if (string.IsNullOrEmpty(text))
		{
			text = type.Name;
		}
		string text2 = text;
		text2 = ((!(parent != null)) ? TimelineCreateUtilities.GenerateUniqueActorName(trackObjects, text) : TimelineCreateUtilities.GenerateUniqueActorName(parent.subTracksObjects, text));
		TrackAsset trackAsset = AllocateTrack(parent, text2, type);
		if (trackAsset != null)
		{
			trackAsset.name = text2;
			TimelineCreateUtilities.SaveAssetIntoObject(trackAsset, masterAsset);
		}
		return trackAsset;
	}

	public T CreateTrack<T>(TrackAsset parent, string name) where T : TrackAsset, new()
	{
		return (T)CreateTrack(typeof(T), parent, name);
	}

	public bool DeleteClip(TimelineClip clip)
	{
		if (clip == null || clip.parentTrack == null)
		{
			return false;
		}
		if (this != clip.parentTrack.timelineAsset)
		{
			Debug.LogError("Cannot delete a clip from this timeline");
			return false;
		}
		if (clip.curves != null)
		{
			TimelineUndo.PushDestroyUndo(this, clip.parentTrack, clip.curves, "Delete Curves");
		}
		if (clip.asset != null)
		{
			DeleteRecordedAnimation(clip);
			TimelineUndo.PushDestroyUndo(this, clip.parentTrack, clip.asset, "Delete Clip Asset");
		}
		TrackAsset parentTrack = clip.parentTrack;
		parentTrack.RemoveClip(clip);
		parentTrack.CalculateExtrapolationTimes();
		return true;
	}

	public bool DeleteTrack(TrackAsset track)
	{
		if (track.timelineAsset != this)
		{
			return false;
		}
		TrackAsset trackAsset = track.parent as TrackAsset;
		if (trackAsset != null)
		{
		}
		IEnumerable<TrackAsset> childTracks = track.GetChildTracks();
		foreach (TrackAsset item in childTracks)
		{
			DeleteTrack(item);
		}
		DeleteRecordingClip(track);
		List<TimelineClip> list = new List<TimelineClip>(track.clips);
		foreach (TimelineClip item2 in list)
		{
			DeleteClip(item2);
		}
		RemoveTrack(track);
		TimelineUndo.PushDestroyUndo(this, this, track, "Delete Track");
		return true;
	}

	internal TrackAsset AllocateTrack(TrackAsset trackAssetParent, string trackName, Type trackType)
	{
		if (trackAssetParent != null && trackAssetParent.timelineAsset != this)
		{
			throw new InvalidOperationException("Addtrack cannot parent to a track not in the Timeline");
		}
		if (!typeof(TrackAsset).IsAssignableFrom(trackType))
		{
			throw new InvalidOperationException("Supplied type must be a track asset");
		}
		TrackAsset trackAsset = (TrackAsset)ScriptableObject.CreateInstance(trackType);
		trackAsset.name = trackName;
		if (trackAssetParent != null)
		{
			trackAssetParent.AddChild(trackAsset);
		}
		else
		{
			AddTrackInternal(trackAsset);
		}
		return trackAsset;
	}

	private void DeleteRecordingClip(TrackAsset track)
	{
		AnimationTrack animationTrack = track as AnimationTrack;
		if (!(animationTrack == null) && !(animationTrack.animClip == null))
		{
			TimelineUndo.PushDestroyUndo(this, track, animationTrack.animClip, "Delete Track");
		}
	}

	private void DeleteRecordedAnimation(TimelineClip clip)
	{
		if (clip == null)
		{
			return;
		}
		if (clip.curves != null)
		{
			TimelineUndo.PushDestroyUndo(this, clip.parentTrack, clip.curves, "Delete Parameters");
		}
		if (clip.recordable)
		{
			AnimationPlayableAsset animationPlayableAsset = clip.asset as AnimationPlayableAsset;
			if (!(animationPlayableAsset == null) && !(animationPlayableAsset.clip == null))
			{
				TimelineUndo.PushDestroyUndo(this, animationPlayableAsset, animationPlayableAsset.clip, "Delete Recording");
			}
		}
	}

	private void UpgradeToLatestVersion()
	{
	}
}

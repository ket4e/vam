using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

[Serializable]
[TrackClipType(typeof(AnimationClip))]
[TrackClipType(typeof(AnimationPlayableAsset), false)]
[TrackMediaType(TimelineAsset.MediaType.Animation)]
[SupportsChildTracks(typeof(AnimationTrack), 1)]
public class AnimationTrack : TrackAsset
{
	private static class AnimationTrackUpgrade
	{
		public static void ConvertRotationsToEuler(AnimationTrack track)
		{
			track.m_EulerAngles = track.m_Rotation.eulerAngles;
			track.m_OpenClipOffsetEulerAngles = track.m_OpenClipOffsetRotation.eulerAngles;
		}
	}

	[SerializeField]
	private TimelineClip.ClipExtrapolation m_OpenClipPreExtrapolation = TimelineClip.ClipExtrapolation.None;

	[SerializeField]
	private TimelineClip.ClipExtrapolation m_OpenClipPostExtrapolation = TimelineClip.ClipExtrapolation.None;

	[SerializeField]
	private Vector3 m_OpenClipOffsetPosition = Vector3.zero;

	[SerializeField]
	private Vector3 m_OpenClipOffsetEulerAngles = Vector3.zero;

	[SerializeField]
	private double m_OpenClipTimeOffset;

	[SerializeField]
	private MatchTargetFields m_MatchTargetFields = MatchTargetFieldConstants.All;

	[SerializeField]
	private Vector3 m_Position = Vector3.zero;

	[SerializeField]
	private Vector3 m_EulerAngles = Vector3.zero;

	[SerializeField]
	private bool m_ApplyOffsets;

	[SerializeField]
	private AvatarMask m_AvatarMask;

	[SerializeField]
	private bool m_ApplyAvatarMask = true;

	[SerializeField]
	[Obsolete("Use m_OpenClipOffsetEuler Instead", false)]
	[HideInInspector]
	private Quaternion m_OpenClipOffsetRotation = Quaternion.identity;

	[SerializeField]
	[Obsolete("Use m_RotationEuler Instead", false)]
	[HideInInspector]
	private Quaternion m_Rotation = Quaternion.identity;

	public Vector3 position
	{
		get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
		}
	}

	public Quaternion rotation
	{
		get
		{
			return Quaternion.Euler(m_EulerAngles);
		}
		set
		{
			m_EulerAngles = value.eulerAngles;
		}
	}

	public Vector3 eulerAngles
	{
		get
		{
			return m_EulerAngles;
		}
		set
		{
			m_EulerAngles = value;
		}
	}

	public bool applyOffsets
	{
		get
		{
			return m_ApplyOffsets;
		}
		set
		{
			m_ApplyOffsets = value;
		}
	}

	public MatchTargetFields matchTargetFields
	{
		get
		{
			return m_MatchTargetFields;
		}
		set
		{
			m_MatchTargetFields = value & MatchTargetFieldConstants.All;
		}
	}

	private bool compilableIsolated => !base.muted && (m_Clips.Count > 0 || (base.animClip != null && !base.animClip.empty));

	public AvatarMask avatarMask
	{
		get
		{
			return m_AvatarMask;
		}
		set
		{
			m_AvatarMask = value;
		}
	}

	public bool applyAvatarMask
	{
		get
		{
			return m_ApplyAvatarMask;
		}
		set
		{
			m_ApplyAvatarMask = value;
		}
	}

	internal override bool compilable
	{
		get
		{
			if (compilableIsolated)
			{
				return true;
			}
			foreach (TrackAsset childTrack in GetChildTracks())
			{
				if (childTrack.compilable)
				{
					return true;
				}
			}
			return false;
		}
	}

	public override IEnumerable<PlayableBinding> outputs
	{
		get
		{
			yield return new PlayableBinding
			{
				sourceObject = this,
				streamName = base.name,
				streamType = DataStreamType.Animation
			};
		}
	}

	public bool inClipMode => base.clips != null && base.clips.Length != 0;

	public Vector3 openClipOffsetPosition
	{
		get
		{
			return m_OpenClipOffsetPosition;
		}
		set
		{
			m_OpenClipOffsetPosition = value;
		}
	}

	public Quaternion openClipOffsetRotation
	{
		get
		{
			return Quaternion.Euler(m_OpenClipOffsetEulerAngles);
		}
		set
		{
			m_OpenClipOffsetEulerAngles = value.eulerAngles;
		}
	}

	public Vector3 openClipOffsetEulerAngles
	{
		get
		{
			return m_OpenClipOffsetEulerAngles;
		}
		set
		{
			m_OpenClipOffsetEulerAngles = value;
		}
	}

	internal double openClipTimeOffset
	{
		get
		{
			return m_OpenClipTimeOffset;
		}
		set
		{
			m_OpenClipTimeOffset = value;
		}
	}

	public TimelineClip.ClipExtrapolation openClipPreExtrapolation
	{
		get
		{
			return m_OpenClipPreExtrapolation;
		}
		set
		{
			m_OpenClipPreExtrapolation = value;
		}
	}

	public TimelineClip.ClipExtrapolation openClipPostExtrapolation
	{
		get
		{
			return m_OpenClipPostExtrapolation;
		}
		set
		{
			m_OpenClipPostExtrapolation = value;
		}
	}

	[ContextMenu("Reset Offsets")]
	private void ResetOffsets()
	{
		m_Position = Vector3.zero;
		m_EulerAngles = Vector3.zero;
		UpdateClipOffsets();
	}

	public TimelineClip CreateClip(AnimationClip clip)
	{
		if (clip == null)
		{
			return null;
		}
		TimelineClip timelineClip = CreateClip<AnimationPlayableAsset>();
		AssignAnimationClip(timelineClip, clip);
		return timelineClip;
	}

	internal void UpdateClipOffsets()
	{
	}

	internal override void OnCreateClipFromAsset(Object asset, TimelineClip clip)
	{
		AnimationClip animationClip = asset as AnimationClip;
		if (animationClip != null)
		{
			if (animationClip.legacy)
			{
				throw new InvalidOperationException("Legacy Animation Clips are not supported");
			}
			AnimationPlayableAsset animationPlayableAsset = ScriptableObject.CreateInstance<AnimationPlayableAsset>();
			TimelineCreateUtilities.SaveAssetIntoObject(animationPlayableAsset, this);
			animationPlayableAsset.clip = animationClip;
			clip.asset = animationPlayableAsset;
			AssignAnimationClip(clip, animationClip);
		}
	}

	internal Playable CompileTrackPlayable(PlayableGraph graph, TrackAsset track, GameObject go, IntervalTree<RuntimeElement> tree)
	{
		AnimationMixerPlayable animationMixerPlayable = AnimationMixerPlayable.Create(graph, track.clips.Length);
		for (int i = 0; i < track.clips.Length; i++)
		{
			TimelineClip timelineClip = track.clips[i];
			PlayableAsset playableAsset = timelineClip.asset as PlayableAsset;
			if (playableAsset == null)
			{
				continue;
			}
			if (timelineClip.recordable)
			{
				AnimationPlayableAsset animationPlayableAsset = playableAsset as AnimationPlayableAsset;
				if (animationPlayableAsset != null)
				{
					animationPlayableAsset.removeStartOffset = !timelineClip.recordable;
				}
			}
			Playable playable = playableAsset.CreatePlayable(graph, go);
			if (playable.IsValid())
			{
				RuntimeClip item = new RuntimeClip(timelineClip, playable, animationMixerPlayable);
				tree.Add(item);
				graph.Connect(playable, 0, animationMixerPlayable, i);
				animationMixerPlayable.SetInputWeight(i, 0f);
			}
		}
		return ApplyTrackOffset(graph, animationMixerPlayable);
	}

	internal override Playable OnCreatePlayableGraph(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
	{
		if (base.isSubTrack)
		{
			throw new InvalidOperationException("Nested animation tracks should never be asked to create a graph directly");
		}
		List<AnimationTrack> list = new List<AnimationTrack>();
		if (compilableIsolated)
		{
			list.Add(this);
		}
		foreach (TrackAsset childTrack in GetChildTracks())
		{
			AnimationTrack animationTrack = childTrack as AnimationTrack;
			if (animationTrack != null && animationTrack.compilable)
			{
				list.Add(animationTrack);
			}
		}
		AnimationMotionXToDeltaPlayable animationMotionXToDeltaPlayable = AnimationMotionXToDeltaPlayable.Create(graph);
		AnimationLayerMixerPlayable animationLayerMixerPlayable = CreateGroupMixer(graph, go, list.Count);
		graph.Connect(animationLayerMixerPlayable, 0, animationMotionXToDeltaPlayable, 0);
		animationMotionXToDeltaPlayable.SetInputWeight(0, 1f);
		for (int i = 0; i < list.Count; i++)
		{
			Playable source = ((!list[i].inClipMode) ? list[i].CreateInfiniteTrackPlayable(graph, go, tree) : CompileTrackPlayable(graph, list[i], go, tree));
			graph.Connect(source, 0, animationLayerMixerPlayable, i);
			animationLayerMixerPlayable.SetInputWeight(i, (!list[i].inClipMode) ? 1 : 0);
			if (list[i].applyAvatarMask && list[i].avatarMask != null)
			{
				animationLayerMixerPlayable.SetLayerMaskFromAvatarMask((uint)i, list[i].avatarMask);
			}
		}
		return animationMotionXToDeltaPlayable;
	}

	private static AnimationLayerMixerPlayable CreateGroupMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		return AnimationLayerMixerPlayable.Create(graph, inputCount);
	}

	private Playable CreateInfiniteTrackPlayable(PlayableGraph graph, GameObject go, IntervalTree<RuntimeElement> tree)
	{
		if (base.animClip == null)
		{
			return Playable.Null;
		}
		AnimationMixerPlayable animationMixerPlayable = AnimationMixerPlayable.Create(graph, 1);
		Playable playable = AnimationPlayableAsset.CreatePlayable(graph, base.animClip, m_OpenClipOffsetPosition, m_OpenClipOffsetEulerAngles, removeStartOffset: false);
		if (playable.IsValid())
		{
			tree.Add(new InfiniteRuntimeClip(playable));
			graph.Connect(playable, 0, animationMixerPlayable, 0);
			animationMixerPlayable.SetInputWeight(0, 1f);
		}
		return ApplyTrackOffset(graph, animationMixerPlayable);
	}

	private Playable ApplyTrackOffset(PlayableGraph graph, Playable root)
	{
		if (!m_ApplyOffsets)
		{
			return root;
		}
		AnimationOffsetPlayable animationOffsetPlayable = AnimationOffsetPlayable.Create(graph, position, rotation, 1);
		graph.Connect(root, 0, animationOffsetPlayable, 0);
		animationOffsetPlayable.SetInputWeight(0, 1f);
		return animationOffsetPlayable;
	}

	internal override void GetEvaluationTime(out double outStart, out double outDuration)
	{
		if (inClipMode)
		{
			base.GetEvaluationTime(out outStart, out outDuration);
			return;
		}
		outStart = 0.0;
		outDuration = TimelineClip.kMaxTimeValue;
	}

	internal override void GetSequenceTime(out double outStart, out double outDuration)
	{
		if (inClipMode)
		{
			base.GetSequenceTime(out outStart, out outDuration);
			return;
		}
		outStart = 0.0;
		outDuration = 0.0;
		if (base.animClip != null)
		{
			outDuration = base.animClip.length;
		}
	}

	private void AssignAnimationClip(TimelineClip clip, AnimationClip animClip)
	{
		if (clip != null && !(animClip == null))
		{
			if (animClip.legacy)
			{
				throw new InvalidOperationException("Legacy Animation Clips are not supported");
			}
			if (animClip.frameRate > 0f)
			{
				double num = Mathf.Round(animClip.length * animClip.frameRate);
				clip.duration = num / (double)animClip.frameRate;
			}
			else
			{
				clip.duration = animClip.length;
			}
			TimelineClip.ClipExtrapolation clipExtrapolation = TimelineClip.ClipExtrapolation.None;
			if (!base.isSubTrack)
			{
				clipExtrapolation = TimelineClip.ClipExtrapolation.Hold;
			}
			AnimationPlayableAsset animationPlayableAsset = clip.asset as AnimationPlayableAsset;
			if (animationPlayableAsset != null)
			{
				animationPlayableAsset.clip = animClip;
			}
			clip.preExtrapolationMode = clipExtrapolation;
			clip.postExtrapolationMode = clipExtrapolation;
		}
	}

	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		base.GatherProperties(director, driver);
	}

	protected internal override void OnUpgradeFromVersion(int oldVersion)
	{
		if (oldVersion < 1)
		{
			AnimationTrackUpgrade.ConvertRotationsToEuler(this);
		}
	}
}

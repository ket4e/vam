using System;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

[Serializable]
[NotKeyable]
public class AnimationPlayableAsset : PlayableAsset, ITimelineClipAsset, IPropertyPreview, ISerializationCallbackReceiver
{
	private enum Versions
	{
		Initial,
		RotationAsEuler
	}

	private static class AnimationPlayableAssetUpgrade
	{
		public static void ConvertRotationToEuler(AnimationPlayableAsset asset)
		{
			asset.m_EulerAngles = asset.m_Rotation.eulerAngles;
		}
	}

	[SerializeField]
	private AnimationClip m_Clip;

	[SerializeField]
	private Vector3 m_Position = Vector3.zero;

	[SerializeField]
	private Vector3 m_EulerAngles = Vector3.zero;

	[SerializeField]
	private bool m_UseTrackMatchFields = false;

	[SerializeField]
	private MatchTargetFields m_MatchTargetFields = MatchTargetFieldConstants.All;

	[SerializeField]
	private bool m_RemoveStartOffset = true;

	private static readonly int k_LatestVersion = 1;

	[SerializeField]
	[HideInInspector]
	private int m_Version;

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

	public bool useTrackMatchFields
	{
		get
		{
			return m_UseTrackMatchFields;
		}
		set
		{
			m_UseTrackMatchFields = value;
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
			m_MatchTargetFields = value;
		}
	}

	internal bool removeStartOffset
	{
		get
		{
			return m_RemoveStartOffset;
		}
		set
		{
			m_RemoveStartOffset = value;
		}
	}

	public AnimationClip clip
	{
		get
		{
			return m_Clip;
		}
		set
		{
			if (value != null)
			{
				base.name = "AnimationPlayableAsset of " + value.name;
			}
			m_Clip = value;
		}
	}

	public override double duration
	{
		get
		{
			if (clip == null || clip.empty)
			{
				return base.duration;
			}
			double num = clip.length;
			if (num < 1.401298464324817E-45)
			{
				return base.duration;
			}
			if (clip.frameRate > 0f)
			{
				double num2 = Mathf.Round(clip.length * clip.frameRate);
				num = num2 / (double)clip.frameRate;
			}
			return num;
		}
	}

	public override IEnumerable<PlayableBinding> outputs
	{
		get
		{
			yield return new PlayableBinding
			{
				streamType = DataStreamType.Animation,
				streamName = "Animation"
			};
		}
	}

	public ClipCaps clipCaps
	{
		get
		{
			ClipCaps clipCaps = ClipCaps.All;
			if (m_Clip == null || !m_Clip.isLooping)
			{
				clipCaps &= ~ClipCaps.Looping;
			}
			if (m_Clip == null || m_Clip.empty)
			{
				clipCaps &= ~ClipCaps.ClipIn;
			}
			return clipCaps;
		}
	}

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		return CreatePlayable(graph, m_Clip, position, eulerAngles, removeStartOffset);
	}

	internal static Playable CreatePlayable(PlayableGraph graph, AnimationClip clip, Vector3 positionOffset, Vector3 eulerOffset, bool removeStartOffset)
	{
		if (clip == null || clip.legacy)
		{
			return Playable.Null;
		}
		AnimationClipPlayable animationClipPlayable = AnimationClipPlayable.Create(graph, clip);
		animationClipPlayable.SetRemoveStartOffset(removeStartOffset);
		Playable result = animationClipPlayable;
		if (ShouldApplyRootMotion(positionOffset, eulerOffset, clip))
		{
			AnimationOffsetPlayable animationOffsetPlayable = AnimationOffsetPlayable.Create(graph, positionOffset, Quaternion.Euler(eulerOffset), 1);
			graph.Connect(animationClipPlayable, 0, animationOffsetPlayable, 0);
			animationOffsetPlayable.SetInputWeight(0, 1f);
			result = animationOffsetPlayable;
		}
		return result;
	}

	private static bool ShouldApplyRootMotion(Vector3 position, Vector3 rotation, AnimationClip clip)
	{
		return position != Vector3.zero || rotation != Vector3.zero || (clip != null && clip.hasRootMotion);
	}

	public void ResetOffsets()
	{
		position = Vector3.zero;
		eulerAngles = Vector3.zero;
	}

	public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		driver.AddFromClip(m_Clip);
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		m_Version = k_LatestVersion;
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		if (m_Version < k_LatestVersion)
		{
			OnUpgradeFromVersion(m_Version);
		}
	}

	private void OnUpgradeFromVersion(int oldVersion)
	{
		if (oldVersion < 1)
		{
			AnimationPlayableAssetUpgrade.ConvertRotationToEuler(this);
		}
	}
}

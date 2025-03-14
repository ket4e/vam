using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

[Serializable]
[TrackClipType(typeof(ActivationPlayableAsset))]
[TrackMediaType(TimelineAsset.MediaType.Script)]
[TrackBindingType(typeof(GameObject))]
public class ActivationTrack : TrackAsset
{
	public enum PostPlaybackState
	{
		Active,
		Inactive,
		Revert,
		LeaveAsIs
	}

	[SerializeField]
	private PostPlaybackState m_PostPlaybackState = PostPlaybackState.LeaveAsIs;

	private ActivationMixerPlayable m_ActivationMixer;

	internal override bool compilable => isEmpty || base.compilable;

	public PostPlaybackState postPlaybackState
	{
		get
		{
			return m_PostPlaybackState;
		}
		set
		{
			m_PostPlaybackState = value;
			UpdateTrackMode();
		}
	}

	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		ScriptPlayable<ActivationMixerPlayable> scriptPlayable = ActivationMixerPlayable.Create(graph, inputCount);
		m_ActivationMixer = scriptPlayable.GetBehaviour();
		PlayableDirector component = go.GetComponent<PlayableDirector>();
		UpdateBoundGameObject(component);
		UpdateTrackMode();
		return scriptPlayable;
	}

	private void UpdateBoundGameObject(PlayableDirector director)
	{
		if (director != null)
		{
			GameObject gameObject = director.GetGenericBinding(this) as GameObject;
			if (gameObject != null && m_ActivationMixer != null)
			{
				m_ActivationMixer.boundGameObject = gameObject;
			}
		}
	}

	internal void UpdateTrackMode()
	{
		if (m_ActivationMixer != null)
		{
			m_ActivationMixer.postPlaybackState = m_PostPlaybackState;
		}
	}

	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		GameObject gameObjectBinding = GetGameObjectBinding(director);
		if (gameObjectBinding != null)
		{
			driver.AddFromName(gameObjectBinding, "m_IsActive");
		}
	}
}

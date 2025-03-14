using UnityEngine.Playables;

namespace UnityEngine.Timeline;

internal class ActivationMixerPlayable : PlayableBehaviour
{
	private ActivationTrack.PostPlaybackState m_PostPlaybackState;

	private bool m_BoundGameObjectInitialStateIsActive;

	private GameObject m_BoundGameObject;

	public GameObject boundGameObject
	{
		get
		{
			return m_BoundGameObject;
		}
		set
		{
			m_BoundGameObject = value;
			m_BoundGameObjectInitialStateIsActive = value != null && value.activeSelf;
		}
	}

	public ActivationTrack.PostPlaybackState postPlaybackState
	{
		get
		{
			return m_PostPlaybackState;
		}
		set
		{
			m_PostPlaybackState = value;
		}
	}

	public static ScriptPlayable<ActivationMixerPlayable> Create(PlayableGraph graph, int inputCount)
	{
		return ScriptPlayable<ActivationMixerPlayable>.Create(graph, inputCount);
	}

	public override void OnPlayableDestroy(Playable playable)
	{
		if (boundGameObject == null)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			boundGameObject.SetActive(m_BoundGameObjectInitialStateIsActive);
			return;
		}
		switch (m_PostPlaybackState)
		{
		case ActivationTrack.PostPlaybackState.Active:
			boundGameObject.SetActive(value: true);
			break;
		case ActivationTrack.PostPlaybackState.Inactive:
			boundGameObject.SetActive(value: false);
			break;
		case ActivationTrack.PostPlaybackState.Revert:
			boundGameObject.SetActive(m_BoundGameObjectInitialStateIsActive);
			break;
		case ActivationTrack.PostPlaybackState.LeaveAsIs:
			break;
		}
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (boundGameObject == null)
		{
			return;
		}
		int inputCount = playable.GetInputCount();
		bool active = false;
		for (int i = 0; i < inputCount; i++)
		{
			if (playable.GetInputWeight(i) > 0f)
			{
				active = true;
				break;
			}
		}
		boundGameObject.SetActive(active);
	}
}

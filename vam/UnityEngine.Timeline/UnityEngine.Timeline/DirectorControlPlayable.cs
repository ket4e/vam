using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline;

public class DirectorControlPlayable : PlayableBehaviour
{
	public PlayableDirector director;

	private bool m_SyncTime = false;

	public static ScriptPlayable<DirectorControlPlayable> Create(PlayableGraph graph, PlayableDirector director)
	{
		if (director == null)
		{
			return ScriptPlayable<DirectorControlPlayable>.Null;
		}
		ScriptPlayable<DirectorControlPlayable> result = ScriptPlayable<DirectorControlPlayable>.Create(graph);
		result.GetBehaviour().director = director;
		return result;
	}

	public override void PrepareFrame(Playable playable, FrameData info)
	{
		if (!(director == null) && director.isActiveAndEnabled && !(director.playableAsset == null))
		{
			m_SyncTime |= info.evaluationType == FrameData.EvaluationType.Evaluate || DetectDiscontinuity(playable, info);
			SyncSpeed(info.effectiveSpeed);
			SyncPlayState(playable.GetGraph());
		}
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		m_SyncTime = true;
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		if (director != null && director.playableAsset != null)
		{
			director.Stop();
		}
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (!(director == null) && director.isActiveAndEnabled && !(director.playableAsset == null))
		{
			if (m_SyncTime || DetectOutOfSync(playable))
			{
				UpdateTime(playable);
				director.Evaluate();
			}
			m_SyncTime = false;
		}
	}

	private void SyncSpeed(double speed)
	{
		if (!director.playableGraph.IsValid())
		{
			return;
		}
		int rootPlayableCount = director.playableGraph.GetRootPlayableCount();
		for (int i = 0; i < rootPlayableCount; i++)
		{
			Playable rootPlayable = director.playableGraph.GetRootPlayable(i);
			if (rootPlayable.IsValid())
			{
				rootPlayable.SetSpeed(speed);
			}
		}
	}

	private void SyncPlayState(PlayableGraph graph)
	{
		if (graph.IsPlaying())
		{
			director.Play();
		}
		else
		{
			director.Pause();
		}
	}

	private bool DetectDiscontinuity(Playable playable, FrameData info)
	{
		return Math.Abs(playable.GetTime() - playable.GetPreviousTime() - info.m_DeltaTime) > DiscreteTime.tickValue;
	}

	private bool DetectOutOfSync(Playable playable)
	{
		if (!Mathf.Approximately((float)playable.GetTime(), (float)director.time))
		{
			return true;
		}
		return false;
	}

	private void UpdateTime(Playable playable)
	{
		double num = Math.Max(0.1, director.playableAsset.duration);
		switch (director.extrapolationMode)
		{
		case DirectorWrapMode.Hold:
			director.time = Math.Min(num, Math.Max(0.0, playable.GetTime()));
			break;
		case DirectorWrapMode.Loop:
			director.time = Math.Max(0.0, playable.GetTime() % num);
			break;
		case DirectorWrapMode.None:
			director.time = playable.GetTime();
			break;
		}
	}
}

using UnityEngine.Playables;

namespace UnityEngine.Timeline;

internal abstract class EventPlayable : PlayableBehaviour
{
	private bool m_HasFired;

	private double m_PreviousTime;

	public double triggerTime { get; set; }

	public virtual void OnTrigger()
	{
	}

	public sealed override void PrepareFrame(Playable playable, FrameData info)
	{
		if (m_HasFired && (HasLooped(playable.GetTime(), info) || CanRestoreEvent(playable.GetTime(), info)))
		{
			Restore_internal();
		}
		if (!m_HasFired && CanTriggerEvent(playable.GetTime(), info))
		{
			Trigger_internal();
		}
		m_PreviousTime = playable.GetTime();
	}

	public sealed override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
	}

	public sealed override void OnBehaviourPlay(Playable playable, FrameData info)
	{
	}

	public sealed override void OnBehaviourPause(Playable playable, FrameData info)
	{
	}

	public override void OnPlayableDestroy(Playable playable)
	{
		if (m_HasFired)
		{
			Restore_internal();
		}
	}

	private bool CanTriggerEvent(double playableTime, FrameData info)
	{
		if (DiscreteTime.GetNearestTick(playableTime) >= DiscreteTime.GetNearestTick(triggerTime))
		{
			return true;
		}
		return HasLooped(playableTime, info) && triggerTime >= m_PreviousTime && triggerTime <= m_PreviousTime + (double)info.deltaTime;
	}

	private bool CanRestoreEvent(double playableTime, FrameData info)
	{
		return DiscreteTime.GetNearestTick(playableTime) < DiscreteTime.GetNearestTick(triggerTime);
	}

	private void Trigger_internal()
	{
		OnTrigger();
		m_HasFired = true;
	}

	private void Restore_internal()
	{
		m_HasFired = false;
	}

	private bool HasLooped(double playableTime, FrameData info)
	{
		return m_PreviousTime > playableTime && (double)info.deltaTime > playableTime;
	}
}

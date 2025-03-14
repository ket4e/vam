using System;

namespace UnityEngine.Experimental.UIElements;

internal abstract class ScheduledItem : IScheduledItem
{
	public Func<bool> timerUpdateStopCondition;

	public static readonly Func<bool> OnceCondition = () => true;

	public static readonly Func<bool> ForeverCondition = () => false;

	public long startMs { get; set; }

	public long delayMs { get; set; }

	public long intervalMs { get; set; }

	public long endTimeMs { get; private set; }

	public ScheduledItem()
	{
		ResetStartTime();
		timerUpdateStopCondition = OnceCondition;
	}

	protected void ResetStartTime()
	{
		startMs = Panel.TimeSinceStartupMs();
	}

	public void SetDuration(long durationMs)
	{
		endTimeMs = startMs + durationMs;
	}

	public abstract void PerformTimerUpdate(TimerState state);

	internal virtual void OnItemUnscheduled()
	{
	}

	public virtual bool ShouldUnschedule()
	{
		if (endTimeMs > 0 && Panel.TimeSinceStartupMs() > endTimeMs)
		{
			return true;
		}
		if (timerUpdateStopCondition != null)
		{
			return timerUpdateStopCondition();
		}
		return false;
	}
}

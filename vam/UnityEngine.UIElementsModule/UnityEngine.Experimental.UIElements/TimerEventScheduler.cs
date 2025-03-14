using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

internal class TimerEventScheduler : IScheduler
{
	private class TimerEventSchedulerItem : ScheduledItem
	{
		private readonly Action<TimerState> m_TimerUpdateEvent;

		public TimerEventSchedulerItem(Action<TimerState> updateEvent)
		{
			m_TimerUpdateEvent = updateEvent;
		}

		public override void PerformTimerUpdate(TimerState state)
		{
			if (m_TimerUpdateEvent != null)
			{
				m_TimerUpdateEvent(state);
			}
		}

		public override string ToString()
		{
			return m_TimerUpdateEvent.ToString();
		}
	}

	private readonly List<ScheduledItem> m_ScheduledItems = new List<ScheduledItem>();

	private bool m_TransactionMode;

	private readonly List<ScheduledItem> m_ScheduleTransactions = new List<ScheduledItem>();

	private readonly List<ScheduledItem> m_UnscheduleTransactions = new List<ScheduledItem>();

	internal bool disableThrottling = false;

	private int m_LastUpdatedIndex = -1;

	public void Schedule(IScheduledItem item)
	{
		if (item == null)
		{
			return;
		}
		if (!(item is ScheduledItem scheduledItem))
		{
			throw new NotSupportedException("Scheduled Item type is not supported by this scheduler");
		}
		if (m_TransactionMode)
		{
			m_ScheduleTransactions.Add(scheduledItem);
			return;
		}
		if (m_ScheduledItems.Contains(scheduledItem))
		{
			throw new ArgumentException(string.Concat("Cannot schedule function ", scheduledItem, " more than once"));
		}
		m_ScheduledItems.Add(scheduledItem);
	}

	public IScheduledItem ScheduleOnce(Action<TimerState> timerUpdateEvent, long delayMs)
	{
		TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventSchedulerItem(timerUpdateEvent);
		timerEventSchedulerItem.delayMs = delayMs;
		TimerEventSchedulerItem timerEventSchedulerItem2 = timerEventSchedulerItem;
		Schedule(timerEventSchedulerItem2);
		return timerEventSchedulerItem2;
	}

	public IScheduledItem ScheduleUntil(Action<TimerState> timerUpdateEvent, long delayMs, long intervalMs, Func<bool> stopCondition)
	{
		TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventSchedulerItem(timerUpdateEvent);
		timerEventSchedulerItem.delayMs = delayMs;
		timerEventSchedulerItem.intervalMs = intervalMs;
		timerEventSchedulerItem.timerUpdateStopCondition = stopCondition;
		TimerEventSchedulerItem timerEventSchedulerItem2 = timerEventSchedulerItem;
		Schedule(timerEventSchedulerItem2);
		return timerEventSchedulerItem2;
	}

	public IScheduledItem ScheduleForDuration(Action<TimerState> timerUpdateEvent, long delayMs, long intervalMs, long durationMs)
	{
		TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventSchedulerItem(timerUpdateEvent);
		timerEventSchedulerItem.delayMs = delayMs;
		timerEventSchedulerItem.intervalMs = intervalMs;
		timerEventSchedulerItem.timerUpdateStopCondition = null;
		TimerEventSchedulerItem timerEventSchedulerItem2 = timerEventSchedulerItem;
		timerEventSchedulerItem2.SetDuration(durationMs);
		Schedule(timerEventSchedulerItem2);
		return timerEventSchedulerItem2;
	}

	private bool RemovedScheduledItemAt(int index)
	{
		if (index >= 0)
		{
			ScheduledItem scheduledItem = m_ScheduledItems[index];
			m_ScheduledItems.RemoveAt(index);
			scheduledItem.OnItemUnscheduled();
			return true;
		}
		return false;
	}

	public void Unschedule(IScheduledItem item)
	{
		if (item is ScheduledItem scheduledItem)
		{
			if (m_TransactionMode)
			{
				m_UnscheduleTransactions.Add(scheduledItem);
			}
			else if (!RemovedScheduledItemAt(m_ScheduledItems.IndexOf(scheduledItem)))
			{
				throw new ArgumentException("Cannot unschedule unknown scheduled function " + scheduledItem);
			}
		}
	}

	public void UpdateScheduledEvents()
	{
		try
		{
			m_TransactionMode = true;
			long num = Panel.TimeSinceStartupMs();
			int count = m_ScheduledItems.Count;
			int num2 = m_LastUpdatedIndex + 1;
			if (num2 >= count)
			{
				num2 = 0;
			}
			for (int i = 0; i < count; i++)
			{
				int num3 = num2 + i;
				if (num3 >= count)
				{
					num3 -= count;
				}
				ScheduledItem scheduledItem = m_ScheduledItems[num3];
				if (num - scheduledItem.delayMs >= scheduledItem.startMs)
				{
					TimerState timerState = default(TimerState);
					timerState.start = scheduledItem.startMs;
					timerState.now = num;
					TimerState state = timerState;
					scheduledItem.PerformTimerUpdate(state);
					scheduledItem.startMs = num;
					scheduledItem.delayMs = scheduledItem.intervalMs;
					if (scheduledItem.ShouldUnschedule())
					{
						Unschedule(scheduledItem);
					}
				}
				m_LastUpdatedIndex = num3;
			}
		}
		finally
		{
			m_TransactionMode = false;
			for (int j = 0; j < m_UnscheduleTransactions.Count; j++)
			{
				Unschedule(m_UnscheduleTransactions[j]);
			}
			m_UnscheduleTransactions.Clear();
			for (int k = 0; k < m_ScheduleTransactions.Count; k++)
			{
				Schedule(m_ScheduleTransactions[k]);
			}
			m_ScheduleTransactions.Clear();
		}
	}
}

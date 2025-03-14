using System;

namespace UnityEngine.Timeline;

[Serializable]
internal class TimelineMarker : ITimelineItem
{
	[SerializeField]
	private string m_Key;

	[SerializeField]
	[HideInInspector]
	private TrackAsset m_ParentTrack;

	[SerializeField]
	[TimeField]
	private double m_Time;

	[SerializeField]
	[HideInInspector]
	private bool m_Selected;

	double ITimelineItem.start => m_Time;

	public string key
	{
		get
		{
			return m_Key;
		}
		internal set
		{
			if (m_Key != value && parentTrack != null && parentTrack.timelineAsset != null)
			{
				parentTrack.timelineAsset.Invalidate();
			}
			m_Key = value;
		}
	}

	public double time
	{
		get
		{
			return m_Time;
		}
		set
		{
			m_Time = value;
		}
	}

	public bool selected
	{
		get
		{
			return m_Selected;
		}
		set
		{
			m_Selected = value;
		}
	}

	public TrackAsset parentTrack => m_ParentTrack;

	public TimelineMarker(string key, double time, TrackAsset parentTrack)
	{
		if (key == null)
		{
			key = string.Empty;
		}
		m_Key = key;
		m_Time = time;
		m_ParentTrack = parentTrack;
	}

	int ITimelineItem.Hash()
	{
		return m_Time.GetHashCode() ^ m_Key.GetHashCode();
	}
}

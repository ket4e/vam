using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI;

[Serializable]
public struct Navigation : IEquatable<Navigation>
{
	[Flags]
	public enum Mode
	{
		None = 0,
		Horizontal = 1,
		Vertical = 2,
		Automatic = 3,
		Explicit = 4
	}

	[FormerlySerializedAs("mode")]
	[SerializeField]
	private Mode m_Mode;

	[FormerlySerializedAs("selectOnUp")]
	[SerializeField]
	private Selectable m_SelectOnUp;

	[FormerlySerializedAs("selectOnDown")]
	[SerializeField]
	private Selectable m_SelectOnDown;

	[FormerlySerializedAs("selectOnLeft")]
	[SerializeField]
	private Selectable m_SelectOnLeft;

	[FormerlySerializedAs("selectOnRight")]
	[SerializeField]
	private Selectable m_SelectOnRight;

	public Mode mode
	{
		get
		{
			return m_Mode;
		}
		set
		{
			m_Mode = value;
		}
	}

	public Selectable selectOnUp
	{
		get
		{
			return m_SelectOnUp;
		}
		set
		{
			m_SelectOnUp = value;
		}
	}

	public Selectable selectOnDown
	{
		get
		{
			return m_SelectOnDown;
		}
		set
		{
			m_SelectOnDown = value;
		}
	}

	public Selectable selectOnLeft
	{
		get
		{
			return m_SelectOnLeft;
		}
		set
		{
			m_SelectOnLeft = value;
		}
	}

	public Selectable selectOnRight
	{
		get
		{
			return m_SelectOnRight;
		}
		set
		{
			m_SelectOnRight = value;
		}
	}

	public static Navigation defaultNavigation
	{
		get
		{
			Navigation result = default(Navigation);
			result.m_Mode = Mode.Automatic;
			return result;
		}
	}

	public bool Equals(Navigation other)
	{
		return mode == other.mode && selectOnUp == other.selectOnUp && selectOnDown == other.selectOnDown && selectOnLeft == other.selectOnLeft && selectOnRight == other.selectOnRight;
	}
}

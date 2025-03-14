using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Represents an axis aligned bounding box with all values as integers.</para>
/// </summary>
[UsedByNativeCode]
public struct BoundsInt
{
	/// <summary>
	///   <para>An iterator that allows you to iterate over all positions within the BoundsInt.</para>
	/// </summary>
	public struct PositionEnumerator : IEnumerator<Vector3Int>, IEnumerator, IDisposable
	{
		private readonly Vector3Int _min;

		private readonly Vector3Int _max;

		private Vector3Int _current;

		object IEnumerator.Current => Current;

		/// <summary>
		///   <para>Current position of the enumerator.</para>
		/// </summary>
		public Vector3Int Current => _current;

		public PositionEnumerator(Vector3Int min, Vector3Int max)
		{
			_min = (_current = min);
			_max = max;
			Reset();
		}

		/// <summary>
		///   <para>Returns this as an iterator that allows you to iterate over all positions within the BoundsInt.</para>
		/// </summary>
		/// <returns>
		///   <para>This BoundsInt.PositionEnumerator.</para>
		/// </returns>
		public PositionEnumerator GetEnumerator()
		{
			return this;
		}

		/// <summary>
		///   <para>Moves the enumerator to the next position.</para>
		/// </summary>
		/// <returns>
		///   <para>Whether the enumerator has successfully moved to the next position.</para>
		/// </returns>
		public bool MoveNext()
		{
			if (_current.z >= _max.z)
			{
				return false;
			}
			_current.x++;
			if (_current.x >= _max.x)
			{
				_current.x = _min.x;
				_current.y++;
				if (_current.y >= _max.y)
				{
					_current.y = _min.y;
					_current.z++;
					if (_current.z >= _max.z)
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		///   <para>Resets this enumerator to its starting state.</para>
		/// </summary>
		public void Reset()
		{
			_current = _min;
			_current.x--;
		}

		void IDisposable.Dispose()
		{
		}
	}

	private Vector3Int m_Position;

	private Vector3Int m_Size;

	/// <summary>
	///   <para>X value of the minimal point of the box.</para>
	/// </summary>
	public int x
	{
		get
		{
			return m_Position.x;
		}
		set
		{
			m_Position.x = value;
		}
	}

	/// <summary>
	///   <para>Y value of the minimal point of the box.</para>
	/// </summary>
	public int y
	{
		get
		{
			return m_Position.y;
		}
		set
		{
			m_Position.y = value;
		}
	}

	/// <summary>
	///   <para>Z value of the minimal point of the box.</para>
	/// </summary>
	public int z
	{
		get
		{
			return m_Position.z;
		}
		set
		{
			m_Position.z = value;
		}
	}

	/// <summary>
	///   <para>The center of the bounding box.</para>
	/// </summary>
	public Vector3 center => new Vector3((float)x + (float)m_Size.x / 2f, (float)y + (float)m_Size.y / 2f, (float)z + (float)m_Size.z / 2f);

	/// <summary>
	///   <para>The minimal point of the box.</para>
	/// </summary>
	public Vector3Int min
	{
		get
		{
			return new Vector3Int(xMin, yMin, zMin);
		}
		set
		{
			xMin = value.x;
			yMin = value.y;
			zMin = value.z;
		}
	}

	/// <summary>
	///   <para>The maximal point of the box.</para>
	/// </summary>
	public Vector3Int max
	{
		get
		{
			return new Vector3Int(xMax, yMax, zMax);
		}
		set
		{
			xMax = value.x;
			yMax = value.y;
			zMax = value.z;
		}
	}

	/// <summary>
	///   <para>The minimal x point of the box.</para>
	/// </summary>
	public int xMin
	{
		get
		{
			return Math.Min(m_Position.x, m_Position.x + m_Size.x);
		}
		set
		{
			int num = xMax;
			m_Position.x = value;
			m_Size.x = num - m_Position.x;
		}
	}

	/// <summary>
	///   <para>The minimal y point of the box.</para>
	/// </summary>
	public int yMin
	{
		get
		{
			return Math.Min(m_Position.y, m_Position.y + m_Size.y);
		}
		set
		{
			int num = yMax;
			m_Position.y = value;
			m_Size.y = num - m_Position.y;
		}
	}

	/// <summary>
	///   <para>The minimal z point of the box.</para>
	/// </summary>
	public int zMin
	{
		get
		{
			return Math.Min(m_Position.z, m_Position.z + m_Size.z);
		}
		set
		{
			int num = zMax;
			m_Position.z = value;
			m_Size.z = num - m_Position.z;
		}
	}

	/// <summary>
	///   <para>The maximal x point of the box.</para>
	/// </summary>
	public int xMax
	{
		get
		{
			return Math.Max(m_Position.x, m_Position.x + m_Size.x);
		}
		set
		{
			m_Size.x = value - m_Position.x;
		}
	}

	/// <summary>
	///   <para>The maximal y point of the box.</para>
	/// </summary>
	public int yMax
	{
		get
		{
			return Math.Max(m_Position.y, m_Position.y + m_Size.y);
		}
		set
		{
			m_Size.y = value - m_Position.y;
		}
	}

	/// <summary>
	///   <para>The maximal z point of the box.</para>
	/// </summary>
	public int zMax
	{
		get
		{
			return Math.Max(m_Position.z, m_Position.z + m_Size.z);
		}
		set
		{
			m_Size.z = value - m_Position.z;
		}
	}

	/// <summary>
	///   <para>The position of the bounding box.</para>
	/// </summary>
	public Vector3Int position
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

	/// <summary>
	///   <para>The total size of the box.</para>
	/// </summary>
	public Vector3Int size
	{
		get
		{
			return m_Size;
		}
		set
		{
			m_Size = value;
		}
	}

	/// <summary>
	///   <para>A BoundsInt.PositionCollection that contains all positions within the BoundsInt.</para>
	/// </summary>
	public PositionEnumerator allPositionsWithin => new PositionEnumerator(min, max);

	public BoundsInt(int xMin, int yMin, int zMin, int sizeX, int sizeY, int sizeZ)
	{
		m_Position = new Vector3Int(xMin, yMin, zMin);
		m_Size = new Vector3Int(sizeX, sizeY, sizeZ);
	}

	public BoundsInt(Vector3Int position, Vector3Int size)
	{
		m_Position = position;
		m_Size = size;
	}

	/// <summary>
	///   <para>Sets the bounds to the min and max value of the box.</para>
	/// </summary>
	/// <param name="minPosition"></param>
	/// <param name="maxPosition"></param>
	public void SetMinMax(Vector3Int minPosition, Vector3Int maxPosition)
	{
		min = minPosition;
		max = maxPosition;
	}

	/// <summary>
	///   <para>Clamps the position and size of this bounding box to the given bounds.</para>
	/// </summary>
	/// <param name="bounds">Bounds to clamp to.</param>
	public void ClampToBounds(BoundsInt bounds)
	{
		position = new Vector3Int(Math.Max(Math.Min(bounds.xMax, position.x), bounds.xMin), Math.Max(Math.Min(bounds.yMax, position.y), bounds.yMin), Math.Max(Math.Min(bounds.zMax, position.z), bounds.zMin));
		size = new Vector3Int(Math.Min(bounds.xMax - position.x, size.x), Math.Min(bounds.yMax - position.y, size.y), Math.Min(bounds.zMax - position.z, size.z));
	}

	/// <summary>
	///   <para>Is point contained in the bounding box?</para>
	/// </summary>
	/// <param name="position">Point to check.</param>
	/// <param name="inclusive">Whether the max limits are included in the check.</param>
	/// <returns>
	///   <para>Is point contained in the bounding box?</para>
	/// </returns>
	public bool Contains(Vector3Int position)
	{
		return position.x >= m_Position.x && position.y >= m_Position.y && position.z >= m_Position.z && position.x < m_Position.x + m_Size.x && position.y < m_Position.y + m_Size.y && position.z < m_Position.z + m_Size.z;
	}

	/// <summary>
	///   <para>Returns a nicely formatted string for the bounds.</para>
	/// </summary>
	public override string ToString()
	{
		return UnityString.Format("Position: {0}, Size: {1}", m_Position, m_Size);
	}

	public static bool operator ==(BoundsInt lhs, BoundsInt rhs)
	{
		return lhs.m_Position == rhs.m_Position && lhs.m_Size == rhs.m_Size;
	}

	public static bool operator !=(BoundsInt lhs, BoundsInt rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object other)
	{
		if (!(other is BoundsInt boundsInt))
		{
			return false;
		}
		return m_Position.Equals(boundsInt.m_Position) && m_Size.Equals(boundsInt.m_Size);
	}

	public override int GetHashCode()
	{
		return m_Position.GetHashCode() ^ (m_Size.GetHashCode() << 2);
	}
}

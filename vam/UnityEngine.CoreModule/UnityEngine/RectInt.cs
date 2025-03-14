using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A 2D Rectangle defined by x, y, width, height with integers.</para>
/// </summary>
[UsedByNativeCode]
public struct RectInt
{
	/// <summary>
	///   <para>An iterator that allows you to iterate over all positions within the RectInt.</para>
	/// </summary>
	public struct PositionEnumerator : IEnumerator<Vector2Int>, IEnumerator, IDisposable
	{
		private readonly Vector2Int _min;

		private readonly Vector2Int _max;

		private Vector2Int _current;

		object IEnumerator.Current => Current;

		/// <summary>
		///   <para>Current position of the enumerator.</para>
		/// </summary>
		public Vector2Int Current => _current;

		public PositionEnumerator(Vector2Int min, Vector2Int max)
		{
			_min = (_current = min);
			_max = max;
			Reset();
		}

		/// <summary>
		///   <para>Returns this as an iterator that allows you to iterate over all positions within the RectInt.</para>
		/// </summary>
		/// <returns>
		///   <para>This RectInt.PositionEnumerator.</para>
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
			if (_current.y >= _max.y)
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
					return false;
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

	private int m_XMin;

	private int m_YMin;

	private int m_Width;

	private int m_Height;

	/// <summary>
	///   <para>Left coordinate of the rectangle.</para>
	/// </summary>
	public int x
	{
		get
		{
			return m_XMin;
		}
		set
		{
			m_XMin = value;
		}
	}

	/// <summary>
	///   <para>Top coordinate of the rectangle.</para>
	/// </summary>
	public int y
	{
		get
		{
			return m_YMin;
		}
		set
		{
			m_YMin = value;
		}
	}

	/// <summary>
	///   <para>Center coordinate of the rectangle.</para>
	/// </summary>
	public Vector2 center => new Vector2((float)x + (float)m_Width / 2f, (float)y + (float)m_Height / 2f);

	/// <summary>
	///   <para>Lower left corner of the rectangle.</para>
	/// </summary>
	public Vector2Int min
	{
		get
		{
			return new Vector2Int(xMin, yMin);
		}
		set
		{
			xMin = value.x;
			yMin = value.y;
		}
	}

	/// <summary>
	///   <para>Upper right corner of the rectangle.</para>
	/// </summary>
	public Vector2Int max
	{
		get
		{
			return new Vector2Int(xMax, yMax);
		}
		set
		{
			xMax = value.x;
			yMax = value.y;
		}
	}

	/// <summary>
	///   <para>Width of the rectangle.</para>
	/// </summary>
	public int width
	{
		get
		{
			return m_Width;
		}
		set
		{
			m_Width = value;
		}
	}

	/// <summary>
	///   <para>Height of the rectangle.</para>
	/// </summary>
	public int height
	{
		get
		{
			return m_Height;
		}
		set
		{
			m_Height = value;
		}
	}

	/// <summary>
	///   <para>Returns the minimum X value of the RectInt.</para>
	/// </summary>
	public int xMin
	{
		get
		{
			return Math.Min(m_XMin, m_XMin + m_Width);
		}
		set
		{
			int num = xMax;
			m_XMin = value;
			m_Width = num - m_XMin;
		}
	}

	/// <summary>
	///   <para>Returns the minimum Y value of the RectInt.</para>
	/// </summary>
	public int yMin
	{
		get
		{
			return Math.Min(m_YMin, m_YMin + m_Height);
		}
		set
		{
			int num = yMax;
			m_YMin = value;
			m_Height = num - m_YMin;
		}
	}

	/// <summary>
	///   <para>Returns the maximum X value of the RectInt.</para>
	/// </summary>
	public int xMax
	{
		get
		{
			return Math.Max(m_XMin, m_XMin + m_Width);
		}
		set
		{
			m_Width = value - m_XMin;
		}
	}

	/// <summary>
	///   <para>Returns the maximum Y value of the RectInt.</para>
	/// </summary>
	public int yMax
	{
		get
		{
			return Math.Max(m_YMin, m_YMin + m_Height);
		}
		set
		{
			m_Height = value - m_YMin;
		}
	}

	/// <summary>
	///   <para>Returns the position (x, y) of the RectInt.</para>
	/// </summary>
	public Vector2Int position
	{
		get
		{
			return new Vector2Int(m_XMin, m_YMin);
		}
		set
		{
			m_XMin = value.x;
			m_YMin = value.y;
		}
	}

	/// <summary>
	///   <para>Returns the width and height of the RectInt.</para>
	/// </summary>
	public Vector2Int size
	{
		get
		{
			return new Vector2Int(m_Width, m_Height);
		}
		set
		{
			m_Width = value.x;
			m_Height = value.y;
		}
	}

	/// <summary>
	///   <para>A RectInt.PositionCollection that contains all positions within the RectInt.</para>
	/// </summary>
	public PositionEnumerator allPositionsWithin => new PositionEnumerator(min, max);

	public RectInt(int xMin, int yMin, int width, int height)
	{
		m_XMin = xMin;
		m_YMin = yMin;
		m_Width = width;
		m_Height = height;
	}

	public RectInt(Vector2Int position, Vector2Int size)
	{
		m_XMin = position.x;
		m_YMin = position.y;
		m_Width = size.x;
		m_Height = size.y;
	}

	/// <summary>
	///   <para>Sets the bounds to the min and max value of the rect.</para>
	/// </summary>
	/// <param name="minPosition"></param>
	/// <param name="maxPosition"></param>
	public void SetMinMax(Vector2Int minPosition, Vector2Int maxPosition)
	{
		min = minPosition;
		max = maxPosition;
	}

	/// <summary>
	///   <para>Clamps the position and size of the RectInt to the given bounds.</para>
	/// </summary>
	/// <param name="bounds">Bounds to clamp the RectInt.</param>
	public void ClampToBounds(RectInt bounds)
	{
		position = new Vector2Int(Math.Max(Math.Min(bounds.xMax, position.x), bounds.xMin), Math.Max(Math.Min(bounds.yMax, position.y), bounds.yMin));
		size = new Vector2Int(Math.Min(bounds.xMax - position.x, size.x), Math.Min(bounds.yMax - position.y, size.y));
	}

	/// <summary>
	///   <para>Returns true if the given position is within the RectInt.</para>
	/// </summary>
	/// <param name="position">Position to check.</param>
	/// <param name="inclusive">Whether the max limits are included in the check.</param>
	/// <returns>
	///   <para>Whether the position is within the RectInt.</para>
	/// </returns>
	public bool Contains(Vector2Int position)
	{
		return position.x >= m_XMin && position.y >= m_YMin && position.x < m_XMin + m_Width && position.y < m_YMin + m_Height;
	}

	/// <summary>
	///   <para>Returns the x, y, width and height of the RectInt.</para>
	/// </summary>
	public override string ToString()
	{
		return UnityString.Format("(x:{0}, y:{1}, width:{2}, height:{3})", x, y, width, height);
	}
}

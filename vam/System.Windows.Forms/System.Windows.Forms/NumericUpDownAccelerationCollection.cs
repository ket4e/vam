using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

[ListBindable(false)]
public class NumericUpDownAccelerationCollection : MarshalByRefObject, ICollection<NumericUpDownAcceleration>, IEnumerable<NumericUpDownAcceleration>, IEnumerable
{
	private List<NumericUpDownAcceleration> items;

	public int Count => items.Count;

	public bool IsReadOnly => false;

	public NumericUpDownAcceleration this[int index] => items[index];

	public NumericUpDownAccelerationCollection()
	{
		items = new List<NumericUpDownAcceleration>();
	}

	IEnumerator<NumericUpDownAcceleration> IEnumerable<NumericUpDownAcceleration>.GetEnumerator()
	{
		return items.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return items.GetEnumerator();
	}

	public void Add(NumericUpDownAcceleration acceleration)
	{
		if (acceleration == null)
		{
			throw new ArgumentNullException("Acceleration cannot be null");
		}
		int i;
		for (i = 0; i < items.Count && acceleration.Seconds >= items[i].Seconds; i++)
		{
		}
		items.Insert(i, acceleration);
	}

	public void AddRange(params NumericUpDownAcceleration[] accelerations)
	{
		for (int i = 0; i < accelerations.Length; i++)
		{
			Add(accelerations[i]);
		}
	}

	public void Clear()
	{
		items.Clear();
	}

	public bool Contains(NumericUpDownAcceleration acceleration)
	{
		return items.Contains(acceleration);
	}

	public void CopyTo(NumericUpDownAcceleration[] array, int index)
	{
		items.CopyTo(array, index);
	}

	public bool Remove(NumericUpDownAcceleration acceleration)
	{
		return items.Remove(acceleration);
	}
}

using System.Collections;

namespace System.ComponentModel.Design.Serialization;

public sealed class ContextStack
{
	private ArrayList _contextList;

	public object Current
	{
		get
		{
			int count = _contextList.Count;
			if (count > 0)
			{
				return _contextList[count - 1];
			}
			return null;
		}
	}

	public object this[Type type]
	{
		get
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			for (int num = _contextList.Count - 1; num >= 0; num--)
			{
				object obj = _contextList[num];
				if (type.IsInstanceOfType(obj))
				{
					return obj;
				}
			}
			return null;
		}
	}

	public object this[int level]
	{
		get
		{
			if (level < 0)
			{
				throw new ArgumentOutOfRangeException("level");
			}
			int count = _contextList.Count;
			if (count > 0 && count > level)
			{
				return _contextList[count - 1 - level];
			}
			return null;
		}
	}

	public ContextStack()
	{
		_contextList = new ArrayList();
	}

	public object Pop()
	{
		object result = null;
		int count = _contextList.Count;
		if (count > 0)
		{
			int index = count - 1;
			result = _contextList[index];
			_contextList.RemoveAt(index);
		}
		return result;
	}

	public void Push(object context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		_contextList.Add(context);
	}

	public void Append(object context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		_contextList.Insert(0, context);
	}
}

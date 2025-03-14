using System.Collections.Specialized;

namespace System.Configuration;

public sealed class CommaDelimitedStringCollection : StringCollection
{
	private bool modified;

	private bool readOnly;

	public bool IsModified => modified;

	public new bool IsReadOnly => readOnly;

	public new string this[int index]
	{
		get
		{
			return base[index];
		}
		set
		{
			if (readOnly)
			{
				throw new ConfigurationErrorsException("The configuration is read only");
			}
			base[index] = value;
			modified = true;
		}
	}

	public new void Add(string value)
	{
		if (readOnly)
		{
			throw new ConfigurationErrorsException("The configuration is read only");
		}
		base.Add(value);
		modified = true;
	}

	public new void AddRange(string[] range)
	{
		if (readOnly)
		{
			throw new ConfigurationErrorsException("The configuration is read only");
		}
		base.AddRange(range);
		modified = true;
	}

	public new void Clear()
	{
		if (readOnly)
		{
			throw new ConfigurationErrorsException("The configuration is read only");
		}
		base.Clear();
		modified = true;
	}

	public CommaDelimitedStringCollection Clone()
	{
		CommaDelimitedStringCollection commaDelimitedStringCollection = new CommaDelimitedStringCollection();
		string[] array = new string[Count];
		CopyTo(array, 0);
		commaDelimitedStringCollection.AddRange(array);
		return commaDelimitedStringCollection;
	}

	public new void Insert(int index, string value)
	{
		if (readOnly)
		{
			throw new ConfigurationErrorsException("The configuration is read only");
		}
		base.Insert(index, value);
		modified = true;
	}

	public new void Remove(string value)
	{
		if (readOnly)
		{
			throw new ConfigurationErrorsException("The configuration is read only");
		}
		base.Remove(value);
		modified = true;
	}

	public void SetReadOnly()
	{
		readOnly = true;
	}

	public override string ToString()
	{
		if (Count == 0)
		{
			return null;
		}
		string[] array = new string[Count];
		CopyTo(array, 0);
		return string.Join(",", array);
	}
}

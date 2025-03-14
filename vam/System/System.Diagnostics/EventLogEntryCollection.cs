using System.Collections;

namespace System.Diagnostics;

public class EventLogEntryCollection : ICollection, IEnumerable
{
	private class EventLogEntryEnumerator : IEnumerator
	{
		private readonly System.Diagnostics.EventLogImpl _impl;

		private int _currentIndex = -1;

		private EventLogEntry _currentEntry;

		object IEnumerator.Current => Current;

		public EventLogEntry Current
		{
			get
			{
				if (_currentEntry != null)
				{
					return _currentEntry;
				}
				throw new InvalidOperationException("No current EventLog entry available, cursor is located before the first or after the last element of the enumeration.");
			}
		}

		internal EventLogEntryEnumerator(System.Diagnostics.EventLogImpl impl)
		{
			_impl = impl;
		}

		public bool MoveNext()
		{
			_currentIndex++;
			if (_currentIndex >= _impl.EntryCount)
			{
				_currentEntry = null;
				return false;
			}
			_currentEntry = _impl[_currentIndex];
			return true;
		}

		public void Reset()
		{
			_currentIndex = -1;
		}
	}

	private readonly System.Diagnostics.EventLogImpl _impl;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	public int Count => _impl.EntryCount;

	public virtual EventLogEntry this[int index] => _impl[index];

	internal EventLogEntryCollection(System.Diagnostics.EventLogImpl impl)
	{
		_impl = impl;
	}

	void ICollection.CopyTo(Array array, int index)
	{
		EventLogEntry[] entries = _impl.GetEntries();
		Array.Copy(entries, 0, array, index, entries.Length);
	}

	public void CopyTo(EventLogEntry[] eventLogEntries, int index)
	{
		EventLogEntry[] entries = _impl.GetEntries();
		Array.Copy(entries, 0, eventLogEntries, index, entries.Length);
	}

	public IEnumerator GetEnumerator()
	{
		return new EventLogEntryEnumerator(_impl);
	}
}

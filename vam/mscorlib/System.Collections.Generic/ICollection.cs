namespace System.Collections.Generic;

public interface ICollection<T> : IEnumerable, IEnumerable<T>
{
	int Count { get; }

	bool IsReadOnly { get; }

	void Add(T item);

	void Clear();

	bool Contains(T item);

	void CopyTo(T[] array, int arrayIndex);

	bool Remove(T item);
}

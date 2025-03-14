using System.Runtime.InteropServices;

namespace System.Collections;

[ComVisible(true)]
public interface ICollection : IEnumerable
{
	int Count { get; }

	bool IsSynchronized { get; }

	object SyncRoot { get; }

	void CopyTo(Array array, int index);
}

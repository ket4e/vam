using System.Collections;

namespace System.Data;

public interface IDataParameterCollection : IList, IEnumerable, ICollection
{
	object this[string parameterName] { get; set; }

	void RemoveAt(string parameterName);

	int IndexOf(string parameterName);

	bool Contains(string parameterName);
}

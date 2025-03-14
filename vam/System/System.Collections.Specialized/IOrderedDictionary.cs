namespace System.Collections.Specialized;

public interface IOrderedDictionary : IDictionary, ICollection, IEnumerable
{
	object this[int idx] { get; set; }

	new IDictionaryEnumerator GetEnumerator();

	void Insert(int idx, object key, object value);

	void RemoveAt(int idx);
}

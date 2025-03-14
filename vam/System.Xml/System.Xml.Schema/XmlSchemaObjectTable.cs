using System.Collections;
using System.Collections.Specialized;

namespace System.Xml.Schema;

public class XmlSchemaObjectTable
{
	internal class XmlSchemaObjectTableEnumerator : IEnumerator, IDictionaryEnumerator
	{
		private IDictionaryEnumerator xenum;

		private IEnumerable tmp;

		object IEnumerator.Current => xenum.Entry;

		DictionaryEntry IDictionaryEnumerator.Entry => xenum.Entry;

		object IDictionaryEnumerator.Key => (XmlQualifiedName)xenum.Key;

		object IDictionaryEnumerator.Value => (XmlSchemaObject)xenum.Value;

		public XmlSchemaObject Current => (XmlSchemaObject)xenum.Value;

		public DictionaryEntry Entry => xenum.Entry;

		public XmlQualifiedName Key => (XmlQualifiedName)xenum.Key;

		public XmlSchemaObject Value => (XmlSchemaObject)xenum.Value;

		internal XmlSchemaObjectTableEnumerator(XmlSchemaObjectTable table)
		{
			tmp = table.table;
			xenum = (IDictionaryEnumerator)tmp.GetEnumerator();
		}

		bool IEnumerator.MoveNext()
		{
			return xenum.MoveNext();
		}

		void IEnumerator.Reset()
		{
			xenum.Reset();
		}

		public bool MoveNext()
		{
			return xenum.MoveNext();
		}
	}

	private HybridDictionary table;

	public int Count => table.Count;

	public XmlSchemaObject this[XmlQualifiedName name] => (XmlSchemaObject)table[name];

	public ICollection Names => table.Keys;

	public ICollection Values => table.Values;

	internal XmlSchemaObjectTable()
	{
		table = new HybridDictionary();
	}

	public bool Contains(XmlQualifiedName name)
	{
		return table.Contains(name);
	}

	public IDictionaryEnumerator GetEnumerator()
	{
		return new XmlSchemaObjectTableEnumerator(this);
	}

	internal void Add(XmlQualifiedName name, XmlSchemaObject value)
	{
		table[name] = value;
	}

	internal void Clear()
	{
		table.Clear();
	}

	internal void Set(XmlQualifiedName name, XmlSchemaObject value)
	{
		table[name] = value;
	}
}

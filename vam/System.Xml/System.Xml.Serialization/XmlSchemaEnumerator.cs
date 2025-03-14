using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;

namespace System.Xml.Serialization;

[System.MonoTODO]
public class XmlSchemaEnumerator : IEnumerator<XmlSchema>, IDisposable, IEnumerator
{
	private IEnumerator e;

	object IEnumerator.Current => Current;

	public XmlSchema Current => (XmlSchema)e.Current;

	public XmlSchemaEnumerator(XmlSchemas list)
	{
		e = list.GetEnumerator();
	}

	void IEnumerator.Reset()
	{
		e.Reset();
	}

	public void Dispose()
	{
	}

	public bool MoveNext()
	{
		return e.MoveNext();
	}
}

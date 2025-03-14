using System.Collections;
using System.IO;

namespace System.Resources;

public class ResXResourceSet : ResourceSet
{
	public ResXResourceSet(Stream stream)
	{
		Reader = new ResXResourceReader(stream);
		Table = new Hashtable();
		ReadResources();
	}

	public ResXResourceSet(string fileName)
	{
		Reader = new ResXResourceReader(fileName);
		Table = new Hashtable();
		ReadResources();
	}

	public override Type GetDefaultReader()
	{
		return typeof(ResXResourceReader);
	}

	public override Type GetDefaultWriter()
	{
		return typeof(ResXResourceWriter);
	}
}

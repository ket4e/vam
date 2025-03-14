using System.IO;

namespace System.Resources;

[Serializable]
internal class RuntimeResourceSet : ResourceSet
{
	public RuntimeResourceSet(UnmanagedMemoryStream stream)
		: base(stream)
	{
	}

	public RuntimeResourceSet(Stream stream)
		: base(stream)
	{
	}

	public RuntimeResourceSet(string fileName)
		: base(fileName)
	{
	}

	public override object GetObject(string name)
	{
		if (Reader == null)
		{
			throw new ObjectDisposedException("ResourceSet is closed.");
		}
		return CloneDisposableObjectIfPossible(base.GetObject(name));
	}

	public override object GetObject(string name, bool ignoreCase)
	{
		if (Reader == null)
		{
			throw new ObjectDisposedException("ResourceSet is closed.");
		}
		return CloneDisposableObjectIfPossible(base.GetObject(name, ignoreCase));
	}

	private object CloneDisposableObjectIfPossible(object value)
	{
		return (!(value is ICloneable cloneable) || !(value is IDisposable)) ? value : cloneable.Clone();
	}
}

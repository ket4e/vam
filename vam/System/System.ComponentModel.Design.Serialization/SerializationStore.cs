using System.Collections;
using System.IO;

namespace System.ComponentModel.Design.Serialization;

public abstract class SerializationStore : IDisposable
{
	public abstract ICollection Errors { get; }

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
	}

	public abstract void Close();

	public abstract void Save(Stream stream);

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			Close();
		}
	}
}

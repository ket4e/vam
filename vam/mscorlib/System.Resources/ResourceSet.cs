using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Resources;

[Serializable]
[ComVisible(true)]
public class ResourceSet : IEnumerable, IDisposable
{
	[NonSerialized]
	protected IResourceReader Reader;

	protected Hashtable Table;

	private bool resources_read;

	[NonSerialized]
	private bool disposed;

	protected ResourceSet()
	{
		Table = new Hashtable();
		resources_read = true;
	}

	public ResourceSet(IResourceReader reader)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		Table = new Hashtable();
		Reader = reader;
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
	public ResourceSet(Stream stream)
	{
		Table = new Hashtable();
		Reader = new ResourceReader(stream);
	}

	internal ResourceSet(UnmanagedMemoryStream stream)
	{
		Table = new Hashtable();
		Reader = new ResourceReader(stream);
	}

	public ResourceSet(string fileName)
	{
		Table = new Hashtable();
		Reader = new ResourceReader(fileName);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public virtual void Close()
	{
		Dispose();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && Reader != null)
		{
			Reader.Close();
		}
		Reader = null;
		Table = null;
		disposed = true;
	}

	public virtual Type GetDefaultReader()
	{
		return typeof(ResourceReader);
	}

	public virtual Type GetDefaultWriter()
	{
		return typeof(ResourceWriter);
	}

	[ComVisible(false)]
	public virtual IDictionaryEnumerator GetEnumerator()
	{
		if (disposed)
		{
			throw new ObjectDisposedException("ResourceSet is closed.");
		}
		ReadResources();
		return Table.GetEnumerator();
	}

	private object GetObjectInternal(string name, bool ignoreCase)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (disposed)
		{
			throw new ObjectDisposedException("ResourceSet is closed.");
		}
		ReadResources();
		object obj = Table[name];
		if (obj != null)
		{
			return obj;
		}
		if (ignoreCase)
		{
			foreach (DictionaryEntry item in Table)
			{
				string strA = (string)item.Key;
				if (string.Compare(strA, name, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
				{
					return item.Value;
				}
			}
		}
		return null;
	}

	public virtual object GetObject(string name)
	{
		return GetObjectInternal(name, ignoreCase: false);
	}

	public virtual object GetObject(string name, bool ignoreCase)
	{
		return GetObjectInternal(name, ignoreCase);
	}

	private string GetStringInternal(string name, bool ignoreCase)
	{
		object @object = GetObject(name, ignoreCase);
		if (@object == null)
		{
			return null;
		}
		if (!(@object is string result))
		{
			throw new InvalidOperationException($"Resource '{name}' is not a String. Use GetObject instead.");
		}
		return result;
	}

	public virtual string GetString(string name)
	{
		return GetStringInternal(name, ignoreCase: false);
	}

	public virtual string GetString(string name, bool ignoreCase)
	{
		return GetStringInternal(name, ignoreCase);
	}

	protected virtual void ReadResources()
	{
		if (resources_read)
		{
			return;
		}
		if (Reader == null)
		{
			throw new ObjectDisposedException("ResourceSet is closed.");
		}
		lock (Table)
		{
			if (!resources_read)
			{
				IDictionaryEnumerator enumerator = Reader.GetEnumerator();
				enumerator.Reset();
				while (enumerator.MoveNext())
				{
					Table.Add(enumerator.Key, enumerator.Value);
				}
				resources_read = true;
			}
		}
	}

	internal UnmanagedMemoryStream GetStream(string name, bool ignoreCase)
	{
		if (Reader == null)
		{
			throw new ObjectDisposedException("ResourceSet is closed.");
		}
		IDictionaryEnumerator enumerator = Reader.GetEnumerator();
		enumerator.Reset();
		while (enumerator.MoveNext())
		{
			if (string.Compare(name, (string)enumerator.Key, ignoreCase) == 0)
			{
				return ((ResourceReader.ResourceEnumerator)enumerator).ValueAsStream;
			}
		}
		return null;
	}
}

using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Resources;

[Serializable]
public sealed class ResXDataNode : ISerializable
{
	private string name;

	private object value;

	private Type type;

	private ResXFileRef fileRef;

	private string comment;

	private Point pos;

	public string Comment
	{
		get
		{
			return comment;
		}
		set
		{
			comment = value;
		}
	}

	public ResXFileRef FileRef => fileRef;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	internal object Value => value;

	public ResXDataNode(string name, object value)
		: this(name, value, Point.Empty)
	{
	}

	public ResXDataNode(string name, ResXFileRef fileRef)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (fileRef == null)
		{
			throw new ArgumentNullException("fileRef");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("name");
		}
		this.name = name;
		this.fileRef = fileRef;
		pos = Point.Empty;
	}

	internal ResXDataNode(string name, object value, Point position)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Length == 0)
		{
			throw new ArgumentException("name");
		}
		Type type = ((value != null) ? value.GetType() : typeof(object));
		if (value != null && !type.IsSerializable)
		{
			throw new InvalidOperationException($"'{name}' of type '{type}' cannot be added because it is not serializable");
		}
		this.type = type;
		this.name = name;
		this.value = value;
		pos = position;
	}

	void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
	{
		si.AddValue("Name", Name);
		si.AddValue("Comment", Comment);
	}

	public Point GetNodePosition()
	{
		return pos;
	}

	[System.MonoInternalNote("Move the type parsing process from ResxResourceReader")]
	public string GetValueTypeName(AssemblyName[] names)
	{
		return type.AssemblyQualifiedName;
	}

	[System.MonoInternalNote("Move the type parsing process from ResxResourceReader")]
	public string GetValueTypeName(ITypeResolutionService typeResolver)
	{
		return type.AssemblyQualifiedName;
	}

	[System.MonoInternalNote("Move the value parsing process from ResxResourceReader")]
	public object GetValue(AssemblyName[] names)
	{
		return value;
	}

	[System.MonoInternalNote("Move the value parsing process from ResxResourceReader")]
	public object GetValue(ITypeResolutionService typeResolver)
	{
		return value;
	}
}

using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System;

[Serializable]
[ComVisible(true)]
[CLSCompliant(false)]
public struct UIntPtr : ISerializable
{
	public static readonly UIntPtr Zero = new UIntPtr(0u);

	private unsafe void* _pointer;

	public unsafe static int Size => sizeof(void*);

	public unsafe UIntPtr(ulong value)
	{
		if (value > uint.MaxValue && Size < 8)
		{
			throw new OverflowException(Locale.GetText("This isn't a 64bits machine."));
		}
		_pointer = (void*)value;
	}

	public unsafe UIntPtr(uint value)
	{
		_pointer = (void*)value;
	}

	[CLSCompliant(false)]
	public unsafe UIntPtr(void* value)
	{
		_pointer = value;
	}

	unsafe void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("pointer", (ulong)_pointer);
	}

	public unsafe override bool Equals(object obj)
	{
		if (obj is UIntPtr uIntPtr)
		{
			return _pointer == uIntPtr._pointer;
		}
		return false;
	}

	public unsafe override int GetHashCode()
	{
		return (int)_pointer;
	}

	public unsafe uint ToUInt32()
	{
		return (uint)_pointer;
	}

	public unsafe ulong ToUInt64()
	{
		return (ulong)_pointer;
	}

	[CLSCompliant(false)]
	public unsafe void* ToPointer()
	{
		return _pointer;
	}

	public unsafe override string ToString()
	{
		return ((uint)_pointer).ToString();
	}

	public unsafe static bool operator ==(UIntPtr value1, UIntPtr value2)
	{
		return value1._pointer == value2._pointer;
	}

	public unsafe static bool operator !=(UIntPtr value1, UIntPtr value2)
	{
		return value1._pointer != value2._pointer;
	}

	public unsafe static explicit operator ulong(UIntPtr value)
	{
		return (ulong)value._pointer;
	}

	public unsafe static explicit operator uint(UIntPtr value)
	{
		return (uint)value._pointer;
	}

	public static explicit operator UIntPtr(ulong value)
	{
		return new UIntPtr(value);
	}

	[CLSCompliant(false)]
	public unsafe static explicit operator UIntPtr(void* value)
	{
		return new UIntPtr(value);
	}

	[CLSCompliant(false)]
	public unsafe static explicit operator void*(UIntPtr value)
	{
		return value.ToPointer();
	}

	public static explicit operator UIntPtr(uint value)
	{
		return new UIntPtr(value);
	}
}

using System.Runtime.InteropServices;

namespace System;

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
[ComVisible(true)]
public sealed class NonSerializedAttribute : Attribute
{
}

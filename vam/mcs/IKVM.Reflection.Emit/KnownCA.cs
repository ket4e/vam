namespace IKVM.Reflection.Emit;

internal enum KnownCA
{
	Unknown,
	DllImportAttribute,
	ComImportAttribute,
	SerializableAttribute,
	NonSerializedAttribute,
	MethodImplAttribute,
	MarshalAsAttribute,
	PreserveSigAttribute,
	InAttribute,
	OutAttribute,
	OptionalAttribute,
	StructLayoutAttribute,
	FieldOffsetAttribute,
	SpecialNameAttribute,
	SuppressUnmanagedCodeSecurityAttribute
}

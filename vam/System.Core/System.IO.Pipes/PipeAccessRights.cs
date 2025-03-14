namespace System.IO.Pipes;

[Flags]
public enum PipeAccessRights
{
	ReadData = 1,
	WriteData = 2,
	ReadAttributes = 4,
	WriteAttributes = 8,
	ReadExtendedAttributes = 0x10,
	WriteExtendedAttributes = 0x20,
	CreateNewInstance = 0x40,
	Delete = 0x80,
	ReadPermissions = 0x100,
	ChangePermissions = 0x200,
	TakeOwnership = 0x400,
	Synchronize = 0x800,
	FullControl = 0x73F,
	Read = 0x115,
	Write = 0x22A,
	ReadWrite = 0x33F,
	AccessSystemSecurity = 0x700
}

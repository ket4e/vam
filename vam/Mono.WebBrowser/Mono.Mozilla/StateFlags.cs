using System;

namespace Mono.Mozilla;

[Flags]
internal enum StateFlags
{
	Start = 1,
	Redirecting = 2,
	Transferring = 4,
	Negotiating = 8,
	Stop = 0x10,
	IsRequest = 0x10000,
	IsDocument = 0x20000,
	IsNetwork = 0x40000,
	IsWindow = 0x80000,
	Restoring = 0x1000000,
	IsInsecure = 4,
	IsBroken = 1,
	IsSecure = 2,
	SecureHigh = 0x40000,
	SecureMed = 0x10000,
	SecureLow = 0x20000
}

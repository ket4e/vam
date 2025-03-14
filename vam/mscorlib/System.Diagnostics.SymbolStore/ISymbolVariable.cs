using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore;

[ComVisible(true)]
public interface ISymbolVariable
{
	int AddressField1 { get; }

	int AddressField2 { get; }

	int AddressField3 { get; }

	SymAddressKind AddressKind { get; }

	object Attributes { get; }

	int EndOffset { get; }

	string Name { get; }

	int StartOffset { get; }

	byte[] GetSignature();
}

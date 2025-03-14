using System;
using System.IO;
using System.IO.Compression;
using Mono.Cecil.PE;

namespace Mono.Cecil.Cil;

public sealed class EmbeddedPortablePdbReaderProvider : ISymbolReaderProvider
{
	public ISymbolReader GetSymbolReader(ModuleDefinition module, string fileName)
	{
		Mixin.CheckModule(module);
		ImageDebugHeaderEntry embeddedPortablePdbEntry = module.GetDebugHeader().GetEmbeddedPortablePdbEntry();
		if (embeddedPortablePdbEntry == null)
		{
			throw new InvalidOperationException();
		}
		return new EmbeddedPortablePdbReader((PortablePdbReader)new PortablePdbReaderProvider().GetSymbolReader(module, GetPortablePdbStream(embeddedPortablePdbEntry)));
	}

	private static Stream GetPortablePdbStream(ImageDebugHeaderEntry entry)
	{
		MemoryStream memoryStream = new MemoryStream(entry.Data);
		BinaryStreamReader binaryStreamReader = new BinaryStreamReader(memoryStream);
		binaryStreamReader.ReadInt32();
		MemoryStream memoryStream2 = new MemoryStream(binaryStreamReader.ReadInt32());
		using DeflateStream self = new DeflateStream(memoryStream, CompressionMode.Decompress, leaveOpen: true);
		self.CopyTo(memoryStream2);
		return memoryStream2;
	}

	public ISymbolReader GetSymbolReader(ModuleDefinition module, Stream symbolStream)
	{
		throw new NotSupportedException();
	}
}

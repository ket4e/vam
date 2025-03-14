namespace Mono.Mozilla.DOM;

internal enum DocumentEncoderFlags : uint
{
	OutputSelectionOnly = 1u,
	OutputFormatted = 2u,
	OutputRaw = 4u,
	OutputBodyOnly = 8u,
	OutputPreformatted = 0x10u,
	OutputWrap = 0x20u,
	OutputFormatFlowed = 0x40u,
	OutputAbsoluteLinks = 0x80u,
	OutputEncodeW3CEntities = 0x100u,
	OutputCRLineBreak = 0x200u,
	OutputLFLineBreak = 0x400u,
	OutputNoScriptContent = 0x800u,
	OutputNoFramesContent = 0x1000u,
	OutputNoFormattingInPre = 0x2000u,
	OutputEncodeBasicEntities = 0x4000u,
	OutputEncodeLatin1Entities = 0x8000u,
	OutputEncodeHTMLEntities = 0x10000u,
	OutputPersistNBSP = 0x20000u
}

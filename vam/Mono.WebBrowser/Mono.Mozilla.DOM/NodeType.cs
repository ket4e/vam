namespace Mono.Mozilla.DOM;

internal enum NodeType : ushort
{
	Element = 1,
	Attribute,
	Text,
	CDataSection,
	EntityReference,
	Entity,
	ProcessingInstruction,
	Comment,
	Document,
	DocumentType,
	DocumentFragment,
	Notation
}

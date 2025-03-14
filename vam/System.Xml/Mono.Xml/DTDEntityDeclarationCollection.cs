using System;

namespace Mono.Xml;

internal class DTDEntityDeclarationCollection : DTDCollectionBase
{
	public DTDEntityDeclaration this[string name] => BaseGet(name) as DTDEntityDeclaration;

	public DTDEntityDeclarationCollection(DTDObjectModel root)
		: base(root)
	{
	}

	public void Add(string name, DTDEntityDeclaration decl)
	{
		if (Contains(name))
		{
			throw new InvalidOperationException($"Entity declaration for {name} was already added.");
		}
		decl.SetRoot(base.Root);
		BaseAdd(name, decl);
	}
}

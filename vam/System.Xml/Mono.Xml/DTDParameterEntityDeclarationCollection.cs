using System.Collections;

namespace Mono.Xml;

internal class DTDParameterEntityDeclarationCollection
{
	private Hashtable peDecls = new Hashtable();

	private DTDObjectModel root;

	public DTDParameterEntityDeclaration this[string name] => peDecls[name] as DTDParameterEntityDeclaration;

	public ICollection Keys => peDecls.Keys;

	public ICollection Values => peDecls.Values;

	public DTDParameterEntityDeclarationCollection(DTDObjectModel root)
	{
		this.root = root;
	}

	public void Add(string name, DTDParameterEntityDeclaration decl)
	{
		if (peDecls[name] == null)
		{
			decl.SetRoot(root);
			peDecls.Add(name, decl);
		}
	}
}

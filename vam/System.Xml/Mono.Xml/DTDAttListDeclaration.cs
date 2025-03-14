using System;
using System.Collections;

namespace Mono.Xml;

internal class DTDAttListDeclaration : DTDNode
{
	private string name;

	private Hashtable attributeOrders = new Hashtable();

	private ArrayList attributes = new ArrayList();

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public DTDAttributeDefinition this[int i] => Get(i);

	public DTDAttributeDefinition this[string name] => Get(name);

	public IList Definitions => attributes;

	public int Count => attributeOrders.Count;

	internal DTDAttListDeclaration(DTDObjectModel root)
	{
		SetRoot(root);
	}

	public DTDAttributeDefinition Get(int i)
	{
		return attributes[i] as DTDAttributeDefinition;
	}

	public DTDAttributeDefinition Get(string name)
	{
		object obj = attributeOrders[name];
		if (obj != null)
		{
			return attributes[(int)obj] as DTDAttributeDefinition;
		}
		return null;
	}

	public void Add(DTDAttributeDefinition def)
	{
		if (attributeOrders[def.Name] != null)
		{
			throw new InvalidOperationException($"Attribute definition for {def.Name} was already added at element {Name}.");
		}
		def.SetRoot(base.Root);
		attributeOrders.Add(def.Name, attributes.Count);
		attributes.Add(def);
	}
}

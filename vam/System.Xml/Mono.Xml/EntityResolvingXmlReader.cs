using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Xml;

namespace Mono.Xml;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
internal class EntityResolvingXmlReader : XmlReader, IHasXmlParserContext, IXmlLineInfo, IXmlNamespaceResolver
{
	private EntityResolvingXmlReader entity;

	private XmlReader source;

	private XmlParserContext context;

	private XmlResolver resolver;

	private EntityHandling entity_handling;

	private bool entity_inside_attr;

	private bool inside_attr;

	private bool do_resolve;

	XmlParserContext IHasXmlParserContext.ParserContext => context;

	private XmlReader Current => (entity == null || entity.ReadState == ReadState.Initial) ? source : entity;

	public override int AttributeCount => Current.AttributeCount;

	public override string BaseURI => Current.BaseURI;

	public override bool CanResolveEntity => true;

	public override int Depth
	{
		get
		{
			if (entity != null && entity.ReadState == ReadState.Interactive)
			{
				return source.Depth + entity.Depth + 1;
			}
			return source.Depth;
		}
	}

	public override bool EOF => source.EOF;

	public override bool HasValue => Current.HasValue;

	public override bool IsDefault => Current.IsDefault;

	public override bool IsEmptyElement => Current.IsEmptyElement;

	public override string LocalName => Current.LocalName;

	public override string Name => Current.Name;

	public override string NamespaceURI => Current.NamespaceURI;

	public override XmlNameTable NameTable => Current.NameTable;

	public override XmlNodeType NodeType
	{
		get
		{
			if (entity != null)
			{
				if (entity.ReadState == ReadState.Initial)
				{
					return source.NodeType;
				}
				return (!entity.EOF) ? entity.NodeType : XmlNodeType.EndEntity;
			}
			return source.NodeType;
		}
	}

	internal XmlParserContext ParserContext => context;

	public override string Prefix => Current.Prefix;

	public override char QuoteChar => Current.QuoteChar;

	public override ReadState ReadState => (entity != null) ? ReadState.Interactive : source.ReadState;

	public override string Value => Current.Value;

	public override string XmlLang => Current.XmlLang;

	public override XmlSpace XmlSpace => Current.XmlSpace;

	public EntityHandling EntityHandling
	{
		get
		{
			return entity_handling;
		}
		set
		{
			if (entity != null)
			{
				entity.EntityHandling = value;
			}
			entity_handling = value;
		}
	}

	public int LineNumber => (Current is IXmlLineInfo xmlLineInfo) ? xmlLineInfo.LineNumber : 0;

	public int LinePosition => (Current is IXmlLineInfo xmlLineInfo) ? xmlLineInfo.LinePosition : 0;

	public XmlResolver XmlResolver
	{
		set
		{
			if (entity != null)
			{
				entity.XmlResolver = value;
			}
			resolver = value;
		}
	}

	public EntityResolvingXmlReader(XmlReader source)
	{
		this.source = source;
		if (source is IHasXmlParserContext hasXmlParserContext)
		{
			context = hasXmlParserContext.ParserContext;
		}
		else
		{
			context = new XmlParserContext(source.NameTable, new XmlNamespaceManager(source.NameTable), null, XmlSpace.None);
		}
	}

	private EntityResolvingXmlReader(XmlReader entityContainer, bool inside_attr)
	{
		source = entityContainer;
		entity_inside_attr = inside_attr;
	}

	IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
	{
		return GetNamespacesInScope(scope);
	}

	string IXmlNamespaceResolver.LookupPrefix(string ns)
	{
		return ((IXmlNamespaceResolver)Current).LookupPrefix(ns);
	}

	private void CopyProperties(EntityResolvingXmlReader other)
	{
		context = other.context;
		resolver = other.resolver;
		entity_handling = other.entity_handling;
	}

	public override void Close()
	{
		if (entity != null)
		{
			entity.Close();
		}
		source.Close();
	}

	public override string GetAttribute(int i)
	{
		return Current.GetAttribute(i);
	}

	public override string GetAttribute(string name)
	{
		return Current.GetAttribute(name);
	}

	public override string GetAttribute(string localName, string namespaceURI)
	{
		return Current.GetAttribute(localName, namespaceURI);
	}

	public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
	{
		return ((IXmlNamespaceResolver)Current).GetNamespacesInScope(scope);
	}

	public override string LookupNamespace(string prefix)
	{
		return Current.LookupNamespace(prefix);
	}

	public override void MoveToAttribute(int i)
	{
		if (entity != null && entity_inside_attr)
		{
			entity.Close();
			entity = null;
		}
		Current.MoveToAttribute(i);
		inside_attr = true;
	}

	public override bool MoveToAttribute(string name)
	{
		if (entity != null && !entity_inside_attr)
		{
			return entity.MoveToAttribute(name);
		}
		if (!source.MoveToAttribute(name))
		{
			return false;
		}
		if (entity != null && entity_inside_attr)
		{
			entity.Close();
			entity = null;
		}
		inside_attr = true;
		return true;
	}

	public override bool MoveToAttribute(string localName, string namespaceName)
	{
		if (entity != null && !entity_inside_attr)
		{
			return entity.MoveToAttribute(localName, namespaceName);
		}
		if (!source.MoveToAttribute(localName, namespaceName))
		{
			return false;
		}
		if (entity != null && entity_inside_attr)
		{
			entity.Close();
			entity = null;
		}
		inside_attr = true;
		return true;
	}

	public override bool MoveToElement()
	{
		if (entity != null && entity_inside_attr)
		{
			entity.Close();
			entity = null;
		}
		if (!Current.MoveToElement())
		{
			return false;
		}
		inside_attr = false;
		return true;
	}

	public override bool MoveToFirstAttribute()
	{
		if (entity != null && !entity_inside_attr)
		{
			return entity.MoveToFirstAttribute();
		}
		if (!source.MoveToFirstAttribute())
		{
			return false;
		}
		if (entity != null && entity_inside_attr)
		{
			entity.Close();
			entity = null;
		}
		inside_attr = true;
		return true;
	}

	public override bool MoveToNextAttribute()
	{
		if (entity != null && !entity_inside_attr)
		{
			return entity.MoveToNextAttribute();
		}
		if (!source.MoveToNextAttribute())
		{
			return false;
		}
		if (entity != null && entity_inside_attr)
		{
			entity.Close();
			entity = null;
		}
		inside_attr = true;
		return true;
	}

	public override bool Read()
	{
		if (do_resolve)
		{
			DoResolveEntity();
			do_resolve = false;
		}
		inside_attr = false;
		if (entity != null && (entity_inside_attr || entity.EOF))
		{
			entity.Close();
			entity = null;
		}
		if (entity != null)
		{
			if (entity.Read())
			{
				return true;
			}
			if (EntityHandling == EntityHandling.ExpandEntities)
			{
				entity.Close();
				entity = null;
				return Read();
			}
			return true;
		}
		if (!source.Read())
		{
			return false;
		}
		if (EntityHandling == EntityHandling.ExpandEntities && source.NodeType == XmlNodeType.EntityReference)
		{
			ResolveEntity();
			return Read();
		}
		return true;
	}

	public override bool ReadAttributeValue()
	{
		if (entity != null && entity_inside_attr)
		{
			if (!entity.EOF)
			{
				entity.Read();
				return true;
			}
			entity.Close();
			entity = null;
		}
		return Current.ReadAttributeValue();
	}

	public override string ReadString()
	{
		return base.ReadString();
	}

	public override void ResolveEntity()
	{
		DoResolveEntity();
	}

	private void DoResolveEntity()
	{
		if (entity != null)
		{
			entity.ResolveEntity();
			return;
		}
		if (source.NodeType != XmlNodeType.EntityReference)
		{
			throw new InvalidOperationException("The current node is not an Entity Reference");
		}
		if (ParserContext.Dtd == null)
		{
			throw new XmlException(this, BaseURI, $"Cannot resolve entity without DTD: '{source.Name}'");
		}
		XmlReader xmlReader = ParserContext.Dtd.GenerateEntityContentReader(source.Name, ParserContext);
		if (xmlReader == null)
		{
			throw new XmlException(this, BaseURI, $"Reference to undeclared entity '{source.Name}'.");
		}
		entity = new EntityResolvingXmlReader(xmlReader, inside_attr);
		entity.CopyProperties(this);
	}

	public override void Skip()
	{
		base.Skip();
	}

	public bool HasLineInfo()
	{
		return Current is IXmlLineInfo xmlLineInfo && xmlLineInfo.HasLineInfo();
	}
}

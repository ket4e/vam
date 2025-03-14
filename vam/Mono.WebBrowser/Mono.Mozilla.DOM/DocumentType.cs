using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class DocumentType : Node, IDocumentType, INode
{
	internal nsIDOMDocumentType doctype;

	internal nsIDOMDocumentType ComObject => doctype;

	public string Name
	{
		get
		{
			doctype.getName(storage);
			return Base.StringGet(storage);
		}
	}

	public INamedNodeMap Entities
	{
		get
		{
			doctype.getEntities(out var ret);
			return new NamedNodeMap(control, ret);
		}
	}

	public INamedNodeMap Notations
	{
		get
		{
			doctype.getNotations(out var ret);
			return new NamedNodeMap(control, ret);
		}
	}

	public string PublicId
	{
		get
		{
			doctype.getPublicId(storage);
			return Base.StringGet(storage);
		}
	}

	public string SystemId
	{
		get
		{
			doctype.getSystemId(storage);
			return Base.StringGet(storage);
		}
	}

	public string InternalSubset
	{
		get
		{
			doctype.getInternalSubset(storage);
			return Base.StringGet(storage);
		}
	}

	public DocumentType(WebBrowser control, nsIDOMDocumentType doctype)
		: base(control, doctype)
	{
		if (control.platform != control.enginePlatform)
		{
			this.doctype = nsDOMDocumentType.GetProxy(control, doctype);
		}
		else
		{
			this.doctype = doctype;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			resources.Clear();
			doctype = null;
		}
		base.Dispose(disposing);
	}

	public override int GetHashCode()
	{
		return hashcode;
	}
}

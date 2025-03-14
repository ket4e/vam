using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class Attribute : Node, IAttribute, INode
{
	private nsIDOMAttr attribute;

	public string Name
	{
		get
		{
			attribute.getName(storage);
			return Base.StringGet(storage);
		}
	}

	public new string Value
	{
		get
		{
			attribute.getValue(storage);
			return Base.StringGet(storage);
		}
		set
		{
			Base.StringSet(storage, value);
			attribute.setValue(storage);
		}
	}

	public Attribute(WebBrowser control, nsIDOMAttr domAttribute)
		: base(control, domAttribute)
	{
		if (control.platform != control.enginePlatform)
		{
			attribute = nsDOMAttr.GetProxy(control, domAttribute);
		}
		else
		{
			attribute = domAttribute;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			attribute = null;
		}
		base.Dispose(disposing);
	}

	public override int GetHashCode()
	{
		return hashcode;
	}
}

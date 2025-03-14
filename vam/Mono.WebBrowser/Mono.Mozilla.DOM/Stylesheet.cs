using Mono.WebBrowser.DOM;

namespace Mono.Mozilla.DOM;

internal class Stylesheet : DOMObject, IStylesheet
{
	private nsIDOMStyleSheet unmanagedStyle;

	protected int hashcode;

	public string Type
	{
		get
		{
			unmanagedStyle.getType(storage);
			return Base.StringGet(storage);
		}
	}

	public string Href
	{
		get
		{
			unmanagedStyle.getHref(storage);
			return Base.StringGet(storage);
		}
	}

	public bool Disabled
	{
		get
		{
			unmanagedStyle.getDisabled(out var ret);
			return ret;
		}
		set
		{
			unmanagedStyle.setDisabled(value);
		}
	}

	public INode OwnerNode
	{
		get
		{
			unmanagedStyle.getOwnerNode(out var ret);
			return GetTypedNode(ret);
		}
	}

	public IStylesheet ParentStyleSheet
	{
		get
		{
			unmanagedStyle.getParentStyleSheet(out var ret);
			return new Stylesheet(control, ret);
		}
	}

	public string Title
	{
		get
		{
			unmanagedStyle.getTitle(storage);
			return Base.StringGet(storage);
		}
	}

	public IMediaList Media => null;

	public Stylesheet(WebBrowser control, nsIDOMStyleSheet stylesheet)
		: base(control)
	{
		if (control.platform != control.enginePlatform)
		{
			unmanagedStyle = nsDOMStyleSheet.GetProxy(control, stylesheet);
		}
		else
		{
			unmanagedStyle = stylesheet;
		}
		hashcode = unmanagedStyle.GetHashCode();
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			unmanagedStyle = null;
		}
		base.Dispose(disposing);
	}

	public override int GetHashCode()
	{
		return hashcode;
	}
}

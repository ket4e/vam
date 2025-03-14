using System;

namespace Mono.WebBrowser.DOM;

public class NodeEventArgs : EventArgs
{
	private INode node;

	public INode Node => node;

	public IElement Element
	{
		get
		{
			if (node is IElement)
			{
				return (IElement)node;
			}
			return null;
		}
	}

	public IDocument Document
	{
		get
		{
			if (node is IDocument)
			{
				return (IDocument)node;
			}
			return null;
		}
	}

	public NodeEventArgs(INode node)
	{
		this.node = node;
	}
}

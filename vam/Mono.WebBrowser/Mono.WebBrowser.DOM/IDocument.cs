using System;

namespace Mono.WebBrowser.DOM;

public interface IDocument : INode
{
	IElement Active { get; }

	string ActiveLinkColor { get; set; }

	IElementCollection Anchors { get; }

	IElementCollection Applets { get; }

	string Background { get; set; }

	string BackColor { get; set; }

	IElement Body { get; }

	string Charset { get; set; }

	string Cookie { get; set; }

	IElement DocumentElement { get; }

	IDocumentType DocType { get; }

	string Domain { get; }

	string ForeColor { get; set; }

	IElementCollection Forms { get; }

	IElementCollection Images { get; }

	IDOMImplementation Implementation { get; }

	string LinkColor { get; set; }

	IElementCollection Links { get; }

	IStylesheetList Stylesheets { get; }

	string Title { get; set; }

	string Url { get; }

	string VisitedLinkColor { get; set; }

	IWindow Window { get; }

	event EventHandler LoadStopped;

	IAttribute CreateAttribute(string name);

	IElement CreateElement(string tagName);

	IElement GetElementById(string id);

	IElement GetElement(int x, int y);

	IElementCollection GetElementsByTagName(string id);

	void Write(string text);

	string InvokeScript(string script);

	new int GetHashCode();
}

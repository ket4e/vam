using System.IO;

namespace Mono.WebBrowser.DOM;

public interface IElement : INode
{
	IElementCollection All { get; }

	IElementCollection Children { get; }

	int ClientWidth { get; }

	int ClientHeight { get; }

	int ScrollHeight { get; }

	int ScrollWidth { get; }

	int ScrollLeft { get; set; }

	int ScrollTop { get; set; }

	int OffsetHeight { get; }

	int OffsetWidth { get; }

	int OffsetLeft { get; }

	int OffsetTop { get; }

	IElement OffsetParent { get; }

	string InnerText { get; set; }

	string InnerHTML { get; set; }

	string OuterText { get; set; }

	string OuterHTML { get; set; }

	string Style { get; set; }

	int TabIndex { get; set; }

	string TagName { get; }

	bool Disabled { get; set; }

	Stream ContentStream { get; }

	IElement AppendChild(IElement child);

	void Blur();

	void Focus();

	bool HasAttribute(string name);

	string GetAttribute(string name);

	IElementCollection GetElementsByTagName(string id);

	new int GetHashCode();

	void ScrollIntoView(bool alignWithTop);

	void SetAttribute(string name, string value);
}

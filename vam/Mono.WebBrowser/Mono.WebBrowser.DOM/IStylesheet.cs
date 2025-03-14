namespace Mono.WebBrowser.DOM;

public interface IStylesheet
{
	string Type { get; }

	bool Disabled { get; set; }

	INode OwnerNode { get; }

	IStylesheet ParentStyleSheet { get; }

	string Href { get; }

	string Title { get; }

	IMediaList Media { get; }
}

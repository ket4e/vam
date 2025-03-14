using System;
using Mono.WebBrowser.DOM;

namespace Mono.WebBrowser;

public interface IWebBrowser
{
	bool Initialized { get; }

	IWindow Window { get; }

	IDocument Document { get; }

	bool Offline { get; set; }

	INavigation Navigation { get; }

	event NodeEventHandler KeyDown;

	event NodeEventHandler KeyPress;

	event NodeEventHandler KeyUp;

	event NodeEventHandler MouseClick;

	event NodeEventHandler MouseDoubleClick;

	event NodeEventHandler MouseDown;

	event NodeEventHandler MouseEnter;

	event NodeEventHandler MouseLeave;

	event NodeEventHandler MouseMove;

	event NodeEventHandler MouseUp;

	event EventHandler Focus;

	event CreateNewWindowEventHandler CreateNewWindow;

	event AlertEventHandler Alert;

	event LoadStartedEventHandler LoadStarted;

	event LoadCommitedEventHandler LoadCommited;

	event ProgressChangedEventHandler ProgressChanged;

	event LoadFinishedEventHandler LoadFinished;

	event StatusChangedEventHandler StatusChanged;

	event SecurityChangedEventHandler SecurityChanged;

	event ContextMenuEventHandler ContextMenuShown;

	event NavigationRequestedEventHandler NavigationRequested;

	bool Load(IntPtr handle, int width, int height);

	void Shutdown();

	void FocusIn(FocusOption focus);

	void FocusOut();

	void Activate();

	void Deactivate();

	void Resize(int width, int height);

	void Render(byte[] data);

	void Render(string html);

	void Render(string html, string uri, string contentType);

	void ExecuteScript(string script);
}

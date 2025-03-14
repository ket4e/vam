using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class LinkLabelLinkClickedEventArgs : EventArgs
{
	private MouseButtons button;

	private LinkLabel.Link link;

	public MouseButtons Button => button;

	public LinkLabel.Link Link => link;

	public LinkLabelLinkClickedEventArgs(LinkLabel.Link link)
	{
		button = MouseButtons.Left;
		this.link = link;
	}

	public LinkLabelLinkClickedEventArgs(LinkLabel.Link link, MouseButtons button)
	{
		this.button = button;
		this.link = link;
	}
}

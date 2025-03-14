using System.Drawing;

namespace System.Windows.Forms;

internal class TitleButton
{
	public Rectangle Rectangle;

	public ButtonState State;

	public CaptionButton Caption;

	private EventHandler Clicked;

	public bool Visible;

	private bool entered;

	public bool Entered
	{
		get
		{
			return entered;
		}
		set
		{
			entered = value;
		}
	}

	public TitleButton(CaptionButton caption, EventHandler clicked)
	{
		Caption = caption;
		Clicked = clicked;
	}

	public void OnClick()
	{
		if (Clicked != null)
		{
			Clicked(this, EventArgs.Empty);
		}
	}
}

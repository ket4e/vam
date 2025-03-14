using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public sealed class HtmlElementEventArgs : EventArgs
{
	private bool alt_key_pressed;

	private bool bubble_event;

	private Point client_mouse_position;

	private bool ctrl_key_pressed;

	private string event_type;

	private HtmlElement from_element;

	private int key_pressed_code;

	private MouseButtons mouse_buttons_pressed;

	private Point mouse_position;

	private Point offset_mouse_position;

	private bool return_value;

	private bool shift_key_pressed;

	private HtmlElement to_element;

	public bool AltKeyPressed => alt_key_pressed;

	public bool BubbleEvent
	{
		get
		{
			return bubble_event;
		}
		set
		{
			bubble_event = value;
		}
	}

	public Point ClientMousePosition => client_mouse_position;

	public bool CtrlKeyPressed => ctrl_key_pressed;

	public string EventType => event_type;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public HtmlElement FromElement => from_element;

	public int KeyPressedCode => key_pressed_code;

	public MouseButtons MouseButtonsPressed => mouse_buttons_pressed;

	public Point MousePosition => mouse_position;

	public Point OffsetMousePosition => offset_mouse_position;

	public bool ReturnValue
	{
		get
		{
			return return_value;
		}
		set
		{
			return_value = value;
		}
	}

	public bool ShiftKeyPressed => shift_key_pressed;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public HtmlElement ToElement => to_element;

	internal HtmlElementEventArgs()
	{
		alt_key_pressed = false;
		bubble_event = false;
		client_mouse_position = Point.Empty;
		ctrl_key_pressed = false;
		event_type = null;
		from_element = null;
		key_pressed_code = 0;
		mouse_buttons_pressed = MouseButtons.None;
		mouse_position = Point.Empty;
		offset_mouse_position = Point.Empty;
		return_value = false;
		shift_key_pressed = false;
		to_element = null;
	}
}

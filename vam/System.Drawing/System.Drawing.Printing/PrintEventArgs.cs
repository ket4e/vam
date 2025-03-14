using System.ComponentModel;

namespace System.Drawing.Printing;

public class PrintEventArgs : CancelEventArgs
{
	private GraphicsPrinter graphics_context;

	internal PrintAction action;

	public PrintAction PrintAction => action;

	internal GraphicsPrinter GraphicsContext
	{
		get
		{
			return graphics_context;
		}
		set
		{
			graphics_context = value;
		}
	}
}

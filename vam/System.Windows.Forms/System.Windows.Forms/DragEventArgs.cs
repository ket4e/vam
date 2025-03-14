using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class DragEventArgs : EventArgs
{
	internal int x;

	internal int y;

	internal int keystate;

	internal DragDropEffects allowed_effect;

	internal DragDropEffects current_effect;

	internal IDataObject data_object;

	public DragDropEffects AllowedEffect => allowed_effect;

	public IDataObject Data => data_object;

	public DragDropEffects Effect
	{
		get
		{
			return current_effect;
		}
		set
		{
			current_effect = value;
		}
	}

	public int KeyState => keystate;

	public int X => x;

	public int Y => y;

	public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
	{
		this.x = x;
		this.y = y;
		keystate = keyState;
		allowed_effect = allowedEffect;
		current_effect = effect;
		data_object = data;
	}
}

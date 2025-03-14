namespace System.Windows.Forms;

public class PreviewKeyDownEventArgs : EventArgs
{
	private Keys key_data;

	private bool is_input_key;

	public bool Alt => (key_data & Keys.Alt) != 0;

	public bool Control => (key_data & Keys.Control) != 0;

	public bool IsInputKey
	{
		get
		{
			return is_input_key;
		}
		set
		{
			is_input_key = value;
		}
	}

	public Keys KeyCode => key_data & Keys.KeyCode;

	public Keys KeyData => key_data;

	public int KeyValue => Convert.ToInt32(key_data);

	public Keys Modifiers => key_data & Keys.Modifiers;

	public bool Shift => (key_data & Keys.Shift) != 0;

	public PreviewKeyDownEventArgs(Keys keyData)
	{
		key_data = keyData;
	}
}

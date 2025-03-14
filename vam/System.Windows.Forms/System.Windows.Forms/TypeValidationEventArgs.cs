namespace System.Windows.Forms;

public class TypeValidationEventArgs : EventArgs
{
	private bool cancel;

	private bool is_valid_input;

	private string message;

	private object return_value;

	private Type validating_type;

	public bool Cancel
	{
		get
		{
			return cancel;
		}
		set
		{
			cancel = value;
		}
	}

	public bool IsValidInput => is_valid_input;

	public string Message => message;

	public object ReturnValue => return_value;

	public Type ValidatingType => validating_type;

	public TypeValidationEventArgs(Type validatingType, bool isValidInput, object returnValue, string message)
	{
		is_valid_input = isValidInput;
		this.message = message;
		return_value = returnValue;
		validating_type = validatingType;
		cancel = false;
	}
}

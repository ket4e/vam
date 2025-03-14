using System.ComponentModel;

namespace System.Windows.Forms;

public class BindingCompleteEventArgs : CancelEventArgs
{
	private Binding binding;

	private BindingCompleteState state;

	private BindingCompleteContext context;

	private string error_text;

	private Exception exception;

	public Binding Binding => binding;

	public BindingCompleteContext BindingCompleteContext => context;

	public BindingCompleteState BindingCompleteState => state;

	public string ErrorText => error_text;

	public Exception Exception => exception;

	public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context)
		: this(binding, state, context, string.Empty, null, cancel: false)
	{
	}

	public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText)
		: this(binding, state, context, errorText, null, cancel: false)
	{
	}

	public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception)
		: this(binding, state, context, errorText, exception, cancel: false)
	{
	}

	public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception, bool cancel)
		: base(cancel)
	{
		this.binding = binding;
		this.state = state;
		this.context = context;
		error_text = errorText;
		this.exception = exception;
	}

	internal void SetErrorText(string error_text)
	{
		this.error_text = error_text;
	}

	internal void SetException(Exception exception)
	{
		this.exception = exception;
	}
}

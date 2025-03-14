namespace UnityEngine.Experimental.UIElements;

internal class TouchScreenTextEditorEventHandler : TextEditorEventHandler
{
	private string m_SecureText;

	public string secureText
	{
		get
		{
			return m_SecureText;
		}
		set
		{
			string text = value ?? string.Empty;
			if (text != m_SecureText)
			{
				m_SecureText = text;
			}
		}
	}

	public TouchScreenTextEditorEventHandler(TextEditorEngine editorEngine, TextInputFieldBase textInputField)
		: base(editorEngine, textInputField)
	{
		secureText = string.Empty;
	}

	public override void ExecuteDefaultActionAtTarget(EventBase evt)
	{
		base.ExecuteDefaultActionAtTarget(evt);
		long num = EventBase<MouseDownEvent>.TypeId();
		if (evt.GetEventTypeId() == num)
		{
			base.textInputField.SyncTextEngine();
			base.textInputField.UpdateText(base.editorEngine.text);
			base.textInputField.TakeMouseCapture();
			base.editorEngine.keyboardOnScreen = TouchScreenKeyboard.Open(string.IsNullOrEmpty(secureText) ? base.textInputField.text : secureText, TouchScreenKeyboardType.Default, autocorrection: true, base.editorEngine.multiline, !string.IsNullOrEmpty(secureText));
			base.editorEngine.UpdateScrollOffset();
			evt.StopPropagation();
		}
	}
}

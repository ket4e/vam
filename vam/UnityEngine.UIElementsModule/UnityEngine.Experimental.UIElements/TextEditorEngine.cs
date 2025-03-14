namespace UnityEngine.Experimental.UIElements;

internal class TextEditorEngine : TextEditor
{
	private TextInputFieldBase textInputField { get; set; }

	internal override Rect localPosition => new Rect(0f, 0f, base.position.width, base.position.height);

	public TextEditorEngine(TextInputFieldBase field)
	{
		textInputField = field;
	}

	internal override void OnDetectFocusChange()
	{
		if (m_HasFocus && !textInputField.hasFocus)
		{
			OnFocus();
		}
		if (!m_HasFocus && textInputField.hasFocus)
		{
			OnLostFocus();
		}
	}

	internal override void OnCursorIndexChange()
	{
		textInputField.Dirty(ChangeType.Repaint);
	}

	internal override void OnSelectIndexChange()
	{
		textInputField.Dirty(ChangeType.Repaint);
	}
}

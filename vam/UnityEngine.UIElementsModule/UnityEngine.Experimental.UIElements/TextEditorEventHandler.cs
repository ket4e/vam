namespace UnityEngine.Experimental.UIElements;

internal class TextEditorEventHandler
{
	protected TextEditorEngine editorEngine { get; private set; }

	protected TextInputFieldBase textInputField { get; private set; }

	protected TextEditorEventHandler(TextEditorEngine editorEngine, TextInputFieldBase textInputField)
	{
		this.editorEngine = editorEngine;
		this.textInputField = textInputField;
		this.textInputField.SyncTextEngine();
	}

	public virtual void ExecuteDefaultActionAtTarget(EventBase evt)
	{
	}

	public virtual void ExecuteDefaultAction(EventBase evt)
	{
		if (evt.GetEventTypeId() == EventBase<FocusEvent>.TypeId())
		{
			editorEngine.OnFocus();
		}
		else if (evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
		{
			editorEngine.OnLostFocus();
			editorEngine.SelectNone();
		}
	}
}

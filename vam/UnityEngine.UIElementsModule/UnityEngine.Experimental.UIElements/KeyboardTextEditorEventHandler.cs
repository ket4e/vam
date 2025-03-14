using System;

namespace UnityEngine.Experimental.UIElements;

internal class KeyboardTextEditorEventHandler : TextEditorEventHandler
{
	internal bool m_Changed;

	private bool m_Dragged;

	private bool m_DragToPosition = true;

	private bool m_PostponeMove;

	private bool m_SelectAllOnMouseUp = true;

	private string m_PreDrawCursorText;

	public KeyboardTextEditorEventHandler(TextEditorEngine editorEngine, TextInputFieldBase textInputField)
		: base(editorEngine, textInputField)
	{
	}

	public override void ExecuteDefaultActionAtTarget(EventBase evt)
	{
		base.ExecuteDefaultActionAtTarget(evt);
		if (evt.GetEventTypeId() == EventBase<MouseDownEvent>.TypeId())
		{
			OnMouseDown(evt as MouseDownEvent);
		}
		else if (evt.GetEventTypeId() == EventBase<MouseUpEvent>.TypeId())
		{
			OnMouseUp(evt as MouseUpEvent);
		}
		else if (evt.GetEventTypeId() == EventBase<MouseMoveEvent>.TypeId())
		{
			OnMouseMove(evt as MouseMoveEvent);
		}
		else if (evt.GetEventTypeId() == EventBase<KeyDownEvent>.TypeId())
		{
			OnKeyDown(evt as KeyDownEvent);
		}
		else if (evt.GetEventTypeId() == EventBase<IMGUIEvent>.TypeId())
		{
			OnIMGUIEvent(evt as IMGUIEvent);
		}
	}

	private void OnMouseDown(MouseDownEvent evt)
	{
		base.textInputField.SyncTextEngine();
		m_Changed = false;
		if (!base.textInputField.hasFocus)
		{
			base.editorEngine.m_HasFocus = true;
			base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, evt.button == 0 && evt.shiftKey);
			if (evt.button == 0)
			{
				base.textInputField.TakeMouseCapture();
			}
			evt.StopPropagation();
		}
		else if (evt.button == 0)
		{
			if (evt.clickCount == 2 && base.textInputField.doubleClickSelectsWord)
			{
				base.editorEngine.SelectCurrentWord();
				base.editorEngine.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
				base.editorEngine.MouseDragSelectsWholeWords(on: true);
				m_DragToPosition = false;
			}
			else if (evt.clickCount == 3 && base.textInputField.tripleClickSelectsLine)
			{
				base.editorEngine.SelectCurrentParagraph();
				base.editorEngine.MouseDragSelectsWholeWords(on: true);
				base.editorEngine.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
				m_DragToPosition = false;
			}
			else
			{
				base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, evt.shiftKey);
				m_SelectAllOnMouseUp = false;
			}
			base.textInputField.TakeMouseCapture();
			evt.StopPropagation();
		}
		else if (evt.button == 1)
		{
			if (base.editorEngine.cursorIndex == base.editorEngine.selectIndex)
			{
				base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, shift: false);
			}
			m_SelectAllOnMouseUp = false;
			m_DragToPosition = false;
		}
		base.editorEngine.UpdateScrollOffset();
	}

	private void OnMouseUp(MouseUpEvent evt)
	{
		if (evt.button == 0 && base.textInputField.HasMouseCapture())
		{
			base.textInputField.SyncTextEngine();
			m_Changed = false;
			if (m_Dragged && m_DragToPosition)
			{
				base.editorEngine.MoveSelectionToAltCursor();
			}
			else if (m_PostponeMove)
			{
				base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, evt.shiftKey);
			}
			else if (m_SelectAllOnMouseUp)
			{
				m_SelectAllOnMouseUp = false;
			}
			base.editorEngine.MouseDragSelectsWholeWords(on: false);
			base.textInputField.ReleaseMouseCapture();
			m_DragToPosition = true;
			m_Dragged = false;
			m_PostponeMove = false;
			evt.StopPropagation();
			base.editorEngine.UpdateScrollOffset();
		}
	}

	private void OnMouseMove(MouseMoveEvent evt)
	{
		if (evt.button != 0 || !base.textInputField.HasMouseCapture())
		{
			return;
		}
		base.textInputField.SyncTextEngine();
		m_Changed = false;
		if (!evt.shiftKey && base.editorEngine.hasSelection && m_DragToPosition)
		{
			base.editorEngine.MoveAltCursorToPosition(evt.localMousePosition);
		}
		else
		{
			if (evt.shiftKey)
			{
				base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, evt.shiftKey);
			}
			else
			{
				base.editorEngine.SelectToPosition(evt.localMousePosition);
			}
			m_DragToPosition = false;
			m_SelectAllOnMouseUp = !base.editorEngine.hasSelection;
		}
		m_Dragged = true;
		evt.StopPropagation();
		base.editorEngine.UpdateScrollOffset();
	}

	private void OnKeyDown(KeyDownEvent evt)
	{
		if (!base.textInputField.hasFocus)
		{
			return;
		}
		base.textInputField.SyncTextEngine();
		m_Changed = false;
		if (base.editorEngine.HandleKeyEvent(evt.imguiEvent))
		{
			if (base.textInputField.text != base.editorEngine.text)
			{
				m_Changed = true;
			}
			evt.StopPropagation();
		}
		else
		{
			if (evt.keyCode == KeyCode.Tab || evt.character == '\t')
			{
				return;
			}
			evt.StopPropagation();
			char character = evt.character;
			if ((character == '\n' && !base.editorEngine.multiline && !evt.altKey) || !base.textInputField.AcceptCharacter(character))
			{
				return;
			}
			Font font = base.editorEngine.style.font;
			if ((font != null && font.HasCharacter(character)) || character == '\n')
			{
				base.editorEngine.Insert(character);
				m_Changed = true;
			}
			else if (character == '\0' && !string.IsNullOrEmpty(Input.compositionString))
			{
				base.editorEngine.ReplaceSelection("");
				m_Changed = true;
			}
		}
		if (m_Changed)
		{
			base.editorEngine.text = base.textInputField.CullString(base.editorEngine.text);
			base.textInputField.UpdateText(base.editorEngine.text);
		}
		base.editorEngine.UpdateScrollOffset();
	}

	private void OnIMGUIEvent(IMGUIEvent evt)
	{
		if (!base.textInputField.hasFocus)
		{
			return;
		}
		base.textInputField.SyncTextEngine();
		m_Changed = false;
		switch (evt.imguiEvent.type)
		{
		case EventType.ValidateCommand:
			switch (evt.imguiEvent.commandName)
			{
			case "Cut":
			case "Copy":
				if (!base.editorEngine.hasSelection)
				{
					return;
				}
				break;
			case "Paste":
				if (!base.editorEngine.CanPaste())
				{
					return;
				}
				break;
			}
			evt.StopPropagation();
			break;
		case EventType.ExecuteCommand:
		{
			bool flag = false;
			string text = base.editorEngine.text;
			if (!base.textInputField.hasFocus)
			{
				return;
			}
			switch (evt.imguiEvent.commandName)
			{
			case "OnLostFocus":
				evt.StopPropagation();
				return;
			case "Cut":
				base.editorEngine.Cut();
				flag = true;
				break;
			case "Copy":
				base.editorEngine.Copy();
				evt.StopPropagation();
				return;
			case "Paste":
				base.editorEngine.Paste();
				flag = true;
				break;
			case "SelectAll":
				base.editorEngine.SelectAll();
				evt.StopPropagation();
				return;
			case "Delete":
				if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
				{
					base.editorEngine.Delete();
				}
				else
				{
					base.editorEngine.Cut();
				}
				flag = true;
				break;
			}
			if (flag)
			{
				if (text != base.editorEngine.text)
				{
					m_Changed = true;
				}
				evt.StopPropagation();
			}
			break;
		}
		}
		if (m_Changed)
		{
			base.editorEngine.text = base.textInputField.CullString(base.editorEngine.text);
			base.textInputField.UpdateText(base.editorEngine.text);
			evt.StopPropagation();
		}
		base.editorEngine.UpdateScrollOffset();
	}

	public void PreDrawCursor(string newText)
	{
		base.textInputField.SyncTextEngine();
		m_PreDrawCursorText = base.editorEngine.text;
		int num = base.editorEngine.cursorIndex;
		if (!string.IsNullOrEmpty(Input.compositionString))
		{
			base.editorEngine.text = newText.Substring(0, base.editorEngine.cursorIndex) + Input.compositionString + newText.Substring(base.editorEngine.selectIndex);
			num += Input.compositionString.Length;
		}
		else
		{
			base.editorEngine.text = newText;
		}
		base.editorEngine.text = base.textInputField.CullString(base.editorEngine.text);
		num = Math.Min(num, base.editorEngine.text.Length);
		base.editorEngine.graphicalCursorPos = base.editorEngine.style.GetCursorPixelPosition(base.editorEngine.localPosition, new GUIContent(base.editorEngine.text), num);
	}

	public void PostDrawCursor()
	{
		base.editorEngine.text = m_PreDrawCursorText;
	}
}

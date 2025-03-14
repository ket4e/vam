using System.Collections;
using System.Text;

namespace System.Windows.Forms;

internal class UndoManager
{
	internal enum ActionType
	{
		Typing,
		InsertString,
		DeleteString,
		UserActionBegin,
		UserActionEnd
	}

	internal class Action
	{
		internal ActionType type;

		internal int line_no;

		internal int pos;

		internal object data;
	}

	private Document document;

	private Stack undo_actions;

	private Stack redo_actions;

	private bool locked;

	internal bool CanUndo => undo_actions.Count > 0;

	internal bool CanRedo => redo_actions.Count > 0;

	internal string UndoActionName
	{
		get
		{
			foreach (Action undo_action in undo_actions)
			{
				if (undo_action.type == ActionType.UserActionBegin)
				{
					return (string)undo_action.data;
				}
				if (undo_action.type == ActionType.Typing)
				{
					return Locale.GetText("Typing");
				}
			}
			return string.Empty;
		}
	}

	internal string RedoActionName
	{
		get
		{
			foreach (Action redo_action in redo_actions)
			{
				if (redo_action.type == ActionType.UserActionBegin)
				{
					return (string)redo_action.data;
				}
				if (redo_action.type == ActionType.Typing)
				{
					return Locale.GetText("Typing");
				}
			}
			return string.Empty;
		}
	}

	internal UndoManager(Document document)
	{
		this.document = document;
		undo_actions = new Stack(50);
		redo_actions = new Stack(50);
	}

	internal void Clear()
	{
		undo_actions.Clear();
		redo_actions.Clear();
	}

	internal bool Undo()
	{
		bool flag = false;
		if (undo_actions.Count == 0)
		{
			return false;
		}
		locked = true;
		do
		{
			Action action = (Action)undo_actions.Pop();
			redo_actions.Push(action);
			switch (action.type)
			{
			case ActionType.UserActionBegin:
				flag = true;
				break;
			case ActionType.InsertString:
			{
				Line line = document.GetLine(action.line_no);
				document.SuspendUpdate();
				document.DeleteMultiline(line, action.pos, ((string)action.data).Length + 1);
				document.PositionCaret(line, action.pos);
				document.SetSelectionToCaret(start: true);
				document.ResumeUpdate(immediate_update: true);
				break;
			}
			case ActionType.Typing:
			{
				Line line = document.GetLine(action.line_no);
				document.SuspendUpdate();
				document.DeleteMultiline(line, action.pos, ((StringBuilder)action.data).Length);
				document.PositionCaret(line, action.pos);
				document.SetSelectionToCaret(start: true);
				document.ResumeUpdate(immediate_update: true);
				flag = true;
				break;
			}
			case ActionType.DeleteString:
			{
				Line line = document.GetLine(action.line_no);
				document.SuspendUpdate();
				Insert(line, action.pos, (Line)action.data, select: true);
				document.ResumeUpdate(immediate_update: true);
				break;
			}
			}
		}
		while (!flag && undo_actions.Count > 0);
		locked = false;
		return true;
	}

	internal bool Redo()
	{
		bool flag = false;
		if (redo_actions.Count == 0)
		{
			return false;
		}
		locked = true;
		do
		{
			Action action = (Action)redo_actions.Pop();
			undo_actions.Push(action);
			switch (action.type)
			{
			case ActionType.UserActionEnd:
				flag = true;
				break;
			case ActionType.InsertString:
			{
				Line line = document.GetLine(action.line_no);
				document.SuspendUpdate();
				int num = document.LineTagToCharIndex(line, action.pos);
				document.InsertString(line, action.pos, (string)action.data);
				document.CharIndexToLineTag(num + ((string)action.data).Length, out document.caret.line, out document.caret.tag, out document.caret.pos);
				document.UpdateCaret();
				document.SetSelectionToCaret(start: true);
				document.ResumeUpdate(immediate_update: true);
				break;
			}
			case ActionType.Typing:
			{
				Line line = document.GetLine(action.line_no);
				document.SuspendUpdate();
				int num = document.LineTagToCharIndex(line, action.pos);
				document.InsertString(line, action.pos, ((StringBuilder)action.data).ToString());
				document.CharIndexToLineTag(num + ((StringBuilder)action.data).Length, out document.caret.line, out document.caret.tag, out document.caret.pos);
				document.UpdateCaret();
				document.SetSelectionToCaret(start: true);
				document.ResumeUpdate(immediate_update: true);
				flag = true;
				break;
			}
			case ActionType.DeleteString:
			{
				Line line = document.GetLine(action.line_no);
				document.SuspendUpdate();
				document.DeleteMultiline(line, action.pos, ((Line)action.data).text.Length);
				document.PositionCaret(line, action.pos);
				document.SetSelectionToCaret(start: true);
				document.ResumeUpdate(immediate_update: true);
				break;
			}
			}
		}
		while (!flag && redo_actions.Count > 0);
		locked = false;
		return true;
	}

	public void BeginUserAction(string name)
	{
		if (!locked)
		{
			redo_actions.Clear();
			Action action = new Action();
			action.type = ActionType.UserActionBegin;
			action.data = name;
			undo_actions.Push(action);
		}
	}

	public void EndUserAction()
	{
		if (!locked)
		{
			Action action = new Action();
			action.type = ActionType.UserActionEnd;
			undo_actions.Push(action);
		}
	}

	public void RecordDeleteString(Line start_line, int start_pos, Line end_line, int end_pos)
	{
		if (!locked)
		{
			redo_actions.Clear();
			Action action = new Action();
			action.type = ActionType.DeleteString;
			action.line_no = start_line.line_no;
			action.pos = start_pos;
			action.data = Duplicate(start_line, start_pos, end_line, end_pos);
			undo_actions.Push(action);
		}
	}

	public void RecordInsertString(Line line, int pos, string str)
	{
		if (!locked && str.Length != 0)
		{
			redo_actions.Clear();
			Action action = new Action();
			action.type = ActionType.InsertString;
			action.data = str;
			action.line_no = line.line_no;
			action.pos = pos;
			undo_actions.Push(action);
		}
	}

	public void RecordTyping(Line line, int pos, char ch)
	{
		if (!locked)
		{
			redo_actions.Clear();
			Action action = null;
			if (undo_actions.Count > 0)
			{
				action = (Action)undo_actions.Peek();
			}
			if (action == null || action.type != 0)
			{
				action = new Action();
				action.type = ActionType.Typing;
				action.data = new StringBuilder();
				action.line_no = line.line_no;
				action.pos = pos;
				undo_actions.Push(action);
			}
			StringBuilder stringBuilder = (StringBuilder)action.data;
			stringBuilder.Append(ch);
		}
	}

	public Line Duplicate(Line start_line, int start_pos, Line end_line, int end_pos)
	{
		Line line = new Line(start_line.document, start_line.ending);
		Line result = line;
		for (int i = start_line.line_no; i <= end_line.line_no; i++)
		{
			Line line2 = document.GetLine(i);
			int num = ((start_line.line_no == i) ? start_pos : 0);
			int num2 = ((end_line.line_no != i) ? line2.text.Length : end_pos);
			if (end_pos == 0)
			{
				continue;
			}
			line.text = new StringBuilder(line2.text.ToString(num, num2 - num));
			LineTag lineTag = line2.FindTag(num + 1);
			while (lineTag != null && lineTag.Start <= num2)
			{
				int num3 = ((lineTag.Start > num || num >= lineTag.Start + lineTag.Length) ? lineTag.Start : num);
				LineTag lineTag2 = new LineTag(line, num3 - num + 1);
				lineTag2.CopyFormattingFrom(lineTag);
				lineTag = lineTag.Next;
				if (line.tags == null)
				{
					line.tags = lineTag2;
					continue;
				}
				LineTag lineTag3 = line.tags;
				while (lineTag3.Next != null)
				{
					lineTag3 = lineTag3.Next;
				}
				lineTag3.Next = lineTag2;
				lineTag2.Previous = lineTag3;
			}
			if (i + 1 <= end_line.line_no)
			{
				line.ending = line2.ending;
				line.right = new Line(start_line.document, start_line.ending);
				line.right.left = line;
				line = line.right;
			}
		}
		return result;
	}

	internal void Insert(Line line, int pos, Line insert, bool select)
	{
		if (insert.right == null)
		{
			document.Split(line, pos);
			if (insert.tags != null)
			{
				LineTag lineTag = line.tags;
				while (lineTag.Next != null)
				{
					lineTag = lineTag.Next;
				}
				int num = lineTag.Start + lineTag.Length - 1;
				lineTag.Next = insert.tags;
				line.text.Insert(num, insert.text.ToString());
				for (lineTag = lineTag.Next; lineTag != null; lineTag = lineTag.Next)
				{
					lineTag.Start += num;
					lineTag.Line = line;
				}
				document.Combine(line.line_no, line.line_no + 1);
				if (select)
				{
					document.SetSelectionStart(line, pos, invalidate: false);
					document.SetSelectionEnd(line, pos + insert.text.Length, invalidate: false);
				}
				document.UpdateView(line, pos);
			}
			return;
		}
		Line line2 = line;
		int num2 = 1;
		Line line3 = insert;
		while (line3 != null)
		{
			int num;
			LineTag lineTag;
			if (line3 == insert)
			{
				document.Split(line.line_no, pos);
				lineTag = line.tags;
				if (lineTag != null && lineTag.Length != 0)
				{
					while (lineTag.Next != null)
					{
						lineTag = lineTag.Next;
					}
					num = lineTag.Start + lineTag.Length - 1;
					lineTag.Next = line3.tags;
					lineTag.Next.Previous = lineTag;
					lineTag = lineTag.Next;
				}
				else
				{
					num = 0;
					line.tags = line3.tags;
					line.tags.Previous = null;
					lineTag = line.tags;
				}
				line.ending = line3.ending;
			}
			else
			{
				document.Split(line.line_no, 0);
				num = 0;
				line.tags = line3.tags;
				line.tags.Previous = null;
				line.ending = line3.ending;
				lineTag = line.tags;
			}
			while (lineTag != null)
			{
				lineTag.Start += num - 1;
				lineTag.Line = line;
				lineTag = lineTag.Next;
			}
			line.text.Insert(num, line3.text.ToString());
			line.Grow(line.text.Length);
			line.recalc = true;
			line = document.GetLine(line.line_no + 1);
			if (line3.right == null && line3.tags.Length != 0)
			{
				document.Combine(line.line_no - 1, line.line_no);
			}
			line3 = line3.right;
			num2++;
		}
		document.UpdateView(line2, num2, pos);
	}
}

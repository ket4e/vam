using System.Drawing;
using System.Text;

namespace System.Windows.Forms;

internal class Line : ICloneable, IComparable
{
	internal Document document;

	internal StringBuilder text;

	internal float[] widths;

	internal int space;

	internal int line_no;

	internal LineTag tags;

	internal int offset;

	internal int height;

	internal int ascent;

	internal HorizontalAlignment alignment;

	internal int align_shift;

	internal int indent;

	internal int hanging_indent;

	internal int right_indent;

	internal LineEnding ending;

	internal Line parent;

	internal Line left;

	internal Line right;

	internal LineColor color;

	private static int DEFAULT_TEXT_LEN;

	internal bool recalc;

	internal HorizontalAlignment Alignment
	{
		get
		{
			return alignment;
		}
		set
		{
			if (alignment != value)
			{
				alignment = value;
				recalc = true;
			}
		}
	}

	internal int HangingIndent
	{
		get
		{
			return hanging_indent;
		}
		set
		{
			hanging_indent = value;
			recalc = true;
		}
	}

	internal int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	internal int Indent
	{
		get
		{
			return indent;
		}
		set
		{
			indent = value;
			recalc = true;
		}
	}

	internal int LineNo
	{
		get
		{
			return line_no;
		}
		set
		{
			line_no = value;
		}
	}

	internal int RightIndent
	{
		get
		{
			return right_indent;
		}
		set
		{
			right_indent = value;
			recalc = true;
		}
	}

	internal int Width => (int)widths[text.Length];

	internal string Text
	{
		get
		{
			return text.ToString();
		}
		set
		{
			int length = text.Length;
			text = new StringBuilder(value, (value.Length <= DEFAULT_TEXT_LEN) ? DEFAULT_TEXT_LEN : (value.Length + 1));
			if (text.Length > length)
			{
				Grow(text.Length - length);
			}
		}
	}

	internal int X
	{
		get
		{
			if (document.multiline)
			{
				return align_shift;
			}
			return offset + align_shift;
		}
	}

	internal int Y
	{
		get
		{
			if (!document.multiline)
			{
				return document.top_margin;
			}
			return document.top_margin + offset;
		}
	}

	internal Line(Document document, LineEnding ending)
	{
		this.document = document;
		color = LineColor.Red;
		left = null;
		right = null;
		parent = null;
		text = null;
		recalc = true;
		alignment = document.alignment;
		this.ending = ending;
	}

	internal Line(Document document, int LineNo, string Text, Font font, Color color, LineEnding ending)
		: this(document, ending)
	{
		space = ((Text.Length <= DEFAULT_TEXT_LEN) ? DEFAULT_TEXT_LEN : (Text.Length + 1));
		text = new StringBuilder(Text, space);
		line_no = LineNo;
		this.ending = ending;
		widths = new float[space + 1];
		tags = new LineTag(this, 1);
		tags.Font = font;
		tags.Color = color;
	}

	internal Line(Document document, int LineNo, string Text, HorizontalAlignment align, Font font, Color color, LineEnding ending)
		: this(document, ending)
	{
		space = ((Text.Length <= DEFAULT_TEXT_LEN) ? DEFAULT_TEXT_LEN : (Text.Length + 1));
		text = new StringBuilder(Text, space);
		line_no = LineNo;
		this.ending = ending;
		alignment = align;
		widths = new float[space + 1];
		tags = new LineTag(this, 1);
		tags.Font = font;
		tags.Color = color;
	}

	internal Line(Document document, int LineNo, string Text, LineTag tag, LineEnding ending)
		: this(document, ending)
	{
		space = ((Text.Length <= DEFAULT_TEXT_LEN) ? DEFAULT_TEXT_LEN : (Text.Length + 1));
		text = new StringBuilder(Text, space);
		this.ending = ending;
		line_no = LineNo;
		widths = new float[space + 1];
		tags = tag;
	}

	internal void LinkRecord(StringBuilder linkRecord)
	{
		for (LineTag next = tags; next != null; next = next.Next)
		{
			if (next.IsLink)
			{
				linkRecord.Append("L");
			}
			else
			{
				linkRecord.Append("N");
			}
		}
	}

	internal void ClearLinks()
	{
		for (LineTag next = tags; next != null; next = next.Next)
		{
			next.IsLink = false;
		}
	}

	public void DeleteCharacters(int pos, int count)
	{
		bool flag = false;
		if (pos >= text.Length)
		{
			return;
		}
		LineTag lineTag = FindTag(pos + 1);
		text.Remove(pos, count);
		if (lineTag == null)
		{
			return;
		}
		if (pos + count > lineTag.Start + lineTag.Length - 1)
		{
			flag = true;
			int num = count;
			num -= lineTag.Start + lineTag.Length - pos - 1;
			lineTag = lineTag.Next;
			while (lineTag != null && num > 0)
			{
				int length = lineTag.Length;
				lineTag.Start -= count - num;
				if (length > num)
				{
					num = 0;
					continue;
				}
				num -= length;
				lineTag = lineTag.Next;
			}
		}
		else if (lineTag.Length == 0)
		{
			flag = true;
		}
		LineTag lineTag2 = lineTag;
		while (lineTag2 != null && lineTag2.Next != null && lineTag2.Next.Length == 0)
		{
			LineTag previous = lineTag2;
			lineTag2.Next = lineTag2.Next.Next;
			if (lineTag2.Next != null)
			{
				lineTag2.Next.Previous = previous;
			}
			lineTag2 = lineTag2.Next;
		}
		if (lineTag != null)
		{
			for (lineTag = lineTag.Next; lineTag != null; lineTag = lineTag.Next)
			{
				lineTag.Start -= count;
			}
		}
		recalc = true;
		if (flag)
		{
			Streamline(document.Lines);
		}
	}

	internal void DrawEnding(Graphics dc, float y)
	{
		if (!document.multiline)
		{
			LineTag next = tags;
			while (next.Next != null)
			{
				next = next.Next;
			}
			string text = null;
			switch (document.LineEndingLength(ending))
			{
			case 0:
				return;
			case 1:
				text = "\u0013";
				break;
			case 2:
				text = "\u0013\u0013";
				break;
			case 3:
				text = "\u0013\u0013\u0013";
				break;
			}
			TextBoxTextRenderer.DrawText(dc, text, next.Font, next.Color, (float)X + widths[TextLengthWithoutEnding()] - (float)document.viewport_x + (float)document.OffsetX, y, showNonPrint: true);
		}
	}

	internal LineTag FindTag(int pos)
	{
		if (pos == 0)
		{
			return tags;
		}
		LineTag next = tags;
		if (pos >= text.Length)
		{
			pos = text.Length - 1;
		}
		while (next != null)
		{
			if (next.Start - 1 <= pos && pos <= next.Start + next.Length - 1)
			{
				return LineTag.GetFinalTag(next);
			}
			next = next.Next;
		}
		return null;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public LineTag GetTag(int x)
	{
		LineTag next = tags;
		if ((float)x < next.X)
		{
			return LineTag.GetFinalTag(next);
		}
		while (true)
		{
			if ((float)x >= next.X && (float)x < next.X + next.Width)
			{
				return next;
			}
			if (next.Next == null)
			{
				break;
			}
			next = next.Next;
		}
		return LineTag.GetFinalTag(next);
	}

	internal void Grow(int minimum)
	{
		int length = text.Length;
		if (length + minimum > space)
		{
			float[] array;
			if (length + minimum > space * 2)
			{
				array = new float[length + minimum * 2 + 1];
				space = length + minimum * 2;
			}
			else
			{
				array = new float[space * 2 + 1];
				space *= 2;
			}
			widths.CopyTo(array, 0);
			widths = array;
		}
	}

	public void InsertString(int pos, string s)
	{
		InsertString(pos, s, FindTag(pos));
	}

	public void InsertString(int pos, string s, LineTag tag)
	{
		int length = s.Length;
		text.Insert(pos, s);
		for (tag = tag.Next; tag != null; tag = tag.Next)
		{
			tag.Start += length;
		}
		Grow(length);
		recalc = true;
	}

	internal bool RecalculateLine(Graphics g, Document doc)
	{
		int num = 0;
		int length = text.Length;
		LineTag next = tags;
		int num2 = offset;
		int num3 = height;
		int num4 = ascent;
		height = 0;
		ascent = 0;
		next.Shift = 0;
		if (ending == LineEnding.Wrap)
		{
			widths[0] = document.left_margin + hanging_indent;
		}
		else
		{
			widths[0] = document.left_margin + indent;
		}
		recalc = false;
		bool result = false;
		bool flag = false;
		int num5 = 0;
		while (num < length)
		{
			while (next.Length == 0)
			{
				next.Shift = (next.Line.ascent - next.Ascent) / 72;
				next = next.Next;
			}
			float width = next.SizeOfPosition(g, num).Width;
			if (char.IsWhiteSpace(text[num]))
			{
				num5 = num + 1;
			}
			if (doc.wrap)
			{
				if (num5 > 0 && num5 != length && widths[num] + width + 5f > (float)(doc.viewport_width - right_indent))
				{
					widths[num + 1] = widths[num] + width;
					num = num5;
					length = text.Length;
					doc.Split(this, next, num);
					ending = LineEnding.Wrap;
					length = text.Length;
					result = true;
					flag = true;
				}
				else if (num > 1 && widths[num] + width > (float)(doc.viewport_width - right_indent))
				{
					widths[num + 1] = widths[num] + width;
					doc.Split(this, next, num);
					ending = LineEnding.Wrap;
					length = text.Length;
					result = true;
					flag = true;
				}
			}
			if (!flag)
			{
				num++;
				widths[num] = widths[num - 1] + width;
				if (num == length)
				{
					Line line = doc.GetLine(line_no + 1);
					if (line != null && (ending == LineEnding.Wrap || ending == LineEnding.None))
					{
						doc.Combine(line_no, line_no + 1);
						length = text.Length;
						result = true;
					}
				}
			}
			if (num != next.Start - 1 + next.Length)
			{
				continue;
			}
			next.Height = next.MaxHeight();
			if (next.Height > height)
			{
				height = next.Height;
			}
			if (next.Ascent > ascent)
			{
				LineTag next2 = tags;
				while (next2 != null && next2 != next)
				{
					next2.Shift = (next.Ascent - next2.Ascent) / 72;
					next2 = next2.Next;
				}
				ascent = next.Ascent;
			}
			else
			{
				next.Shift = (ascent - next.Ascent) / 72;
			}
			next = next.Next;
			if (next != null)
			{
				next.Shift = 0;
				num5 = num;
			}
		}
		while (next != null)
		{
			next.Shift = (next.Line.ascent - next.Ascent) / 72;
			next = next.Next;
		}
		if (height == 0)
		{
			height = tags.Font.Height;
			tags.Height = height;
			tags.Shift = 0;
		}
		if (num2 != offset || num3 != height || num4 != ascent)
		{
			result = true;
		}
		return result;
	}

	internal bool RecalculatePasswordLine(Graphics g, Document doc)
	{
		int num = 0;
		int length = text.Length;
		LineTag lineTag = tags;
		ascent = 0;
		lineTag.Shift = 0;
		recalc = false;
		widths[0] = document.left_margin + indent;
		float width = TextBoxTextRenderer.MeasureText(g, doc.password_char, tags.Font).Width;
		bool result = ((height != lineTag.Font.Height) ? true : false);
		height = lineTag.Font.Height;
		lineTag.Height = height;
		ascent = lineTag.Ascent;
		while (num < length)
		{
			num++;
			widths[num] = widths[num - 1] + width;
		}
		return result;
	}

	internal void Streamline(int lines)
	{
		LineTag lineTag = tags;
		LineTag next = lineTag.Next;
		while (lineTag.Length == 0 && next != null && next.IsTextTag)
		{
			tags = next;
			tags.Previous = null;
			lineTag = next;
			next = lineTag.Next;
		}
		if (next == null)
		{
			return;
		}
		while (next != null)
		{
			if (lineTag.IsTextTag && next.Length == 0 && next.IsTextTag && (next.Next != null || line_no != lines))
			{
				lineTag.Next = next.Next;
				if (lineTag.Next != null)
				{
					lineTag.Next.Previous = lineTag;
				}
				next = lineTag.Next;
			}
			else if (lineTag.Combine(next))
			{
				next = lineTag.Next;
			}
			else
			{
				lineTag = lineTag.Next;
				next = lineTag.Next;
			}
		}
	}

	internal int TextLengthWithoutEnding()
	{
		return text.Length - document.LineEndingLength(ending);
	}

	internal string TextWithoutEnding()
	{
		return text.ToString(0, text.Length - document.LineEndingLength(ending));
	}

	public object Clone()
	{
		Line line = new Line(document, ending);
		line.text = text;
		if (left != null)
		{
			line.left = (Line)left.Clone();
		}
		if (left != null)
		{
			line.left = (Line)left.Clone();
		}
		return line;
	}

	internal object CloneLine()
	{
		Line line = new Line(document, ending);
		line.text = text;
		return line;
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (!(obj is Line))
		{
			throw new ArgumentException("Object is not of type Line", "obj");
		}
		if (line_no < ((Line)obj).line_no)
		{
			return -1;
		}
		if (line_no > ((Line)obj).line_no)
		{
			return 1;
		}
		return 0;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is Line))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		if (line_no == ((Line)obj).line_no)
		{
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return $"Line {line_no}";
	}
}

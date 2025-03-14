using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms.RTF;

namespace System.Windows.Forms;

internal class Document : IEnumerable, ICloneable
{
	internal struct Marker
	{
		internal Line line;

		internal LineTag tag;

		internal int pos;

		internal int height;

		public void Combine(Line move_to_line, int move_to_line_length)
		{
			line = move_to_line;
			pos += move_to_line_length;
			tag = LineTag.FindTag(line, pos);
		}

		public void Split(Line move_to_line, int split_at)
		{
			line = move_to_line;
			pos -= split_at;
			tag = LineTag.FindTag(line, pos);
		}

		public override bool Equals(object obj)
		{
			return this == (Marker)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Concat("Marker Line ", line, ", Position ", pos);
		}

		public static bool operator <(Marker lhs, Marker rhs)
		{
			if (lhs.line.line_no < rhs.line.line_no)
			{
				return true;
			}
			if (lhs.line.line_no == rhs.line.line_no && lhs.pos < rhs.pos)
			{
				return true;
			}
			return false;
		}

		public static bool operator >(Marker lhs, Marker rhs)
		{
			if (lhs.line.line_no > rhs.line.line_no)
			{
				return true;
			}
			if (lhs.line.line_no == rhs.line.line_no && lhs.pos > rhs.pos)
			{
				return true;
			}
			return false;
		}

		public static bool operator ==(Marker lhs, Marker rhs)
		{
			if (lhs.line.line_no == rhs.line.line_no && lhs.pos == rhs.pos)
			{
				return true;
			}
			return false;
		}

		public static bool operator !=(Marker lhs, Marker rhs)
		{
			if (lhs.line.line_no != rhs.line.line_no || lhs.pos != rhs.pos)
			{
				return true;
			}
			return false;
		}
	}

	private Line document;

	private int lines;

	private Line sentinel;

	private int document_id;

	private Random random = new Random();

	internal string password_char;

	private StringBuilder password_cache;

	private bool calc_pass;

	private int char_count;

	private bool enable_links;

	public static readonly StringFormat string_format = new StringFormat(StringFormat.GenericTypographic);

	private int recalc_suspended;

	private bool recalc_pending;

	private int recalc_start = 1;

	private int recalc_end;

	private bool recalc_optimize;

	private int update_suspended;

	private bool update_pending;

	private int update_start = 1;

	internal bool multiline;

	internal HorizontalAlignment alignment;

	internal bool wrap;

	internal UndoManager undo;

	internal Marker caret;

	internal Marker selection_start;

	internal Marker selection_end;

	internal bool selection_visible;

	internal Marker selection_anchor;

	internal Marker selection_prev;

	internal bool selection_end_anchor;

	internal int viewport_x;

	internal int viewport_y;

	internal int offset_x;

	internal int offset_y;

	internal int viewport_width;

	internal int viewport_height;

	internal int document_x;

	internal int document_y;

	internal int crlf_size;

	internal TextBoxBase owner;

	internal static int caret_width = 1;

	internal static int caret_shift = 1;

	internal int left_margin = 2;

	internal int top_margin = 2;

	internal int right_margin = 2;

	internal Line Root
	{
		get
		{
			return document;
		}
		set
		{
			document = value;
		}
	}

	internal int Lines => lines;

	internal Line CaretLine => caret.line;

	internal int CaretPosition => caret.pos;

	internal Point Caret => new Point((int)caret.tag.Line.widths[caret.pos] + caret.line.X, caret.line.Y);

	internal LineTag CaretTag
	{
		get
		{
			return caret.tag;
		}
		set
		{
			caret.tag = value;
		}
	}

	internal int CRLFSize
	{
		get
		{
			return crlf_size;
		}
		set
		{
			crlf_size = value;
		}
	}

	internal bool EnableLinks
	{
		get
		{
			return enable_links;
		}
		set
		{
			enable_links = value;
		}
	}

	internal string PasswordChar
	{
		get
		{
			return password_char;
		}
		set
		{
			password_char = value;
			PasswordCache.Length = 0;
			if (password_char.Length != 0 && password_char[0] != 0)
			{
				calc_pass = true;
			}
			else
			{
				calc_pass = false;
			}
		}
	}

	private StringBuilder PasswordCache
	{
		get
		{
			if (password_cache == null)
			{
				password_cache = new StringBuilder();
			}
			return password_cache;
		}
	}

	internal int ViewPortX
	{
		get
		{
			return viewport_x;
		}
		set
		{
			viewport_x = value;
		}
	}

	internal int Length => char_count + lines - 1;

	private int CharCount
	{
		get
		{
			return char_count;
		}
		set
		{
			char_count = value;
			if (this.LengthChanged != null)
			{
				this.LengthChanged(this, EventArgs.Empty);
			}
		}
	}

	internal int ViewPortY
	{
		get
		{
			return viewport_y;
		}
		set
		{
			viewport_y = value;
		}
	}

	internal int OffsetX
	{
		get
		{
			return offset_x;
		}
		set
		{
			offset_x = value;
		}
	}

	internal int OffsetY
	{
		get
		{
			return offset_y;
		}
		set
		{
			offset_y = value;
		}
	}

	internal int ViewPortWidth
	{
		get
		{
			return viewport_width;
		}
		set
		{
			viewport_width = value;
		}
	}

	internal int ViewPortHeight
	{
		get
		{
			return viewport_height;
		}
		set
		{
			viewport_height = value;
		}
	}

	internal int Width => document_x;

	internal int Height => document_y;

	internal bool SelectionVisible => selection_visible;

	internal bool Wrap
	{
		get
		{
			return wrap;
		}
		set
		{
			wrap = value;
		}
	}

	internal event EventHandler CaretMoved;

	internal event EventHandler WidthChanged;

	internal event EventHandler HeightChanged;

	internal event EventHandler LengthChanged;

	internal event EventHandler UIASelectionChanged;

	internal Document(TextBoxBase owner)
	{
		lines = 0;
		this.owner = owner;
		multiline = true;
		password_char = string.Empty;
		calc_pass = false;
		recalc_pending = false;
		sentinel = new Line(this, LineEnding.None);
		sentinel.color = LineColor.Black;
		document = sentinel;
		owner.HandleCreated += owner_HandleCreated;
		owner.VisibleChanged += owner_VisibleChanged;
		Add(1, string.Empty, owner.Font, owner.ForeColor, LineEnding.None);
		undo = new UndoManager(this);
		selection_visible = false;
		selection_start.line = document;
		selection_start.pos = 0;
		selection_start.tag = selection_start.line.tags;
		selection_end.line = document;
		selection_end.pos = 0;
		selection_end.tag = selection_end.line.tags;
		selection_anchor.line = document;
		selection_anchor.pos = 0;
		selection_anchor.tag = selection_anchor.line.tags;
		caret.line = document;
		caret.pos = 0;
		caret.tag = caret.line.tags;
		viewport_x = 0;
		viewport_y = 0;
		offset_x = 0;
		offset_y = 0;
		crlf_size = 2;
		document_id = random.Next();
		string_format.Trimming = StringTrimming.None;
		string_format.FormatFlags = StringFormatFlags.DisplayFormatControl;
		UpdateMargins();
	}

	internal void UpdateMargins()
	{
		switch (owner.actual_border_style)
		{
		case BorderStyle.None:
			left_margin = 0;
			top_margin = 0;
			right_margin = 1;
			break;
		case BorderStyle.FixedSingle:
			left_margin = 2;
			top_margin = 2;
			right_margin = 3;
			break;
		case BorderStyle.Fixed3D:
			left_margin = 1;
			top_margin = 1;
			right_margin = 2;
			break;
		}
	}

	internal void SuspendRecalc()
	{
		if (recalc_suspended == 0)
		{
			recalc_start = int.MaxValue;
			recalc_end = int.MinValue;
		}
		recalc_suspended++;
	}

	internal void ResumeRecalc(bool immediate_update)
	{
		if (recalc_suspended > 0)
		{
			recalc_suspended--;
		}
		if (recalc_suspended == 0 && (immediate_update || recalc_pending) && (recalc_start != int.MaxValue || recalc_end != int.MinValue))
		{
			RecalculateDocument(owner.CreateGraphicsInternal(), recalc_start, recalc_end, recalc_optimize);
			recalc_pending = false;
		}
	}

	internal void SuspendUpdate()
	{
		update_suspended++;
	}

	internal void ResumeUpdate(bool immediate_update)
	{
		if (update_suspended > 0)
		{
			update_suspended--;
		}
		if (immediate_update && update_suspended == 0 && update_pending)
		{
			UpdateView(GetLine(update_start), 0);
			update_pending = false;
		}
	}

	internal int DumpTree(Line line, bool with_tags)
	{
		int num = 1;
		Console.Write("Line {0} [# {1}], Y: {2}, ending style: {3},  Text: '{4}'", line.line_no, line.GetHashCode(), line.Y, line.ending, (line.text == null) ? "undefined" : line.text.ToString());
		if (line.left == sentinel)
		{
			Console.Write(", left = sentinel");
		}
		else if (line.left == null)
		{
			Console.Write(", left = NULL");
		}
		if (line.right == sentinel)
		{
			Console.Write(", right = sentinel");
		}
		else if (line.right == null)
		{
			Console.Write(", right = NULL");
		}
		Console.WriteLine(string.Empty);
		if (with_tags)
		{
			LineTag lineTag = line.tags;
			int num2 = 1;
			int num3 = 0;
			Console.Write("   Tags: ");
			while (lineTag != null)
			{
				Console.Write("{0} <{1}>-<{2}>", num2++, lineTag.Start, lineTag.End);
				num3 += lineTag.Length;
				if (lineTag.Line != line)
				{
					Console.Write("BAD line link");
					throw new Exception("Bad line link in tree");
				}
				lineTag = lineTag.Next;
				if (lineTag != null)
				{
					Console.Write(", ");
				}
			}
			if (num3 > line.text.Length)
			{
				throw new Exception($"Length of tags more than length of text on line (expected {line.text.Length} calculated {num3})");
			}
			if (num3 < line.text.Length)
			{
				throw new Exception($"Length of tags less than length of text on line (expected {line.text.Length} calculated {num3})");
			}
			Console.WriteLine(string.Empty);
		}
		if (line.left != null)
		{
			if (line.left != sentinel)
			{
				num += DumpTree(line.left, with_tags);
			}
		}
		else if (line != sentinel)
		{
			throw new Exception("Left should not be NULL");
		}
		if (line.right != null)
		{
			if (line.right != sentinel)
			{
				num += DumpTree(line.right, with_tags);
			}
		}
		else if (line != sentinel)
		{
			throw new Exception("Right should not be NULL");
		}
		for (int i = 1; i <= lines; i++)
		{
			if (GetLine(i) == null)
			{
				throw new Exception($"Hole in line order, missing {i}");
			}
		}
		if (line == Root)
		{
			if (num < lines)
			{
				throw new Exception($"Not enough nodes in tree, found {num}, expected {lines}");
			}
			if (num > lines)
			{
				throw new Exception($"Too many nodes in tree, found {num}, expected {lines}");
			}
		}
		return num;
	}

	private void SetSelectionVisible(bool value)
	{
		bool flag = selection_visible;
		selection_visible = value;
		if (owner.IsHandleCreated && !owner.show_caret_w_selection)
		{
			XplatUI.CaretVisible(owner.Handle, !selection_visible);
		}
		if (this.UIASelectionChanged != null && (selection_visible || flag))
		{
			this.UIASelectionChanged(this, EventArgs.Empty);
		}
	}

	private void DecrementLines(int line_no)
	{
		for (int i = line_no; i <= lines; i++)
		{
			GetLine(i).line_no--;
		}
	}

	private void IncrementLines(int line_no)
	{
		for (int num = lines; num >= line_no; num--)
		{
			GetLine(num).line_no++;
		}
	}

	private void RebalanceAfterAdd(Line line1)
	{
		while (line1 != document && line1.parent.color == LineColor.Red)
		{
			Line right;
			if (line1.parent == line1.parent.parent.left)
			{
				right = line1.parent.parent.right;
				if (right != null && right.color == LineColor.Red)
				{
					line1.parent.color = LineColor.Black;
					right.color = LineColor.Black;
					line1.parent.parent.color = LineColor.Red;
					line1 = line1.parent.parent;
					continue;
				}
				if (line1 == line1.parent.right)
				{
					line1 = line1.parent;
					RotateLeft(line1);
				}
				line1.parent.color = LineColor.Black;
				line1.parent.parent.color = LineColor.Red;
				RotateRight(line1.parent.parent);
				continue;
			}
			right = line1.parent.parent.left;
			if (right != null && right.color == LineColor.Red)
			{
				line1.parent.color = LineColor.Black;
				right.color = LineColor.Black;
				line1.parent.parent.color = LineColor.Red;
				line1 = line1.parent.parent;
				continue;
			}
			if (line1 == line1.parent.left)
			{
				line1 = line1.parent;
				RotateRight(line1);
			}
			line1.parent.color = LineColor.Black;
			line1.parent.parent.color = LineColor.Red;
			RotateLeft(line1.parent.parent);
		}
		document.color = LineColor.Black;
	}

	private void RebalanceAfterDelete(Line line1)
	{
		while (line1 != document && line1.color == LineColor.Black)
		{
			Line right;
			if (line1 == line1.parent.left)
			{
				right = line1.parent.right;
				if (right.color == LineColor.Red)
				{
					right.color = LineColor.Black;
					line1.parent.color = LineColor.Red;
					RotateLeft(line1.parent);
					right = line1.parent.right;
				}
				if (right.left.color == LineColor.Black && right.right.color == LineColor.Black)
				{
					right.color = LineColor.Red;
					line1 = line1.parent;
					continue;
				}
				if (right.right.color == LineColor.Black)
				{
					right.left.color = LineColor.Black;
					right.color = LineColor.Red;
					RotateRight(right);
					right = line1.parent.right;
				}
				right.color = line1.parent.color;
				line1.parent.color = LineColor.Black;
				right.right.color = LineColor.Black;
				RotateLeft(line1.parent);
				line1 = document;
				continue;
			}
			right = line1.parent.left;
			if (right.color == LineColor.Red)
			{
				right.color = LineColor.Black;
				line1.parent.color = LineColor.Red;
				RotateRight(line1.parent);
				right = line1.parent.left;
			}
			if (right.right.color == LineColor.Black && right.left.color == LineColor.Black)
			{
				right.color = LineColor.Red;
				line1 = line1.parent;
				continue;
			}
			if (right.left.color == LineColor.Black)
			{
				right.right.color = LineColor.Black;
				right.color = LineColor.Red;
				RotateLeft(right);
				right = line1.parent.left;
			}
			right.color = line1.parent.color;
			line1.parent.color = LineColor.Black;
			right.left.color = LineColor.Black;
			RotateRight(line1.parent);
			line1 = document;
		}
		line1.color = LineColor.Black;
	}

	private void RotateLeft(Line line1)
	{
		Line right = line1.right;
		line1.right = right.left;
		if (right.left != sentinel)
		{
			right.left.parent = line1;
		}
		if (right != sentinel)
		{
			right.parent = line1.parent;
		}
		if (line1.parent != null)
		{
			if (line1 == line1.parent.left)
			{
				line1.parent.left = right;
			}
			else
			{
				line1.parent.right = right;
			}
		}
		else
		{
			document = right;
		}
		right.left = line1;
		if (line1 != sentinel)
		{
			line1.parent = right;
		}
	}

	private void RotateRight(Line line1)
	{
		Line left = line1.left;
		line1.left = left.right;
		if (left.right != sentinel)
		{
			left.right.parent = line1;
		}
		if (left != sentinel)
		{
			left.parent = line1.parent;
		}
		if (line1.parent != null)
		{
			if (line1 == line1.parent.right)
			{
				line1.parent.right = left;
			}
			else
			{
				line1.parent.left = left;
			}
		}
		else
		{
			document = left;
		}
		left.right = line1;
		if (line1 != sentinel)
		{
			line1.parent = left;
		}
	}

	internal void UpdateView(Line line, int pos)
	{
		if (!owner.IsHandleCreated)
		{
			return;
		}
		if (update_suspended > 0)
		{
			update_start = Math.Min(update_start, line.line_no);
			update_pending = true;
			return;
		}
		if (RecalculateDocument(owner.CreateGraphicsInternal(), line.line_no, line.line_no, optimize: true))
		{
			if (line.Y - viewport_y >= 0)
			{
				owner.Invalidate(new Rectangle(offset_x, line.Y - viewport_y + offset_y, viewport_width, owner.Height - (line.Y - viewport_y)));
			}
			else
			{
				owner.Invalidate();
			}
			return;
		}
		switch (line.alignment)
		{
		case HorizontalAlignment.Left:
			owner.Invalidate(new Rectangle(line.X + ((int)line.widths[pos] - viewport_x - 1) + offset_x, line.Y - viewport_y + offset_y, viewport_width, line.height + 1));
			break;
		case HorizontalAlignment.Center:
			owner.Invalidate(new Rectangle(line.X + offset_x, line.Y - viewport_y + offset_y, viewport_width, line.height + 1));
			break;
		case HorizontalAlignment.Right:
			owner.Invalidate(new Rectangle(line.X + offset_x, line.Y - viewport_y + offset_y, (int)line.widths[pos + 1] - viewport_x + line.X, line.height + 1));
			break;
		}
	}

	internal void UpdateView(Line line, int line_count, int pos)
	{
		if (!owner.IsHandleCreated)
		{
			return;
		}
		if (recalc_suspended > 0)
		{
			recalc_start = Math.Min(recalc_start, line.line_no);
			recalc_end = Math.Max(recalc_end, line.line_no + line_count);
			recalc_optimize = true;
			recalc_pending = true;
			return;
		}
		int y = line.Y;
		Line line2 = GetLine(line.line_no + line_count);
		if (line2 == null)
		{
			line2 = GetLine(lines);
		}
		int num = line2.Y + line2.height;
		if (RecalculateDocument(owner.CreateGraphicsInternal(), line.line_no, line.line_no + line_count, optimize: true))
		{
			if (line.Y - viewport_y >= 0)
			{
				owner.Invalidate(new Rectangle(offset_x, line.Y - viewport_y + offset_y, viewport_width, owner.Height - (line.Y - viewport_y)));
			}
			else
			{
				owner.Invalidate();
			}
		}
		else
		{
			int x = -viewport_x + offset_x;
			int width = viewport_width;
			int num2 = Math.Min(y - viewport_y, line.Y - viewport_y) + offset_y;
			int height = Math.Max(num - num2, line2.Y + line2.height - num2);
			owner.Invalidate(new Rectangle(x, num2, width, height));
		}
	}

	private void ScanForLinks(Line start_line, ref bool link_changed)
	{
		Line line = start_line;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		ArrayList arrayList = new ArrayList();
		bool flag = false;
		arrayList.Add(0);
		while (line != null)
		{
			stringBuilder.Append(line.text);
			if (!link_changed)
			{
				line.LinkRecord(stringBuilder2);
			}
			line.ClearLinks();
			arrayList.Add(stringBuilder.Length);
			if (line.ending == LineEnding.Wrap)
			{
				line = GetLine(line.LineNo + 1);
				continue;
			}
			break;
		}
		string[] array = new string[4] { "www.", "http:/", "ftp:/", "https:/" };
		int term_found = 0;
		int num = 0;
		string text = stringBuilder.ToString();
		int num2 = 0;
		int num3 = 0;
		while (num2 < text.Length)
		{
			num = FirstIndexOfAny(text, array, num2, out term_found);
			if (num == -1)
			{
				break;
			}
			if (term_found == 0)
			{
				if (text.Length == num + array[0].Length)
				{
					break;
				}
				if (!char.IsLetterOrDigit(text[num + array[0].Length]) && "@/~".IndexOf(text[num + array[0].Length].ToString()) == -1)
				{
					num2 = num + array[0].Length;
					continue;
				}
			}
			num3 = text.Length - 1;
			num2 = text.Length;
			for (int i = num + array[term_found].Length; i < text.Length; i++)
			{
				if (text[i - 1] == '.')
				{
					if (!char.IsLetterOrDigit(text[i]) && "@/~".IndexOf(text[i].ToString()) == -1)
					{
						num3 = i - 1;
						num2 = i;
						break;
					}
				}
				else if (!char.IsLetterOrDigit(text[i]) && "@-/:~.?=_&".IndexOf(text[i].ToString()) == -1)
				{
					num3 = i - 1;
					num2 = i;
					break;
				}
			}
			string text2 = text.Substring(num, num3 - num + 1);
			int num4 = 0;
			line = start_line;
			for (num4 = 1; num4 < arrayList.Count && (int)arrayList[num4] <= num; num4++)
			{
			}
			line = GetLine(start_line.LineNo + num4 - 1);
			LineTag lineTag = line.FindTag(num - (int)arrayList[num4 - 1] + 1);
			if (lineTag.Start != num - (int)arrayList[num4 - 1] + 1)
			{
				if (lineTag == CaretTag)
				{
					flag = true;
				}
				lineTag = lineTag.Break(num - (int)arrayList[num4 - 1] + 1);
			}
			lineTag.IsLink = true;
			lineTag.LinkText = text2;
			for (int j = 1; j < text2.Length; j++)
			{
				if ((int)arrayList[num4] <= num + j)
				{
					line = GetLine(start_line.LineNo + num4++);
					lineTag = line.FindTag(num + j - (int)arrayList[num4 - 1] + 1);
					lineTag.IsLink = true;
					lineTag.LinkText = text2;
				}
				else if (lineTag.End < num + 1 + j - (int)arrayList[num4 - 1])
				{
					do
					{
						lineTag = lineTag.Next;
					}
					while (lineTag.Length == 0);
					lineTag.IsLink = true;
					lineTag.LinkText = text2;
				}
			}
			if (lineTag.End > num + text2.Length + 1 - (int)arrayList[num4 - 1])
			{
				if (lineTag == CaretTag)
				{
					flag = true;
				}
				lineTag.Break(num + text2.Length + 1 - (int)arrayList[num4 - 1]);
			}
		}
		if (flag)
		{
			CaretTag = LineTag.FindTag(CaretLine, CaretPosition);
			link_changed = true;
		}
		else
		{
			if (link_changed)
			{
				return;
			}
			line = start_line;
			StringBuilder stringBuilder3 = new StringBuilder();
			while (line != null)
			{
				line.LinkRecord(stringBuilder3);
				if (line.ending == LineEnding.Wrap)
				{
					line = GetLine(line.LineNo + 1);
					continue;
				}
				break;
			}
			if (!stringBuilder3.Equals(stringBuilder2))
			{
				link_changed = true;
			}
		}
	}

	private int FirstIndexOfAny(string haystack, string[] needles, int start_index, out int term_found)
	{
		term_found = -1;
		int num = -1;
		for (int i = 0; i < needles.Length; i++)
		{
			int num2 = haystack.IndexOf(needles[i], start_index, StringComparison.InvariantCultureIgnoreCase);
			if (num2 <= -1)
			{
				continue;
			}
			if (term_found > -1)
			{
				if (num2 < num)
				{
					num = num2;
					term_found = i;
				}
			}
			else
			{
				num = num2;
				term_found = i;
			}
		}
		return num;
	}

	private void InvalidateLinks(Rectangle clip)
	{
		for (int num = owner.list_links.Count - 1; num >= 0; num--)
		{
			TextBoxBase.LinkRectangle linkRectangle = (TextBoxBase.LinkRectangle)owner.list_links[num];
			if (clip.IntersectsWith(linkRectangle.LinkAreaRectangle))
			{
				owner.list_links.RemoveAt(num);
			}
		}
	}

	internal void ScanForLinks(int start, int end, ref bool link_changed)
	{
		Line line = null;
		LineEnding lineEnding = LineEnding.Rich;
		while (start != 1 && GetLine(start - 1).ending == LineEnding.Wrap)
		{
			start--;
		}
		for (int i = start; i <= end && i <= lines; i++)
		{
			line = GetLine(i);
			if (lineEnding != LineEnding.Wrap)
			{
				ScanForLinks(line, ref link_changed);
			}
			lineEnding = line.ending;
			if (lineEnding == LineEnding.Wrap && i + 1 <= end)
			{
				end++;
			}
		}
	}

	internal void Empty()
	{
		document = sentinel;
		lines = 0;
		Add(1, string.Empty, owner.Font, owner.ForeColor, LineEnding.None);
		RecalculateDocument(owner.CreateGraphicsInternal());
		PositionCaret(0, 0);
		SetSelectionVisible(value: false);
		selection_start.line = document;
		selection_start.pos = 0;
		selection_start.tag = selection_start.line.tags;
		selection_end.line = document;
		selection_end.pos = 0;
		selection_end.tag = selection_end.line.tags;
		char_count = 0;
		viewport_x = 0;
		viewport_y = 0;
		document_x = 0;
		document_y = 0;
		if (owner.IsHandleCreated)
		{
			owner.Invalidate();
		}
	}

	internal void PositionCaret(Line line, int pos)
	{
		caret.tag = line.FindTag(pos);
		MoveCaretToTextTag();
		caret.line = line;
		caret.pos = pos;
		if (owner.IsHandleCreated)
		{
			if (owner.Focused)
			{
				if (caret.height != caret.tag.Height)
				{
					XplatUI.CreateCaret(owner.Handle, caret_width, caret.height);
				}
				XplatUI.SetCaretPos(owner.Handle, offset_x + (int)caret.tag.Line.widths[caret.pos] + caret.line.X - viewport_x, offset_y + caret.line.Y + caret.tag.Shift - viewport_y + caret_shift);
			}
			if (this.CaretMoved != null)
			{
				this.CaretMoved(this, EventArgs.Empty);
			}
		}
		caret.height = caret.tag.Height;
	}

	internal void PositionCaret(int x, int y)
	{
		if (owner.IsHandleCreated)
		{
			caret.tag = FindCursor(x, y, out caret.pos);
			MoveCaretToTextTag();
			caret.line = caret.tag.Line;
			caret.height = caret.tag.Height;
			if (owner.ShowSelection && (!selection_visible || owner.show_caret_w_selection))
			{
				XplatUI.CreateCaret(owner.Handle, caret_width, caret.height);
				XplatUI.SetCaretPos(owner.Handle, (int)caret.tag.Line.widths[caret.pos] + caret.line.X - viewport_x + offset_x, offset_y + caret.line.Y + caret.tag.Shift - viewport_y + caret_shift);
			}
			if (this.CaretMoved != null)
			{
				this.CaretMoved(this, EventArgs.Empty);
			}
		}
	}

	internal void CaretHasFocus()
	{
		if (caret.tag != null && owner.IsHandleCreated)
		{
			XplatUI.CreateCaret(owner.Handle, caret_width, caret.height);
			XplatUI.SetCaretPos(owner.Handle, offset_x + (int)caret.tag.Line.widths[caret.pos] + caret.line.X - viewport_x, offset_y + caret.line.Y + caret.tag.Shift - viewport_y + caret_shift);
			DisplayCaret();
		}
		if (owner.IsHandleCreated && SelectionLength() > 0)
		{
			InvalidateSelectionArea();
		}
	}

	internal void CaretLostFocus()
	{
		if (owner.IsHandleCreated)
		{
			XplatUI.DestroyCaret(owner.Handle);
		}
	}

	internal void AlignCaret()
	{
		AlignCaret(changeCaretTag: true);
	}

	internal void AlignCaret(bool changeCaretTag)
	{
		if (owner.IsHandleCreated)
		{
			if (changeCaretTag)
			{
				caret.tag = LineTag.FindTag(caret.line, caret.pos);
				MoveCaretToTextTag();
			}
			if (caret.tag.Height > caret.tag.Line.Height)
			{
				caret.height = caret.line.height;
			}
			else
			{
				caret.height = caret.tag.Height;
			}
			if (owner.Focused)
			{
				XplatUI.CreateCaret(owner.Handle, caret_width, caret.height);
				XplatUI.SetCaretPos(owner.Handle, offset_x + (int)caret.tag.Line.widths[caret.pos] + caret.line.X - viewport_x, offset_y + caret.line.Y + viewport_y + caret_shift);
				DisplayCaret();
			}
			if (this.CaretMoved != null)
			{
				this.CaretMoved(this, EventArgs.Empty);
			}
		}
	}

	internal void UpdateCaret()
	{
		if (!owner.IsHandleCreated || caret.tag == null)
		{
			return;
		}
		MoveCaretToTextTag();
		if (caret.tag.Height != caret.height)
		{
			caret.height = caret.tag.Height;
			if (owner.Focused)
			{
				XplatUI.CreateCaret(owner.Handle, caret_width, caret.height);
			}
		}
		if (owner.Focused)
		{
			XplatUI.SetCaretPos(owner.Handle, offset_x + (int)caret.tag.Line.widths[caret.pos] + caret.line.X - viewport_x, offset_y + caret.line.Y + caret.tag.Shift - viewport_y + caret_shift);
			DisplayCaret();
		}
		if (this.CaretMoved != null)
		{
			this.CaretMoved(this, EventArgs.Empty);
		}
	}

	internal void DisplayCaret()
	{
		if (owner.IsHandleCreated && owner.ShowSelection && (!selection_visible || owner.show_caret_w_selection))
		{
			XplatUI.CaretVisible(owner.Handle, visible: true);
		}
	}

	internal void HideCaret()
	{
		if (owner.IsHandleCreated && owner.Focused)
		{
			XplatUI.CaretVisible(owner.Handle, visible: false);
		}
	}

	internal void MoveCaretToTextTag()
	{
		if (caret.tag != null && !caret.tag.IsTextTag)
		{
			if (caret.pos < caret.tag.Start)
			{
				caret.tag = caret.tag.Previous;
			}
			else
			{
				caret.tag = caret.tag.Next;
			}
		}
	}

	internal void MoveCaret(CaretDirection direction)
	{
		bool flag = false;
		switch (direction)
		{
		case CaretDirection.CharForwardNoWrap:
			flag = true;
			goto case CaretDirection.CharForward;
		case CaretDirection.CharForward:
			caret.pos++;
			if (caret.pos > caret.line.TextLengthWithoutEnding())
			{
				if (!flag)
				{
					if (caret.line.line_no < lines)
					{
						caret.line = GetLine(caret.line.line_no + 1);
						caret.pos = 0;
						caret.tag = caret.line.tags;
					}
					else
					{
						caret.pos--;
					}
				}
				else
				{
					caret.pos--;
				}
			}
			else if (caret.tag.Start - 1 + caret.tag.Length < caret.pos)
			{
				caret.tag = caret.tag.Next;
			}
			UpdateCaret();
			break;
		case CaretDirection.CharBackNoWrap:
			flag = true;
			goto case CaretDirection.CharBack;
		case CaretDirection.CharBack:
			if (caret.pos > 0)
			{
				if (--caret.pos > 0 && caret.tag.Start > caret.pos)
				{
					caret.tag = caret.tag.Previous;
				}
			}
			else if (caret.line.line_no > 1 && !flag)
			{
				caret.line = GetLine(caret.line.line_no - 1);
				caret.pos = caret.line.TextLengthWithoutEnding();
				caret.tag = LineTag.FindTag(caret.line, caret.pos);
			}
			UpdateCaret();
			break;
		case CaretDirection.WordForward:
		{
			int length = caret.line.text.Length;
			if (caret.pos < length)
			{
				while (caret.pos < length && caret.line.text[caret.pos] != ' ')
				{
					caret.pos++;
				}
				if (caret.pos < length)
				{
					while (caret.pos < length && caret.line.text[caret.pos] == ' ')
					{
						caret.pos++;
					}
				}
				caret.tag = LineTag.FindTag(caret.line, caret.pos);
			}
			else if (caret.line.line_no < lines)
			{
				caret.line = GetLine(caret.line.line_no + 1);
				caret.pos = 0;
				caret.tag = caret.line.tags;
			}
			UpdateCaret();
			break;
		}
		case CaretDirection.WordBack:
			if (caret.pos > 0)
			{
				caret.pos--;
				while (caret.pos > 0 && caret.line.text[caret.pos] == ' ')
				{
					caret.pos--;
				}
				while (caret.pos > 0 && caret.line.text[caret.pos] != ' ')
				{
					caret.pos--;
				}
				if (caret.line.text.ToString(caret.pos, 1) == " ")
				{
					if (caret.pos != 0)
					{
						caret.pos++;
					}
					else
					{
						caret.line = GetLine(caret.line.line_no - 1);
						caret.pos = caret.line.text.Length;
					}
				}
				caret.tag = LineTag.FindTag(caret.line, caret.pos);
			}
			else if (caret.line.line_no > 1)
			{
				caret.line = GetLine(caret.line.line_no - 1);
				caret.pos = caret.line.text.Length;
				caret.tag = LineTag.FindTag(caret.line, caret.pos);
			}
			UpdateCaret();
			break;
		case CaretDirection.LineUp:
			if (caret.line.line_no > 1)
			{
				int x = (int)caret.line.widths[caret.pos];
				PositionCaret(x, GetLine(caret.line.line_no - 1).Y);
				DisplayCaret();
			}
			break;
		case CaretDirection.LineDown:
			if (caret.line.line_no < lines)
			{
				int x2 = (int)caret.line.widths[caret.pos];
				PositionCaret(x2, GetLine(caret.line.line_no + 1).Y);
				DisplayCaret();
			}
			break;
		case CaretDirection.Home:
			if (caret.pos > 0)
			{
				caret.pos = 0;
				caret.tag = caret.line.tags;
				UpdateCaret();
			}
			break;
		case CaretDirection.End:
			if (caret.pos < caret.line.TextLengthWithoutEnding())
			{
				caret.pos = caret.line.TextLengthWithoutEnding();
				caret.tag = LineTag.FindTag(caret.line, caret.pos);
				UpdateCaret();
			}
			break;
		case CaretDirection.PgUp:
		{
			if (caret.line.line_no == 1 && owner.richtext)
			{
				owner.vscroll.Value = 0;
				Line line3 = GetLine(1);
				PositionCaret(line3, 0);
			}
			int num2 = caret.line.Y + caret.line.height - 1 - viewport_y;
			int index3;
			LineTag lineTag3 = FindCursor((int)caret.line.widths[caret.pos], viewport_y - viewport_height, out index3);
			owner.vscroll.Value = Math.Min(lineTag3.Line.Y, owner.vscroll.Maximum - viewport_height);
			PositionCaret((int)caret.line.widths[caret.pos], num2 + viewport_y);
			break;
		}
		case CaretDirection.PgDn:
		{
			if (caret.line.line_no == lines && owner.richtext)
			{
				owner.vscroll.Value = owner.vscroll.Maximum - viewport_height + 1;
				Line line2 = GetLine(lines);
				PositionCaret(line2, line2.TextLengthWithoutEnding());
			}
			int num = caret.line.Y - viewport_y;
			int index2;
			LineTag lineTag2 = FindCursor((int)caret.line.widths[caret.pos], viewport_y + viewport_height, out index2);
			owner.vscroll.Value = Math.Min(lineTag2.Line.Y, owner.vscroll.Maximum - viewport_height);
			PositionCaret((int)caret.line.widths[caret.pos], num + viewport_y);
			break;
		}
		case CaretDirection.CtrlPgUp:
			PositionCaret(0, viewport_y);
			DisplayCaret();
			break;
		case CaretDirection.CtrlPgDn:
		{
			int index;
			LineTag lineTag = FindCursor(0, viewport_y + viewport_height, out index);
			Line line = ((lineTag.Line.line_no <= 1) ? lineTag.Line : GetLine(lineTag.Line.line_no - 1));
			PositionCaret(line, line.Text.Length);
			DisplayCaret();
			break;
		}
		case CaretDirection.CtrlHome:
			caret.line = GetLine(1);
			caret.pos = 0;
			caret.tag = caret.line.tags;
			UpdateCaret();
			break;
		case CaretDirection.CtrlEnd:
			caret.line = GetLine(lines);
			caret.pos = caret.line.TextLengthWithoutEnding();
			caret.tag = LineTag.FindTag(caret.line, caret.pos);
			UpdateCaret();
			break;
		case CaretDirection.SelectionStart:
			caret.line = selection_start.line;
			caret.pos = selection_start.pos;
			caret.tag = selection_start.tag;
			UpdateCaret();
			break;
		case CaretDirection.SelectionEnd:
			caret.line = selection_end.line;
			caret.pos = selection_end.pos;
			caret.tag = selection_end.tag;
			UpdateCaret();
			break;
		}
	}

	internal void DumpDoc()
	{
		Console.WriteLine("<doc lines='{0}'>", lines);
		for (int i = 1; i <= lines; i++)
		{
			Line line = GetLine(i);
			Console.WriteLine("<line no='{0}' ending='{1}'>", line.line_no, line.ending);
			for (LineTag lineTag = line.tags; lineTag != null; lineTag = lineTag.Next)
			{
				Console.Write("\t<tag type='{0}' span='{1}->{2}' font='{3}' color='{4}'>", lineTag.GetType(), lineTag.Start, lineTag.Length, lineTag.Font, lineTag.Color);
				Console.Write(lineTag.Text());
				Console.WriteLine("</tag>");
			}
			Console.WriteLine("</line>");
		}
		Console.WriteLine("</doc>");
	}

	internal void GetVisibleLineIndexes(Rectangle clip, out int start, out int end)
	{
		if (multiline)
		{
			start = GetLineByPixel(clip.Top + viewport_y - offset_y, exact: false).line_no;
			end = GetLineByPixel(clip.Bottom + viewport_y - offset_y, exact: false).line_no;
		}
		else
		{
			start = GetLineByPixel(clip.Left + viewport_x - offset_x, exact: false).line_no;
			end = GetLineByPixel(clip.Right + viewport_x - offset_x, exact: false).line_no;
		}
	}

	internal void Draw(Graphics g, Rectangle clip)
	{
		GetVisibleLineIndexes(clip, out var start, out var end);
		InvalidateLinks(clip);
		if (owner.actual_border_style == BorderStyle.FixedSingle)
		{
			ControlPaint.DrawBorder(g, owner.ClientRectangle, System.Drawing.Color.Black, ButtonBorderStyle.Solid);
		}
		Line line = GetLine(end - 1);
		if (line != null && clip.Bottom == offset_y + line.Y + line.height - viewport_y)
		{
			end--;
		}
		int i = start;
		if (!multiline && selection_visible && owner.ShowSelection)
		{
			g.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ThemeEngine.Current.ColorHighlight), (float)offset_x + selection_start.line.widths[selection_start.pos] + (float)selection_start.line.X - (float)viewport_x, offset_y + selection_start.line.Y, (float)selection_end.line.X + selection_end.line.widths[selection_end.pos] - ((float)selection_start.line.X + selection_start.line.widths[selection_start.pos]), selection_start.line.height);
		}
		for (; i <= end; i++)
		{
			line = GetLine(i);
			float num = line.Y - viewport_y + offset_y;
			LineTag lineTag = line.tags;
			StringBuilder stringBuilder;
			if (!calc_pass)
			{
				stringBuilder = line.text;
			}
			else
			{
				if (PasswordCache.Length < line.text.Length)
				{
					PasswordCache.Append(char.Parse(password_char), line.text.Length - PasswordCache.Length);
				}
				else if (PasswordCache.Length > line.text.Length)
				{
					PasswordCache.Remove(line.text.Length, PasswordCache.Length - line.text.Length);
				}
				stringBuilder = PasswordCache;
			}
			int num2 = stringBuilder.Length + 1;
			int num3 = stringBuilder.Length + 1;
			if (selection_visible && owner.ShowSelection && i >= selection_start.line.line_no && i <= selection_end.line.line_no)
			{
				num2 = ((i != selection_start.line.line_no) ? 1 : (selection_start.pos + 1));
				num3 = ((i != selection_end.line.line_no) ? (stringBuilder.Length + 1) : (selection_end.pos + 1));
				if (num3 == num2)
				{
					num2 = stringBuilder.Length + 1;
					num3 = num2;
				}
				else if (multiline)
				{
					g.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ThemeEngine.Current.ColorHighlight), (float)offset_x + line.widths[num2 - 1] + (float)line.X - (float)viewport_x, num, line.widths[num3 - 1] - line.widths[num2 - 1], line.height);
				}
			}
			System.Drawing.Color colorToDisplay = line.tags.ColorToDisplay;
			while (lineTag != null)
			{
				if (lineTag.Length == 0)
				{
					lineTag = lineTag.Next;
					continue;
				}
				if (lineTag.X + lineTag.Width < (float)(clip.Left - viewport_x - offset_x) && lineTag.X > (float)(clip.Right - viewport_x - offset_x))
				{
					lineTag = lineTag.Next;
					continue;
				}
				if (lineTag.BackColor != System.Drawing.Color.Empty)
				{
					g.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(lineTag.BackColor), (float)offset_x + lineTag.X + (float)line.X - (float)viewport_x, num + (float)lineTag.Shift, lineTag.Width, line.height);
				}
				System.Drawing.Color color = lineTag.ColorToDisplay;
				colorToDisplay = color;
				if (!owner.Enabled)
				{
					System.Drawing.Color color2 = lineTag.Color;
					System.Drawing.Color colorWindowText = ThemeEngine.Current.ColorWindowText;
					if (color2.R == colorWindowText.R && color2.G == colorWindowText.G && color2.B == colorWindowText.B)
					{
						color = ThemeEngine.Current.ColorGrayText;
					}
				}
				int num4 = lineTag.Start;
				colorToDisplay = color;
				while (num4 < lineTag.Start + lineTag.Length)
				{
					int num5 = num4;
					if (num4 >= num2 && num4 < num3)
					{
						colorToDisplay = ThemeEngine.Current.ColorHighlightText;
						num4 = Math.Min(lineTag.End, num3);
					}
					else if (num4 < num2)
					{
						colorToDisplay = color;
						num4 = Math.Min(lineTag.End, num2);
					}
					else
					{
						colorToDisplay = color;
						num4 = lineTag.End;
					}
					lineTag.Draw(g, colorToDisplay, offset_x + line.X - viewport_x, num + (float)lineTag.Shift, num5 - 1, Math.Min(lineTag.Start + lineTag.Length, num4) - 1, stringBuilder.ToString(), out var measuredText, lineTag.IsLink);
					if (lineTag.IsLink)
					{
						TextBoxBase.LinkRectangle linkRectangle = new TextBoxBase.LinkRectangle(measuredText);
						linkRectangle.LinkTag = lineTag;
						owner.list_links.Add(linkRectangle);
					}
				}
				lineTag = lineTag.Next;
			}
			line.DrawEnding(g, num);
		}
	}

	private int GetLineEnding(string line, int start, out LineEnding ending)
	{
		if (start >= line.Length)
		{
			ending = LineEnding.Wrap;
			return -1;
		}
		int num = line.IndexOf('\r', start);
		int num2 = line.IndexOf('\n', start);
		if (num != -1 && num2 != -1 && num2 < num)
		{
			ending = LineEnding.Rich;
			return num2;
		}
		if (num != -1)
		{
			if (num + 2 < line.Length && line[num + 1] == '\r' && line[num + 2] == '\n')
			{
				ending = LineEnding.Soft;
				return num;
			}
			if (num + 1 < line.Length && line[num + 1] == '\n')
			{
				ending = LineEnding.Hard;
				return num;
			}
			ending = LineEnding.Limp;
			return num;
		}
		if (num2 != -1)
		{
			ending = LineEnding.Rich;
			return num2;
		}
		ending = LineEnding.Wrap;
		return line.Length;
	}

	private int GetLineEnding(string line, int start, out LineEnding ending, LineEnding type)
	{
		int num = start;
		int num2 = 0;
		do
		{
			num = GetLineEnding(line, num + num2, out ending);
			num2 = LineEndingLength(ending);
		}
		while ((ending & type) != ending && num != -1);
		return (num != -1) ? num : line.Length;
	}

	internal int LineEndingLength(LineEnding ending)
	{
		switch (ending)
		{
		case LineEnding.Limp:
		case LineEnding.Rich:
			return 1;
		case LineEnding.Hard:
			return 2;
		case LineEnding.Soft:
			return 3;
		default:
			return 0;
		}
	}

	internal string LineEndingToString(LineEnding ending)
	{
		return ending switch
		{
			LineEnding.Limp => "\r", 
			LineEnding.Hard => "\r\n", 
			LineEnding.Soft => "\r\r\n", 
			LineEnding.Rich => "\n", 
			_ => string.Empty, 
		};
	}

	internal LineEnding StringToLineEnding(string ending)
	{
		return ending switch
		{
			"\r" => LineEnding.Limp, 
			"\r\n" => LineEnding.Hard, 
			"\r\r\n" => LineEnding.Soft, 
			"\n" => LineEnding.Rich, 
			_ => LineEnding.None, 
		};
	}

	internal void Insert(Line line, int pos, bool update_caret, string s)
	{
		Insert(line, pos, update_caret, s, line.FindTag(pos));
	}

	internal void Insert(Line line, int pos, bool update_caret, string s, LineTag tag)
	{
		int num = 1;
		SuspendRecalc();
		int line_no = line.line_no;
		int num2 = lines;
		int num3 = s.IndexOf('\0');
		if (num3 != -1)
		{
			s = s.Substring(0, num3);
		}
		int lineEnding = GetLineEnding(s, 0, out var ending, (LineEnding)20);
		if (lineEnding == s.Length)
		{
			line.InsertString(pos, s, tag);
		}
		else
		{
			line.InsertString(pos, s.Substring(0, lineEnding + LineEndingLength(ending)), tag);
			Split(line, pos + (lineEnding + LineEndingLength(ending)));
			line.ending = ending;
			lineEnding += LineEndingLength(ending);
			Line line2 = GetLine(line.line_no + 1);
			while (true)
			{
				int lineEnding2 = GetLineEnding(s, lineEnding, out ending, (LineEnding)20);
				if (lineEnding2 == s.Length)
				{
					break;
				}
				string text = s.Substring(lineEnding, lineEnding2 - lineEnding + LineEndingLength(ending));
				Add(line_no + num, text, line.alignment, tag.Font, tag.Color, ending);
				Line line3 = GetLine(line_no + num);
				line3.ending = ending;
				num++;
				lineEnding = lineEnding2 + LineEndingLength(ending);
			}
			line2.InsertString(0, s.Substring(lineEnding));
		}
		ResumeRecalc(immediate_update: false);
		CharCount += s.Length;
		UpdateView(line, lines - num2 + 1, pos);
		if (update_caret)
		{
			Line line4 = GetLine(line.line_no + lines - num2);
			PositionCaret(line4, line4.text.Length);
			DisplayCaret();
		}
	}

	internal void InsertString(Line line, int pos, string s)
	{
		CharCount += s.Length;
		line.InsertString(pos, s);
	}

	internal void InsertCharAtCaret(char ch, bool move_caret)
	{
		caret.line.InsertString(caret.pos, ch.ToString(), caret.tag);
		CharCount++;
		undo.RecordTyping(caret.line, caret.pos, ch);
		UpdateView(caret.line, caret.pos);
		if (move_caret)
		{
			caret.pos++;
			UpdateCaret();
			SetSelectionToCaret(start: true);
		}
	}

	internal void InsertPicture(Line line, int pos, Picture picture)
	{
		int num = 1;
		line.text.Insert(pos, "I");
		PictureTag pictureTag = new PictureTag(line, pos + 1, picture);
		LineTag lineTag = LineTag.FindTag(line, pos);
		pictureTag.CopyFormattingFrom(lineTag);
		lineTag.Break(pos + 1);
		pictureTag.Previous = lineTag;
		pictureTag.Next = lineTag.Next;
		lineTag.Next = pictureTag;
		if (pictureTag.Next == null)
		{
			pictureTag.Next = new LineTag(line, pos + 1);
			pictureTag.Next.CopyFormattingFrom(lineTag);
			pictureTag.Next.Previous = pictureTag;
		}
		for (lineTag = pictureTag.Next; lineTag != null; lineTag = lineTag.Next)
		{
			lineTag.Start += num;
		}
		line.Grow(num);
		line.recalc = true;
		UpdateView(line, pos);
	}

	internal void DeleteMultiline(Line start_line, int pos, int length)
	{
		Marker marker = default(Marker);
		Marker marker2 = default(Marker);
		int num = LineTagToCharIndex(start_line, pos);
		marker.line = start_line;
		marker.pos = pos;
		marker.tag = LineTag.FindTag(start_line, pos);
		CharIndexToLineTag(num + length, out marker2.line, out marker2.tag, out marker2.pos);
		SuspendUpdate();
		if (marker.line == marker2.line)
		{
			DeleteChars(marker.line, pos, marker2.pos - pos);
		}
		else
		{
			DeleteChars(marker.line, marker.pos, marker.line.text.Length - marker.pos);
			DeleteChars(marker2.line, 0, marker2.pos);
			int num2 = marker.line.line_no + 1;
			if (num2 < marker2.line.line_no)
			{
				for (int num3 = marker2.line.line_no - 1; num3 >= num2; num3--)
				{
					Delete(num3);
				}
			}
			Combine(marker.line.line_no, num2);
		}
		ResumeUpdate(immediate_update: true);
	}

	public void DeleteChars(Line line, int pos, int count)
	{
		CharCount -= count;
		line.DeleteCharacters(pos, count);
		if (pos >= line.TextLengthWithoutEnding())
		{
			LineEnding ending = line.ending;
			GetLineEnding(line.text.ToString(), 0, out ending);
			if (ending != line.ending)
			{
				line.ending = ending;
				if (!multiline)
				{
					UpdateView(line, lines, pos);
					owner.Invalidate();
					return;
				}
			}
		}
		if (!multiline)
		{
			UpdateView(line, lines, pos);
			owner.Invalidate();
		}
		else
		{
			UpdateView(line, pos);
		}
	}

	public void DeleteChar(Line line, int pos, bool forward)
	{
		if ((pos != 0 || forward) && (pos != line.text.Length || !forward))
		{
			undo.BeginUserAction("Delete");
			if (forward)
			{
				undo.RecordDeleteString(line, pos, line, pos + 1);
				DeleteChars(line, pos, 1);
			}
			else
			{
				undo.RecordDeleteString(line, pos - 1, line, pos);
				DeleteChars(line, pos - 1, 1);
			}
			undo.EndUserAction();
		}
	}

	internal void Combine(int FirstLine, int SecondLine)
	{
		Combine(GetLine(FirstLine), GetLine(SecondLine));
	}

	internal void Combine(Line first, Line second)
	{
		first.text.Length = first.text.Length - LineEndingLength(first.ending);
		LineTag lineTag = first.tags;
		first.ending = second.ending;
		while (lineTag.Next != null)
		{
			lineTag = lineTag.Next;
		}
		int num = lineTag.Start + lineTag.Length - 1;
		lineTag.Next = second.tags;
		lineTag.Next.Previous = lineTag;
		for (lineTag = lineTag.Next; lineTag != null; lineTag = lineTag.Next)
		{
			lineTag.Line = first;
			lineTag.Start += num;
		}
		first.text.Insert(first.text.Length, second.text.ToString());
		first.Grow(first.text.Length);
		second.tags = null;
		DecrementLines(first.line_no + 2);
		first.recalc = true;
		first.height = 0;
		first.Streamline(lines);
		if (caret.line == second)
		{
			caret.Combine(first, num);
		}
		if (selection_anchor.line == second)
		{
			selection_anchor.Combine(first, num);
		}
		if (selection_start.line == second)
		{
			selection_start.Combine(first, num);
		}
		if (selection_end.line == second)
		{
			selection_end.Combine(first, num);
		}
		Delete(second);
	}

	internal void Split(int LineNo, int pos)
	{
		Line line = GetLine(LineNo);
		LineTag tag = LineTag.FindTag(line, pos);
		Split(line, tag, pos);
	}

	internal void Split(Line line, int pos)
	{
		LineTag tag = LineTag.FindTag(line, pos);
		Split(line, tag, pos);
	}

	internal void Split(Line line, LineTag tag, int pos)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (caret.line == line && caret.pos >= pos)
		{
			flag = true;
		}
		if (selection_start.line == line && selection_start.pos > pos)
		{
			flag2 = true;
		}
		if (selection_end.line == line && selection_end.pos > pos)
		{
			flag3 = true;
		}
		Line line2;
		if (pos == line.text.Length)
		{
			Add(line.line_no + 1, string.Empty, line.alignment, tag.Font, tag.Color, line.ending);
			line2 = GetLine(line.line_no + 1);
			if (flag)
			{
				caret.line = line2;
				caret.tag = line2.tags;
				caret.pos = 0;
				if (!selection_visible)
				{
					SetSelectionToCaret(start: true);
				}
			}
			if (flag2)
			{
				selection_start.line = line2;
				selection_start.pos = 0;
				selection_start.tag = line2.tags;
			}
			if (flag3)
			{
				selection_end.line = line2;
				selection_end.pos = 0;
				selection_end.tag = line2.tags;
			}
			return;
		}
		Add(line.line_no + 1, line.text.ToString(pos, line.text.Length - pos), line.alignment, tag.Font, tag.Color, line.ending);
		line2 = GetLine(line.line_no + 1);
		line.recalc = true;
		line2.recalc = true;
		if (tag.Next != null && tag.Next.Start - 1 == pos)
		{
			tag = tag.Next;
		}
		if (tag.Start - 1 == pos)
		{
			if (tag == line.tags)
			{
				LineTag lineTag = new LineTag(line, 1);
				lineTag.CopyFormattingFrom(tag);
				line.tags = lineTag;
			}
			if (tag.Previous != null)
			{
				tag.Previous.Next = null;
			}
			line2.tags = tag;
			tag.Previous = null;
			tag.Line = line2;
			int num = tag.Start - 1;
			for (LineTag lineTag = tag; lineTag != null; lineTag = lineTag.Next)
			{
				lineTag.Start -= num;
				lineTag.Line = line2;
			}
		}
		else
		{
			LineTag lineTag = new LineTag(line2, 1);
			lineTag.Next = tag.Next;
			lineTag.CopyFormattingFrom(tag);
			line2.tags = lineTag;
			if (lineTag.Next != null)
			{
				lineTag.Next.Previous = lineTag;
			}
			tag.Next = null;
			for (lineTag = lineTag.Next; lineTag != null; lineTag = lineTag.Next)
			{
				lineTag.Start -= pos;
				lineTag.Line = line2;
			}
		}
		if (flag)
		{
			caret.line = line2;
			caret.pos -= pos;
			caret.tag = caret.line.FindTag(caret.pos);
			if (!selection_visible)
			{
				SetSelectionToCaret(start: true);
			}
		}
		if (flag2)
		{
			selection_start.line = line2;
			selection_start.pos -= pos;
			if (selection_start.Equals(selection_end))
			{
				selection_start.tag = line2.FindTag(selection_start.pos);
			}
			else
			{
				selection_start.tag = line2.FindTag(selection_start.pos + 1);
			}
		}
		if (flag3)
		{
			selection_end.line = line2;
			selection_end.pos -= pos;
			selection_end.tag = line2.FindTag(selection_end.pos);
		}
		CharCount -= line.text.Length - pos;
		line.text.Remove(pos, line.text.Length - pos);
	}

	internal void Add(int LineNo, string Text, System.Drawing.Font font, System.Drawing.Color color, LineEnding ending)
	{
		Add(LineNo, Text, alignment, font, color, ending);
	}

	internal void Add(int LineNo, string Text, HorizontalAlignment align, System.Drawing.Font font, System.Drawing.Color color, LineEnding ending)
	{
		CharCount += Text.Length;
		if (LineNo < 1 || Text == null)
		{
			if (LineNo < 1)
			{
				throw new ArgumentNullException("LineNo", "Line numbers must be positive");
			}
			throw new ArgumentNullException("Text", "Cannot insert NULL line");
		}
		Line line = new Line(this, LineNo, Text, align, font, color, ending);
		Line line2 = document;
		while (line2 != sentinel)
		{
			line.parent = line2;
			int line_no = line2.line_no;
			if (LineNo > line_no)
			{
				line2 = line2.right;
				continue;
			}
			if (LineNo < line_no)
			{
				line2 = line2.left;
				continue;
			}
			IncrementLines(line2.line_no);
			line2 = line2.left;
		}
		line.left = sentinel;
		line.right = sentinel;
		if (line.parent != null)
		{
			if (LineNo > line.parent.line_no)
			{
				line.parent.right = line;
			}
			else
			{
				line.parent.left = line;
			}
		}
		else
		{
			document = line;
		}
		RebalanceAfterAdd(line);
		lines++;
	}

	internal virtual void Clear()
	{
		lines = 0;
		CharCount = 0;
		document = sentinel;
	}

	public virtual object Clone()
	{
		Document document = new Document(null);
		document.lines = lines;
		document.document = (Line)this.document.Clone();
		return document;
	}

	private void Delete(int LineNo)
	{
		if (LineNo <= lines)
		{
			Line line = GetLine(LineNo);
			CharCount -= line.text.Length;
			DecrementLines(LineNo + 1);
			Delete(line);
		}
	}

	private void Delete(Line line1)
	{
		Line line2;
		if (line1.left == sentinel || line1.right == sentinel)
		{
			line2 = line1;
		}
		else
		{
			line2 = line1.right;
			while (line2.left != sentinel)
			{
				line2 = line2.left;
			}
		}
		Line line3 = ((line2.left == sentinel) ? line2.right : line2.left);
		line3.parent = line2.parent;
		if (line2.parent != null)
		{
			if (line2 == line2.parent.left)
			{
				line2.parent.left = line3;
			}
			else
			{
				line2.parent.right = line3;
			}
		}
		else
		{
			document = line3;
		}
		if (line2 != line1)
		{
			if (selection_start.line == line2)
			{
				selection_start.line = line1;
			}
			if (selection_end.line == line2)
			{
				selection_end.line = line1;
			}
			if (selection_anchor.line == line2)
			{
				selection_anchor.line = line1;
			}
			if (caret.line == line2)
			{
				caret.line = line1;
			}
			line1.alignment = line2.alignment;
			line1.ascent = line2.ascent;
			line1.hanging_indent = line2.hanging_indent;
			line1.height = line2.height;
			line1.indent = line2.indent;
			line1.line_no = line2.line_no;
			line1.recalc = line2.recalc;
			line1.right_indent = line2.right_indent;
			line1.ending = line2.ending;
			line1.space = line2.space;
			line1.tags = line2.tags;
			line1.text = line2.text;
			line1.widths = line2.widths;
			line1.offset = line2.offset;
			for (LineTag lineTag = line1.tags; lineTag != null; lineTag = lineTag.Next)
			{
				lineTag.Line = line1;
			}
		}
		if (line2.color == LineColor.Black)
		{
			RebalanceAfterDelete(line3);
		}
		lines--;
	}

	internal void InvalidateLinesAfter(Line start)
	{
		owner.Invalidate(new Rectangle(0, start.Y - viewport_y, viewport_width, viewport_height - start.Y));
	}

	internal void Invalidate(Line start, int start_pos, Line end, int end_pos)
	{
		if (start == end && start_pos == end_pos)
		{
			return;
		}
		if (end_pos == -1)
		{
			end_pos = end.text.Length;
		}
		Line line;
		int num;
		int num2;
		Line line2;
		if (start.line_no < end.line_no)
		{
			line = start;
			num = start_pos;
			line2 = end;
			num2 = end_pos;
		}
		else
		{
			if (start.line_no <= end.line_no)
			{
				if (start_pos < end_pos)
				{
					line = start;
					num = start_pos;
					line2 = end;
					num2 = end_pos;
				}
				else
				{
					line = end;
					num = end_pos;
					line2 = start;
					num2 = start_pos;
				}
				int num3 = (int)line.widths[num2];
				if (num2 == line.text.Length + 1)
				{
					num3 = viewport_width;
				}
				owner.Invalidate(new Rectangle(offset_x + (int)line.widths[num] + line.X - viewport_x, offset_y + line.Y - viewport_y, num3 - (int)line.widths[num] + 1, line.height));
				return;
			}
			line = end;
			num = end_pos;
			line2 = start;
			num2 = start_pos;
		}
		owner.Invalidate(new Rectangle(offset_x + (int)line.widths[num] + line.X - viewport_x, offset_y + line.Y - viewport_y, viewport_width, line.height));
		if (line.line_no + 1 < line2.line_no)
		{
			int y = GetLine(line.line_no + 1).Y;
			owner.Invalidate(new Rectangle(offset_x, offset_y + y - viewport_y, viewport_width, line2.Y - y));
		}
		owner.Invalidate(new Rectangle(offset_x + (int)line2.widths[0] + line2.X - viewport_x, offset_y + line2.Y - viewport_y, (int)line2.widths[num2] + 1, line2.height));
	}

	internal void ExpandSelection(CaretSelection mode, bool to_caret)
	{
		if (to_caret)
		{
			switch (mode)
			{
			case CaretSelection.Line:
				if (caret > selection_prev)
				{
					Invalidate(selection_prev.line, 0, caret.line, caret.line.text.Length);
				}
				else
				{
					Invalidate(selection_prev.line, selection_prev.line.text.Length, caret.line, 0);
				}
				if (caret.line.line_no <= selection_anchor.line.line_no)
				{
					selection_start.line = caret.line;
					selection_start.tag = caret.line.tags;
					selection_start.pos = 0;
					selection_end.line = selection_anchor.line;
					selection_end.tag = selection_anchor.tag;
					selection_end.pos = selection_anchor.pos;
					selection_end_anchor = true;
				}
				else
				{
					selection_start.line = selection_anchor.line;
					selection_start.pos = selection_anchor.height;
					selection_start.tag = selection_anchor.line.FindTag(selection_anchor.height + 1);
					selection_end.line = caret.line;
					selection_end.tag = caret.line.tags;
					selection_end.pos = caret.line.text.Length;
					selection_end_anchor = false;
				}
				selection_prev.line = caret.line;
				selection_prev.tag = caret.tag;
				selection_prev.pos = caret.pos;
				break;
			case CaretSelection.Word:
			{
				int num = FindWordSeparator(caret.line, caret.pos, forward: false);
				int num2 = FindWordSeparator(caret.line, caret.pos, forward: true);
				if (caret > selection_prev)
				{
					Invalidate(selection_prev.line, selection_prev.pos, caret.line, num2);
				}
				else
				{
					Invalidate(selection_prev.line, selection_prev.pos, caret.line, num);
				}
				if (caret < selection_anchor)
				{
					selection_start.line = caret.line;
					selection_start.tag = caret.line.FindTag(num + 1);
					selection_start.pos = num;
					selection_end.line = selection_anchor.line;
					selection_end.tag = selection_anchor.tag;
					selection_end.pos = selection_anchor.pos;
					selection_prev.line = caret.line;
					selection_prev.tag = caret.tag;
					selection_prev.pos = num;
					selection_end_anchor = true;
				}
				else
				{
					selection_start.line = selection_anchor.line;
					selection_start.pos = selection_anchor.height;
					selection_start.tag = selection_anchor.line.FindTag(selection_anchor.height + 1);
					selection_end.line = caret.line;
					selection_end.tag = caret.line.FindTag(num2);
					selection_end.pos = num2;
					selection_prev.line = caret.line;
					selection_prev.tag = caret.tag;
					selection_prev.pos = num2;
					selection_end_anchor = false;
				}
				break;
			}
			case CaretSelection.Position:
				SetSelectionToCaret(start: false);
				return;
			}
		}
		else
		{
			switch (mode)
			{
			case CaretSelection.Line:
				Invalidate(caret.line, 0, caret.line, caret.line.text.Length);
				selection_start.line = caret.line;
				selection_start.tag = caret.line.tags;
				selection_start.pos = 0;
				selection_end.line = caret.line;
				selection_end.pos = caret.line.text.Length;
				selection_end.tag = caret.line.FindTag(selection_end.pos);
				selection_anchor.line = selection_end.line;
				selection_anchor.tag = selection_end.tag;
				selection_anchor.pos = selection_end.pos;
				selection_anchor.height = 0;
				selection_prev.line = caret.line;
				selection_prev.tag = caret.tag;
				selection_prev.pos = caret.pos;
				selection_end_anchor = true;
				break;
			case CaretSelection.Word:
			{
				int num3 = FindWordSeparator(caret.line, caret.pos, forward: false);
				int num4 = FindWordSeparator(caret.line, caret.pos, forward: true);
				Invalidate(selection_start.line, num3, caret.line, num4);
				selection_start.line = caret.line;
				selection_start.tag = caret.line.FindTag(num3 + 1);
				selection_start.pos = num3;
				selection_end.line = caret.line;
				selection_end.tag = caret.line.FindTag(num4);
				selection_end.pos = num4;
				selection_anchor.line = selection_end.line;
				selection_anchor.tag = selection_end.tag;
				selection_anchor.pos = selection_end.pos;
				selection_anchor.height = num3;
				selection_prev.line = caret.line;
				selection_prev.tag = caret.tag;
				selection_prev.pos = caret.pos;
				selection_end_anchor = true;
				break;
			}
			}
		}
		SetSelectionVisible(!(selection_start == selection_end));
	}

	internal void SetSelectionToCaret(bool start)
	{
		if (start)
		{
			Invalidate(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
			selection_start.line = caret.line;
			selection_start.tag = caret.tag;
			selection_start.pos = caret.pos;
			selection_end.line = caret.line;
			selection_end.tag = caret.tag;
			selection_end.pos = caret.pos;
			selection_anchor.line = caret.line;
			selection_anchor.tag = caret.tag;
			selection_anchor.pos = caret.pos;
		}
		else
		{
			if (selection_end_anchor)
			{
				if (selection_start != caret)
				{
					Invalidate(selection_start.line, selection_start.pos, caret.line, caret.pos);
				}
			}
			else if (selection_end != caret)
			{
				Invalidate(selection_end.line, selection_end.pos, caret.line, caret.pos);
			}
			if (caret < selection_anchor)
			{
				selection_start.line = caret.line;
				selection_start.tag = caret.tag;
				selection_start.pos = caret.pos;
				selection_end.line = selection_anchor.line;
				selection_end.tag = selection_anchor.tag;
				selection_end.pos = selection_anchor.pos;
				selection_end_anchor = true;
			}
			else
			{
				selection_start.line = selection_anchor.line;
				selection_start.tag = selection_anchor.tag;
				selection_start.pos = selection_anchor.pos;
				selection_end.line = caret.line;
				selection_end.tag = caret.tag;
				selection_end.pos = caret.pos;
				selection_end_anchor = false;
			}
		}
		SetSelectionVisible(!(selection_start == selection_end));
	}

	internal void SetSelection(Line start, int start_pos, Line end, int end_pos)
	{
		if (selection_visible)
		{
			Invalidate(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
		}
		if (end.line_no < start.line_no || (end == start && end_pos <= start_pos))
		{
			selection_start.line = end;
			selection_start.tag = LineTag.FindTag(end, end_pos);
			selection_start.pos = end_pos;
			selection_end.line = start;
			selection_end.tag = LineTag.FindTag(start, start_pos);
			selection_end.pos = start_pos;
			selection_end_anchor = true;
		}
		else
		{
			selection_start.line = start;
			selection_start.tag = LineTag.FindTag(start, start_pos);
			selection_start.pos = start_pos;
			selection_end.line = end;
			selection_end.tag = LineTag.FindTag(end, end_pos);
			selection_end.pos = end_pos;
			selection_end_anchor = false;
		}
		selection_anchor.line = start;
		selection_anchor.tag = selection_start.tag;
		selection_anchor.pos = start_pos;
		if ((start == end && start_pos == end_pos) || start == null || end == null)
		{
			SetSelectionVisible(value: false);
			return;
		}
		SetSelectionVisible(value: true);
		Invalidate(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
	}

	internal void SetSelectionStart(Line start, int start_pos, bool invalidate)
	{
		if (invalidate)
		{
			Invalidate(selection_start.line, selection_start.pos, start, start_pos);
		}
		selection_start.line = start;
		selection_start.pos = start_pos;
		selection_start.tag = LineTag.FindTag(start, start_pos);
		selection_anchor.line = start;
		selection_anchor.pos = start_pos;
		selection_anchor.tag = selection_start.tag;
		selection_end_anchor = false;
		if (selection_end.line != selection_start.line || selection_end.pos != selection_start.pos)
		{
			SetSelectionVisible(value: true);
		}
		else
		{
			SetSelectionVisible(value: false);
		}
		if (invalidate)
		{
			Invalidate(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
		}
	}

	internal void SetSelectionStart(int character_index, bool invalidate)
	{
		if (character_index >= 0)
		{
			CharIndexToLineTag(character_index, out var line_out, out var _, out var pos);
			SetSelectionStart(line_out, pos, invalidate);
		}
	}

	internal void SetSelectionEnd(Line end, int end_pos, bool invalidate)
	{
		if (end == selection_end.line && end_pos == selection_start.pos)
		{
			selection_anchor.line = selection_start.line;
			selection_anchor.tag = selection_start.tag;
			selection_anchor.pos = selection_start.pos;
			selection_end.line = selection_start.line;
			selection_end.tag = selection_start.tag;
			selection_end.pos = selection_start.pos;
			selection_end_anchor = false;
		}
		else if (end.line_no < selection_anchor.line.line_no || (end == selection_anchor.line && end_pos <= selection_anchor.pos))
		{
			selection_start.line = end;
			selection_start.tag = LineTag.FindTag(end, end_pos);
			selection_start.pos = end_pos;
			selection_end.line = selection_anchor.line;
			selection_end.tag = selection_anchor.tag;
			selection_end.pos = selection_anchor.pos;
			selection_end_anchor = true;
		}
		else
		{
			selection_start.line = selection_anchor.line;
			selection_start.tag = selection_anchor.tag;
			selection_start.pos = selection_anchor.pos;
			selection_end.line = end;
			selection_end.tag = LineTag.FindTag(end, end_pos);
			selection_end.pos = end_pos;
			selection_end_anchor = false;
		}
		if (selection_end.line != selection_start.line || selection_end.pos != selection_start.pos)
		{
			SetSelectionVisible(value: true);
			if (invalidate)
			{
				Invalidate(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
			}
		}
		else
		{
			SetSelectionVisible(value: false);
		}
	}

	internal void SetSelectionEnd(int character_index, bool invalidate)
	{
		if (character_index >= 0)
		{
			CharIndexToLineTag(character_index, out var line_out, out var _, out var pos);
			SetSelectionEnd(line_out, pos, invalidate);
		}
	}

	internal void SetSelection(Line start, int start_pos)
	{
		if (selection_visible)
		{
			Invalidate(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
		}
		selection_start.line = start;
		selection_start.pos = start_pos;
		selection_start.tag = LineTag.FindTag(start, start_pos);
		selection_end.line = start;
		selection_end.tag = selection_start.tag;
		selection_end.pos = start_pos;
		selection_anchor.line = start;
		selection_anchor.tag = selection_start.tag;
		selection_anchor.pos = start_pos;
		selection_end_anchor = false;
		SetSelectionVisible(value: false);
	}

	internal void InvalidateSelectionArea()
	{
		Invalidate(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
	}

	internal string GetSelection()
	{
		if (selection_start.pos == selection_end.pos && selection_start.line == selection_end.line)
		{
			return string.Empty;
		}
		if (selection_start.line == selection_end.line)
		{
			return selection_start.line.text.ToString(selection_start.pos, selection_end.pos - selection_start.pos);
		}
		StringBuilder stringBuilder = new StringBuilder();
		int line_no = selection_start.line.line_no;
		int line_no2 = selection_end.line.line_no;
		stringBuilder.Append(selection_start.line.text.ToString(selection_start.pos, selection_start.line.text.Length - selection_start.pos));
		if (line_no + 1 < line_no2)
		{
			for (int i = line_no + 1; i < line_no2; i++)
			{
				stringBuilder.Append(GetLine(i).text.ToString());
			}
		}
		stringBuilder.Append(selection_end.line.text.ToString(0, selection_end.pos));
		return stringBuilder.ToString();
	}

	internal void ReplaceSelection(string s, bool select_new)
	{
		int num = LineTagToCharIndex(selection_start.line, selection_start.pos);
		SuspendRecalc();
		if (selection_start.pos != selection_end.pos || selection_start.line != selection_end.line)
		{
			if (selection_start.line == selection_end.line)
			{
				undo.RecordDeleteString(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
				DeleteChars(selection_start.line, selection_start.pos, selection_end.pos - selection_start.pos);
				selection_start.tag = selection_start.line.FindTag(selection_start.pos + 1);
			}
			else
			{
				int line_no = selection_start.line.line_no;
				int line_no2 = selection_end.line.line_no;
				undo.RecordDeleteString(selection_start.line, selection_start.pos, selection_end.line, selection_end.pos);
				InvalidateLinesAfter(selection_start.line);
				DeleteChars(selection_start.line, selection_start.pos, selection_start.line.text.Length - selection_start.pos);
				selection_start.line.recalc = true;
				DeleteChars(selection_end.line, 0, selection_end.pos);
				line_no++;
				if (line_no < line_no2)
				{
					for (int num2 = line_no2 - 1; num2 >= line_no; num2--)
					{
						Delete(num2);
					}
				}
				Combine(selection_start.line.line_no, line_no);
			}
		}
		Insert(selection_start.line, selection_start.pos, update_caret: false, s);
		undo.RecordInsertString(selection_start.line, selection_start.pos, s);
		ResumeRecalc(immediate_update: false);
		Line line = selection_start.line;
		int pos = selection_start.pos;
		if (!select_new)
		{
			CharIndexToLineTag(num + s.Length, out selection_start.line, out selection_start.tag, out selection_start.pos);
			selection_end.line = selection_start.line;
			selection_end.pos = selection_start.pos;
			selection_end.tag = selection_start.tag;
			selection_anchor.line = selection_start.line;
			selection_anchor.pos = selection_start.pos;
			selection_anchor.tag = selection_start.tag;
			SetSelectionVisible(value: false);
		}
		else
		{
			CharIndexToLineTag(num, out selection_start.line, out selection_start.tag, out selection_start.pos);
			CharIndexToLineTag(num + s.Length, out selection_end.line, out selection_end.tag, out selection_end.pos);
			selection_anchor.line = selection_start.line;
			selection_anchor.pos = selection_start.pos;
			selection_anchor.tag = selection_start.tag;
			SetSelectionVisible(value: true);
		}
		PositionCaret(selection_start.line, selection_start.pos);
		UpdateView(line, selection_end.line.line_no - line.line_no, pos);
	}

	internal void CharIndexToLineTag(int index, out Line line_out, out LineTag tag_out, out int pos)
	{
		int num = 0;
		LineTag lineTag;
		for (int i = 1; i <= lines; i++)
		{
			Line line = GetLine(i);
			int num2 = num;
			num += line.text.Length;
			if (index > num)
			{
				continue;
			}
			for (lineTag = line.tags; lineTag != null; lineTag = lineTag.Next)
			{
				if (index < num2 + lineTag.Start + lineTag.Length - 1)
				{
					line_out = line;
					tag_out = LineTag.GetFinalTag(lineTag);
					pos = index - num2;
					return;
				}
				if (lineTag.Next == null)
				{
					Line line2 = GetLine(line.line_no + 1);
					if (line2 != null)
					{
						line_out = line2;
						tag_out = LineTag.GetFinalTag(line2.tags);
						pos = 0;
					}
					else
					{
						line_out = line;
						tag_out = LineTag.GetFinalTag(lineTag);
						pos = line_out.text.Length;
					}
					return;
				}
			}
		}
		line_out = GetLine(lines);
		lineTag = line_out.tags;
		while (lineTag.Next != null)
		{
			lineTag = lineTag.Next;
		}
		tag_out = lineTag;
		pos = line_out.text.Length;
	}

	internal int LineTagToCharIndex(Line line, int pos)
	{
		int num = 0;
		for (int i = 1; i < line.line_no; i++)
		{
			num += GetLine(i).text.Length;
		}
		return num + pos;
	}

	internal int SelectionLength()
	{
		if (selection_start.pos == selection_end.pos && selection_start.line == selection_end.line)
		{
			return 0;
		}
		if (selection_start.line == selection_end.line)
		{
			return selection_end.pos - selection_start.pos;
		}
		int num = selection_start.line.text.Length - selection_start.pos + selection_end.pos + crlf_size;
		int num2 = selection_start.line.line_no + 1;
		int line_no = selection_end.line.line_no;
		if (num2 < line_no)
		{
			for (int i = num2; i < line_no; i++)
			{
				Line line = GetLine(i);
				num += line.text.Length + LineEndingLength(line.ending);
			}
		}
		return num;
	}

	internal Line GetLine(int LineNo)
	{
		for (Line line = document; line != sentinel; line = ((LineNo >= line.line_no) ? line.right : line.left))
		{
			if (LineNo == line.line_no)
			{
				return line;
			}
		}
		return null;
	}

	internal LineTag PreviousTag(LineTag tag)
	{
		if (tag.Previous != null)
		{
			return tag.Previous;
		}
		if (tag.Line.line_no == 1)
		{
			return null;
		}
		Line line = GetLine(tag.Line.line_no - 1);
		if (line != null)
		{
			LineTag lineTag = line.tags;
			while (lineTag.Next != null)
			{
				lineTag = lineTag.Next;
			}
			return lineTag;
		}
		return null;
	}

	internal LineTag NextTag(LineTag tag)
	{
		if (tag.Next != null)
		{
			return tag.Next;
		}
		return GetLine(tag.Line.line_no + 1)?.tags;
	}

	internal Line ParagraphStart(Line line)
	{
		Line line2 = line;
		while (line.line_no > 1)
		{
			line = line2;
			line2 = GetLine(line.line_no - 1);
			if (line2.ending != LineEnding.Wrap)
			{
				break;
			}
		}
		return line;
	}

	internal Line ParagraphEnd(Line line)
	{
		while (line.ending == LineEnding.Wrap)
		{
			Line line2 = GetLine(line.line_no + 1);
			if (line2 == null || line2.ending != LineEnding.Wrap)
			{
				break;
			}
			line = line2;
		}
		return line;
	}

	internal Line GetLineByPixel(int offset, bool exact)
	{
		Line line = document;
		Line result = null;
		if (multiline)
		{
			while (line != sentinel)
			{
				result = line;
				if (offset >= line.Y && offset < line.Y + line.height)
				{
					return line;
				}
				line = ((offset >= line.Y) ? line.right : line.left);
			}
		}
		else
		{
			while (line != sentinel)
			{
				result = line;
				if (offset >= line.X && offset < line.X + line.Width)
				{
					return line;
				}
				line = ((offset >= line.X) ? line.right : line.left);
			}
		}
		if (exact)
		{
			return null;
		}
		return result;
	}

	internal LineTag FindCursor(int x, int y, out int index)
	{
		x -= offset_x;
		y -= offset_y;
		Line lineByPixel = GetLineByPixel((!multiline) ? x : y, exact: false);
		LineTag tag = lineByPixel.GetTag(x);
		if (tag.Length == 0 && tag.Start == 1)
		{
			index = 0;
		}
		else
		{
			index = tag.GetCharIndex(x - lineByPixel.align_shift);
		}
		return tag;
	}

	internal void FormatText(Line start_line, int start_pos, Line end_line, int end_pos, System.Drawing.Font font, System.Drawing.Color color, System.Drawing.Color back_color, FormatSpecified specified)
	{
		if (start_line != end_line)
		{
			LineTag.FormatText(start_line, start_pos, start_line.text.Length - start_pos + 1, font, color, back_color, specified);
			LineTag.FormatText(end_line, 1, end_pos, font, color, back_color, specified);
			for (int i = start_line.line_no + 1; i < end_line.line_no; i++)
			{
				Line line = GetLine(i);
				LineTag.FormatText(line, 1, line.text.Length, font, color, back_color, specified);
			}
		}
		else
		{
			LineTag.FormatText(start_line, start_pos, end_pos - start_pos, font, color, back_color, specified);
			if (end_pos - start_pos == 0 && CaretTag.Length != 0)
			{
				CaretTag = CaretTag.Next;
			}
		}
	}

	internal void RecalculateAlignments()
	{
		for (int i = 1; i <= lines; i++)
		{
			Line line = GetLine(i);
			if (line != null)
			{
				switch (line.alignment)
				{
				case HorizontalAlignment.Left:
					line.align_shift = 0;
					break;
				case HorizontalAlignment.Center:
					line.align_shift = (viewport_width - (int)line.widths[line.text.Length]) / 2;
					break;
				case HorizontalAlignment.Right:
					line.align_shift = viewport_width - (int)line.widths[line.text.Length] - right_margin;
					break;
				}
			}
		}
	}

	internal bool RecalculateDocument(Graphics g)
	{
		return RecalculateDocument(g, 1, lines, optimize: false);
	}

	internal bool RecalculateDocument(Graphics g, int start)
	{
		return RecalculateDocument(g, start, lines, optimize: false);
	}

	internal bool RecalculateDocument(Graphics g, int start, int end)
	{
		return RecalculateDocument(g, start, end, optimize: false);
	}

	internal bool RecalculateDocument(Graphics g, int start, int end, bool optimize)
	{
		if (recalc_suspended > 0)
		{
			recalc_pending = true;
			recalc_start = Math.Min(recalc_start, start);
			recalc_end = Math.Max(recalc_end, end);
			recalc_optimize = optimize;
			return false;
		}
		start = Math.Max(start, 1);
		end = Math.Min(end, lines);
		int num = GetLine(start).offset;
		int num2 = start;
		int num3 = 0;
		int num4 = lines;
		bool link_changed = ((!optimize) ? true : false);
		Line line;
		while (num2 <= end + lines - num4)
		{
			line = GetLine(num2++);
			line.offset = num;
			if (!calc_pass)
			{
				if (!optimize)
				{
					line.RecalculateLine(g, this);
				}
				else if (line.recalc && line.RecalculateLine(g, this))
				{
					link_changed = true;
					end = lines;
					num4 = lines;
				}
			}
			else if (!optimize)
			{
				line.RecalculatePasswordLine(g, this);
			}
			else if (line.recalc && line.RecalculatePasswordLine(g, this))
			{
				link_changed = true;
				end = lines;
				num4 = lines;
			}
			if (line.widths[line.text.Length] > (float)num3)
			{
				num3 = (int)line.widths[line.text.Length];
			}
			if (line.alignment != 0)
			{
				if (line.alignment == HorizontalAlignment.Center)
				{
					line.align_shift = (viewport_width - (int)line.widths[line.text.Length]) / 2;
				}
				else
				{
					line.align_shift = viewport_width - (int)line.widths[line.text.Length] - 1;
				}
			}
			num = ((!multiline) ? (num + (int)line.widths[line.text.Length]) : (num + line.height));
			if (num2 > lines)
			{
				break;
			}
		}
		if (document_x != num3)
		{
			document_x = num3;
			if (this.WidthChanged != null)
			{
				this.WidthChanged(this, null);
			}
		}
		RecalculateAlignments();
		line = GetLine(lines);
		if (document_y != line.Y + line.height)
		{
			document_y = line.Y + line.height;
			if (this.HeightChanged != null)
			{
				this.HeightChanged(this, null);
			}
		}
		if (EnableLinks)
		{
			ScanForLinks(start, end, ref link_changed);
		}
		UpdateCaret();
		return link_changed;
	}

	internal int Size()
	{
		return lines;
	}

	private void owner_HandleCreated(object sender, EventArgs e)
	{
		RecalculateDocument(owner.CreateGraphicsInternal());
		AlignCaret();
	}

	private void owner_VisibleChanged(object sender, EventArgs e)
	{
		if (owner.Visible)
		{
			RecalculateDocument(owner.CreateGraphicsInternal());
		}
	}

	internal static bool IsWordSeparator(char ch)
	{
		switch (ch)
		{
		case '\t':
		case '\n':
		case '\r':
		case ' ':
		case '(':
		case ')':
			return true;
		default:
			return false;
		}
	}

	internal int FindWordSeparator(Line line, int pos, bool forward)
	{
		int length = line.text.Length;
		if (forward)
		{
			for (int i = pos + 1; i < length; i++)
			{
				if (IsWordSeparator(line.Text[i]))
				{
					return i + 1;
				}
			}
			return length;
		}
		for (int num = pos - 1; num > 0; num--)
		{
			if (IsWordSeparator(line.Text[num - 1]))
			{
				return num;
			}
		}
		return 0;
	}

	internal bool FindChars(char[] chars, Marker start, Marker end, out Marker result)
	{
		result = default(Marker);
		Line line = start.line;
		int num = start.line.line_no;
		int i = start.pos;
		while (num <= end.line.line_no)
		{
			for (int length = line.text.Length; i < length; i++)
			{
				for (int j = 0; j < chars.Length; j++)
				{
					if (line.text[i] == chars[j])
					{
						if (line.line_no == end.line.line_no && i >= end.pos)
						{
							return false;
						}
						result.line = line;
						result.pos = i;
						return true;
					}
				}
			}
			i = 0;
			num++;
			line = GetLine(num);
		}
		return false;
	}

	internal bool Find(string search, Marker start, Marker end, out Marker result, RichTextBoxFinds options)
	{
		result = default(Marker);
		bool flag = (options & RichTextBoxFinds.WholeWord) != 0;
		bool flag2 = (options & RichTextBoxFinds.MatchCase) == 0;
		bool flag3 = (options & RichTextBoxFinds.Reverse) != 0;
		Line line = start.line;
		int num = start.line.line_no;
		int num2 = start.pos;
		int num3 = 0;
		string text;
		if (flag2)
		{
			StringBuilder stringBuilder = new StringBuilder(search);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				stringBuilder[i] = char.ToLower(stringBuilder[i]);
			}
			text = stringBuilder.ToString();
		}
		else
		{
			text = search;
		}
		bool flag4;
		if (flag)
		{
			if (num == 1)
			{
				flag4 = ((num2 == 0 || IsWordSeparator(line.text[num2 - 1])) ? true : false);
			}
			else if (num2 > 0)
			{
				flag4 = (IsWordSeparator(line.text[num2 - 1]) ? true : false);
			}
			else
			{
				Line line2 = GetLine(num - 1);
				flag4 = line2.ending != LineEnding.Wrap || (IsWordSeparator(line2.text[line2.text.Length - 1]) ? true : false);
			}
		}
		else
		{
			flag4 = false;
		}
		Marker marker = default(Marker);
		marker.height = -1;
		while (num <= end.line.line_no)
		{
			int num4 = ((num == end.line.line_no) ? end.pos : line.text.Length);
			while (true)
			{
				if (num2 < num4)
				{
					if (flag && num3 == text.Length)
					{
						if (IsWordSeparator(line.text[num2]))
						{
							if (!flag3)
							{
								goto IL_036e;
							}
							marker = result;
							num3 = 0;
						}
						else
						{
							num3 = 0;
						}
					}
					char c = ((!flag2) ? line.text[num2] : char.ToLower(line.text[num2]));
					if (c == text[num3])
					{
						if (num3 == 0)
						{
							result.line = line;
							result.pos = num2;
						}
						if (!flag || (flag && (flag4 || num3 > 0)))
						{
							num3++;
						}
						if (!flag && num3 == text.Length)
						{
							if (!flag3)
							{
								goto IL_036e;
							}
							marker = result;
							num3 = 0;
						}
					}
					else
					{
						num3 = 0;
					}
					num2++;
					if (flag)
					{
						flag4 = (IsWordSeparator(c) ? true : false);
					}
					continue;
				}
				if (!flag)
				{
					break;
				}
				if (line.ending != LineEnding.Wrap || line.line_no == lines - 1)
				{
					flag4 = true;
				}
				if (num3 != text.Length)
				{
					break;
				}
				if (flag4)
				{
					if (!flag3)
					{
						goto IL_036e;
					}
					marker = result;
					num3 = 0;
					break;
				}
				num3 = 0;
				break;
				IL_036e:
				if (!flag3)
				{
					return true;
				}
				result = marker;
				return true;
			}
			num2 = 0;
			num++;
			line = GetLine(num);
		}
		if (flag3 && marker.height != -1)
		{
			result = marker;
			return true;
		}
		return false;
	}

	internal void GetMarker(out Marker mark, bool start)
	{
		mark = default(Marker);
		if (start)
		{
			mark.line = GetLine(1);
			mark.tag = mark.line.tags;
			mark.pos = 0;
			return;
		}
		mark.line = GetLine(lines);
		mark.tag = mark.line.tags;
		while (mark.tag.Next != null)
		{
			mark.tag = mark.tag.Next;
		}
		mark.pos = mark.line.text.Length;
	}

	public IEnumerator GetEnumerator()
	{
		return null;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is Document))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		if (ToString().Equals(((Document)obj).ToString()))
		{
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return document_id;
	}

	public override string ToString()
	{
		return "document " + document_id;
	}
}

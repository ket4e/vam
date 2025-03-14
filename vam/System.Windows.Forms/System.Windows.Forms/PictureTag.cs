using System.Drawing;
using System.Windows.Forms.RTF;

namespace System.Windows.Forms;

internal class PictureTag : LineTag
{
	internal Picture picture;

	public override bool IsTextTag => false;

	internal PictureTag(Line line, int start, Picture picture)
		: base(line, start)
	{
		this.picture = picture;
	}

	public override SizeF SizeOfPosition(Graphics dc, int pos)
	{
		return picture.Size;
	}

	internal override int MaxHeight()
	{
		return (int)(picture.Height + 0.5f);
	}

	public override void Draw(Graphics dc, System.Drawing.Color color, float xoff, float y, int start, int end)
	{
		picture.DrawImage(dc, xoff + base.Line.widths[start], y, selected: false);
	}

	public override void Draw(Graphics dc, System.Drawing.Color color, float xoff, float y, int start, int end, string text)
	{
		picture.DrawImage(dc, xoff + base.Line.widths[start], y, selected: false);
	}

	public override string Text()
	{
		return "I";
	}
}

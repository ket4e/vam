using System.Drawing;

namespace System.Windows.Forms.Theming.Default;

internal class LinkLabelPainter
{
	private Color GetPieceColor(LinkLabel label, LinkLabel.Piece piece, int i)
	{
		if (!label.Enabled)
		{
			return label.DisabledLinkColor;
		}
		if (piece.link == null)
		{
			return label.ForeColor;
		}
		if (!piece.link.Enabled)
		{
			return label.DisabledLinkColor;
		}
		if (piece.link.Active)
		{
			return label.ActiveLinkColor;
		}
		if ((label.LinkVisited && i == 0) || piece.link.Visited)
		{
			return label.VisitedLinkColor;
		}
		return label.LinkColor;
	}

	public virtual void Draw(Graphics dc, Rectangle clip_rectangle, LinkLabel label)
	{
		Rectangle paddingClientRectangle = label.PaddingClientRectangle;
		label.DrawImage(dc, label.Image, paddingClientRectangle, label.ImageAlign);
		if (label.pieces == null)
		{
			return;
		}
		if (!label.Enabled)
		{
			dc.SetClip(clip_rectangle);
			ThemeEngine.Current.CPDrawStringDisabled(dc, label.Text, label.Font, label.BackColor, paddingClientRectangle, label.string_format);
			return;
		}
		Font linkFont = ThemeEngine.Current.GetLinkFont(label);
		Region region = new Region(default(Rectangle));
		for (int i = 0; i < label.pieces.Length; i++)
		{
			LinkLabel.Piece piece = label.pieces[i];
			if (piece.link == null)
			{
				region.Union(piece.region);
				continue;
			}
			Color pieceColor = GetPieceColor(label, piece, i);
			Font font = ((label.LinkBehavior != LinkBehavior.AlwaysUnderline && label.LinkBehavior != 0 && (label.LinkBehavior != LinkBehavior.HoverUnderline || !piece.link.Hovered)) ? label.Font : linkFont);
			dc.Clip = piece.region;
			dc.Clip.Intersect(clip_rectangle);
			dc.DrawString(label.Text, font, ThemeEngine.Current.ResPool.GetSolidBrush(pieceColor), paddingClientRectangle, label.string_format);
			if (piece.link != null && piece.link.Focused)
			{
				RectangleF[] regionScans = piece.region.GetRegionScans(dc.Transform);
				foreach (RectangleF value in regionScans)
				{
					ControlPaint.DrawFocusRectangle(dc, Rectangle.Round(value), label.ForeColor, label.BackColor);
				}
			}
		}
		if (!region.IsEmpty(dc))
		{
			dc.Clip = region;
			dc.Clip.Intersect(clip_rectangle);
			if (!dc.Clip.IsEmpty(dc))
			{
				dc.DrawString(label.Text, label.Font, ThemeEngine.Current.ResPool.GetSolidBrush(label.ForeColor), paddingClientRectangle, label.string_format);
			}
		}
	}
}

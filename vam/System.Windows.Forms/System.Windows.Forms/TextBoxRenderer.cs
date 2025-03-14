using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class TextBoxRenderer
{
	public static bool IsSupported => VisualStyleInformation.IsEnabledByUser && (Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled || Application.VisualStyleState == VisualStyleState.ClientAreaEnabled);

	private TextBoxRenderer()
	{
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, TextBoxState state)
	{
		DrawTextBox(g, bounds, string.Empty, null, Rectangle.Empty, TextFormatFlags.Left, state);
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, string textBoxText, Font font, TextBoxState state)
	{
		DrawTextBox(g, bounds, textBoxText, font, Rectangle.Empty, TextFormatFlags.Left, state);
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, string textBoxText, Font font, Rectangle textBounds, TextBoxState state)
	{
		DrawTextBox(g, bounds, textBoxText, font, textBounds, TextFormatFlags.Left, state);
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, string textBoxText, Font font, TextFormatFlags flags, TextBoxState state)
	{
		DrawTextBox(g, bounds, textBoxText, font, Rectangle.Empty, flags, state);
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, string textBoxText, Font font, Rectangle textBounds, TextFormatFlags flags, TextBoxState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		(state switch
		{
			TextBoxState.Assist => new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Assist), 
			TextBoxState.Disabled => new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Disabled), 
			TextBoxState.Hot => new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Hot), 
			TextBoxState.Selected => new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Selected), 
			_ => new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Normal), 
		}).DrawBackground(g, bounds);
		if (textBounds == Rectangle.Empty)
		{
			textBounds = new Rectangle(bounds.Left + 3, bounds.Top + 3, bounds.Width - 6, bounds.Height - 6);
		}
		if (textBoxText != string.Empty)
		{
			if (state == TextBoxState.Disabled)
			{
				TextRenderer.DrawText(g, textBoxText, font, textBounds, SystemColors.GrayText, flags);
			}
			else
			{
				TextRenderer.DrawText(g, textBoxText, font, textBounds, SystemColors.ControlText, flags);
			}
		}
	}
}

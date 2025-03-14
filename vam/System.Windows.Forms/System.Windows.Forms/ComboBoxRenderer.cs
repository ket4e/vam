using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public sealed class ComboBoxRenderer
{
	public static bool IsSupported => VisualStyleInformation.IsEnabledByUser && (Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled || Application.VisualStyleState == VisualStyleState.ClientAreaEnabled);

	private ComboBoxRenderer()
	{
	}

	public static void DrawDropDownButton(Graphics g, Rectangle bounds, ComboBoxState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		GetComboRenderer(state).DrawBackground(g, bounds);
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, Rectangle textBounds, TextFormatFlags flags, ComboBoxState state)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		GetTextBoxRenderer(state).DrawBackground(g, bounds);
		if (textBounds == Rectangle.Empty)
		{
			textBounds = new Rectangle(bounds.Left + 3, bounds.Top, bounds.Width - 4, bounds.Height);
		}
		if (comboBoxText != string.Empty)
		{
			if (state == ComboBoxState.Disabled)
			{
				TextRenderer.DrawText(g, comboBoxText, font, textBounds, SystemColors.GrayText, flags);
			}
			else
			{
				TextRenderer.DrawText(g, comboBoxText, font, textBounds, SystemColors.ControlText, flags);
			}
		}
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, ComboBoxState state)
	{
		DrawTextBox(g, bounds, string.Empty, null, Rectangle.Empty, TextFormatFlags.VerticalCenter, state);
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, ComboBoxState state)
	{
		DrawTextBox(g, bounds, comboBoxText, font, Rectangle.Empty, TextFormatFlags.VerticalCenter, state);
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, Rectangle textBounds, ComboBoxState state)
	{
		DrawTextBox(g, bounds, comboBoxText, font, textBounds, TextFormatFlags.Left, state);
	}

	public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, TextFormatFlags flags, ComboBoxState state)
	{
		DrawTextBox(g, bounds, comboBoxText, font, Rectangle.Empty, flags |= TextFormatFlags.VerticalCenter, state);
	}

	private static VisualStyleRenderer GetComboRenderer(ComboBoxState state)
	{
		return state switch
		{
			ComboBoxState.Disabled => new VisualStyleRenderer(VisualStyleElement.ComboBox.DropDownButton.Disabled), 
			ComboBoxState.Hot => new VisualStyleRenderer(VisualStyleElement.ComboBox.DropDownButton.Hot), 
			ComboBoxState.Pressed => new VisualStyleRenderer(VisualStyleElement.ComboBox.DropDownButton.Pressed), 
			_ => new VisualStyleRenderer(VisualStyleElement.ComboBox.DropDownButton.Normal), 
		};
	}

	private static VisualStyleRenderer GetTextBoxRenderer(ComboBoxState state)
	{
		return state switch
		{
			ComboBoxState.Disabled => new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Disabled), 
			ComboBoxState.Hot => new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Hot), 
			_ => new VisualStyleRenderer(VisualStyleElement.TextBox.TextEdit.Normal), 
		};
	}
}

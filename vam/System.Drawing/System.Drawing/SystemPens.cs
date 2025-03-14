namespace System.Drawing;

public sealed class SystemPens
{
	private static Pen active_caption_text;

	private static Pen control;

	private static Pen control_dark;

	private static Pen control_dark_dark;

	private static Pen control_light;

	private static Pen control_light_light;

	private static Pen control_text;

	private static Pen gray_text;

	private static Pen highlight;

	private static Pen highlight_text;

	private static Pen inactive_caption_text;

	private static Pen info_text;

	private static Pen menu_text;

	private static Pen window_frame;

	private static Pen window_text;

	private static Pen active_border;

	private static Pen active_caption;

	private static Pen app_workspace;

	private static Pen button_face;

	private static Pen button_highlight;

	private static Pen button_shadow;

	private static Pen desktop;

	private static Pen gradient_activecaption;

	private static Pen gradient_inactivecaption;

	private static Pen hot_track;

	private static Pen inactive_border;

	private static Pen inactive_caption;

	private static Pen info;

	private static Pen menu;

	private static Pen menu_bar;

	private static Pen menu_highlight;

	private static Pen scroll_bar;

	private static Pen window;

	public static Pen ActiveCaptionText
	{
		get
		{
			if (active_caption_text == null)
			{
				active_caption_text = new Pen(SystemColors.ActiveCaptionText);
				active_caption_text.isModifiable = false;
			}
			return active_caption_text;
		}
	}

	public static Pen Control
	{
		get
		{
			if (control == null)
			{
				control = new Pen(SystemColors.Control);
				control.isModifiable = false;
			}
			return control;
		}
	}

	public static Pen ControlDark
	{
		get
		{
			if (control_dark == null)
			{
				control_dark = new Pen(SystemColors.ControlDark);
				control_dark.isModifiable = false;
			}
			return control_dark;
		}
	}

	public static Pen ControlDarkDark
	{
		get
		{
			if (control_dark_dark == null)
			{
				control_dark_dark = new Pen(SystemColors.ControlDarkDark);
				control_dark_dark.isModifiable = false;
			}
			return control_dark_dark;
		}
	}

	public static Pen ControlLight
	{
		get
		{
			if (control_light == null)
			{
				control_light = new Pen(SystemColors.ControlLight);
				control_light.isModifiable = false;
			}
			return control_light;
		}
	}

	public static Pen ControlLightLight
	{
		get
		{
			if (control_light_light == null)
			{
				control_light_light = new Pen(SystemColors.ControlLightLight);
				control_light_light.isModifiable = false;
			}
			return control_light_light;
		}
	}

	public static Pen ControlText
	{
		get
		{
			if (control_text == null)
			{
				control_text = new Pen(SystemColors.ControlText);
				control_text.isModifiable = false;
			}
			return control_text;
		}
	}

	public static Pen GrayText
	{
		get
		{
			if (gray_text == null)
			{
				gray_text = new Pen(SystemColors.GrayText);
				gray_text.isModifiable = false;
			}
			return gray_text;
		}
	}

	public static Pen Highlight
	{
		get
		{
			if (highlight == null)
			{
				highlight = new Pen(SystemColors.Highlight);
				highlight.isModifiable = false;
			}
			return highlight;
		}
	}

	public static Pen HighlightText
	{
		get
		{
			if (highlight_text == null)
			{
				highlight_text = new Pen(SystemColors.HighlightText);
				highlight_text.isModifiable = false;
			}
			return highlight_text;
		}
	}

	public static Pen InactiveCaptionText
	{
		get
		{
			if (inactive_caption_text == null)
			{
				inactive_caption_text = new Pen(SystemColors.InactiveCaptionText);
				inactive_caption_text.isModifiable = false;
			}
			return inactive_caption_text;
		}
	}

	public static Pen InfoText
	{
		get
		{
			if (info_text == null)
			{
				info_text = new Pen(SystemColors.InfoText);
				info_text.isModifiable = false;
			}
			return info_text;
		}
	}

	public static Pen MenuText
	{
		get
		{
			if (menu_text == null)
			{
				menu_text = new Pen(SystemColors.MenuText);
				menu_text.isModifiable = false;
			}
			return menu_text;
		}
	}

	public static Pen WindowFrame
	{
		get
		{
			if (window_frame == null)
			{
				window_frame = new Pen(SystemColors.WindowFrame);
				window_frame.isModifiable = false;
			}
			return window_frame;
		}
	}

	public static Pen WindowText
	{
		get
		{
			if (window_text == null)
			{
				window_text = new Pen(SystemColors.WindowText);
				window_text.isModifiable = false;
			}
			return window_text;
		}
	}

	public static Pen ActiveBorder
	{
		get
		{
			if (active_border == null)
			{
				active_border = new Pen(SystemColors.ActiveBorder);
				active_border.isModifiable = false;
			}
			return active_border;
		}
	}

	public static Pen ActiveCaption
	{
		get
		{
			if (active_caption == null)
			{
				active_caption = new Pen(SystemColors.ActiveCaption);
				active_caption.isModifiable = false;
			}
			return active_caption;
		}
	}

	public static Pen AppWorkspace
	{
		get
		{
			if (app_workspace == null)
			{
				app_workspace = new Pen(SystemColors.AppWorkspace);
				app_workspace.isModifiable = false;
			}
			return app_workspace;
		}
	}

	public static Pen ButtonFace
	{
		get
		{
			if (button_face == null)
			{
				button_face = new Pen(SystemColors.ButtonFace);
				button_face.isModifiable = false;
			}
			return button_face;
		}
	}

	public static Pen ButtonHighlight
	{
		get
		{
			if (button_highlight == null)
			{
				button_highlight = new Pen(SystemColors.ButtonHighlight);
				button_highlight.isModifiable = false;
			}
			return button_highlight;
		}
	}

	public static Pen ButtonShadow
	{
		get
		{
			if (button_shadow == null)
			{
				button_shadow = new Pen(SystemColors.ButtonShadow);
				button_shadow.isModifiable = false;
			}
			return button_shadow;
		}
	}

	public static Pen Desktop
	{
		get
		{
			if (desktop == null)
			{
				desktop = new Pen(SystemColors.Desktop);
				desktop.isModifiable = false;
			}
			return desktop;
		}
	}

	public static Pen GradientActiveCaption
	{
		get
		{
			if (gradient_activecaption == null)
			{
				gradient_activecaption = new Pen(SystemColors.GradientActiveCaption);
				gradient_activecaption.isModifiable = false;
			}
			return gradient_activecaption;
		}
	}

	public static Pen GradientInactiveCaption
	{
		get
		{
			if (gradient_inactivecaption == null)
			{
				gradient_inactivecaption = new Pen(SystemColors.GradientInactiveCaption);
				gradient_inactivecaption.isModifiable = false;
			}
			return gradient_inactivecaption;
		}
	}

	public static Pen HotTrack
	{
		get
		{
			if (hot_track == null)
			{
				hot_track = new Pen(SystemColors.HotTrack);
				hot_track.isModifiable = false;
			}
			return hot_track;
		}
	}

	public static Pen InactiveBorder
	{
		get
		{
			if (inactive_border == null)
			{
				inactive_border = new Pen(SystemColors.InactiveBorder);
				inactive_border.isModifiable = false;
			}
			return inactive_border;
		}
	}

	public static Pen InactiveCaption
	{
		get
		{
			if (inactive_caption == null)
			{
				inactive_caption = new Pen(SystemColors.InactiveCaption);
				inactive_caption.isModifiable = false;
			}
			return inactive_caption;
		}
	}

	public static Pen Info
	{
		get
		{
			if (info == null)
			{
				info = new Pen(SystemColors.Info);
				info.isModifiable = false;
			}
			return info;
		}
	}

	public static Pen Menu
	{
		get
		{
			if (menu == null)
			{
				menu = new Pen(SystemColors.Menu);
				menu.isModifiable = false;
			}
			return menu;
		}
	}

	public static Pen MenuBar
	{
		get
		{
			if (menu_bar == null)
			{
				menu_bar = new Pen(SystemColors.MenuBar);
				menu_bar.isModifiable = false;
			}
			return menu_bar;
		}
	}

	public static Pen MenuHighlight
	{
		get
		{
			if (menu_highlight == null)
			{
				menu_highlight = new Pen(SystemColors.MenuHighlight);
				menu_highlight.isModifiable = false;
			}
			return menu_highlight;
		}
	}

	public static Pen ScrollBar
	{
		get
		{
			if (scroll_bar == null)
			{
				scroll_bar = new Pen(SystemColors.ScrollBar);
				scroll_bar.isModifiable = false;
			}
			return scroll_bar;
		}
	}

	public static Pen Window
	{
		get
		{
			if (window == null)
			{
				window = new Pen(SystemColors.Window);
				window.isModifiable = false;
			}
			return window;
		}
	}

	private SystemPens()
	{
	}

	public static Pen FromSystemColor(Color c)
	{
		if (c.IsSystemColor)
		{
			Pen pen = new Pen(c);
			pen.isModifiable = false;
			return pen;
		}
		string message = $"The color {c} is not a system color.";
		throw new ArgumentException(message);
	}
}

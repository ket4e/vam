using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[DefaultProperty("Color")]
public class ColorDialog : CommonDialog
{
	internal struct HSB
	{
		public int hue;

		public int sat;

		public int bri;

		public static HSB RGB2HSB(Color color)
		{
			HSB result = default(HSB);
			result.hue = (int)(color.GetHue() / 360f * 240f);
			result.sat = (int)(color.GetSaturation() * 241f);
			result.bri = (int)(color.GetBrightness() * 241f);
			if (result.hue > 239)
			{
				result.hue = 239;
			}
			if (result.sat > 240)
			{
				result.sat = 240;
			}
			if (result.bri > 240)
			{
				result.bri = 240;
			}
			return result;
		}

		public static Color HSB2RGB(int hue, int saturation, int brightness)
		{
			if (hue > 239)
			{
				hue = 239;
			}
			else if (hue < 0)
			{
				hue = 0;
			}
			if (saturation > 240)
			{
				saturation = 240;
			}
			else if (saturation < 0)
			{
				saturation = 0;
			}
			if (brightness > 240)
			{
				brightness = 240;
			}
			else if (brightness < 0)
			{
				brightness = 0;
			}
			float num = (float)hue / 239f;
			float num2 = (float)saturation / 240f;
			float num3 = (float)brightness / 240f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			if (num3 == 0f)
			{
				num4 = (num5 = (num6 = 0f));
			}
			else if (num2 == 0f)
			{
				num4 = (num5 = (num6 = num3));
			}
			else
			{
				float num7 = ((!(num3 <= 0.5f)) ? (num3 + num2 - num3 * num2) : (num3 * (1f + num2)));
				float num8 = 2f * num3 - num7;
				float[] array = new float[3]
				{
					num + 1f / 3f,
					num,
					num - 1f / 3f
				};
				float[] array2 = new float[3];
				for (int i = 0; i < 3; i++)
				{
					if (array[i] < 0f)
					{
						array[i] += 1f;
					}
					if (array[i] > 1f)
					{
						array[i] -= 1f;
					}
					if (6f * array[i] < 1f)
					{
						array2[i] = num8 + (num7 - num8) * array[i] * 6f;
					}
					else if (2f * array[i] < 1f)
					{
						array2[i] = num7;
					}
					else if (3f * array[i] < 2f)
					{
						array2[i] = num8 + (num7 - num8) * (2f / 3f - array[i]) * 6f;
					}
					else
					{
						array2[i] = num8;
					}
				}
				num4 = array2[0];
				num5 = array2[1];
				num6 = array2[2];
			}
			num4 = 255f * num4;
			num5 = 255f * num5;
			num6 = 255f * num6;
			if (num4 < 1f)
			{
				num4 = 0f;
			}
			else if (num4 > 255f)
			{
				num4 = 255f;
			}
			if (num5 < 1f)
			{
				num5 = 0f;
			}
			else if (num5 > 255f)
			{
				num5 = 255f;
			}
			if (num6 < 1f)
			{
				num6 = 0f;
			}
			else if (num6 > 255f)
			{
				num6 = 255f;
			}
			return Color.FromArgb((int)num4, (int)num5, (int)num6);
		}

		public static int Brightness(Color color)
		{
			return (int)(color.GetBrightness() * 241f);
		}

		public static void GetHueSaturation(Color color, out int hue, out int sat)
		{
			hue = (int)(color.GetHue() / 360f * 240f);
			sat = (int)(color.GetSaturation() * 241f);
		}

		public static void TestColor(Color color)
		{
			Console.WriteLine("Color: " + color);
			HSB hSB = RGB2HSB(color);
			Console.WriteLine("RGB2HSB: " + hSB.hue + ", " + hSB.sat + ", " + hSB.bri);
			Console.WriteLine("HSB2RGB: " + HSB2RGB(hSB.hue, hSB.sat, hSB.bri));
			Console.WriteLine();
		}
	}

	internal class BaseColorControl : Control
	{
		internal class SmallColorControl : Control
		{
			private Color internalcolor;

			private bool isSelected;

			public bool IsSelected
			{
				get
				{
					return isSelected;
				}
				set
				{
					isSelected = value;
					Invalidate();
				}
			}

			public Color InternalColor
			{
				get
				{
					return internalcolor;
				}
				set
				{
					internalcolor = value;
					Invalidate();
				}
			}

			public SmallColorControl(Color color)
			{
				SuspendLayout();
				internalcolor = color;
				base.Size = new Size(25, 23);
				ResumeLayout(performLayout: false);
				SetStyle(ControlStyles.DoubleBuffer, value: true);
				SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
				SetStyle(ControlStyles.UserPaint, value: true);
				SetStyle(ControlStyles.Selectable, value: true);
			}

			protected override void OnPaint(PaintEventArgs pe)
			{
				base.OnPaint(pe);
				pe.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(internalcolor), new Rectangle(4, 4, 17, 15));
				ControlPaint.DrawBorder3D(pe.Graphics, 3, 3, 19, 17, Border3DStyle.Sunken);
				if (isSelected)
				{
					pe.Graphics.DrawRectangle(ThemeEngine.Current.ResPool.GetPen(Color.Black), new Rectangle(2, 2, 20, 18));
				}
				if (Focused)
				{
					ControlPaint.DrawFocusRectangle(pe.Graphics, new Rectangle(0, 0, 25, 23));
				}
			}

			protected override void OnClick(EventArgs e)
			{
				Focus();
				IsSelected = true;
				base.OnClick(e);
			}

			protected override void OnLostFocus(EventArgs e)
			{
				Invalidate();
				base.OnLostFocus(e);
			}
		}

		private SmallColorControl[] smallColorControl;

		private SmallColorControl[] userSmallColorControl;

		private Label userColorLabel;

		private Label baseColorLabel;

		private SmallColorControl selectedSmallColorControl;

		private int currentlyUsedUserSmallColorControl;

		private ColorDialog colorDialog;

		public SmallColorControl UIASelectedSmallColorControl
		{
			get
			{
				for (int i = 0; i < smallColorControl.Length - 1; i++)
				{
					if (smallColorControl[i].IsSelected)
					{
						return smallColorControl[i];
					}
				}
				for (int j = 0; j < userSmallColorControl.Length - 1; j++)
				{
					if (userSmallColorControl[j].IsSelected)
					{
						return userSmallColorControl[j];
					}
				}
				return null;
			}
		}

		public Color ColorToShow => selectedSmallColorControl.InternalColor;

		public BaseColorControl(ColorDialog colorDialog)
		{
			this.colorDialog = colorDialog;
			userSmallColorControl = new SmallColorControl[16];
			userSmallColorControl[0] = new SmallColorControl(Color.White);
			userSmallColorControl[1] = new SmallColorControl(Color.White);
			userSmallColorControl[2] = new SmallColorControl(Color.White);
			userSmallColorControl[3] = new SmallColorControl(Color.White);
			userSmallColorControl[4] = new SmallColorControl(Color.White);
			userSmallColorControl[5] = new SmallColorControl(Color.White);
			userSmallColorControl[6] = new SmallColorControl(Color.White);
			userSmallColorControl[7] = new SmallColorControl(Color.White);
			userSmallColorControl[8] = new SmallColorControl(Color.White);
			userSmallColorControl[9] = new SmallColorControl(Color.White);
			userSmallColorControl[10] = new SmallColorControl(Color.White);
			userSmallColorControl[11] = new SmallColorControl(Color.White);
			userSmallColorControl[12] = new SmallColorControl(Color.White);
			userSmallColorControl[13] = new SmallColorControl(Color.White);
			userSmallColorControl[14] = new SmallColorControl(Color.White);
			userSmallColorControl[15] = new SmallColorControl(Color.White);
			smallColorControl = new SmallColorControl[48];
			smallColorControl[0] = new SmallColorControl(Color.FromArgb(255, 128, 128));
			smallColorControl[1] = new SmallColorControl(Color.FromArgb(128, 128, 64));
			smallColorControl[2] = new SmallColorControl(Color.Gray);
			smallColorControl[3] = new SmallColorControl(Color.FromArgb(128, 0, 255));
			smallColorControl[4] = new SmallColorControl(Color.Silver);
			smallColorControl[5] = new SmallColorControl(Color.FromArgb(64, 128, 128));
			smallColorControl[6] = new SmallColorControl(Color.White);
			smallColorControl[7] = new SmallColorControl(Color.FromArgb(64, 0, 64));
			smallColorControl[8] = new SmallColorControl(Color.FromArgb(255, 128, 64));
			smallColorControl[9] = new SmallColorControl(Color.FromArgb(128, 64, 64));
			smallColorControl[10] = new SmallColorControl(Color.Teal);
			smallColorControl[11] = new SmallColorControl(Color.Lime);
			smallColorControl[12] = new SmallColorControl(Color.FromArgb(128, 128, 255));
			smallColorControl[13] = new SmallColorControl(Color.FromArgb(0, 64, 128));
			smallColorControl[14] = new SmallColorControl(Color.FromArgb(255, 0, 128));
			smallColorControl[15] = new SmallColorControl(Color.FromArgb(128, 255, 0));
			smallColorControl[16] = new SmallColorControl(Color.FromArgb(0, 255, 64));
			smallColorControl[17] = new SmallColorControl(Color.Red);
			smallColorControl[18] = new SmallColorControl(Color.FromArgb(255, 128, 0));
			smallColorControl[19] = new SmallColorControl(Color.FromArgb(255, 128, 255));
			smallColorControl[20] = new SmallColorControl(Color.Fuchsia);
			smallColorControl[21] = new SmallColorControl(Color.Aqua);
			smallColorControl[22] = new SmallColorControl(Color.FromArgb(128, 255, 128));
			smallColorControl[23] = new SmallColorControl(Color.FromArgb(128, 255, 255));
			smallColorControl[24] = new SmallColorControl(Color.FromArgb(0, 128, 255));
			smallColorControl[25] = new SmallColorControl(Color.FromArgb(128, 64, 0));
			smallColorControl[26] = new SmallColorControl(Color.FromArgb(64, 0, 0));
			smallColorControl[27] = new SmallColorControl(Color.Maroon);
			smallColorControl[28] = new SmallColorControl(Color.Purple);
			smallColorControl[29] = new SmallColorControl(Color.FromArgb(0, 0, 160));
			smallColorControl[30] = new SmallColorControl(Color.Blue);
			smallColorControl[31] = new SmallColorControl(Color.FromArgb(0, 128, 64));
			smallColorControl[32] = new SmallColorControl(Color.Green);
			smallColorControl[33] = new SmallColorControl(Color.Yellow);
			smallColorControl[34] = new SmallColorControl(Color.FromArgb(128, 128, 192));
			smallColorControl[35] = new SmallColorControl(Color.FromArgb(0, 128, 192));
			smallColorControl[36] = new SmallColorControl(Color.FromArgb(128, 0, 64));
			smallColorControl[37] = new SmallColorControl(Color.FromArgb(255, 128, 192));
			smallColorControl[38] = new SmallColorControl(Color.FromArgb(0, 255, 128));
			smallColorControl[39] = new SmallColorControl(Color.FromArgb(255, 255, 128));
			smallColorControl[40] = new SmallColorControl(Color.FromArgb(0, 64, 0));
			smallColorControl[41] = new SmallColorControl(Color.FromArgb(0, 64, 64));
			smallColorControl[42] = new SmallColorControl(Color.Navy);
			smallColorControl[43] = new SmallColorControl(Color.FromArgb(0, 0, 64));
			smallColorControl[44] = new SmallColorControl(Color.FromArgb(64, 0, 64));
			smallColorControl[45] = new SmallColorControl(Color.FromArgb(64, 0, 128));
			smallColorControl[46] = new SmallColorControl(Color.Black);
			smallColorControl[47] = new SmallColorControl(Color.Olive);
			baseColorLabel = new Label();
			userColorLabel = new Label();
			SuspendLayout();
			smallColorControl[0].Location = new Point(0, 15);
			smallColorControl[0].TabIndex = 51;
			smallColorControl[0].Click += OnSmallColorControlClick;
			smallColorControl[1].Location = new Point(50, 130);
			smallColorControl[1].TabIndex = 92;
			smallColorControl[1].Click += OnSmallColorControlClick;
			smallColorControl[2].Location = new Point(75, 130);
			smallColorControl[2].TabIndex = 93;
			smallColorControl[2].Click += OnSmallColorControlClick;
			smallColorControl[3].Location = new Point(175, 84);
			smallColorControl[3].TabIndex = 98;
			smallColorControl[3].Click += OnSmallColorControlClick;
			smallColorControl[4].Location = new Point(125, 130);
			smallColorControl[4].TabIndex = 95;
			smallColorControl[4].Click += OnSmallColorControlClick;
			smallColorControl[5].Location = new Point(100, 130);
			smallColorControl[5].TabIndex = 94;
			smallColorControl[5].Click += OnSmallColorControlClick;
			smallColorControl[6].Location = new Point(175, 130);
			smallColorControl[6].TabIndex = 97;
			smallColorControl[6].Click += OnSmallColorControlClick;
			smallColorControl[7].Location = new Point(150, 130);
			smallColorControl[7].TabIndex = 96;
			smallColorControl[7].Click += OnSmallColorControlClick;
			smallColorControl[8].Location = new Point(25, 61);
			smallColorControl[8].TabIndex = 68;
			smallColorControl[8].Click += OnSmallColorControlClick;
			smallColorControl[9].Location = new Point(0, 61);
			smallColorControl[9].TabIndex = 67;
			smallColorControl[9].Click += OnSmallColorControlClick;
			smallColorControl[10].Location = new Point(75, 61);
			smallColorControl[10].TabIndex = 70;
			smallColorControl[10].Click += OnSmallColorControlClick;
			smallColorControl[11].Location = new Point(50, 61);
			smallColorControl[11].TabIndex = 69;
			smallColorControl[11].Click += OnSmallColorControlClick;
			smallColorControl[12].Location = new Point(125, 61);
			smallColorControl[12].TabIndex = 72;
			smallColorControl[12].Click += OnSmallColorControlClick;
			smallColorControl[13].Location = new Point(100, 61);
			smallColorControl[13].TabIndex = 71;
			smallColorControl[13].Click += OnSmallColorControlClick;
			smallColorControl[14].Location = new Point(175, 61);
			smallColorControl[14].TabIndex = 74;
			smallColorControl[14].Click += OnSmallColorControlClick;
			smallColorControl[15].Location = new Point(50, 38);
			smallColorControl[15].TabIndex = 61;
			smallColorControl[15].Click += OnSmallColorControlClick;
			smallColorControl[16].Location = new Point(75, 38);
			smallColorControl[16].TabIndex = 62;
			smallColorControl[16].Click += OnSmallColorControlClick;
			smallColorControl[17].Location = new Point(0, 38);
			smallColorControl[17].TabIndex = 59;
			smallColorControl[17].Click += OnSmallColorControlClick;
			smallColorControl[18].Location = new Point(25, 84);
			smallColorControl[18].TabIndex = 75;
			smallColorControl[18].Click += OnSmallColorControlClick;
			smallColorControl[19].Location = new Point(175, 15);
			smallColorControl[19].TabIndex = 58;
			smallColorControl[19].Click += OnSmallColorControlClick;
			smallColorControl[20].Location = new Point(175, 38);
			smallColorControl[20].TabIndex = 66;
			smallColorControl[20].Click += OnSmallColorControlClick;
			smallColorControl[21].Location = new Point(100, 38);
			smallColorControl[21].TabIndex = 63;
			smallColorControl[21].Click += OnSmallColorControlClick;
			smallColorControl[22].Location = new Point(50, 15);
			smallColorControl[22].TabIndex = 53;
			smallColorControl[22].Click += OnSmallColorControlClick;
			smallColorControl[23].Location = new Point(100, 15);
			smallColorControl[23].TabIndex = 55;
			smallColorControl[23].Click += OnSmallColorControlClick;
			smallColorControl[24].Location = new Point(125, 15);
			smallColorControl[24].TabIndex = 56;
			smallColorControl[24].Click += OnSmallColorControlClick;
			smallColorControl[25].Location = new Point(25, 107);
			smallColorControl[25].TabIndex = 83;
			smallColorControl[25].Click += OnSmallColorControlClick;
			smallColorControl[26].Location = new Point(0, 107);
			smallColorControl[26].TabIndex = 82;
			smallColorControl[26].Click += OnSmallColorControlClick;
			smallColorControl[27].Location = new Point(0, 84);
			smallColorControl[27].TabIndex = 81;
			smallColorControl[27].Click += OnSmallColorControlClick;
			smallColorControl[28].Location = new Point(150, 84);
			smallColorControl[28].TabIndex = 80;
			smallColorControl[28].Click += OnSmallColorControlClick;
			smallColorControl[29].Location = new Point(125, 84);
			smallColorControl[29].TabIndex = 79;
			smallColorControl[29].Click += OnSmallColorControlClick;
			smallColorControl[30].Location = new Point(100, 84);
			smallColorControl[30].TabIndex = 78;
			smallColorControl[30].Click += OnSmallColorControlClick;
			smallColorControl[31].Location = new Point(75, 84);
			smallColorControl[31].TabIndex = 77;
			smallColorControl[31].Click += OnSmallColorControlClick;
			smallColorControl[32].Location = new Point(50, 84);
			smallColorControl[32].TabIndex = 76;
			smallColorControl[32].Click += OnSmallColorControlClick;
			smallColorControl[33].Location = new Point(25, 38);
			smallColorControl[33].TabIndex = 60;
			smallColorControl[33].Click += OnSmallColorControlClick;
			smallColorControl[34].Location = new Point(150, 38);
			smallColorControl[34].TabIndex = 65;
			smallColorControl[34].Click += OnSmallColorControlClick;
			smallColorControl[35].Location = new Point(125, 38);
			smallColorControl[35].TabIndex = 64;
			smallColorControl[35].Click += OnSmallColorControlClick;
			smallColorControl[36].Location = new Point(150, 61);
			smallColorControl[36].TabIndex = 73;
			smallColorControl[36].Click += OnSmallColorControlClick;
			smallColorControl[37].Location = new Point(150, 15);
			smallColorControl[37].TabIndex = 57;
			smallColorControl[37].Click += OnSmallColorControlClick;
			smallColorControl[38].Location = new Point(75, 15);
			smallColorControl[38].TabIndex = 54;
			smallColorControl[38].Click += OnSmallColorControlClick;
			smallColorControl[39].Location = new Point(25, 15);
			smallColorControl[39].TabIndex = 52;
			smallColorControl[39].Click += OnSmallColorControlClick;
			smallColorControl[40].Location = new Point(50, 107);
			smallColorControl[40].TabIndex = 84;
			smallColorControl[40].Click += OnSmallColorControlClick;
			smallColorControl[41].Location = new Point(75, 107);
			smallColorControl[41].TabIndex = 85;
			smallColorControl[41].Click += OnSmallColorControlClick;
			smallColorControl[42].Location = new Point(100, 107);
			smallColorControl[42].TabIndex = 86;
			smallColorControl[42].Click += OnSmallColorControlClick;
			smallColorControl[43].Location = new Point(125, 107);
			smallColorControl[43].TabIndex = 87;
			smallColorControl[43].Click += OnSmallColorControlClick;
			smallColorControl[44].Location = new Point(150, 107);
			smallColorControl[44].TabIndex = 88;
			smallColorControl[44].Click += OnSmallColorControlClick;
			smallColorControl[45].Location = new Point(175, 107);
			smallColorControl[45].TabIndex = 89;
			smallColorControl[45].Click += OnSmallColorControlClick;
			smallColorControl[46].Location = new Point(0, 130);
			smallColorControl[46].TabIndex = 90;
			smallColorControl[46].Click += OnSmallColorControlClick;
			smallColorControl[47].Location = new Point(25, 130);
			smallColorControl[47].TabIndex = 91;
			smallColorControl[47].Click += OnSmallColorControlClick;
			userSmallColorControl[0].Location = new Point(0, 180);
			userSmallColorControl[0].TabIndex = 99;
			userSmallColorControl[0].Click += OnSmallColorControlClick;
			userSmallColorControl[1].Location = new Point(0, 203);
			userSmallColorControl[1].TabIndex = 108;
			userSmallColorControl[1].Click += OnSmallColorControlClick;
			userSmallColorControl[2].Location = new Point(25, 180);
			userSmallColorControl[2].TabIndex = 100;
			userSmallColorControl[2].Click += OnSmallColorControlClick;
			userSmallColorControl[3].Location = new Point(25, 203);
			userSmallColorControl[3].TabIndex = 109;
			userSmallColorControl[3].Click += OnSmallColorControlClick;
			userSmallColorControl[4].Location = new Point(50, 180);
			userSmallColorControl[4].TabIndex = 101;
			userSmallColorControl[4].Click += OnSmallColorControlClick;
			userSmallColorControl[5].Location = new Point(50, 203);
			userSmallColorControl[5].TabIndex = 110;
			userSmallColorControl[5].Click += OnSmallColorControlClick;
			userSmallColorControl[6].Location = new Point(75, 180);
			userSmallColorControl[6].TabIndex = 102;
			userSmallColorControl[6].Click += OnSmallColorControlClick;
			userSmallColorControl[7].Location = new Point(75, 203);
			userSmallColorControl[7].TabIndex = 111;
			userSmallColorControl[7].Click += OnSmallColorControlClick;
			userSmallColorControl[8].Location = new Point(100, 180);
			userSmallColorControl[8].TabIndex = 103;
			userSmallColorControl[8].Click += OnSmallColorControlClick;
			userSmallColorControl[9].Location = new Point(100, 203);
			userSmallColorControl[9].TabIndex = 112;
			userSmallColorControl[9].Click += OnSmallColorControlClick;
			userSmallColorControl[10].Location = new Point(125, 180);
			userSmallColorControl[10].TabIndex = 105;
			userSmallColorControl[10].Click += OnSmallColorControlClick;
			userSmallColorControl[11].Location = new Point(125, 203);
			userSmallColorControl[11].TabIndex = 113;
			userSmallColorControl[11].Click += OnSmallColorControlClick;
			userSmallColorControl[12].Location = new Point(150, 180);
			userSmallColorControl[12].TabIndex = 106;
			userSmallColorControl[12].Click += OnSmallColorControlClick;
			userSmallColorControl[13].Location = new Point(150, 203);
			userSmallColorControl[13].TabIndex = 114;
			userSmallColorControl[13].Click += OnSmallColorControlClick;
			userSmallColorControl[14].Location = new Point(175, 180);
			userSmallColorControl[14].TabIndex = 107;
			userSmallColorControl[14].Click += OnSmallColorControlClick;
			userSmallColorControl[15].Location = new Point(175, 203);
			userSmallColorControl[15].TabIndex = 115;
			userSmallColorControl[15].Click += OnSmallColorControlClick;
			baseColorLabel.Location = new Point(2, 0);
			baseColorLabel.Size = new Size(200, 12);
			baseColorLabel.TabIndex = 5;
			baseColorLabel.Text = Locale.GetText("Base Colors") + ":";
			userColorLabel.FlatStyle = FlatStyle.System;
			userColorLabel.Location = new Point(2, 164);
			userColorLabel.Size = new Size(200, 14);
			userColorLabel.TabIndex = 104;
			userColorLabel.Text = Locale.GetText("User Colors") + ":";
			base.Controls.Add(userSmallColorControl[7]);
			base.Controls.Add(userSmallColorControl[6]);
			base.Controls.Add(userSmallColorControl[5]);
			base.Controls.Add(userSmallColorControl[4]);
			base.Controls.Add(userSmallColorControl[3]);
			base.Controls.Add(userSmallColorControl[2]);
			base.Controls.Add(userSmallColorControl[1]);
			base.Controls.Add(userSmallColorControl[0]);
			base.Controls.Add(userSmallColorControl[15]);
			base.Controls.Add(userSmallColorControl[14]);
			base.Controls.Add(userSmallColorControl[13]);
			base.Controls.Add(userSmallColorControl[12]);
			base.Controls.Add(userSmallColorControl[11]);
			base.Controls.Add(userSmallColorControl[10]);
			base.Controls.Add(userSmallColorControl[9]);
			base.Controls.Add(userSmallColorControl[8]);
			base.Controls.Add(smallColorControl[0]);
			base.Controls.Add(smallColorControl[3]);
			base.Controls.Add(smallColorControl[6]);
			base.Controls.Add(smallColorControl[7]);
			base.Controls.Add(smallColorControl[4]);
			base.Controls.Add(smallColorControl[5]);
			base.Controls.Add(smallColorControl[2]);
			base.Controls.Add(smallColorControl[1]);
			base.Controls.Add(smallColorControl[47]);
			base.Controls.Add(smallColorControl[46]);
			base.Controls.Add(smallColorControl[45]);
			base.Controls.Add(smallColorControl[44]);
			base.Controls.Add(smallColorControl[43]);
			base.Controls.Add(smallColorControl[42]);
			base.Controls.Add(smallColorControl[41]);
			base.Controls.Add(smallColorControl[40]);
			base.Controls.Add(smallColorControl[25]);
			base.Controls.Add(smallColorControl[26]);
			base.Controls.Add(smallColorControl[27]);
			base.Controls.Add(smallColorControl[28]);
			base.Controls.Add(smallColorControl[29]);
			base.Controls.Add(smallColorControl[30]);
			base.Controls.Add(smallColorControl[31]);
			base.Controls.Add(smallColorControl[32]);
			base.Controls.Add(smallColorControl[18]);
			base.Controls.Add(smallColorControl[14]);
			base.Controls.Add(smallColorControl[36]);
			base.Controls.Add(smallColorControl[12]);
			base.Controls.Add(smallColorControl[13]);
			base.Controls.Add(smallColorControl[10]);
			base.Controls.Add(smallColorControl[11]);
			base.Controls.Add(smallColorControl[8]);
			base.Controls.Add(smallColorControl[9]);
			base.Controls.Add(smallColorControl[20]);
			base.Controls.Add(smallColorControl[34]);
			base.Controls.Add(smallColorControl[35]);
			base.Controls.Add(smallColorControl[21]);
			base.Controls.Add(smallColorControl[16]);
			base.Controls.Add(smallColorControl[15]);
			base.Controls.Add(smallColorControl[33]);
			base.Controls.Add(smallColorControl[17]);
			base.Controls.Add(smallColorControl[19]);
			base.Controls.Add(smallColorControl[37]);
			base.Controls.Add(smallColorControl[24]);
			base.Controls.Add(smallColorControl[23]);
			base.Controls.Add(smallColorControl[38]);
			base.Controls.Add(smallColorControl[22]);
			base.Controls.Add(smallColorControl[39]);
			base.Controls.Add(userColorLabel);
			base.Controls.Add(baseColorLabel);
			base.Size = new Size(212, 238);
			ResumeLayout(performLayout: false);
		}

		private void CheckIfColorIsInPanel(Color color)
		{
			for (int i = 0; i < smallColorControl.Length; i++)
			{
				if (smallColorControl[i].InternalColor == color)
				{
					selectedSmallColorControl = smallColorControl[i];
					selectedSmallColorControl.IsSelected = true;
					break;
				}
			}
		}

		private void OnSmallColorControlClick(object sender, EventArgs e)
		{
			if (selectedSmallColorControl != (SmallColorControl)sender)
			{
				selectedSmallColorControl.IsSelected = false;
			}
			selectedSmallColorControl = (SmallColorControl)sender;
			TriangleControl.CurrentBrightness = HSB.Brightness(selectedSmallColorControl.InternalColor);
			colorDialog.UpdateControls(selectedSmallColorControl.InternalColor);
			colorDialog.UpdateRGBTextBoxes(selectedSmallColorControl.InternalColor);
			colorDialog.UpdateHSBTextBoxes(selectedSmallColorControl.InternalColor);
		}

		public void SetColor(Color acolor)
		{
			if (selectedSmallColorControl != null)
			{
				selectedSmallColorControl.IsSelected = false;
			}
			CheckIfColorIsInPanel(acolor);
			TriangleControl.CurrentBrightness = HSB.Brightness(acolor);
			colorDialog.UpdateControls(acolor);
			colorDialog.UpdateRGBTextBoxes(acolor);
			colorDialog.UpdateHSBTextBoxes(acolor);
		}

		public void SetUserColor(Color col)
		{
			userSmallColorControl[currentlyUsedUserSmallColorControl].InternalColor = col;
			colorDialog.customColors[currentlyUsedUserSmallColorControl] = col.ToArgb();
			currentlyUsedUserSmallColorControl++;
			if (currentlyUsedUserSmallColorControl > 15)
			{
				currentlyUsedUserSmallColorControl = 0;
			}
		}

		public void SetCustomColors()
		{
			for (int i = 0; i < colorDialog.customColors.Length; i++)
			{
				userSmallColorControl[i].InternalColor = Color.FromArgb(colorDialog.customColors[i]);
			}
		}
	}

	internal class ColorMatrixControl : Panel
	{
		internal class DrawingBitmap
		{
			private Bitmap bitmap;

			public Bitmap Bitmap
			{
				get
				{
					return bitmap;
				}
				set
				{
					bitmap = value;
				}
			}

			public DrawingBitmap(Size size)
			{
				bitmap = new Bitmap(size.Width, size.Height);
				float num = 240f / (float)(size.Width - 1);
				float num2 = 241f / (float)(size.Height - 1);
				float num3 = 240f;
				for (int i = 0; i < size.Height; i++)
				{
					float num4 = 0f;
					for (int j = 0; j < size.Width; j++)
					{
						HSB hSB = new HSB
						{
							hue = (int)num4,
							sat = (int)num3,
							bri = 120
						};
						bitmap.SetPixel(j, i, HSB.HSB2RGB(hSB.hue, hSB.sat, hSB.bri));
						num4 += num;
					}
					num3 -= num2;
				}
			}
		}

		internal class CrossCursor
		{
			private Bitmap bitmap;

			private Color cursorColor;

			public Bitmap Bitmap
			{
				get
				{
					return bitmap;
				}
				set
				{
					bitmap = value;
				}
			}

			public Color CursorColor
			{
				get
				{
					return cursorColor;
				}
				set
				{
					cursorColor = value;
				}
			}

			public CrossCursor()
			{
				bitmap = new Bitmap(22, 22);
				cursorColor = Color.Black;
				Draw();
			}

			public void Draw()
			{
				using Pen pen = new Pen(ThemeEngine.Current.ResPool.GetSolidBrush(cursorColor), 3f);
				using Graphics graphics = Graphics.FromImage(bitmap);
				graphics.DrawLine(pen, 11, 0, 11, 7);
				graphics.DrawLine(pen, 11, 14, 11, 21);
				graphics.DrawLine(pen, 0, 11, 7, 11);
				graphics.DrawLine(pen, 14, 11, 21, 11);
			}
		}

		private DrawingBitmap drawingBitmap;

		private CrossCursor crossCursor = new CrossCursor();

		private bool mouseButtonDown;

		private bool drawCross = true;

		private Color color;

		private int currentXPos;

		private int currentYPos;

		private float xstep;

		private float ystep;

		private ColorDialog colorDialog;

		public Color ColorToShow
		{
			set
			{
				ComputePos(value);
			}
		}

		public ColorMatrixControl(ColorDialog colorDialog)
		{
			this.colorDialog = colorDialog;
			SuspendLayout();
			base.BorderStyle = BorderStyle.Fixed3D;
			base.Location = new Point(0, 0);
			base.Size = new Size(179, 190);
			base.TabIndex = 0;
			base.TabStop = false;
			ResumeLayout(performLayout: false);
			xstep = 240f / (float)(base.ClientSize.Width - 1);
			ystep = 241f / (float)(base.ClientSize.Height - 1);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (drawingBitmap == null)
			{
				drawingBitmap = new DrawingBitmap(base.ClientSize);
			}
			Draw(e);
			base.OnPaint(e);
		}

		private void Draw(PaintEventArgs e)
		{
			e.Graphics.DrawImage(drawingBitmap.Bitmap, base.ClientRectangle.X, base.ClientRectangle.Y);
			if (drawCross)
			{
				e.Graphics.DrawImage(crossCursor.Bitmap, currentXPos - 11, currentYPos - 11);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			mouseButtonDown = true;
			currentXPos = e.X;
			currentYPos = e.Y;
			if (drawCross)
			{
				drawCross = false;
				Invalidate();
				Update();
			}
			UpdateControls();
			XplatUI.GrabWindow(Handle, Handle);
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (mouseButtonDown && e.X < base.ClientSize.Width && e.X >= 0 && e.Y < base.ClientSize.Height && e.Y >= 0)
			{
				currentXPos = e.X;
				currentYPos = e.Y;
				UpdateControls();
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			XplatUI.UngrabWindow(Handle);
			mouseButtonDown = false;
			drawCross = true;
			Invalidate();
			Update();
		}

		private void ComputePos(Color acolor)
		{
			if (acolor != color)
			{
				color = acolor;
				HSB hSB = HSB.RGB2HSB(color);
				currentXPos = (int)((float)hSB.hue / xstep);
				currentYPos = base.ClientSize.Height - 1 - (int)((float)hSB.sat / ystep);
				if (currentXPos < 0)
				{
					currentXPos = 0;
				}
				if (currentYPos < 0)
				{
					currentYPos = 0;
				}
				Invalidate();
				Update();
			}
		}

		private Color GetColorFromHSB()
		{
			int hue = (int)((float)currentXPos * xstep);
			int saturation = 240 - (int)((float)currentYPos * ystep);
			int currentBrightness = TriangleControl.CurrentBrightness;
			return HSB.HSB2RGB(hue, saturation, currentBrightness);
		}

		private void UpdateControls()
		{
			Color colorFromHSB = GetColorFromHSB();
			colorDialog.brightnessControl.ShowColor((int)((float)currentXPos * xstep), 240 - (int)((float)currentYPos * ystep));
			int num = 240 - (int)((float)currentYPos * ystep);
			colorDialog.satTextBox.Text = num.ToString();
			int num2 = (int)((float)currentXPos * xstep);
			if (num2 > 239)
			{
				num2 = 239;
			}
			colorDialog.hueTextBox.Text = num2.ToString();
			colorDialog.selectedColorPanel.BackColor = colorFromHSB;
			colorDialog.UpdateRGBTextBoxes(colorFromHSB);
		}
	}

	internal class BrightnessControl : Panel
	{
		internal class DrawingBitmap
		{
			private Bitmap bitmap;

			public Bitmap Bitmap
			{
				get
				{
					return bitmap;
				}
				set
				{
					bitmap = value;
				}
			}

			public DrawingBitmap()
			{
				bitmap = new Bitmap(14, 190);
			}

			public void Draw(int hue, int sat)
			{
				float num = 1.268421f;
				float num2 = 241f;
				for (int i = 0; i < 190; i++)
				{
					for (int j = 0; j < 14; j++)
					{
						Color color = HSB.HSB2RGB(hue, sat, (int)num2);
						bitmap.SetPixel(j, i, color);
					}
					num2 -= num;
				}
			}
		}

		private const float step = 1.2751323f;

		private DrawingBitmap bitmap;

		private ColorDialog colorDialog;

		public Color ColorToShow
		{
			set
			{
				HSB.GetHueSaturation(value, out var hue, out var sat);
				bitmap.Draw(hue, sat);
				Invalidate();
				Update();
			}
		}

		public BrightnessControl(ColorDialog colorDialog)
		{
			this.colorDialog = colorDialog;
			bitmap = new DrawingBitmap();
			SuspendLayout();
			base.BorderStyle = BorderStyle.Fixed3D;
			base.Location = new Point(0, 0);
			base.Size = new Size(14, 190);
			base.TabIndex = 0;
			base.TabStop = false;
			base.Size = new Size(14, 190);
			ResumeLayout(performLayout: false);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawImage(bitmap.Bitmap, 0, 0);
			base.OnPaint(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			colorDialog.triangleControl.TrianglePosition = (int)((float)(189 - e.Y) * 1.2751323f);
			base.OnMouseDown(e);
		}

		public void ShowColor(int hue, int sat)
		{
			bitmap.Draw(hue, sat);
			Invalidate();
			Update();
		}
	}

	internal class TriangleControl : Panel
	{
		private const float briStep = 1.2956989f;

		private bool mouseButtonDown;

		private int currentTrianglePosition = 195;

		private static int currentBrightness;

		private ColorDialog colorDialog;

		public static int CurrentBrightness
		{
			get
			{
				return currentBrightness;
			}
			set
			{
				currentBrightness = value;
			}
		}

		public int TrianglePosition
		{
			get
			{
				float num = currentTrianglePosition - 9;
				num *= 1.2956989f;
				return CurrentBrightness = 240 - (int)num;
			}
			set
			{
				float num = (float)value / 1.2956989f;
				currentTrianglePosition = 186 - (int)num + 9;
				colorDialog.briTextBox.Text = TrianglePosition.ToString();
				colorDialog.UpdateFromHSBTextBoxes();
				Invalidate();
				Update();
			}
		}

		public Color ColorToShow
		{
			set
			{
				SetColor(value);
			}
		}

		public TriangleControl(ColorDialog colorDialog)
		{
			this.colorDialog = colorDialog;
			SuspendLayout();
			base.Size = new Size(16, 203);
			ResumeLayout(performLayout: false);
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.UserPaint, value: true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Draw(e);
			base.OnPaint(e);
		}

		private void Draw(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(BackColor), new Rectangle(0, 0, 16, 203));
			Point[] points = new Point[3]
			{
				new Point(0, currentTrianglePosition),
				new Point(8, currentTrianglePosition - 8),
				new Point(8, currentTrianglePosition + 8)
			};
			e.Graphics.FillPolygon(ThemeEngine.Current.ResPool.GetSolidBrush(Color.Black), points);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Y <= 195 && e.Y >= 9)
			{
				mouseButtonDown = true;
				currentTrianglePosition = e.Y;
				colorDialog.briTextBox.Text = TrianglePosition.ToString();
				colorDialog.UpdateFromHSBTextBoxes();
				Invalidate();
				Update();
				base.OnMouseDown(e);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (mouseButtonDown && e.Y < 196 && e.Y > 8)
			{
				currentTrianglePosition = e.Y;
				colorDialog.briTextBox.Text = TrianglePosition.ToString();
				colorDialog.UpdateFromHSBTextBoxes();
				Invalidate();
				Update();
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			mouseButtonDown = false;
			base.OnMouseUp(e);
		}

		public void SetColor(Color color)
		{
			int num = HSB.Brightness(color);
			float num2 = (float)num / 1.2956989f;
			currentTrianglePosition = 186 - (int)num2 + 9;
			if (colorDialog.edit_textbox == null)
			{
				colorDialog.briTextBox.Text = TrianglePosition.ToString();
			}
			Invalidate();
		}
	}

	private bool allowFullOpen = true;

	private bool anyColor;

	private Color color;

	private int[] customColors;

	private bool fullOpen;

	private bool showHelp;

	private bool solidColorOnly;

	private Panel selectedColorPanel;

	private BaseColorControl baseColorControl;

	private ColorMatrixControl colorMatrixControl;

	private BrightnessControl brightnessControl;

	private TriangleControl triangleControl;

	private Button okButton;

	private Button cancelButton;

	private Button helpButton;

	private Button addColoursButton;

	private Button defineColoursButton;

	private TextBox hueTextBox;

	private TextBox satTextBox;

	private TextBox briTextBox;

	private TextBox redTextBox;

	private TextBox greenTextBox;

	private TextBox blueTextBox;

	private Label briLabel;

	private Label satLabel;

	private Label hueLabel;

	private Label colorBaseLabel;

	private Label greenLabel;

	private Label blueLabel;

	private Label redLabel;

	private string textBox_text_old = string.Empty;

	internal TextBox edit_textbox;

	private bool internal_textbox_change;

	public Color Color
	{
		get
		{
			return selectedColorPanel.BackColor;
		}
		set
		{
			if (value.IsEmpty)
			{
				color = Color.Black;
				baseColorControl.SetColor(color);
			}
			else if (color != value)
			{
				color = value;
				baseColorControl.SetColor(color);
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool AllowFullOpen
	{
		get
		{
			return allowFullOpen;
		}
		set
		{
			if (allowFullOpen != value)
			{
				allowFullOpen = value;
				if (!allowFullOpen)
				{
					defineColoursButton.Enabled = false;
				}
				else
				{
					defineColoursButton.Enabled = true;
				}
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool AnyColor
	{
		get
		{
			return anyColor;
		}
		set
		{
			anyColor = value;
		}
	}

	[DefaultValue(false)]
	public virtual bool FullOpen
	{
		get
		{
			return fullOpen;
		}
		set
		{
			if (fullOpen == value)
			{
				return;
			}
			fullOpen = value;
			if (fullOpen && allowFullOpen)
			{
				defineColoursButton.Enabled = false;
				colorMatrixControl.ColorToShow = baseColorControl.ColorToShow;
				form.Size = GetFormSize(fullOpen: true);
				return;
			}
			if (allowFullOpen)
			{
				defineColoursButton.Enabled = true;
			}
			form.Size = GetFormSize(fullOpen: false);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public int[] CustomColors
	{
		get
		{
			return customColors;
		}
		set
		{
			if (value == null)
			{
				ResetCustomColors();
			}
			else
			{
				Array.Copy(value, customColors, value.Length);
			}
			baseColorControl.SetCustomColors();
		}
	}

	[DefaultValue(false)]
	public virtual bool ShowHelp
	{
		get
		{
			return showHelp;
		}
		set
		{
			if (showHelp != value)
			{
				showHelp = value;
				if (showHelp)
				{
					helpButton.Show();
				}
				else
				{
					helpButton.Hide();
				}
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool SolidColorOnly
	{
		get
		{
			return solidColorOnly;
		}
		set
		{
			solidColorOnly = value;
		}
	}

	protected virtual IntPtr Instance => (IntPtr)GetHashCode();

	protected virtual int Options => 0;

	public ColorDialog()
	{
		form = new DialogForm(this);
		form.SuspendLayout();
		form.Text = "Color";
		form.FormBorderStyle = FormBorderStyle.FixedDialog;
		form.MaximizeBox = false;
		satTextBox = new TextBox();
		briTextBox = new TextBox();
		blueTextBox = new TextBox();
		greenTextBox = new TextBox();
		redTextBox = new TextBox();
		hueTextBox = new TextBox();
		redLabel = new Label();
		blueLabel = new Label();
		greenLabel = new Label();
		colorBaseLabel = new Label();
		hueLabel = new Label();
		satLabel = new Label();
		briLabel = new Label();
		okButton = new Button();
		cancelButton = new Button();
		form.CancelButton = cancelButton;
		helpButton = new Button();
		defineColoursButton = new Button();
		addColoursButton = new Button();
		baseColorControl = new BaseColorControl(this);
		colorMatrixControl = new ColorMatrixControl(this);
		brightnessControl = new BrightnessControl(this);
		triangleControl = new TriangleControl(this);
		selectedColorPanel = new Panel();
		hueTextBox.Location = new Point(324, 203);
		hueTextBox.Size = new Size(27, 21);
		hueTextBox.TabIndex = 11;
		hueTextBox.MaxLength = 3;
		satTextBox.Location = new Point(324, 225);
		satTextBox.Size = new Size(27, 21);
		satTextBox.TabIndex = 15;
		satTextBox.MaxLength = 3;
		greenTextBox.Location = new Point(404, 225);
		greenTextBox.Size = new Size(27, 21);
		greenTextBox.TabIndex = 18;
		greenTextBox.MaxLength = 3;
		briTextBox.Location = new Point(324, 247);
		briTextBox.Size = new Size(27, 21);
		briTextBox.TabIndex = 16;
		briTextBox.MaxLength = 3;
		blueTextBox.Location = new Point(404, 247);
		blueTextBox.Size = new Size(27, 21);
		blueTextBox.TabIndex = 19;
		blueTextBox.MaxLength = 3;
		redTextBox.Location = new Point(404, 203);
		redTextBox.Size = new Size(27, 21);
		redTextBox.TabIndex = 17;
		redTextBox.MaxLength = 3;
		redLabel.FlatStyle = FlatStyle.System;
		redLabel.Location = new Point(361, 206);
		redLabel.Size = new Size(40, 16);
		redLabel.TabIndex = 25;
		redLabel.Text = Locale.GetText("Red") + ":";
		redLabel.TextAlign = ContentAlignment.MiddleRight;
		blueLabel.FlatStyle = FlatStyle.System;
		blueLabel.Location = new Point(361, 250);
		blueLabel.Size = new Size(40, 16);
		blueLabel.TabIndex = 26;
		blueLabel.Text = Locale.GetText("Blue") + ":";
		blueLabel.TextAlign = ContentAlignment.MiddleRight;
		greenLabel.FlatStyle = FlatStyle.System;
		greenLabel.Location = new Point(361, 228);
		greenLabel.Size = new Size(40, 16);
		greenLabel.TabIndex = 27;
		greenLabel.Text = Locale.GetText("Green") + ":";
		greenLabel.TextAlign = ContentAlignment.MiddleRight;
		colorBaseLabel.Location = new Point(228, 247);
		colorBaseLabel.Size = new Size(60, 25);
		colorBaseLabel.TabIndex = 28;
		colorBaseLabel.Text = Locale.GetText("Color");
		colorBaseLabel.TextAlign = ContentAlignment.MiddleCenter;
		hueLabel.FlatStyle = FlatStyle.System;
		hueLabel.Location = new Point(287, 206);
		hueLabel.Size = new Size(36, 16);
		hueLabel.TabIndex = 23;
		hueLabel.Text = Locale.GetText("Hue") + ":";
		hueLabel.TextAlign = ContentAlignment.MiddleRight;
		satLabel.FlatStyle = FlatStyle.System;
		satLabel.Location = new Point(287, 228);
		satLabel.Size = new Size(36, 16);
		satLabel.TabIndex = 22;
		satLabel.Text = Locale.GetText("Sat") + ":";
		satLabel.TextAlign = ContentAlignment.MiddleRight;
		briLabel.FlatStyle = FlatStyle.System;
		briLabel.Location = new Point(287, 250);
		briLabel.Size = new Size(36, 16);
		briLabel.TabIndex = 24;
		briLabel.Text = Locale.GetText("Bri") + ":";
		briLabel.TextAlign = ContentAlignment.MiddleRight;
		defineColoursButton.FlatStyle = FlatStyle.System;
		defineColoursButton.Location = new Point(5, 244);
		defineColoursButton.Size = new Size(210, 22);
		defineColoursButton.TabIndex = 6;
		defineColoursButton.Text = "Define Custom Colors >>";
		okButton.FlatStyle = FlatStyle.System;
		okButton.Location = new Point(5, 271);
		okButton.Size = new Size(66, 22);
		okButton.TabIndex = 0;
		okButton.Text = Locale.GetText("OK");
		cancelButton.FlatStyle = FlatStyle.System;
		cancelButton.Location = new Point(78, 271);
		cancelButton.Size = new Size(66, 22);
		cancelButton.TabIndex = 1;
		cancelButton.Text = Locale.GetText("Cancel");
		helpButton.FlatStyle = FlatStyle.System;
		helpButton.Location = new Point(149, 271);
		helpButton.Size = new Size(66, 22);
		helpButton.TabIndex = 5;
		helpButton.Text = Locale.GetText("Help");
		helpButton.Hide();
		addColoursButton.FlatStyle = FlatStyle.System;
		addColoursButton.Location = new Point(227, 271);
		addColoursButton.Size = new Size(213, 22);
		addColoursButton.TabIndex = 7;
		addColoursButton.Text = "Add To Custom Colors";
		baseColorControl.Location = new Point(3, 6);
		baseColorControl.Size = new Size(212, 231);
		baseColorControl.TabIndex = 13;
		colorMatrixControl.Location = new Point(227, 7);
		colorMatrixControl.Size = new Size(179, 190);
		colorMatrixControl.TabIndex = 14;
		triangleControl.Location = new Point(432, 0);
		triangleControl.Size = new Size(16, 204);
		triangleControl.TabIndex = 12;
		brightnessControl.Location = new Point(415, 7);
		brightnessControl.Size = new Size(14, 190);
		brightnessControl.TabIndex = 20;
		selectedColorPanel.BackColor = SystemColors.Desktop;
		selectedColorPanel.BorderStyle = BorderStyle.Fixed3D;
		selectedColorPanel.Location = new Point(227, 202);
		selectedColorPanel.Size = new Size(60, 42);
		selectedColorPanel.TabIndex = 10;
		form.Controls.Add(hueTextBox);
		form.Controls.Add(satTextBox);
		form.Controls.Add(briTextBox);
		form.Controls.Add(redTextBox);
		form.Controls.Add(greenTextBox);
		form.Controls.Add(blueTextBox);
		form.Controls.Add(defineColoursButton);
		form.Controls.Add(okButton);
		form.Controls.Add(cancelButton);
		form.Controls.Add(addColoursButton);
		form.Controls.Add(helpButton);
		form.Controls.Add(baseColorControl);
		form.Controls.Add(colorMatrixControl);
		form.Controls.Add(brightnessControl);
		form.Controls.Add(triangleControl);
		form.Controls.Add(colorBaseLabel);
		form.Controls.Add(greenLabel);
		form.Controls.Add(blueLabel);
		form.Controls.Add(redLabel);
		form.Controls.Add(briLabel);
		form.Controls.Add(hueLabel);
		form.Controls.Add(satLabel);
		form.Controls.Add(selectedColorPanel);
		form.ResumeLayout(performLayout: false);
		Color = Color.Black;
		defineColoursButton.Click += OnClickButtonDefineColours;
		addColoursButton.Click += OnClickButtonAddColours;
		helpButton.Click += OnClickHelpButton;
		cancelButton.Click += OnClickCancelButton;
		okButton.Click += OnClickOkButton;
		hueTextBox.KeyPress += OnKeyPressTextBoxes;
		satTextBox.KeyPress += OnKeyPressTextBoxes;
		briTextBox.KeyPress += OnKeyPressTextBoxes;
		redTextBox.KeyPress += OnKeyPressTextBoxes;
		greenTextBox.KeyPress += OnKeyPressTextBoxes;
		blueTextBox.KeyPress += OnKeyPressTextBoxes;
		hueTextBox.TextChanged += OnTextChangedTextBoxes;
		satTextBox.TextChanged += OnTextChangedTextBoxes;
		briTextBox.TextChanged += OnTextChangedTextBoxes;
		redTextBox.TextChanged += OnTextChangedTextBoxes;
		greenTextBox.TextChanged += OnTextChangedTextBoxes;
		blueTextBox.TextChanged += OnTextChangedTextBoxes;
		hueTextBox.GotFocus += OnGotFocusTextBoxes;
		satTextBox.GotFocus += OnGotFocusTextBoxes;
		briTextBox.GotFocus += OnGotFocusTextBoxes;
		redTextBox.GotFocus += OnGotFocusTextBoxes;
		greenTextBox.GotFocus += OnGotFocusTextBoxes;
		blueTextBox.GotFocus += OnGotFocusTextBoxes;
		hueTextBox.LostFocus += OnLostFocusTextBoxes;
		satTextBox.LostFocus += OnLostFocusTextBoxes;
		briTextBox.LostFocus += OnLostFocusTextBoxes;
		redTextBox.LostFocus += OnLostFocusTextBoxes;
		greenTextBox.LostFocus += OnLostFocusTextBoxes;
		blueTextBox.LostFocus += OnLostFocusTextBoxes;
		ResetCustomColors();
	}

	public override void Reset()
	{
		AllowFullOpen = true;
		anyColor = false;
		Color = Color.Black;
		CustomColors = null;
		FullOpen = false;
		ShowHelp = false;
		solidColorOnly = false;
	}

	public override string ToString()
	{
		return base.ToString() + ",  Color: " + Color.ToString();
	}

	protected override bool RunDialog(IntPtr hwndOwner)
	{
		defineColoursButton.Enabled = AllowFullOpen && !FullOpen;
		defineColoursButton.Refresh();
		form.Size = GetFormSize(FullOpen && AllowFullOpen);
		form.Refresh();
		return true;
	}

	private Size GetFormSize(bool fullOpen)
	{
		if (fullOpen)
		{
			return new Size(448, 332);
		}
		return new Size(221, 332);
	}

	private void OnClickCancelButton(object sender, EventArgs e)
	{
		form.DialogResult = DialogResult.Cancel;
	}

	private void OnClickOkButton(object sender, EventArgs e)
	{
		form.DialogResult = DialogResult.OK;
	}

	private void OnClickButtonAddColours(object sender, EventArgs e)
	{
		baseColorControl.SetUserColor(selectedColorPanel.BackColor);
	}

	private void OnClickButtonDefineColours(object sender, EventArgs e)
	{
		if (allowFullOpen)
		{
			defineColoursButton.Enabled = false;
			colorMatrixControl.ColorToShow = baseColorControl.ColorToShow;
			form.Size = GetFormSize(fullOpen: true);
		}
	}

	private void OnClickHelpButton(object sender, EventArgs e)
	{
		OnHelpRequest(e);
	}

	private void OnGotFocusTextBoxes(object sender, EventArgs e)
	{
		TextBox textBox = sender as TextBox;
		textBox_text_old = textBox.Text;
	}

	private void OnLostFocusTextBoxes(object sender, EventArgs e)
	{
		TextBox textBox = sender as TextBox;
		if (textBox.Text.Length == 0)
		{
			textBox.Text = textBox_text_old;
		}
	}

	private void OnKeyPressTextBoxes(object sender, KeyPressEventArgs e)
	{
		if (char.IsLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar) || char.IsPunctuation(e.KeyChar) || e.KeyChar == ',')
		{
			e.Handled = true;
		}
		else
		{
			internal_textbox_change = true;
		}
	}

	private void OnTextChangedTextBoxes(object sender, EventArgs e)
	{
		if (!internal_textbox_change)
		{
			return;
		}
		internal_textbox_change = false;
		TextBox textBox = sender as TextBox;
		if (textBox.Text.Length == 0)
		{
			return;
		}
		string text = textBox.Text;
		int num = 0;
		try
		{
			num = Convert.ToInt32(text);
		}
		catch (Exception)
		{
		}
		if (sender == hueTextBox)
		{
			if (num > 239)
			{
				hueTextBox.Text = 239.ToString();
			}
			else if (num < 0)
			{
				hueTextBox.Text = 0.ToString();
			}
			edit_textbox = hueTextBox;
			UpdateFromHSBTextBoxes();
			UpdateControls(selectedColorPanel.BackColor);
		}
		else if (sender == satTextBox)
		{
			if (num > 240)
			{
				satTextBox.Text = 240.ToString();
			}
			else if (num < 0)
			{
				satTextBox.Text = 0.ToString();
			}
			edit_textbox = satTextBox;
			UpdateFromHSBTextBoxes();
			UpdateControls(selectedColorPanel.BackColor);
		}
		else if (sender == briTextBox)
		{
			if (num > 240)
			{
				briTextBox.Text = 240.ToString();
			}
			else if (num < 0)
			{
				briTextBox.Text = 0.ToString();
			}
			edit_textbox = briTextBox;
			UpdateFromHSBTextBoxes();
			UpdateControls(selectedColorPanel.BackColor);
		}
		else if (sender == redTextBox)
		{
			if (num > 255)
			{
				redTextBox.Text = 255.ToString();
			}
			else if (num < 0)
			{
				redTextBox.Text = 0.ToString();
			}
			edit_textbox = redTextBox;
			UpdateFromRGBTextBoxes();
		}
		else if (sender == greenTextBox)
		{
			if (num > 255)
			{
				greenTextBox.Text = 255.ToString();
			}
			else if (num < 0)
			{
				greenTextBox.Text = 0.ToString();
			}
			edit_textbox = greenTextBox;
			UpdateFromRGBTextBoxes();
		}
		else if (sender == blueTextBox)
		{
			if (num > 255)
			{
				blueTextBox.Text = 255.ToString();
			}
			else if (num < 0)
			{
				blueTextBox.Text = 0.ToString();
			}
			edit_textbox = blueTextBox;
			UpdateFromRGBTextBoxes();
		}
		textBox_text_old = edit_textbox.Text;
		edit_textbox = null;
	}

	internal void UpdateControls(Color acolor)
	{
		selectedColorPanel.BackColor = acolor;
		colorMatrixControl.ColorToShow = acolor;
		brightnessControl.ColorToShow = acolor;
		triangleControl.ColorToShow = acolor;
	}

	internal void UpdateRGBTextBoxes(Color acolor)
	{
		if (edit_textbox != redTextBox)
		{
			redTextBox.Text = acolor.R.ToString();
		}
		if (edit_textbox != greenTextBox)
		{
			greenTextBox.Text = acolor.G.ToString();
		}
		if (edit_textbox != blueTextBox)
		{
			blueTextBox.Text = acolor.B.ToString();
		}
	}

	internal void UpdateHSBTextBoxes(Color acolor)
	{
		HSB hSB = HSB.RGB2HSB(acolor);
		if (edit_textbox != hueTextBox)
		{
			hueTextBox.Text = hSB.hue.ToString();
		}
		if (edit_textbox != satTextBox)
		{
			satTextBox.Text = hSB.sat.ToString();
		}
		if (edit_textbox != briTextBox)
		{
			briTextBox.Text = hSB.bri.ToString();
		}
	}

	internal void UpdateFromHSBTextBoxes()
	{
		Color color = HSB.HSB2RGB(Convert.ToInt32(hueTextBox.Text), Convert.ToInt32(satTextBox.Text), Convert.ToInt32(briTextBox.Text));
		selectedColorPanel.BackColor = color;
		UpdateRGBTextBoxes(color);
	}

	internal void UpdateFromRGBTextBoxes()
	{
		Color color = Color.FromArgb(Convert.ToInt32(redTextBox.Text), Convert.ToInt32(greenTextBox.Text), Convert.ToInt32(blueTextBox.Text));
		selectedColorPanel.BackColor = color;
		UpdateHSBTextBoxes(color);
		UpdateControls(color);
	}

	private void ResetCustomColors()
	{
		if (customColors == null)
		{
			customColors = new int[16];
		}
		int num = Color.FromArgb(0, 255, 255, 255).ToArgb();
		for (int i = 0; i < customColors.Length; i++)
		{
			customColors[i] = num;
		}
	}
}

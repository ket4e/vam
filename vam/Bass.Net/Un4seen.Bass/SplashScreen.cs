using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Un4seen.Bass;

internal class SplashScreen : Form
{
	private PictureBox pictureBoxLogo;

	private Label labelCopyrightNote;

	private Label labelCopyright;

	private Panel panelAll;

	private Label labelVersion;

	private ToolTip toolTip;

	private IContainer components;

	private Label labelMessage;

	private bool _close = true;

	internal int _waitTime = 4000;

	private int _moveX;

	private int _moveY;

	public SplashScreen(bool close, int wait)
	{
		_close = close;
		if (!close && wait > 0)
		{
			_waitTime = wait;
		}
		InitializeComponent();
		if (close)
		{
			toolTip.SetToolTip(pictureBoxLogo, null);
			pictureBoxLogo.Cursor = Cursors.Default;
		}
		else
		{
			toolTip.SetToolTip(pictureBoxLogo, "Close");
			pictureBoxLogo.Cursor = Cursors.Hand;
		}
		labelVersion.Text += Assembly.GetExecutingAssembly().GetName().Version.ToString();
		if (string.IsNullOrEmpty(BassNet._eMail))
		{
			labelMessage.Text = "Unregistered Freeware Version";
		}
		else
		{
			labelMessage.Text = "Version registered for: " + BassNet._eMail;
		}
		if (close)
		{
			Console.WriteLine("**********************************************************");
			Console.WriteLine("* " + labelVersion.Text + " *");
			Console.WriteLine("*       Freeware version - For personal use only!        *");
			Console.WriteLine("* " + labelCopyright.Text + " *");
			Console.WriteLine("**********************************************************");
		}
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Un4seen.Bass.SplashScreen));
		this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
		this.labelCopyrightNote = new System.Windows.Forms.Label();
		this.labelCopyright = new System.Windows.Forms.Label();
		this.panelAll = new System.Windows.Forms.Panel();
		this.labelVersion = new System.Windows.Forms.Label();
		this.labelMessage = new System.Windows.Forms.Label();
		this.toolTip = new System.Windows.Forms.ToolTip(this.components);
		((System.ComponentModel.ISupportInitialize)this.pictureBoxLogo).BeginInit();
		this.panelAll.SuspendLayout();
		base.SuspendLayout();
		this.pictureBoxLogo.Cursor = System.Windows.Forms.Cursors.Hand;
		this.pictureBoxLogo.Dock = System.Windows.Forms.DockStyle.Top;
		this.pictureBoxLogo.ErrorImage = null;
		this.pictureBoxLogo.Image = (System.Drawing.Image)resources.GetObject("pictureBoxLogo.Image");
		this.pictureBoxLogo.InitialImage = null;
		this.pictureBoxLogo.Location = new System.Drawing.Point(0, 0);
		this.pictureBoxLogo.Name = "pictureBoxLogo";
		this.pictureBoxLogo.Size = new System.Drawing.Size(288, 115);
		this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBoxLogo.TabIndex = 0;
		this.pictureBoxLogo.TabStop = false;
		this.pictureBoxLogo.Click += new System.EventHandler(pictureBoxLogo_Click);
		this.pictureBoxLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseDown);
		this.pictureBoxLogo.MouseMove += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseMove);
		this.pictureBoxLogo.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseUp);
		this.labelCopyrightNote.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.labelCopyrightNote.FlatStyle = System.Windows.Forms.FlatStyle.System;
		this.labelCopyrightNote.Font = new System.Drawing.Font("Tahoma", 6.75f);
		this.labelCopyrightNote.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.labelCopyrightNote.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.labelCopyrightNote.Location = new System.Drawing.Point(4, 132);
		this.labelCopyrightNote.Name = "labelCopyrightNote";
		this.labelCopyrightNote.Size = new System.Drawing.Size(280, 12);
		this.labelCopyrightNote.TabIndex = 11;
		this.labelCopyrightNote.Text = "This software is protected by international law. All rights reserved.";
		this.labelCopyrightNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.labelCopyrightNote.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseDown);
		this.labelCopyrightNote.MouseMove += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseMove);
		this.labelCopyrightNote.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseUp);
		this.labelCopyright.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.labelCopyright.Cursor = System.Windows.Forms.Cursors.Default;
		this.labelCopyright.FlatStyle = System.Windows.Forms.FlatStyle.System;
		this.labelCopyright.Font = new System.Drawing.Font("Tahoma", 6.75f);
		this.labelCopyright.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.labelCopyright.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.labelCopyright.Location = new System.Drawing.Point(4, 118);
		this.labelCopyright.Name = "labelCopyright";
		this.labelCopyright.Size = new System.Drawing.Size(280, 12);
		this.labelCopyright.TabIndex = 10;
		this.labelCopyright.Text = "© 2018 www.bass.radio42.com  :  BASS by www.un4seen.com";
		this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.toolTip.SetToolTip(this.labelCopyright, "radio42 : www.bass.radio42.com");
		this.labelCopyright.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseDown);
		this.labelCopyright.MouseMove += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseMove);
		this.labelCopyright.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseUp);
		this.panelAll.BackColor = System.Drawing.Color.Transparent;
		this.panelAll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.panelAll.Controls.Add(this.labelVersion);
		this.panelAll.Controls.Add(this.labelMessage);
		this.panelAll.Controls.Add(this.labelCopyrightNote);
		this.panelAll.Controls.Add(this.labelCopyright);
		this.panelAll.Controls.Add(this.pictureBoxLogo);
		this.panelAll.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelAll.Location = new System.Drawing.Point(0, 0);
		this.panelAll.Name = "panelAll";
		this.panelAll.Size = new System.Drawing.Size(290, 190);
		this.panelAll.TabIndex = 12;
		this.labelVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.labelVersion.Cursor = System.Windows.Forms.Cursors.Hand;
		this.labelVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
		this.labelVersion.Font = new System.Drawing.Font("Tahoma", 6.75f);
		this.labelVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.labelVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.labelVersion.Location = new System.Drawing.Point(4, 172);
		this.labelVersion.Name = "labelVersion";
		this.labelVersion.Size = new System.Drawing.Size(280, 12);
		this.labelVersion.TabIndex = 13;
		this.labelVersion.Text = "BASS.NET API  by Bernd Niedergesaess - Version ";
		this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.toolTip.SetToolTip(this.labelVersion, "radio42 : www.bass.radio42.com");
		this.labelVersion.Click += new System.EventHandler(labelVersion_Click);
		this.labelMessage.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.labelMessage.FlatStyle = System.Windows.Forms.FlatStyle.System;
		this.labelMessage.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelMessage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.labelMessage.Location = new System.Drawing.Point(4, 150);
		this.labelMessage.Name = "labelMessage";
		this.labelMessage.Size = new System.Drawing.Size(280, 16);
		this.labelMessage.TabIndex = 12;
		this.labelMessage.Text = "Freeware version - For personal use only!";
		this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.labelMessage.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseDown);
		this.labelMessage.MouseMove += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseMove);
		this.labelMessage.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseUp);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(290, 190);
		base.ControlBox = false;
		base.Controls.Add(this.panelAll);
		this.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SplashScreen";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "BASS.NET API";
		base.TopMost = true;
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(SplashScreen_KeyDown);
		base.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseDown);
		base.MouseMove += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseMove);
		base.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureBoxLogo_MouseUp);
		((System.ComponentModel.ISupportInitialize)this.pictureBoxLogo).EndInit();
		this.panelAll.ResumeLayout(false);
		base.ResumeLayout(false);
	}

	internal void SetClose(bool close)
	{
		_close = close;
		if (close)
		{
			toolTip.SetToolTip(pictureBoxLogo, null);
			pictureBoxLogo.Cursor = Cursors.Default;
		}
		else
		{
			toolTip.SetToolTip(pictureBoxLogo, "Close");
			pictureBoxLogo.Cursor = Cursors.Hand;
		}
	}

	internal void SetOpacity(double opacity)
	{
		base.Opacity = opacity;
	}

	internal void SetPosition(int pos)
	{
		switch (pos)
		{
		case 1:
			base.StartPosition = FormStartPosition.WindowsDefaultLocation;
			break;
		case 2:
			base.StartPosition = FormStartPosition.CenterParent;
			break;
		default:
			base.StartPosition = FormStartPosition.CenterScreen;
			break;
		}
	}

	private void pictureBoxLogo_Click(object sender, EventArgs e)
	{
		if (!_close)
		{
			Close();
		}
	}

	private void SplashScreen_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape && !_close)
		{
			Close();
		}
	}

	private void labelVersion_Click(object sender, EventArgs e)
	{
		try
		{
			Process process = new Process();
			process.StartInfo.FileName = "http://www.bass.radio42.com/";
			process.Start();
		}
		catch
		{
		}
	}

	private void pictureBoxLogo_MouseDown(object sender, MouseEventArgs e)
	{
		_moveX = e.X;
		_moveY = e.Y;
	}

	private void pictureBoxLogo_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			base.Location = new Point(base.Location.X - (_moveX - e.X), base.Location.Y - (_moveY - e.Y));
		}
	}

	private void pictureBoxLogo_MouseUp(object sender, MouseEventArgs e)
	{
		_moveX = 0;
		_moveY = 0;
	}
}

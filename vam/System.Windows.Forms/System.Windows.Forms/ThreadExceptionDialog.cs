using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class ThreadExceptionDialog : Form
{
	private Exception e;

	private bool details;

	private Button buttonIgnore;

	private Button buttonAbort;

	private Button buttonDetails;

	private Label labelException;

	private Label label1;

	private TextBox textBoxDetails;

	private Label helpText;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override bool AutoSize
	{
		get
		{
			return base.AutoSize;
		}
		set
		{
			base.AutoSize = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler AutoSizeChanged
	{
		add
		{
			base.AutoSizeChanged += value;
		}
		remove
		{
			base.AutoSizeChanged -= value;
		}
	}

	public ThreadExceptionDialog(Exception t)
	{
		e = t;
		InitializeComponent();
		labelException.Text = t.Message;
		if (Form.ActiveForm != null)
		{
			Text = Form.ActiveForm.Text;
		}
		else
		{
			Text = "Mono";
		}
		buttonAbort.Enabled = Application.AllowQuit;
		RefreshDetails();
		FillExceptionDetails();
	}

	private void InitializeComponent()
	{
		this.helpText = new System.Windows.Forms.Label();
		this.buttonAbort = new System.Windows.Forms.Button();
		this.buttonIgnore = new System.Windows.Forms.Button();
		this.buttonDetails = new System.Windows.Forms.Button();
		this.labelException = new System.Windows.Forms.Label();
		this.textBoxDetails = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.helpText.Location = new System.Drawing.Point(60, 8);
		this.helpText.Name = "helpText";
		this.helpText.Size = new System.Drawing.Size(356, 40);
		this.helpText.TabIndex = 0;
		this.helpText.Text = "An unhandled exception has occurred in you application. If you click Ignore the application will ignore this error and attempt to continue. If you click Abort, the application will quit immediately.";
		this.buttonAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
		this.buttonAbort.Location = new System.Drawing.Point(332, 112);
		this.buttonAbort.Name = "buttonAbort";
		this.buttonAbort.Size = new System.Drawing.Size(85, 23);
		this.buttonAbort.TabIndex = 4;
		this.buttonAbort.Text = "&Abort";
		this.buttonAbort.Click += new System.EventHandler(buttonAbort_Click);
		this.buttonIgnore.DialogResult = System.Windows.Forms.DialogResult.Ignore;
		this.buttonIgnore.Location = new System.Drawing.Point(236, 112);
		this.buttonIgnore.Name = "buttonIgnore";
		this.buttonIgnore.Size = new System.Drawing.Size(85, 23);
		this.buttonIgnore.TabIndex = 3;
		this.buttonIgnore.Text = "&Ignore";
		this.buttonDetails.Location = new System.Drawing.Point(140, 112);
		this.buttonDetails.Name = "buttonDetails";
		this.buttonDetails.Size = new System.Drawing.Size(85, 23);
		this.buttonDetails.TabIndex = 2;
		this.buttonDetails.Text = "Show &Details";
		this.buttonDetails.Click += new System.EventHandler(buttonDetails_Click);
		this.labelException.Location = new System.Drawing.Point(60, 64);
		this.labelException.Name = "labelException";
		this.labelException.Size = new System.Drawing.Size(356, 32);
		this.labelException.TabIndex = 1;
		this.textBoxDetails.Location = new System.Drawing.Point(8, 168);
		this.textBoxDetails.Multiline = true;
		this.textBoxDetails.Name = "textBoxDetails";
		this.textBoxDetails.ReadOnly = true;
		this.textBoxDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
		this.textBoxDetails.Size = new System.Drawing.Size(408, 196);
		this.textBoxDetails.TabIndex = 5;
		this.textBoxDetails.TabStop = false;
		this.textBoxDetails.Text = string.Empty;
		this.textBoxDetails.WordWrap = false;
		this.label1.Location = new System.Drawing.Point(8, 148);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(100, 16);
		this.label1.TabIndex = 0;
		this.label1.Text = "Exception details";
		base.AcceptButton = this.buttonIgnore;
		base.CancelButton = this.buttonAbort;
		base.ClientSize = new System.Drawing.Size(428, 374);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.textBoxDetails);
		base.Controls.Add(this.labelException);
		base.Controls.Add(this.buttonDetails);
		base.Controls.Add(this.buttonIgnore);
		base.Controls.Add(this.buttonAbort);
		base.Controls.Add(this.helpText);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ThreadExceptionDialog";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		base.TopMost = true;
		base.Paint += new System.Windows.Forms.PaintEventHandler(PaintHandler);
		base.ResumeLayout(false);
	}

	private void buttonDetails_Click(object sender, EventArgs e)
	{
		details = !details;
		RefreshDetails();
	}

	private void FillExceptionDetails()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(e.ToString());
		stringBuilder.Append(Environment.NewLine + Environment.NewLine);
		stringBuilder.Append("Loaded assemblies:" + Environment.NewLine + Environment.NewLine);
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			AssemblyName assemblyName = assembly.GetName();
			stringBuilder.AppendFormat("Name:\t{0}" + Environment.NewLine, assemblyName.Name);
			stringBuilder.AppendFormat("Version:\t{0}" + Environment.NewLine, assemblyName.Version);
			stringBuilder.AppendFormat("Location:\t{0}" + Environment.NewLine, assemblyName.CodeBase);
			stringBuilder.Append(Environment.NewLine);
		}
		textBoxDetails.Text = stringBuilder.ToString();
	}

	private void RefreshDetails()
	{
		if (details)
		{
			buttonDetails.Text = "Hide &Details";
			base.Height = 410;
			label1.Visible = true;
			textBoxDetails.Visible = true;
		}
		else
		{
			buttonDetails.Text = "Show &Details";
			label1.Visible = false;
			textBoxDetails.Visible = false;
			base.Height = 180;
		}
	}

	private void buttonAbort_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void PaintHandler(object o, PaintEventArgs args)
	{
		Graphics graphics = args.Graphics;
		graphics.DrawIcon(SystemIcons.Error, 15, 10);
	}
}

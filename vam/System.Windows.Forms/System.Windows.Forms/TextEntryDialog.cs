using System.Drawing;

namespace System.Windows.Forms;

internal class TextEntryDialog : Form
{
	private Label label1;

	private Button okButton;

	private TextBox newNameTextBox;

	private PictureBox iconPictureBox;

	private Button cancelButton;

	private GroupBox groupBox1;

	public Image IconPictureBoxImage
	{
		set
		{
			iconPictureBox.Image = value;
		}
	}

	public string FileName
	{
		get
		{
			return newNameTextBox.Text;
		}
		set
		{
			newNameTextBox.Text = value;
		}
	}

	public TextEntryDialog()
	{
		groupBox1 = new GroupBox();
		cancelButton = new Button();
		iconPictureBox = new PictureBox();
		newNameTextBox = new TextBox();
		okButton = new Button();
		label1 = new Label();
		groupBox1.SuspendLayout();
		SuspendLayout();
		groupBox1.Controls.Add(newNameTextBox);
		groupBox1.Controls.Add(label1);
		groupBox1.Controls.Add(iconPictureBox);
		groupBox1.Location = new Point(8, 8);
		groupBox1.Size = new Size(232, 160);
		groupBox1.TabIndex = 5;
		groupBox1.TabStop = false;
		groupBox1.Text = "New Name";
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.Location = new Point(168, 176);
		cancelButton.TabIndex = 4;
		cancelButton.Text = "Cancel";
		iconPictureBox.BorderStyle = BorderStyle.Fixed3D;
		iconPictureBox.Location = new Point(86, 24);
		iconPictureBox.Size = new Size(60, 60);
		iconPictureBox.TabIndex = 3;
		iconPictureBox.TabStop = false;
		iconPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
		newNameTextBox.Location = new Point(16, 128);
		newNameTextBox.Size = new Size(200, 20);
		newNameTextBox.TabIndex = 5;
		newNameTextBox.Text = string.Empty;
		okButton.DialogResult = DialogResult.OK;
		okButton.Location = new Point(80, 176);
		okButton.TabIndex = 3;
		okButton.Text = "OK";
		label1.Location = new Point(16, 96);
		label1.Size = new Size(200, 23);
		label1.TabIndex = 4;
		label1.Text = "Enter Name:";
		label1.TextAlign = ContentAlignment.MiddleCenter;
		base.AcceptButton = okButton;
		AutoScaleBaseSize = new Size(5, 13);
		base.CancelButton = cancelButton;
		base.ClientSize = new Size(248, 205);
		base.Controls.Add(groupBox1);
		base.Controls.Add(cancelButton);
		base.Controls.Add(okButton);
		base.FormBorderStyle = FormBorderStyle.FixedDialog;
		Text = "New Folder or File";
		groupBox1.ResumeLayout(performLayout: false);
		ResumeLayout(performLayout: false);
		newNameTextBox.Select();
	}
}

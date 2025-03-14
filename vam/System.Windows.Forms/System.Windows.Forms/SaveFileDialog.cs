using System.ComponentModel;
using System.IO;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.SaveFileDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public sealed class SaveFileDialog : FileDialog
{
	[DefaultValue(false)]
	public bool CreatePrompt
	{
		get
		{
			return createPrompt;
		}
		set
		{
			createPrompt = value;
		}
	}

	[DefaultValue(true)]
	public bool OverwritePrompt
	{
		get
		{
			return overwritePrompt;
		}
		set
		{
			overwritePrompt = value;
		}
	}

	internal override string DialogTitle
	{
		get
		{
			string text = base.DialogTitle;
			if (text.Length == 0)
			{
				text = "Save As";
			}
			return text;
		}
	}

	public SaveFileDialog()
	{
		form.SuspendLayout();
		form.Text = "Save As";
		base.FileTypeLabel = "Save as type:";
		base.OpenSaveButtonText = "Save";
		base.SearchSaveLabel = "Save in:";
		fileDialogType = FileDialogType.SaveFileDialog;
		form.ResumeLayout(performLayout: false);
	}

	public Stream OpenFile()
	{
		if (base.FileName == null)
		{
			throw new ArgumentNullException("OpenFile", "FileName is null");
		}
		try
		{
			return new FileStream(base.FileName, FileMode.Create, FileAccess.ReadWrite);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public override void Reset()
	{
		base.Reset();
		overwritePrompt = true;
		createPrompt = false;
	}
}

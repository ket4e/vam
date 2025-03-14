using System.ComponentModel;
using System.IO;

namespace System.Windows.Forms;

public sealed class OpenFileDialog : FileDialog
{
	[DefaultValue(true)]
	public override bool CheckFileExists
	{
		get
		{
			return base.CheckFileExists;
		}
		set
		{
			base.CheckFileExists = value;
		}
	}

	[DefaultValue(false)]
	public bool Multiselect
	{
		get
		{
			return base.BMultiSelect;
		}
		set
		{
			base.BMultiSelect = value;
		}
	}

	[DefaultValue(false)]
	public new bool ReadOnlyChecked
	{
		get
		{
			return base.ReadOnlyChecked;
		}
		set
		{
			base.ReadOnlyChecked = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string SafeFileName => Path.GetFileName(base.FileName);

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public string[] SafeFileNames
	{
		get
		{
			string[] array = base.FileNames;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Path.GetFileName(array[i]);
			}
			return array;
		}
	}

	[DefaultValue(false)]
	public new bool ShowReadOnly
	{
		get
		{
			return base.ShowReadOnly;
		}
		set
		{
			base.ShowReadOnly = value;
		}
	}

	internal override string DialogTitle
	{
		get
		{
			string text = base.DialogTitle;
			if (text.Length == 0)
			{
				text = "Open";
			}
			return text;
		}
	}

	public OpenFileDialog()
	{
		form.SuspendLayout();
		form.Text = "Open";
		CheckFileExists = true;
		base.OpenSaveButtonText = "Open";
		base.SearchSaveLabel = "Look in:";
		fileDialogType = FileDialogType.OpenFileDialog;
		form.ResumeLayout(performLayout: false);
	}

	public Stream OpenFile()
	{
		if (base.FileName.Length == 0)
		{
			throw new ArgumentNullException("OpenFile", "FileName is null");
		}
		return new FileStream(base.FileName, FileMode.Open, FileAccess.Read);
	}

	public override void Reset()
	{
		base.Reset();
		base.BMultiSelect = false;
		base.CheckFileExists = true;
		base.ReadOnlyChecked = false;
		base.ShowReadOnly = false;
	}
}

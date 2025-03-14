namespace System.Windows.Forms.WebBrowserDialogs;

internal class ConfirmCheck : Generic
{
	private bool check;

	public bool Checked => check;

	public ConfirmCheck(string title, string text, string checkMessage, bool checkState)
		: base(title)
	{
		InitTable(3, 2);
		AddLabel(0, 0, 2, text, -1, -1);
		AddCheck(1, 0, 2, checkMessage, checkState, -1, -1, CheckedChanged);
		AddButton(2, 0, 0, Locale.GetText("OK"), -1, -1, isAccept: true, isCancel: false, OkClick);
		AddButton(2, 1, 0, Locale.GetText("Cancel"), -1, -1, isAccept: false, isCancel: true, CancelClick);
	}

	private void OkClick(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void CancelClick(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void CheckedChanged(object sender, EventArgs e)
	{
		CheckBox checkBox = sender as CheckBox;
		check = checkBox.Checked;
	}
}

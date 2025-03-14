namespace System.Windows.Forms.WebBrowserDialogs;

internal class AlertCheck : Generic
{
	private bool check;

	public bool Checked => check;

	public AlertCheck(string title, string text, string checkMessage, bool checkState)
		: base(title)
	{
		InitTable(3, 1);
		AddLabel(0, 0, 0, text, -1, -1);
		AddCheck(1, 0, 0, checkMessage, checkState, -1, -1, CheckedChanged);
		AddButton(2, 0, 0, "OK", -1, -1, isAccept: true, isCancel: false, OkClick);
	}

	private void OkClick(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void CheckedChanged(object sender, EventArgs e)
	{
		CheckBox checkBox = sender as CheckBox;
		check = checkBox.Checked;
	}
}

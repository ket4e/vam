namespace System.Windows.Forms.WebBrowserDialogs;

internal class Prompt : Generic
{
	private string text;

	public new string Text => text;

	public Prompt(string title, string message, string text)
		: base(title)
	{
		InitTable(3, 1);
		AddLabel(0, 0, 0, message, -1, -1);
		AddText(1, 0, 0, text, -1, -1, onText);
		AddButton(2, 0, 0, Locale.GetText("OK"), -1, -1, isAccept: true, isCancel: false, OkClick);
	}

	private void OkClick(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void onText(object sender, EventArgs e)
	{
		TextBox textBox = sender as TextBox;
		text = textBox.Text;
	}
}

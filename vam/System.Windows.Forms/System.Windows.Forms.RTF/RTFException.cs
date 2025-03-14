using System.Text;

namespace System.Windows.Forms.RTF;

internal class RTFException : ApplicationException
{
	private int pos;

	private int line;

	private TokenClass token_class;

	private Major major;

	private Minor minor;

	private int param;

	private string text;

	private string error_message;

	public override string Message
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(error_message);
			stringBuilder.Append("\n");
			stringBuilder.Append("RTF Stream Info: Pos:" + pos + " Line:" + line);
			stringBuilder.Append("\n");
			stringBuilder.Append(string.Concat("TokenClass:", token_class, ", "));
			stringBuilder.Append("Major:" + $"{(int)major}" + ", ");
			stringBuilder.Append("Minor:" + $"{(int)minor}" + ", ");
			stringBuilder.Append("Param:" + $"{param}" + ", ");
			stringBuilder.Append("Text:" + text);
			return stringBuilder.ToString();
		}
	}

	public RTFException(RTF rtf, string error_message)
	{
		pos = rtf.LinePos;
		line = rtf.LineNumber;
		token_class = rtf.TokenClass;
		major = rtf.Major;
		minor = rtf.Minor;
		param = rtf.Param;
		text = rtf.Text;
		this.error_message = error_message;
	}
}

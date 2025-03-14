using System.Globalization;

namespace System.CodeDom.Compiler;

[Serializable]
public class CompilerError
{
	private string fileName;

	private int line;

	private int column;

	private string errorNumber;

	private string errorText;

	private bool isWarning;

	public int Line
	{
		get
		{
			return line;
		}
		set
		{
			line = value;
		}
	}

	public int Column
	{
		get
		{
			return column;
		}
		set
		{
			column = value;
		}
	}

	public string ErrorNumber
	{
		get
		{
			return errorNumber;
		}
		set
		{
			errorNumber = value;
		}
	}

	public string ErrorText
	{
		get
		{
			return errorText;
		}
		set
		{
			errorText = value;
		}
	}

	public bool IsWarning
	{
		get
		{
			return isWarning;
		}
		set
		{
			isWarning = value;
		}
	}

	public string FileName
	{
		get
		{
			return fileName;
		}
		set
		{
			fileName = value;
		}
	}

	public CompilerError()
		: this(string.Empty, 0, 0, string.Empty, string.Empty)
	{
	}

	public CompilerError(string fileName, int line, int column, string errorNumber, string errorText)
	{
		this.fileName = fileName;
		this.line = line;
		this.column = column;
		this.errorNumber = errorNumber;
		this.errorText = errorText;
	}

	public override string ToString()
	{
		string text = ((!isWarning) ? "error" : "warning");
		return string.Format(CultureInfo.InvariantCulture, "{0}({1},{2}) : {3} {4}: {5}", fileName, line, column, text, errorNumber, errorText);
	}
}

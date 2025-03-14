using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeLinePragma
{
	private string fileName;

	private int lineNumber;

	public string FileName
	{
		get
		{
			if (fileName == null)
			{
				return string.Empty;
			}
			return fileName;
		}
		set
		{
			fileName = value;
		}
	}

	public int LineNumber
	{
		get
		{
			return lineNumber;
		}
		set
		{
			lineNumber = value;
		}
	}

	public CodeLinePragma()
	{
	}

	public CodeLinePragma(string fileName, int lineNumber)
	{
		this.fileName = fileName;
		this.lineNumber = lineNumber;
	}
}

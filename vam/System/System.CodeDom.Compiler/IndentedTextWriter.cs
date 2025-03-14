using System.IO;
using System.Security.Permissions;
using System.Text;

namespace System.CodeDom.Compiler;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class IndentedTextWriter : TextWriter
{
	public const string DefaultTabString = "    ";

	private TextWriter writer;

	private string tabString;

	private int indent;

	private bool newline;

	public override Encoding Encoding => writer.Encoding;

	public int Indent
	{
		get
		{
			return indent;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			indent = value;
		}
	}

	public TextWriter InnerWriter => writer;

	public override string NewLine
	{
		get
		{
			return writer.NewLine;
		}
		set
		{
			writer.NewLine = value;
		}
	}

	public IndentedTextWriter(TextWriter writer)
	{
		this.writer = writer;
		tabString = "    ";
		newline = true;
	}

	public IndentedTextWriter(TextWriter writer, string tabString)
	{
		this.writer = writer;
		this.tabString = tabString;
		newline = true;
	}

	public override void Close()
	{
		writer.Close();
	}

	public override void Flush()
	{
		writer.Flush();
	}

	public override void Write(bool value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(char value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(char[] value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(double value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(int value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(long value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(object value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(float value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(string value)
	{
		OutputTabs();
		writer.Write(value);
	}

	public override void Write(string format, object arg)
	{
		OutputTabs();
		writer.Write(format, arg);
	}

	public override void Write(string format, params object[] args)
	{
		OutputTabs();
		writer.Write(format, args);
	}

	public override void Write(char[] buffer, int index, int count)
	{
		OutputTabs();
		writer.Write(buffer, index, count);
	}

	public override void Write(string format, object arg0, object arg1)
	{
		OutputTabs();
		writer.Write(format, arg0, arg1);
	}

	public override void WriteLine()
	{
		OutputTabs();
		writer.WriteLine();
		newline = true;
	}

	public override void WriteLine(bool value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(char value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(char[] value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(double value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(int value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(long value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(object value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(float value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(string value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	[CLSCompliant(false)]
	public override void WriteLine(uint value)
	{
		OutputTabs();
		writer.WriteLine(value);
		newline = true;
	}

	public override void WriteLine(string format, object arg)
	{
		OutputTabs();
		writer.WriteLine(format, arg);
		newline = true;
	}

	public override void WriteLine(string format, params object[] args)
	{
		OutputTabs();
		writer.WriteLine(format, args);
		newline = true;
	}

	public override void WriteLine(char[] buffer, int index, int count)
	{
		OutputTabs();
		writer.WriteLine(buffer, index, count);
		newline = true;
	}

	public override void WriteLine(string format, object arg0, object arg1)
	{
		OutputTabs();
		writer.WriteLine(format, arg0, arg1);
		newline = true;
	}

	public void WriteLineNoTabs(string value)
	{
		writer.WriteLine(value);
		newline = true;
	}

	protected virtual void OutputTabs()
	{
		if (newline)
		{
			for (int i = 0; i < indent; i++)
			{
				writer.Write(tabString);
			}
			newline = false;
		}
	}
}

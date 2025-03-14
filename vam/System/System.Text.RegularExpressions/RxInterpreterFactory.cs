using System.Collections;

namespace System.Text.RegularExpressions;

internal class RxInterpreterFactory : System.Text.RegularExpressions.IMachineFactory
{
	private IDictionary mapping;

	private byte[] program;

	private System.Text.RegularExpressions.EvalDelegate eval_del;

	private string[] namesMapping;

	private int gap;

	public int GroupCount => program[1] | (program[2] << 8);

	public int Gap
	{
		get
		{
			return gap;
		}
		set
		{
			gap = value;
		}
	}

	public IDictionary Mapping
	{
		get
		{
			return mapping;
		}
		set
		{
			mapping = value;
		}
	}

	public string[] NamesMapping
	{
		get
		{
			return namesMapping;
		}
		set
		{
			namesMapping = value;
		}
	}

	public RxInterpreterFactory(byte[] program, System.Text.RegularExpressions.EvalDelegate eval_del)
	{
		this.program = program;
		this.eval_del = eval_del;
	}

	public System.Text.RegularExpressions.IMachine NewInstance()
	{
		return new System.Text.RegularExpressions.RxInterpreter(program, eval_del);
	}
}

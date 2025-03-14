using System.Data;

namespace Mono.Data.SqlExpressions;

internal class TrimFunction : StringFunction
{
	public TrimFunction(IExpression e)
		: base(e)
	{
	}

	public override object Eval(DataRow row)
	{
		return ((string)base.Eval(row))?.Trim();
	}
}

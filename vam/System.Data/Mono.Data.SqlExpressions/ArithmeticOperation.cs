using System;
using System.Data;

namespace Mono.Data.SqlExpressions;

internal class ArithmeticOperation : BinaryOpExpression
{
	public ArithmeticOperation(Operation op, IExpression e1, IExpression e2)
		: base(op, e1, e2)
	{
	}

	public override object Eval(DataRow row)
	{
		object obj = expr1.Eval(row);
		if (obj == DBNull.Value || obj == null)
		{
			return obj;
		}
		object obj2 = expr2.Eval(row);
		if (obj2 == DBNull.Value || obj2 == null)
		{
			return obj2;
		}
		if (op == Operation.ADD && (obj is string || obj2 is string))
		{
			return obj.ToString() + obj2.ToString();
		}
		IConvertible o = (IConvertible)obj;
		IConvertible o2 = (IConvertible)obj2;
		return op switch
		{
			Operation.ADD => Numeric.Add(o, o2), 
			Operation.SUB => Numeric.Subtract(o, o2), 
			Operation.MUL => Numeric.Multiply(o, o2), 
			Operation.DIV => Numeric.Divide(o, o2), 
			Operation.MOD => Numeric.Modulo(o, o2), 
			_ => 0, 
		};
	}
}

using System;
using System.Collections;
using System.Data;

namespace Mono.Data.SqlExpressions;

internal class In : UnaryExpression
{
	private IList set;

	public In(IExpression e, IList set)
		: base(e)
	{
		this.set = set;
	}

	public override bool Equals(object obj)
	{
		if (!base.Equals(obj))
		{
			return false;
		}
		if (!(obj is In))
		{
			return false;
		}
		In @in = (In)obj;
		if (@in.set.Count != set.Count)
		{
			return false;
		}
		int i = 0;
		for (int count = set.Count; i < count; i++)
		{
			object obj2 = set[i];
			object obj3 = @in.set[i];
			if (obj2 == null && obj3 != null)
			{
				return false;
			}
			if (!obj2.Equals(obj3))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = base.GetHashCode();
		int i = 0;
		for (int count = set.Count; i < count; i++)
		{
			object obj = set[i];
			if (obj != null)
			{
				num ^= obj.GetHashCode();
			}
		}
		return num;
	}

	public override object Eval(DataRow row)
	{
		object obj = expr.Eval(row);
		if (obj == DBNull.Value)
		{
			return obj;
		}
		if (!(obj is IComparable o))
		{
			return false;
		}
		foreach (IExpression item in set)
		{
			IComparable comparable = (IComparable)item.Eval(row);
			if (comparable == null || Comparison.Compare(o, comparable, row.Table.CaseSensitive) != 0)
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public override bool EvalBoolean(DataRow row)
	{
		return (bool)Eval(row);
	}
}

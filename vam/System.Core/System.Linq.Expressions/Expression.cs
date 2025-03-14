using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace System.Linq.Expressions;

public sealed class Expression<TDelegate> : LambdaExpression
{
	internal Expression(Expression body, ReadOnlyCollection<ParameterExpression> parameters)
		: base(typeof(TDelegate), body, parameters)
	{
	}

	public new TDelegate Compile()
	{
		return (TDelegate)(object)base.Compile();
	}
}
public abstract class Expression
{
	internal const BindingFlags PublicInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

	internal const BindingFlags NonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	internal const BindingFlags PublicStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

	internal const BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	internal const BindingFlags AllStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	internal const BindingFlags All = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	private ExpressionType node_type;

	private Type type;

	public ExpressionType NodeType => node_type;

	public Type Type => type;

	protected Expression(ExpressionType node_type, Type type)
	{
		this.node_type = node_type;
		this.type = type;
	}

	public override string ToString()
	{
		return ExpressionPrinter.ToString(this);
	}

	private static MethodInfo GetUnaryOperator(string oper_name, Type declaring, Type param)
	{
		return GetUnaryOperator(oper_name, declaring, param, null);
	}

	private static MethodInfo GetUnaryOperator(string oper_name, Type declaring, Type param, Type ret)
	{
		MethodInfo[] methods = declaring.GetNotNullableType().GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			if (!(methodInfo.Name != oper_name))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == 1 && !methodInfo.IsGenericMethod && IsAssignableToParameterType(param.GetNotNullableType(), parameters[0]) && (ret == null || methodInfo.ReturnType == ret.GetNotNullableType()))
				{
					return methodInfo;
				}
			}
		}
		return null;
	}

	internal static MethodInfo GetTrueOperator(Type self)
	{
		return GetBooleanOperator("op_True", self);
	}

	internal static MethodInfo GetFalseOperator(Type self)
	{
		return GetBooleanOperator("op_False", self);
	}

	private static MethodInfo GetBooleanOperator(string op, Type self)
	{
		return GetUnaryOperator(op, self, self, typeof(bool));
	}

	private static bool IsAssignableToParameterType(Type type, ParameterInfo param)
	{
		Type type2 = param.ParameterType;
		if (type2.IsByRef)
		{
			type2 = type2.GetElementType();
		}
		return type.GetNotNullableType().IsAssignableTo(type2);
	}

	private static MethodInfo CheckUnaryMethod(MethodInfo method, Type param)
	{
		if (method.ReturnType == typeof(void))
		{
			throw new ArgumentException("Specified method must return a value", "method");
		}
		if (!method.IsStatic)
		{
			throw new ArgumentException("Method must be static", "method");
		}
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length != 1)
		{
			throw new ArgumentException("Must have only one parameters", "method");
		}
		if (!IsAssignableToParameterType(param.GetNotNullableType(), parameters[0]))
		{
			throw new InvalidOperationException("left-side argument type does not match expression type");
		}
		return method;
	}

	private static MethodInfo UnaryCoreCheck(string oper_name, Expression expression, MethodInfo method, Func<Type, bool> validator)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (method != null)
		{
			return CheckUnaryMethod(method, expression.Type);
		}
		Type notNullableType = expression.Type.GetNotNullableType();
		if (validator(notNullableType))
		{
			return null;
		}
		if (oper_name != null)
		{
			method = GetUnaryOperator(oper_name, notNullableType, expression.Type);
			if (method != null)
			{
				return method;
			}
		}
		throw new InvalidOperationException(string.Format("Operation {0} not defined for {1}", (oper_name == null) ? "is" : oper_name.Substring(3), expression.Type));
	}

	private static MethodInfo GetBinaryOperator(string oper_name, Type on_type, Expression left, Expression right)
	{
		MethodInfo[] methods = on_type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			if (!(methodInfo.Name != oper_name))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == 2 && !methodInfo.IsGenericMethod && IsAssignableToParameterType(left.Type, parameters[0]) && IsAssignableToParameterType(right.Type, parameters[1]))
				{
					return methodInfo;
				}
			}
		}
		return null;
	}

	private static MethodInfo BinaryCoreCheck(string oper_name, Expression left, Expression right, MethodInfo method)
	{
		if (left == null)
		{
			throw new ArgumentNullException("left");
		}
		if (right == null)
		{
			throw new ArgumentNullException("right");
		}
		if (method != null)
		{
			if (method.ReturnType == typeof(void))
			{
				throw new ArgumentException("Specified method must return a value", "method");
			}
			if (!method.IsStatic)
			{
				throw new ArgumentException("Method must be static", "method");
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != 2)
			{
				throw new ArgumentException("Must have only two parameters", "method");
			}
			if (!IsAssignableToParameterType(left.Type, parameters[0]))
			{
				throw new InvalidOperationException("left-side argument type does not match left expression type");
			}
			if (!IsAssignableToParameterType(right.Type, parameters[1]))
			{
				throw new InvalidOperationException("right-side argument type does not match right expression type");
			}
			return method;
		}
		Type type = left.Type;
		Type type2 = right.Type;
		Type notNullableType = type.GetNotNullableType();
		Type notNullableType2 = type2.GetNotNullableType();
		if ((oper_name == "op_BitwiseOr" || oper_name == "op_BitwiseAnd") && notNullableType == typeof(bool) && notNullableType == notNullableType2 && type == type2)
		{
			return null;
		}
		if (IsNumber(notNullableType))
		{
			if (notNullableType == notNullableType2 && type == type2)
			{
				return null;
			}
			if (oper_name != null)
			{
				method = GetBinaryOperator(oper_name, notNullableType2, left, right);
				if (method != null)
				{
					return method;
				}
			}
		}
		if (oper_name != null)
		{
			method = GetBinaryOperator(oper_name, notNullableType, left, right);
			if (method != null)
			{
				return method;
			}
		}
		if (oper_name == "op_Equality" || oper_name == "op_Inequality")
		{
			if (!type.IsValueType && !type2.IsValueType)
			{
				return null;
			}
			if (type == type2 && notNullableType.IsEnum)
			{
				return null;
			}
			if (type == type2 && notNullableType == typeof(bool))
			{
				return null;
			}
		}
		if ((oper_name == "op_LeftShift" || oper_name == "op_RightShift") && IsInt(notNullableType) && notNullableType2 == typeof(int))
		{
			return null;
		}
		throw new InvalidOperationException(string.Format("Operation {0} not defined for {1} and {2}", (oper_name == null) ? "is" : oper_name.Substring(3), type, type2));
	}

	private static MethodInfo BinaryBitwiseCoreCheck(string oper_name, Expression left, Expression right, MethodInfo method)
	{
		if (left == null)
		{
			throw new ArgumentNullException("left");
		}
		if (right == null)
		{
			throw new ArgumentNullException("right");
		}
		if (method == null && left.Type == right.Type && IsIntOrBool(left.Type))
		{
			return null;
		}
		method = BinaryCoreCheck(oper_name, left, right, method);
		if (method == null && (left.Type == typeof(double) || left.Type == typeof(float)))
		{
			throw new InvalidOperationException("Types not supported");
		}
		return method;
	}

	private static BinaryExpression MakeSimpleBinary(ExpressionType et, Expression left, Expression right, MethodInfo method)
	{
		bool flag;
		Type type;
		if (method == null)
		{
			flag = left.Type.IsNullable();
			type = left.Type;
		}
		else
		{
			ParameterInfo[] parameters = method.GetParameters();
			ParameterInfo parameterInfo = parameters[0];
			ParameterInfo parameterInfo2 = parameters[1];
			if (IsAssignableToOperatorParameter(left, parameterInfo) && IsAssignableToOperatorParameter(right, parameterInfo2))
			{
				flag = false;
				type = method.ReturnType;
			}
			else
			{
				if (!left.Type.IsNullable() || !right.Type.IsNullable() || left.Type.GetNotNullableType() != parameterInfo.ParameterType || right.Type.GetNotNullableType() != parameterInfo2.ParameterType || method.ReturnType.IsNullable())
				{
					throw new InvalidOperationException();
				}
				flag = true;
				type = method.ReturnType.MakeNullableType();
			}
		}
		return new BinaryExpression(et, type, left, right, flag, flag, method, null);
	}

	private static bool IsAssignableToOperatorParameter(Expression expression, ParameterInfo parameter)
	{
		if (expression.Type == parameter.ParameterType)
		{
			return true;
		}
		if (!expression.Type.IsNullable() && !parameter.ParameterType.IsNullable() && IsAssignableToParameterType(expression.Type, parameter))
		{
			return true;
		}
		return false;
	}

	private static UnaryExpression MakeSimpleUnary(ExpressionType et, Expression expression, MethodInfo method)
	{
		Type self;
		bool is_lifted;
		if (method == null)
		{
			self = expression.Type;
			is_lifted = self.IsNullable();
		}
		else
		{
			ParameterInfo parameterInfo = method.GetParameters()[0];
			if (IsAssignableToOperatorParameter(expression, parameterInfo))
			{
				is_lifted = false;
				self = method.ReturnType;
			}
			else
			{
				if (!expression.Type.IsNullable() || expression.Type.GetNotNullableType() != parameterInfo.ParameterType || method.ReturnType.IsNullable())
				{
					throw new InvalidOperationException();
				}
				is_lifted = true;
				self = method.ReturnType.MakeNullableType();
			}
		}
		return new UnaryExpression(et, expression, self, method, is_lifted);
	}

	private static BinaryExpression MakeBoolBinary(ExpressionType et, Expression left, Expression right, bool liftToNull, MethodInfo method)
	{
		bool is_lifted;
		Type type;
		if (method == null)
		{
			if (!left.Type.IsNullable() && !right.Type.IsNullable())
			{
				is_lifted = false;
				liftToNull = false;
				type = typeof(bool);
			}
			else
			{
				if (!left.Type.IsNullable() || !right.Type.IsNullable())
				{
					throw new InvalidOperationException();
				}
				is_lifted = true;
				type = ((!liftToNull) ? typeof(bool) : typeof(bool?));
			}
		}
		else
		{
			ParameterInfo[] parameters = method.GetParameters();
			ParameterInfo parameterInfo = parameters[0];
			ParameterInfo parameterInfo2 = parameters[1];
			if (IsAssignableToOperatorParameter(left, parameterInfo) && IsAssignableToOperatorParameter(right, parameterInfo2))
			{
				is_lifted = false;
				liftToNull = false;
				type = method.ReturnType;
			}
			else
			{
				if (!left.Type.IsNullable() || !right.Type.IsNullable() || left.Type.GetNotNullableType() != parameterInfo.ParameterType || right.Type.GetNotNullableType() != parameterInfo2.ParameterType)
				{
					throw new InvalidOperationException();
				}
				is_lifted = true;
				if (method.ReturnType == typeof(bool))
				{
					type = ((!liftToNull) ? typeof(bool) : typeof(bool?));
				}
				else
				{
					if (method.ReturnType.IsNullable())
					{
						throw new InvalidOperationException();
					}
					type = method.ReturnType.MakeNullableType();
				}
			}
		}
		return new BinaryExpression(et, type, left, right, liftToNull, is_lifted, method, null);
	}

	public static BinaryExpression Add(Expression left, Expression right)
	{
		return Add(left, right, null);
	}

	public static BinaryExpression Add(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Addition", left, right, method);
		return MakeSimpleBinary(ExpressionType.Add, left, right, method);
	}

	public static BinaryExpression AddChecked(Expression left, Expression right)
	{
		return AddChecked(left, right, null);
	}

	public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Addition", left, right, method);
		if (method == null && (left.Type == typeof(byte) || left.Type == typeof(sbyte)))
		{
			throw new InvalidOperationException($"AddChecked not defined for {left.Type} and {right.Type}");
		}
		return MakeSimpleBinary(ExpressionType.AddChecked, left, right, method);
	}

	public static BinaryExpression Subtract(Expression left, Expression right)
	{
		return Subtract(left, right, null);
	}

	public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Subtraction", left, right, method);
		return MakeSimpleBinary(ExpressionType.Subtract, left, right, method);
	}

	public static BinaryExpression SubtractChecked(Expression left, Expression right)
	{
		return SubtractChecked(left, right, null);
	}

	public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Subtraction", left, right, method);
		if (method == null && (left.Type == typeof(byte) || left.Type == typeof(sbyte)))
		{
			throw new InvalidOperationException($"SubtractChecked not defined for {left.Type} and {right.Type}");
		}
		return MakeSimpleBinary(ExpressionType.SubtractChecked, left, right, method);
	}

	public static BinaryExpression Modulo(Expression left, Expression right)
	{
		return Modulo(left, right, null);
	}

	public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Modulus", left, right, method);
		return MakeSimpleBinary(ExpressionType.Modulo, left, right, method);
	}

	public static BinaryExpression Multiply(Expression left, Expression right)
	{
		return Multiply(left, right, null);
	}

	public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Multiply", left, right, method);
		return MakeSimpleBinary(ExpressionType.Multiply, left, right, method);
	}

	public static BinaryExpression MultiplyChecked(Expression left, Expression right)
	{
		return MultiplyChecked(left, right, null);
	}

	public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Multiply", left, right, method);
		return MakeSimpleBinary(ExpressionType.MultiplyChecked, left, right, method);
	}

	public static BinaryExpression Divide(Expression left, Expression right)
	{
		return Divide(left, right, null);
	}

	public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Division", left, right, method);
		return MakeSimpleBinary(ExpressionType.Divide, left, right, method);
	}

	public static BinaryExpression Power(Expression left, Expression right)
	{
		return Power(left, right, null);
	}

	public static BinaryExpression Power(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck(null, left, right, method);
		if (left.Type.GetNotNullableType() != typeof(double))
		{
			throw new InvalidOperationException("Power only supports double arguments");
		}
		return MakeSimpleBinary(ExpressionType.Power, left, right, method);
	}

	public static BinaryExpression And(Expression left, Expression right)
	{
		return And(left, right, null);
	}

	public static BinaryExpression And(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryBitwiseCoreCheck("op_BitwiseAnd", left, right, method);
		return MakeSimpleBinary(ExpressionType.And, left, right, method);
	}

	public static BinaryExpression Or(Expression left, Expression right)
	{
		return Or(left, right, null);
	}

	public static BinaryExpression Or(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryBitwiseCoreCheck("op_BitwiseOr", left, right, method);
		return MakeSimpleBinary(ExpressionType.Or, left, right, method);
	}

	public static BinaryExpression ExclusiveOr(Expression left, Expression right)
	{
		return ExclusiveOr(left, right, null);
	}

	public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryBitwiseCoreCheck("op_ExclusiveOr", left, right, method);
		return MakeSimpleBinary(ExpressionType.ExclusiveOr, left, right, method);
	}

	public static BinaryExpression LeftShift(Expression left, Expression right)
	{
		return LeftShift(left, right, null);
	}

	public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryBitwiseCoreCheck("op_LeftShift", left, right, method);
		return MakeSimpleBinary(ExpressionType.LeftShift, left, right, method);
	}

	public static BinaryExpression RightShift(Expression left, Expression right)
	{
		return RightShift(left, right, null);
	}

	public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck("op_RightShift", left, right, method);
		return MakeSimpleBinary(ExpressionType.RightShift, left, right, method);
	}

	public static BinaryExpression AndAlso(Expression left, Expression right)
	{
		return AndAlso(left, right, null);
	}

	public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method)
	{
		method = ConditionalBinaryCheck("op_BitwiseAnd", left, right, method);
		return MakeBoolBinary(ExpressionType.AndAlso, left, right, liftToNull: true, method);
	}

	private static MethodInfo ConditionalBinaryCheck(string oper, Expression left, Expression right, MethodInfo method)
	{
		method = BinaryCoreCheck(oper, left, right, method);
		if (method == null)
		{
			if (left.Type.GetNotNullableType() != typeof(bool))
			{
				throw new InvalidOperationException("Only booleans are allowed");
			}
		}
		else
		{
			Type notNullableType = left.Type.GetNotNullableType();
			if (left.Type != right.Type || method.ReturnType != notNullableType)
			{
				throw new ArgumentException("left, right and return type must match");
			}
			MethodInfo trueOperator = GetTrueOperator(notNullableType);
			MethodInfo falseOperator = GetFalseOperator(notNullableType);
			if (trueOperator == null || falseOperator == null)
			{
				throw new ArgumentException("Operators true and false are required but not defined");
			}
		}
		return method;
	}

	public static BinaryExpression OrElse(Expression left, Expression right)
	{
		return OrElse(left, right, null);
	}

	public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method)
	{
		method = ConditionalBinaryCheck("op_BitwiseOr", left, right, method);
		return MakeBoolBinary(ExpressionType.OrElse, left, right, liftToNull: true, method);
	}

	public static BinaryExpression Equal(Expression left, Expression right)
	{
		return Equal(left, right, liftToNull: false, null);
	}

	public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Equality", left, right, method);
		return MakeBoolBinary(ExpressionType.Equal, left, right, liftToNull, method);
	}

	public static BinaryExpression NotEqual(Expression left, Expression right)
	{
		return NotEqual(left, right, liftToNull: false, null);
	}

	public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
	{
		method = BinaryCoreCheck("op_Inequality", left, right, method);
		return MakeBoolBinary(ExpressionType.NotEqual, left, right, liftToNull, method);
	}

	public static BinaryExpression GreaterThan(Expression left, Expression right)
	{
		return GreaterThan(left, right, liftToNull: false, null);
	}

	public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
	{
		method = BinaryCoreCheck("op_GreaterThan", left, right, method);
		return MakeBoolBinary(ExpressionType.GreaterThan, left, right, liftToNull, method);
	}

	public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right)
	{
		return GreaterThanOrEqual(left, right, liftToNull: false, null);
	}

	public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
	{
		method = BinaryCoreCheck("op_GreaterThanOrEqual", left, right, method);
		return MakeBoolBinary(ExpressionType.GreaterThanOrEqual, left, right, liftToNull, method);
	}

	public static BinaryExpression LessThan(Expression left, Expression right)
	{
		return LessThan(left, right, liftToNull: false, null);
	}

	public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
	{
		method = BinaryCoreCheck("op_LessThan", left, right, method);
		return MakeBoolBinary(ExpressionType.LessThan, left, right, liftToNull, method);
	}

	public static BinaryExpression LessThanOrEqual(Expression left, Expression right)
	{
		return LessThanOrEqual(left, right, liftToNull: false, null);
	}

	public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
	{
		method = BinaryCoreCheck("op_LessThanOrEqual", left, right, method);
		return MakeBoolBinary(ExpressionType.LessThanOrEqual, left, right, liftToNull, method);
	}

	private static void CheckArray(Expression array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (!array.Type.IsArray)
		{
			throw new ArgumentException("The array argument must be of type array");
		}
	}

	public static BinaryExpression ArrayIndex(Expression array, Expression index)
	{
		CheckArray(array);
		if (index == null)
		{
			throw new ArgumentNullException("index");
		}
		if (array.Type.GetArrayRank() != 1)
		{
			throw new ArgumentException("The array argument must be a single dimensional array");
		}
		if (index.Type != typeof(int))
		{
			throw new ArgumentException("The index must be of type int");
		}
		return new BinaryExpression(ExpressionType.ArrayIndex, array.Type.GetElementType(), array, index);
	}

	public static BinaryExpression Coalesce(Expression left, Expression right)
	{
		return Coalesce(left, right, null);
	}

	private static BinaryExpression MakeCoalesce(Expression left, Expression right)
	{
		Type type = null;
		if (left.Type.IsNullable())
		{
			Type notNullableType = left.Type.GetNotNullableType();
			if (!right.Type.IsNullable() && right.Type.IsAssignableTo(notNullableType))
			{
				type = notNullableType;
			}
		}
		if (type == null && right.Type.IsAssignableTo(left.Type))
		{
			type = left.Type;
		}
		if (type == null && left.Type.IsNullable() && left.Type.GetNotNullableType().IsAssignableTo(right.Type))
		{
			type = right.Type;
		}
		if (type == null)
		{
			throw new ArgumentException("Incompatible argument types");
		}
		return new BinaryExpression(ExpressionType.Coalesce, type, left, right, lift_to_null: false, is_lifted: false, null, null);
	}

	private static BinaryExpression MakeConvertedCoalesce(Expression left, Expression right, LambdaExpression conversion)
	{
		MethodInfo invokeMethod = conversion.Type.GetInvokeMethod();
		CheckNotVoid(invokeMethod.ReturnType);
		if (invokeMethod.ReturnType != right.Type)
		{
			throw new InvalidOperationException("Conversion return type doesn't march right type");
		}
		ParameterInfo[] parameters = invokeMethod.GetParameters();
		if (parameters.Length != 1)
		{
			throw new ArgumentException("Conversion has wrong number of parameters");
		}
		if (!IsAssignableToParameterType(left.Type, parameters[0]))
		{
			throw new InvalidOperationException("Conversion argument doesn't marcht left type");
		}
		return new BinaryExpression(ExpressionType.Coalesce, right.Type, left, right, lift_to_null: false, is_lifted: false, null, conversion);
	}

	public static BinaryExpression Coalesce(Expression left, Expression right, LambdaExpression conversion)
	{
		if (left == null)
		{
			throw new ArgumentNullException("left");
		}
		if (right == null)
		{
			throw new ArgumentNullException("right");
		}
		if (left.Type.IsValueType && !left.Type.IsNullable())
		{
			throw new InvalidOperationException("Left expression can never be null");
		}
		if (conversion != null)
		{
			return MakeConvertedCoalesce(left, right, conversion);
		}
		return MakeCoalesce(left, right);
	}

	public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right)
	{
		return MakeBinary(binaryType, left, right, liftToNull: false, null);
	}

	public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method)
	{
		return MakeBinary(binaryType, left, right, liftToNull, method, null);
	}

	public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method, LambdaExpression conversion)
	{
		return binaryType switch
		{
			ExpressionType.Add => Add(left, right, method), 
			ExpressionType.AddChecked => AddChecked(left, right, method), 
			ExpressionType.AndAlso => AndAlso(left, right), 
			ExpressionType.Coalesce => Coalesce(left, right, conversion), 
			ExpressionType.Divide => Divide(left, right, method), 
			ExpressionType.Equal => Equal(left, right, liftToNull, method), 
			ExpressionType.ExclusiveOr => ExclusiveOr(left, right, method), 
			ExpressionType.GreaterThan => GreaterThan(left, right, liftToNull, method), 
			ExpressionType.GreaterThanOrEqual => GreaterThanOrEqual(left, right, liftToNull, method), 
			ExpressionType.LeftShift => LeftShift(left, right, method), 
			ExpressionType.LessThan => LessThan(left, right, liftToNull, method), 
			ExpressionType.LessThanOrEqual => LessThanOrEqual(left, right, liftToNull, method), 
			ExpressionType.Modulo => Modulo(left, right, method), 
			ExpressionType.Multiply => Multiply(left, right, method), 
			ExpressionType.MultiplyChecked => MultiplyChecked(left, right, method), 
			ExpressionType.NotEqual => NotEqual(left, right, liftToNull, method), 
			ExpressionType.OrElse => OrElse(left, right), 
			ExpressionType.Power => Power(left, right, method), 
			ExpressionType.RightShift => RightShift(left, right, method), 
			ExpressionType.Subtract => Subtract(left, right, method), 
			ExpressionType.SubtractChecked => SubtractChecked(left, right, method), 
			ExpressionType.And => And(left, right, method), 
			ExpressionType.Or => Or(left, right, method), 
			_ => throw new ArgumentException("MakeBinary expect a binary node type"), 
		};
	}

	public static MethodCallExpression ArrayIndex(Expression array, params Expression[] indexes)
	{
		return ArrayIndex(array, (IEnumerable<Expression>)indexes);
	}

	public static MethodCallExpression ArrayIndex(Expression array, IEnumerable<Expression> indexes)
	{
		CheckArray(array);
		if (indexes == null)
		{
			throw new ArgumentNullException("indexes");
		}
		ReadOnlyCollection<Expression> readOnlyCollection = indexes.ToReadOnlyCollection();
		if (array.Type.GetArrayRank() != readOnlyCollection.Count)
		{
			throw new ArgumentException("The number of arguments doesn't match the rank of the array");
		}
		foreach (Expression item in readOnlyCollection)
		{
			if (item.Type != typeof(int))
			{
				throw new ArgumentException("The index must be of type int");
			}
		}
		return Call(array, array.Type.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy), readOnlyCollection);
	}

	public static UnaryExpression ArrayLength(Expression array)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (!array.Type.IsArray)
		{
			throw new ArgumentException("The type of the expression must me Array");
		}
		if (array.Type.GetArrayRank() != 1)
		{
			throw new ArgumentException("The array must be a single dimensional array");
		}
		return new UnaryExpression(ExpressionType.ArrayLength, array, typeof(int));
	}

	public static MemberAssignment Bind(MemberInfo member, Expression expression)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		Type type = null;
		if (member is PropertyInfo propertyInfo && propertyInfo.GetSetMethod(nonPublic: true) != null)
		{
			type = propertyInfo.PropertyType;
		}
		if (member is FieldInfo fieldInfo)
		{
			type = fieldInfo.FieldType;
		}
		if (type == null)
		{
			throw new ArgumentException("member");
		}
		if (!expression.Type.IsAssignableTo(type))
		{
			throw new ArgumentException("member");
		}
		return new MemberAssignment(member, expression);
	}

	public static MemberAssignment Bind(MethodInfo propertyAccessor, Expression expression)
	{
		if (propertyAccessor == null)
		{
			throw new ArgumentNullException("propertyAccessor");
		}
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		CheckNonGenericMethod(propertyAccessor);
		PropertyInfo associatedProperty = GetAssociatedProperty(propertyAccessor);
		if (associatedProperty == null)
		{
			throw new ArgumentException("propertyAccessor");
		}
		MethodInfo setMethod = associatedProperty.GetSetMethod(nonPublic: true);
		if (setMethod == null)
		{
			throw new ArgumentException("setter");
		}
		if (!expression.Type.IsAssignableTo(associatedProperty.PropertyType))
		{
			throw new ArgumentException("member");
		}
		return new MemberAssignment(associatedProperty, expression);
	}

	public static MethodCallExpression Call(Expression instance, MethodInfo method)
	{
		return Call(instance, method, (IEnumerable<Expression>)null);
	}

	public static MethodCallExpression Call(MethodInfo method, params Expression[] arguments)
	{
		return Call((Expression)null, method, (IEnumerable<Expression>)arguments);
	}

	public static MethodCallExpression Call(Expression instance, MethodInfo method, params Expression[] arguments)
	{
		return Call(instance, method, (IEnumerable<Expression>)arguments);
	}

	public static MethodCallExpression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
	{
		if (method == null)
		{
			throw new ArgumentNullException("method");
		}
		if (instance == null && !method.IsStatic)
		{
			throw new ArgumentNullException("instance");
		}
		if (method.IsStatic && instance != null)
		{
			throw new ArgumentException("instance");
		}
		if (!method.IsStatic && !instance.Type.IsAssignableTo(method.DeclaringType))
		{
			throw new ArgumentException("Type is not assignable to the declaring type of the method");
		}
		ReadOnlyCollection<Expression> arguments2 = CheckMethodArguments(method, arguments);
		return new MethodCallExpression(instance, method, arguments2);
	}

	private static Type[] CollectTypes(IEnumerable<Expression> expressions)
	{
		return expressions.Select((Expression arg) => arg.Type).ToArray();
	}

	private static MethodInfo TryMakeGeneric(MethodInfo method, Type[] args)
	{
		if (method == null)
		{
			return null;
		}
		if (!method.IsGenericMethod && (args == null || args.Length == 0))
		{
			return method;
		}
		if (args.Length == method.GetGenericArguments().Length)
		{
			return method.MakeGenericMethod(args);
		}
		return null;
	}

	public static MethodCallExpression Call(Expression instance, string methodName, Type[] typeArguments, params Expression[] arguments)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		if (methodName == null)
		{
			throw new ArgumentNullException("methodName");
		}
		MethodInfo method = TryGetMethod(instance.Type, methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, CollectTypes(arguments), typeArguments);
		ReadOnlyCollection<Expression> arguments2 = CheckMethodArguments(method, arguments);
		return new MethodCallExpression(instance, method, arguments2);
	}

	private static bool MethodMatch(MethodInfo method, string name, Type[] parameterTypes, Type[] argumentTypes)
	{
		if (method.Name != name)
		{
			return false;
		}
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length != parameterTypes.Length)
		{
			return false;
		}
		if (method.IsGenericMethod && method.IsGenericMethodDefinition)
		{
			MethodInfo methodInfo = TryMakeGeneric(method, argumentTypes);
			if (methodInfo == null)
			{
				return false;
			}
			return MethodMatch(methodInfo, name, parameterTypes, argumentTypes);
		}
		if (!method.IsGenericMethod && argumentTypes != null && argumentTypes.Length > 0)
		{
			return false;
		}
		for (int i = 0; i < parameters.Length; i++)
		{
			Type type = parameterTypes[i];
			ParameterInfo parameterInfo = parameters[i];
			if (!IsAssignableToParameterType(type, parameterInfo) && !IsExpressionOfParameter(type, parameterInfo.ParameterType))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsExpressionOfParameter(Type type, Type ptype)
	{
		return ptype.IsGenericInstanceOf(typeof(Expression<>)) && ptype.GetFirstGenericArgument() == type;
	}

	private static MethodInfo TryGetMethod(Type type, string methodName, BindingFlags flags, Type[] parameterTypes, Type[] argumentTypes)
	{
		IEnumerable<MethodInfo> source = from meth in type.GetMethods(flags)
			where MethodMatch(meth, methodName, parameterTypes, argumentTypes)
			select meth;
		if (source.Count() > 1)
		{
			throw new InvalidOperationException("Too many method candidates");
		}
		MethodInfo methodInfo = TryMakeGeneric(source.FirstOrDefault(), argumentTypes);
		if (methodInfo != null)
		{
			return methodInfo;
		}
		throw new InvalidOperationException("No such method");
	}

	public static MethodCallExpression Call(Type type, string methodName, Type[] typeArguments, params Expression[] arguments)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (methodName == null)
		{
			throw new ArgumentNullException("methodName");
		}
		MethodInfo method = TryGetMethod(type, methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, CollectTypes(arguments), typeArguments);
		ReadOnlyCollection<Expression> arguments2 = CheckMethodArguments(method, arguments);
		return new MethodCallExpression(method, arguments2);
	}

	public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
	{
		if (test == null)
		{
			throw new ArgumentNullException("test");
		}
		if (ifTrue == null)
		{
			throw new ArgumentNullException("ifTrue");
		}
		if (ifFalse == null)
		{
			throw new ArgumentNullException("ifFalse");
		}
		if (test.Type != typeof(bool))
		{
			throw new ArgumentException("Test expression should be of type bool");
		}
		if (ifTrue.Type != ifFalse.Type)
		{
			throw new ArgumentException("The ifTrue and ifFalse type do not match");
		}
		return new ConditionalExpression(test, ifTrue, ifFalse);
	}

	public static ConstantExpression Constant(object value)
	{
		if (value == null)
		{
			return new ConstantExpression(null, typeof(object));
		}
		return Constant(value, value.GetType());
	}

	public static ConstantExpression Constant(object value, Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (value == null)
		{
			if (type.IsValueType && !type.IsNullable())
			{
				throw new ArgumentException();
			}
		}
		else if ((!type.IsValueType || !type.IsNullable()) && !value.GetType().IsAssignableTo(type))
		{
			throw new ArgumentException();
		}
		return new ConstantExpression(value, type);
	}

	private static bool IsConvertiblePrimitive(Type type)
	{
		Type notNullableType = type.GetNotNullableType();
		if (notNullableType == typeof(bool))
		{
			return false;
		}
		if (notNullableType.IsEnum)
		{
			return true;
		}
		return notNullableType.IsPrimitive;
	}

	internal static bool IsPrimitiveConversion(Type type, Type target)
	{
		if (type == target)
		{
			return true;
		}
		if (type.IsNullable() && target == type.GetNotNullableType())
		{
			return true;
		}
		if (target.IsNullable() && type == target.GetNotNullableType())
		{
			return true;
		}
		if (IsConvertiblePrimitive(type) && IsConvertiblePrimitive(target))
		{
			return true;
		}
		return false;
	}

	internal static bool IsReferenceConversion(Type type, Type target)
	{
		if (type == target)
		{
			return true;
		}
		if (type == typeof(object) || target == typeof(object))
		{
			return true;
		}
		if (type.IsInterface || target.IsInterface)
		{
			return true;
		}
		if (type.IsValueType || target.IsValueType)
		{
			return false;
		}
		if (type.IsAssignableTo(target) || target.IsAssignableTo(type))
		{
			return true;
		}
		return false;
	}

	public static UnaryExpression Convert(Expression expression, Type type)
	{
		return Convert(expression, type, null);
	}

	private static MethodInfo GetUserConversionMethod(Type type, Type target)
	{
		MethodInfo unaryOperator = GetUnaryOperator("op_Explicit", type, type, target);
		if (unaryOperator == null)
		{
			unaryOperator = GetUnaryOperator("op_Implicit", type, type, target);
		}
		if (unaryOperator == null)
		{
			unaryOperator = GetUnaryOperator("op_Explicit", target, type, target);
		}
		if (unaryOperator == null)
		{
			unaryOperator = GetUnaryOperator("op_Implicit", target, type, target);
		}
		if (unaryOperator == null)
		{
			throw new InvalidOperationException();
		}
		return unaryOperator;
	}

	public static UnaryExpression Convert(Expression expression, Type type, MethodInfo method)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		Type param = expression.Type;
		if (method != null)
		{
			CheckUnaryMethod(method, param);
		}
		else if (!IsPrimitiveConversion(param, type) && !IsReferenceConversion(param, type))
		{
			method = GetUserConversionMethod(param, type);
		}
		return new UnaryExpression(ExpressionType.Convert, expression, type, method, IsConvertNodeLifted(method, expression, type));
	}

	private static bool IsConvertNodeLifted(MethodInfo method, Expression operand, Type target)
	{
		if (method == null)
		{
			return operand.Type.IsNullable() || target.IsNullable();
		}
		if (operand.Type.IsNullable() && !ParameterMatch(method, operand.Type))
		{
			return true;
		}
		if (target.IsNullable() && !ReturnTypeMatch(method, target))
		{
			return true;
		}
		return false;
	}

	private static bool ParameterMatch(MethodInfo method, Type type)
	{
		return method.GetParameters()[0].ParameterType == type;
	}

	private static bool ReturnTypeMatch(MethodInfo method, Type type)
	{
		return method.ReturnType == type;
	}

	public static UnaryExpression ConvertChecked(Expression expression, Type type)
	{
		return ConvertChecked(expression, type, null);
	}

	public static UnaryExpression ConvertChecked(Expression expression, Type type, MethodInfo method)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		Type param = expression.Type;
		if (method != null)
		{
			CheckUnaryMethod(method, param);
		}
		else
		{
			if (IsReferenceConversion(param, type))
			{
				return Convert(expression, type, method);
			}
			if (!IsPrimitiveConversion(param, type))
			{
				method = GetUserConversionMethod(param, type);
			}
		}
		return new UnaryExpression(ExpressionType.ConvertChecked, expression, type, method, IsConvertNodeLifted(method, expression, type));
	}

	public static ElementInit ElementInit(MethodInfo addMethod, params Expression[] arguments)
	{
		return ElementInit(addMethod, (IEnumerable<Expression>)arguments);
	}

	public static ElementInit ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
	{
		if (addMethod == null)
		{
			throw new ArgumentNullException("addMethod");
		}
		if (arguments == null)
		{
			throw new ArgumentNullException("arguments");
		}
		if (addMethod.Name.ToLower(CultureInfo.InvariantCulture) != "add")
		{
			throw new ArgumentException("addMethod");
		}
		if (addMethod.IsStatic)
		{
			throw new ArgumentException("addMethod must be an instance method", "addMethod");
		}
		ReadOnlyCollection<Expression> arguments2 = CheckMethodArguments(addMethod, arguments);
		return new ElementInit(addMethod, arguments2);
	}

	public static MemberExpression Field(Expression expression, FieldInfo field)
	{
		if (field == null)
		{
			throw new ArgumentNullException("field");
		}
		if (!field.IsStatic)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (!expression.Type.IsAssignableTo(field.DeclaringType))
			{
				throw new ArgumentException("field");
			}
		}
		else if (expression != null)
		{
			throw new ArgumentException("expression");
		}
		return new MemberExpression(expression, field, field.FieldType);
	}

	public static MemberExpression Field(Expression expression, string fieldName)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		FieldInfo field = expression.Type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (field == null)
		{
			throw new ArgumentException($"No field named {fieldName} on {expression.Type}");
		}
		return new MemberExpression(expression, field, field.FieldType);
	}

	public static Type GetActionType(params Type[] typeArgs)
	{
		if (typeArgs == null)
		{
			throw new ArgumentNullException("typeArgs");
		}
		if (typeArgs.Length > 4)
		{
			throw new ArgumentException("No Action type of this arity");
		}
		if (typeArgs.Length == 0)
		{
			return typeof(Action);
		}
		Type type = null;
		switch (typeArgs.Length)
		{
		case 1:
			type = typeof(Action<>);
			break;
		case 2:
			type = typeof(Action<, >);
			break;
		case 3:
			type = typeof(Action<, , >);
			break;
		case 4:
			type = typeof(Action<, , , >);
			break;
		}
		return type.MakeGenericType(typeArgs);
	}

	public static Type GetFuncType(params Type[] typeArgs)
	{
		if (typeArgs == null)
		{
			throw new ArgumentNullException("typeArgs");
		}
		if (typeArgs.Length < 1 || typeArgs.Length > 5)
		{
			throw new ArgumentException("No Func type of this arity");
		}
		Type type = null;
		switch (typeArgs.Length)
		{
		case 1:
			type = typeof(Func<>);
			break;
		case 2:
			type = typeof(Func<, >);
			break;
		case 3:
			type = typeof(Func<, , >);
			break;
		case 4:
			type = typeof(Func<, , , >);
			break;
		case 5:
			type = typeof(Func<, , , , >);
			break;
		}
		return type.MakeGenericType(typeArgs);
	}

	public static InvocationExpression Invoke(Expression expression, params Expression[] arguments)
	{
		return Invoke(expression, (IEnumerable<Expression>)arguments);
	}

	private static Type GetInvokableType(Type t)
	{
		if (t.IsAssignableTo(typeof(Delegate)))
		{
			return t;
		}
		return GetGenericType(t, typeof(Expression<>));
	}

	private static Type GetGenericType(Type t, Type def)
	{
		if (t == null)
		{
			return null;
		}
		if (t.IsGenericType && t.GetGenericTypeDefinition() == def)
		{
			return t;
		}
		return GetGenericType(t.BaseType, def);
	}

	public static InvocationExpression Invoke(Expression expression, IEnumerable<Expression> arguments)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		Type invokableType = GetInvokableType(expression.Type);
		if (invokableType == null)
		{
			throw new ArgumentException("The type of the expression is not invokable");
		}
		ReadOnlyCollection<Expression> readOnlyCollection = arguments.ToReadOnlyCollection();
		CheckForNull(readOnlyCollection, "arguments");
		MethodInfo invokeMethod = invokableType.GetInvokeMethod();
		if (invokeMethod == null)
		{
			throw new ArgumentException("expression");
		}
		if (invokeMethod.GetParameters().Length != readOnlyCollection.Count)
		{
			throw new InvalidOperationException("Arguments count doesn't match parameters length");
		}
		readOnlyCollection = CheckMethodArguments(invokeMethod, readOnlyCollection);
		return new InvocationExpression(expression, invokeMethod.ReturnType, readOnlyCollection);
	}

	private static bool CanAssign(Type target, Type source)
	{
		if (target.IsValueType ^ source.IsValueType)
		{
			return false;
		}
		return source.IsAssignableTo(target);
	}

	private static Expression CheckLambda(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
	{
		if (!delegateType.IsSubclassOf(typeof(Delegate)))
		{
			throw new ArgumentException("delegateType");
		}
		MethodInfo invokeMethod = delegateType.GetInvokeMethod();
		if (invokeMethod == null)
		{
			throw new ArgumentException("delegate must contain an Invoke method", "delegateType");
		}
		ParameterInfo[] parameters2 = invokeMethod.GetParameters();
		if (parameters2.Length != parameters.Count)
		{
			throw new ArgumentException($"Different number of arguments in delegate {delegateType}", "delegateType");
		}
		for (int i = 0; i < parameters2.Length; i++)
		{
			ParameterExpression parameterExpression = parameters[i];
			if (parameterExpression == null)
			{
				throw new ArgumentNullException("parameters");
			}
			if (!CanAssign(parameterExpression.Type, parameters2[i].ParameterType))
			{
				throw new ArgumentException($"Can not assign a {parameters2[i].ParameterType} to a {parameterExpression.Type}");
			}
		}
		if (invokeMethod.ReturnType != typeof(void) && !CanAssign(invokeMethod.ReturnType, body.Type))
		{
			if (invokeMethod.ReturnType.IsExpression())
			{
				return Quote(body);
			}
			throw new ArgumentException($"body type {body.Type} can not be assigned to {invokeMethod.ReturnType}");
		}
		return body;
	}

	public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
	{
		return Lambda<TDelegate>(body, (IEnumerable<ParameterExpression>)parameters);
	}

	public static Expression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
	{
		if (body == null)
		{
			throw new ArgumentNullException("body");
		}
		ReadOnlyCollection<ParameterExpression> parameters2 = parameters.ToReadOnlyCollection();
		body = CheckLambda(typeof(TDelegate), body, parameters2);
		return new Expression<TDelegate>(body, parameters2);
	}

	public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
	{
		if (body == null)
		{
			throw new ArgumentNullException("body");
		}
		if (parameters.Length > 4)
		{
			throw new ArgumentException("Too many parameters");
		}
		return Lambda(GetDelegateType(body.Type, parameters), body, parameters);
	}

	private static Type GetDelegateType(Type return_type, ParameterExpression[] parameters)
	{
		if (parameters == null)
		{
			parameters = new ParameterExpression[0];
		}
		if (return_type == typeof(void))
		{
			return GetActionType(parameters.Select((ParameterExpression p) => p.Type).ToArray());
		}
		Type[] array = new Type[parameters.Length + 1];
		for (int i = 0; i < array.Length - 1; i++)
		{
			array[i] = parameters[i].Type;
		}
		array[array.Length - 1] = return_type;
		return GetFuncType(array);
	}

	public static LambdaExpression Lambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
	{
		return Lambda(delegateType, body, (IEnumerable<ParameterExpression>)parameters);
	}

	private static LambdaExpression CreateExpressionOf(Type type, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
	{
		return (LambdaExpression)typeof(Expression<>).MakeGenericType(type).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, new Type[2]
		{
			typeof(Expression),
			typeof(ReadOnlyCollection<ParameterExpression>)
		}, null).Invoke(new object[2] { body, parameters });
	}

	public static LambdaExpression Lambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
	{
		if (delegateType == null)
		{
			throw new ArgumentNullException("delegateType");
		}
		if (body == null)
		{
			throw new ArgumentNullException("body");
		}
		ReadOnlyCollection<ParameterExpression> parameters2 = parameters.ToReadOnlyCollection();
		body = CheckLambda(delegateType, body, parameters2);
		return CreateExpressionOf(delegateType, body, parameters2);
	}

	public static MemberListBinding ListBind(MemberInfo member, params ElementInit[] initializers)
	{
		return ListBind(member, (IEnumerable<ElementInit>)initializers);
	}

	private static void CheckIsAssignableToIEnumerable(Type t)
	{
		if (!t.IsAssignableTo(typeof(IEnumerable)))
		{
			throw new ArgumentException($"Type {t} doesn't implemen IEnumerable");
		}
	}

	public static MemberListBinding ListBind(MemberInfo member, IEnumerable<ElementInit> initializers)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		if (initializers == null)
		{
			throw new ArgumentNullException("initializers");
		}
		ReadOnlyCollection<ElementInit> readOnlyCollection = initializers.ToReadOnlyCollection();
		CheckForNull(readOnlyCollection, "initializers");
		member.OnFieldOrProperty(delegate(FieldInfo field)
		{
			CheckIsAssignableToIEnumerable(field.FieldType);
		}, delegate(PropertyInfo prop)
		{
			CheckIsAssignableToIEnumerable(prop.PropertyType);
		});
		return new MemberListBinding(member, readOnlyCollection);
	}

	public static MemberListBinding ListBind(MethodInfo propertyAccessor, params ElementInit[] initializers)
	{
		return ListBind(propertyAccessor, (IEnumerable<ElementInit>)initializers);
	}

	private static void CheckForNull<T>(ReadOnlyCollection<T> collection, string name) where T : class
	{
		foreach (T item in collection)
		{
			if (item == null)
			{
				throw new ArgumentNullException(name);
			}
		}
	}

	public static MemberListBinding ListBind(MethodInfo propertyAccessor, IEnumerable<ElementInit> initializers)
	{
		if (propertyAccessor == null)
		{
			throw new ArgumentNullException("propertyAccessor");
		}
		if (initializers == null)
		{
			throw new ArgumentNullException("initializers");
		}
		ReadOnlyCollection<ElementInit> readOnlyCollection = initializers.ToReadOnlyCollection();
		CheckForNull(readOnlyCollection, "initializers");
		PropertyInfo associatedProperty = GetAssociatedProperty(propertyAccessor);
		if (associatedProperty == null)
		{
			throw new ArgumentException("propertyAccessor");
		}
		CheckIsAssignableToIEnumerable(associatedProperty.PropertyType);
		return new MemberListBinding(associatedProperty, readOnlyCollection);
	}

	public static ListInitExpression ListInit(NewExpression newExpression, params ElementInit[] initializers)
	{
		return ListInit(newExpression, (IEnumerable<ElementInit>)initializers);
	}

	public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<ElementInit> initializers)
	{
		ReadOnlyCollection<ElementInit> initializers2 = CheckListInit(newExpression, initializers);
		return new ListInitExpression(newExpression, initializers2);
	}

	public static ListInitExpression ListInit(NewExpression newExpression, params Expression[] initializers)
	{
		return ListInit(newExpression, (IEnumerable<Expression>)initializers);
	}

	public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<Expression> initializers)
	{
		ReadOnlyCollection<Expression> readOnlyCollection = CheckListInit(newExpression, initializers);
		MethodInfo addMethod = GetAddMethod(newExpression.Type, readOnlyCollection[0].Type);
		if (addMethod == null)
		{
			throw new InvalidOperationException("No suitable add method found");
		}
		return new ListInitExpression(newExpression, CreateInitializers(addMethod, readOnlyCollection));
	}

	private static ReadOnlyCollection<ElementInit> CreateInitializers(MethodInfo add_method, ReadOnlyCollection<Expression> initializers)
	{
		return initializers.Select((Expression init) => ElementInit(add_method, init)).ToReadOnlyCollection();
	}

	private static MethodInfo GetAddMethod(Type type, Type arg)
	{
		return type.GetMethod("Add", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[1] { arg }, null);
	}

	public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, params Expression[] initializers)
	{
		return ListInit(newExpression, addMethod, (IEnumerable<Expression>)initializers);
	}

	private static ReadOnlyCollection<T> CheckListInit<T>(NewExpression newExpression, IEnumerable<T> initializers) where T : class
	{
		if (newExpression == null)
		{
			throw new ArgumentNullException("newExpression");
		}
		if (initializers == null)
		{
			throw new ArgumentNullException("initializers");
		}
		if (!newExpression.Type.IsAssignableTo(typeof(IEnumerable)))
		{
			throw new InvalidOperationException("The type of the new expression does not implement IEnumerable");
		}
		ReadOnlyCollection<T> readOnlyCollection = initializers.ToReadOnlyCollection();
		if (readOnlyCollection.Count == 0)
		{
			throw new ArgumentException("Empty initializers");
		}
		CheckForNull(readOnlyCollection, "initializers");
		return readOnlyCollection;
	}

	public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, IEnumerable<Expression> initializers)
	{
		ReadOnlyCollection<Expression> readOnlyCollection = CheckListInit(newExpression, initializers);
		if (addMethod != null)
		{
			if (addMethod.Name.ToLower(CultureInfo.InvariantCulture) != "add")
			{
				throw new ArgumentException("addMethod");
			}
			ParameterInfo[] parameters = addMethod.GetParameters();
			if (parameters.Length != 1)
			{
				throw new ArgumentException("addMethod");
			}
			foreach (Expression item in readOnlyCollection)
			{
				if (!IsAssignableToParameterType(item.Type, parameters[0]))
				{
					throw new InvalidOperationException("Initializer not assignable to the add method parameter type");
				}
			}
		}
		if (addMethod == null)
		{
			addMethod = GetAddMethod(newExpression.Type, readOnlyCollection[0].Type);
		}
		if (addMethod == null)
		{
			throw new InvalidOperationException("No suitable add method found");
		}
		return new ListInitExpression(newExpression, CreateInitializers(addMethod, readOnlyCollection));
	}

	public static MemberExpression MakeMemberAccess(Expression expression, MemberInfo member)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		if (member is FieldInfo field)
		{
			return Field(expression, field);
		}
		if (member is PropertyInfo property)
		{
			return Property(expression, property);
		}
		throw new ArgumentException("Member should either be a field or a property");
	}

	public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type)
	{
		return MakeUnary(unaryType, operand, type, null);
	}

	public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type, MethodInfo method)
	{
		return unaryType switch
		{
			ExpressionType.ArrayLength => ArrayLength(operand), 
			ExpressionType.Convert => Convert(operand, type, method), 
			ExpressionType.ConvertChecked => ConvertChecked(operand, type, method), 
			ExpressionType.Negate => Negate(operand, method), 
			ExpressionType.NegateChecked => NegateChecked(operand, method), 
			ExpressionType.Not => Not(operand, method), 
			ExpressionType.Quote => Quote(operand), 
			ExpressionType.TypeAs => TypeAs(operand, type), 
			ExpressionType.UnaryPlus => UnaryPlus(operand, method), 
			_ => throw new ArgumentException("MakeUnary expect an unary operator"), 
		};
	}

	public static MemberMemberBinding MemberBind(MemberInfo member, params MemberBinding[] bindings)
	{
		return MemberBind(member, (IEnumerable<MemberBinding>)bindings);
	}

	public static MemberMemberBinding MemberBind(MemberInfo member, IEnumerable<MemberBinding> bindings)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		Type type = member.OnFieldOrProperty((FieldInfo field) => field.FieldType, (PropertyInfo prop) => prop.PropertyType);
		return new MemberMemberBinding(member, CheckMemberBindings(type, bindings));
	}

	public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, params MemberBinding[] bindings)
	{
		return MemberBind(propertyAccessor, (IEnumerable<MemberBinding>)bindings);
	}

	public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, IEnumerable<MemberBinding> bindings)
	{
		if (propertyAccessor == null)
		{
			throw new ArgumentNullException("propertyAccessor");
		}
		ReadOnlyCollection<MemberBinding> collection = bindings.ToReadOnlyCollection();
		CheckForNull(collection, "bindings");
		PropertyInfo associatedProperty = GetAssociatedProperty(propertyAccessor);
		if (associatedProperty == null)
		{
			throw new ArgumentException("propertyAccessor");
		}
		return new MemberMemberBinding(associatedProperty, CheckMemberBindings(associatedProperty.PropertyType, bindings));
	}

	private static ReadOnlyCollection<MemberBinding> CheckMemberBindings(Type type, IEnumerable<MemberBinding> bindings)
	{
		if (bindings == null)
		{
			throw new ArgumentNullException("bindings");
		}
		ReadOnlyCollection<MemberBinding> readOnlyCollection = bindings.ToReadOnlyCollection();
		CheckForNull(readOnlyCollection, "bindings");
		foreach (MemberBinding item in readOnlyCollection)
		{
			if (!type.IsAssignableTo(item.Member.DeclaringType))
			{
				throw new ArgumentException("Type not assignable to member type");
			}
		}
		return readOnlyCollection;
	}

	public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
	{
		return MemberInit(newExpression, (IEnumerable<MemberBinding>)bindings);
	}

	public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
	{
		if (newExpression == null)
		{
			throw new ArgumentNullException("newExpression");
		}
		return new MemberInitExpression(newExpression, CheckMemberBindings(newExpression.Type, bindings));
	}

	public static UnaryExpression Negate(Expression expression)
	{
		return Negate(expression, null);
	}

	public static UnaryExpression Negate(Expression expression, MethodInfo method)
	{
		method = UnaryCoreCheck("op_UnaryNegation", expression, method, (Type type) => IsSignedNumber(type));
		return MakeSimpleUnary(ExpressionType.Negate, expression, method);
	}

	public static UnaryExpression NegateChecked(Expression expression)
	{
		return NegateChecked(expression, null);
	}

	public static UnaryExpression NegateChecked(Expression expression, MethodInfo method)
	{
		method = UnaryCoreCheck("op_UnaryNegation", expression, method, (Type type) => IsSignedNumber(type));
		return MakeSimpleUnary(ExpressionType.NegateChecked, expression, method);
	}

	public static NewExpression New(ConstructorInfo constructor)
	{
		if (constructor == null)
		{
			throw new ArgumentNullException("constructor");
		}
		if (constructor.GetParameters().Length > 0)
		{
			throw new ArgumentException("Constructor must be parameter less");
		}
		return new NewExpression(constructor, ((IEnumerable<Expression>)null).ToReadOnlyCollection(), null);
	}

	public static NewExpression New(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		CheckNotVoid(type);
		ReadOnlyCollection<Expression> arguments = ((IEnumerable<Expression>)null).ToReadOnlyCollection();
		if (type.IsValueType)
		{
			return new NewExpression(type, arguments);
		}
		ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
		if (constructor == null)
		{
			throw new ArgumentException("Type doesn't have a parameter less constructor");
		}
		return new NewExpression(constructor, arguments, null);
	}

	public static NewExpression New(ConstructorInfo constructor, params Expression[] arguments)
	{
		return New(constructor, (IEnumerable<Expression>)arguments);
	}

	public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments)
	{
		if (constructor == null)
		{
			throw new ArgumentNullException("constructor");
		}
		ReadOnlyCollection<Expression> arguments2 = CheckMethodArguments(constructor, arguments);
		return new NewExpression(constructor, arguments2, null);
	}

	private static IList<Expression> CreateArgumentList(IEnumerable<Expression> arguments)
	{
		if (arguments == null)
		{
			return arguments.ToReadOnlyCollection();
		}
		return arguments.ToList();
	}

	private static void CheckNonGenericMethod(MethodBase method)
	{
		if (method.IsGenericMethodDefinition || method.ContainsGenericParameters)
		{
			throw new ArgumentException("Can not used open generic methods");
		}
	}

	private static ReadOnlyCollection<Expression> CheckMethodArguments(MethodBase method, IEnumerable<Expression> args)
	{
		CheckNonGenericMethod(method);
		IList<Expression> list = CreateArgumentList(args);
		ParameterInfo[] parameters = method.GetParameters();
		if (list.Count != parameters.Length)
		{
			throw new ArgumentException("The number of arguments doesn't match the number of parameters");
		}
		for (int i = 0; i < parameters.Length; i++)
		{
			if (list[i] == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (!IsAssignableToParameterType(list[i].Type, parameters[i]))
			{
				if (!parameters[i].ParameterType.IsExpression())
				{
					throw new ArgumentException("arguments");
				}
				list[i] = Quote(list[i]);
			}
		}
		return list.ToReadOnlyCollection();
	}

	public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, params MemberInfo[] members)
	{
		return New(constructor, arguments, (IEnumerable<MemberInfo>)members);
	}

	public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, IEnumerable<MemberInfo> members)
	{
		if (constructor == null)
		{
			throw new ArgumentNullException("constructor");
		}
		ReadOnlyCollection<Expression> collection = arguments.ToReadOnlyCollection();
		ReadOnlyCollection<MemberInfo> readOnlyCollection = members.ToReadOnlyCollection();
		CheckForNull(collection, "arguments");
		CheckForNull(readOnlyCollection, "members");
		collection = CheckMethodArguments(constructor, arguments);
		if (collection.Count != readOnlyCollection.Count)
		{
			throw new ArgumentException("Arguments count does not match members count");
		}
		for (int i = 0; i < readOnlyCollection.Count; i++)
		{
			MemberInfo memberInfo = readOnlyCollection[i];
			Type type = null;
			switch (memberInfo.MemberType)
			{
			case MemberTypes.Field:
				type = (memberInfo as FieldInfo).FieldType;
				break;
			case MemberTypes.Method:
				type = (memberInfo as MethodInfo).ReturnType;
				break;
			case MemberTypes.Property:
			{
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				if (propertyInfo.GetGetMethod(nonPublic: true) == null)
				{
					throw new ArgumentException("Property must have a getter");
				}
				type = (memberInfo as PropertyInfo).PropertyType;
				break;
			}
			default:
				throw new ArgumentException("Member type not allowed");
			}
			if (!collection[i].Type.IsAssignableTo(type))
			{
				throw new ArgumentException("Argument type not assignable to member type");
			}
		}
		return new NewExpression(constructor, collection, readOnlyCollection);
	}

	public static NewArrayExpression NewArrayBounds(Type type, params Expression[] bounds)
	{
		return NewArrayBounds(type, (IEnumerable<Expression>)bounds);
	}

	public static NewArrayExpression NewArrayBounds(Type type, IEnumerable<Expression> bounds)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (bounds == null)
		{
			throw new ArgumentNullException("bounds");
		}
		CheckNotVoid(type);
		ReadOnlyCollection<Expression> readOnlyCollection = bounds.ToReadOnlyCollection();
		foreach (Expression item in readOnlyCollection)
		{
			if (!IsInt(item.Type))
			{
				throw new ArgumentException("The bounds collection can only contain expression of integers types");
			}
		}
		return new NewArrayExpression(ExpressionType.NewArrayBounds, type.MakeArrayType(readOnlyCollection.Count), readOnlyCollection);
	}

	public static NewArrayExpression NewArrayInit(Type type, params Expression[] initializers)
	{
		return NewArrayInit(type, (IEnumerable<Expression>)initializers);
	}

	public static NewArrayExpression NewArrayInit(Type type, IEnumerable<Expression> initializers)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (initializers == null)
		{
			throw new ArgumentNullException("initializers");
		}
		CheckNotVoid(type);
		ReadOnlyCollection<Expression> readOnlyCollection = initializers.ToReadOnlyCollection();
		foreach (Expression item in readOnlyCollection)
		{
			if (item == null)
			{
				throw new ArgumentNullException("initializers");
			}
			if (!item.Type.IsAssignableTo(type))
			{
				throw new InvalidOperationException($"{item.Type} IsAssignableTo {type}, expression [ {item.NodeType} ] : {item}");
			}
		}
		return new NewArrayExpression(ExpressionType.NewArrayInit, type.MakeArrayType(), readOnlyCollection);
	}

	public static UnaryExpression Not(Expression expression)
	{
		return Not(expression, null);
	}

	public static UnaryExpression Not(Expression expression, MethodInfo method)
	{
		Func<Type, bool> validator = (Type type) => IsIntOrBool(type);
		method = UnaryCoreCheck("op_LogicalNot", expression, method, validator);
		if (method == null)
		{
			method = UnaryCoreCheck("op_OnesComplement", expression, method, validator);
		}
		return MakeSimpleUnary(ExpressionType.Not, expression, method);
	}

	private static void CheckNotVoid(Type type)
	{
		if (type == typeof(void))
		{
			throw new ArgumentException("Type can't be void");
		}
	}

	public static ParameterExpression Parameter(Type type, string name)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		CheckNotVoid(type);
		return new ParameterExpression(type, name);
	}

	public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
	{
		if (propertyAccessor == null)
		{
			throw new ArgumentNullException("propertyAccessor");
		}
		CheckNonGenericMethod(propertyAccessor);
		if (!propertyAccessor.IsStatic)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (!expression.Type.IsAssignableTo(propertyAccessor.DeclaringType))
			{
				throw new ArgumentException("expression");
			}
		}
		PropertyInfo associatedProperty = GetAssociatedProperty(propertyAccessor);
		if (associatedProperty == null)
		{
			throw new ArgumentException($"Method {propertyAccessor} has no associated property");
		}
		return new MemberExpression(expression, associatedProperty, associatedProperty.PropertyType);
	}

	private static PropertyInfo GetAssociatedProperty(MethodInfo method)
	{
		if (method == null)
		{
			return null;
		}
		PropertyInfo[] properties = method.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (method.Equals(propertyInfo.GetGetMethod(nonPublic: true)))
			{
				return propertyInfo;
			}
			if (method.Equals(propertyInfo.GetSetMethod(nonPublic: true)))
			{
				return propertyInfo;
			}
		}
		return null;
	}

	public static MemberExpression Property(Expression expression, PropertyInfo property)
	{
		if (property == null)
		{
			throw new ArgumentNullException("property");
		}
		MethodInfo getMethod = property.GetGetMethod(nonPublic: true);
		if (getMethod == null)
		{
			throw new ArgumentException("getter");
		}
		if (!getMethod.IsStatic)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (!expression.Type.IsAssignableTo(property.DeclaringType))
			{
				throw new ArgumentException("expression");
			}
		}
		else if (expression != null)
		{
			throw new ArgumentException("expression");
		}
		return new MemberExpression(expression, property, property.PropertyType);
	}

	public static MemberExpression Property(Expression expression, string propertyName)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		PropertyInfo property = expression.Type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (property == null)
		{
			throw new ArgumentException($"No property named {propertyName} on {expression.Type}");
		}
		return new MemberExpression(expression, property, property.PropertyType);
	}

	public static MemberExpression PropertyOrField(Expression expression, string propertyOrFieldName)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (propertyOrFieldName == null)
		{
			throw new ArgumentNullException("propertyOrFieldName");
		}
		PropertyInfo property = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (property != null)
		{
			return new MemberExpression(expression, property, property.PropertyType);
		}
		FieldInfo field = expression.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (field != null)
		{
			return new MemberExpression(expression, field, field.FieldType);
		}
		throw new ArgumentException($"No field or property named {propertyOrFieldName} on {expression.Type}");
	}

	public static UnaryExpression Quote(Expression expression)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		return new UnaryExpression(ExpressionType.Quote, expression, expression.GetType());
	}

	public static UnaryExpression TypeAs(Expression expression, Type type)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (type.IsValueType && !type.IsNullable())
		{
			throw new ArgumentException("TypeAs expect a reference or a nullable type");
		}
		return new UnaryExpression(ExpressionType.TypeAs, expression, type);
	}

	public static TypeBinaryExpression TypeIs(Expression expression, Type type)
	{
		if (expression == null)
		{
			throw new ArgumentNullException("expression");
		}
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		CheckNotVoid(type);
		return new TypeBinaryExpression(ExpressionType.TypeIs, expression, type, typeof(bool));
	}

	public static UnaryExpression UnaryPlus(Expression expression)
	{
		return UnaryPlus(expression, null);
	}

	public static UnaryExpression UnaryPlus(Expression expression, MethodInfo method)
	{
		method = UnaryCoreCheck("op_UnaryPlus", expression, method, (Type type) => IsNumber(type));
		return MakeSimpleUnary(ExpressionType.UnaryPlus, expression, method);
	}

	private static bool IsInt(Type t)
	{
		return t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort) || t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong);
	}

	private static bool IsIntOrBool(Type t)
	{
		return IsInt(t) || t == typeof(bool);
	}

	private static bool IsNumber(Type t)
	{
		if (IsInt(t))
		{
			return true;
		}
		return t == typeof(float) || t == typeof(double);
	}

	private static bool IsSignedNumber(Type t)
	{
		return IsNumber(t) && !IsUnsigned(t);
	}

	internal static bool IsUnsigned(Type t)
	{
		if (t.IsPointer)
		{
			return IsUnsigned(t.GetElementType());
		}
		return t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong) || t == typeof(byte);
	}

	internal virtual void Emit(EmitContext ec)
	{
		throw new NotImplementedException($"Emit method is not implemented in expression type {GetType()}");
	}
}

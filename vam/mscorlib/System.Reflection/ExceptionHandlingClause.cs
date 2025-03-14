using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection;

[ComVisible(true)]
public sealed class ExceptionHandlingClause
{
	internal Type catch_type;

	internal int filter_offset;

	internal ExceptionHandlingClauseOptions flags;

	internal int try_offset;

	internal int try_length;

	internal int handler_offset;

	internal int handler_length;

	public Type CatchType => catch_type;

	public int FilterOffset => filter_offset;

	public ExceptionHandlingClauseOptions Flags => flags;

	public int HandlerLength => handler_length;

	public int HandlerOffset => handler_offset;

	public int TryLength => try_length;

	public int TryOffset => try_offset;

	internal ExceptionHandlingClause()
	{
	}

	public override string ToString()
	{
		string text = $"Flags={flags}, TryOffset={try_offset}, TryLength={try_length}, HandlerOffset={handler_offset}, HandlerLength={handler_length}";
		if (catch_type != null)
		{
			text = $"{text}, CatchType={catch_type}";
		}
		if (flags == ExceptionHandlingClauseOptions.Filter)
		{
			text = string.Format(CultureInfo.InvariantCulture, "{0}, FilterOffset={1}", text, filter_offset);
		}
		return text;
	}
}

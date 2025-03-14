using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Reflection;

[ComVisible(true)]
public sealed class MethodBody
{
	private ExceptionHandlingClause[] clauses;

	private LocalVariableInfo[] locals;

	private byte[] il;

	private bool init_locals;

	private int sig_token;

	private int max_stack;

	public IList<ExceptionHandlingClause> ExceptionHandlingClauses => Array.AsReadOnly(clauses);

	public IList<LocalVariableInfo> LocalVariables => Array.AsReadOnly(locals);

	public bool InitLocals => init_locals;

	public int LocalSignatureMetadataToken => sig_token;

	public int MaxStackSize => max_stack;

	internal MethodBody()
	{
	}

	public byte[] GetILAsByteArray()
	{
		return il;
	}
}

using System.Collections.Generic;

namespace Mono.CompilerServices.SymbolWriter;

public class SourceMethodBuilder
{
	private List<LocalVariableEntry> _locals;

	private List<CodeBlockEntry> _blocks;

	private List<ScopeVariable> _scope_vars;

	private Stack<CodeBlockEntry> _block_stack;

	private readonly List<LineNumberEntry> method_lines;

	private readonly ICompileUnit _comp_unit;

	private readonly int ns_id;

	private readonly IMethodDef method;

	public CodeBlockEntry[] Blocks
	{
		get
		{
			if (_blocks == null)
			{
				return new CodeBlockEntry[0];
			}
			CodeBlockEntry[] array = new CodeBlockEntry[_blocks.Count];
			_blocks.CopyTo(array, 0);
			return array;
		}
	}

	public CodeBlockEntry CurrentBlock
	{
		get
		{
			if (_block_stack != null && _block_stack.Count > 0)
			{
				return _block_stack.Peek();
			}
			return null;
		}
	}

	public LocalVariableEntry[] Locals
	{
		get
		{
			if (_locals == null)
			{
				return new LocalVariableEntry[0];
			}
			return _locals.ToArray();
		}
	}

	public ICompileUnit SourceFile => _comp_unit;

	public ScopeVariable[] ScopeVariables
	{
		get
		{
			if (_scope_vars == null)
			{
				return new ScopeVariable[0];
			}
			return _scope_vars.ToArray();
		}
	}

	public SourceMethodBuilder(ICompileUnit comp_unit)
	{
		_comp_unit = comp_unit;
		method_lines = new List<LineNumberEntry>();
	}

	public SourceMethodBuilder(ICompileUnit comp_unit, int ns_id, IMethodDef method)
		: this(comp_unit)
	{
		this.ns_id = ns_id;
		this.method = method;
	}

	public void MarkSequencePoint(int offset, SourceFileEntry file, int line, int column, bool is_hidden)
	{
		MarkSequencePoint(offset, file, line, column, -1, -1, is_hidden);
	}

	public void MarkSequencePoint(int offset, SourceFileEntry file, int line, int column, int end_line, int end_column, bool is_hidden)
	{
		LineNumberEntry lineNumberEntry = new LineNumberEntry(file?.Index ?? 0, line, column, end_line, end_column, offset, is_hidden);
		if (method_lines.Count > 0)
		{
			LineNumberEntry lineNumberEntry2 = method_lines[method_lines.Count - 1];
			if (lineNumberEntry2.Offset == offset)
			{
				if (LineNumberEntry.LocationComparer.Default.Compare(lineNumberEntry, lineNumberEntry2) > 0)
				{
					method_lines[method_lines.Count - 1] = lineNumberEntry;
				}
				return;
			}
		}
		method_lines.Add(lineNumberEntry);
	}

	public void StartBlock(CodeBlockEntry.Type type, int start_offset)
	{
		if (_block_stack == null)
		{
			_block_stack = new Stack<CodeBlockEntry>();
		}
		if (_blocks == null)
		{
			_blocks = new List<CodeBlockEntry>();
		}
		int parent = ((CurrentBlock != null) ? CurrentBlock.Index : (-1));
		CodeBlockEntry codeBlockEntry = new CodeBlockEntry(_blocks.Count + 1, parent, type, start_offset);
		_block_stack.Push(codeBlockEntry);
		_blocks.Add(codeBlockEntry);
	}

	public void EndBlock(int end_offset)
	{
		_block_stack.Pop().Close(end_offset);
	}

	public void AddLocal(int index, string name)
	{
		if (_locals == null)
		{
			_locals = new List<LocalVariableEntry>();
		}
		int block = ((CurrentBlock != null) ? CurrentBlock.Index : 0);
		_locals.Add(new LocalVariableEntry(index, name, block));
	}

	public void AddScopeVariable(int scope, int index)
	{
		if (_scope_vars == null)
		{
			_scope_vars = new List<ScopeVariable>();
		}
		_scope_vars.Add(new ScopeVariable(scope, index));
	}

	public void DefineMethod(MonoSymbolFile file)
	{
		DefineMethod(file, method.Token);
	}

	public void DefineMethod(MonoSymbolFile file, int token)
	{
		MethodEntry entry = new MethodEntry(file, _comp_unit.Entry, token, ScopeVariables, Locals, method_lines.ToArray(), Blocks, null, MethodEntry.Flags.ColumnsInfoIncluded, ns_id);
		file.AddMethod(entry);
	}
}

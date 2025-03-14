using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace System.Windows.Forms;

internal class DebugHelper
{
	private struct Data
	{
		public MethodBase method;

		public object[] args;

		public Data(MethodBase m, object[] a)
		{
			method = m;
			args = a;
		}
	}

	private static Stack<Data> methods;

	static DebugHelper()
	{
		methods = new Stack<Data>();
		Debug.AutoFlush = true;
	}

	[Conditional("DEBUG")]
	internal static void DumpCallers()
	{
		StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
		int frameCount = stackTrace.FrameCount;
		for (int i = 1; i < frameCount; i++)
		{
			StackFrame frame = stackTrace.GetFrame(i);
			MethodBase method = frame.GetMethod();
			string fileName = frame.GetFileName();
			if (fileName != null && fileName.Length > 1)
			{
				fileName = fileName.Substring(fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
			}
		}
	}

	[Conditional("DEBUG")]
	internal static void DumpCallers(int count)
	{
		StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
		int num = ((count <= stackTrace.FrameCount) ? count : stackTrace.FrameCount);
		for (int i = 1; i < num; i++)
		{
			StackFrame frame = stackTrace.GetFrame(i);
			MethodBase method = frame.GetMethod();
			string fileName = frame.GetFileName();
			if (fileName != null && fileName.Length > 1)
			{
				fileName = fileName.Substring(fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
			}
		}
	}

	[Conditional("DEBUG")]
	internal static void Enter()
	{
		StackTrace stackTrace = new StackTrace();
		methods.Push(new Data(stackTrace.GetFrame(1).GetMethod(), null));
	}

	[Conditional("DEBUG")]
	internal static void Enter(object[] args)
	{
		StackTrace stackTrace = new StackTrace();
		methods.Push(new Data(stackTrace.GetFrame(1).GetMethod(), args));
	}

	[Conditional("DEBUG")]
	internal static void Leave()
	{
		if (methods.Count > 0)
		{
			methods.Pop();
		}
	}

	[Conditional("DEBUG")]
	internal static void Print()
	{
		if (methods.Count != 0)
		{
			Data data = methods.Peek();
		}
	}

	[Conditional("DEBUG")]
	internal static void Print(int index)
	{
		if (methods.Count != 0 && methods.Count > index && index >= 0)
		{
			Stack<Data> stack = new Stack<Data>(index - 1);
			for (int i = 0; i < index; i++)
			{
				stack.Push(methods.Pop());
			}
			Data data = methods.Peek();
			for (int j = 0; j < stack.Count; j++)
			{
				methods.Push(stack.Pop());
			}
			stack = null;
		}
	}

	[Conditional("DEBUG")]
	internal static void Print(string methodName, string parameterName)
	{
		if (methods.Count == 0)
		{
			return;
		}
		Stack<Data> stack = new Stack<Data>();
		Data data = methods.Peek();
		bool flag = false;
		for (int i = 0; i < methods.Count; i++)
		{
			data = methods.Peek();
			if (data.method.Name.Equals(methodName))
			{
				flag = true;
				break;
			}
			stack.Push(methods.Pop());
		}
		for (int j = 0; j < stack.Count; j++)
		{
			methods.Push(stack.Pop());
		}
		stack = null;
		if (!flag)
		{
			return;
		}
		ParameterInfo[] parameters = data.method.GetParameters();
		for (int k = 0; k < parameters.Length; k++)
		{
			if (!(parameters[k].Name == parameterName) || parameters[k].ParameterType == typeof(IntPtr))
			{
			}
		}
	}

	[Conditional("DEBUG")]
	internal static void Print(string parameterName)
	{
		if (methods.Count == 0)
		{
			return;
		}
		ParameterInfo[] parameters = methods.Peek().method.GetParameters();
		for (int i = 0; i < parameters.Length; i++)
		{
			if (!(parameters[i].Name == parameterName) || parameters[i].ParameterType == typeof(IntPtr))
			{
			}
		}
	}

	[Conditional("DEBUG")]
	internal static void WriteLine(object arg)
	{
	}

	[Conditional("DEBUG")]
	internal static void WriteLine(string format, params object[] arg)
	{
	}

	[Conditional("DEBUG")]
	internal static void WriteLine(string message)
	{
	}

	[Conditional("DEBUG")]
	internal static void Indent()
	{
	}

	[Conditional("DEBUG")]
	internal static void Unindent()
	{
	}

	[Conditional("TRACE")]
	internal static void TraceWriteLine(string format, params object[] arg)
	{
	}

	[Conditional("TRACE")]
	internal static void TraceWriteLine(string message)
	{
	}
}

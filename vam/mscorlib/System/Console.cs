using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace System;

public static class Console
{
	private class WindowsConsole
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern int GetConsoleCP();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern int GetConsoleOutputCP();

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static int GetInputCodePage()
		{
			return GetConsoleCP();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static int GetOutputCodePage()
		{
			return GetConsoleOutputCP();
		}
	}

	private delegate void InternalCancelHandler();

	internal static TextWriter stdout;

	private static TextWriter stderr;

	private static TextReader stdin;

	private static Encoding inputEncoding;

	private static Encoding outputEncoding;

	private static ConsoleCancelEventHandler cancel_event;

	private static readonly InternalCancelHandler cancel_handler;

	public static TextWriter Error => stderr;

	public static TextWriter Out => stdout;

	public static TextReader In => stdin;

	public static Encoding InputEncoding
	{
		get
		{
			return inputEncoding;
		}
		set
		{
			inputEncoding = value;
			SetEncodings(inputEncoding, outputEncoding);
		}
	}

	public static Encoding OutputEncoding
	{
		get
		{
			return outputEncoding;
		}
		set
		{
			outputEncoding = value;
			SetEncodings(inputEncoding, outputEncoding);
		}
	}

	public static ConsoleColor BackgroundColor
	{
		get
		{
			return ConsoleDriver.BackgroundColor;
		}
		set
		{
			ConsoleDriver.BackgroundColor = value;
		}
	}

	public static int BufferHeight
	{
		get
		{
			return ConsoleDriver.BufferHeight;
		}
		[MonoLimitation("Implemented only on Windows")]
		set
		{
			ConsoleDriver.BufferHeight = value;
		}
	}

	public static int BufferWidth
	{
		get
		{
			return ConsoleDriver.BufferWidth;
		}
		[MonoLimitation("Implemented only on Windows")]
		set
		{
			ConsoleDriver.BufferWidth = value;
		}
	}

	[MonoLimitation("Implemented only on Windows")]
	public static bool CapsLock => ConsoleDriver.CapsLock;

	public static int CursorLeft
	{
		get
		{
			return ConsoleDriver.CursorLeft;
		}
		set
		{
			ConsoleDriver.CursorLeft = value;
		}
	}

	public static int CursorTop
	{
		get
		{
			return ConsoleDriver.CursorTop;
		}
		set
		{
			ConsoleDriver.CursorTop = value;
		}
	}

	public static int CursorSize
	{
		get
		{
			return ConsoleDriver.CursorSize;
		}
		set
		{
			ConsoleDriver.CursorSize = value;
		}
	}

	public static bool CursorVisible
	{
		get
		{
			return ConsoleDriver.CursorVisible;
		}
		set
		{
			ConsoleDriver.CursorVisible = value;
		}
	}

	public static ConsoleColor ForegroundColor
	{
		get
		{
			return ConsoleDriver.ForegroundColor;
		}
		set
		{
			ConsoleDriver.ForegroundColor = value;
		}
	}

	public static bool KeyAvailable => ConsoleDriver.KeyAvailable;

	public static int LargestWindowHeight => ConsoleDriver.LargestWindowHeight;

	public static int LargestWindowWidth => ConsoleDriver.LargestWindowWidth;

	[MonoLimitation("Only works on windows")]
	public static bool NumberLock => ConsoleDriver.NumberLock;

	public static string Title
	{
		get
		{
			return ConsoleDriver.Title;
		}
		set
		{
			ConsoleDriver.Title = value;
		}
	}

	public static bool TreatControlCAsInput
	{
		get
		{
			return ConsoleDriver.TreatControlCAsInput;
		}
		set
		{
			ConsoleDriver.TreatControlCAsInput = value;
		}
	}

	[MonoLimitation("Only works on windows")]
	public static int WindowHeight
	{
		get
		{
			return ConsoleDriver.WindowHeight;
		}
		set
		{
			ConsoleDriver.WindowHeight = value;
		}
	}

	[MonoLimitation("Only works on windows")]
	public static int WindowLeft
	{
		get
		{
			return ConsoleDriver.WindowLeft;
		}
		set
		{
			ConsoleDriver.WindowLeft = value;
		}
	}

	[MonoLimitation("Only works on windows")]
	public static int WindowTop
	{
		get
		{
			return ConsoleDriver.WindowTop;
		}
		set
		{
			ConsoleDriver.WindowTop = value;
		}
	}

	[MonoLimitation("Only works on windows")]
	public static int WindowWidth
	{
		get
		{
			return ConsoleDriver.WindowWidth;
		}
		set
		{
			ConsoleDriver.WindowWidth = value;
		}
	}

	public static event ConsoleCancelEventHandler CancelKeyPress
	{
		add
		{
			if (!ConsoleDriver.Initialized)
			{
				ConsoleDriver.Init();
			}
			cancel_event = (ConsoleCancelEventHandler)Delegate.Combine(cancel_event, value);
		}
		remove
		{
			if (!ConsoleDriver.Initialized)
			{
				ConsoleDriver.Init();
			}
			cancel_event = (ConsoleCancelEventHandler)Delegate.Remove(cancel_event, value);
		}
	}

	static Console()
	{
		cancel_handler = DoConsoleCancelEvent;
		if (Environment.IsRunningOnWindows)
		{
			try
			{
				inputEncoding = Encoding.GetEncoding(WindowsConsole.GetInputCodePage());
				outputEncoding = Encoding.GetEncoding(WindowsConsole.GetOutputCodePage());
			}
			catch
			{
				inputEncoding = (outputEncoding = Encoding.Default);
			}
		}
		else
		{
			int code_page = 0;
			Encoding.InternalCodePage(ref code_page);
			if (code_page != -1 && ((code_page & 0xFFFFFFF) == 3 || ((uint)code_page & 0x10000000u) != 0))
			{
				inputEncoding = (outputEncoding = Encoding.UTF8Unmarked);
			}
			else
			{
				inputEncoding = (outputEncoding = Encoding.Default);
			}
		}
		SetEncodings(inputEncoding, outputEncoding);
	}

	private static void SetEncodings(Encoding inputEncoding, Encoding outputEncoding)
	{
		stderr = new UnexceptionalStreamWriter(OpenStandardError(0), outputEncoding);
		((StreamWriter)stderr).AutoFlush = true;
		stderr = TextWriter.Synchronized(stderr, neverClose: true);
		if (!Environment.IsRunningOnWindows && ConsoleDriver.IsConsole)
		{
			StreamWriter streamWriter = new CStreamWriter(OpenStandardOutput(0), outputEncoding);
			streamWriter.AutoFlush = true;
			stdout = TextWriter.Synchronized(streamWriter, neverClose: true);
			stdin = new CStreamReader(OpenStandardInput(0), inputEncoding);
		}
		else
		{
			stdout = new UnexceptionalStreamWriter(OpenStandardOutput(0), outputEncoding);
			((StreamWriter)stdout).AutoFlush = true;
			stdout = TextWriter.Synchronized(stdout, neverClose: true);
			stdin = new UnexceptionalStreamReader(OpenStandardInput(0), inputEncoding);
			stdin = TextReader.Synchronized(stdin);
		}
		GC.SuppressFinalize(stdout);
		GC.SuppressFinalize(stderr);
		GC.SuppressFinalize(stdin);
	}

	public static Stream OpenStandardError()
	{
		return OpenStandardError(0);
	}

	private static Stream Open(IntPtr handle, FileAccess access, int bufferSize)
	{
		try
		{
			return new FileStream(handle, access, ownsHandle: false, bufferSize, isAsync: false, bufferSize == 0);
		}
		catch (IOException)
		{
			return new NullStream();
		}
	}

	[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static Stream OpenStandardError(int bufferSize)
	{
		return Open(MonoIO.ConsoleError, FileAccess.Write, bufferSize);
	}

	public static Stream OpenStandardInput()
	{
		return OpenStandardInput(0);
	}

	[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static Stream OpenStandardInput(int bufferSize)
	{
		return Open(MonoIO.ConsoleInput, FileAccess.Read, bufferSize);
	}

	public static Stream OpenStandardOutput()
	{
		return OpenStandardOutput(0);
	}

	[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static Stream OpenStandardOutput(int bufferSize)
	{
		return Open(MonoIO.ConsoleOutput, FileAccess.Write, bufferSize);
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static void SetError(TextWriter newError)
	{
		if (newError == null)
		{
			throw new ArgumentNullException("newError");
		}
		stderr = newError;
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static void SetIn(TextReader newIn)
	{
		if (newIn == null)
		{
			throw new ArgumentNullException("newIn");
		}
		stdin = newIn;
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static void SetOut(TextWriter newOut)
	{
		if (newOut == null)
		{
			throw new ArgumentNullException("newOut");
		}
		stdout = newOut;
	}

	public static void Write(bool value)
	{
		stdout.Write(value);
	}

	public static void Write(char value)
	{
		stdout.Write(value);
	}

	public static void Write(char[] buffer)
	{
		stdout.Write(buffer);
	}

	public static void Write(decimal value)
	{
		stdout.Write(value);
	}

	public static void Write(double value)
	{
		stdout.Write(value);
	}

	public static void Write(int value)
	{
		stdout.Write(value);
	}

	public static void Write(long value)
	{
		stdout.Write(value);
	}

	public static void Write(object value)
	{
		stdout.Write(value);
	}

	public static void Write(float value)
	{
		stdout.Write(value);
	}

	public static void Write(string value)
	{
		stdout.Write(value);
	}

	[CLSCompliant(false)]
	public static void Write(uint value)
	{
		stdout.Write(value);
	}

	[CLSCompliant(false)]
	public static void Write(ulong value)
	{
		stdout.Write(value);
	}

	public static void Write(string format, object arg0)
	{
		stdout.Write(format, arg0);
	}

	public static void Write(string format, params object[] arg)
	{
		stdout.Write(format, arg);
	}

	public static void Write(char[] buffer, int index, int count)
	{
		stdout.Write(buffer, index, count);
	}

	public static void Write(string format, object arg0, object arg1)
	{
		stdout.Write(format, arg0, arg1);
	}

	public static void Write(string format, object arg0, object arg1, object arg2)
	{
		stdout.Write(format, arg0, arg1, arg2);
	}

	[CLSCompliant(false)]
	public static void Write(string format, object arg0, object arg1, object arg2, object arg3, __arglist)
	{
		ArgIterator argIterator = new ArgIterator(__arglist);
		int remainingCount = argIterator.GetRemainingCount();
		object[] array = new object[remainingCount + 4];
		array[0] = arg0;
		array[1] = arg1;
		array[2] = arg2;
		array[3] = arg3;
		for (int i = 0; i < remainingCount; i++)
		{
			TypedReference nextArg = argIterator.GetNextArg();
			array[i + 4] = TypedReference.ToObject(nextArg);
		}
		stdout.Write(string.Format(format, array));
	}

	public static void WriteLine()
	{
		stdout.WriteLine();
	}

	public static void WriteLine(bool value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(char value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(char[] buffer)
	{
		stdout.WriteLine(buffer);
	}

	public static void WriteLine(decimal value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(double value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(int value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(long value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(object value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(float value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(string value)
	{
		stdout.WriteLine(value);
	}

	[CLSCompliant(false)]
	public static void WriteLine(uint value)
	{
		stdout.WriteLine(value);
	}

	[CLSCompliant(false)]
	public static void WriteLine(ulong value)
	{
		stdout.WriteLine(value);
	}

	public static void WriteLine(string format, object arg0)
	{
		stdout.WriteLine(format, arg0);
	}

	public static void WriteLine(string format, params object[] arg)
	{
		stdout.WriteLine(format, arg);
	}

	public static void WriteLine(char[] buffer, int index, int count)
	{
		stdout.WriteLine(buffer, index, count);
	}

	public static void WriteLine(string format, object arg0, object arg1)
	{
		stdout.WriteLine(format, arg0, arg1);
	}

	public static void WriteLine(string format, object arg0, object arg1, object arg2)
	{
		stdout.WriteLine(format, arg0, arg1, arg2);
	}

	[CLSCompliant(false)]
	public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, __arglist)
	{
		ArgIterator argIterator = new ArgIterator(__arglist);
		int remainingCount = argIterator.GetRemainingCount();
		object[] array = new object[remainingCount + 4];
		array[0] = arg0;
		array[1] = arg1;
		array[2] = arg2;
		array[3] = arg3;
		for (int i = 0; i < remainingCount; i++)
		{
			TypedReference nextArg = argIterator.GetNextArg();
			array[i + 4] = TypedReference.ToObject(nextArg);
		}
		stdout.WriteLine(string.Format(format, array));
	}

	public static int Read()
	{
		if (stdin is CStreamReader && ConsoleDriver.IsConsole)
		{
			return ConsoleDriver.Read();
		}
		return stdin.Read();
	}

	public static string ReadLine()
	{
		if (stdin is CStreamReader && ConsoleDriver.IsConsole)
		{
			return ConsoleDriver.ReadLine();
		}
		return stdin.ReadLine();
	}

	public static void Beep()
	{
		Beep(1000, 500);
	}

	public static void Beep(int frequency, int duration)
	{
		if (frequency < 37 || frequency > 32767)
		{
			throw new ArgumentOutOfRangeException("frequency");
		}
		if (duration <= 0)
		{
			throw new ArgumentOutOfRangeException("duration");
		}
		ConsoleDriver.Beep(frequency, duration);
	}

	public static void Clear()
	{
		ConsoleDriver.Clear();
	}

	[MonoLimitation("Implemented only on Windows")]
	public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
	{
		ConsoleDriver.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
	}

	[MonoLimitation("Implemented only on Windows")]
	public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
	{
		ConsoleDriver.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, sourceForeColor, sourceBackColor);
	}

	public static ConsoleKeyInfo ReadKey()
	{
		return ReadKey(intercept: false);
	}

	public static ConsoleKeyInfo ReadKey(bool intercept)
	{
		return ConsoleDriver.ReadKey(intercept);
	}

	public static void ResetColor()
	{
		ConsoleDriver.ResetColor();
	}

	[MonoLimitation("Only works on windows")]
	public static void SetBufferSize(int width, int height)
	{
		ConsoleDriver.SetBufferSize(width, height);
	}

	public static void SetCursorPosition(int left, int top)
	{
		ConsoleDriver.SetCursorPosition(left, top);
	}

	public static void SetWindowPosition(int left, int top)
	{
		ConsoleDriver.SetWindowPosition(left, top);
	}

	public static void SetWindowSize(int width, int height)
	{
		ConsoleDriver.SetWindowSize(width, height);
	}

	internal static void DoConsoleCancelEvent()
	{
		bool flag = true;
		if (cancel_event != null)
		{
			ConsoleCancelEventArgs consoleCancelEventArgs = new ConsoleCancelEventArgs(ConsoleSpecialKey.ControlC);
			Delegate[] invocationList = cancel_event.GetInvocationList();
			Delegate[] array = invocationList;
			for (int i = 0; i < array.Length; i++)
			{
				ConsoleCancelEventHandler consoleCancelEventHandler = (ConsoleCancelEventHandler)array[i];
				try
				{
					consoleCancelEventHandler(null, consoleCancelEventArgs);
				}
				catch
				{
				}
			}
			flag = !consoleCancelEventArgs.Cancel;
		}
		if (flag)
		{
			Environment.Exit(58);
		}
	}
}

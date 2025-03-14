using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.IO.Ports;

internal class WinSerialStream : Stream, IDisposable, System.IO.Ports.ISerialStream
{
	private const uint GenericRead = 2147483648u;

	private const uint GenericWrite = 1073741824u;

	private const uint OpenExisting = 3u;

	private const uint FileFlagOverlapped = 1073741824u;

	private const uint PurgeRxClear = 4u;

	private const uint PurgeTxClear = 8u;

	private const uint WinInfiniteTimeout = uint.MaxValue;

	private const uint FileIOPending = 997u;

	private const uint SetRts = 3u;

	private const uint ClearRts = 4u;

	private const uint SetDtr = 5u;

	private const uint ClearDtr = 6u;

	private const uint SetBreak = 8u;

	private const uint ClearBreak = 9u;

	private const uint CtsOn = 16u;

	private const uint DsrOn = 32u;

	private const uint RsldOn = 128u;

	private const uint EvRxChar = 1u;

	private const uint EvCts = 8u;

	private const uint EvDsr = 16u;

	private const uint EvRlsd = 32u;

	private const uint EvBreak = 64u;

	private const uint EvErr = 128u;

	private const uint EvRing = 256u;

	private int handle;

	private int read_timeout;

	private int write_timeout;

	private bool disposed;

	private IntPtr write_overlapped;

	private IntPtr read_overlapped;

	private ManualResetEvent read_event;

	private ManualResetEvent write_event;

	private System.IO.Ports.Timeouts timeouts;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanTimeout => true;

	public override bool CanWrite => true;

	public override int ReadTimeout
	{
		get
		{
			return read_timeout;
		}
		set
		{
			if (value < 0 && value != -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			timeouts.SetValues(value, write_timeout);
			if (!SetCommTimeouts(handle, timeouts))
			{
				ReportIOError(null);
			}
			read_timeout = value;
		}
	}

	public override int WriteTimeout
	{
		get
		{
			return write_timeout;
		}
		set
		{
			if (value < 0 && value != -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			timeouts.SetValues(read_timeout, value);
			if (!SetCommTimeouts(handle, timeouts))
			{
				ReportIOError(null);
			}
			write_timeout = value;
		}
	}

	public override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public int BytesToRead
	{
		get
		{
			if (!ClearCommError(handle, out var _, out var stat))
			{
				ReportIOError(null);
			}
			return (int)stat.BytesIn;
		}
	}

	public int BytesToWrite
	{
		get
		{
			if (!ClearCommError(handle, out var _, out var stat))
			{
				ReportIOError(null);
			}
			return (int)stat.BytesOut;
		}
	}

	public WinSerialStream(string port_name, int baud_rate, int data_bits, Parity parity, StopBits sb, bool dtr_enable, bool rts_enable, Handshake hs, int read_timeout, int write_timeout, int read_buffer_size, int write_buffer_size)
	{
		handle = CreateFile(port_name, 3221225472u, 0u, 0u, 3u, 1073741824u, 0u);
		if (handle == -1)
		{
			ReportIOError(port_name);
		}
		SetAttributes(baud_rate, parity, data_bits, sb, hs);
		if (!PurgeComm(handle, 12u) || !SetupComm(handle, read_buffer_size, write_buffer_size))
		{
			ReportIOError(null);
		}
		this.read_timeout = read_timeout;
		this.write_timeout = write_timeout;
		timeouts = new System.IO.Ports.Timeouts(read_timeout, write_timeout);
		if (!SetCommTimeouts(handle, timeouts))
		{
			ReportIOError(null);
		}
		SetSignal(System.IO.Ports.SerialSignal.Dtr, dtr_enable);
		if (hs != Handshake.RequestToSend && hs != Handshake.RequestToSendXOnXOff)
		{
			SetSignal(System.IO.Ports.SerialSignal.Rts, rts_enable);
		}
		NativeOverlapped nativeOverlapped = default(NativeOverlapped);
		write_event = new ManualResetEvent(initialState: false);
		nativeOverlapped.EventHandle = write_event.Handle;
		write_overlapped = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
		Marshal.StructureToPtr(nativeOverlapped, write_overlapped, fDeleteOld: true);
		NativeOverlapped nativeOverlapped2 = default(NativeOverlapped);
		read_event = new ManualResetEvent(initialState: false);
		nativeOverlapped2.EventHandle = read_event.Handle;
		read_overlapped = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
		Marshal.StructureToPtr(nativeOverlapped2, read_overlapped, fDeleteOld: true);
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	[DllImport("kernel32", SetLastError = true)]
	private static extern int CreateFile(string port_name, uint desired_access, uint share_mode, uint security_attrs, uint creation, uint flags, uint template);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool SetupComm(int handle, int read_buffer_size, int write_buffer_size);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool PurgeComm(int handle, uint flags);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool SetCommTimeouts(int handle, System.IO.Ports.Timeouts timeouts);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool CloseHandle(int handle);

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			disposed = true;
			CloseHandle(handle);
			Marshal.FreeHGlobal(write_overlapped);
			Marshal.FreeHGlobal(read_overlapped);
		}
	}

	public override void Close()
	{
		((IDisposable)this).Dispose();
	}

	~WinSerialStream()
	{
		Dispose(disposing: false);
	}

	public override void Flush()
	{
		CheckDisposed();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	[DllImport("kernel32", SetLastError = true)]
	private unsafe static extern bool ReadFile(int handle, byte* buffer, int bytes_to_read, out int bytes_read, IntPtr overlapped);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool GetOverlappedResult(int handle, IntPtr overlapped, ref int bytes_transfered, bool wait);

	public unsafe override int Read([In][Out] byte[] buffer, int offset, int count)
	{
		//IL_0060->IL0067: Incompatible stack types: I vs Ref
		CheckDisposed();
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0 || count < 0)
		{
			throw new ArgumentOutOfRangeException("offset or count less than zero.");
		}
		if (buffer.Length - offset < count)
		{
			throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
		}
		int bytes_read;
		fixed (byte* ptr = &System.Runtime.CompilerServices.Unsafe.AsRef<byte>((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref buffer != null && buffer.Length != 0 ? ref System.Runtime.CompilerServices.Unsafe.As<byte, _003F>(ref buffer[0]) : ref *(_003F*)null)))
		{
			if (ReadFile(handle, ptr + offset, count, out bytes_read, read_overlapped))
			{
				return bytes_read;
			}
			if ((long)Marshal.GetLastWin32Error() != 997)
			{
				ReportIOError(null);
			}
			if (!GetOverlappedResult(handle, read_overlapped, ref bytes_read, wait: true))
			{
				ReportIOError(null);
			}
		}
		if (bytes_read == 0)
		{
			throw new TimeoutException();
		}
		return bytes_read;
	}

	[DllImport("kernel32", SetLastError = true)]
	private unsafe static extern bool WriteFile(int handle, byte* buffer, int bytes_to_write, out int bytes_written, IntPtr overlapped);

	public unsafe override void Write(byte[] buffer, int offset, int count)
	{
		//IL_005d->IL0064: Incompatible stack types: I vs Ref
		CheckDisposed();
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0 || count < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (buffer.Length - offset < count)
		{
			throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
		}
		int bytes_written = 0;
		fixed (byte* ptr = &System.Runtime.CompilerServices.Unsafe.AsRef<byte>((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref buffer != null && buffer.Length != 0 ? ref System.Runtime.CompilerServices.Unsafe.As<byte, _003F>(ref buffer[0]) : ref *(_003F*)null)))
		{
			if (WriteFile(handle, ptr + offset, count, out bytes_written, write_overlapped))
			{
				return;
			}
			if ((long)Marshal.GetLastWin32Error() != 997)
			{
				ReportIOError(null);
			}
			if (!GetOverlappedResult(handle, write_overlapped, ref bytes_written, wait: true))
			{
				ReportIOError(null);
			}
		}
		if (bytes_written < count)
		{
			throw new TimeoutException();
		}
	}

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool GetCommState(int handle, [Out] System.IO.Ports.DCB dcb);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool SetCommState(int handle, System.IO.Ports.DCB dcb);

	public void SetAttributes(int baud_rate, Parity parity, int data_bits, StopBits bits, Handshake hs)
	{
		System.IO.Ports.DCB dCB = new System.IO.Ports.DCB();
		if (!GetCommState(handle, dCB))
		{
			ReportIOError(null);
		}
		dCB.SetValues(baud_rate, parity, data_bits, bits, hs);
		if (!SetCommState(handle, dCB))
		{
			ReportIOError(null);
		}
	}

	private void ReportIOError(string optional_arg)
	{
		string message;
		switch (Marshal.GetLastWin32Error())
		{
		case 2:
		case 3:
			message = "The port `" + optional_arg + "' does not exist.";
			break;
		case 87:
			message = "Parameter is incorrect.";
			break;
		default:
			message = new Win32Exception().Message;
			break;
		}
		throw new IOException(message);
	}

	private void CheckDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
	}

	public void DiscardInBuffer()
	{
		if (!PurgeComm(handle, 4u))
		{
			ReportIOError(null);
		}
	}

	public void DiscardOutBuffer()
	{
		if (!PurgeComm(handle, 4u))
		{
			ReportIOError(null);
		}
	}

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool ClearCommError(int handle, out uint errors, out System.IO.Ports.CommStat stat);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool GetCommModemStatus(int handle, out uint flags);

	public System.IO.Ports.SerialSignal GetSignals()
	{
		if (!GetCommModemStatus(handle, out var flags))
		{
			ReportIOError(null);
		}
		System.IO.Ports.SerialSignal serialSignal = System.IO.Ports.SerialSignal.None;
		if ((flags & 0x80u) != 0)
		{
			serialSignal |= System.IO.Ports.SerialSignal.Cd;
		}
		if ((flags & 0x10u) != 0)
		{
			serialSignal |= System.IO.Ports.SerialSignal.Cts;
		}
		if ((flags & 0x20u) != 0)
		{
			serialSignal |= System.IO.Ports.SerialSignal.Dsr;
		}
		return serialSignal;
	}

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool EscapeCommFunction(int handle, uint flags);

	public void SetSignal(System.IO.Ports.SerialSignal signal, bool value)
	{
		if (signal != System.IO.Ports.SerialSignal.Rts && signal != System.IO.Ports.SerialSignal.Dtr)
		{
			throw new Exception("Wrong internal value");
		}
		uint flags = ((signal == System.IO.Ports.SerialSignal.Rts) ? ((!value) ? 4u : 3u) : ((!value) ? 6u : 5u));
		if (!EscapeCommFunction(handle, flags))
		{
			ReportIOError(null);
		}
	}

	public void SetBreakState(bool value)
	{
		if (!EscapeCommFunction(handle, (!value) ? 9u : 8u))
		{
			ReportIOError(null);
		}
	}
}

using System.Runtime.InteropServices;

namespace System.IO.Ports;

internal class SerialPortStream : Stream, IDisposable, System.IO.Ports.ISerialStream
{
	private int fd;

	private int read_timeout;

	private int write_timeout;

	private bool disposed;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

	public override bool CanTimeout => true;

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

	public int BytesToRead => get_bytes_in_buffer(fd, 1);

	public int BytesToWrite => get_bytes_in_buffer(fd, 0);

	public SerialPortStream(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits, bool dtrEnable, bool rtsEnable, Handshake handshake, int readTimeout, int writeTimeout, int readBufferSize, int writeBufferSize)
	{
		fd = open_serial(portName);
		if (fd == -1)
		{
			ThrowIOException();
		}
		if (!set_attributes(fd, baudRate, parity, dataBits, stopBits, handshake))
		{
			ThrowIOException();
		}
		read_timeout = readTimeout;
		write_timeout = writeTimeout;
		SetSignal(System.IO.Ports.SerialSignal.Dtr, dtrEnable);
		if (handshake != Handshake.RequestToSend && handshake != Handshake.RequestToSendXOnXOff)
		{
			SetSignal(System.IO.Ports.SerialSignal.Rts, rtsEnable);
		}
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern int open_serial(string portName);

	public override void Flush()
	{
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern int read_serial(int fd, byte[] buffer, int offset, int count);

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern bool poll_serial(int fd, out int error, int timeout);

	public override int Read([In][Out] byte[] buffer, int offset, int count)
	{
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
		int error;
		bool flag = poll_serial(fd, out error, read_timeout);
		if (error == -1)
		{
			ThrowIOException();
		}
		if (!flag)
		{
			throw new TimeoutException();
		}
		return read_serial(fd, buffer, offset, count);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern int write_serial(int fd, byte[] buffer, int offset, int count, int timeout);

	public override void Write(byte[] buffer, int offset, int count)
	{
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
		if (write_serial(fd, buffer, offset, count, write_timeout) < 0)
		{
			throw new TimeoutException("The operation has timed-out");
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			disposed = true;
			if (close_serial(fd) != 0)
			{
				ThrowIOException();
			}
		}
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern int close_serial(int fd);

	public override void Close()
	{
		((IDisposable)this).Dispose();
	}

	~SerialPortStream()
	{
		Dispose(disposing: false);
	}

	private void CheckDisposed()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern bool set_attributes(int fd, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake);

	public void SetAttributes(int baud_rate, Parity parity, int data_bits, StopBits sb, Handshake hs)
	{
		if (!set_attributes(fd, baud_rate, parity, data_bits, sb, hs))
		{
			ThrowIOException();
		}
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern int get_bytes_in_buffer(int fd, int input);

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern void discard_buffer(int fd, bool inputBuffer);

	public void DiscardInBuffer()
	{
		discard_buffer(fd, inputBuffer: true);
	}

	public void DiscardOutBuffer()
	{
		discard_buffer(fd, inputBuffer: false);
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern System.IO.Ports.SerialSignal get_signals(int fd, out int error);

	public System.IO.Ports.SerialSignal GetSignals()
	{
		int error;
		System.IO.Ports.SerialSignal result = get_signals(fd, out error);
		if (error == -1)
		{
			ThrowIOException();
		}
		return result;
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern int set_signal(int fd, System.IO.Ports.SerialSignal signal, bool value);

	public void SetSignal(System.IO.Ports.SerialSignal signal, bool value)
	{
		if (signal < System.IO.Ports.SerialSignal.Cd || signal > System.IO.Ports.SerialSignal.Rts || signal == System.IO.Ports.SerialSignal.Cd || signal == System.IO.Ports.SerialSignal.Cts || signal == System.IO.Ports.SerialSignal.Dsr)
		{
			throw new Exception("Invalid internal value");
		}
		if (set_signal(fd, signal, value) == -1)
		{
			ThrowIOException();
		}
	}

	[DllImport("MonoPosixHelper", SetLastError = true)]
	private static extern int breakprop(int fd);

	public void SetBreakState(bool value)
	{
		if (value)
		{
			breakprop(fd);
		}
	}

	[DllImport("libc")]
	private static extern IntPtr strerror(int errnum);

	private static void ThrowIOException()
	{
		int lastWin32Error = Marshal.GetLastWin32Error();
		string message = Marshal.PtrToStringAnsi(strerror(lastWin32Error));
		throw new IOException(message);
	}
}

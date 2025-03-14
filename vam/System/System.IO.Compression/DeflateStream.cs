using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.IO.Compression;

public class DeflateStream : Stream
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate int UnmanagedReadOrWrite(IntPtr buffer, int length, IntPtr data);

	private delegate int ReadMethod(byte[] array, int offset, int count);

	private delegate void WriteMethod(byte[] array, int offset, int count);

	private const int BufferSize = 4096;

	private const string LIBNAME = "MonoPosixHelper";

	private Stream base_stream;

	private CompressionMode mode;

	private bool leaveOpen;

	private bool disposed;

	private UnmanagedReadOrWrite feeder;

	private IntPtr z_stream;

	private byte[] io_buffer;

	private GCHandle data;

	public Stream BaseStream => base_stream;

	public override bool CanRead => !disposed && mode == CompressionMode.Decompress && base_stream.CanRead;

	public override bool CanSeek => false;

	public override bool CanWrite => !disposed && mode == CompressionMode.Compress && base_stream.CanWrite;

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

	public DeflateStream(Stream compressedStream, CompressionMode mode)
		: this(compressedStream, mode, leaveOpen: false, gzip: false)
	{
	}

	public DeflateStream(Stream compressedStream, CompressionMode mode, bool leaveOpen)
		: this(compressedStream, mode, leaveOpen, gzip: false)
	{
	}

	internal DeflateStream(Stream compressedStream, CompressionMode mode, bool leaveOpen, bool gzip)
	{
		if (compressedStream == null)
		{
			throw new ArgumentNullException("compressedStream");
		}
		if (mode != CompressionMode.Compress && mode != 0)
		{
			throw new ArgumentException("mode");
		}
		data = GCHandle.Alloc(this);
		base_stream = compressedStream;
		feeder = ((mode != CompressionMode.Compress) ? new UnmanagedReadOrWrite(UnmanagedRead) : new UnmanagedReadOrWrite(UnmanagedWrite));
		z_stream = CreateZStream(mode, gzip, feeder, GCHandle.ToIntPtr(data));
		if (z_stream == IntPtr.Zero)
		{
			base_stream = null;
			feeder = null;
			throw new NotImplementedException("Failed to initialize zlib. You probably have an old zlib installed. Version 1.2.0.4 or later is required.");
		}
		this.mode = mode;
		this.leaveOpen = leaveOpen;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && !disposed)
		{
			disposed = true;
			IntPtr intPtr = z_stream;
			z_stream = IntPtr.Zero;
			int result = 0;
			if (intPtr != IntPtr.Zero)
			{
				result = CloseZStream(intPtr);
			}
			io_buffer = null;
			if (!leaveOpen)
			{
				base_stream?.Close();
				base_stream = null;
			}
			CheckResult(result, "Dispose");
		}
		if (data.IsAllocated)
		{
			data.Free();
			data = default(GCHandle);
		}
		base.Dispose(disposing);
	}

	private static int UnmanagedRead(IntPtr buffer, int length, IntPtr data)
	{
		if (!(GCHandle.FromIntPtr(data).Target is DeflateStream deflateStream))
		{
			return -1;
		}
		return deflateStream.UnmanagedRead(buffer, length);
	}

	private unsafe int UnmanagedRead(IntPtr buffer, int length)
	{
		int num = 0;
		int num2 = 1;
		while (length > 0 && num2 > 0)
		{
			if (io_buffer == null)
			{
				io_buffer = new byte[4096];
			}
			int count = Math.Min(length, io_buffer.Length);
			num2 = base_stream.Read(io_buffer, 0, count);
			if (num2 > 0)
			{
				Marshal.Copy(io_buffer, 0, buffer, num2);
				buffer = new IntPtr((byte*)buffer.ToPointer() + num2);
				length -= num2;
				num += num2;
			}
		}
		return num;
	}

	private static int UnmanagedWrite(IntPtr buffer, int length, IntPtr data)
	{
		if (!(GCHandle.FromIntPtr(data).Target is DeflateStream deflateStream))
		{
			return -1;
		}
		return deflateStream.UnmanagedWrite(buffer, length);
	}

	private unsafe int UnmanagedWrite(IntPtr buffer, int length)
	{
		int num = 0;
		while (length > 0)
		{
			if (io_buffer == null)
			{
				io_buffer = new byte[4096];
			}
			int num2 = Math.Min(length, io_buffer.Length);
			Marshal.Copy(buffer, io_buffer, 0, num2);
			base_stream.Write(io_buffer, 0, num2);
			buffer = new IntPtr((byte*)buffer.ToPointer() + num2);
			length -= num2;
			num += num2;
		}
		return num;
	}

	private unsafe int ReadInternal(byte[] array, int offset, int count)
	{
		//IL_001f->IL0026: Incompatible stack types: I vs Ref
		if (count == 0)
		{
			return 0;
		}
		int num = 0;
		fixed (byte* ptr = &System.Runtime.CompilerServices.Unsafe.AsRef<byte>((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref array != null && array.Length != 0 ? ref System.Runtime.CompilerServices.Unsafe.As<byte, _003F>(ref array[0]) : ref *(_003F*)null)))
		{
			num = ReadZStream(buffer: new IntPtr(ptr + offset), stream: z_stream, length: count);
		}
		CheckResult(num, "ReadInternal");
		return num;
	}

	public override int Read(byte[] dest, int dest_offset, int count)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		if (dest == null)
		{
			throw new ArgumentNullException("Destination array is null.");
		}
		if (!CanRead)
		{
			throw new InvalidOperationException("Stream does not support reading.");
		}
		int num = dest.Length;
		if (dest_offset < 0 || count < 0)
		{
			throw new ArgumentException("Dest or count is negative.");
		}
		if (dest_offset > num)
		{
			throw new ArgumentException("destination offset is beyond array size");
		}
		if (dest_offset + count > num)
		{
			throw new ArgumentException("Reading would overrun buffer");
		}
		return ReadInternal(dest, dest_offset, count);
	}

	private unsafe void WriteInternal(byte[] array, int offset, int count)
	{
		//IL_001e->IL0025: Incompatible stack types: I vs Ref
		if (count != 0)
		{
			int num = 0;
			fixed (byte* ptr = &System.Runtime.CompilerServices.Unsafe.AsRef<byte>((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref array != null && array.Length != 0 ? ref System.Runtime.CompilerServices.Unsafe.As<byte, _003F>(ref array[0]) : ref *(_003F*)null)))
			{
				num = WriteZStream(buffer: new IntPtr(ptr + offset), stream: z_stream, length: count);
			}
			CheckResult(num, "WriteInternal");
		}
	}

	public override void Write(byte[] src, int src_offset, int count)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (src_offset < 0)
		{
			throw new ArgumentOutOfRangeException("src_offset");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (!CanWrite)
		{
			throw new NotSupportedException("Stream does not support writing");
		}
		WriteInternal(src, src_offset, count);
	}

	private static void CheckResult(int result, string where)
	{
		if (result >= 0)
		{
			return;
		}
		throw new IOException(result switch
		{
			-1 => "Unknown error", 
			-2 => "Internal error", 
			-3 => "Corrupted data", 
			-4 => "Not enough memory", 
			-5 => "Internal error (no progress possible)", 
			-6 => "Invalid version", 
			-10 => "Invalid argument(s)", 
			-11 => "IO error", 
			_ => "Unknown error", 
		} + " " + where);
	}

	public override void Flush()
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		if (CanWrite)
		{
			int result = Flush(z_stream);
			CheckResult(result, "Flush");
		}
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		if (!CanRead)
		{
			throw new NotSupportedException("This stream does not support reading");
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Must be >= 0");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
		}
		if (count + offset > buffer.Length)
		{
			throw new ArgumentException("Buffer too small. count/offset wrong.");
		}
		ReadMethod readMethod = ReadInternal;
		return readMethod.BeginInvoke(buffer, offset, count, cback, state);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(GetType().FullName);
		}
		if (!CanWrite)
		{
			throw new InvalidOperationException("This stream does not support writing");
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Must be >= 0");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
		}
		if (count + offset > buffer.Length)
		{
			throw new ArgumentException("Buffer too small. count/offset wrong.");
		}
		WriteMethod writeMethod = WriteInternal;
		return writeMethod.BeginInvoke(buffer, offset, count, cback, state);
	}

	public override int EndRead(IAsyncResult async_result)
	{
		if (async_result == null)
		{
			throw new ArgumentNullException("async_result");
		}
		if (!(async_result is AsyncResult asyncResult))
		{
			throw new ArgumentException("Invalid IAsyncResult", "async_result");
		}
		if (!(asyncResult.AsyncDelegate is ReadMethod readMethod))
		{
			throw new ArgumentException("Invalid IAsyncResult", "async_result");
		}
		return readMethod.EndInvoke(async_result);
	}

	public override void EndWrite(IAsyncResult async_result)
	{
		if (async_result == null)
		{
			throw new ArgumentNullException("async_result");
		}
		if (!(async_result is AsyncResult asyncResult))
		{
			throw new ArgumentException("Invalid IAsyncResult", "async_result");
		}
		if (!(asyncResult.AsyncDelegate is WriteMethod writeMethod))
		{
			throw new ArgumentException("Invalid IAsyncResult", "async_result");
		}
		writeMethod.EndInvoke(async_result);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr CreateZStream(CompressionMode compress, bool gzip, UnmanagedReadOrWrite feeder, IntPtr data);

	[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
	private static extern int CloseZStream(IntPtr stream);

	[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
	private static extern int Flush(IntPtr stream);

	[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
	private static extern int ReadZStream(IntPtr stream, IntPtr buffer, int length);

	[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl)]
	private static extern int WriteZStream(IntPtr stream, IntPtr buffer, int length);
}

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mono.Unix.Native;

namespace Mono.Unix;

public sealed class UnixStream : Stream, IDisposable
{
	public const int InvalidFileDescriptor = -1;

	public const int StandardInputFileDescriptor = 0;

	public const int StandardOutputFileDescriptor = 1;

	public const int StandardErrorFileDescriptor = 2;

	private bool canSeek;

	private bool canRead;

	private bool canWrite;

	private bool owner = true;

	private int fileDescriptor = -1;

	private Stat stat;

	public int Handle => fileDescriptor;

	public override bool CanRead => canRead;

	public override bool CanSeek => canSeek;

	public override bool CanWrite => canWrite;

	public override long Length
	{
		get
		{
			AssertNotDisposed();
			if (!CanSeek)
			{
				throw new NotSupportedException("File descriptor doesn't support seeking");
			}
			RefreshStat();
			return stat.st_size;
		}
	}

	public override long Position
	{
		get
		{
			AssertNotDisposed();
			if (!CanSeek)
			{
				throw new NotSupportedException("The stream does not support seeking");
			}
			long num = Syscall.lseek(fileDescriptor, 0L, SeekFlags.SEEK_CUR);
			if (num == -1)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}
		set
		{
			Seek(value, SeekOrigin.Begin);
		}
	}

	[CLSCompliant(false)]
	public FilePermissions Protection
	{
		get
		{
			RefreshStat();
			return stat.st_mode;
		}
		set
		{
			value &= ~FilePermissions.S_IFMT;
			int retval = Syscall.fchmod(fileDescriptor, value);
			UnixMarshal.ThrowExceptionForLastErrorIf(retval);
		}
	}

	public FileTypes FileType
	{
		get
		{
			int protection = (int)Protection;
			return (FileTypes)(protection & 0xF000);
		}
	}

	public FileAccessPermissions FileAccessPermissions
	{
		get
		{
			int protection = (int)Protection;
			return (FileAccessPermissions)(protection & 0x1FF);
		}
		set
		{
			int protection = (int)Protection;
			protection &= -512;
			protection |= (int)value;
			Protection = (FilePermissions)protection;
		}
	}

	public FileSpecialAttributes FileSpecialAttributes
	{
		get
		{
			int protection = (int)Protection;
			return (FileSpecialAttributes)(protection & 0xE00);
		}
		set
		{
			int protection = (int)Protection;
			protection &= -3585;
			protection |= (int)value;
			Protection = (FilePermissions)protection;
		}
	}

	public UnixUserInfo OwnerUser
	{
		get
		{
			RefreshStat();
			return new UnixUserInfo(stat.st_uid);
		}
	}

	public long OwnerUserId
	{
		get
		{
			RefreshStat();
			return stat.st_uid;
		}
	}

	public UnixGroupInfo OwnerGroup
	{
		get
		{
			RefreshStat();
			return new UnixGroupInfo(stat.st_gid);
		}
	}

	public long OwnerGroupId
	{
		get
		{
			RefreshStat();
			return stat.st_gid;
		}
	}

	public UnixStream(int fileDescriptor)
		: this(fileDescriptor, ownsHandle: true)
	{
	}

	public UnixStream(int fileDescriptor, bool ownsHandle)
	{
		if (fileDescriptor == -1)
		{
			throw new ArgumentException(global::Locale.GetText("Invalid file descriptor"), "fileDescriptor");
		}
		this.fileDescriptor = fileDescriptor;
		owner = ownsHandle;
		long num = Syscall.lseek(fileDescriptor, 0L, SeekFlags.SEEK_CUR);
		if (num != -1)
		{
			canSeek = true;
		}
		long num2 = Syscall.read(fileDescriptor, IntPtr.Zero, 0uL);
		if (num2 != -1)
		{
			canRead = true;
		}
		long num3 = Syscall.write(fileDescriptor, IntPtr.Zero, 0uL);
		if (num3 != -1)
		{
			canWrite = true;
		}
	}

	void IDisposable.Dispose()
	{
		AssertNotDisposed();
		if (owner)
		{
			Close();
		}
		GC.SuppressFinalize(this);
	}

	private void AssertNotDisposed()
	{
		if (fileDescriptor == -1)
		{
			throw new ObjectDisposedException("Invalid File Descriptor");
		}
	}

	private void RefreshStat()
	{
		AssertNotDisposed();
		int retval = Syscall.fstat(fileDescriptor, out stat);
		UnixMarshal.ThrowExceptionForLastErrorIf(retval);
	}

	public void AdviseFileAccessPattern(FileAccessPattern pattern, long offset, long len)
	{
		FileHandleOperations.AdviseFileAccessPattern(fileDescriptor, pattern, offset, len);
	}

	public void AdviseFileAccessPattern(FileAccessPattern pattern)
	{
		AdviseFileAccessPattern(pattern, 0L, 0L);
	}

	public override void Flush()
	{
	}

	public unsafe override int Read([In][Out] byte[] buffer, int offset, int count)
	{
		AssertNotDisposed();
		AssertValidBuffer(buffer, offset, count);
		if (!CanRead)
		{
			throw new NotSupportedException("Stream does not support reading");
		}
		if (buffer.Length == 0)
		{
			return 0;
		}
		long num = 0L;
		fixed (byte* buf = &System.Runtime.CompilerServices.Unsafe.AsRef<byte>((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref buffer[offset])))
		{
			do
			{
				num = Syscall.read(fileDescriptor, buf, (ulong)count);
			}
			while (UnixMarshal.ShouldRetrySyscall((int)num));
		}
		if (num == -1)
		{
			UnixMarshal.ThrowExceptionForLastError();
		}
		return (int)num;
	}

	private void AssertValidBuffer(byte[] buffer, int offset, int count)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset", "< 0");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "< 0");
		}
		if (offset > buffer.Length)
		{
			throw new ArgumentException("destination offset is beyond array size");
		}
		if (offset > buffer.Length - count)
		{
			throw new ArgumentException("would overrun buffer");
		}
	}

	public unsafe int ReadAtOffset([In][Out] byte[] buffer, int offset, int count, long fileOffset)
	{
		AssertNotDisposed();
		AssertValidBuffer(buffer, offset, count);
		if (!CanRead)
		{
			throw new NotSupportedException("Stream does not support reading");
		}
		if (buffer.Length == 0)
		{
			return 0;
		}
		long num = 0L;
		fixed (byte* buf = &System.Runtime.CompilerServices.Unsafe.AsRef<byte>((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref buffer[offset])))
		{
			do
			{
				num = Syscall.pread(fileDescriptor, buf, (ulong)count, fileOffset);
			}
			while (UnixMarshal.ShouldRetrySyscall((int)num));
		}
		if (num == -1)
		{
			UnixMarshal.ThrowExceptionForLastError();
		}
		return (int)num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		AssertNotDisposed();
		if (!CanSeek)
		{
			throw new NotSupportedException("The File Descriptor does not support seeking");
		}
		SeekFlags whence = SeekFlags.SEEK_CUR;
		switch (origin)
		{
		case SeekOrigin.Begin:
			whence = SeekFlags.SEEK_SET;
			break;
		case SeekOrigin.Current:
			whence = SeekFlags.SEEK_CUR;
			break;
		case SeekOrigin.End:
			whence = SeekFlags.SEEK_END;
			break;
		}
		long num = Syscall.lseek(fileDescriptor, offset, whence);
		if (num == -1)
		{
			UnixMarshal.ThrowExceptionForLastError();
		}
		return num;
	}

	public override void SetLength(long value)
	{
		AssertNotDisposed();
		if (value < 0)
		{
			throw new ArgumentOutOfRangeException("value", "< 0");
		}
		if (!CanSeek && !CanWrite)
		{
			throw new NotSupportedException("You can't truncating the current file descriptor");
		}
		int num;
		do
		{
			num = Syscall.ftruncate(fileDescriptor, value);
		}
		while (UnixMarshal.ShouldRetrySyscall(num));
		UnixMarshal.ThrowExceptionForLastErrorIf(num);
	}

	public unsafe override void Write(byte[] buffer, int offset, int count)
	{
		AssertNotDisposed();
		AssertValidBuffer(buffer, offset, count);
		if (!CanWrite)
		{
			throw new NotSupportedException("File Descriptor does not support writing");
		}
		if (buffer.Length == 0)
		{
			return;
		}
		long num = 0L;
		fixed (byte* buf = &System.Runtime.CompilerServices.Unsafe.AsRef<byte>((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref buffer[offset])))
		{
			do
			{
				num = Syscall.write(fileDescriptor, buf, (ulong)count);
			}
			while (UnixMarshal.ShouldRetrySyscall((int)num));
		}
		if (num == -1)
		{
			UnixMarshal.ThrowExceptionForLastError();
		}
	}

	public unsafe void WriteAtOffset(byte[] buffer, int offset, int count, long fileOffset)
	{
		AssertNotDisposed();
		AssertValidBuffer(buffer, offset, count);
		if (!CanWrite)
		{
			throw new NotSupportedException("File Descriptor does not support writing");
		}
		if (buffer.Length == 0)
		{
			return;
		}
		long num = 0L;
		fixed (byte* buf = &System.Runtime.CompilerServices.Unsafe.AsRef<byte>((byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref buffer[offset])))
		{
			do
			{
				num = Syscall.pwrite(fileDescriptor, buf, (ulong)count, fileOffset);
			}
			while (UnixMarshal.ShouldRetrySyscall((int)num));
		}
		if (num == -1)
		{
			UnixMarshal.ThrowExceptionForLastError();
		}
	}

	public void SendTo(UnixStream output)
	{
		SendTo(output, (ulong)output.Length);
	}

	[CLSCompliant(false)]
	public void SendTo(UnixStream output, ulong count)
	{
		SendTo(output.Handle, count);
	}

	[CLSCompliant(false)]
	public void SendTo(int out_fd, ulong count)
	{
		if (!CanWrite)
		{
			throw new NotSupportedException("Unable to write to the current file descriptor");
		}
		long offset = Position;
		long num = Syscall.sendfile(out_fd, fileDescriptor, ref offset, count);
		if (num == -1)
		{
			UnixMarshal.ThrowExceptionForLastError();
		}
	}

	public void SetOwner(long user, long group)
	{
		AssertNotDisposed();
		int retval = Syscall.fchown(fileDescriptor, Convert.ToUInt32(user), Convert.ToUInt32(group));
		UnixMarshal.ThrowExceptionForLastErrorIf(retval);
	}

	public void SetOwner(string user, string group)
	{
		AssertNotDisposed();
		long userId = new UnixUserInfo(user).UserId;
		long groupId = new UnixGroupInfo(group).GroupId;
		SetOwner(userId, groupId);
	}

	public void SetOwner(string user)
	{
		AssertNotDisposed();
		Passwd passwd = Syscall.getpwnam(user);
		if (passwd == null)
		{
			throw new ArgumentException(global::Locale.GetText("invalid username"), "user");
		}
		long user2 = passwd.pw_uid;
		long group = passwd.pw_gid;
		SetOwner(user2, group);
	}

	[CLSCompliant(false)]
	public long GetConfigurationValue(PathconfName name)
	{
		AssertNotDisposed();
		long num = Syscall.fpathconf(fileDescriptor, name);
		if (num == -1 && Stdlib.GetLastError() != 0)
		{
			UnixMarshal.ThrowExceptionForLastError();
		}
		return num;
	}

	~UnixStream()
	{
		Close();
	}

	public override void Close()
	{
		if (fileDescriptor == -1)
		{
			return;
		}
		Flush();
		if (owner)
		{
			int num;
			do
			{
				num = Syscall.close(fileDescriptor);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			fileDescriptor = -1;
			GC.SuppressFinalize(this);
		}
	}
}

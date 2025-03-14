namespace System.Net.FtpClient;

public class FtpDataStream : FtpSocketStream
{
	private FtpReply m_commandStatus;

	private FtpClient m_control;

	private long m_length;

	private long m_position;

	public FtpReply CommandStatus
	{
		get
		{
			return m_commandStatus;
		}
		set
		{
			m_commandStatus = value;
		}
	}

	public FtpClient ControlConnection
	{
		get
		{
			return m_control;
		}
		set
		{
			m_control = value;
		}
	}

	public override long Length => m_length;

	public override long Position
	{
		get
		{
			return m_position;
		}
		set
		{
			throw new InvalidOperationException("You cannot modify the position of a FtpDataStream. This property is updated as data is read or written to the stream.");
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = base.Read(buffer, offset, count);
		m_position += num;
		return num;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		base.Write(buffer, offset, count);
		m_position += count;
	}

	public override void SetLength(long value)
	{
		m_length = value;
	}

	public void SetPosition(long pos)
	{
		m_position = pos;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (base.IsConnected)
			{
				Close();
			}
			m_control = null;
		}
		base.Dispose(disposing);
	}

	public new FtpReply Close()
	{
		base.Close();
		try
		{
			if (ControlConnection != null)
			{
				return ControlConnection.CloseDataStream(this);
			}
		}
		finally
		{
			m_commandStatus = default(FtpReply);
			m_control = null;
		}
		return default(FtpReply);
	}

	public FtpDataStream(FtpClient conn)
	{
		if (conn == null)
		{
			throw new ArgumentException("The control connection cannot be null.");
		}
		ControlConnection = conn;
		base.ValidateCertificate += delegate(FtpSocketStream obj, FtpSslValidationEventArgs e)
		{
			e.Accept = true;
		};
		m_position = 0L;
	}

	~FtpDataStream()
	{
		try
		{
			Dispose();
		}
		catch (Exception ex)
		{
			FtpTrace.WriteLine("[Finalizer] Caught and discarded an exception while disposing the FtpDataStream: {0}", ex.ToString());
		}
	}
}

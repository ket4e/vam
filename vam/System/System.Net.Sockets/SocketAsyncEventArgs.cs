using System.Collections.Generic;
using System.Threading;

namespace System.Net.Sockets;

public class SocketAsyncEventArgs : EventArgs, IDisposable
{
	private IList<ArraySegment<byte>> _bufferList;

	private Socket curSocket;

	public Socket AcceptSocket { get; set; }

	public byte[] Buffer { get; private set; }

	[System.MonoTODO("not supported in all cases")]
	public IList<ArraySegment<byte>> BufferList
	{
		get
		{
			return _bufferList;
		}
		set
		{
			if (Buffer != null && value != null)
			{
				throw new ArgumentException("Buffer and BufferList properties cannot both be non-null.");
			}
			_bufferList = value;
		}
	}

	public int BytesTransferred { get; private set; }

	public int Count { get; private set; }

	public bool DisconnectReuseSocket { get; set; }

	public SocketAsyncOperation LastOperation { get; private set; }

	public int Offset { get; private set; }

	public EndPoint RemoteEndPoint { get; set; }

	public IPPacketInformation ReceiveMessageFromPacketInfo { get; private set; }

	public SendPacketsElement[] SendPacketsElements { get; set; }

	public TransmitFileOptions SendPacketsFlags { get; set; }

	[System.MonoTODO("unused property")]
	public int SendPacketsSendSize { get; set; }

	public SocketError SocketError { get; set; }

	public SocketFlags SocketFlags { get; set; }

	public object UserToken { get; set; }

	public event EventHandler<SocketAsyncEventArgs> Completed;

	public SocketAsyncEventArgs()
	{
		AcceptSocket = null;
		Buffer = null;
		BufferList = null;
		BytesTransferred = 0;
		Count = 0;
		DisconnectReuseSocket = false;
		LastOperation = SocketAsyncOperation.None;
		Offset = 0;
		RemoteEndPoint = null;
		SendPacketsElements = null;
		SendPacketsFlags = TransmitFileOptions.UseDefaultWorkerThread;
		SendPacketsSendSize = -1;
		SocketError = SocketError.Success;
		SocketFlags = SocketFlags.None;
		UserToken = null;
	}

	~SocketAsyncEventArgs()
	{
		Dispose(disposing: false);
	}

	private void Dispose(bool disposing)
	{
		AcceptSocket?.Close();
		if (disposing)
		{
			GC.SuppressFinalize(this);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void OnCompleted(SocketAsyncEventArgs e)
	{
		e?.Completed?.Invoke(e.curSocket, e);
	}

	public void SetBuffer(int offset, int count)
	{
		SetBufferInternal(Buffer, offset, count);
	}

	public void SetBuffer(byte[] buffer, int offset, int count)
	{
		SetBufferInternal(buffer, offset, count);
	}

	private void SetBufferInternal(byte[] buffer, int offset, int count)
	{
		if (buffer != null)
		{
			if (BufferList != null)
			{
				throw new ArgumentException("Buffer and BufferList properties cannot both be non-null.");
			}
			int num = buffer.Length;
			if (offset < 0 || (offset != 0 && offset >= num))
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > num - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			Count = count;
			Offset = offset;
		}
		Buffer = buffer;
	}

	private void ReceiveCallback()
	{
		SocketError = SocketError.Success;
		LastOperation = SocketAsyncOperation.Receive;
		SocketError error = SocketError.Success;
		if (!curSocket.Connected)
		{
			SocketError = SocketError.NotConnected;
			return;
		}
		try
		{
			BytesTransferred = curSocket.Receive_nochecks(Buffer, Offset, Count, SocketFlags, out error);
		}
		finally
		{
			SocketError = error;
			OnCompleted(this);
		}
	}

	private void ConnectCallback()
	{
		LastOperation = SocketAsyncOperation.Connect;
		SocketError socketError = SocketError.AccessDenied;
		try
		{
			socketError = TryConnect(RemoteEndPoint);
		}
		finally
		{
			SocketError = socketError;
			OnCompleted(this);
		}
	}

	private SocketError TryConnect(EndPoint endpoint)
	{
		curSocket.Connected = false;
		SocketError result = SocketError.Success;
		try
		{
			if (!curSocket.Blocking)
			{
				curSocket.Poll(-1, SelectMode.SelectWrite, out var socket_error);
				result = (SocketError)socket_error;
				if (socket_error != 0)
				{
					return result;
				}
				curSocket.Connected = true;
			}
			else
			{
				curSocket.seed_endpoint = endpoint;
				curSocket.Connect(endpoint);
				curSocket.Connected = true;
			}
		}
		catch (SocketException ex)
		{
			result = ex.SocketErrorCode;
		}
		return result;
	}

	private void SendCallback()
	{
		SocketError = SocketError.Success;
		LastOperation = SocketAsyncOperation.Send;
		SocketError error = SocketError.Success;
		if (!curSocket.Connected)
		{
			SocketError = SocketError.NotConnected;
			return;
		}
		try
		{
			if (Buffer != null)
			{
				BytesTransferred = curSocket.Send_nochecks(Buffer, Offset, Count, SocketFlags.None, out error);
			}
			else
			{
				if (BufferList == null)
				{
					return;
				}
				BytesTransferred = 0;
				{
					foreach (ArraySegment<byte> buffer in BufferList)
					{
						BytesTransferred += curSocket.Send_nochecks(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, out error);
						if (error != 0)
						{
							break;
						}
					}
					return;
				}
			}
		}
		finally
		{
			SocketError = error;
			OnCompleted(this);
		}
	}

	private void AcceptCallback()
	{
		SocketError = SocketError.Success;
		LastOperation = SocketAsyncOperation.Accept;
		try
		{
			curSocket.Accept(AcceptSocket);
		}
		catch (SocketException ex)
		{
			SocketError = ex.SocketErrorCode;
			throw;
		}
		finally
		{
			OnCompleted(this);
		}
	}

	private void DisconnectCallback()
	{
		SocketError = SocketError.Success;
		LastOperation = SocketAsyncOperation.Disconnect;
		try
		{
			curSocket.Disconnect(DisconnectReuseSocket);
		}
		catch (SocketException ex)
		{
			SocketError = ex.SocketErrorCode;
			throw;
		}
		finally
		{
			OnCompleted(this);
		}
	}

	private void ReceiveFromCallback()
	{
		SocketError = SocketError.Success;
		LastOperation = SocketAsyncOperation.ReceiveFrom;
		try
		{
			EndPoint remote_end = RemoteEndPoint;
			if (Buffer != null)
			{
				BytesTransferred = curSocket.ReceiveFrom_nochecks(Buffer, Offset, Count, SocketFlags, ref remote_end);
			}
			else if (BufferList != null)
			{
				throw new NotImplementedException();
			}
		}
		catch (SocketException ex)
		{
			SocketError = ex.SocketErrorCode;
			throw;
		}
		finally
		{
			OnCompleted(this);
		}
	}

	private void SendToCallback()
	{
		SocketError = SocketError.Success;
		LastOperation = SocketAsyncOperation.SendTo;
		int i = 0;
		try
		{
			for (int count = Count; i < count; i += curSocket.SendTo_nochecks(Buffer, Offset, count, SocketFlags, RemoteEndPoint))
			{
			}
			BytesTransferred = i;
		}
		catch (SocketException ex)
		{
			SocketError = ex.SocketErrorCode;
			throw;
		}
		finally
		{
			OnCompleted(this);
		}
	}

	internal void DoOperation(SocketAsyncOperation operation, Socket socket)
	{
		curSocket = socket;
		Thread thread = new Thread(operation switch
		{
			SocketAsyncOperation.Accept => AcceptCallback, 
			SocketAsyncOperation.Disconnect => DisconnectCallback, 
			SocketAsyncOperation.ReceiveFrom => ReceiveFromCallback, 
			SocketAsyncOperation.SendTo => SendToCallback, 
			SocketAsyncOperation.Receive => ReceiveCallback, 
			SocketAsyncOperation.Connect => ConnectCallback, 
			SocketAsyncOperation.Send => SendCallback, 
			_ => throw new NotSupportedException(), 
		});
		thread.IsBackground = true;
		thread.Start();
	}
}

using System;
using System.ComponentModel;
using System.Security;
using System.Threading;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Tags;

namespace Un4seen.Bass.Misc;

[SuppressUnmanagedCodeSecurity]
public sealed class BroadCast
{
	public enum BROADCASTSTATUS
	{
		NotConnected,
		Connected,
		Unknown
	}

	private IStreamingServer _server;

	private ENCODEPROC _autoEncProc;

	private ENCODENOTIFYPROC _myNotifyProc;

	private BROADCASTSTATUS _status;

	private bool _automaticMode;

	private bool _autoReconnect;

	private bool _isStarted;

	private bool _notificationSuppressDataSend;

	private bool _notificationSuppressIsAlive;

	private TimeSpan _reconnectTimeout = TimeSpan.FromSeconds(5.0);

	private TimerCallback _retryTimerDelegate;

	private Timer _retryTimer;

	private long _bytesSend;

	private DateTime _startTime = DateTime.Now;

	private DateTime _lastDisconnect = DateTime.Now;

	private DateTime _lastReconnectTry = DateTime.MinValue;

	private volatile object _lock = false;

	private volatile bool _autoReconnectTryRunning;

	private volatile bool _killAutoReconnectTry;

	public IStreamingServer Server => _server;

	public bool IsConnected => Server.IsConnected;

	public bool IsStarted => _isStarted;

	public BROADCASTSTATUS Status => _status;

	public bool AutomaticMode => _automaticMode;

	public bool AutoReconnect
	{
		get
		{
			return _autoReconnect;
		}
		set
		{
			_autoReconnect = value;
		}
	}

	public int ReconnectTimeout
	{
		get
		{
			return (int)_reconnectTimeout.TotalSeconds;
		}
		set
		{
			if (value < 1)
			{
				_reconnectTimeout = TimeSpan.FromSeconds(1.0);
			}
			else if (value > 86400)
			{
				_reconnectTimeout = TimeSpan.FromSeconds(86400.0);
			}
			else
			{
				_reconnectTimeout = TimeSpan.FromSeconds(value);
			}
			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_ENCODE_CAST_TIMEOUT, (int)_reconnectTimeout.TotalMilliseconds);
			if (!Server.UseBASS && _retryTimer != null)
			{
				_retryTimer.Change(_reconnectTimeout, _reconnectTimeout);
			}
		}
	}

	public bool NotificationSuppressDataSend
	{
		get
		{
			return _notificationSuppressDataSend;
		}
		set
		{
			_notificationSuppressDataSend = value;
		}
	}

	public bool NotificationSuppressIsAlive
	{
		get
		{
			return _notificationSuppressIsAlive;
		}
		set
		{
			_notificationSuppressIsAlive = value;
		}
	}

	public long TotalBytesSend
	{
		get
		{
			if (Server is WMAcast)
			{
				return ((EncoderWMA)((WMAcast)Server).Encoder).ByteSend;
			}
			if (Server.UseBASS)
			{
				return BassEnc.BASS_Encode_GetCount(Server.Encoder.EncoderHandle, BASSEncodeCount.BASS_ENCODE_COUNT_CAST);
			}
			return _bytesSend;
		}
	}

	public TimeSpan TotalConnectionTime => DateTime.Now - _startTime;

	public event BroadCastEventHandler Notification;

	private BroadCast()
	{
	}

	public BroadCast(IStreamingServer server)
	{
		_server = server;
		_autoEncProc = AutoEncodingCallback;
		_myNotifyProc = EncoderNotifyProc;
		_retryTimerDelegate = CheckStatus;
	}

	public bool AutoConnect()
	{
		return DoAutoConnect(startTimer: true);
	}

	public bool Connect()
	{
		return DoConnect(startTimer: true);
	}

	public bool Disconnect()
	{
		return Disconnect(calledFromReconnectTry: false);
	}

	public bool StartEncoder(ENCODEPROC proc, IntPtr user, bool paused)
	{
		bool result = true;
		if (Server.Encoder.Start(proc, user, paused))
		{
			RaiseNotification(BroadCastEventType.EncoderStarted, Server.Encoder);
		}
		else
		{
			Server.LastError = StreamingServer.STREAMINGERROR.Error_EncoderError;
			Server.LastErrorMessage = Enum.GetName(typeof(BASSError), Bass.BASS_ErrorGetCode());
			switch (Bass.BASS_ErrorGetCode())
			{
			case BASSError.BASS_ERROR_FILEOPEN:
				if (Server.Encoder is EncoderWMA)
				{
					Server.LastError = StreamingServer.STREAMINGERROR.Error_CreatingConnection;
					Server.LastErrorMessage = "Couldn't connect to the server. Check URL!";
				}
				else
				{
					Server.LastError = StreamingServer.STREAMINGERROR.Error_EncoderError;
					Server.LastErrorMessage = "Couldn't start the encoder. Check that executable exists!";
				}
				break;
			case BASSError.BASS_ERROR_NOTAVAIL:
				Server.LastError = StreamingServer.STREAMINGERROR.Error_EncoderError;
				Server.LastErrorMessage = "Encoder codec missing!";
				break;
			case BASSError.BASS_ERROR_WMA_WM9:
			case BASSError.BASS_ERROR_WMA_CODEC:
				Server.LastError = StreamingServer.STREAMINGERROR.Error_EncoderError;
				Server.LastErrorMessage = "WMA codec missing or WMA9 required!";
				break;
			case BASSError.BASS_ERROR_WMA_DENIED:
				Server.LastError = StreamingServer.STREAMINGERROR.Error_Login;
				Server.LastErrorMessage = "Access denied. Check username/password!";
				break;
			case BASSError.BASS_ERROR_ILLPARAM:
				Server.LastError = StreamingServer.STREAMINGERROR.Error_EncoderError;
				Server.LastErrorMessage = "Illegal parameters used to start encoder!";
				break;
			default:
				Server.LastError = StreamingServer.STREAMINGERROR.Error_EncoderError;
				Server.LastErrorMessage = "Some mystery problem occurred when trying to start the encoder!";
				break;
			}
			result = false;
			RaiseNotification(BroadCastEventType.EncoderStartError, Server.Encoder);
		}
		return result;
	}

	public bool StopEncoder()
	{
		Server.Encoder.Stop();
		if (Server.Encoder.IsActive)
		{
			RaiseNotification(BroadCastEventType.EncoderStopError, Server.Encoder);
			return false;
		}
		RaiseNotification(BroadCastEventType.EncoderStopped, Server.Encoder);
		return true;
	}

	public bool SendData(IntPtr buffer, int length)
	{
		if (buffer == IntPtr.Zero || length == 0 || Server.UseBASS)
		{
			return true;
		}
		if (!IsConnected || Server == null)
		{
			_status = BROADCASTSTATUS.NotConnected;
			return false;
		}
		int num = -1;
		lock (_lock)
		{
			try
			{
				num = Server.SendData(buffer, length);
				if (num > 0)
				{
					IncrementBytesSend(num);
				}
				if (num < 0)
				{
					RaiseNotification(BroadCastEventType.ConnectionLost, DateTime.Now);
				}
				else if (num != length)
				{
					RaiseNotification(BroadCastEventType.LessDataSend, length - num);
				}
				else if (!NotificationSuppressDataSend)
				{
					RaiseNotification(BroadCastEventType.DataSend, num);
				}
			}
			catch (Exception ex)
			{
				Server.LastError = StreamingServer.STREAMINGERROR.Error_SendingData;
				Server.LastErrorMessage = ex.Message;
				_status = BROADCASTSTATUS.NotConnected;
				Server.Disconnect();
				RaiseNotification(BroadCastEventType.ConnectionLost, DateTime.Now);
				return false;
			}
		}
		return num == length;
	}

	public bool UpdateTitle(string song, string url)
	{
		bool num = Server.UpdateTitle(song, url);
		if (num)
		{
			RaiseNotification(BroadCastEventType.TitleUpdated, song);
			return num;
		}
		RaiseNotification(BroadCastEventType.TitleUpdateError, song);
		return num;
	}

	public bool UpdateTitle(TAG_INFO tag, string url)
	{
		bool num = Server.UpdateTitle(tag, url);
		if (num)
		{
			RaiseNotification(BroadCastEventType.TitleUpdated, tag);
			return num;
		}
		RaiseNotification(BroadCastEventType.TitleUpdateError, tag);
		return num;
	}

	public int GetListeners(string password)
	{
		return Server.GetListeners(password);
	}

	public string GetStats(string password)
	{
		return Server.GetStats(password);
	}

	private bool DoAutoConnect(bool startTimer)
	{
		Server.LastError = StreamingServer.STREAMINGERROR.Ok;
		Server.LastErrorMessage = string.Empty;
		_automaticMode = true;
		bool flag = false;
		lock (_lock)
		{
			flag = !IsConnected || Disconnect();
			if (flag)
			{
				flag = ((!Server.UseBASS) ? StartEncoder(_autoEncProc, IntPtr.Zero, paused: true) : StartEncoder(null, IntPtr.Zero, paused: true));
				if (flag)
				{
					flag = InternalConnect(startTimer);
					if (!flag)
					{
						StopEncoder();
						_status = BROADCASTSTATUS.NotConnected;
					}
					else
					{
						Server.Encoder.Pause(paused: false);
					}
				}
				else
				{
					_status = BROADCASTSTATUS.NotConnected;
				}
			}
			else
			{
				_status = BROADCASTSTATUS.NotConnected;
			}
			if (AutoReconnect)
			{
				if (_retryTimer != null)
				{
					_retryTimer.Change(-1, -1);
					_retryTimer.Dispose();
					_retryTimer = null;
				}
				if (flag && Server.UseBASS)
				{
					if (Server is WMAcast)
					{
						((EncoderWMA)Server.Encoder).WMA_Notify = _myNotifyProc;
					}
					else
					{
						Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_ENCODE_CAST_TIMEOUT, (int)_reconnectTimeout.TotalMilliseconds);
						BassEnc.BASS_Encode_SetNotify(Server.Encoder.EncoderHandle, _myNotifyProc, IntPtr.Zero);
					}
				}
				else if (startTimer)
				{
					_retryTimer = new Timer(_retryTimerDelegate, null, _reconnectTimeout, _reconnectTimeout);
				}
			}
		}
		_isStarted = flag || AutoReconnect;
		return flag;
	}

	private bool DoConnect(bool startTimer)
	{
		Server.LastError = StreamingServer.STREAMINGERROR.Ok;
		Server.LastErrorMessage = string.Empty;
		_automaticMode = false;
		bool flag = false;
		lock (_lock)
		{
			flag = !IsConnected || Disconnect();
			if (flag)
			{
				flag = InternalConnect(startTimer);
				if (!flag)
				{
					StopEncoder();
					_status = BROADCASTSTATUS.NotConnected;
				}
			}
			else
			{
				_status = BROADCASTSTATUS.NotConnected;
			}
			if (AutoReconnect)
			{
				if (_retryTimer != null)
				{
					_retryTimer.Change(-1, -1);
					_retryTimer.Dispose();
					_retryTimer = null;
				}
				if (flag && Server.UseBASS)
				{
					if (Server is WMAcast)
					{
						((EncoderWMA)Server.Encoder).WMA_Notify = _myNotifyProc;
					}
					else
					{
						Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_ENCODE_CAST_TIMEOUT, (int)_reconnectTimeout.TotalMilliseconds);
						BassEnc.BASS_Encode_SetNotify(Server.Encoder.EncoderHandle, _myNotifyProc, IntPtr.Zero);
					}
				}
				else if (startTimer)
				{
					_retryTimer = new Timer(_retryTimerDelegate, null, _reconnectTimeout, _reconnectTimeout);
				}
			}
		}
		_isStarted = flag;
		return flag;
	}

	private bool InternalConnect(bool initial)
	{
		bool flag = false;
		if (Server.Connect())
		{
			if (Server.Login())
			{
				_startTime = DateTime.Now;
				_bytesSend = 0L;
				flag = true;
				Server.UpdateTitle(Server.SongTitle, Server.SongUrl);
				if (initial)
				{
					RaiseNotification(BroadCastEventType.Connected, _startTime);
				}
				else
				{
					RaiseNotification(BroadCastEventType.Reconnected, _startTime);
				}
			}
			else
			{
				RaiseNotification(BroadCastEventType.ConnectionError, DateTime.Now);
			}
		}
		else
		{
			RaiseNotification(BroadCastEventType.ConnectionError, DateTime.Now);
		}
		if (flag)
		{
			_status = BROADCASTSTATUS.Connected;
		}
		else
		{
			_status = BROADCASTSTATUS.NotConnected;
		}
		return flag;
	}

	private bool Disconnect(bool calledFromReconnectTry)
	{
		if (!calledFromReconnectTry)
		{
			Server.LastError = StreamingServer.STREAMINGERROR.Ok;
			Server.LastErrorMessage = string.Empty;
		}
		_isStarted = calledFromReconnectTry;
		bool flag = false;
		lock (_lock)
		{
			flag = InternalDisconnect();
			if (flag)
			{
				if (!calledFromReconnectTry)
				{
					_killAutoReconnectTry = true;
					if (_autoReconnectTryRunning)
					{
						while (_autoReconnectTryRunning)
						{
							Thread.Sleep(3);
						}
					}
				}
				if (_retryTimer != null)
				{
					_retryTimer.Change(-1, -1);
					_retryTimer.Dispose();
					_retryTimer = null;
				}
				if (Server.UseBASS)
				{
					if (Server is WMAcast)
					{
						((EncoderWMA)Server.Encoder).WMA_Notify = null;
					}
					else
					{
						BassEnc.BASS_Encode_SetNotify(Server.Encoder.EncoderHandle, null, IntPtr.Zero);
					}
				}
			}
		}
		return flag;
	}

	private bool InternalDisconnect()
	{
		bool result = false;
		if (!StopEncoder())
		{
			RaiseNotification(BroadCastEventType.DisconnectError, DateTime.Now);
			return false;
		}
		_status = BROADCASTSTATUS.Unknown;
		if (Server.Disconnect())
		{
			_status = BROADCASTSTATUS.NotConnected;
			_startTime = DateTime.Now;
			_bytesSend = 0L;
			_lastDisconnect = DateTime.Now;
			result = true;
			RaiseNotification(BroadCastEventType.Disconnected, _startTime);
		}
		else
		{
			RaiseNotification(BroadCastEventType.DisconnectError, DateTime.Now);
		}
		return result;
	}

	private void EncoderNotifyProc(int handle, BASSEncodeNotify status, IntPtr user)
	{
		switch (status)
		{
		case BASSEncodeNotify.BASS_ENCODE_NOTIFY_CAST_TIMEOUT:
			Server.LastError = StreamingServer.STREAMINGERROR.Error_SendingData;
			Server.LastErrorMessage = "Data sending timeout!";
			break;
		case BASSEncodeNotify.BASS_ENCODE_NOTIFY_ENCODER:
			Server.LastError = StreamingServer.STREAMINGERROR.Error_EncoderError;
			Server.LastErrorMessage = "Encoder died!";
			break;
		case BASSEncodeNotify.BASS_ENCODE_NOTIFY_CAST:
			Server.LastError = StreamingServer.STREAMINGERROR.Error_NotConnected;
			Server.LastErrorMessage = "Connection to the server died!";
			break;
		case BASSEncodeNotify.BASS_ENCODE_NOTIFY_FREE:
			Server.LastError = StreamingServer.STREAMINGERROR.Ok;
			Server.LastErrorMessage = string.Empty;
			return;
		case BASSEncodeNotify.BASS_ENCODE_NOTIFY_QUEUE_FULL:
			Server.LastError = StreamingServer.STREAMINGERROR.Warning_LessDataSend;
			Server.LastErrorMessage = "Encoding queue is out of space (some data could not be send to the server)!";
			return;
		default:
			Server.LastError = StreamingServer.STREAMINGERROR.Unknown;
			Server.LastErrorMessage = "Unknown encoder status";
			return;
		}
		RaiseNotification(BroadCastEventType.ConnectionLost, DateTime.Now);
		if (_retryTimer != null)
		{
			_retryTimer.Change(-1, -1);
			_retryTimer.Dispose();
			_retryTimer = null;
		}
		if (!_autoReconnectTryRunning)
		{
			ThreadPool.QueueUserWorkItem(AutoReconnectTry);
		}
	}

	private void AutoEncodingCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (IsConnected)
		{
			SendData(buffer, length);
		}
	}

	private void CheckStatus(object state)
	{
		if ((bool)_lock)
		{
			return;
		}
		_lock = true;
		if (IsConnected && Server.Encoder.IsActive)
		{
			if (!NotificationSuppressIsAlive)
			{
				RaiseNotification(BroadCastEventType.IsAlive, DateTime.Now);
			}
			_lock = false;
			return;
		}
		if (_retryTimer != null)
		{
			_retryTimer.Change(-1, -1);
			_retryTimer.Dispose();
			_retryTimer = null;
		}
		if (!_autoReconnectTryRunning)
		{
			ThreadPool.QueueUserWorkItem(AutoReconnectTry);
		}
		_lock = false;
	}

	private void AutoReconnectTry(object state)
	{
		Disconnect(calledFromReconnectTry: true);
		_autoReconnectTryRunning = true;
		_killAutoReconnectTry = false;
		if (AutoReconnect)
		{
			Thread.Sleep(_reconnectTimeout);
		}
		if (_killAutoReconnectTry)
		{
			_autoReconnectTryRunning = false;
			return;
		}
		bool flag;
		do
		{
			flag = true;
			if (!AutoReconnect || _killAutoReconnectTry)
			{
				continue;
			}
			_lastReconnectTry = DateTime.Now;
			RaiseNotification(BroadCastEventType.ReconnectTry, _lastReconnectTry);
			flag = ((!AutomaticMode) ? DoConnect(startTimer: false) : DoAutoConnect(startTimer: false));
			if (!flag)
			{
				RaiseNotification(BroadCastEventType.UnsuccessfulReconnectTry, _lastReconnectTry);
				if (!AutomaticMode)
				{
					RaiseNotification(BroadCastEventType.EncoderRestartRequired, Server.Encoder);
				}
				if (!_killAutoReconnectTry)
				{
					Thread.Sleep(_reconnectTimeout);
				}
			}
		}
		while (!flag);
		_autoReconnectTryRunning = false;
	}

	private void RaiseNotification(BroadCastEventType pEventType, object pData)
	{
		if (this.Notification != null)
		{
			ProcessDelegate(this.Notification, this, new BroadCastEventArgs(pEventType, pData));
		}
	}

	private void ProcessDelegate(Delegate del, params object[] args)
	{
		if ((object)del != null)
		{
			Delegate[] invocationList = del.GetInvocationList();
			foreach (Delegate del2 in invocationList)
			{
				InvokeDelegate(del2, args);
			}
		}
	}

	private void InvokeDelegate(Delegate del, object[] args)
	{
		if (del.Target is ISynchronizeInvoke synchronizeInvoke)
		{
			if (synchronizeInvoke.InvokeRequired)
			{
				try
				{
					synchronizeInvoke.BeginInvoke(del, args);
					return;
				}
				catch
				{
					return;
				}
			}
			del.DynamicInvoke(args);
		}
		else
		{
			del.DynamicInvoke(args);
		}
	}

	private void IncrementBytesSend(int sendData)
	{
		try
		{
			_bytesSend += sendData;
		}
		catch
		{
			_bytesSend = sendData;
		}
	}
}

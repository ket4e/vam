using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Microsoft.Win32;

namespace System.IO.Ports;

[MonitoringDescription("")]
public class SerialPort : Component
{
	public const int InfiniteTimeout = -1;

	private const int DefaultReadBufferSize = 4096;

	private const int DefaultWriteBufferSize = 2048;

	private const int DefaultBaudRate = 9600;

	private const int DefaultDataBits = 8;

	private const Parity DefaultParity = Parity.None;

	private const StopBits DefaultStopBits = StopBits.One;

	private bool is_open;

	private int baud_rate;

	private Parity parity;

	private StopBits stop_bits;

	private Handshake handshake;

	private int data_bits;

	private bool break_state;

	private bool dtr_enable;

	private bool rts_enable;

	private System.IO.Ports.ISerialStream stream;

	private Encoding encoding = Encoding.ASCII;

	private string new_line = Environment.NewLine;

	private string port_name;

	private int read_timeout = -1;

	private int write_timeout = -1;

	private int readBufferSize = 4096;

	private int writeBufferSize = 2048;

	private object error_received = new object();

	private object data_received = new object();

	private object pin_changed = new object();

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public Stream BaseStream
	{
		get
		{
			CheckOpen();
			return (Stream)stream;
		}
	}

	[Browsable(true)]
	[MonitoringDescription("")]
	[DefaultValue(9600)]
	public int BaudRate
	{
		get
		{
			return baud_rate;
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (is_open)
			{
				stream.SetAttributes(value, parity, data_bits, stop_bits, handshake);
			}
			baud_rate = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool BreakState
	{
		get
		{
			return break_state;
		}
		set
		{
			CheckOpen();
			if (value != break_state)
			{
				stream.SetBreakState(value);
				break_state = value;
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int BytesToRead
	{
		get
		{
			CheckOpen();
			return stream.BytesToRead;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int BytesToWrite
	{
		get
		{
			CheckOpen();
			return stream.BytesToWrite;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool CDHolding
	{
		get
		{
			CheckOpen();
			return (stream.GetSignals() & System.IO.Ports.SerialSignal.Cd) != 0;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CtsHolding
	{
		get
		{
			CheckOpen();
			return (stream.GetSignals() & System.IO.Ports.SerialSignal.Cts) != 0;
		}
	}

	[DefaultValue(8)]
	[MonitoringDescription("")]
	[Browsable(true)]
	public int DataBits
	{
		get
		{
			return data_bits;
		}
		set
		{
			if (value < 5 || value > 8)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (is_open)
			{
				stream.SetAttributes(baud_rate, parity, value, stop_bits, handshake);
			}
			data_bits = value;
		}
	}

	[MonitoringDescription("")]
	[Browsable(true)]
	[System.MonoTODO("Not implemented")]
	[DefaultValue(false)]
	public bool DiscardNull
	{
		get
		{
			CheckOpen();
			throw new NotImplementedException();
		}
		set
		{
			CheckOpen();
			throw new NotImplementedException();
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool DsrHolding
	{
		get
		{
			CheckOpen();
			return (stream.GetSignals() & System.IO.Ports.SerialSignal.Dsr) != 0;
		}
	}

	[Browsable(true)]
	[DefaultValue(false)]
	[MonitoringDescription("")]
	public bool DtrEnable
	{
		get
		{
			return dtr_enable;
		}
		set
		{
			if (value != dtr_enable)
			{
				if (is_open)
				{
					stream.SetSignal(System.IO.Ports.SerialSignal.Dtr, value);
				}
				dtr_enable = value;
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[MonitoringDescription("")]
	public Encoding Encoding
	{
		get
		{
			return encoding;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			encoding = value;
		}
	}

	[DefaultValue(Handshake.None)]
	[MonitoringDescription("")]
	[Browsable(true)]
	public Handshake Handshake
	{
		get
		{
			return handshake;
		}
		set
		{
			if (value < Handshake.None || value > Handshake.RequestToSendXOnXOff)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (is_open)
			{
				stream.SetAttributes(baud_rate, parity, data_bits, stop_bits, value);
			}
			handshake = value;
		}
	}

	[Browsable(false)]
	public bool IsOpen => is_open;

	[MonitoringDescription("")]
	[Browsable(false)]
	[DefaultValue("\n")]
	public string NewLine
	{
		get
		{
			return new_line;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0)
			{
				throw new ArgumentException("NewLine cannot be null or empty.", "value");
			}
			new_line = value;
		}
	}

	[DefaultValue(Parity.None)]
	[MonitoringDescription("")]
	[Browsable(true)]
	public Parity Parity
	{
		get
		{
			return parity;
		}
		set
		{
			if (value < Parity.None || value > Parity.Space)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (is_open)
			{
				stream.SetAttributes(baud_rate, value, data_bits, stop_bits, handshake);
			}
			parity = value;
		}
	}

	[System.MonoTODO("Not implemented")]
	[DefaultValue(63)]
	[MonitoringDescription("")]
	[Browsable(true)]
	public byte ParityReplace
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	[DefaultValue("COM1")]
	[Browsable(true)]
	[MonitoringDescription("")]
	public string PortName
	{
		get
		{
			return port_name;
		}
		set
		{
			if (is_open)
			{
				throw new InvalidOperationException("Port name cannot be set while port is open.");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0 || value.StartsWith("\\\\"))
			{
				throw new ArgumentException("value");
			}
			port_name = value;
		}
	}

	[MonitoringDescription("")]
	[DefaultValue(4096)]
	[Browsable(true)]
	public int ReadBufferSize
	{
		get
		{
			return readBufferSize;
		}
		set
		{
			if (is_open)
			{
				throw new InvalidOperationException();
			}
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (value > 4096)
			{
				readBufferSize = value;
			}
		}
	}

	[Browsable(true)]
	[DefaultValue(-1)]
	[MonitoringDescription("")]
	public int ReadTimeout
	{
		get
		{
			return read_timeout;
		}
		set
		{
			if (value <= 0 && value != -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (is_open)
			{
				stream.ReadTimeout = value;
			}
			read_timeout = value;
		}
	}

	[System.MonoTODO("Not implemented")]
	[MonitoringDescription("")]
	[DefaultValue(1)]
	[Browsable(true)]
	public int ReceivedBytesThreshold
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			throw new NotImplementedException();
		}
	}

	[Browsable(true)]
	[DefaultValue(false)]
	[MonitoringDescription("")]
	public bool RtsEnable
	{
		get
		{
			return rts_enable;
		}
		set
		{
			if (value != rts_enable)
			{
				if (is_open)
				{
					stream.SetSignal(System.IO.Ports.SerialSignal.Rts, value);
				}
				rts_enable = value;
			}
		}
	}

	[DefaultValue(StopBits.One)]
	[Browsable(true)]
	[MonitoringDescription("")]
	public StopBits StopBits
	{
		get
		{
			return stop_bits;
		}
		set
		{
			if (value < StopBits.One || value > StopBits.OnePointFive)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (is_open)
			{
				stream.SetAttributes(baud_rate, parity, data_bits, value, handshake);
			}
			stop_bits = value;
		}
	}

	[MonitoringDescription("")]
	[DefaultValue(2048)]
	[Browsable(true)]
	public int WriteBufferSize
	{
		get
		{
			return writeBufferSize;
		}
		set
		{
			if (is_open)
			{
				throw new InvalidOperationException();
			}
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (value > 2048)
			{
				writeBufferSize = value;
			}
		}
	}

	[Browsable(true)]
	[DefaultValue(-1)]
	[MonitoringDescription("")]
	public int WriteTimeout
	{
		get
		{
			return write_timeout;
		}
		set
		{
			if (value <= 0 && value != -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (is_open)
			{
				stream.WriteTimeout = value;
			}
			write_timeout = value;
		}
	}

	private static bool IsWindows
	{
		get
		{
			PlatformID platform = Environment.OSVersion.Platform;
			return platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT;
		}
	}

	[MonitoringDescription("")]
	public event SerialErrorReceivedEventHandler ErrorReceived
	{
		add
		{
			base.Events.AddHandler(error_received, value);
		}
		remove
		{
			base.Events.RemoveHandler(error_received, value);
		}
	}

	[MonitoringDescription("")]
	public event SerialPinChangedEventHandler PinChanged
	{
		add
		{
			base.Events.AddHandler(pin_changed, value);
		}
		remove
		{
			base.Events.RemoveHandler(pin_changed, value);
		}
	}

	[MonitoringDescription("")]
	public event SerialDataReceivedEventHandler DataReceived
	{
		add
		{
			base.Events.AddHandler(data_received, value);
		}
		remove
		{
			base.Events.RemoveHandler(data_received, value);
		}
	}

	public SerialPort()
		: this(GetDefaultPortName(), 9600, Parity.None, 8, StopBits.One)
	{
	}

	public SerialPort(IContainer container)
		: this()
	{
	}

	public SerialPort(string portName)
		: this(portName, 9600, Parity.None, 8, StopBits.One)
	{
	}

	public SerialPort(string portName, int baudRate)
		: this(portName, baudRate, Parity.None, 8, StopBits.One)
	{
	}

	public SerialPort(string portName, int baudRate, Parity parity)
		: this(portName, baudRate, parity, 8, StopBits.One)
	{
	}

	public SerialPort(string portName, int baudRate, Parity parity, int dataBits)
		: this(portName, baudRate, parity, dataBits, StopBits.One)
	{
	}

	public SerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
	{
		port_name = portName;
		baud_rate = baudRate;
		data_bits = dataBits;
		stop_bits = stopBits;
		this.parity = parity;
	}

	private static string GetDefaultPortName()
	{
		string[] portNames = GetPortNames();
		if (portNames.Length > 0)
		{
			return portNames[0];
		}
		int platform = (int)Environment.OSVersion.Platform;
		if (platform == 4 || platform == 128 || platform == 6)
		{
			return "ttyS0";
		}
		return "COM1";
	}

	public void Close()
	{
		Dispose(disposing: true);
	}

	protected override void Dispose(bool disposing)
	{
		if (is_open)
		{
			is_open = false;
			if (disposing)
			{
				stream.Close();
			}
			stream = null;
		}
	}

	public void DiscardInBuffer()
	{
		CheckOpen();
		stream.DiscardInBuffer();
	}

	public void DiscardOutBuffer()
	{
		CheckOpen();
		stream.DiscardOutBuffer();
	}

	public static string[] GetPortNames()
	{
		int platform = (int)Environment.OSVersion.Platform;
		List<string> list = new List<string>();
		if (platform == 4 || platform == 128 || platform == 6)
		{
			string[] files = Directory.GetFiles("/dev/", "tty*");
			string[] array = files;
			foreach (string text in array)
			{
				if (text.StartsWith("/dev/ttyS") || text.StartsWith("/dev/ttyUSB"))
				{
					list.Add(text);
				}
			}
		}
		else
		{
			using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM");
			if (registryKey != null)
			{
				string[] valueNames = registryKey.GetValueNames();
				string[] array2 = valueNames;
				foreach (string name in array2)
				{
					string text2 = registryKey.GetValue(name, string.Empty).ToString();
					if (text2 != string.Empty)
					{
						list.Add(text2);
					}
				}
			}
		}
		return list.ToArray();
	}

	public void Open()
	{
		if (is_open)
		{
			throw new InvalidOperationException("Port is already open");
		}
		if (IsWindows)
		{
			stream = new System.IO.Ports.WinSerialStream(port_name, baud_rate, data_bits, parity, stop_bits, dtr_enable, rts_enable, handshake, read_timeout, write_timeout, readBufferSize, writeBufferSize);
		}
		else
		{
			stream = new System.IO.Ports.SerialPortStream(port_name, baud_rate, data_bits, parity, stop_bits, dtr_enable, rts_enable, handshake, read_timeout, write_timeout, readBufferSize, writeBufferSize);
		}
		is_open = true;
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		CheckOpen();
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
		return stream.Read(buffer, offset, count);
	}

	[System.MonoTODO("Read of char buffers is currently broken")]
	public int Read(char[] buffer, int offset, int count)
	{
		CheckOpen();
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
		byte[] bytes = encoding.GetBytes(buffer, offset, count);
		return stream.Read(bytes, 0, bytes.Length);
	}

	internal int read_byte()
	{
		byte[] array = new byte[1];
		if (stream.Read(array, 0, 1) > 0)
		{
			return array[0];
		}
		return -1;
	}

	public int ReadByte()
	{
		CheckOpen();
		return read_byte();
	}

	public int ReadChar()
	{
		CheckOpen();
		byte[] array = new byte[16];
		int num = 0;
		do
		{
			int num2 = read_byte();
			if (num2 == -1)
			{
				return -1;
			}
			array[num++] = (byte)num2;
			char[] chars = encoding.GetChars(array, 0, 1);
			if (chars.Length > 0)
			{
				return chars[0];
			}
		}
		while (num < array.Length);
		return -1;
	}

	public string ReadExisting()
	{
		CheckOpen();
		int bytesToRead = BytesToRead;
		byte[] array = new byte[bytesToRead];
		int count = stream.Read(array, 0, bytesToRead);
		return new string(encoding.GetChars(array, 0, count));
	}

	public string ReadLine()
	{
		return ReadTo(new_line);
	}

	public string ReadTo(string value)
	{
		CheckOpen();
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.Length == 0)
		{
			throw new ArgumentException("value");
		}
		byte[] bytes = encoding.GetBytes(value);
		int num = 0;
		List<byte> list = new List<byte>();
		while (true)
		{
			int num2 = read_byte();
			if (num2 == -1)
			{
				break;
			}
			list.Add((byte)num2);
			if (num2 == bytes[num])
			{
				num++;
				if (num == bytes.Length)
				{
					return encoding.GetString(list.ToArray(), 0, list.Count - bytes.Length);
				}
			}
			else
			{
				num = ((bytes[0] == num2) ? 1 : 0);
			}
		}
		return encoding.GetString(list.ToArray());
	}

	public void Write(string str)
	{
		CheckOpen();
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		byte[] bytes = encoding.GetBytes(str);
		Write(bytes, 0, bytes.Length);
	}

	public void Write(byte[] buffer, int offset, int count)
	{
		CheckOpen();
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
		stream.Write(buffer, offset, count);
	}

	public void Write(char[] buffer, int offset, int count)
	{
		CheckOpen();
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
		byte[] bytes = encoding.GetBytes(buffer, offset, count);
		stream.Write(bytes, 0, bytes.Length);
	}

	public void WriteLine(string str)
	{
		Write(str + new_line);
	}

	private void CheckOpen()
	{
		if (!is_open)
		{
			throw new InvalidOperationException("Specified port is not open.");
		}
	}

	internal void OnErrorReceived(SerialErrorReceivedEventArgs args)
	{
		((SerialErrorReceivedEventHandler)base.Events[error_received])?.Invoke(this, args);
	}

	internal void OnDataReceived(SerialDataReceivedEventArgs args)
	{
		((SerialDataReceivedEventHandler)base.Events[data_received])?.Invoke(this, args);
	}

	internal void OnDataReceived(SerialPinChangedEventArgs args)
	{
		((SerialPinChangedEventHandler)base.Events[pin_changed])?.Invoke(this, args);
	}
}

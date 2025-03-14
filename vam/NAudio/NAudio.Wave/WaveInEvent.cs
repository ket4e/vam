using System;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Mixer;

namespace NAudio.Wave;

public class WaveInEvent : IWaveIn, IDisposable
{
	private readonly AutoResetEvent callbackEvent;

	private readonly SynchronizationContext syncContext;

	private IntPtr waveInHandle;

	private volatile CaptureState captureState;

	private WaveInBuffer[] buffers;

	public static int DeviceCount => WaveInterop.waveInGetNumDevs();

	public int BufferMilliseconds { get; set; }

	public int NumberOfBuffers { get; set; }

	public int DeviceNumber { get; set; }

	public WaveFormat WaveFormat { get; set; }

	public event EventHandler<WaveInEventArgs> DataAvailable;

	public event EventHandler<StoppedEventArgs> RecordingStopped;

	public WaveInEvent()
	{
		callbackEvent = new AutoResetEvent(initialState: false);
		syncContext = SynchronizationContext.Current;
		DeviceNumber = 0;
		WaveFormat = new WaveFormat(8000, 16, 1);
		BufferMilliseconds = 100;
		NumberOfBuffers = 3;
		captureState = CaptureState.Stopped;
	}

	public static WaveInCapabilities GetCapabilities(int devNumber)
	{
		WaveInCapabilities waveInCaps = default(WaveInCapabilities);
		int waveInCapsSize = Marshal.SizeOf(waveInCaps);
		MmException.Try(WaveInterop.waveInGetDevCaps((IntPtr)devNumber, out waveInCaps, waveInCapsSize), "waveInGetDevCaps");
		return waveInCaps;
	}

	private void CreateBuffers()
	{
		int num = BufferMilliseconds * WaveFormat.AverageBytesPerSecond / 1000;
		if (num % WaveFormat.BlockAlign != 0)
		{
			num -= num % WaveFormat.BlockAlign;
		}
		buffers = new WaveInBuffer[NumberOfBuffers];
		for (int i = 0; i < buffers.Length; i++)
		{
			buffers[i] = new WaveInBuffer(waveInHandle, num);
		}
	}

	private void OpenWaveInDevice()
	{
		CloseWaveInDevice();
		MmException.Try(WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)DeviceNumber, WaveFormat, callbackEvent.SafeWaitHandle.DangerousGetHandle(), IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackEvent), "waveInOpen");
		CreateBuffers();
	}

	public void StartRecording()
	{
		if (captureState != 0)
		{
			throw new InvalidOperationException("Already recording");
		}
		OpenWaveInDevice();
		MmException.Try(WaveInterop.waveInStart(waveInHandle), "waveInStart");
		captureState = CaptureState.Starting;
		ThreadPool.QueueUserWorkItem(delegate
		{
			RecordThread();
		}, null);
	}

	private void RecordThread()
	{
		Exception e = null;
		try
		{
			DoRecording();
		}
		catch (Exception ex)
		{
			e = ex;
		}
		finally
		{
			captureState = CaptureState.Stopped;
			RaiseRecordingStoppedEvent(e);
		}
	}

	private void DoRecording()
	{
		captureState = CaptureState.Capturing;
		WaveInBuffer[] array = buffers;
		foreach (WaveInBuffer waveInBuffer in array)
		{
			if (!waveInBuffer.InQueue)
			{
				waveInBuffer.Reuse();
			}
		}
		while (captureState == CaptureState.Capturing)
		{
			if (!callbackEvent.WaitOne())
			{
				continue;
			}
			array = buffers;
			foreach (WaveInBuffer waveInBuffer2 in array)
			{
				if (waveInBuffer2.Done)
				{
					this.DataAvailable?.Invoke(this, new WaveInEventArgs(waveInBuffer2.Data, waveInBuffer2.BytesRecorded));
					if (captureState == CaptureState.Capturing)
					{
						waveInBuffer2.Reuse();
					}
				}
			}
		}
	}

	private void RaiseRecordingStoppedEvent(Exception e)
	{
		EventHandler<StoppedEventArgs> handler = this.RecordingStopped;
		if (handler == null)
		{
			return;
		}
		if (syncContext == null)
		{
			handler(this, new StoppedEventArgs(e));
			return;
		}
		syncContext.Post(delegate
		{
			handler(this, new StoppedEventArgs(e));
		}, null);
	}

	public void StopRecording()
	{
		if (captureState != 0)
		{
			captureState = CaptureState.Stopping;
			callbackEvent.Set();
			MmException.Try(WaveInterop.waveInStop(waveInHandle), "waveInStop");
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (captureState != 0)
			{
				StopRecording();
			}
			CloseWaveInDevice();
		}
	}

	private void CloseWaveInDevice()
	{
		WaveInterop.waveInReset(waveInHandle);
		if (buffers != null)
		{
			for (int i = 0; i < buffers.Length; i++)
			{
				buffers[i].Dispose();
			}
			buffers = null;
		}
		WaveInterop.waveInClose(waveInHandle);
		waveInHandle = IntPtr.Zero;
	}

	public MixerLine GetMixerLine()
	{
		if (waveInHandle != IntPtr.Zero)
		{
			return new MixerLine(waveInHandle, 0, MixerFlags.WaveInHandle);
		}
		return new MixerLine((IntPtr)DeviceNumber, 0, MixerFlags.WaveIn);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

using System;
using System.Security;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class DSP_IIRDelay : BaseDSP
{
	private int _delay = 4096;

	private double _feedback = 0.5;

	private double _wetDry = 0.7;

	private double _d;

	private bool _useDithering;

	private double _ditherFactor = 0.7;

	private float _maxDelay = 2f;

	private int MAX_DELAY = 88200;

	private double[] buffer;

	private int counter;

	private int back;

	private int idx_1;

	private int idx0;

	private int idx1;

	private int idx2;

	private double y_1;

	private double y0;

	private double y1;

	private double y2;

	private double c0;

	private double c1;

	private double c2;

	private double c3;

	private double output;

	public double WetDry
	{
		get
		{
			return _wetDry;
		}
		set
		{
			if (value < 0.0)
			{
				_wetDry = 0.0;
			}
			else if (value > 1.0)
			{
				_wetDry = 1.0;
			}
			else
			{
				_wetDry = value;
			}
		}
	}

	public double Feedback
	{
		get
		{
			return _feedback;
		}
		set
		{
			if (value < 0.0)
			{
				_feedback = 0.0;
			}
			else if (value > 1.0)
			{
				_feedback = 1.0;
			}
			else
			{
				_feedback = value;
			}
		}
	}

	public int Delay
	{
		get
		{
			return _delay;
		}
		set
		{
			if (value < 0)
			{
				_delay = 0;
			}
			else
			{
				_delay = value;
			}
		}
	}

	public float DelaySeconds
	{
		get
		{
			return (float)Bass.BASS_ChannelBytes2Seconds(base.ChannelHandle, _delay * (base.ChannelBitwidth / 8));
		}
		set
		{
			if (value < 0f)
			{
				_delay = 0;
			}
			else if (value > 60f)
			{
				_delay = (int)(Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, 60.0) / (base.ChannelBitwidth / 8));
			}
			else
			{
				_delay = (int)(Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, value) / (base.ChannelBitwidth / 8));
			}
		}
	}

	public bool UseDithering
	{
		get
		{
			return _useDithering;
		}
		set
		{
			_useDithering = value;
		}
	}

	public double DitherFactor
	{
		get
		{
			return _ditherFactor;
		}
		set
		{
			_ditherFactor = value;
		}
	}

	public DSP_IIRDelay(float maxDelay)
	{
		_maxDelay = maxDelay;
	}

	public DSP_IIRDelay(int channel, int priority, float maxDelay)
		: base(channel, priority, IntPtr.Zero)
	{
		if (maxDelay > 60f)
		{
			maxDelay = 60f;
		}
		else if (maxDelay < 0.001f)
		{
			maxDelay = 0.001f;
		}
		_maxDelay = maxDelay;
		MAX_DELAY = (int)(Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, maxDelay) / (base.ChannelBitwidth / 8));
		buffer = new double[MAX_DELAY];
	}

	public void Preset_Default()
	{
		WetDry = 0.5;
		Feedback = 0.5;
		DelaySeconds = 0.25f;
	}

	public void Preset_Metallic()
	{
		WetDry = 0.5;
		Feedback = 0.75;
		DelaySeconds = 0.004f;
	}

	public void Preset_Echo()
	{
		WetDry = 0.42;
		Feedback = 0.3;
		DelaySeconds = 0.5f;
	}

	public void Reset()
	{
		counter = 0;
		for (int i = 0; i < MAX_DELAY; i++)
		{
			buffer[i] = 0.0;
		}
	}

	public override void OnChannelChanged()
	{
		MAX_DELAY = (int)(Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, _maxDelay) / (base.ChannelBitwidth / 8));
		buffer = new double[MAX_DELAY];
	}

	public unsafe override void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (base.IsBypassed)
		{
			return;
		}
		if (base.ChannelBitwidth == 16)
		{
			short* ptr = (short*)(void*)buffer;
			for (int i = 0; i < length / 2; i++)
			{
				_d = (double)ptr[i] * (1.0 - _wetDry) + ProcessSample(ptr[i]) * _wetDry;
				if (_useDithering)
				{
					_d = Utils.SampleDither(_d, _ditherFactor, 32768.0);
				}
				else
				{
					_d = Math.Round(_d);
				}
				if (_d > 32767.0)
				{
					ptr[i] = short.MaxValue;
				}
				else if (_d < -32768.0)
				{
					ptr[i] = short.MinValue;
				}
				else
				{
					ptr[i] = (short)_d;
				}
			}
			return;
		}
		if (base.ChannelBitwidth == 32)
		{
			float* ptr2 = (float*)(void*)buffer;
			for (int j = 0; j < length / 4; j++)
			{
				ptr2[j] = (float)((double)ptr2[j] * (1.0 - _wetDry) + ProcessSample(ptr2[j]) * _wetDry);
			}
			return;
		}
		byte* ptr3 = (byte*)(void*)buffer;
		for (int k = 0; k < length; k++)
		{
			_d = (ptr3[k] - 128) * 256;
			_d = _d * (1.0 - _wetDry) + ProcessSample(_d) * _wetDry;
			if (_useDithering)
			{
				_d = Utils.SampleDither(_d, _ditherFactor, 32768.0);
			}
			else
			{
				_d = Math.Round(_d);
			}
			if (_d > 32767.0)
			{
				ptr3[k] = byte.MaxValue;
			}
			else if (_d < -32768.0)
			{
				ptr3[k] = 0;
			}
			else
			{
				ptr3[k] = (byte)((int)_d / 256 + 128);
			}
		}
	}

	private double ProcessSample(double input)
	{
		if (_delay > 0)
		{
			back = counter - _delay;
			if (back < 0)
			{
				back = MAX_DELAY + back;
			}
			idx0 = back;
			idx_1 = idx0 - base.ChannelInfo.chans;
			idx1 = idx0 + base.ChannelInfo.chans;
			idx2 = idx0 + base.ChannelInfo.chans * 2;
			if (idx_1 < 0)
			{
				idx_1 = MAX_DELAY + idx_1;
			}
			if (idx1 >= MAX_DELAY)
			{
				idx1 %= MAX_DELAY;
			}
			if (idx2 >= MAX_DELAY)
			{
				idx2 %= MAX_DELAY;
			}
			y_1 = buffer[idx_1];
			y0 = buffer[idx0];
			y1 = buffer[idx1];
			y2 = buffer[idx2];
			c0 = y0;
			c1 = 0.5 * (y1 - y_1);
			c2 = y_1 - 2.5 * y0 + 2.0 * y1 - 0.5 * y2;
			c3 = 0.5 * (y2 - y_1) + 1.5 * (y0 - y1);
			output = ((c3 * 0.5 + c2) * 0.5 + c1) * 0.5 + c0;
		}
		else
		{
			output = input;
		}
		buffer[counter] = input * (1.5 - _feedback) + output * _feedback;
		counter++;
		if (counter >= MAX_DELAY)
		{
			counter = 0;
		}
		return output;
	}

	public override string ToString()
	{
		return "IIR Delay DSP";
	}
}

using System;
using System.Security;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class DSP_Pan : BaseDSP
{
	private double _lCh;

	private double _rCh;

	private double _pan;

	private bool _useDithering;

	private double _ditherFactor = 0.7;

	public double Pan
	{
		get
		{
			return _pan;
		}
		set
		{
			if (value < -1.0)
			{
				_pan = -1.0;
			}
			else if (value > 1.0)
			{
				_pan = 1.0;
			}
			else
			{
				_pan = value;
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

	public DSP_Pan()
	{
	}

	public DSP_Pan(int channel, int priority)
		: base(channel, priority, IntPtr.Zero)
	{
	}

	public unsafe override void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (base.IsBypassed)
		{
			return;
		}
		if (base.ChannelBitwidth == 32)
		{
			float* ptr = (float*)(void*)buffer;
			for (int i = 0; i < length / 4; i += 2)
			{
				if (_pan < 0.0)
				{
					ptr[i + 1] = (float)((double)ptr[i + 1] * (1.0 + _pan));
				}
				else if (_pan > 0.0)
				{
					ptr[i] = (float)((double)ptr[i] * (1.0 - _pan));
				}
			}
			return;
		}
		if (base.ChannelBitwidth == 16)
		{
			short* ptr2 = (short*)(void*)buffer;
			for (int j = 0; j < length / 2; j += 2)
			{
				if (_useDithering)
				{
					if (_pan < 0.0)
					{
						_rCh = Utils.SampleDither((double)ptr2[j + 1] * (1.0 + _pan), _ditherFactor, 32768.0);
					}
					else if (_pan > 0.0)
					{
						_lCh = Utils.SampleDither((double)ptr2[j] * (1.0 - _pan), _ditherFactor, 32768.0);
					}
				}
				else if (_pan < 0.0)
				{
					_rCh = Math.Round((double)ptr2[j + 1] * (1.0 + _pan));
				}
				else if (_pan > 0.0)
				{
					_rCh = Math.Round((double)ptr2[j] * (1.0 - _pan));
				}
				if (_lCh > 32767.0)
				{
					ptr2[j] = short.MaxValue;
				}
				else if (_lCh < -32768.0)
				{
					ptr2[j] = short.MinValue;
				}
				else
				{
					ptr2[j] = (short)_lCh;
				}
				if (_rCh > 32767.0)
				{
					ptr2[j + 1] = short.MaxValue;
				}
				else if (_rCh < -32768.0)
				{
					ptr2[j + 1] = short.MinValue;
				}
				else
				{
					ptr2[j + 1] = (short)_rCh;
				}
			}
			return;
		}
		byte* ptr3 = (byte*)(void*)buffer;
		for (int k = 0; k < length; k += 2)
		{
			if (_useDithering)
			{
				if (_pan < 0.0)
				{
					_rCh = Utils.SampleDither((double)(ptr3[k + 1] - 128) * (1.0 + _pan), _ditherFactor, 128.0);
				}
				else if (_pan > 0.0)
				{
					_lCh = Utils.SampleDither((double)(ptr3[k] - 128) * (1.0 - _pan), _ditherFactor, 128.0);
				}
			}
			else if (_pan < 0.0)
			{
				_rCh = Math.Round((double)(ptr3[k + 1] - 128) * (1.0 + _pan));
			}
			else if (_pan > 0.0)
			{
				_rCh = Math.Round((double)(ptr3[k] - 128) * (1.0 - _pan));
			}
			if (_lCh > 127.0)
			{
				ptr3[k] = byte.MaxValue;
			}
			else if (_lCh < -128.0)
			{
				ptr3[k] = 0;
			}
			else
			{
				ptr3[k] = (byte)((int)_lCh + 128);
			}
			if (_rCh > 127.0)
			{
				ptr3[k] = byte.MaxValue;
			}
			else if (_rCh < -128.0)
			{
				ptr3[k] = 0;
			}
			else
			{
				ptr3[k] = (byte)((int)_rCh + 128);
			}
		}
	}

	public override string ToString()
	{
		return "Stereo Panning DSP";
	}
}

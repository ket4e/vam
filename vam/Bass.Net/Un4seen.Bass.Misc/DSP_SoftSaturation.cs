using System;
using System.Security;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class DSP_SoftSaturation : BaseDSP
{
	private double _d;

	private double _s = 1.0;

	private double _factor = 0.5;

	private double _depth = 0.5;

	private bool _useDithering;

	private double _ditherFactor = 0.7;

	public double Depth
	{
		get
		{
			return _depth;
		}
		set
		{
			if (value < 0.0)
			{
				_depth = 0.0;
			}
			else if (_depth > 1.0)
			{
				_depth = 1.0;
			}
			else
			{
				_depth = value;
			}
		}
	}

	public double Factor
	{
		get
		{
			return _factor;
		}
		set
		{
			if (value < 0.0)
			{
				_factor = 0.0;
			}
			else if (_factor > 0.99998848714)
			{
				_factor = 0.99998848714;
			}
			else
			{
				_factor = value;
			}
		}
	}

	public double Factor_dBV
	{
		get
		{
			return Utils.LevelToDB(_factor, 1.0);
		}
		set
		{
			if (value > -0.0001)
			{
				_factor = 0.99998848714;
			}
			else if (value == double.NegativeInfinity)
			{
				_factor = 0.0;
			}
			else
			{
				_factor = Utils.DBToLevel(value, 1.0);
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

	public DSP_SoftSaturation()
	{
	}

	public DSP_SoftSaturation(int channel, int priority)
		: base(channel, priority, IntPtr.Zero)
	{
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
				_d = Math.Abs((double)ptr[i] / 32768.0);
				_s = Math.Sign(ptr[i]);
				if (_d > 1.0)
				{
					_d = (_factor + 1.0) / 2.0;
				}
				else if (_d > _factor)
				{
					_d = _factor + (_d - _factor) / (1.0 + Math.Pow((_d - _factor) / (1.0 - _factor), 2.0));
				}
				if (_depth > 0.0)
				{
					_d = Math.Tanh(1.4 * ((double)ptr[i] / 32768.0 + _d * _s * _depth));
				}
				else
				{
					_d *= _s;
				}
				if (_useDithering)
				{
					_d = Utils.SampleDither(_d * 32768.0, _ditherFactor, 32768.0);
				}
				else
				{
					_d = Math.Round(_d * 32768.0);
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
				_d = Math.Abs(ptr2[j]);
				_s = Math.Sign(ptr2[j]);
				if (_d > 1.0)
				{
					_d = (_factor + 1.0) / 2.0;
				}
				else if (_d > _factor)
				{
					_d = _factor + (_d - _factor) / (1.0 + Math.Pow((_d - _factor) / (1.0 - _factor), 2.0));
				}
				if (_depth > 0.0)
				{
					ptr2[j] = (float)Math.Tanh((double)ptr2[j] + _d * _s * _depth);
				}
				else
				{
					ptr2[j] = (float)(_d * _s);
				}
			}
			return;
		}
		byte* ptr3 = (byte*)(void*)buffer;
		for (int k = 0; k < length; k++)
		{
			_d = Math.Abs((double)(int)ptr3[k] / 255.0);
			if (_d > 1.0)
			{
				_d = (_factor + 1.0) / 2.0;
			}
			else if (_d > _factor)
			{
				_d = _factor + (_d - _factor) / (1.0 + Math.Pow((_d - _factor) / (1.0 - _factor), 2.0));
			}
			if (_depth > 0.0)
			{
				_d = Math.Tanh(1.4 * ((double)(int)ptr3[k] / 255.0 + _d * _depth));
			}
			if (_useDithering)
			{
				_d = Utils.SampleDither(_d * 255.0, _ditherFactor, 255.0);
			}
			else
			{
				_d = Math.Round(_d * 255.0);
			}
			if (_d > 255.0)
			{
				ptr3[k] = byte.MaxValue;
			}
			else if (_d < 0.0)
			{
				ptr3[k] = 0;
			}
			else
			{
				ptr3[k] = (byte)((double)(int)_d - 128.0);
			}
		}
	}

	public override string ToString()
	{
		return "Soft Saturation DSP";
	}
}

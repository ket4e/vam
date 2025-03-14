using System;
using System.Security;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class DSP_StereoEnhancer : BaseDSP
{
	private double _wetDry = 0.5;

	private double _wideCoeff = 2.0;

	private double _gain = 1.0;

	private double _lCh;

	private double _rCh;

	private double _mono;

	private bool _useDithering;

	private double _ditherFactor = 0.7;

	public double WideCoeff
	{
		get
		{
			return _wideCoeff;
		}
		set
		{
			_wideCoeff = value;
			_gain = Utils.DBToLevel(-1.0 * _wideCoeff * _wetDry, 1.0) + 0.2;
		}
	}

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
			_gain = Utils.DBToLevel(-1.0 * _wideCoeff * _wetDry, 1.0) + 0.2;
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

	public DSP_StereoEnhancer()
	{
		_gain = Utils.DBToLevel(-1.0 * _wideCoeff * _wetDry, 1.0) + 0.2;
	}

	public DSP_StereoEnhancer(int channel, int priority)
		: base(channel, priority, IntPtr.Zero)
	{
		_gain = Utils.DBToLevel(-1.0 * _wideCoeff * _wetDry, 1.0) + 0.2;
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
				_lCh = ptr[i];
				_rCh = ptr[i + 1];
				_mono = (_lCh + _rCh) / 2.0;
				_lCh = ((_lCh - _mono) * _wideCoeff + _lCh) / 2.0 * _gain;
				_rCh = ((_rCh - _mono) * _wideCoeff + _rCh) / 2.0 * _gain;
				ptr[i] = (float)(_lCh * _wetDry + (double)ptr[i] * (1.0 - _wetDry));
				ptr[i + 1] = (float)(_rCh * _wetDry + (double)ptr[i + 1] * (1.0 - _wetDry));
			}
			return;
		}
		if (base.ChannelBitwidth == 16)
		{
			short* ptr2 = (short*)(void*)buffer;
			for (int j = 0; j < length / 2; j += 2)
			{
				_lCh = (double)ptr2[j] / 32768.0;
				_rCh = (double)ptr2[j + 1] / 32768.0;
				_mono = (_lCh + _rCh) / 2.0;
				if (_useDithering)
				{
					_lCh = Utils.SampleDither((((_lCh - _mono) * _wideCoeff + _lCh) / 2.0 * _gain + _lCh * (1.0 - _wetDry)) * 32768.0, _ditherFactor, 32768.0);
					_rCh = Utils.SampleDither((((_rCh - _mono) * _wideCoeff + _rCh) / 2.0 * _gain + _rCh * (1.0 - _wetDry)) * 32768.0, _ditherFactor, 32768.0);
				}
				else
				{
					_lCh = Math.Round((((_lCh - _mono) * _wideCoeff + _lCh) / 2.0 * _gain + _lCh * (1.0 - _wetDry)) * 32768.0);
					_rCh = Math.Round((((_rCh - _mono) * _wideCoeff + _rCh) / 2.0 * _gain + _rCh * (1.0 - _wetDry)) * 32768.0);
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
			_lCh = (double)(ptr3[k] - 128) / 128.0;
			_rCh = (double)(ptr3[k + 1] - 128) / 128.0;
			_mono = (_lCh + _rCh) / 2.0;
			if (_useDithering)
			{
				_lCh = Utils.SampleDither((((_lCh - _mono) * _wideCoeff + _lCh) / 2.0 * _gain + _lCh * (1.0 - _wetDry)) * 128.0, _ditherFactor, 128.0);
				_rCh = Utils.SampleDither((((_rCh - _mono) * _wideCoeff + _rCh) / 2.0 * _gain + _rCh * (1.0 - _wetDry)) * 128.0, _ditherFactor, 128.0);
			}
			else
			{
				_lCh = Math.Round((((_lCh - _mono) * _wideCoeff + _lCh) / 2.0 * _gain + _lCh * (1.0 - _wetDry)) * 128.0);
				_rCh = Math.Round((((_rCh - _mono) * _wideCoeff + _rCh) / 2.0 * _gain + _rCh * (1.0 - _wetDry)) * 128.0);
			}
			if (_lCh > 32767.0)
			{
				ptr3[k] = byte.MaxValue;
			}
			else if (_lCh < -32768.0)
			{
				ptr3[k] = 0;
			}
			else
			{
				ptr3[k] = (byte)((int)_lCh / 256 + 128);
			}
			if (_rCh > 32767.0)
			{
				ptr3[k + 1] = byte.MaxValue;
			}
			else if (_rCh < -32768.0)
			{
				ptr3[k + 1] = 0;
			}
			else
			{
				ptr3[k + 1] = (byte)((int)_rCh / 256 + 128);
			}
		}
	}

	public override string ToString()
	{
		return "Stereo Enhancer DSP";
	}
}

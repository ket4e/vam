using System;
using System.Security;

namespace Un4seen.Bass.Misc;

[SuppressUnmanagedCodeSecurity]
public sealed class DSP_PeakLevelMeter : BaseDSP
{
	private double _levelL;

	private double _levelR;

	private double _peakHoldLevelL;

	private double _peakHoldLevelR;

	private int _length;

	private double _dummy;

	private float _updateTime = 0.1f;

	private long _updateLength = 35280L;

	private long _refreshLength;

	private bool _calcRMS;

	private double _rms;

	private double _avg;

	private double _squSum;

	private double _avgSum;

	public int LevelL => (int)Math.Round(_levelL);

	public int PeakHoldLevelL => (int)Math.Round(_peakHoldLevelL);

	public double LevelL_dBV => Utils.LevelToDB(_levelL, 32768.0);

	public double PeakHoldLevelL_dBV => Utils.LevelToDB(_peakHoldLevelL, 32768.0);

	public int LevelR => (int)Math.Round(_levelR);

	public int PeakHoldLevelR => (int)Math.Round(_peakHoldLevelR);

	public double LevelR_dBV => Utils.LevelToDB(_levelR, 32768.0);

	public double PeakHoldLevelR_dBV => Utils.LevelToDB(_peakHoldLevelR, 32768.0);

	public bool CalcRMS
	{
		get
		{
			return _calcRMS;
		}
		set
		{
			_calcRMS = value;
			if (!_calcRMS)
			{
				_rms = 0.0;
				_avg = 0.0;
			}
		}
	}

	public double AVG => _avg;

	public double AVG_dBV => Utils.LevelToDB(_avg, 32768.0);

	public double RMS => _rms;

	public double RMS_dBV => Utils.LevelToDB(_rms, 32768.0);

	public float UpdateTime
	{
		get
		{
			return (float)Bass.BASS_ChannelBytes2Seconds(base.ChannelHandle, _updateLength);
		}
		set
		{
			if (value <= 0f)
			{
				_updateTime = 0f;
				_updateLength = 0L;
			}
			else
			{
				if (value > 60f)
				{
					_updateTime = 60f;
				}
				else
				{
					_updateTime = value;
				}
				_updateLength = Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, _updateTime);
				if (_updateLength <= 0)
				{
					_updateLength = 0L;
				}
			}
			_refreshLength = 0L;
		}
	}

	public DSP_PeakLevelMeter()
	{
	}

	public DSP_PeakLevelMeter(int channel, int priority)
		: base(channel, priority, IntPtr.Zero)
	{
		UpdateTime = _updateTime;
	}

	public override void OnChannelChanged()
	{
		ResetPeakHold();
		_updateLength = (int)Bass.BASS_ChannelSeconds2Bytes(base.ChannelHandle, _updateTime);
	}

	public void ResetPeakHold()
	{
		_peakHoldLevelL = 0.0;
		_peakHoldLevelR = 0.0;
	}

	public unsafe override void DSPCallback(int handle, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (base.IsBypassed)
		{
			return;
		}
		if (_refreshLength == 0L)
		{
			_squSum = 0.0;
			_avgSum = 0.0;
			_levelL = 0.0;
			_levelR = 0.0;
		}
		if (base.ChannelBitwidth == 32)
		{
			_length = length / 4;
			float* ptr = (float*)(void*)buffer;
			for (int i = 0; i < _length; i++)
			{
				_dummy = Math.Round(Math.Abs((double)ptr[i]) * 32768.0);
				if (i % 2 == 0)
				{
					if (_dummy > _levelL)
					{
						_levelL = _dummy;
					}
				}
				else if (_dummy > _levelR)
				{
					_levelR = _dummy;
				}
				if (CalcRMS)
				{
					_avgSum += _dummy;
					_squSum += _dummy * _dummy;
				}
			}
		}
		else if (base.ChannelBitwidth == 16)
		{
			_length = length / 2;
			short* ptr2 = (short*)(void*)buffer;
			for (int j = 0; j < _length; j++)
			{
				_dummy = Math.Abs((double)ptr2[j]);
				if (j % 2 == 0)
				{
					if (_dummy > _levelL)
					{
						_levelL = _dummy;
					}
				}
				else if (_dummy > _levelR)
				{
					_levelR = _dummy;
				}
				if (CalcRMS)
				{
					_avgSum += _dummy;
					_squSum += _dummy * _dummy;
				}
			}
		}
		else
		{
			_length = length;
			byte* ptr3 = (byte*)(void*)buffer;
			for (int k = 0; k < _length; k++)
			{
				_dummy = (ptr3[k] - 128) * 256;
				if (k % 2 == 0)
				{
					if (_dummy > _levelL)
					{
						_levelL = _dummy;
					}
				}
				else if (_dummy > _levelR)
				{
					_levelR = _dummy;
				}
				if (CalcRMS)
				{
					_avgSum += _dummy;
					_squSum += _dummy * _dummy;
				}
			}
		}
		if (base.ChannelNumChans == 1)
		{
			_levelR = (_levelL = Math.Max(_levelL, _levelR));
		}
		_refreshLength += length;
		if (_refreshLength >= _updateLength)
		{
			if (CalcRMS && _refreshLength > 0)
			{
				_avg = _avgSum / (double)_refreshLength;
				_rms = Math.Sqrt(((double)_refreshLength * _squSum - _avgSum * _avgSum) / ((double)_refreshLength * ((double)_refreshLength - 1.0)));
			}
			if (_levelL > _peakHoldLevelL)
			{
				_peakHoldLevelL = _levelL;
			}
			if (_levelR > _peakHoldLevelR)
			{
				_peakHoldLevelR = _levelR;
			}
			_refreshLength = 0L;
			RaiseNotification();
		}
	}

	public override string ToString()
	{
		return "Peak Level Meter DSP";
	}
}

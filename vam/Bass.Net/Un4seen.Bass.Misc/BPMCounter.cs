using System;
using System.Security;

namespace Un4seen.Bass.Misc;

[SuppressUnmanagedCodeSecurity]
public sealed class BPMCounter
{
	private bool beat;

	private bool bpmBeat;

	private double deltaMS;

	private int _errorBPMs;

	private double _minBPM = 60.0;

	private double _maxBPM = 180.0;

	private DateTime _lastBeatTime = DateTime.MinValue;

	private DateTime _lastTapBeatTime = DateTime.MinValue;

	private double _minBPMms;

	private DateTime _nextBeatStart = DateTime.MinValue;

	private DateTime _nextBeatEnd = DateTime.MinValue;

	private int _startCounter;

	private int _LF = 350;

	private int _MF1 = 1500;

	private int _MF2 = 4000;

	private int _LFfftBin1 = 5;

	private int _MFfftBin0 = 11;

	private int _MFfftBin1 = 52;

	private int _historyCount;

	private float[][] _historyEnergy;

	private int _historyBPMSize = 10;

	private double[] _historyBPM;

	private double[] _historyTapBPM;

	private float[] _fft = new float[256];

	private double BEAT_RTIME = 0.019999999552965164;

	private double _beatRelease = 0.12;

	private double[] _peakEnv = new double[128];

	private bool[] _beatTrigger = new bool[128];

	private bool[] _prevBeatPulse = new bool[128];

	private bool _beatTriggerC;

	private bool _beatTriggerD;

	private bool _beatTriggerE;

	private bool _prevBeatPulseC;

	private double envIn;

	private float Es;

	private float avgE;

	private float varianceE;

	private double subbandBeatHits;

	private double _peakSubbandBeatHits;

	private double _prevPeakSubbandBeatHits;

	private DateTime beatTime;

	private int autoresetCounter;

	private int autoresetCounterTap;

	public int BPMHistorySize
	{
		get
		{
			return _historyBPMSize;
		}
		set
		{
			if (_historyBPMSize != value && value >= 2 && value <= 50)
			{
				_historyBPMSize = value;
				_historyBPM = new double[_historyBPMSize];
				_historyTapBPM = new double[_historyBPMSize];
				_startCounter = 0;
				Array.Clear(_historyBPM, 0, _historyBPMSize);
				Array.Clear(_historyTapBPM, 0, _historyBPMSize);
				_beatTriggerC = false;
				_beatTriggerD = false;
				_beatTriggerE = false;
				_prevBeatPulseC = false;
				autoresetCounter = 0;
				_errorBPMs = 0;
			}
		}
	}

	public double MinBPM
	{
		get
		{
			return _minBPM;
		}
		set
		{
			if (!(value < 30.0) && !(value > _maxBPM))
			{
				_minBPM = value;
			}
		}
	}

	public double MaxBPM
	{
		get
		{
			return _maxBPM;
		}
		set
		{
			if (!(value > 250.0) && !(value < _minBPM))
			{
				_maxBPM = value;
			}
		}
	}

	public double BPM
	{
		get
		{
			double num = 0.0;
			int num2 = 0;
			double[] historyBPM = _historyBPM;
			foreach (double num3 in historyBPM)
			{
				if (num3 > 0.0)
				{
					num += num3;
					num2++;
				}
			}
			return num / (double)num2;
		}
		set
		{
			autoresetCounter++;
			if (autoresetCounter > 2 * _historyBPMSize)
			{
				autoresetCounter = 0;
				Array.Clear(_historyBPM, 2, _historyBPMSize - 2);
			}
			double num;
			for (num = value; num < _minBPM; num *= 2.0)
			{
			}
			if (_historyBPM[0] == 0.0)
			{
				while (num > _maxBPM)
				{
					num /= 2.0;
				}
				_historyBPM[0] = num;
				return;
			}
			if (num < 0.91 * _historyBPM[0] || num > 1.11 * _historyBPM[0])
			{
				_errorBPMs++;
				if (_errorBPMs == 3)
				{
					_errorBPMs = 0;
					autoresetCounter = 0;
					Array.Clear(_historyBPM, 0, _historyBPMSize);
				}
				if (autoresetCounter > 0)
				{
					return;
				}
			}
			else
			{
				_errorBPMs = 0;
			}
			while (num > _maxBPM)
			{
				num /= 2.0;
			}
			ShiftHistoryBPM();
			_historyBPM[0] = num;
		}
	}

	public double TappedBPM
	{
		get
		{
			double num = 0.0;
			int num2 = 0;
			double[] historyTapBPM = _historyTapBPM;
			foreach (double num3 in historyTapBPM)
			{
				if (num3 > 0.0)
				{
					num += num3;
					num2++;
				}
			}
			return num / (double)num2;
		}
		set
		{
			autoresetCounterTap++;
			if (autoresetCounterTap > 2 * _historyBPMSize)
			{
				autoresetCounterTap = 0;
				Array.Clear(_historyTapBPM, 2, _historyBPMSize - 2);
			}
			double num;
			for (num = value; num < _minBPM; num *= 2.0)
			{
			}
			if (_historyTapBPM[0] == 0.0)
			{
				while (num > _maxBPM)
				{
					num /= 2.0;
				}
				_historyTapBPM[0] = num;
				return;
			}
			if ((num < 0.85 * _historyTapBPM[0] || num > 1.15 * _historyTapBPM[0]) && autoresetCounterTap > 0)
			{
				_lastTapBeatTime = DateTime.MinValue;
				_beatTriggerE = false;
				autoresetCounterTap = 0;
				Array.Clear(_historyTapBPM, 0, _historyBPMSize);
			}
			while (num > _maxBPM)
			{
				num /= 2.0;
			}
			ShiftHistoryTapBPM();
			_historyTapBPM[0] = num;
		}
	}

	public BPMCounter(int timerfreq, int samplerate)
	{
		if (timerfreq < 0)
		{
			timerfreq = 50;
		}
		if (samplerate < 1)
		{
			samplerate = 44100;
		}
		_historyCount = 1000 / timerfreq + 1;
		BEAT_RTIME = 1.0 / (double)timerfreq;
		_beatRelease = 3.0 * BEAT_RTIME;
		_minBPMms = 60000.0 / _minBPM;
		_historyBPM = new double[_historyBPMSize];
		_historyTapBPM = new double[_historyBPMSize];
		_startCounter = 0;
		_LFfftBin1 = Utils.FFTFrequency2Index(_LF, 512, samplerate);
		_MFfftBin0 = Utils.FFTFrequency2Index(_MF1, 512, samplerate);
		_MFfftBin1 = Utils.FFTFrequency2Index(_MF2, 512, samplerate);
		Array.Clear(_peakEnv, 0, 128);
		Array.Clear(_beatTrigger, 0, 128);
		Array.Clear(_prevBeatPulse, 0, 128);
		Array.Clear(_historyBPM, 0, _historyBPMSize);
		Array.Clear(_historyTapBPM, 0, _historyBPMSize);
		_beatTriggerC = false;
		_beatTriggerD = false;
		_beatTriggerE = false;
		_prevBeatPulseC = false;
		_lastBeatTime = DateTime.MinValue;
		_lastTapBeatTime = DateTime.MinValue;
		_nextBeatStart = DateTime.MinValue;
		_nextBeatEnd = DateTime.MinValue;
		_historyEnergy = new float[128][];
		for (int i = 0; i < 128; i++)
		{
			_historyEnergy[i] = new float[_historyCount];
			Array.Clear(_historyEnergy[i], 0, _historyCount);
		}
		autoresetCounter = 0;
		autoresetCounterTap = 0;
		_errorBPMs = 0;
	}

	private double BPMToMilliseconds(double bpm)
	{
		return 60000.0 / bpm;
	}

	private double MillisecondsToBPM(double ms)
	{
		return 60000.0 / ms;
	}

	private float[] HistoryEnergy(int subband)
	{
		return _historyEnergy[subband];
	}

	private float AverageLocalEnergy(int subband)
	{
		float num = 0f;
		float[] array = _historyEnergy[subband];
		foreach (float num2 in array)
		{
			num += num2;
		}
		return num / (float)_historyCount;
	}

	private void ShiftHistoryEnergy(int subband)
	{
		for (int num = _historyCount - 2; num >= 0; num--)
		{
			_historyEnergy[subband][num + 1] = _historyEnergy[subband][num];
		}
	}

	private void ShiftHistoryBPM()
	{
		for (int num = _historyBPMSize - 2; num >= 0; num--)
		{
			_historyBPM[num + 1] = _historyBPM[num];
		}
	}

	private void ShiftHistoryTapBPM()
	{
		for (int num = _historyBPMSize - 2; num >= 0; num--)
		{
			_historyTapBPM[num + 1] = _historyTapBPM[num];
		}
	}

	private float VarianceLocalEnergy(int subband, float avgE)
	{
		float num = 0f;
		float[] array = _historyEnergy[subband];
		foreach (float num2 in array)
		{
			num += Math.Abs(num2 - avgE);
		}
		return num / (float)_historyCount;
	}

	public void TapBeat()
	{
		DateTime now = DateTime.Now;
		if (_beatTriggerE)
		{
			double totalMilliseconds = (now - _lastTapBeatTime).TotalMilliseconds;
			TappedBPM = MillisecondsToBPM(totalMilliseconds);
		}
		_lastTapBeatTime = now;
		_beatTriggerE = true;
	}

	public void Reset(int samplerate)
	{
		_lastBeatTime = DateTime.MinValue;
		_lastTapBeatTime = DateTime.MinValue;
		_prevPeakSubbandBeatHits = 0.0;
		_startCounter = 0;
		_errorBPMs = 0;
		_minBPMms = 60000.0 / _minBPM;
		if (samplerate > 0)
		{
			_LFfftBin1 = Utils.FFTFrequency2Index(_LF, 512, samplerate);
			_MFfftBin0 = Utils.FFTFrequency2Index(_MF1, 512, samplerate);
			_MFfftBin1 = Utils.FFTFrequency2Index(_MF2, 512, samplerate);
		}
		Array.Clear(_peakEnv, 0, 128);
		Array.Clear(_beatTrigger, 0, 128);
		Array.Clear(_prevBeatPulse, 0, 128);
		Array.Clear(_historyBPM, 0, _historyBPMSize);
		Array.Clear(_historyTapBPM, 0, _historyBPMSize);
		_beatTriggerC = false;
		_beatTriggerD = false;
		_beatTriggerE = false;
		_prevBeatPulseC = false;
		_nextBeatStart = DateTime.MinValue;
		_nextBeatEnd = DateTime.MinValue;
		for (int i = 0; i < 128; i++)
		{
			Array.Clear(_historyEnergy[i], 0, _historyCount);
		}
		autoresetCounter = 0;
	}

	public void SetSamperate(int samplerate)
	{
		if (samplerate > 0)
		{
			_LFfftBin1 = Utils.FFTFrequency2Index(_LF, 512, samplerate);
			_MFfftBin0 = Utils.FFTFrequency2Index(_MF1, 512, samplerate);
			_MFfftBin1 = Utils.FFTFrequency2Index(_MF2, 512, samplerate);
		}
	}

	public bool ProcessAudio(int channel, bool bpmBeatsOnly)
	{
		if (channel == 0)
		{
			return false;
		}
		beat = false;
		bpmBeat = false;
		subbandBeatHits = 0.0;
		if (_startCounter <= _historyCount)
		{
			_startCounter++;
		}
		try
		{
			if (Bass.BASS_ChannelGetData(channel, _fft, -2147483647) > 0)
			{
				for (int i = 1; i < 128; i++)
				{
					Es = _fft[i];
					if (_startCounter > _historyCount)
					{
						avgE = AverageLocalEnergy(i);
						varianceE = VarianceLocalEnergy(i, avgE);
						envIn = (double)Es / (double)avgE;
						if (avgE > 0f)
						{
							if (envIn > _peakEnv[i])
							{
								_peakEnv[i] = envIn;
							}
							else
							{
								_peakEnv[i] *= _beatRelease;
								_peakEnv[i] += (1.0 - _beatRelease) * envIn;
							}
							if (!_beatTrigger[i])
							{
								if (i <= _LFfftBin1)
								{
									if (_peakEnv[i] > 1.7 * (double)((avgE + varianceE) / avgE))
									{
										_beatTrigger[i] = true;
									}
								}
								else if (i >= _MFfftBin0 && i <= _MFfftBin1)
								{
									if (_peakEnv[i] > 2.4 * (double)((avgE + varianceE) / avgE))
									{
										_beatTrigger[i] = true;
									}
								}
								else if (_peakEnv[i] > 2.8 * (double)((avgE + varianceE) / avgE))
								{
									_beatTrigger[i] = true;
								}
							}
							else if (i <= _LFfftBin1)
							{
								if (_peakEnv[i] < 1.4 * (double)((avgE + varianceE) / avgE))
								{
									_beatTrigger[i] = false;
								}
							}
							else if (i >= _MFfftBin0 && i <= _MFfftBin1)
							{
								if (_peakEnv[i] < 1.1 * (double)((avgE + varianceE) / avgE))
								{
									_beatTrigger[i] = false;
								}
							}
							else if (_peakEnv[i] < 1.4 * (double)((avgE + varianceE) / avgE))
							{
								_beatTrigger[i] = false;
							}
							if (_beatTrigger[i] && !_prevBeatPulse[i])
							{
								if (i <= _LFfftBin1)
								{
									subbandBeatHits += 100.0 * (double)(avgE / varianceE);
								}
								else if (i >= _MFfftBin0 && i <= _MFfftBin1)
								{
									subbandBeatHits += 10.0 * (double)(avgE / varianceE);
								}
								else
								{
									subbandBeatHits += 2.0 * (double)(avgE / varianceE);
								}
							}
							_prevBeatPulse[i] = _beatTrigger[i];
						}
					}
					else
					{
						_nextBeatStart = DateTime.Now;
						_nextBeatEnd = _nextBeatStart.AddMilliseconds(330.0);
					}
					ShiftHistoryEnergy(i);
					_historyEnergy[i][0] = Es;
				}
				if (subbandBeatHits > _peakSubbandBeatHits)
				{
					_peakSubbandBeatHits = subbandBeatHits;
				}
				else
				{
					_peakSubbandBeatHits = (_peakSubbandBeatHits + subbandBeatHits) / 2.0;
				}
				if (!_beatTriggerC)
				{
					if (_peakSubbandBeatHits > 200.0)
					{
						_beatTriggerC = true;
						beatTime = DateTime.Now;
					}
				}
				else if (_peakSubbandBeatHits < 100.0)
				{
					_beatTriggerC = false;
				}
				if (_beatTriggerC && !_prevBeatPulseC)
				{
					beat = true;
				}
				_prevBeatPulseC = _beatTriggerC;
				if (beat)
				{
					if (_beatTriggerD)
					{
						deltaMS = Math.Round((beatTime - _lastBeatTime).TotalMilliseconds);
						if (deltaMS < 333.0 && _peakSubbandBeatHits < _prevPeakSubbandBeatHits)
						{
							beat = false;
							beatTime = _lastBeatTime;
						}
						else
						{
							_prevPeakSubbandBeatHits = _peakSubbandBeatHits;
							if (beatTime >= _nextBeatStart && beatTime <= _nextBeatEnd)
							{
								BPM = MillisecondsToBPM(deltaMS);
								bpmBeat = true;
								_nextBeatStart = beatTime.AddMilliseconds((1.0 - _beatRelease) * deltaMS);
								_nextBeatEnd = beatTime.AddMilliseconds((1.0 + _beatRelease) * deltaMS);
							}
							else if (beatTime < _nextBeatStart)
							{
								_beatTriggerD = false;
							}
						}
						if (beatTime > _nextBeatEnd)
						{
							_nextBeatStart = beatTime.AddMilliseconds((1.0 - _beatRelease) * deltaMS);
							_nextBeatEnd = beatTime.AddMilliseconds((1.0 + _beatRelease) * deltaMS);
						}
					}
					_lastBeatTime = beatTime;
					_beatTriggerD = true;
				}
			}
		}
		catch
		{
		}
		if (bpmBeatsOnly)
		{
			return bpmBeat;
		}
		return beat;
	}
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Security;
using System.Windows.Forms;
using Un4seen.Bass.AddOn.Mix;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class Visuals
{
	private float[] _fft = new float[2048];

	private float[] _lastPeak = new float[2048];

	private bool _channelIsMixerSource;

	private int _scaleFactorLinear = 9;

	private float _scaleFactorLinearBoost = 0.05f;

	private int _scaleFactorSqr = 4;

	private float _scaleFactorSqrBoost = 0.005f;

	private int _maxFFTData = 4096;

	private int _maxFFTSampleIndex = 2047;

	private BASSData _maxFFT = BASSData.BASS_DATA_FFT4096;

	private int _maxFrequencySpectrum = 2047;

	private double spp = 1.0;

	private bool ylinear;

	public bool ChannelIsMixerSource
	{
		get
		{
			return _channelIsMixerSource;
		}
		set
		{
			_channelIsMixerSource = value;
		}
	}

	public int ScaleFactorLinear
	{
		get
		{
			return _scaleFactorLinear;
		}
		set
		{
			_scaleFactorLinear = value;
		}
	}

	public float ScaleFactorLinearBoost
	{
		get
		{
			return _scaleFactorLinearBoost;
		}
		set
		{
			_scaleFactorLinearBoost = value;
		}
	}

	public int ScaleFactorSqr
	{
		get
		{
			return _scaleFactorSqr;
		}
		set
		{
			_scaleFactorSqr = value;
		}
	}

	public float ScaleFactorSqrBoost
	{
		get
		{
			return _scaleFactorSqrBoost;
		}
		set
		{
			_scaleFactorSqrBoost = value;
		}
	}

	public int MaxFFTData => _maxFFTData;

	public int MaxFFTSampleIndex => _maxFFTSampleIndex;

	public BASSData MaxFFT
	{
		get
		{
			return _maxFFT;
		}
		set
		{
			switch (value)
			{
			case BASSData.BASS_DATA_FFT8192:
				_maxFFTData = 8192;
				_maxFFT = value;
				_maxFFTSampleIndex = 4095;
				break;
			case BASSData.BASS_DATA_FFT4096:
				_maxFFTData = 4096;
				_maxFFT = value;
				_maxFFTSampleIndex = 2047;
				break;
			case BASSData.BASS_DATA_FFT2048:
				_maxFFTData = 2048;
				_maxFFT = value;
				_maxFFTSampleIndex = 1023;
				break;
			case BASSData.BASS_DATA_FFT1024:
				_maxFFTData = 1024;
				_maxFFT = value;
				_maxFFTSampleIndex = 511;
				break;
			case BASSData.BASS_DATA_FFT512:
				_maxFFTData = 1024;
				_maxFFT = value;
				_maxFFTSampleIndex = 255;
				break;
			default:
				_maxFFTData = 4096;
				_maxFFT = BASSData.BASS_DATA_FFT4096;
				_maxFFTSampleIndex = 2047;
				break;
			}
			if (_maxFrequencySpectrum > _maxFFTSampleIndex)
			{
				_maxFrequencySpectrum = _maxFFTSampleIndex;
			}
		}
	}

	public int MaxFrequencySpectrum
	{
		get
		{
			return _maxFrequencySpectrum;
		}
		set
		{
			if (value > MaxFFTSampleIndex)
			{
				_maxFrequencySpectrum = MaxFFTSampleIndex;
			}
			if (value < 1)
			{
				_maxFrequencySpectrum = 1;
			}
			else
			{
				_maxFrequencySpectrum = value;
			}
		}
	}

	public int GetFrequencyFromPosX(int x, int samplerate)
	{
		int num = (int)Math.Round((double)(x + 1) * spp);
		if (num > MaxFFTSampleIndex)
		{
			num = MaxFFTSampleIndex;
		}
		if (num < 1)
		{
			num = 1;
		}
		return Utils.FFTIndex2Frequency(num, MaxFFTData, samplerate);
	}

	public float GetAmplitudeFromPosY(int y, int height)
	{
		float num = 0f;
		y = height - y;
		if (ylinear)
		{
			return (float)y / ((float)ScaleFactorLinear * (float)height);
		}
		return (float)Math.Pow((float)y / ((float)ScaleFactorSqr * (float)height), 2.0);
	}

	public Bitmap CreateSpectrum(int channel, int width, int height, Color color1, Color color2, Color background, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		Pen pen = null;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, 1, height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.AssumeLinear;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrum(graphics, width, height, pen, background, linear, fullSpectrum);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			pen?.Dispose();
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateSpectrum(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color background, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (g == null || channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1)
		{
			return false;
		}
		Pen pen = null;
		bool result = true;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, 1, clipRectangle.Height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.AssumeLinear;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrum(g, clipRectangle.Width, clipRectangle.Height, pen, background, linear, fullSpectrum);
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
		}
		return result;
	}

	public Bitmap CreateSpectrumLine(int channel, int width, int height, Color color1, Color color2, Color background, int linewidth, int distance, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1 || linewidth < 1 || distance < 0)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		Pen pen = null;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, linewidth, height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, linewidth);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.AssumeLinear;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumLine(graphics, width, height, pen, linewidth, distance, background, linear, fullSpectrum);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			pen?.Dispose();
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateSpectrumLine(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color background, int linewidth, int distance, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (g == null || channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || linewidth < 1 || distance < 0)
		{
			return false;
		}
		Pen pen = null;
		bool result = true;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, linewidth, clipRectangle.Height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, linewidth);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.AssumeLinear;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumLine(g, clipRectangle.Width, clipRectangle.Height, pen, linewidth, distance, background, linear, fullSpectrum);
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
		}
		return result;
	}

	public Bitmap CreateSpectrumEllipse(int channel, int width, int height, Color color1, Color color2, Color background, int linewidth, int distance, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1 || linewidth < 1 || distance < 0)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		Pen pen = null;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, linewidth, height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, linewidth);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.GammaCorrected;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumEllipse(graphics, width, height, pen, 2 * distance, background, linear, fullSpectrum);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			pen?.Dispose();
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateSpectrumEllipse(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color background, int linewidth, int distance, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (g == null || channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || linewidth < 1 || distance < 0)
		{
			return false;
		}
		Pen pen = null;
		bool result = true;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, linewidth, clipRectangle.Height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, linewidth);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.GammaCorrected;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumEllipse(g, clipRectangle.Width, clipRectangle.Height, pen, 2 * distance, background, linear, fullSpectrum);
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
		}
		return result;
	}

	public Bitmap CreateSpectrumDot(int channel, int width, int height, Color color1, Color color2, Color background, int dotwidth, int distance, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1 || dotwidth < 1 || distance < 0)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		Pen pen = null;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, dotwidth, height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, dotwidth);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.AssumeLinear;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumDot(graphics, width, height, pen, dotwidth, distance, background, linear, fullSpectrum);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			pen?.Dispose();
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateSpectrumDot(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color background, int dotwidth, int distance, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || dotwidth < 1 || distance < 0)
		{
			return false;
		}
		Pen pen = null;
		bool result = true;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, dotwidth, clipRectangle.Height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, dotwidth);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.AssumeLinear;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumDot(g, clipRectangle.Width, clipRectangle.Height, pen, dotwidth, distance, background, linear, fullSpectrum);
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
		}
		return result;
	}

	public Bitmap CreateSpectrumLinePeak(int channel, int width, int height, Color color1, Color color2, Color color3, Color background, int linewidth, int peakwidth, int distance, int peakdelay, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1 || linewidth < 1 || distance < 0 || peakdelay < 0)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		Pen pen = null;
		Pen pen2 = null;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, linewidth, height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, linewidth);
				pen2 = new Pen(color3, peakwidth);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.AssumeLinear;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumLinePeak(graphics, width, height, pen, pen2, linewidth, distance, peakdelay, background, linear, fullSpectrum);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			pen?.Dispose();
			pen2?.Dispose();
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateSpectrumLinePeak(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color color3, Color background, int linewidth, int peakwidth, int distance, int peakdelay, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (g == null || channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || linewidth < 1 || distance < 0 || peakdelay < 0)
		{
			return false;
		}
		Pen pen = null;
		Pen pen2 = null;
		bool result = true;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, linewidth, clipRectangle.Height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, linewidth);
				pen2 = new Pen(color3, peakwidth);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.AssumeLinear;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumLinePeak(g, clipRectangle.Width, clipRectangle.Height, pen, pen2, linewidth, distance, peakdelay, background, linear, fullSpectrum);
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
			pen2?.Dispose();
		}
		return result;
	}

	public void ClearPeaks()
	{
		for (int i = 0; i < _lastPeak.Length; i++)
		{
			_lastPeak[i] = 0f;
		}
	}

	public Bitmap CreateSpectrumWave(int channel, int width, int height, Color color1, Color color2, Color background, int linewidth, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1 || linewidth < 1)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		Pen pen = null;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, linewidth, height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, linewidth);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.AssumeLinear;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumWave(graphics, width, height, pen, background, linear, fullSpectrum);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			pen?.Dispose();
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateSpectrumWave(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color background, int linewidth, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || linewidth < 1)
		{
			return false;
		}
		Pen pen = null;
		bool result = true;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, linewidth, clipRectangle.Height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, linewidth);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.AssumeLinear;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumWave(g, clipRectangle.Width, clipRectangle.Height, pen, background, linear, fullSpectrum);
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
		}
		return result;
	}

	public Bitmap CreateWaveForm(int channel, int width, int height, Color color1, Color color2, Color color3, Color background, int linewidth, bool fullSpectrum, bool mono, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1 || linewidth < 1)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		Pen pen = null;
		Pen pen2 = null;
		Pen pen3 = null;
		try
		{
			if (GetFFTData(channel, MaxFFTData * 2 + 1073741824) > 0)
			{
				pen = new Pen(color1, linewidth);
				pen2 = new Pen(color2, linewidth);
				pen3 = new Pen(color3, 1f);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.AssumeLinear;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawWaveForm(graphics, width, height, pen, pen2, pen3, background, fullSpectrum, mono);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			pen?.Dispose();
			pen2?.Dispose();
			pen3?.Dispose();
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateWaveForm(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color color3, Color background, int linewidth, bool fullSpectrum, bool mono, bool highQuality)
	{
		if (channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || linewidth < 1)
		{
			return false;
		}
		Pen pen = null;
		Pen pen2 = null;
		Pen pen3 = null;
		bool result = true;
		try
		{
			if (GetFFTData(channel, MaxFFTData * 2 + 1073741824) > 0)
			{
				pen = new Pen(color1, linewidth);
				pen2 = new Pen(color2, linewidth);
				pen3 = new Pen(color3, 1f);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.AssumeLinear;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawWaveForm(g, clipRectangle.Width, clipRectangle.Height, pen, pen2, pen3, background, fullSpectrum, mono);
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
			pen2?.Dispose();
			pen3?.Dispose();
		}
		return result;
	}

	public Bitmap CreateSpectrumBean(int channel, int width, int height, Color color1, Color color2, Color background, int beanwidth, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1 || beanwidth < 1)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		Pen pen = null;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, beanwidth, height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, 2f);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.GammaCorrected;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumBean(graphics, width, height, pen, beanwidth, background, linear, fullSpectrum);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			pen?.Dispose();
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateSpectrumBean(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color background, int beanwidth, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || beanwidth < 1)
		{
			return false;
		}
		Pen pen = null;
		bool result = true;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush brush = new LinearGradientBrush(new Rectangle(0, 0, beanwidth, clipRectangle.Height), color2, color1, LinearGradientMode.Vertical);
				pen = new Pen(brush, 2f);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.GammaCorrected;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumBean(g, clipRectangle.Width, clipRectangle.Height, pen, beanwidth, background, linear, fullSpectrum);
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
		}
		return result;
	}

	public Bitmap CreateSpectrumText(int channel, int width, int height, Color color1, Color color2, Color background, string text, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || width <= 1 || height <= 1 || text == null)
		{
			return null;
		}
		Bitmap bitmap = null;
		Graphics graphics = null;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush b = new LinearGradientBrush(new Rectangle(0, 0, width, height), color2, color1, LinearGradientMode.Vertical);
				bitmap = new Bitmap(width, height);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.GammaCorrected;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.CompositingQuality = CompositingQuality.HighSpeed;
					graphics.PixelOffsetMode = PixelOffsetMode.None;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumText(graphics, width, height, b, text, background, linear, fullSpectrum);
			}
		}
		catch
		{
			bitmap = null;
		}
		finally
		{
			graphics?.Dispose();
		}
		return bitmap;
	}

	public bool CreateSpectrumText(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, Color background, string text, bool linear, bool fullSpectrum, bool highQuality)
	{
		if (channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || text == null)
		{
			return false;
		}
		bool result = true;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				using Brush b = new LinearGradientBrush(new Rectangle(0, 0, clipRectangle.Height, clipRectangle.Height), color2, color1, LinearGradientMode.Vertical);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.GammaCorrected;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.None;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawSpectrumText(g, clipRectangle.Width, clipRectangle.Height, b, text, background, linear, fullSpectrum);
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public float DetectFrequency(int channel, int freq1, int freq2, bool linear)
	{
		if (freq1 < 1 || freq2 < 1 || freq1 > freq2 || channel == 0)
		{
			return 0f;
		}
		float num = 0f;
		int num2 = 1;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
				if (Bass.BASS_ChannelGetInfo(channel, bASS_CHANNELINFO))
				{
					int num3 = Utils.FFTFrequency2Index(freq1, MaxFFTData, bASS_CHANNELINFO.freq);
					int num4 = Utils.FFTFrequency2Index(freq2, MaxFFTData, bASS_CHANNELINFO.freq);
					num2 = num4 - num3 + 1;
					for (int i = num3; i <= num4; i++)
					{
						num = ((!linear) ? (num + (float)Math.Sqrt(_fft[i]) * (float)ScaleFactorSqr) : (num + _fft[i] * (float)ScaleFactorLinear));
					}
				}
			}
		}
		catch
		{
			return 0f;
		}
		return num / (float)num2;
	}

	public int DetectPeakFrequency(int channel, out float energy)
	{
		energy = 0f;
		if (channel == 0)
		{
			return 0;
		}
		int result = 0;
		int index = 0;
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
				if (Bass.BASS_ChannelGetInfo(channel, bASS_CHANNELINFO))
				{
					for (int i = 1; i <= MaxFFTSampleIndex; i++)
					{
						if (_fft[i] > energy)
						{
							energy = _fft[i];
							index = i;
						}
					}
					result = Utils.FFTIndex2Frequency(index, MaxFFTData, bASS_CHANNELINFO.freq);
				}
			}
		}
		catch
		{
			return 0;
		}
		return result;
	}

	public bool CreateSpectrum3DVoicePrint(int channel, Graphics g, Rectangle clipRectangle, Color color1, Color color2, int pos, bool linear, bool fullSpectrum)
	{
		if (channel == 0 || clipRectangle.Width <= 1 || clipRectangle.Height <= 1)
		{
			return false;
		}
		if (pos > clipRectangle.Width - 1)
		{
			pos = 0;
		}
		bool result = true;
		Pen pen = new Pen(color1, 1f);
		Pen pen2 = new Pen(color2, 1f);
		try
		{
			if (GetFFTData(channel, (int)MaxFFT) > 0)
			{
				float num = 0f;
				float num2 = 0f;
				int num3 = 0;
				int num4 = 0;
				double num5 = (double)MaxFrequencySpectrum / (double)clipRectangle.Height;
				int num6 = MaxFrequencySpectrum + 1;
				if (!fullSpectrum)
				{
					num5 = 1.0;
					num6 = clipRectangle.Height + 1;
					if (num6 > MaxFFTSampleIndex + 1)
					{
						num6 = MaxFFTSampleIndex + 1;
					}
				}
				for (int i = 1; i < num6; i++)
				{
					if (linear)
					{
						num3 = (int)(_fft[i] * (float)ScaleFactorLinear * 65535f);
						if (num4 < num3)
						{
							num4 = num3;
						}
						if (num4 < 0)
						{
							num4 = 0;
						}
					}
					else
					{
						num3 = (int)(Math.Sqrt(_fft[i]) * (double)ScaleFactorSqr * 65535.0);
						if (num4 < num3)
						{
							num4 = num3;
						}
						if (num4 < 0)
						{
							num4 = 0;
						}
					}
					pen.Color = Color.FromArgb((color1.R + ((num4 & 0xFFFFFF) >> 16)) & 0xFF, (num4 & 0xFFFF) >> 8, num4 & 0xFF);
					num = (float)Math.Round((double)i / num5) - 1f;
					if (num > num2)
					{
						g.DrawLine(pen, pos, num2, pos, num2 + 1f);
						num2 = num;
						num4 = 0;
					}
				}
				if (pos < clipRectangle.Width - 1)
				{
					g.DrawLine(pen2, (float)pos + 1f, 0f, (float)pos + 1f, clipRectangle.Height);
				}
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			pen?.Dispose();
			pen2?.Dispose();
		}
		return result;
	}

	private int GetFFTData(int channel, int length)
	{
		if (_channelIsMixerSource)
		{
			return BassMix.BASS_Mixer_ChannelGetData(channel, _fft, length);
		}
		return Bass.BASS_ChannelGetData(channel, _fft, length);
	}

	private void DrawSpectrum(Graphics g, int width, int height, Pen p, Color background, bool linear, bool fullSpectrum)
	{
		g.Clear(background);
		ylinear = linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		spp = (double)MaxFrequencySpectrum / (double)width;
		int num5 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num5 = width + 1;
			if (num5 > MaxFFTSampleIndex + 1)
			{
				num5 = MaxFFTSampleIndex + 1;
			}
		}
		int num6 = 0;
		float num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
		for (int i = 1; i < num5; i++)
		{
			num3 = ((!linear) ? ((float)Math.Sqrt(_fft[i] * num7) * (float)ScaleFactorSqr * (float)height) : (_fft[i] * (float)ScaleFactorLinear * num7 * (float)height));
			num4 += num3;
			num = (float)Math.Round((double)i / spp) - 1f;
			if (num > num2)
			{
				num4 /= num - num2;
				if (num4 > (float)height)
				{
					num4 = height;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				g.DrawLine(p, num2, (float)height - 1f, num2, (float)height - 1f - num4);
				num2 = num;
				num4 = 0f;
				num6++;
				num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
			}
		}
	}

	private void DrawSpectrumLine(Graphics g, int width, int height, Pen p, int linewidth, int distance, Color background, bool linear, bool fullSpectrum)
	{
		g.Clear(background);
		ylinear = linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		spp = (double)MaxFrequencySpectrum / (double)width;
		int num5 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num5 = width + 1;
			if (num5 > MaxFFTSampleIndex + 1)
			{
				num5 = MaxFFTSampleIndex + 1;
			}
		}
		int num6 = 0;
		float num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
		for (int i = 1; i < num5; i++)
		{
			num3 = ((!linear) ? ((float)Math.Sqrt(_fft[i] * num7) * (float)ScaleFactorSqr * (float)height - 4f) : (_fft[i] * (float)ScaleFactorLinear * num7 * (float)height));
			num4 += num3;
			num = (float)Math.Round((double)i / spp) - 1f;
			if ((int)num % (distance + linewidth) == 0 && num > num2)
			{
				num4 /= (float)(distance + linewidth);
				if (num4 > (float)height)
				{
					num4 = height;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				g.DrawLine(p, num2 + (float)(linewidth / 2) + 1f, (float)height - 1f, num2 + (float)(linewidth / 2) + 1f, (float)height - 1f - num4);
				num2 = num;
				num4 = 0f;
				num6++;
				num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
			}
		}
	}

	private void DrawSpectrumEllipse(Graphics g, int width, int height, Pen p, int distance, Color background, bool linear, bool fullSpectrum)
	{
		g.Clear(background);
		ylinear = linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		spp = (double)MaxFrequencySpectrum / (double)width;
		int num5 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num5 = width + 1;
			if (num5 > MaxFFTSampleIndex + 1)
			{
				num5 = MaxFFTSampleIndex + 1;
			}
		}
		int num6 = 0;
		float num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
		for (int i = 1; i < num5; i++)
		{
			num3 = ((!linear) ? ((float)Math.Sqrt(_fft[i] * num7) * (float)ScaleFactorSqr * (float)height - 4f) : (_fft[i] * (float)ScaleFactorLinear * num7 * (float)height));
			num4 += num3;
			num = (float)Math.Round((double)i / spp) - 1f;
			if ((int)num % distance == 0 && num > num2)
			{
				num4 /= (float)distance;
				if (num4 > (float)height)
				{
					num4 = height;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				g.DrawEllipse(p, num2, (float)height - 1f - num4, num2 + (float)distance, (float)height - 1f);
				num2 = num;
				num4 = 0f;
				num6++;
				num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
			}
		}
	}

	private void DrawSpectrumDot(Graphics g, int width, int height, Pen p, int dotwidth, int distance, Color background, bool linear, bool fullSpectrum)
	{
		g.Clear(background);
		ylinear = linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		spp = (double)MaxFrequencySpectrum / (double)width;
		int num5 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num5 = width + 1;
			if (num5 > MaxFFTSampleIndex + 1)
			{
				num5 = MaxFFTSampleIndex + 1;
			}
		}
		int num6 = 0;
		float num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
		for (int i = 1; i < num5; i++)
		{
			num3 = ((!linear) ? ((float)Math.Sqrt(_fft[i] * num7) * (float)ScaleFactorSqr * (float)height - 4f) : (_fft[i] * (float)ScaleFactorLinear * num7 * (float)height));
			num4 += num3;
			num = (float)Math.Round((double)i / spp) - 1f;
			if ((int)num % (distance + dotwidth) == 0 && num > num2)
			{
				num4 /= (float)(distance + dotwidth);
				if (num4 > (float)height)
				{
					num4 = height;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				g.DrawLine(p, num2 + (float)distance, (float)height - (float)dotwidth - num4, num2 + (float)distance, (float)height - num4);
				num2 = num;
				num4 = 0f;
				num6++;
				num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
			}
		}
	}

	private void DrawSpectrumLinePeak(Graphics g, int width, int height, Pen p1, Pen p2, int linewidth, int distance, int peakdelay, Color background, bool linear, bool fullSpectrum)
	{
		g.Clear(background);
		ylinear = linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		spp = (double)MaxFrequencySpectrum / (double)width;
		int num6 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num6 = width + 1;
			if (num6 > MaxFFTSampleIndex + 1)
			{
				num6 = MaxFFTSampleIndex + 1;
			}
		}
		int num7 = 0;
		float num8 = 1f + (float)num7 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
		for (int i = 1; i < num6; i++)
		{
			num3 = ((!linear) ? ((float)Math.Sqrt(_fft[i] * num8) * (float)ScaleFactorSqr * (float)height - 4f) : (_fft[i] * (float)ScaleFactorLinear * num8 * (float)height));
			num4 += num3;
			num = (float)Math.Round((double)i / spp) - 1f;
			if ((int)num % (distance + linewidth) == 0 && num > num2)
			{
				num4 /= (float)(distance + linewidth);
				if (num4 > (float)height)
				{
					num4 = height;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				num5 = num4;
				if (_lastPeak[(int)num] < num5)
				{
					_lastPeak[(int)num] = num5;
				}
				else
				{
					_lastPeak[(int)num] = (num5 + (float)peakdelay * _lastPeak[(int)num]) / (float)(peakdelay + 1);
				}
				g.DrawLine(p1, num2 + (float)(linewidth / 2) + 1f, (float)height - 1f, num2 + (float)(linewidth / 2) + 1f, (float)height - 1f - num4);
				float num9 = ((!(p2.Width > 1f)) ? (-1) : 0);
				g.DrawLine(p2, num2 + 1f, (float)height - p2.Width / 2f - _lastPeak[(int)num], num2 + (float)linewidth + num9 + 1f, (float)height - p2.Width / 2f - _lastPeak[(int)num]);
				num2 = num;
				num4 = 0f;
				num7++;
				num8 = 1f + (float)num7 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
			}
		}
	}

	private void DrawSpectrumWave(Graphics g, int width, int height, Pen p, Color background, bool linear, bool fullSpectrum)
	{
		g.Clear(background);
		ylinear = linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		spp = (double)MaxFrequencySpectrum / (double)width;
		int num6 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num6 = width + 1;
			if (num6 > MaxFFTSampleIndex + 1)
			{
				num6 = MaxFFTSampleIndex + 1;
			}
		}
		int num7 = 0;
		float num8 = 1f + (float)num7 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
		for (int i = 1; i < num6; i++)
		{
			num3 = ((!linear) ? ((float)Math.Sqrt(_fft[i] * num8) * (float)ScaleFactorSqr * (float)height - 4f) : (_fft[i] * (float)ScaleFactorLinear * num8 * (float)height));
			num4 += num3;
			num = (float)Math.Round((double)i / spp) - 1f;
			if (num > num2)
			{
				num4 /= num - num2;
				if (num4 > (float)height)
				{
					num4 = height;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				g.DrawLine(p, num, (float)height - num4, num2, (float)height - num5);
				num5 = num4;
				num2 = num;
				num4 = 0f;
				num7++;
				num8 = 1f + (float)num7 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
			}
		}
	}

	private void DrawWaveForm(Graphics g, int width, int height, Pen pL, Pen pR, Pen pM, Color background, bool fullSpectrum, bool mono)
	{
		g.Clear(background);
		ylinear = true;
		float num = (float)height / 2f - 1f;
		float num2 = 0f;
		float num3 = -1f;
		float num4 = 0f;
		float num5 = -1f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		float num11 = 0f;
		g.DrawLine(pM, 0f, num, width, num);
		spp = (double)(MaxFrequencySpectrum + 1) / (double)width;
		int num12 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num12 = width + 1;
			if (num12 > MaxFFTSampleIndex + 1)
			{
				num12 = MaxFFTSampleIndex + 1;
			}
		}
		for (int i = 0; i < num12; i++)
		{
			if (mono)
			{
				num9 = _fft[i] * num;
				num6 = _fft[i + 1] * num;
				num9 = (num9 + num6) / 2f;
				if (Math.Abs(num10) < Math.Abs(num9))
				{
					num10 = num9;
				}
				num2 = (float)Math.Round((double)i / spp) - 1f;
				if (num3 == -1f)
				{
					num11 = num9;
				}
				if (num2 > num3)
				{
					g.DrawLine(pL, num2, num - num10, num3, num - num11);
					num11 = num10;
					num3 = num2;
					num10 = 0f;
				}
				i++;
			}
			else if (i % 2 == 0)
			{
				num9 = _fft[i] * num;
				if (Math.Abs(num10) < Math.Abs(num9))
				{
					num10 = num9;
				}
				num2 = (float)Math.Round((double)i / spp) - 1f;
				if (num3 == -1f)
				{
					num11 = num9;
				}
				if (num2 > num3)
				{
					g.DrawLine(pL, num2, num - num10, num3, num - num11);
					num11 = num10;
					num3 = num2;
					num10 = 0f;
				}
			}
			else
			{
				num6 = _fft[i] * num;
				if (Math.Abs(num7) < Math.Abs(num6))
				{
					num7 = num6;
				}
				num4 = (float)Math.Round((double)(i - 1) / spp) - 1f;
				if (num5 == -1f)
				{
					num8 = num6;
				}
				if (num4 > num5)
				{
					g.DrawLine(pR, num4, num - num7, num5, num - num8);
					num8 = num7;
					num5 = num4;
					num7 = 0f;
				}
			}
		}
	}

	private void DrawSpectrumBean(Graphics g, int width, int height, Pen p, int beanwidth, Color background, bool linear, bool fullSpectrum)
	{
		g.Clear(background);
		ylinear = linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		spp = (double)MaxFrequencySpectrum / (double)width;
		int num5 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num5 = width + 1;
			if (num5 > MaxFFTSampleIndex + 1)
			{
				num5 = MaxFFTSampleIndex + 1;
			}
		}
		int num6 = 0;
		float num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
		for (int i = 1; i < num5; i++)
		{
			num3 = ((!linear) ? ((float)Math.Sqrt(_fft[i] * num7) * (float)ScaleFactorSqr * (float)height - 4f) : (_fft[i] * (float)ScaleFactorLinear * num7 * (float)height));
			num4 += num3;
			num = (float)Math.Round((double)i / spp) - 1f;
			if ((int)num % (beanwidth + 1) == 0 && num > num2)
			{
				if (num4 > (float)height)
				{
					num4 = height;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				g.DrawEllipse(p, num2 + 1f, (float)height - num4, beanwidth, 2 * beanwidth);
				num2 = num;
				num4 = 0f;
				num6++;
				num7 = 1f + (float)num6 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
			}
		}
	}

	private void DrawSpectrumText(Graphics g, int width, int height, Brush b, string text, Color background, bool linear, bool fullSpectrum)
	{
		g.Clear(background);
		ylinear = linear;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		int num5 = 0;
		spp = (double)MaxFrequencySpectrum / (double)width;
		int num6 = MaxFrequencySpectrum + 1;
		if (!fullSpectrum)
		{
			spp = 1.0;
			num6 = width + 1;
			if (num6 > MaxFFTSampleIndex + 1)
			{
				num6 = MaxFFTSampleIndex + 1;
			}
		}
		int num7 = 0;
		float num8 = 1f + (float)num7 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
		for (int i = 1; i < num6; i++)
		{
			num3 = ((!linear) ? ((float)Math.Sqrt(_fft[i] * num8) * (float)ScaleFactorSqr * (float)height - 4f) : (_fft[i] * (float)ScaleFactorLinear * num8 * (float)height));
			num4 += num3;
			num = (float)Math.Round((double)i / spp) - 1f;
			if ((int)num % 6 == 0 && num > num2)
			{
				num4 /= 6f;
				if (num4 > (float)height)
				{
					num4 = height;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				g.DrawString(text[num5].ToString(), SystemInformation.MenuFont, b, num2 + 1f, (float)height - 15f - num4, StringFormat.GenericDefault);
				num2 = num;
				num4 = 0f;
				num5++;
				if (num5 >= text.Length)
				{
					num5 = 0;
				}
				num7++;
				num8 = 1f + (float)num7 * (linear ? ScaleFactorLinearBoost : ScaleFactorSqrBoost);
			}
		}
	}
}

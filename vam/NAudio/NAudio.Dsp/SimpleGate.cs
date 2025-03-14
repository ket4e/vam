using System;
using NAudio.Utils;

namespace NAudio.Dsp;

internal class SimpleGate : AttRelEnvelope
{
	private double threshdB;

	private double thresh;

	private double env;

	public double Threshold
	{
		get
		{
			return threshdB;
		}
		set
		{
			threshdB = value;
			thresh = Decibels.DecibelsToLinear(value);
		}
	}

	public SimpleGate()
		: base(10.0, 10.0, 44100.0)
	{
		threshdB = 0.0;
		thresh = 1.0;
		env = 1E-25;
	}

	public void Process(ref double in1, ref double in2)
	{
		double val = Math.Abs(in1);
		double val2 = Math.Abs(in2);
		double num = ((Math.Max(val, val2) > thresh) ? 1.0 : 0.0);
		num += 1E-25;
		Run(num, ref env);
		num = env - 1E-25;
		in1 *= num;
		in2 *= num;
	}
}

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

public sealed class EncoderParameters : IDisposable
{
	private EncoderParameter[] parameters;

	public EncoderParameter[] Param
	{
		get
		{
			return parameters;
		}
		set
		{
			parameters = value;
		}
	}

	public EncoderParameters()
	{
		parameters = new EncoderParameter[1];
	}

	public EncoderParameters(int count)
	{
		parameters = new EncoderParameter[count];
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	internal IntPtr ToNativePtr()
	{
		IntPtr intPtr = Marshal.AllocHGlobal(4 + parameters.Length * EncoderParameter.NativeSize());
		IntPtr ptr = intPtr;
		Marshal.WriteInt32(ptr, parameters.Length);
		ptr = (IntPtr)(ptr.ToInt64() + 4);
		for (int i = 0; i < parameters.Length; i++)
		{
			parameters[i].ToNativePtr(ptr);
			ptr = (IntPtr)(ptr.ToInt64() + EncoderParameter.NativeSize());
		}
		return intPtr;
	}

	internal static EncoderParameters FromNativePtr(IntPtr epPtr)
	{
		if (epPtr == IntPtr.Zero)
		{
			return null;
		}
		IntPtr ptr = epPtr;
		int num = Marshal.ReadInt32(ptr);
		ptr = (IntPtr)(ptr.ToInt64() + 4);
		if (num == 0)
		{
			return null;
		}
		EncoderParameters encoderParameters = new EncoderParameters(num);
		for (int i = 0; i < num; i++)
		{
			encoderParameters.parameters[i] = EncoderParameter.FromNativePtr(ptr);
			ptr = (IntPtr)(ptr.ToInt64() + EncoderParameter.NativeSize());
		}
		return encoderParameters;
	}
}

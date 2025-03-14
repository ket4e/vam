using System.Drawing;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace Un4seen.Bass;

[SuppressUnmanagedCodeSecurity]
public sealed class BassNet
{
	internal static string _eMail;

	internal static string _registrationKey;

	internal static string _internalName;

	public static bool OmitCheckVersion;

	private static bool _useBrokenLatin1;

	private static bool _useRiffInfoUTF8;

	public static string InternalName => _internalName;

	public static bool UseBrokenLatin1Behavior
	{
		get
		{
			return _useBrokenLatin1;
		}
		set
		{
			_useBrokenLatin1 = value;
		}
	}

	public static bool UseRiffInfoUTF8
	{
		get
		{
			return _useRiffInfoUTF8;
		}
		set
		{
			_useRiffInfoUTF8 = value;
		}
	}

	private BassNet()
	{
	}

	static BassNet()
	{
		_eMail = string.Empty;
		_registrationKey = string.Empty;
		_internalName = "BASS.NET";
		OmitCheckVersion = false;
		_useBrokenLatin1 = false;
		_useRiffInfoUTF8 = false;
		AssemblyName name = Assembly.GetExecutingAssembly().GetName();
		_internalName = $"{name.Name} v{name.Version}";
	}

	public static void Registration(string eMail, string registrationKey)
	{
		_eMail = eMail;
		_registrationKey = registrationKey;
	}

	public static void ShowSplash(Form owner, int wait, double opacity, int pos)
	{
		SplashScreen splashScreen = new SplashScreen(close: false, wait);
		splashScreen.SetOpacity(opacity);
		splashScreen.SetPosition(pos);
		if (owner != null && pos == 2)
		{
			splashScreen.StartPosition = FormStartPosition.Manual;
			Point location = owner.Location;
			location.Offset(owner.Width / 2 - splashScreen.Width / 2, owner.Height / 2 - splashScreen.Height / 2);
			splashScreen.Location = location;
		}
		if (wait <= 0)
		{
			splashScreen.SetClose(close: false);
		}
		else
		{
			splashScreen.SetClose(close: true);
		}
		splashScreen.Show();
		Application.DoEvents();
		if (wait > 0)
		{
			ThreadPool.QueueUserWorkItem(WaitMe, splashScreen);
		}
	}

	public static void ShowAbout(Form owner)
	{
		SplashScreen splashScreen = new SplashScreen(close: false, 0);
		if (owner != null)
		{
			splashScreen.SetPosition(2);
		}
		splashScreen.ShowDialog(owner);
	}

	private static void WaitMe(object splash)
	{
		if (splash != null && splash is SplashScreen splashScreen)
		{
			Thread.Sleep(splashScreen._waitTime);
			if (!splashScreen.IsDisposed)
			{
				splashScreen.Invoke(new MethodInvoker(splashScreen.Close));
			}
			splashScreen.Dispose();
			SplashScreen splashScreen2 = null;
			splash = null;
		}
	}
}

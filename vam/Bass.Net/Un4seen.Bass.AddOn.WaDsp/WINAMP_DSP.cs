using System.Collections.Generic;
using System.IO;

namespace Un4seen.Bass.AddOn.WaDsp;

public sealed class WINAMP_DSP
{
	private string _file = string.Empty;

	private string _description = string.Empty;

	private int _modulecount;

	private string[] _modulenames;

	private int _plugin;

	private int _dsp;

	private int _module = -1;

	private static List<WINAMP_DSP> _plugins = new List<WINAMP_DSP>();

	public string File => _file;

	public string Description => _description;

	public int ModuleCount => _modulecount;

	public string[] ModuleNames => _modulenames;

	public bool IsLoaded => _plugin != 0;

	public bool IsStarted => _dsp != 0;

	public int StartedModule => _module;

	public static List<WINAMP_DSP> PlugIns => _plugins;

	private WINAMP_DSP()
	{
	}

	public WINAMP_DSP(string fileName)
	{
		_file = fileName;
		if (BassWaDsp.BASS_WADSP_PluginInfoLoad(_file))
		{
			_description = BassWaDsp.BASS_WADSP_PluginInfoGetName();
			_modulecount = BassWaDsp.BASS_WADSP_PluginInfoGetModuleCount();
			_modulenames = BassWaDsp.BASS_WADSP_PluginInfoGetModuleNames();
			BassWaDsp.BASS_WADSP_PluginInfoFree();
		}
	}

	public bool Load()
	{
		if (IsLoaded)
		{
			BassWaDsp.BASS_WADSP_FreeDSP(_plugin);
		}
		_plugin = BassWaDsp.BASS_WADSP_Load(_file, 5, 5, 100, 100, null);
		return _plugin != 0;
	}

	public bool Unload()
	{
		if (IsStarted)
		{
			Stop();
		}
		if (IsLoaded)
		{
			BassWaDsp.BASS_WADSP_FreeDSP(_plugin);
			_plugin = 0;
			return true;
		}
		return false;
	}

	public int Start(int module, int channel, int prio)
	{
		if (IsLoaded && module >= 0 && module < _modulecount && channel != 0)
		{
			if (IsStarted)
			{
				Stop();
			}
			_dsp = 0;
			_module = -1;
			BassWaDsp.BASS_WADSP_Start(_plugin, module, channel);
			_module = module;
			_dsp = BassWaDsp.BASS_WADSP_ChannelSetDSP(_plugin, channel, prio);
			return _dsp;
		}
		return 0;
	}

	public bool Stop()
	{
		if (IsLoaded)
		{
			if (IsStarted && !BassWaDsp.BASS_WADSP_ChannelRemoveDSP(_plugin))
			{
				return false;
			}
			if (BassWaDsp.BASS_WADSP_Stop(_plugin))
			{
				_dsp = 0;
				_module = -1;
				return true;
			}
			return false;
		}
		return false;
	}

	public bool SetSongTitle(string title)
	{
		if (IsLoaded && IsStarted)
		{
			return BassWaDsp.BASS_WADSP_SetSongTitle(_plugin, title);
		}
		return false;
	}

	public bool SetFilename(string filename)
	{
		if (IsLoaded && IsStarted)
		{
			return BassWaDsp.BASS_WADSP_SetFileName(_plugin, filename);
		}
		return false;
	}

	public bool ShowEditor()
	{
		if (IsLoaded && _module >= 0)
		{
			return BassWaDsp.BASS_WADSP_Config(_plugin);
		}
		return false;
	}

	public override string ToString()
	{
		return $"{_description} ({Path.GetFileNameWithoutExtension(_file)})";
	}

	public static void FindPlugins(string path)
	{
		string[] files = Directory.GetFiles(path, "dsp_*.dll");
		foreach (string text in files)
		{
			if (!containsPlugin(text))
			{
				WINAMP_DSP wINAMP_DSP = new WINAMP_DSP(text);
				if (wINAMP_DSP._modulecount > 0)
				{
					_plugins.Add(wINAMP_DSP);
				}
			}
		}
	}

	private static bool containsPlugin(string file)
	{
		bool result = false;
		foreach (WINAMP_DSP plugin in _plugins)
		{
			if (plugin._file.ToLower().Equals(file.ToLower()))
			{
				result = true;
				break;
			}
		}
		return result;
	}
}

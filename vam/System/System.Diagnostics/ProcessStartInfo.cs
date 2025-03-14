using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System.Diagnostics;

[TypeConverter(typeof(ExpandableObjectConverter))]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public sealed class ProcessStartInfo
{
	private string arguments = string.Empty;

	private IntPtr error_dialog_parent_handle = (IntPtr)0;

	private string filename = string.Empty;

	private string verb = string.Empty;

	private string working_directory = string.Empty;

	private System.Collections.Specialized.ProcessStringDictionary envVars;

	private bool create_no_window;

	private bool error_dialog;

	private bool redirect_standard_error;

	private bool redirect_standard_input;

	private bool redirect_standard_output;

	private bool use_shell_execute = true;

	private ProcessWindowStyle window_style;

	private Encoding encoding_stderr;

	private Encoding encoding_stdout;

	private string username;

	private string domain;

	private SecureString password;

	private bool load_user_profile;

	private static readonly string[] empty = new string[0];

	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[RecommendedAsConfigurable(true)]
	[DefaultValue("")]
	[MonitoringDescription("Command line agruments for this process.")]
	[NotifyParentProperty(true)]
	public string Arguments
	{
		get
		{
			return arguments;
		}
		set
		{
			arguments = value;
		}
	}

	[DefaultValue(false)]
	[MonitoringDescription("Start this process with a new window.")]
	[NotifyParentProperty(true)]
	public bool CreateNoWindow
	{
		get
		{
			return create_no_window;
		}
		set
		{
			create_no_window = value;
		}
	}

	[MonitoringDescription("Environment variables used for this process.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[DefaultValue(null)]
	[Editor("System.Diagnostics.Design.StringDictionaryEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[NotifyParentProperty(true)]
	public StringDictionary EnvironmentVariables
	{
		get
		{
			if (envVars == null)
			{
				envVars = new System.Collections.Specialized.ProcessStringDictionary();
				foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
				{
					envVars.Add((string)environmentVariable.Key, (string)environmentVariable.Value);
				}
			}
			return envVars;
		}
	}

	internal bool HaveEnvVars => envVars != null;

	[DefaultValue(false)]
	[MonitoringDescription("Thread shows dialogboxes for errors.")]
	[NotifyParentProperty(true)]
	public bool ErrorDialog
	{
		get
		{
			return error_dialog;
		}
		set
		{
			error_dialog = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IntPtr ErrorDialogParentHandle
	{
		get
		{
			return error_dialog_parent_handle;
		}
		set
		{
			error_dialog_parent_handle = value;
		}
	}

	[DefaultValue("")]
	[RecommendedAsConfigurable(true)]
	[Editor("System.Diagnostics.Design.StartFileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[MonitoringDescription("The name of the resource to start this process.")]
	[NotifyParentProperty(true)]
	public string FileName
	{
		get
		{
			return filename;
		}
		set
		{
			filename = value;
		}
	}

	[DefaultValue(false)]
	[NotifyParentProperty(true)]
	[MonitoringDescription("Errors of this process are redirected.")]
	public bool RedirectStandardError
	{
		get
		{
			return redirect_standard_error;
		}
		set
		{
			redirect_standard_error = value;
		}
	}

	[MonitoringDescription("Standard input of this process is redirected.")]
	[NotifyParentProperty(true)]
	[DefaultValue(false)]
	public bool RedirectStandardInput
	{
		get
		{
			return redirect_standard_input;
		}
		set
		{
			redirect_standard_input = value;
		}
	}

	[DefaultValue(false)]
	[MonitoringDescription("Standart output of this process is redirected.")]
	[NotifyParentProperty(true)]
	public bool RedirectStandardOutput
	{
		get
		{
			return redirect_standard_output;
		}
		set
		{
			redirect_standard_output = value;
		}
	}

	public Encoding StandardErrorEncoding
	{
		get
		{
			return encoding_stderr;
		}
		set
		{
			encoding_stderr = value;
		}
	}

	public Encoding StandardOutputEncoding
	{
		get
		{
			return encoding_stdout;
		}
		set
		{
			encoding_stdout = value;
		}
	}

	[NotifyParentProperty(true)]
	[MonitoringDescription("Use the shell to start this process.")]
	[DefaultValue(true)]
	public bool UseShellExecute
	{
		get
		{
			return use_shell_execute;
		}
		set
		{
			use_shell_execute = value;
		}
	}

	[MonitoringDescription("The verb to apply to a used document.")]
	[NotifyParentProperty(true)]
	[DefaultValue("")]
	[TypeConverter("System.Diagnostics.Design.VerbConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string Verb
	{
		get
		{
			return verb;
		}
		set
		{
			verb = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string[] Verbs
	{
		get
		{
			string text = ((!((filename == null) | (filename.Length == 0))) ? Path.GetExtension(filename) : null);
			if (text == null)
			{
				return empty;
			}
			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.Unix:
			case PlatformID.MacOSX:
			case (PlatformID)128:
				return empty;
			default:
			{
				RegistryKey registryKey = null;
				RegistryKey registryKey2 = null;
				RegistryKey registryKey3 = null;
				try
				{
					registryKey = Registry.ClassesRoot.OpenSubKey(text);
					string text2 = ((registryKey == null) ? null : (registryKey.GetValue(null) as string));
					registryKey2 = ((text2 == null) ? null : Registry.ClassesRoot.OpenSubKey(text2));
					registryKey3 = registryKey2?.OpenSubKey("shell");
					return registryKey3?.GetSubKeyNames();
				}
				finally
				{
					registryKey3?.Close();
					registryKey2?.Close();
					registryKey?.Close();
				}
			}
			}
		}
	}

	[NotifyParentProperty(true)]
	[DefaultValue(typeof(ProcessWindowStyle), "Normal")]
	[MonitoringDescription("The window style used to start this process.")]
	public ProcessWindowStyle WindowStyle
	{
		get
		{
			return window_style;
		}
		set
		{
			window_style = value;
		}
	}

	[NotifyParentProperty(true)]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[MonitoringDescription("The initial directory for this process.")]
	[Editor("System.Diagnostics.Design.WorkingDirectoryEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[RecommendedAsConfigurable(true)]
	[DefaultValue("")]
	public string WorkingDirectory
	{
		get
		{
			return working_directory;
		}
		set
		{
			working_directory = ((value != null) ? value : string.Empty);
		}
	}

	[NotifyParentProperty(true)]
	public bool LoadUserProfile
	{
		get
		{
			return load_user_profile;
		}
		set
		{
			load_user_profile = value;
		}
	}

	[NotifyParentProperty(true)]
	public string UserName
	{
		get
		{
			return username;
		}
		set
		{
			username = value;
		}
	}

	[NotifyParentProperty(true)]
	public string Domain
	{
		get
		{
			return domain;
		}
		set
		{
			domain = value;
		}
	}

	public SecureString Password
	{
		get
		{
			return password;
		}
		set
		{
			password = value;
		}
	}

	public ProcessStartInfo()
	{
	}

	public ProcessStartInfo(string filename)
	{
		this.filename = filename;
	}

	public ProcessStartInfo(string filename, string arguments)
	{
		this.filename = filename;
		this.arguments = arguments;
	}
}

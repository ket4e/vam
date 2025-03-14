using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MHLab.PATCH.Settings;

public class SettingsOverrider
{
	public string VERSIONS_FILE_DOWNLOAD_URL;

	public string PATCHES_DOWNLOAD_URL;

	public string BUILDS_DOWNLOAD_URL;

	public string PATCHER_DOWNLOAD_URL;

	public ushort PATCH_DOWNLOAD_RETRY_ATTEMPTS;

	public string PATCHNOTES_URL;

	public string NEWS_URL;

	public string LAUNCH_APP;

	public string LAUNCHER_NAME;

	public string LAUNCH_ARG;

	public bool USE_RAW_LAUNCH;

	public bool ENABLE_FTP;

	public string FTP_USERNAME;

	public string FTP_PASSWORD;

	public bool INSTALL_IN_LOCAL_PATH;

	public string PROGRAM_FILES_DIRECTORY_TO_INSTALL;

	public bool ENABLE_PATCHER;

	public bool ENABLE_INSTALLER;

	public bool ENABLE_REPAIRER;

	public bool INSTALL_PATCHER;

	public bool CREATE_DESKTOP_SHORTCUT;

	public SettingsOverrider()
	{
		ENABLE_PATCHER = true;
		ENABLE_INSTALLER = true;
		ENABLE_REPAIRER = true;
		CREATE_DESKTOP_SHORTCUT = true;
		VERSIONS_FILE_DOWNLOAD_URL = "http://your/url/to/versions.txt";
		PATCHES_DOWNLOAD_URL = "http://your/url/to/patches/directory/";
		BUILDS_DOWNLOAD_URL = "http://your/url/to/builds/directory/";
		PATCHER_DOWNLOAD_URL = "http://your/url/to/patcher/directory/";
		PATCH_DOWNLOAD_RETRY_ATTEMPTS = 0;
		ENABLE_FTP = false;
		FTP_USERNAME = "YourFTPUsernameHere";
		FTP_PASSWORD = "YourFTPPasswordHere";
		LAUNCH_APP = "Build.exe";
		LAUNCHER_NAME = "PATCH.exe";
		LAUNCH_ARG = "default";
		USE_RAW_LAUNCH = false;
		INSTALL_IN_LOCAL_PATH = false;
		INSTALL_PATCHER = false;
		PROGRAM_FILES_DIRECTORY_TO_INSTALL = "MHLab";
		PATCHNOTES_URL = "http://your/url/to/patchnotes.html";
		NEWS_URL = "http://your/url/to/news.html";
	}

	public void OverrideSettings()
	{
		SettingsManager.VERSIONS_FILE_DOWNLOAD_URL = VERSIONS_FILE_DOWNLOAD_URL;
		SettingsManager.PATCHES_DOWNLOAD_URL = PATCHES_DOWNLOAD_URL;
		SettingsManager.BUILDS_DOWNLOAD_URL = BUILDS_DOWNLOAD_URL;
		SettingsManager.PATCHER_DOWNLOAD_URL = PATCHER_DOWNLOAD_URL;
		SettingsManager.PATCH_DOWNLOAD_RETRY_ATTEMPTS = PATCH_DOWNLOAD_RETRY_ATTEMPTS;
		SettingsManager.LAUNCH_APP = SettingsManager.APP_PATH + Path.DirectorySeparatorChar + LAUNCH_APP;
		SettingsManager.LAUNCHER_NAME = LAUNCHER_NAME;
		SettingsManager.LAUNCH_ARG = LAUNCH_ARG;
		SettingsManager.LAUNCH_COMMAND = "-LaunchArg=" + LAUNCH_ARG;
		SettingsManager.USE_RAW_LAUNCH_ARG = USE_RAW_LAUNCH;
		SettingsManager.ENABLE_FTP = ENABLE_FTP;
		SettingsManager.FTP_USERNAME = FTP_USERNAME;
		SettingsManager.FTP_PASSWORD = FTP_PASSWORD;
		SettingsManager.INSTALL_IN_LOCAL_PATH = INSTALL_IN_LOCAL_PATH;
		SettingsManager.PROGRAM_FILES_DIRECTORY_TO_INSTALL = PROGRAM_FILES_DIRECTORY_TO_INSTALL;
		SettingsManager.PROGRAM_FILES_DIRECTORY_TO_INSTALL_ABS_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), PROGRAM_FILES_DIRECTORY_TO_INSTALL);
		SettingsManager.CREATE_DESKTOP_SHORTCUT = CREATE_DESKTOP_SHORTCUT;
		SettingsManager.ENABLE_PATCHER = ENABLE_PATCHER;
		SettingsManager.ENABLE_INSTALLER = ENABLE_INSTALLER;
		SettingsManager.ENABLE_REPAIRER = ENABLE_REPAIRER;
		SettingsManager.INSTALL_PATCHER = INSTALL_PATCHER;
		SettingsManager.PATCHNOTES_URL = PATCHNOTES_URL;
		SettingsManager.NEWS_URL = NEWS_URL;
	}

	public static string XmlSerializeToString(object objectInstance)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(objectInstance.GetType());
		StringBuilder stringBuilder = new StringBuilder();
		using (TextWriter textWriter = new StringWriter(stringBuilder))
		{
			xmlSerializer.Serialize(textWriter, objectInstance);
		}
		return stringBuilder.ToString();
	}

	public static T XmlDeserializeFromString<T>(string objectData)
	{
		return (T)XmlDeserializeFromString(objectData, typeof(T));
	}

	public static object XmlDeserializeFromString(string objectData, Type type)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(type);
		using TextReader textReader = new StringReader(objectData);
		return xmlSerializer.Deserialize(textReader);
	}
}

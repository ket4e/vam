using System;
using System.IO;
using System.Reflection;

namespace MHLab.PATCH.Settings;

public class SettingsManager
{
	public static string APP_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

	public static string CURRENT_BUILD_PATH = APP_PATH + Path.DirectorySeparatorChar + "current" + Path.DirectorySeparatorChar;

	public static string BUILDS_PATH = APP_PATH + Path.DirectorySeparatorChar + "builds" + Path.DirectorySeparatorChar;

	public static string PATCHES_PATH = APP_PATH + Path.DirectorySeparatorChar + "patches" + Path.DirectorySeparatorChar + "tmp" + Path.DirectorySeparatorChar;

	public static string SIGNATURES_PATH = APP_PATH + Path.DirectorySeparatorChar + "patches" + Path.DirectorySeparatorChar + "signatures" + Path.DirectorySeparatorChar;

	public static string FINAL_PATCHES_PATH = APP_PATH + Path.DirectorySeparatorChar + "patches" + Path.DirectorySeparatorChar;

	public static string DEPLOY_PATH = APP_PATH + Path.DirectorySeparatorChar + "deploy" + Path.DirectorySeparatorChar;

	public static string PATCHER_FILES_PATH = APP_PATH + Path.DirectorySeparatorChar + "patcher" + Path.DirectorySeparatorChar;

	public static string LOGS_ERROR_PATH = APP_PATH + Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "patch.log";

	public static string PATCHES_TMP_FOLDER = APP_PATH + Path.DirectorySeparatorChar + "_tmp" + Path.DirectorySeparatorChar;

	public static string LAUNCHER_CONFIG_GENERATION_PATH = APP_PATH + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar;

	public static char PATCHES_SYMBOL_SEPARATOR = '>';

	public static string PATCH_VERSION_ENCRYPTION_PASSWORD = "dwqqe2231ffe32";

	public static string PATCH_EXTENSION = ".patch";

	public static string PATCH_DELETE_FILE_EXTENSION = ".delete";

	public static string PATCH_VERSION_PATH = APP_PATH + Path.DirectorySeparatorChar + "version";

	public static string VERSIONS_FILE_DOWNLOAD_URL = "http://mhlab.altervista.org/patches/versions.txt";

	public static string PATCHES_DOWNLOAD_URL = "http://mhlab.altervista.org/patches/";

	public static string BUILDS_DOWNLOAD_URL = "http://mhlab.altervista.org/builds/";

	public static string PATCHER_DOWNLOAD_URL = "http://mhlab.altervista.org/patcher/";

	public static ushort PATCH_DOWNLOAD_RETRY_ATTEMPTS = 3;

	public static ushort FILE_DELETE_RETRY_ATTEMPTS = 10;

	public static string LAUNCH_APP = APP_PATH + Path.DirectorySeparatorChar + "Build.exe";

	public static string LAUNCHER_NAME = "PATCH.exe";

	public static string LAUNCH_ARG = "default";

	public static string LAUNCH_COMMAND = "-LaunchArg=" + LAUNCH_ARG;

	public static bool USE_RAW_LAUNCH_ARG = false;

	public static bool INSTALL_IN_LOCAL_PATH = true;

	public static string PROGRAM_FILES_DIRECTORY_TO_INSTALL = "MHLab";

	public static string PROGRAM_FILES_DIRECTORY_TO_INSTALL_ABS_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), PROGRAM_FILES_DIRECTORY_TO_INSTALL);

	public static string LAUNCHER_CONFIG_PATH = APP_PATH + Path.DirectorySeparatorChar + "config";

	public static string PATCH_SAFE_BACKUP = APP_PATH + Path.DirectorySeparatorChar + "_safe_backup";

	public static string VERSION_FILE_LOCAL_PATH = APP_PATH + Path.DirectorySeparatorChar + "version";

	public static bool ENABLE_FTP = false;

	public static string FTP_USERNAME = "YourFTPUsernameHere";

	public static string FTP_PASSWORD = "YourFTPPasswordHere";

	public static bool ENABLE_PATCHER = true;

	public static bool ENABLE_INSTALLER = true;

	public static bool ENABLE_REPAIRER = true;

	public static bool INSTALL_PATCHER = false;

	public static bool CREATE_DESKTOP_SHORTCUT = true;

	public static string PATCHNOTES_URL = "http://your/url/to/patchnotes.html";

	public static string NEWS_URL = "http://your/url/to/news.html";

	public static int DOWNLOAD_BUFFER_SIZE = 8192;

	public static void RegeneratePaths()
	{
		CURRENT_BUILD_PATH = APP_PATH + Path.DirectorySeparatorChar + "current" + Path.DirectorySeparatorChar;
		BUILDS_PATH = APP_PATH + Path.DirectorySeparatorChar + "builds" + Path.DirectorySeparatorChar;
		PATCHES_PATH = APP_PATH + Path.DirectorySeparatorChar + "patches" + Path.DirectorySeparatorChar + "tmp" + Path.DirectorySeparatorChar;
		SIGNATURES_PATH = APP_PATH + Path.DirectorySeparatorChar + "patches" + Path.DirectorySeparatorChar + "signatures" + Path.DirectorySeparatorChar;
		FINAL_PATCHES_PATH = APP_PATH + Path.DirectorySeparatorChar + "patches" + Path.DirectorySeparatorChar;
		DEPLOY_PATH = APP_PATH + Path.DirectorySeparatorChar + "deploy" + Path.DirectorySeparatorChar;
		PATCHER_FILES_PATH = APP_PATH + Path.DirectorySeparatorChar + "patcher" + Path.DirectorySeparatorChar;
		LOGS_ERROR_PATH = APP_PATH + Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "error.log";
		PATCHES_TMP_FOLDER = APP_PATH + Path.DirectorySeparatorChar + "_tmp" + Path.DirectorySeparatorChar;
		LAUNCHER_CONFIG_GENERATION_PATH = APP_PATH + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar;
		PATCH_VERSION_PATH = APP_PATH + Path.DirectorySeparatorChar + "version";
		LAUNCH_APP = APP_PATH + Path.DirectorySeparatorChar + "Build.exe";
		LAUNCHER_CONFIG_PATH = APP_PATH + Path.DirectorySeparatorChar + "config";
		PATCH_SAFE_BACKUP = APP_PATH + Path.DirectorySeparatorChar + "_safe_backup";
		VERSION_FILE_LOCAL_PATH = APP_PATH + Path.DirectorySeparatorChar + "version";
	}
}

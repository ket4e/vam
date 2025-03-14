using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MVR.FileManagement;
using SimpleJSON;

namespace MVR.Hub;

public class HubResourcePackage
{
	public delegate void DownloadQueuedCallback(HubResourcePackage hrp);

	public delegate void DownloadStartCallback(HubResourcePackage hrp);

	public delegate void DownloadCompleteCallback(HubResourcePackage hrp, bool alreadyHad);

	public delegate void DownloadProgressCallback(HubResourcePackage hrp, float f);

	public delegate void DownloadErrorCallback(HubResourcePackage hrp, string err);

	protected HubBrowse browser;

	private static readonly string[] SizeSuffixes = new string[9] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

	protected string package_id;

	protected string resource_id;

	protected string resolvedVarName;

	protected JSONStorableAction goToResourceAction;

	protected JSONStorableBool isDependencyJSON;

	protected JSONStorableString nameJSON;

	protected JSONStorableString licenseTypeJSON;

	protected JSONStorableString fileSizeJSON;

	protected JSONStorableBool alreadyHaveJSON;

	protected JSONStorableString updateMsgJSON;

	protected JSONStorableBool updateAvailableJSON;

	protected JSONStorableBool alreadyHaveSceneJSON;

	protected string alreadyHaveScenePath;

	protected JSONStorableBool notOnHubJSON;

	public string promotionalUrl;

	protected string downloadUrl;

	protected string latestUrl;

	public DownloadQueuedCallback downloadQueuedCallback;

	public DownloadStartCallback downloadStartCallback;

	public DownloadCompleteCallback downloadCompleteCallback;

	public DownloadProgressCallback downloadProgressCallback;

	public DownloadErrorCallback downloadErrorCallback;

	protected JSONStorableFloat downloadProgressJSON;

	protected JSONStorableBool isDownloadQueuedJSON;

	protected JSONStorableBool isDownloadingJSON;

	protected JSONStorableBool isDownloadedJSON;

	protected bool hadDownloadError;

	protected JSONStorableAction downloadAction;

	protected JSONStorableAction updateAction;

	protected JSONStorableAction openInPackageManagerAction;

	protected JSONStorableAction openSceneAction;

	public string GroupName { get; protected set; }

	public string Creator { get; protected set; }

	public string Version { get; protected set; }

	public int LatestVersion { get; protected set; }

	public string PublishingUser { get; protected set; }

	public string Name => nameJSON.val;

	public string LicenseType => licenseTypeJSON.val;

	public int FileSize { get; protected set; }

	public bool NeedsDownload => !alreadyHaveJSON.val || updateAvailableJSON.val;

	public bool IsDownloading => isDownloadingJSON.val;

	public bool IsDownloadQueued => isDownloadQueuedJSON.val;

	public bool HadDownloadError => hadDownloadError;

	public bool CanBeDownloaded => browser != null && downloadUrl != null && downloadUrl != string.Empty && downloadUrl != "null" && !isDownloadQueuedJSON.val;

	public HubResourcePackage(JSONClass package, HubBrowse hubBrowse, bool isDependency)
	{
		browser = hubBrowse;
		package_id = package["package_id"];
		resource_id = package["resource_id"];
		string input = package["filename"];
		input = Regex.Replace(input, ".var$", string.Empty);
		GroupName = Regex.Replace(input, "(.*)\\..*", "$1");
		Creator = Regex.Replace(GroupName, "(.*)\\..*", "$1");
		PublishingUser = package["username"];
		if (PublishingUser == null)
		{
			PublishingUser = "Unknown";
		}
		Version = package["version"];
		if (Version == null)
		{
			Version = Regex.Replace(input, ".*\\.([0-9]+)$", "$1");
		}
		resolvedVarName = GroupName + "." + Version + ".var";
		string text = package["latest_version"];
		if (text == null)
		{
			text = Version;
		}
		if (text != null)
		{
			if (int.TryParse(text, out var result))
			{
				LatestVersion = result;
			}
			else
			{
				LatestVersion = -1;
			}
		}
		string startingValue = package["licenseType"];
		string s = package["file_size"];
		SyncFileSize(s);
		string startingValue2 = SizeSuffix(FileSize);
		downloadUrl = package["downloadUrl"];
		if (downloadUrl == null)
		{
			downloadUrl = package["urlHosted"];
		}
		latestUrl = package["latestUrl"];
		if (latestUrl == null)
		{
			latestUrl = downloadUrl;
		}
		bool startingValue3 = downloadUrl == "null";
		promotionalUrl = package["promotional_link"];
		goToResourceAction = new JSONStorableAction("GoToResource", GoToResource);
		isDependencyJSON = new JSONStorableBool("isDependency", isDependency);
		nameJSON = new JSONStorableString("name", input);
		licenseTypeJSON = new JSONStorableString("licenseType", startingValue);
		fileSizeJSON = new JSONStorableString("fileSize", startingValue2);
		alreadyHaveJSON = new JSONStorableBool("alreadyHave", startingValue: false);
		alreadyHaveSceneJSON = new JSONStorableBool("alreadyHaveScene", startingValue: false);
		updateAvailableJSON = new JSONStorableBool("updateAvailable", startingValue: false);
		updateMsgJSON = new JSONStorableString("updateMsg", "Update");
		updateAction = new JSONStorableAction("Update", Update);
		notOnHubJSON = new JSONStorableBool("notOnHub", startingValue3);
		downloadAction = new JSONStorableAction("Download", Download);
		isDownloadQueuedJSON = new JSONStorableBool("isDownloadQueued", startingValue: false);
		isDownloadingJSON = new JSONStorableBool("isDownloading", startingValue: false);
		isDownloadedJSON = new JSONStorableBool("isDownloaded", startingValue: false);
		downloadProgressJSON = new JSONStorableFloat("downloadProgress", 0f, 0f, 1f, constrain: true, interactable: false);
		openInPackageManagerAction = new JSONStorableAction("OpenInPackageManager", OpenInPackageManager);
		openSceneAction = new JSONStorableAction("OpenScene", OpenScene);
		Refresh();
	}

	private static string SizeSuffix(int value, int decimalPlaces = 1)
	{
		if (value < 0)
		{
			return "-" + SizeSuffix(-value);
		}
		if (value == 0)
		{
			return string.Format("{0:n" + decimalPlaces + "} bytes", 0);
		}
		int num = (int)Math.Log(value, 1024.0);
		decimal num2 = (decimal)value / (decimal)(1L << num * 10);
		if (Math.Round(num2, decimalPlaces) >= 1000m)
		{
			num++;
			num2 /= 1024m;
		}
		return string.Format("{0:n" + decimalPlaces + "} {1}", num2, SizeSuffixes[num]);
	}

	protected void GoToResource()
	{
		if (resource_id != "null")
		{
			browser.OpenDetail(resource_id);
		}
	}

	protected void SyncFileSize(string s)
	{
		if (int.TryParse(s, out var result))
		{
			FileSize = result;
		}
	}

	protected void DownloadStarted()
	{
		isDownloadQueuedJSON.val = false;
		isDownloadingJSON.val = true;
		if (downloadStartCallback != null)
		{
			downloadStartCallback(this);
		}
	}

	protected void DownloadProgress(float f)
	{
		downloadProgressJSON.val = f;
		if (downloadProgressCallback != null)
		{
			downloadProgressCallback(this, f);
		}
	}

	protected void DownloadComplete(byte[] data, Dictionary<string, string> responseHeaders)
	{
		isDownloadingJSON.val = false;
		isDownloadedJSON.val = true;
		string text;
		if (responseHeaders.TryGetValue("Content-Disposition", out var value))
		{
			value = Regex.Replace(value, ";$", string.Empty);
			text = Regex.Replace(value, ".*filename=\"?([^\"]+)\"?.*", "$1");
		}
		else
		{
			text = resolvedVarName;
		}
		try
		{
			FileManager.WriteAllBytes("AddonPackages/" + text, data);
			if (downloadCompleteCallback != null)
			{
				downloadCompleteCallback(this, alreadyHad: false);
			}
		}
		catch (Exception ex)
		{
			SuperController.LogError("Error while trying to save file AddonPackages/" + text + " after download");
			isDownloadQueuedJSON.val = false;
			isDownloadingJSON.val = false;
			if (downloadErrorCallback != null)
			{
				downloadErrorCallback(this, ex.Message);
			}
		}
	}

	protected void DownloadError(string err)
	{
		isDownloadQueuedJSON.val = false;
		isDownloadingJSON.val = false;
		hadDownloadError = true;
		SuperController.LogError("Error while downloading " + Name + ": " + err);
		if (downloadErrorCallback != null)
		{
			downloadErrorCallback(this, err);
		}
	}

	public void Download()
	{
		if (CanBeDownloaded)
		{
			hadDownloadError = false;
			if (downloadQueuedCallback != null)
			{
				downloadQueuedCallback(this);
			}
			if (!alreadyHaveJSON.val)
			{
				isDownloadQueuedJSON.val = true;
				browser.QueueDownload(downloadUrl, promotionalUrl, DownloadStarted, DownloadProgress, DownloadComplete, DownloadError);
			}
			else if (updateAvailableJSON.val && latestUrl != null && latestUrl != string.Empty && latestUrl != "null")
			{
				isDownloadQueuedJSON.val = true;
				browser.QueueDownload(latestUrl, promotionalUrl, DownloadStarted, DownloadProgress, DownloadComplete, DownloadError);
			}
			else if (downloadCompleteCallback != null)
			{
				downloadCompleteCallback(this, alreadyHad: true);
			}
		}
	}

	public void Update()
	{
		if (browser != null && latestUrl != null && latestUrl != string.Empty && !isDownloadQueuedJSON.val && updateAvailableJSON.val)
		{
			isDownloadQueuedJSON.val = true;
			browser.QueueDownload(latestUrl, promotionalUrl, DownloadStarted, DownloadProgress, DownloadComplete, DownloadError);
		}
	}

	public void OpenInPackageManager()
	{
		VarPackage package = FileManager.GetPackage(nameJSON.val);
		if (package != null)
		{
			SuperController.singleton.OpenPackageInManager(nameJSON.val);
		}
	}

	protected void OpenScene()
	{
		if (alreadyHaveScenePath != null)
		{
			SuperController.singleton.Load(alreadyHaveScenePath);
		}
	}

	public void Refresh()
	{
		isDownloadedJSON.val = false;
		VarPackage package;
		if (isDependencyJSON.val)
		{
			package = FileManager.GetPackage(nameJSON.val);
		}
		else
		{
			string text = FileManager.PackageIDToPackageGroupID(nameJSON.val);
			string packageUidOrPath = text + ".latest";
			package = FileManager.GetPackage(packageUidOrPath);
		}
		if (package != null)
		{
			alreadyHaveJSON.val = true;
			if ((Version == "latest" || !isDependencyJSON.val) && LatestVersion != -1)
			{
				if (package.Version < LatestVersion)
				{
					updateAvailableJSON.val = true;
					updateMsgJSON.val = "Update " + package.Version + " -> " + LatestVersion;
				}
				else if (package.Version > LatestVersion)
				{
					updateAvailableJSON.val = false;
					VarPackage package2 = FileManager.GetPackage(nameJSON.val);
					if (package2 == null)
					{
						alreadyHaveJSON.val = false;
					}
				}
				else
				{
					updateAvailableJSON.val = false;
				}
			}
			else
			{
				updateAvailableJSON.val = false;
			}
			if (alreadyHaveJSON.val)
			{
				List<FileEntry> list = new List<FileEntry>();
				package.FindFiles("Saves/scene", "*.json", list);
				if (list.Count > 0)
				{
					FileEntry fileEntry = list[0];
					alreadyHaveScenePath = fileEntry.Uid;
					alreadyHaveSceneJSON.val = true;
				}
				else
				{
					alreadyHaveScenePath = null;
					alreadyHaveSceneJSON.val = false;
				}
			}
			else
			{
				alreadyHaveScenePath = null;
				alreadyHaveSceneJSON.val = false;
			}
		}
		else
		{
			alreadyHaveJSON.val = false;
			alreadyHaveScenePath = null;
			alreadyHaveSceneJSON.val = false;
		}
	}

	public void RegisterUI(HubResourcePackageUI ui)
	{
		if (ui != null)
		{
			ui.connectedItem = this;
			goToResourceAction.button = ui.resourceButton;
			if (ui.resourceButton != null)
			{
				ui.resourceButton.interactable = !notOnHubJSON.val && isDependencyJSON.val;
			}
			isDependencyJSON.indicator = ui.isDependencyIndicator;
			nameJSON.text = ui.nameText;
			licenseTypeJSON.text = ui.licenseTypeText;
			fileSizeJSON.text = ui.fileSizeText;
			notOnHubJSON.indicator = ui.notOnHubIndicator;
			alreadyHaveJSON.indicator = ui.alreadyHaveIndicator;
			alreadyHaveSceneJSON.indicator = ui.alreadyHaveSceneIndicator;
			updateAvailableJSON.indicator = ui.updateAvailableIndicator;
			updateMsgJSON.text = ui.updateMsgText;
			updateAction.button = ui.updateButton;
			downloadAction.button = ui.downloadButton;
			isDownloadQueuedJSON.indicator = ui.isDownloadQueuedIndicator;
			isDownloadingJSON.indicator = ui.isDownloadingIndicator;
			isDownloadedJSON.indicator = ui.isDownloadedIndicator;
			downloadProgressJSON.slider = ui.progressSlider;
			openInPackageManagerAction.button = ui.openInPackageManagerButton;
			openSceneAction.button = ui.openSceneButton;
		}
	}
}

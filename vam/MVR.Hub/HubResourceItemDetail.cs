using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace MVR.Hub;

public class HubResourceItemDetail : HubResourceItem
{
	protected JSONStorableBool hadErrorJSON;

	protected JSONStorableString errorJSON;

	protected JSONStorableAction closeDetailAction;

	protected List<HubResourcePackage> downloadPackages;

	protected string resourceOverviewUrl;

	protected JSONStorableAction navigateToOverviewAction;

	protected string resourceUpdatesUrl;

	protected JSONStorableAction navigateToUpdatesAction;

	protected JSONStorableBool hasUpdatesJSON;

	protected JSONStorableString updatesTextJSON;

	protected string resourceReviewsUrl;

	protected JSONStorableAction navigateToReviewsAction;

	protected JSONStorableBool hasReviewsJSON;

	protected JSONStorableString reviewsTextJSON;

	protected string resourceHistoryUrl;

	protected JSONStorableAction navigateToHistoryAction;

	protected string resourceDiscussionUrl;

	protected JSONStorableAction navigateToDiscussionAction;

	protected JSONStorableBool hasPromotionalLinkJSON;

	protected string promotionalUrl;

	protected JSONStorableAction navigateToPromotionalLinkAction;

	protected JSONStorableString promotionalLinkTextJSON;

	protected JSONStorableString externalDownloadUrl;

	protected JSONStorableAction goToExternalDownloadAction;

	protected RectTransform packagePrefab;

	protected RectTransform packageContent;

	protected RectTransform creatorSupportContent;

	protected JSONStorableBool hasOtherCreatorsJSON;

	protected JSONClass dependencies;

	protected JSONStorableAction downloadAllAction;

	protected JSONStorableBool downloadAvailableJSON;

	public bool IsDownloading
	{
		get
		{
			if (downloadPackages != null)
			{
				foreach (HubResourcePackage downloadPackage in downloadPackages)
				{
					if (downloadPackage.IsDownloading || downloadPackage.IsDownloadQueued)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public HubResourceItemDetail(JSONClass resource, HubBrowse hubBrowse)
		: base(resource, hubBrowse, queueImagesImmediate: true)
	{
		if (resource_id != null)
		{
			resourceOverviewUrl = "https://hub.virtamate.com/resources/" + resource_id + "/overview-panel";
			resourceUpdatesUrl = "https://hub.virtamate.com/resources/" + resource_id + "/updates-panel";
			resourceReviewsUrl = "https://hub.virtamate.com/resources/" + resource_id + "/review-panel";
			resourceHistoryUrl = "https://hub.virtamate.com/resources/" + resource_id + "/history-panel";
		}
		if (discussion_thread_id != null)
		{
			resourceDiscussionUrl = "https://hub.virtamate.com/threads/" + discussion_thread_id + "/discussion-panel";
		}
		bool flag = false;
		string startingValue = string.Empty;
		string text = resource["status"];
		if (text != null && text == "error")
		{
			flag = true;
			startingValue = resource["error"];
		}
		string startingValue2 = resource["download_url"];
		promotionalUrl = resource["promotional_link"];
		dependencies = resource["dependencies"].AsObject;
		int asInt = resource["review_count"].AsInt;
		bool startingValue3 = asInt > 0;
		int asInt2 = resource["update_count"].AsInt;
		bool startingValue4 = asInt2 > 0;
		if (flag)
		{
			browser.NavigateWebPanel("about:blank");
		}
		else
		{
			NavigateToOverview();
		}
		hadErrorJSON = new JSONStorableBool("hadError", flag);
		errorJSON = new JSONStorableString("error", startingValue);
		closeDetailAction = new JSONStorableAction("CloseDetail", CloseDetail);
		navigateToOverviewAction = new JSONStorableAction("NavigateToOverview", NavigateToOverview);
		navigateToUpdatesAction = new JSONStorableAction("NavigateToUpdates", NavigateToUpdates);
		hasUpdatesJSON = new JSONStorableBool("hasUpdates", startingValue4);
		updatesTextJSON = new JSONStorableString("updatesText", "Updates (" + asInt2 + ")");
		navigateToReviewsAction = new JSONStorableAction("NavigateToReviews", NavigateToReviews);
		hasReviewsJSON = new JSONStorableBool("hasReviews", startingValue3);
		reviewsTextJSON = new JSONStorableString("reviewsText", "Reviews (" + asInt + ")");
		navigateToHistoryAction = new JSONStorableAction("NavigateToHistory", NavigateToHistory);
		navigateToDiscussionAction = new JSONStorableAction("NavigateToDiscussion", NavigateToDiscussion);
		hasPromotionalLinkJSON = new JSONStorableBool("hasPromotionalLink", promotionalUrl != null && promotionalUrl != string.Empty && promotionalUrl != "null");
		navigateToPromotionalLinkAction = new JSONStorableAction("NavigateToPromotionalLink", NavigateToPromotionalLink);
		promotionalLinkTextJSON = new JSONStorableString("promotionalLinkText", base.Creator);
		hasOtherCreatorsJSON = new JSONStorableBool("hasOtherCreators", startingValue: false);
		externalDownloadUrl = new JSONStorableString("externalDownloadUrl", startingValue2);
		goToExternalDownloadAction = new JSONStorableAction("GoToExternalDownload", GoToExternalDownload);
		downloadAllAction = new JSONStorableAction("DownloadAll", DownloadAll);
		downloadAvailableJSON = new JSONStorableBool("downloadAvailable", startingValue: false);
		downloadPackages = new List<HubResourcePackage>();
	}

	public void CloseDetail()
	{
		browser.CloseDetail(resource_id);
	}

	public override void Refresh()
	{
		base.Refresh();
		if (downloadPackages != null)
		{
			foreach (HubResourcePackage downloadPackage in downloadPackages)
			{
				downloadPackage.Refresh();
			}
		}
		SyncDownloadAvailable();
	}

	public void NavigateToOverview()
	{
		if (resourceOverviewUrl != null)
		{
			browser.NavigateWebPanel(resourceOverviewUrl);
		}
	}

	public void NavigateToUpdates()
	{
		if (resourceUpdatesUrl != null)
		{
			browser.NavigateWebPanel(resourceUpdatesUrl);
		}
	}

	public void NavigateToReviews()
	{
		if (resourceReviewsUrl != null)
		{
			browser.NavigateWebPanel(resourceReviewsUrl);
		}
	}

	public void NavigateToHistory()
	{
		if (resourceHistoryUrl != null)
		{
			browser.NavigateWebPanel(resourceHistoryUrl);
		}
	}

	public void NavigateToDiscussion()
	{
		if (resourceDiscussionUrl != null)
		{
			browser.NavigateWebPanel(resourceDiscussionUrl);
		}
	}

	public void NavigateToPromotionalLink()
	{
		if (promotionalUrl != null)
		{
			browser.NavigateWebPanel(promotionalUrl);
		}
	}

	protected void GoToExternalDownload()
	{
		if (externalDownloadUrl != null && externalDownloadUrl.val != null)
		{
			browser.NavigateWebPanel(externalDownloadUrl.val);
		}
	}

	public void DownloadAll()
	{
		foreach (HubResourcePackage downloadPackage in downloadPackages)
		{
			downloadPackage.Download();
		}
	}

	protected void SyncDownloadAvailable()
	{
		bool val = false;
		if (downloadPackages != null)
		{
			foreach (HubResourcePackage downloadPackage in downloadPackages)
			{
				if (downloadPackage.NeedsDownload)
				{
					val = true;
				}
			}
		}
		downloadAvailableJSON.val = val;
	}

	public void RegisterUI(HubResourceItemDetailUI ui)
	{
		base.RegisterUI(ui);
		if (!(ui != null))
		{
			return;
		}
		ui.connectedItem = this;
		hadErrorJSON.indicator = ui.hadErrorIndicator;
		errorJSON.text = ui.errorText;
		closeDetailAction.button = ui.closeDetailButton;
		closeDetailAction.buttonAlt = ui.closeDetailButtonAlt;
		navigateToOverviewAction.button = ui.navigateToOverviewButton;
		navigateToUpdatesAction.button = ui.navigateToUpdatesButton;
		hasUpdatesJSON.indicator = ui.hasUpdatesIndicator;
		updatesTextJSON.text = ui.updatesText;
		navigateToReviewsAction.button = ui.navigateToReviewsButton;
		hasReviewsJSON.indicator = ui.hasReviewsIndicator;
		reviewsTextJSON.text = ui.reviewsText;
		navigateToHistoryAction.button = ui.navigateToHistoryButton;
		navigateToDiscussionAction.button = ui.navigateToDiscussionButton;
		if (!SuperController.singleton.promotionalDisabled)
		{
			hasPromotionalLinkJSON.indicator = ui.hasPromotionalLinkIndicator;
			navigateToPromotionalLinkAction.button = ui.navigateToPromotionalLinkButton;
			promotionalLinkTextJSON.text = ui.promotionalLinkText;
		}
		hubDownloadableJSON.indicatorAlt = ui.hubDownloadableIndicatorAlt;
		hubDownloadableJSON.negativeIndicatorAlt = ui.hubDownloadableNegativeIndicatorAlt;
		externalDownloadUrl.text = ui.externalDownloadUrl;
		goToExternalDownloadAction.button = ui.goToExternalDownloadUrlButton;
		downloadAllAction.button = ui.downloadAllButton;
		downloadAvailableJSON.indicator = ui.downloadAvailableIndicator;
		hasOtherCreatorsJSON.indicator = ui.hasOtherCreatorsIndicator;
		if (hasPromotionalLinkJSON.val && ui.promtionalLinkButtonEnterExitAction != null)
		{
			ui.promtionalLinkButtonEnterExitAction.onEnterActions = delegate
			{
				browser.ShowHoverUrl(promotionalUrl);
			};
			ui.promtionalLinkButtonEnterExitAction.onExitActions = delegate
			{
				browser.ShowHoverUrl(string.Empty);
			};
		}
		packageContent = ui.packageContent;
		creatorSupportContent = ui.creatorSupportContent;
		if (!(packageContent != null))
		{
			return;
		}
		foreach (JSONNode item in varFilesJSONArray)
		{
			JSONClass asObject = item.AsObject;
			if (!(asObject != null))
			{
				continue;
			}
			HubResourcePackage hubResourcePackage = new HubResourcePackage(asObject, browser, isDependency: false);
			hubResourcePackage.downloadCompleteCallback = (HubResourcePackage.DownloadCompleteCallback)Delegate.Combine(hubResourcePackage.downloadCompleteCallback, new HubResourcePackage.DownloadCompleteCallback(browser.ResourceDownloadComplete));
			hubResourcePackage.promotionalUrl = promotionalUrl;
			downloadPackages.Add(hubResourcePackage);
			RectTransform rectTransform = browser.CreateDownloadPrefabInstance();
			rectTransform.SetParent(packageContent, worldPositionStays: false);
			HubResourcePackageUI component = rectTransform.GetComponent<HubResourcePackageUI>();
			if (component != null)
			{
				hubResourcePackage.RegisterUI(component);
			}
			if (!(dependencies != null))
			{
				continue;
			}
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.Add(hubResourcePackage.Creator);
			JSONArray asArray = dependencies[hubResourcePackage.GroupName].AsArray;
			if (!(asArray != null))
			{
				continue;
			}
			foreach (JSONNode item2 in asArray)
			{
				JSONClass asObject2 = item2.AsObject;
				if (!(asObject2 != null))
				{
					continue;
				}
				HubResourcePackage dhrp = new HubResourcePackage(asObject2, browser, isDependency: true);
				HubResourcePackage hubResourcePackage2 = dhrp;
				hubResourcePackage2.downloadCompleteCallback = (HubResourcePackage.DownloadCompleteCallback)Delegate.Combine(hubResourcePackage2.downloadCompleteCallback, new HubResourcePackage.DownloadCompleteCallback(browser.ResourceDownloadComplete));
				downloadPackages.Add(dhrp);
				RectTransform rectTransform2 = browser.CreateDownloadPrefabInstance();
				if (rectTransform2 != null)
				{
					rectTransform2.SetParent(packageContent, worldPositionStays: false);
					HubResourcePackageUI component2 = rectTransform2.GetComponent<HubResourcePackageUI>();
					if (component2 != null)
					{
						dhrp.RegisterUI(component2);
					}
				}
				if (SuperController.singleton.promotionalDisabled || !(creatorSupportContent != null) || dhrp.promotionalUrl == null || !(dhrp.promotionalUrl != string.Empty) || !(dhrp.promotionalUrl != "null") || hashSet.Contains(dhrp.Creator))
				{
					continue;
				}
				hasOtherCreatorsJSON.val = true;
				hashSet.Add(dhrp.Creator);
				RectTransform rectTransform3 = browser.CreateCreatorSupportButtonPrefabInstance();
				if (!(rectTransform3 != null))
				{
					continue;
				}
				rectTransform3.SetParent(creatorSupportContent, worldPositionStays: false);
				HubResourceCreatorSupportUI component3 = rectTransform3.GetComponent<HubResourceCreatorSupportUI>();
				if (!(component3 != null))
				{
					continue;
				}
				if (component3.linkButton != null)
				{
					component3.linkButton.onClick.AddListener(delegate
					{
						browser.NavigateWebPanel(dhrp.promotionalUrl);
					});
				}
				if (component3.creatorNameText != null)
				{
					component3.creatorNameText.text = dhrp.Creator;
				}
				if (component3.pointerEnterExitAction != null)
				{
					component3.pointerEnterExitAction.onEnterActions = delegate
					{
						browser.ShowHoverUrl(dhrp.promotionalUrl);
					};
					component3.pointerEnterExitAction.onExitActions = delegate
					{
						browser.ShowHoverUrl(string.Empty);
					};
				}
			}
		}
		SyncDownloadAvailable();
	}
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class DAZCharacterTextureControlUI : UIProvider
{
	public Text uvLabel;

	public Button faceDiffuseFileBrowseButton;

	public Button faceDiffuseReloadButton;

	public Button faceDiffuseClearButton;

	public Text faceDiffuseUrlText;

	public Button torsoDiffuseFileBrowseButton;

	public Button torsoDiffuseReloadButton;

	public Button torsoDiffuseClearButton;

	public Text torsoDiffuseUrlText;

	public Button limbsDiffuseFileBrowseButton;

	public Button limbsDiffuseReloadButton;

	public Button limbsDiffuseClearButton;

	public Text limbsDiffuseUrlText;

	public Button genitalsDiffuseFileBrowseButton;

	public Button genitalsDiffuseReloadButton;

	public Button genitalsDiffuseClearButton;

	public Text genitalsDiffuseUrlText;

	public Button faceSpecularFileBrowseButton;

	public Button faceSpecularReloadButton;

	public Button faceSpecularClearButton;

	public Text faceSpecularUrlText;

	public Button torsoSpecularFileBrowseButton;

	public Button torsoSpecularReloadButton;

	public Button torsoSpecularClearButton;

	public Text torsoSpecularUrlText;

	public Button limbsSpecularFileBrowseButton;

	public Button limbsSpecularReloadButton;

	public Button limbsSpecularClearButton;

	public Text limbsSpecularUrlText;

	public Button genitalsSpecularFileBrowseButton;

	public Button genitalsSpecularReloadButton;

	public Button genitalsSpecularClearButton;

	public Text genitalsSpecularUrlText;

	public Button faceGlossFileBrowseButton;

	public Button faceGlossReloadButton;

	public Button faceGlossClearButton;

	public Text faceGlossUrlText;

	public Button torsoGlossFileBrowseButton;

	public Button torsoGlossReloadButton;

	public Button torsoGlossClearButton;

	public Text torsoGlossUrlText;

	public Button limbsGlossFileBrowseButton;

	public Button limbsGlossReloadButton;

	public Button limbsGlossClearButton;

	public Text limbsGlossUrlText;

	public Button genitalsGlossFileBrowseButton;

	public Button genitalsGlossReloadButton;

	public Button genitalsGlossClearButton;

	public Text genitalsGlossUrlText;

	public Button faceNormalFileBrowseButton;

	public Button faceNormalReloadButton;

	public Button faceNormalClearButton;

	public Text faceNormalUrlText;

	public Button torsoNormalFileBrowseButton;

	public Button torsoNormalReloadButton;

	public Button torsoNormalClearButton;

	public Text torsoNormalUrlText;

	public Button limbsNormalFileBrowseButton;

	public Button limbsNormalReloadButton;

	public Button limbsNormalClearButton;

	public Text limbsNormalUrlText;

	public Button genitalsNormalFileBrowseButton;

	public Button genitalsNormalReloadButton;

	public Button genitalsNormalClearButton;

	public Text genitalsNormalUrlText;

	public Button faceDetailFileBrowseButton;

	public Button faceDetailReloadButton;

	public Button faceDetailClearButton;

	public Text faceDetailUrlText;

	public Button torsoDetailFileBrowseButton;

	public Button torsoDetailReloadButton;

	public Button torsoDetailClearButton;

	public Text torsoDetailUrlText;

	public Button limbsDetailFileBrowseButton;

	public Button limbsDetailReloadButton;

	public Button limbsDetailClearButton;

	public Text limbsDetailUrlText;

	public Button genitalsDetailFileBrowseButton;

	public Button genitalsDetailReloadButton;

	public Button genitalsDetailClearButton;

	public Text genitalsDetailUrlText;

	public Button faceDecalFileBrowseButton;

	public Button faceDecalReloadButton;

	public Button faceDecalClearButton;

	public Text faceDecalUrlText;

	public Button torsoDecalFileBrowseButton;

	public Button torsoDecalReloadButton;

	public Button torsoDecalClearButton;

	public Text torsoDecalUrlText;

	public Button limbsDecalFileBrowseButton;

	public Button limbsDecalReloadButton;

	public Button limbsDecalClearButton;

	public Text limbsDecalUrlText;

	public Button genitalsDecalFileBrowseButton;

	public Button genitalsDecalReloadButton;

	public Button genitalsDecalClearButton;

	public Text genitalsDecalUrlText;

	public Button directoryBrowseButton;

	public Toggle autoBlendGenitalTexturesToggle;

	public Toggle autoBlendGenitalSpecGlossNormalTexturesToggle;

	public Button dumpAutoGeneratedGenitalTexturesButton;

	public Button autoBlendGenitalDiffuseTextureButton;

	public Transform autoBlendGenitalColorAdjustContainer;

	public Slider autoBlendGenitalLightenDarkenSlider;

	public Slider autoBlendGenitalHueOffsetSlider;

	public Slider autoBlendGenitalSaturationOffsetSlider;

	public RectTransform onHoverTextureUrlPanel;

	public Text onHoverTextureUrlText;

	private void OpenHoverTextureUrlPanel(string text)
	{
		if (onHoverTextureUrlPanel != null)
		{
			onHoverTextureUrlPanel.gameObject.SetActive(value: true);
		}
		if (onHoverTextureUrlText != null)
		{
			onHoverTextureUrlText.text = text;
		}
	}

	private void CloseHoverTextureUrlPanel()
	{
		if (onHoverTextureUrlPanel != null)
		{
			onHoverTextureUrlPanel.gameObject.SetActive(value: false);
		}
	}

	private void Awake()
	{
		if (!(onHoverTextureUrlPanel != null) || !(onHoverTextureUrlText != null))
		{
			return;
		}
		if (faceDiffuseUrlText != null)
		{
			UIHoverTextNotifier component = faceDiffuseUrlText.GetComponent<UIHoverTextNotifier>();
			if (component != null)
			{
				component.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (torsoDiffuseUrlText != null)
		{
			UIHoverTextNotifier component2 = torsoDiffuseUrlText.GetComponent<UIHoverTextNotifier>();
			if (component2 != null)
			{
				component2.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component2.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component2.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component2.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (limbsDiffuseUrlText != null)
		{
			UIHoverTextNotifier component3 = limbsDiffuseUrlText.GetComponent<UIHoverTextNotifier>();
			if (component3 != null)
			{
				component3.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component3.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component3.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component3.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (genitalsDiffuseUrlText != null)
		{
			UIHoverTextNotifier component4 = genitalsDiffuseUrlText.GetComponent<UIHoverTextNotifier>();
			if (component4 != null)
			{
				component4.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component4.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component4.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component4.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (faceSpecularUrlText != null)
		{
			UIHoverTextNotifier component5 = faceSpecularUrlText.GetComponent<UIHoverTextNotifier>();
			if (component5 != null)
			{
				component5.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component5.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component5.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component5.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (torsoSpecularUrlText != null)
		{
			UIHoverTextNotifier component6 = torsoSpecularUrlText.GetComponent<UIHoverTextNotifier>();
			if (component6 != null)
			{
				component6.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component6.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component6.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component6.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (limbsSpecularUrlText != null)
		{
			UIHoverTextNotifier component7 = limbsSpecularUrlText.GetComponent<UIHoverTextNotifier>();
			if (component7 != null)
			{
				component7.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component7.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component7.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component7.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (genitalsSpecularUrlText != null)
		{
			UIHoverTextNotifier component8 = genitalsSpecularUrlText.GetComponent<UIHoverTextNotifier>();
			if (component8 != null)
			{
				component8.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component8.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component8.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component8.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (faceGlossUrlText != null)
		{
			UIHoverTextNotifier component9 = faceGlossUrlText.GetComponent<UIHoverTextNotifier>();
			if (component9 != null)
			{
				component9.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component9.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component9.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component9.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (torsoGlossUrlText != null)
		{
			UIHoverTextNotifier component10 = torsoGlossUrlText.GetComponent<UIHoverTextNotifier>();
			if (component10 != null)
			{
				component10.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component10.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component10.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component10.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (limbsGlossUrlText != null)
		{
			UIHoverTextNotifier component11 = limbsGlossUrlText.GetComponent<UIHoverTextNotifier>();
			if (component11 != null)
			{
				component11.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component11.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component11.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component11.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (genitalsGlossUrlText != null)
		{
			UIHoverTextNotifier component12 = genitalsGlossUrlText.GetComponent<UIHoverTextNotifier>();
			if (component12 != null)
			{
				component12.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component12.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component12.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component12.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (faceNormalUrlText != null)
		{
			UIHoverTextNotifier component13 = faceNormalUrlText.GetComponent<UIHoverTextNotifier>();
			if (component13 != null)
			{
				component13.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component13.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component13.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component13.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (torsoNormalUrlText != null)
		{
			UIHoverTextNotifier component14 = torsoNormalUrlText.GetComponent<UIHoverTextNotifier>();
			if (component14 != null)
			{
				component14.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component14.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component14.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component14.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (limbsNormalUrlText != null)
		{
			UIHoverTextNotifier component15 = limbsNormalUrlText.GetComponent<UIHoverTextNotifier>();
			if (component15 != null)
			{
				component15.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component15.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component15.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component15.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (genitalsNormalUrlText != null)
		{
			UIHoverTextNotifier component16 = genitalsNormalUrlText.GetComponent<UIHoverTextNotifier>();
			if (component16 != null)
			{
				component16.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component16.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component16.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component16.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (faceDetailUrlText != null)
		{
			UIHoverTextNotifier component17 = faceDetailUrlText.GetComponent<UIHoverTextNotifier>();
			if (component17 != null)
			{
				component17.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component17.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component17.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component17.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (torsoDetailUrlText != null)
		{
			UIHoverTextNotifier component18 = torsoDetailUrlText.GetComponent<UIHoverTextNotifier>();
			if (component18 != null)
			{
				component18.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component18.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component18.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component18.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (limbsDetailUrlText != null)
		{
			UIHoverTextNotifier component19 = limbsDetailUrlText.GetComponent<UIHoverTextNotifier>();
			if (component19 != null)
			{
				component19.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component19.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component19.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component19.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (genitalsDetailUrlText != null)
		{
			UIHoverTextNotifier component20 = genitalsDetailUrlText.GetComponent<UIHoverTextNotifier>();
			if (component20 != null)
			{
				component20.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component20.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component20.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component20.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (faceDecalUrlText != null)
		{
			UIHoverTextNotifier component21 = faceDecalUrlText.GetComponent<UIHoverTextNotifier>();
			if (component21 != null)
			{
				component21.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component21.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component21.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component21.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (torsoDecalUrlText != null)
		{
			UIHoverTextNotifier component22 = torsoDecalUrlText.GetComponent<UIHoverTextNotifier>();
			if (component22 != null)
			{
				component22.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component22.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component22.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component22.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (limbsDecalUrlText != null)
		{
			UIHoverTextNotifier component23 = limbsDecalUrlText.GetComponent<UIHoverTextNotifier>();
			if (component23 != null)
			{
				component23.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component23.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
				{
					OpenHoverTextureUrlPanel(text.text);
				});
				component23.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component23.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
				{
					CloseHoverTextureUrlPanel();
				});
			}
		}
		if (!(genitalsDecalUrlText != null))
		{
			return;
		}
		UIHoverTextNotifier component24 = genitalsDecalUrlText.GetComponent<UIHoverTextNotifier>();
		if (component24 != null)
		{
			component24.onEnterNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component24.onEnterNotifier, (UIHoverTextNotifier.TextNotifier)delegate(Text text)
			{
				OpenHoverTextureUrlPanel(text.text);
			});
			component24.onExitNotifier = (UIHoverTextNotifier.TextNotifier)Delegate.Combine(component24.onExitNotifier, (UIHoverTextNotifier.TextNotifier)delegate
			{
				CloseHoverTextureUrlPanel();
			});
		}
	}
}

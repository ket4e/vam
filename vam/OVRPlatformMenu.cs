using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OVRPlatformMenu : MonoBehaviour
{
	public enum eHandler
	{
		ShowConfirmQuit,
		RetreatOneLevel
	}

	private enum eBackButtonAction
	{
		NONE,
		SHORT_PRESS
	}

	private OVRInput.RawButton inputCode = OVRInput.RawButton.Back;

	public eHandler shortPressHandler;

	public Func<bool> OnShortPress;

	private static Stack<string> sceneStack = new Stack<string>();

	private eBackButtonAction HandleBackButtonState()
	{
		eBackButtonAction result = eBackButtonAction.NONE;
		if (OVRInput.GetDown(inputCode))
		{
			result = eBackButtonAction.SHORT_PRESS;
		}
		return result;
	}

	private void Awake()
	{
		if (shortPressHandler == eHandler.RetreatOneLevel && OnShortPress == null)
		{
			OnShortPress = RetreatOneLevel;
		}
		if (!OVRManager.isHmdPresent)
		{
			base.enabled = false;
		}
		else
		{
			sceneStack.Push(SceneManager.GetActiveScene().name);
		}
	}

	private void ShowConfirmQuitMenu()
	{
	}

	private static bool RetreatOneLevel()
	{
		if (sceneStack.Count > 1)
		{
			string sceneName = sceneStack.Pop();
			SceneManager.LoadSceneAsync(sceneName);
			return false;
		}
		return true;
	}

	private void Update()
	{
	}
}

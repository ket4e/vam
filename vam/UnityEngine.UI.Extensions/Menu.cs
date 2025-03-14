namespace UnityEngine.UI.Extensions;

public abstract class Menu<T> : Menu where T : Menu<T>
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		Instance = (T)this;
	}

	protected virtual void OnDestroy()
	{
		Instance = (T)null;
	}

	protected static void Open()
	{
		if ((Object)Instance == (Object)null)
		{
			MenuManager.Instance.CreateInstance(typeof(T).Name);
		}
		else
		{
			T instance = Instance;
			instance.gameObject.SetActive(value: true);
		}
		MenuManager.Instance.OpenMenu(Instance);
	}

	protected static void Close()
	{
		if ((Object)Instance == (Object)null)
		{
			Debug.LogErrorFormat("Trying to close menu {0} but Instance is null", typeof(T));
		}
		else
		{
			MenuManager.Instance.CloseMenu(Instance);
		}
	}

	public override void OnBackPressed()
	{
		Close();
	}
}
public abstract class Menu : MonoBehaviour
{
	[Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")]
	public bool DestroyWhenClosed = true;

	[Tooltip("Disable menus that are under this one in the stack")]
	public bool DisableMenusUnderneath = true;

	public abstract void OnBackPressed();
}

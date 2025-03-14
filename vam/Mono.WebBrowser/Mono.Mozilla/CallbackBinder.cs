namespace Mono.Mozilla;

internal struct CallbackBinder
{
	public CallbackVoid OnWidgetLoaded;

	public CallbackOnStateChange OnStateChange;

	public CallbackOnProgress OnProgress;

	public CallbackOnLocationChanged OnLocationChanged;

	public CallbackOnStatusChange OnStatusChange;

	public CallbackOnSecurityChange OnSecurityChange;

	public KeyCallback OnKeyDown;

	public KeyCallback OnKeyUp;

	public KeyCallback OnKeyPress;

	public MouseCallback OnMouseDown;

	public MouseCallback OnMouseUp;

	public MouseCallback OnMouseClick;

	public MouseCallback OnMouseDoubleClick;

	public MouseCallback OnMouseOver;

	public MouseCallback OnMouseOut;

	public Callback2 OnActivate;

	public Callback2 OnFocus;

	public Callback2 OnBlur;

	public CallbackPtrPtr OnAlert;

	public CallbackOnAlertCheck OnAlertCheck;

	public CallbackOnConfirm OnConfirm;

	public CallbackOnConfirmCheck OnConfirmCheck;

	public CallbackOnConfirmEx OnConfirmEx;

	public CallbackOnPrompt OnPrompt;

	public CallbackOnPromptUsernameAndPassword OnPromptUsernameAndPassword;

	public CallbackOnPromptPassword OnPromptPassword;

	public CallbackOnSelect OnSelect;

	public CallbackVoid OnLoad;

	public CallbackVoid OnUnload;

	public CallbackOnShowContextMenu OnShowContextMenu;

	public CallbackWString OnGeneric;

	internal CallbackBinder(Callback callback)
	{
		OnWidgetLoaded = callback.OnWidgetLoaded;
		OnStateChange = callback.OnStateChange;
		OnProgress = callback.OnProgress;
		OnLocationChanged = callback.OnLocationChanged;
		OnStatusChange = callback.OnStatusChange;
		OnSecurityChange = callback.OnSecurityChange;
		OnKeyDown = callback.OnClientDomKeyDown;
		OnKeyUp = callback.OnClientDomKeyUp;
		OnKeyPress = callback.OnClientDomKeyPress;
		OnMouseDown = callback.OnClientMouseDown;
		OnMouseUp = callback.OnClientMouseUp;
		OnMouseClick = callback.OnClientMouseClick;
		OnMouseDoubleClick = callback.OnClientMouseDoubleClick;
		OnMouseOver = callback.OnClientMouseOver;
		OnMouseOut = callback.OnClientMouseOut;
		OnActivate = callback.OnClientActivate;
		OnFocus = callback.OnClientFocus;
		OnBlur = callback.OnClientBlur;
		OnAlert = callback.OnAlert;
		OnAlertCheck = callback.OnAlertCheck;
		OnConfirm = callback.OnConfirm;
		OnConfirmCheck = callback.OnConfirmCheck;
		OnConfirmEx = callback.OnConfirmEx;
		OnPrompt = callback.OnPrompt;
		OnPromptUsernameAndPassword = callback.OnPromptUsernameAndPassword;
		OnPromptPassword = callback.OnPromptPassword;
		OnSelect = callback.OnSelect;
		OnLoad = callback.OnLoad;
		OnUnload = callback.OnUnload;
		OnShowContextMenu = callback.OnShowContextMenu;
		OnGeneric = callback.OnGeneric;
	}
}

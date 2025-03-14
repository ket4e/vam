using UnityEngine.Internal;

namespace UnityEngine;

/// <summary>
///   <para>Interface into the native iPhone, Android, Windows Phone and Windows Store Apps on-screen keyboards - it is not available on other platforms.</para>
/// </summary>
public sealed class TouchScreenKeyboard
{
	/// <summary>
	///   <para>The status of the on-screen keyboard.</para>
	/// </summary>
	public enum Status
	{
		/// <summary>
		///   <para>The on-screen keyboard is visible.</para>
		/// </summary>
		Visible,
		/// <summary>
		///   <para>The user has finished providing input.</para>
		/// </summary>
		Done,
		/// <summary>
		///   <para>The on-screen keyboard was canceled.</para>
		/// </summary>
		Canceled,
		/// <summary>
		///   <para>The on-screen keyboard has lost focus.</para>
		/// </summary>
		LostFocus
	}

	/// <summary>
	///   <para>Returns the text displayed by the input field of the keyboard.</para>
	/// </summary>
	public string text
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Will text input field above the keyboard be hidden when the keyboard is on screen?</para>
	/// </summary>
	public static bool hideInput
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Is the keyboard visible or sliding into the position on the screen?</para>
	/// </summary>
	public bool active
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Specifies if input process was finished. (Read Only)</para>
	/// </summary>
	public bool done => true;

	/// <summary>
	///   <para>Specifies if input process was canceled. (Read Only)</para>
	/// </summary>
	public bool wasCanceled => false;

	/// <summary>
	///   <para>Returns the status of the on-screen keyboard. (Read Only)</para>
	/// </summary>
	public Status status => Status.Done;

	private static Rect area => default(Rect);

	private static bool visible => false;

	/// <summary>
	///   <para>Is touch screen keyboard supported.</para>
	/// </summary>
	public static bool isSupported => false;

	/// <summary>
	///   <para>How many characters the keyboard input field is limited to. 0 = infinite.</para>
	/// </summary>
	public int characterLimit
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Specifies whether the TouchScreenKeyboard supports the selection property. (Read Only)</para>
	/// </summary>
	public bool canGetSelection => false;

	/// <summary>
	///   <para>Specifies whether the TouchScreenKeyboard supports the selection property. (Read Only)</para>
	/// </summary>
	public bool canSetSelection => false;

	/// <summary>
	///   <para>Gets or sets the character range of the selected text within the string currently being edited.</para>
	/// </summary>
	public RangeInt selection
	{
		get
		{
			return new RangeInt(0, 0);
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Opens the native keyboard provided by OS on the screen.</para>
	/// </summary>
	/// <param name="text">Text to edit.</param>
	/// <param name="keyboardType">Type of keyboard (eg, any text, numbers only, etc).</param>
	/// <param name="autocorrection">Is autocorrection applied?</param>
	/// <param name="multiline">Can more than one line of text be entered?</param>
	/// <param name="secure">Is the text masked (for passwords, etc)?</param>
	/// <param name="alert">Is the keyboard opened in alert mode?</param>
	/// <param name="textPlaceholder">Text to be used if no other text is present.</param>
	/// <param name="characterLimit">How many characters the keyboard input field is limited to. 0 = infinite. (Android and iOS only)</param>
	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder)
	{
		int num = 0;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	/// <summary>
	///   <para>Opens the native keyboard provided by OS on the screen.</para>
	/// </summary>
	/// <param name="text">Text to edit.</param>
	/// <param name="keyboardType">Type of keyboard (eg, any text, numbers only, etc).</param>
	/// <param name="autocorrection">Is autocorrection applied?</param>
	/// <param name="multiline">Can more than one line of text be entered?</param>
	/// <param name="secure">Is the text masked (for passwords, etc)?</param>
	/// <param name="alert">Is the keyboard opened in alert mode?</param>
	/// <param name="textPlaceholder">Text to be used if no other text is present.</param>
	/// <param name="characterLimit">How many characters the keyboard input field is limited to. 0 = infinite. (Android and iOS only)</param>
	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert)
	{
		int num = 0;
		string textPlaceholder = "";
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	/// <summary>
	///   <para>Opens the native keyboard provided by OS on the screen.</para>
	/// </summary>
	/// <param name="text">Text to edit.</param>
	/// <param name="keyboardType">Type of keyboard (eg, any text, numbers only, etc).</param>
	/// <param name="autocorrection">Is autocorrection applied?</param>
	/// <param name="multiline">Can more than one line of text be entered?</param>
	/// <param name="secure">Is the text masked (for passwords, etc)?</param>
	/// <param name="alert">Is the keyboard opened in alert mode?</param>
	/// <param name="textPlaceholder">Text to be used if no other text is present.</param>
	/// <param name="characterLimit">How many characters the keyboard input field is limited to. 0 = infinite. (Android and iOS only)</param>
	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	/// <summary>
	///   <para>Opens the native keyboard provided by OS on the screen.</para>
	/// </summary>
	/// <param name="text">Text to edit.</param>
	/// <param name="keyboardType">Type of keyboard (eg, any text, numbers only, etc).</param>
	/// <param name="autocorrection">Is autocorrection applied?</param>
	/// <param name="multiline">Can more than one line of text be entered?</param>
	/// <param name="secure">Is the text masked (for passwords, etc)?</param>
	/// <param name="alert">Is the keyboard opened in alert mode?</param>
	/// <param name="textPlaceholder">Text to be used if no other text is present.</param>
	/// <param name="characterLimit">How many characters the keyboard input field is limited to. 0 = infinite. (Android and iOS only)</param>
	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		bool secure = false;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	/// <summary>
	///   <para>Opens the native keyboard provided by OS on the screen.</para>
	/// </summary>
	/// <param name="text">Text to edit.</param>
	/// <param name="keyboardType">Type of keyboard (eg, any text, numbers only, etc).</param>
	/// <param name="autocorrection">Is autocorrection applied?</param>
	/// <param name="multiline">Can more than one line of text be entered?</param>
	/// <param name="secure">Is the text masked (for passwords, etc)?</param>
	/// <param name="alert">Is the keyboard opened in alert mode?</param>
	/// <param name="textPlaceholder">Text to be used if no other text is present.</param>
	/// <param name="characterLimit">How many characters the keyboard input field is limited to. 0 = infinite. (Android and iOS only)</param>
	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		bool secure = false;
		bool multiline = false;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	/// <summary>
	///   <para>Opens the native keyboard provided by OS on the screen.</para>
	/// </summary>
	/// <param name="text">Text to edit.</param>
	/// <param name="keyboardType">Type of keyboard (eg, any text, numbers only, etc).</param>
	/// <param name="autocorrection">Is autocorrection applied?</param>
	/// <param name="multiline">Can more than one line of text be entered?</param>
	/// <param name="secure">Is the text masked (for passwords, etc)?</param>
	/// <param name="alert">Is the keyboard opened in alert mode?</param>
	/// <param name="textPlaceholder">Text to be used if no other text is present.</param>
	/// <param name="characterLimit">How many characters the keyboard input field is limited to. 0 = infinite. (Android and iOS only)</param>
	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		bool secure = false;
		bool multiline = false;
		bool autocorrection = true;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	/// <summary>
	///   <para>Opens the native keyboard provided by OS on the screen.</para>
	/// </summary>
	/// <param name="text">Text to edit.</param>
	/// <param name="keyboardType">Type of keyboard (eg, any text, numbers only, etc).</param>
	/// <param name="autocorrection">Is autocorrection applied?</param>
	/// <param name="multiline">Can more than one line of text be entered?</param>
	/// <param name="secure">Is the text masked (for passwords, etc)?</param>
	/// <param name="alert">Is the keyboard opened in alert mode?</param>
	/// <param name="textPlaceholder">Text to be used if no other text is present.</param>
	/// <param name="characterLimit">How many characters the keyboard input field is limited to. 0 = infinite. (Android and iOS only)</param>
	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		bool secure = false;
		bool multiline = false;
		bool autocorrection = true;
		TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	/// <summary>
	///   <para>Opens the native keyboard provided by OS on the screen.</para>
	/// </summary>
	/// <param name="text">Text to edit.</param>
	/// <param name="keyboardType">Type of keyboard (eg, any text, numbers only, etc).</param>
	/// <param name="autocorrection">Is autocorrection applied?</param>
	/// <param name="multiline">Can more than one line of text be entered?</param>
	/// <param name="secure">Is the text masked (for passwords, etc)?</param>
	/// <param name="alert">Is the keyboard opened in alert mode?</param>
	/// <param name="textPlaceholder">Text to be used if no other text is present.</param>
	/// <param name="characterLimit">How many characters the keyboard input field is limited to. 0 = infinite. (Android and iOS only)</param>
	public static TouchScreenKeyboard Open(string text, [DefaultValue("TouchScreenKeyboardType.Default")] TouchScreenKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder, [DefaultValue("0")] int characterLimit)
	{
		return null;
	}
}

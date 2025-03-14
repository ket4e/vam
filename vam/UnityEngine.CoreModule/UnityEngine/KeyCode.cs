namespace UnityEngine;

/// <summary>
///   <para>Key codes returned by Event.keyCode. These map directly to a physical key on the keyboard.</para>
/// </summary>
public enum KeyCode
{
	/// <summary>
	///   <para>Not assigned (never returned as the result of a keystroke).</para>
	/// </summary>
	None = 0,
	/// <summary>
	///   <para>The backspace key.</para>
	/// </summary>
	Backspace = 8,
	/// <summary>
	///   <para>The forward delete key.</para>
	/// </summary>
	Delete = 127,
	/// <summary>
	///   <para>The tab key.</para>
	/// </summary>
	Tab = 9,
	/// <summary>
	///   <para>The Clear key.</para>
	/// </summary>
	Clear = 12,
	/// <summary>
	///   <para>Return key.</para>
	/// </summary>
	Return = 13,
	/// <summary>
	///   <para>Pause on PC machines.</para>
	/// </summary>
	Pause = 19,
	/// <summary>
	///   <para>Escape key.</para>
	/// </summary>
	Escape = 27,
	/// <summary>
	///   <para>Space key.</para>
	/// </summary>
	Space = 32,
	/// <summary>
	///   <para>Numeric keypad 0.</para>
	/// </summary>
	Keypad0 = 256,
	/// <summary>
	///   <para>Numeric keypad 1.</para>
	/// </summary>
	Keypad1 = 257,
	/// <summary>
	///   <para>Numeric keypad 2.</para>
	/// </summary>
	Keypad2 = 258,
	/// <summary>
	///   <para>Numeric keypad 3.</para>
	/// </summary>
	Keypad3 = 259,
	/// <summary>
	///   <para>Numeric keypad 4.</para>
	/// </summary>
	Keypad4 = 260,
	/// <summary>
	///   <para>Numeric keypad 5.</para>
	/// </summary>
	Keypad5 = 261,
	/// <summary>
	///   <para>Numeric keypad 6.</para>
	/// </summary>
	Keypad6 = 262,
	/// <summary>
	///   <para>Numeric keypad 7.</para>
	/// </summary>
	Keypad7 = 263,
	/// <summary>
	///   <para>Numeric keypad 8.</para>
	/// </summary>
	Keypad8 = 264,
	/// <summary>
	///   <para>Numeric keypad 9.</para>
	/// </summary>
	Keypad9 = 265,
	/// <summary>
	///   <para>Numeric keypad '.'.</para>
	/// </summary>
	KeypadPeriod = 266,
	/// <summary>
	///   <para>Numeric keypad '/'.</para>
	/// </summary>
	KeypadDivide = 267,
	/// <summary>
	///   <para>Numeric keypad '*'.</para>
	/// </summary>
	KeypadMultiply = 268,
	/// <summary>
	///   <para>Numeric keypad '-'.</para>
	/// </summary>
	KeypadMinus = 269,
	/// <summary>
	///   <para>Numeric keypad '+'.</para>
	/// </summary>
	KeypadPlus = 270,
	/// <summary>
	///   <para>Numeric keypad enter.</para>
	/// </summary>
	KeypadEnter = 271,
	/// <summary>
	///   <para>Numeric keypad '='.</para>
	/// </summary>
	KeypadEquals = 272,
	/// <summary>
	///   <para>Up arrow key.</para>
	/// </summary>
	UpArrow = 273,
	/// <summary>
	///   <para>Down arrow key.</para>
	/// </summary>
	DownArrow = 274,
	/// <summary>
	///   <para>Right arrow key.</para>
	/// </summary>
	RightArrow = 275,
	/// <summary>
	///   <para>Left arrow key.</para>
	/// </summary>
	LeftArrow = 276,
	/// <summary>
	///   <para>Insert key key.</para>
	/// </summary>
	Insert = 277,
	/// <summary>
	///   <para>Home key.</para>
	/// </summary>
	Home = 278,
	/// <summary>
	///   <para>End key.</para>
	/// </summary>
	End = 279,
	/// <summary>
	///   <para>Page up.</para>
	/// </summary>
	PageUp = 280,
	/// <summary>
	///   <para>Page down.</para>
	/// </summary>
	PageDown = 281,
	/// <summary>
	///   <para>F1 function key.</para>
	/// </summary>
	F1 = 282,
	/// <summary>
	///   <para>F2 function key.</para>
	/// </summary>
	F2 = 283,
	/// <summary>
	///   <para>F3 function key.</para>
	/// </summary>
	F3 = 284,
	/// <summary>
	///   <para>F4 function key.</para>
	/// </summary>
	F4 = 285,
	/// <summary>
	///   <para>F5 function key.</para>
	/// </summary>
	F5 = 286,
	/// <summary>
	///   <para>F6 function key.</para>
	/// </summary>
	F6 = 287,
	/// <summary>
	///   <para>F7 function key.</para>
	/// </summary>
	F7 = 288,
	/// <summary>
	///   <para>F8 function key.</para>
	/// </summary>
	F8 = 289,
	/// <summary>
	///   <para>F9 function key.</para>
	/// </summary>
	F9 = 290,
	/// <summary>
	///   <para>F10 function key.</para>
	/// </summary>
	F10 = 291,
	/// <summary>
	///   <para>F11 function key.</para>
	/// </summary>
	F11 = 292,
	/// <summary>
	///   <para>F12 function key.</para>
	/// </summary>
	F12 = 293,
	/// <summary>
	///   <para>F13 function key.</para>
	/// </summary>
	F13 = 294,
	/// <summary>
	///   <para>F14 function key.</para>
	/// </summary>
	F14 = 295,
	/// <summary>
	///   <para>F15 function key.</para>
	/// </summary>
	F15 = 296,
	/// <summary>
	///   <para>The '0' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha0 = 48,
	/// <summary>
	///   <para>The '1' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha1 = 49,
	/// <summary>
	///   <para>The '2' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha2 = 50,
	/// <summary>
	///   <para>The '3' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha3 = 51,
	/// <summary>
	///   <para>The '4' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha4 = 52,
	/// <summary>
	///   <para>The '5' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha5 = 53,
	/// <summary>
	///   <para>The '6' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha6 = 54,
	/// <summary>
	///   <para>The '7' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha7 = 55,
	/// <summary>
	///   <para>The '8' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha8 = 56,
	/// <summary>
	///   <para>The '9' key on the top of the alphanumeric keyboard.</para>
	/// </summary>
	Alpha9 = 57,
	/// <summary>
	///   <para>Exclamation mark key '!'.</para>
	/// </summary>
	Exclaim = 33,
	/// <summary>
	///   <para>Double quote key '"'.</para>
	/// </summary>
	DoubleQuote = 34,
	/// <summary>
	///   <para>Hash key '#'.</para>
	/// </summary>
	Hash = 35,
	/// <summary>
	///   <para>Dollar sign key '$'.</para>
	/// </summary>
	Dollar = 36,
	/// <summary>
	///   <para>Ampersand key '&amp;'.</para>
	/// </summary>
	Ampersand = 38,
	/// <summary>
	///   <para>Quote key '.</para>
	/// </summary>
	Quote = 39,
	/// <summary>
	///   <para>Left Parenthesis key '('.</para>
	/// </summary>
	LeftParen = 40,
	/// <summary>
	///   <para>Right Parenthesis key ')'.</para>
	/// </summary>
	RightParen = 41,
	/// <summary>
	///   <para>Asterisk key '*'.</para>
	/// </summary>
	Asterisk = 42,
	/// <summary>
	///   <para>Plus key '+'.</para>
	/// </summary>
	Plus = 43,
	/// <summary>
	///   <para>Comma ',' key.</para>
	/// </summary>
	Comma = 44,
	/// <summary>
	///   <para>Minus '-' key.</para>
	/// </summary>
	Minus = 45,
	/// <summary>
	///   <para>Period '.' key.</para>
	/// </summary>
	Period = 46,
	/// <summary>
	///   <para>Slash '/' key.</para>
	/// </summary>
	Slash = 47,
	/// <summary>
	///   <para>Colon ':' key.</para>
	/// </summary>
	Colon = 58,
	/// <summary>
	///   <para>Semicolon ';' key.</para>
	/// </summary>
	Semicolon = 59,
	/// <summary>
	///   <para>Less than '&lt;' key.</para>
	/// </summary>
	Less = 60,
	/// <summary>
	///   <para>Equals '=' key.</para>
	/// </summary>
	Equals = 61,
	/// <summary>
	///   <para>Greater than '&gt;' key.</para>
	/// </summary>
	Greater = 62,
	/// <summary>
	///   <para>Question mark '?' key.</para>
	/// </summary>
	Question = 63,
	/// <summary>
	///   <para>At key '@'.</para>
	/// </summary>
	At = 64,
	/// <summary>
	///   <para>Left square bracket key '['.</para>
	/// </summary>
	LeftBracket = 91,
	/// <summary>
	///   <para>Backslash key '\'.</para>
	/// </summary>
	Backslash = 92,
	/// <summary>
	///   <para>Right square bracket key ']'.</para>
	/// </summary>
	RightBracket = 93,
	/// <summary>
	///   <para>Caret key '^'.</para>
	/// </summary>
	Caret = 94,
	/// <summary>
	///   <para>Underscore '_' key.</para>
	/// </summary>
	Underscore = 95,
	/// <summary>
	///   <para>Back quote key '`'.</para>
	/// </summary>
	BackQuote = 96,
	/// <summary>
	///   <para>'a' key.</para>
	/// </summary>
	A = 97,
	/// <summary>
	///   <para>'b' key.</para>
	/// </summary>
	B = 98,
	/// <summary>
	///   <para>'c' key.</para>
	/// </summary>
	C = 99,
	/// <summary>
	///   <para>'d' key.</para>
	/// </summary>
	D = 100,
	/// <summary>
	///   <para>'e' key.</para>
	/// </summary>
	E = 101,
	/// <summary>
	///   <para>'f' key.</para>
	/// </summary>
	F = 102,
	/// <summary>
	///   <para>'g' key.</para>
	/// </summary>
	G = 103,
	/// <summary>
	///   <para>'h' key.</para>
	/// </summary>
	H = 104,
	/// <summary>
	///   <para>'i' key.</para>
	/// </summary>
	I = 105,
	/// <summary>
	///   <para>'j' key.</para>
	/// </summary>
	J = 106,
	/// <summary>
	///   <para>'k' key.</para>
	/// </summary>
	K = 107,
	/// <summary>
	///   <para>'l' key.</para>
	/// </summary>
	L = 108,
	/// <summary>
	///   <para>'m' key.</para>
	/// </summary>
	M = 109,
	/// <summary>
	///   <para>'n' key.</para>
	/// </summary>
	N = 110,
	/// <summary>
	///   <para>'o' key.</para>
	/// </summary>
	O = 111,
	/// <summary>
	///   <para>'p' key.</para>
	/// </summary>
	P = 112,
	/// <summary>
	///   <para>'q' key.</para>
	/// </summary>
	Q = 113,
	/// <summary>
	///   <para>'r' key.</para>
	/// </summary>
	R = 114,
	/// <summary>
	///   <para>'s' key.</para>
	/// </summary>
	S = 115,
	/// <summary>
	///   <para>'t' key.</para>
	/// </summary>
	T = 116,
	/// <summary>
	///   <para>'u' key.</para>
	/// </summary>
	U = 117,
	/// <summary>
	///   <para>'v' key.</para>
	/// </summary>
	V = 118,
	/// <summary>
	///   <para>'w' key.</para>
	/// </summary>
	W = 119,
	/// <summary>
	///   <para>'x' key.</para>
	/// </summary>
	X = 120,
	/// <summary>
	///   <para>'y' key.</para>
	/// </summary>
	Y = 121,
	/// <summary>
	///   <para>'z' key.</para>
	/// </summary>
	Z = 122,
	/// <summary>
	///   <para>Numlock key.</para>
	/// </summary>
	Numlock = 300,
	/// <summary>
	///   <para>Capslock key.</para>
	/// </summary>
	CapsLock = 301,
	/// <summary>
	///   <para>Scroll lock key.</para>
	/// </summary>
	ScrollLock = 302,
	/// <summary>
	///   <para>Right shift key.</para>
	/// </summary>
	RightShift = 303,
	/// <summary>
	///   <para>Left shift key.</para>
	/// </summary>
	LeftShift = 304,
	/// <summary>
	///   <para>Right Control key.</para>
	/// </summary>
	RightControl = 305,
	/// <summary>
	///   <para>Left Control key.</para>
	/// </summary>
	LeftControl = 306,
	/// <summary>
	///   <para>Right Alt key.</para>
	/// </summary>
	RightAlt = 307,
	/// <summary>
	///   <para>Left Alt key.</para>
	/// </summary>
	LeftAlt = 308,
	/// <summary>
	///   <para>Left Command key.</para>
	/// </summary>
	LeftCommand = 310,
	/// <summary>
	///   <para>Left Command key.</para>
	/// </summary>
	LeftApple = 310,
	/// <summary>
	///   <para>Left Windows key.</para>
	/// </summary>
	LeftWindows = 311,
	/// <summary>
	///   <para>Right Command key.</para>
	/// </summary>
	RightCommand = 309,
	/// <summary>
	///   <para>Right Command key.</para>
	/// </summary>
	RightApple = 309,
	/// <summary>
	///   <para>Right Windows key.</para>
	/// </summary>
	RightWindows = 312,
	/// <summary>
	///   <para>Alt Gr key.</para>
	/// </summary>
	AltGr = 313,
	/// <summary>
	///   <para>Help key.</para>
	/// </summary>
	Help = 315,
	/// <summary>
	///   <para>Print key.</para>
	/// </summary>
	Print = 316,
	/// <summary>
	///   <para>Sys Req key.</para>
	/// </summary>
	SysReq = 317,
	/// <summary>
	///   <para>Break key.</para>
	/// </summary>
	Break = 318,
	/// <summary>
	///   <para>Menu key.</para>
	/// </summary>
	Menu = 319,
	/// <summary>
	///   <para>The Left (or primary) mouse button.</para>
	/// </summary>
	Mouse0 = 323,
	/// <summary>
	///   <para>Right mouse button (or secondary mouse button).</para>
	/// </summary>
	Mouse1 = 324,
	/// <summary>
	///   <para>Middle mouse button (or third button).</para>
	/// </summary>
	Mouse2 = 325,
	/// <summary>
	///   <para>Additional (fourth) mouse button.</para>
	/// </summary>
	Mouse3 = 326,
	/// <summary>
	///   <para>Additional (fifth) mouse button.</para>
	/// </summary>
	Mouse4 = 327,
	/// <summary>
	///   <para>Additional (or sixth) mouse button.</para>
	/// </summary>
	Mouse5 = 328,
	/// <summary>
	///   <para>Additional (or seventh) mouse button.</para>
	/// </summary>
	Mouse6 = 329,
	/// <summary>
	///   <para>Button 0 on any joystick.</para>
	/// </summary>
	JoystickButton0 = 330,
	/// <summary>
	///   <para>Button 1 on any joystick.</para>
	/// </summary>
	JoystickButton1 = 331,
	/// <summary>
	///   <para>Button 2 on any joystick.</para>
	/// </summary>
	JoystickButton2 = 332,
	/// <summary>
	///   <para>Button 3 on any joystick.</para>
	/// </summary>
	JoystickButton3 = 333,
	/// <summary>
	///   <para>Button 4 on any joystick.</para>
	/// </summary>
	JoystickButton4 = 334,
	/// <summary>
	///   <para>Button 5 on any joystick.</para>
	/// </summary>
	JoystickButton5 = 335,
	/// <summary>
	///   <para>Button 6 on any joystick.</para>
	/// </summary>
	JoystickButton6 = 336,
	/// <summary>
	///   <para>Button 7 on any joystick.</para>
	/// </summary>
	JoystickButton7 = 337,
	/// <summary>
	///   <para>Button 8 on any joystick.</para>
	/// </summary>
	JoystickButton8 = 338,
	/// <summary>
	///   <para>Button 9 on any joystick.</para>
	/// </summary>
	JoystickButton9 = 339,
	/// <summary>
	///   <para>Button 10 on any joystick.</para>
	/// </summary>
	JoystickButton10 = 340,
	/// <summary>
	///   <para>Button 11 on any joystick.</para>
	/// </summary>
	JoystickButton11 = 341,
	/// <summary>
	///   <para>Button 12 on any joystick.</para>
	/// </summary>
	JoystickButton12 = 342,
	/// <summary>
	///   <para>Button 13 on any joystick.</para>
	/// </summary>
	JoystickButton13 = 343,
	/// <summary>
	///   <para>Button 14 on any joystick.</para>
	/// </summary>
	JoystickButton14 = 344,
	/// <summary>
	///   <para>Button 15 on any joystick.</para>
	/// </summary>
	JoystickButton15 = 345,
	/// <summary>
	///   <para>Button 16 on any joystick.</para>
	/// </summary>
	JoystickButton16 = 346,
	/// <summary>
	///   <para>Button 17 on any joystick.</para>
	/// </summary>
	JoystickButton17 = 347,
	/// <summary>
	///   <para>Button 18 on any joystick.</para>
	/// </summary>
	JoystickButton18 = 348,
	/// <summary>
	///   <para>Button 19 on any joystick.</para>
	/// </summary>
	JoystickButton19 = 349,
	/// <summary>
	///   <para>Button 0 on first joystick.</para>
	/// </summary>
	Joystick1Button0 = 350,
	/// <summary>
	///   <para>Button 1 on first joystick.</para>
	/// </summary>
	Joystick1Button1 = 351,
	/// <summary>
	///   <para>Button 2 on first joystick.</para>
	/// </summary>
	Joystick1Button2 = 352,
	/// <summary>
	///   <para>Button 3 on first joystick.</para>
	/// </summary>
	Joystick1Button3 = 353,
	/// <summary>
	///   <para>Button 4 on first joystick.</para>
	/// </summary>
	Joystick1Button4 = 354,
	/// <summary>
	///   <para>Button 5 on first joystick.</para>
	/// </summary>
	Joystick1Button5 = 355,
	/// <summary>
	///   <para>Button 6 on first joystick.</para>
	/// </summary>
	Joystick1Button6 = 356,
	/// <summary>
	///   <para>Button 7 on first joystick.</para>
	/// </summary>
	Joystick1Button7 = 357,
	/// <summary>
	///   <para>Button 8 on first joystick.</para>
	/// </summary>
	Joystick1Button8 = 358,
	/// <summary>
	///   <para>Button 9 on first joystick.</para>
	/// </summary>
	Joystick1Button9 = 359,
	/// <summary>
	///   <para>Button 10 on first joystick.</para>
	/// </summary>
	Joystick1Button10 = 360,
	/// <summary>
	///   <para>Button 11 on first joystick.</para>
	/// </summary>
	Joystick1Button11 = 361,
	/// <summary>
	///   <para>Button 12 on first joystick.</para>
	/// </summary>
	Joystick1Button12 = 362,
	/// <summary>
	///   <para>Button 13 on first joystick.</para>
	/// </summary>
	Joystick1Button13 = 363,
	/// <summary>
	///   <para>Button 14 on first joystick.</para>
	/// </summary>
	Joystick1Button14 = 364,
	/// <summary>
	///   <para>Button 15 on first joystick.</para>
	/// </summary>
	Joystick1Button15 = 365,
	/// <summary>
	///   <para>Button 16 on first joystick.</para>
	/// </summary>
	Joystick1Button16 = 366,
	/// <summary>
	///   <para>Button 17 on first joystick.</para>
	/// </summary>
	Joystick1Button17 = 367,
	/// <summary>
	///   <para>Button 18 on first joystick.</para>
	/// </summary>
	Joystick1Button18 = 368,
	/// <summary>
	///   <para>Button 19 on first joystick.</para>
	/// </summary>
	Joystick1Button19 = 369,
	/// <summary>
	///   <para>Button 0 on second joystick.</para>
	/// </summary>
	Joystick2Button0 = 370,
	/// <summary>
	///   <para>Button 1 on second joystick.</para>
	/// </summary>
	Joystick2Button1 = 371,
	/// <summary>
	///   <para>Button 2 on second joystick.</para>
	/// </summary>
	Joystick2Button2 = 372,
	/// <summary>
	///   <para>Button 3 on second joystick.</para>
	/// </summary>
	Joystick2Button3 = 373,
	/// <summary>
	///   <para>Button 4 on second joystick.</para>
	/// </summary>
	Joystick2Button4 = 374,
	/// <summary>
	///   <para>Button 5 on second joystick.</para>
	/// </summary>
	Joystick2Button5 = 375,
	/// <summary>
	///   <para>Button 6 on second joystick.</para>
	/// </summary>
	Joystick2Button6 = 376,
	/// <summary>
	///   <para>Button 7 on second joystick.</para>
	/// </summary>
	Joystick2Button7 = 377,
	/// <summary>
	///   <para>Button 8 on second joystick.</para>
	/// </summary>
	Joystick2Button8 = 378,
	/// <summary>
	///   <para>Button 9 on second joystick.</para>
	/// </summary>
	Joystick2Button9 = 379,
	/// <summary>
	///   <para>Button 10 on second joystick.</para>
	/// </summary>
	Joystick2Button10 = 380,
	/// <summary>
	///   <para>Button 11 on second joystick.</para>
	/// </summary>
	Joystick2Button11 = 381,
	/// <summary>
	///   <para>Button 12 on second joystick.</para>
	/// </summary>
	Joystick2Button12 = 382,
	/// <summary>
	///   <para>Button 13 on second joystick.</para>
	/// </summary>
	Joystick2Button13 = 383,
	/// <summary>
	///   <para>Button 14 on second joystick.</para>
	/// </summary>
	Joystick2Button14 = 384,
	/// <summary>
	///   <para>Button 15 on second joystick.</para>
	/// </summary>
	Joystick2Button15 = 385,
	/// <summary>
	///   <para>Button 16 on second joystick.</para>
	/// </summary>
	Joystick2Button16 = 386,
	/// <summary>
	///   <para>Button 17 on second joystick.</para>
	/// </summary>
	Joystick2Button17 = 387,
	/// <summary>
	///   <para>Button 18 on second joystick.</para>
	/// </summary>
	Joystick2Button18 = 388,
	/// <summary>
	///   <para>Button 19 on second joystick.</para>
	/// </summary>
	Joystick2Button19 = 389,
	/// <summary>
	///   <para>Button 0 on third joystick.</para>
	/// </summary>
	Joystick3Button0 = 390,
	/// <summary>
	///   <para>Button 1 on third joystick.</para>
	/// </summary>
	Joystick3Button1 = 391,
	/// <summary>
	///   <para>Button 2 on third joystick.</para>
	/// </summary>
	Joystick3Button2 = 392,
	/// <summary>
	///   <para>Button 3 on third joystick.</para>
	/// </summary>
	Joystick3Button3 = 393,
	/// <summary>
	///   <para>Button 4 on third joystick.</para>
	/// </summary>
	Joystick3Button4 = 394,
	/// <summary>
	///   <para>Button 5 on third joystick.</para>
	/// </summary>
	Joystick3Button5 = 395,
	/// <summary>
	///   <para>Button 6 on third joystick.</para>
	/// </summary>
	Joystick3Button6 = 396,
	/// <summary>
	///   <para>Button 7 on third joystick.</para>
	/// </summary>
	Joystick3Button7 = 397,
	/// <summary>
	///   <para>Button 8 on third joystick.</para>
	/// </summary>
	Joystick3Button8 = 398,
	/// <summary>
	///   <para>Button 9 on third joystick.</para>
	/// </summary>
	Joystick3Button9 = 399,
	/// <summary>
	///   <para>Button 10 on third joystick.</para>
	/// </summary>
	Joystick3Button10 = 400,
	/// <summary>
	///   <para>Button 11 on third joystick.</para>
	/// </summary>
	Joystick3Button11 = 401,
	/// <summary>
	///   <para>Button 12 on third joystick.</para>
	/// </summary>
	Joystick3Button12 = 402,
	/// <summary>
	///   <para>Button 13 on third joystick.</para>
	/// </summary>
	Joystick3Button13 = 403,
	/// <summary>
	///   <para>Button 14 on third joystick.</para>
	/// </summary>
	Joystick3Button14 = 404,
	/// <summary>
	///   <para>Button 15 on third joystick.</para>
	/// </summary>
	Joystick3Button15 = 405,
	/// <summary>
	///   <para>Button 16 on third joystick.</para>
	/// </summary>
	Joystick3Button16 = 406,
	/// <summary>
	///   <para>Button 17 on third joystick.</para>
	/// </summary>
	Joystick3Button17 = 407,
	/// <summary>
	///   <para>Button 18 on third joystick.</para>
	/// </summary>
	Joystick3Button18 = 408,
	/// <summary>
	///   <para>Button 19 on third joystick.</para>
	/// </summary>
	Joystick3Button19 = 409,
	/// <summary>
	///   <para>Button 0 on forth joystick.</para>
	/// </summary>
	Joystick4Button0 = 410,
	/// <summary>
	///   <para>Button 1 on forth joystick.</para>
	/// </summary>
	Joystick4Button1 = 411,
	/// <summary>
	///   <para>Button 2 on forth joystick.</para>
	/// </summary>
	Joystick4Button2 = 412,
	/// <summary>
	///   <para>Button 3 on forth joystick.</para>
	/// </summary>
	Joystick4Button3 = 413,
	/// <summary>
	///   <para>Button 4 on forth joystick.</para>
	/// </summary>
	Joystick4Button4 = 414,
	/// <summary>
	///   <para>Button 5 on forth joystick.</para>
	/// </summary>
	Joystick4Button5 = 415,
	/// <summary>
	///   <para>Button 6 on forth joystick.</para>
	/// </summary>
	Joystick4Button6 = 416,
	/// <summary>
	///   <para>Button 7 on forth joystick.</para>
	/// </summary>
	Joystick4Button7 = 417,
	/// <summary>
	///   <para>Button 8 on forth joystick.</para>
	/// </summary>
	Joystick4Button8 = 418,
	/// <summary>
	///   <para>Button 9 on forth joystick.</para>
	/// </summary>
	Joystick4Button9 = 419,
	/// <summary>
	///   <para>Button 10 on forth joystick.</para>
	/// </summary>
	Joystick4Button10 = 420,
	/// <summary>
	///   <para>Button 11 on forth joystick.</para>
	/// </summary>
	Joystick4Button11 = 421,
	/// <summary>
	///   <para>Button 12 on forth joystick.</para>
	/// </summary>
	Joystick4Button12 = 422,
	/// <summary>
	///   <para>Button 13 on forth joystick.</para>
	/// </summary>
	Joystick4Button13 = 423,
	/// <summary>
	///   <para>Button 14 on forth joystick.</para>
	/// </summary>
	Joystick4Button14 = 424,
	/// <summary>
	///   <para>Button 15 on forth joystick.</para>
	/// </summary>
	Joystick4Button15 = 425,
	/// <summary>
	///   <para>Button 16 on forth joystick.</para>
	/// </summary>
	Joystick4Button16 = 426,
	/// <summary>
	///   <para>Button 17 on forth joystick.</para>
	/// </summary>
	Joystick4Button17 = 427,
	/// <summary>
	///   <para>Button 18 on forth joystick.</para>
	/// </summary>
	Joystick4Button18 = 428,
	/// <summary>
	///   <para>Button 19 on forth joystick.</para>
	/// </summary>
	Joystick4Button19 = 429,
	/// <summary>
	///   <para>Button 0 on fifth joystick.</para>
	/// </summary>
	Joystick5Button0 = 430,
	/// <summary>
	///   <para>Button 1 on fifth joystick.</para>
	/// </summary>
	Joystick5Button1 = 431,
	/// <summary>
	///   <para>Button 2 on fifth joystick.</para>
	/// </summary>
	Joystick5Button2 = 432,
	/// <summary>
	///   <para>Button 3 on fifth joystick.</para>
	/// </summary>
	Joystick5Button3 = 433,
	/// <summary>
	///   <para>Button 4 on fifth joystick.</para>
	/// </summary>
	Joystick5Button4 = 434,
	/// <summary>
	///   <para>Button 5 on fifth joystick.</para>
	/// </summary>
	Joystick5Button5 = 435,
	/// <summary>
	///   <para>Button 6 on fifth joystick.</para>
	/// </summary>
	Joystick5Button6 = 436,
	/// <summary>
	///   <para>Button 7 on fifth joystick.</para>
	/// </summary>
	Joystick5Button7 = 437,
	/// <summary>
	///   <para>Button 8 on fifth joystick.</para>
	/// </summary>
	Joystick5Button8 = 438,
	/// <summary>
	///   <para>Button 9 on fifth joystick.</para>
	/// </summary>
	Joystick5Button9 = 439,
	/// <summary>
	///   <para>Button 10 on fifth joystick.</para>
	/// </summary>
	Joystick5Button10 = 440,
	/// <summary>
	///   <para>Button 11 on fifth joystick.</para>
	/// </summary>
	Joystick5Button11 = 441,
	/// <summary>
	///   <para>Button 12 on fifth joystick.</para>
	/// </summary>
	Joystick5Button12 = 442,
	/// <summary>
	///   <para>Button 13 on fifth joystick.</para>
	/// </summary>
	Joystick5Button13 = 443,
	/// <summary>
	///   <para>Button 14 on fifth joystick.</para>
	/// </summary>
	Joystick5Button14 = 444,
	/// <summary>
	///   <para>Button 15 on fifth joystick.</para>
	/// </summary>
	Joystick5Button15 = 445,
	/// <summary>
	///   <para>Button 16 on fifth joystick.</para>
	/// </summary>
	Joystick5Button16 = 446,
	/// <summary>
	///   <para>Button 17 on fifth joystick.</para>
	/// </summary>
	Joystick5Button17 = 447,
	/// <summary>
	///   <para>Button 18 on fifth joystick.</para>
	/// </summary>
	Joystick5Button18 = 448,
	/// <summary>
	///   <para>Button 19 on fifth joystick.</para>
	/// </summary>
	Joystick5Button19 = 449,
	/// <summary>
	///   <para>Button 0 on sixth joystick.</para>
	/// </summary>
	Joystick6Button0 = 450,
	/// <summary>
	///   <para>Button 1 on sixth joystick.</para>
	/// </summary>
	Joystick6Button1 = 451,
	/// <summary>
	///   <para>Button 2 on sixth joystick.</para>
	/// </summary>
	Joystick6Button2 = 452,
	/// <summary>
	///   <para>Button 3 on sixth joystick.</para>
	/// </summary>
	Joystick6Button3 = 453,
	/// <summary>
	///   <para>Button 4 on sixth joystick.</para>
	/// </summary>
	Joystick6Button4 = 454,
	/// <summary>
	///   <para>Button 5 on sixth joystick.</para>
	/// </summary>
	Joystick6Button5 = 455,
	/// <summary>
	///   <para>Button 6 on sixth joystick.</para>
	/// </summary>
	Joystick6Button6 = 456,
	/// <summary>
	///   <para>Button 7 on sixth joystick.</para>
	/// </summary>
	Joystick6Button7 = 457,
	/// <summary>
	///   <para>Button 8 on sixth joystick.</para>
	/// </summary>
	Joystick6Button8 = 458,
	/// <summary>
	///   <para>Button 9 on sixth joystick.</para>
	/// </summary>
	Joystick6Button9 = 459,
	/// <summary>
	///   <para>Button 10 on sixth joystick.</para>
	/// </summary>
	Joystick6Button10 = 460,
	/// <summary>
	///   <para>Button 11 on sixth joystick.</para>
	/// </summary>
	Joystick6Button11 = 461,
	/// <summary>
	///   <para>Button 12 on sixth joystick.</para>
	/// </summary>
	Joystick6Button12 = 462,
	/// <summary>
	///   <para>Button 13 on sixth joystick.</para>
	/// </summary>
	Joystick6Button13 = 463,
	/// <summary>
	///   <para>Button 14 on sixth joystick.</para>
	/// </summary>
	Joystick6Button14 = 464,
	/// <summary>
	///   <para>Button 15 on sixth joystick.</para>
	/// </summary>
	Joystick6Button15 = 465,
	/// <summary>
	///   <para>Button 16 on sixth joystick.</para>
	/// </summary>
	Joystick6Button16 = 466,
	/// <summary>
	///   <para>Button 17 on sixth joystick.</para>
	/// </summary>
	Joystick6Button17 = 467,
	/// <summary>
	///   <para>Button 18 on sixth joystick.</para>
	/// </summary>
	Joystick6Button18 = 468,
	/// <summary>
	///   <para>Button 19 on sixth joystick.</para>
	/// </summary>
	Joystick6Button19 = 469,
	/// <summary>
	///   <para>Button 0 on seventh joystick.</para>
	/// </summary>
	Joystick7Button0 = 470,
	/// <summary>
	///   <para>Button 1 on seventh joystick.</para>
	/// </summary>
	Joystick7Button1 = 471,
	/// <summary>
	///   <para>Button 2 on seventh joystick.</para>
	/// </summary>
	Joystick7Button2 = 472,
	/// <summary>
	///   <para>Button 3 on seventh joystick.</para>
	/// </summary>
	Joystick7Button3 = 473,
	/// <summary>
	///   <para>Button 4 on seventh joystick.</para>
	/// </summary>
	Joystick7Button4 = 474,
	/// <summary>
	///   <para>Button 5 on seventh joystick.</para>
	/// </summary>
	Joystick7Button5 = 475,
	/// <summary>
	///   <para>Button 6 on seventh joystick.</para>
	/// </summary>
	Joystick7Button6 = 476,
	/// <summary>
	///   <para>Button 7 on seventh joystick.</para>
	/// </summary>
	Joystick7Button7 = 477,
	/// <summary>
	///   <para>Button 8 on seventh joystick.</para>
	/// </summary>
	Joystick7Button8 = 478,
	/// <summary>
	///   <para>Button 9 on seventh joystick.</para>
	/// </summary>
	Joystick7Button9 = 479,
	/// <summary>
	///   <para>Button 10 on seventh joystick.</para>
	/// </summary>
	Joystick7Button10 = 480,
	/// <summary>
	///   <para>Button 11 on seventh joystick.</para>
	/// </summary>
	Joystick7Button11 = 481,
	/// <summary>
	///   <para>Button 12 on seventh joystick.</para>
	/// </summary>
	Joystick7Button12 = 482,
	/// <summary>
	///   <para>Button 13 on seventh joystick.</para>
	/// </summary>
	Joystick7Button13 = 483,
	/// <summary>
	///   <para>Button 14 on seventh joystick.</para>
	/// </summary>
	Joystick7Button14 = 484,
	/// <summary>
	///   <para>Button 15 on seventh joystick.</para>
	/// </summary>
	Joystick7Button15 = 485,
	/// <summary>
	///   <para>Button 16 on seventh joystick.</para>
	/// </summary>
	Joystick7Button16 = 486,
	/// <summary>
	///   <para>Button 17 on seventh joystick.</para>
	/// </summary>
	Joystick7Button17 = 487,
	/// <summary>
	///   <para>Button 18 on seventh joystick.</para>
	/// </summary>
	Joystick7Button18 = 488,
	/// <summary>
	///   <para>Button 19 on seventh joystick.</para>
	/// </summary>
	Joystick7Button19 = 489,
	/// <summary>
	///   <para>Button 0 on eighth joystick.</para>
	/// </summary>
	Joystick8Button0 = 490,
	/// <summary>
	///   <para>Button 1 on eighth joystick.</para>
	/// </summary>
	Joystick8Button1 = 491,
	/// <summary>
	///   <para>Button 2 on eighth joystick.</para>
	/// </summary>
	Joystick8Button2 = 492,
	/// <summary>
	///   <para>Button 3 on eighth joystick.</para>
	/// </summary>
	Joystick8Button3 = 493,
	/// <summary>
	///   <para>Button 4 on eighth joystick.</para>
	/// </summary>
	Joystick8Button4 = 494,
	/// <summary>
	///   <para>Button 5 on eighth joystick.</para>
	/// </summary>
	Joystick8Button5 = 495,
	/// <summary>
	///   <para>Button 6 on eighth joystick.</para>
	/// </summary>
	Joystick8Button6 = 496,
	/// <summary>
	///   <para>Button 7 on eighth joystick.</para>
	/// </summary>
	Joystick8Button7 = 497,
	/// <summary>
	///   <para>Button 8 on eighth joystick.</para>
	/// </summary>
	Joystick8Button8 = 498,
	/// <summary>
	///   <para>Button 9 on eighth joystick.</para>
	/// </summary>
	Joystick8Button9 = 499,
	/// <summary>
	///   <para>Button 10 on eighth joystick.</para>
	/// </summary>
	Joystick8Button10 = 500,
	/// <summary>
	///   <para>Button 11 on eighth joystick.</para>
	/// </summary>
	Joystick8Button11 = 501,
	/// <summary>
	///   <para>Button 12 on eighth joystick.</para>
	/// </summary>
	Joystick8Button12 = 502,
	/// <summary>
	///   <para>Button 13 on eighth joystick.</para>
	/// </summary>
	Joystick8Button13 = 503,
	/// <summary>
	///   <para>Button 14 on eighth joystick.</para>
	/// </summary>
	Joystick8Button14 = 504,
	/// <summary>
	///   <para>Button 15 on eighth joystick.</para>
	/// </summary>
	Joystick8Button15 = 505,
	/// <summary>
	///   <para>Button 16 on eighth joystick.</para>
	/// </summary>
	Joystick8Button16 = 506,
	/// <summary>
	///   <para>Button 17 on eighth joystick.</para>
	/// </summary>
	Joystick8Button17 = 507,
	/// <summary>
	///   <para>Button 18 on eighth joystick.</para>
	/// </summary>
	Joystick8Button18 = 508,
	/// <summary>
	///   <para>Button 19 on eighth joystick.</para>
	/// </summary>
	Joystick8Button19 = 509
}

using System.Collections;
using System.IO;
using System.Text;

namespace System.Windows.Forms.RTF;

internal class RTF
{
	internal const char EOF = '\uffff';

	internal const int NoParam = -1000000;

	internal const int DefaultEncodingCodePage = 1252;

	private TokenClass rtf_class;

	private Major major;

	private Minor minor;

	private int param;

	private string encoded_text;

	private Encoding encoding;

	private int encoding_code_page = 1252;

	private StringBuilder text_buffer;

	private Picture picture;

	private int line_num;

	private int line_pos;

	private char pushed_char;

	private TokenClass pushed_class;

	private Major pushed_major;

	private Minor pushed_minor;

	private int pushed_param;

	private char prev_char;

	private bool bump_line;

	private Font font_list;

	private Charset cur_charset;

	private Stack charset_stack;

	private Style styles;

	private Color colors;

	private Font fonts;

	private StreamReader source;

	private static Hashtable key_table;

	private static KeyStruct[] Keys;

	private DestinationCallback destination_callbacks;

	private ClassCallback class_callbacks;

	public TokenClass TokenClass
	{
		get
		{
			return rtf_class;
		}
		set
		{
			rtf_class = value;
		}
	}

	public Major Major
	{
		get
		{
			return major;
		}
		set
		{
			major = value;
		}
	}

	public Minor Minor
	{
		get
		{
			return minor;
		}
		set
		{
			minor = value;
		}
	}

	public int Param
	{
		get
		{
			return param;
		}
		set
		{
			param = value;
		}
	}

	public string Text
	{
		get
		{
			return text_buffer.ToString();
		}
		set
		{
			if (value == null)
			{
				text_buffer.Length = 0;
			}
			else
			{
				text_buffer = new StringBuilder(value);
			}
		}
	}

	public string EncodedText => encoded_text;

	public Picture Picture
	{
		get
		{
			return picture;
		}
		set
		{
			picture = value;
		}
	}

	public Color Colors
	{
		get
		{
			return colors;
		}
		set
		{
			colors = value;
		}
	}

	public Style Styles
	{
		get
		{
			return styles;
		}
		set
		{
			styles = value;
		}
	}

	public Font Fonts
	{
		get
		{
			return fonts;
		}
		set
		{
			fonts = value;
		}
	}

	public ClassCallback ClassCallback
	{
		get
		{
			return class_callbacks;
		}
		set
		{
			class_callbacks = value;
		}
	}

	public DestinationCallback DestinationCallback
	{
		get
		{
			return destination_callbacks;
		}
		set
		{
			destination_callbacks = value;
		}
	}

	public int LineNumber => line_num;

	public int LinePos => line_pos;

	public RTF(Stream stream)
	{
		source = new StreamReader(stream);
		text_buffer = new StringBuilder(1024);
		rtf_class = TokenClass.None;
		pushed_class = TokenClass.None;
		pushed_char = '\uffff';
		line_num = 0;
		line_pos = 0;
		prev_char = '\uffff';
		bump_line = false;
		font_list = null;
		charset_stack = null;
		cur_charset = new Charset();
		destination_callbacks = new DestinationCallback();
		class_callbacks = new ClassCallback();
		destination_callbacks[Minor.OptDest] = HandleOptDest;
		destination_callbacks[Minor.FontTbl] = ReadFontTbl;
		destination_callbacks[Minor.ColorTbl] = ReadColorTbl;
		destination_callbacks[Minor.StyleSheet] = ReadStyleSheet;
		destination_callbacks[Minor.Info] = ReadInfoGroup;
		destination_callbacks[Minor.Pict] = ReadPictGroup;
		destination_callbacks[Minor.Object] = ReadObjGroup;
	}

	static RTF()
	{
		Keys = KeysInit.Init();
		key_table = new Hashtable(Keys.Length);
		for (int i = 0; i < Keys.Length; i++)
		{
			key_table[Keys[i].Symbol] = Keys[i];
		}
	}

	public void DefaultFont(string name)
	{
		Font font = new Font(this);
		font.Num = 0;
		font.Name = name;
	}

	private char GetChar()
	{
		return GetChar(skipCrLf: true);
	}

	private char GetChar(bool skipCrLf)
	{
		int num;
		bool flag;
		while (true)
		{
			if ((num = source.Read()) != -1)
			{
				text_buffer.Append((char)num);
			}
			if (prev_char == '\uffff')
			{
				bump_line = true;
			}
			flag = bump_line;
			bump_line = false;
			if (!skipCrLf)
			{
				break;
			}
			switch (num)
			{
			case 13:
				bump_line = true;
				text_buffer.Length--;
				continue;
			case 10:
				bump_line = true;
				if (prev_char == '\r')
				{
					flag = false;
				}
				text_buffer.Length--;
				continue;
			}
			break;
		}
		line_pos++;
		if (flag)
		{
			line_num++;
			line_pos = 1;
		}
		prev_char = (char)num;
		return (char)num;
	}

	public void Read()
	{
		while (GetToken() != TokenClass.EOF)
		{
			RouteToken();
		}
	}

	public void RouteToken()
	{
		if (CheckCM(TokenClass.Control, Major.Destination))
		{
			destination_callbacks[minor]?.Invoke(this);
		}
		class_callbacks[rtf_class]?.Invoke(this);
	}

	public void SkipGroup()
	{
		int num = 1;
		while (GetToken() != TokenClass.EOF)
		{
			if (rtf_class != TokenClass.Group)
			{
				continue;
			}
			if (major == Major.BeginGroup)
			{
				num++;
			}
			else if (major == Major.EndGroup)
			{
				num--;
				if (num < 1)
				{
					break;
				}
			}
		}
	}

	public TokenClass GetToken()
	{
		if (pushed_class != TokenClass.None)
		{
			rtf_class = pushed_class;
			major = pushed_major;
			minor = pushed_minor;
			param = pushed_param;
			pushed_class = TokenClass.None;
			return rtf_class;
		}
		GetToken2();
		if (rtf_class == TokenClass.Text)
		{
			minor = (Minor)cur_charset[(int)major];
			if (encoding == null)
			{
				encoding = Encoding.GetEncoding(encoding_code_page);
			}
			encoded_text = new string(encoding.GetChars(new byte[1] { (byte)major }));
		}
		if (cur_charset.Flags == CharsetFlags.None)
		{
			return rtf_class;
		}
		if (CheckCMM(TokenClass.Control, Major.Unicode, Minor.UnicodeAnsiCodepage))
		{
			encoding_code_page = param;
			if (encoding_code_page < 0 || encoding_code_page > 65535)
			{
				encoding_code_page = 1252;
			}
		}
		if ((cur_charset.Flags & CharsetFlags.Read) != 0 && CheckCM(TokenClass.Control, Major.CharSet))
		{
			cur_charset.ReadMap();
		}
		else if ((cur_charset.Flags & CharsetFlags.Switch) != 0 && CheckCMM(TokenClass.Control, Major.CharAttr, Minor.FontNum))
		{
			Font font = Font.GetFont(font_list, param);
			if (font != null)
			{
				if (font.Name.StartsWith("Symbol"))
				{
					cur_charset.ID = CharsetType.Symbol;
				}
				else
				{
					cur_charset.ID = CharsetType.General;
				}
			}
			else if ((cur_charset.Flags & CharsetFlags.Switch) != 0 && rtf_class == TokenClass.Group)
			{
				switch (major)
				{
				case Major.BeginGroup:
					charset_stack.Push(cur_charset);
					break;
				case Major.EndGroup:
					cur_charset = (Charset)charset_stack.Pop();
					break;
				}
			}
		}
		return rtf_class;
	}

	private void GetToken2()
	{
		rtf_class = TokenClass.Unknown;
		param = -1000000;
		text_buffer.Length = 0;
		char @char;
		if (pushed_char != '\uffff')
		{
			@char = pushed_char;
			text_buffer.Append(@char);
			pushed_char = '\uffff';
		}
		else if ((@char = GetChar()) == '\uffff')
		{
			rtf_class = TokenClass.EOF;
			return;
		}
		switch (@char)
		{
		case '{':
			rtf_class = TokenClass.Group;
			major = Major.BeginGroup;
			break;
		case '}':
			rtf_class = TokenClass.Group;
			major = Major.EndGroup;
			break;
		default:
			rtf_class = TokenClass.Text;
			major = (Major)@char;
			break;
		case '\t':
			rtf_class = TokenClass.Control;
			major = Major.SpecialChar;
			minor = Minor.Tab;
			break;
		case '\\':
		{
			if ((@char = GetChar()) == '\uffff')
			{
				break;
			}
			if (!char.IsLetter(@char))
			{
				switch (@char)
				{
				case '\'':
				{
					char char2;
					if ((@char = GetChar()) != '\uffff' && (char2 = GetChar()) != '\uffff')
					{
						rtf_class = TokenClass.Text;
						major = (Major)(ushort)(Convert.ToByte(@char.ToString(), 16) * 16 + Convert.ToByte(char2.ToString(), 16));
					}
					break;
				}
				case ':':
				case '\\':
				case '{':
				case '}':
					rtf_class = TokenClass.Text;
					major = (Major)@char;
					break;
				default:
					Lookup(text_buffer.ToString());
					break;
				}
				break;
			}
			while (char.IsLetter(@char) && (@char = GetChar(skipCrLf: false)) != '\uffff')
			{
			}
			if (@char != '\uffff')
			{
				text_buffer.Length--;
			}
			Lookup(text_buffer.ToString());
			if (@char != '\uffff')
			{
				text_buffer.Append(@char);
			}
			int num = 1;
			if (@char == '-')
			{
				num = -1;
				@char = GetChar();
			}
			if (@char != '\uffff' && char.IsDigit(@char) && minor != Minor.PngBlip)
			{
				param = 0;
				while (char.IsDigit(@char))
				{
					param = param * 10 + Convert.ToByte(@char) - 48;
					if ((@char = GetChar()) == '\uffff')
					{
						break;
					}
				}
				param *= num;
			}
			switch (@char)
			{
			default:
				pushed_char = @char;
				break;
			case '\n':
			case '\r':
			case ' ':
				break;
			case '\uffff':
				return;
			}
			text_buffer.Length--;
			break;
		}
		}
	}

	public void SetToken(TokenClass cl, Major maj, Minor min, int par, string text)
	{
		rtf_class = cl;
		major = maj;
		minor = min;
		param = par;
		if (par == -1000000)
		{
			text_buffer = new StringBuilder(text);
		}
		else
		{
			text_buffer = new StringBuilder(text + par);
		}
	}

	public void UngetToken()
	{
		if (pushed_class != TokenClass.None)
		{
			throw new RTFException(this, "Cannot unget more than one token");
		}
		if (rtf_class == TokenClass.None)
		{
			throw new RTFException(this, "No token to unget");
		}
		pushed_class = rtf_class;
		pushed_major = major;
		pushed_minor = minor;
		pushed_param = param;
	}

	public TokenClass PeekToken()
	{
		GetToken();
		UngetToken();
		return rtf_class;
	}

	public void Lookup(string token)
	{
		object obj = key_table[token.Substring(1)];
		if (obj == null)
		{
			rtf_class = TokenClass.Unknown;
			major = Major - 1;
			minor = Minor - 1;
		}
		else
		{
			KeyStruct keyStruct = (KeyStruct)obj;
			rtf_class = TokenClass.Control;
			major = keyStruct.Major;
			minor = keyStruct.Minor;
		}
	}

	public bool CheckCM(TokenClass rtf_class, Major major)
	{
		if (this.rtf_class == rtf_class && this.major == major)
		{
			return true;
		}
		return false;
	}

	public bool CheckCMM(TokenClass rtf_class, Major major, Minor minor)
	{
		if (this.rtf_class == rtf_class && this.major == major && this.minor == minor)
		{
			return true;
		}
		return false;
	}

	public bool CheckMM(Major major, Minor minor)
	{
		if (this.major == major && this.minor == minor)
		{
			return true;
		}
		return false;
	}

	private void HandleOptDest(RTF rtf)
	{
		int num = 1;
		while (true)
		{
			GetToken();
			if (rtf.CheckCMM(TokenClass.Control, Major.Destination, Minor.Pict))
			{
				ReadPictGroup(rtf);
				break;
			}
			if (rtf.CheckCM(TokenClass.Group, Major.EndGroup) && --num == 0)
			{
				break;
			}
			if (rtf.CheckCM(TokenClass.Group, Major.BeginGroup))
			{
				num++;
			}
		}
	}

	private void ReadFontTbl(RTF rtf)
	{
		int num = -1;
		Font font = null;
		while (true)
		{
			rtf.GetToken();
			if (rtf.CheckCM(TokenClass.Group, Major.EndGroup))
			{
				break;
			}
			if (num < 0)
			{
				if (rtf.CheckCMM(TokenClass.Control, Major.CharAttr, Minor.FontNum))
				{
					num = 1;
				}
				else
				{
					if (!rtf.CheckCM(TokenClass.Group, Major.BeginGroup))
					{
						throw new RTFException(rtf, "Cannot determine format");
					}
					num = 0;
				}
			}
			if (num == 0)
			{
				if (!rtf.CheckCM(TokenClass.Group, Major.BeginGroup))
				{
					throw new RTFException(rtf, "missing \"{\"");
				}
				rtf.GetToken();
			}
			font = new Font(rtf);
			while (rtf.rtf_class != TokenClass.EOF && !rtf.CheckCM(TokenClass.Text, (Major)59) && !rtf.CheckCM(TokenClass.Group, Major.EndGroup))
			{
				if (rtf.rtf_class == TokenClass.Control)
				{
					switch (rtf.major)
					{
					case Major.FontFamily:
						font.Family = (int)rtf.minor;
						break;
					case Major.CharAttr:
					{
						Minor minor = rtf.minor;
						if (minor == Minor.FontNum)
						{
							font.Num = rtf.param;
						}
						break;
					}
					case Major.FontAttr:
						switch (rtf.minor)
						{
						case Minor.FontCharSet:
							font.Charset = (CharsetType)rtf.param;
							break;
						case Minor.FontPitch:
							font.Pitch = rtf.param;
							break;
						case Minor.FontCodePage:
							font.Codepage = rtf.param;
							break;
						case Minor.FTypeNil:
						case Minor.FTypeTrueType:
							font.Type = rtf.param;
							break;
						}
						break;
					}
				}
				else if (rtf.CheckCM(TokenClass.Group, Major.BeginGroup))
				{
					rtf.SkipGroup();
				}
				else if (rtf.rtf_class == TokenClass.Text)
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (rtf.rtf_class != TokenClass.EOF && !rtf.CheckCM(TokenClass.Text, (Major)59) && !rtf.CheckCM(TokenClass.Group, Major.EndGroup) && !rtf.CheckCM(TokenClass.Group, Major.BeginGroup))
					{
						stringBuilder.Append((char)rtf.major);
						rtf.GetToken();
					}
					if (rtf.CheckCM(TokenClass.Group, Major.EndGroup))
					{
						rtf.UngetToken();
					}
					font.Name = stringBuilder.ToString();
					continue;
				}
				rtf.GetToken();
			}
			if (num == 0)
			{
				rtf.GetToken();
				if (!rtf.CheckCM(TokenClass.Group, Major.EndGroup))
				{
					throw new RTFException(rtf, "Missing \"}\"");
				}
			}
		}
		if (font == null)
		{
			throw new RTFException(rtf, "No font created");
		}
		if (font.Num == -1)
		{
			throw new RTFException(rtf, "Missing font number");
		}
		rtf.RouteToken();
	}

	private void ReadColorTbl(RTF rtf)
	{
		int num = 0;
		while (true)
		{
			rtf.GetToken();
			if (rtf.CheckCM(TokenClass.Group, Major.EndGroup))
			{
				break;
			}
			Color color = new Color(rtf);
			color.Num = num++;
			while (rtf.CheckCM(TokenClass.Control, Major.ColorName))
			{
				switch (rtf.minor)
				{
				case Minor.Red:
					color.Red = rtf.param;
					break;
				case Minor.Green:
					color.Green = rtf.param;
					break;
				case Minor.Blue:
					color.Blue = rtf.param;
					break;
				}
				rtf.GetToken();
			}
			if (!rtf.CheckCM(TokenClass.Text, (Major)59))
			{
				throw new RTFException(rtf, "Malformed color entry");
			}
		}
		rtf.RouteToken();
	}

	private void ReadStyleSheet(RTF rtf)
	{
		StringBuilder stringBuilder = new StringBuilder();
		while (true)
		{
			rtf.GetToken();
			if (rtf.CheckCM(TokenClass.Group, Major.EndGroup))
			{
				break;
			}
			Style style = new Style(rtf);
			if (!rtf.CheckCM(TokenClass.Group, Major.BeginGroup))
			{
				throw new RTFException(rtf, "Missing \"{\"");
			}
			while (true)
			{
				rtf.GetToken();
				if (rtf.rtf_class == TokenClass.EOF || rtf.CheckCM(TokenClass.Text, (Major)59))
				{
					break;
				}
				if (rtf.rtf_class == TokenClass.Control)
				{
					if (rtf.CheckMM(Major.ParAttr, Minor.StyleNum))
					{
						style.Num = rtf.param;
						style.Type = StyleType.Paragraph;
					}
					else if (rtf.CheckMM(Major.CharAttr, Minor.CharStyleNum))
					{
						style.Num = rtf.param;
						style.Type = StyleType.Character;
					}
					else if (rtf.CheckMM(Major.StyleAttr, Minor.SectStyleNum))
					{
						style.Num = rtf.param;
						style.Type = StyleType.Section;
					}
					else if (rtf.CheckMM(Major.StyleAttr, Minor.BasedOn))
					{
						style.BasedOn = rtf.param;
					}
					else if (rtf.CheckMM(Major.StyleAttr, Minor.Additive))
					{
						style.Additive = true;
					}
					else if (rtf.CheckMM(Major.StyleAttr, Minor.Next))
					{
						style.NextPar = rtf.param;
					}
					else
					{
						new StyleElement(style, rtf.rtf_class, rtf.major, rtf.minor, rtf.param, rtf.text_buffer.ToString());
					}
				}
				else if (rtf.CheckCM(TokenClass.Group, Major.BeginGroup))
				{
					rtf.SkipGroup();
				}
				else
				{
					if (rtf.rtf_class != TokenClass.Text)
					{
						continue;
					}
					while (rtf.rtf_class == TokenClass.Text)
					{
						if (rtf.major == (Major)59)
						{
							rtf.UngetToken();
							break;
						}
						stringBuilder.Append((char)rtf.major);
						rtf.GetToken();
					}
					style.Name = stringBuilder.ToString();
				}
			}
			rtf.GetToken();
			if (!rtf.CheckCM(TokenClass.Group, Major.EndGroup))
			{
				throw new RTFException(rtf, "Missing EndGroup (\"}\"");
			}
			if (style.Name == null)
			{
				throw new RTFException(rtf, "Style must have name");
			}
			if (style.Num < 0)
			{
				if (!stringBuilder.ToString().StartsWith("Normal") && !stringBuilder.ToString().StartsWith("Standard"))
				{
					throw new RTFException(rtf, "Missing style number");
				}
				style.Num = 0;
			}
			if (style.NextPar == -1)
			{
				style.NextPar = style.Num;
			}
		}
		rtf.RouteToken();
	}

	private void ReadInfoGroup(RTF rtf)
	{
		rtf.SkipGroup();
		rtf.RouteToken();
	}

	private void ReadPictGroup(RTF rtf)
	{
		bool flag = false;
		Picture picture = new Picture();
		while (true)
		{
			rtf.GetToken();
			if (rtf.CheckCM(TokenClass.Group, Major.EndGroup))
			{
				break;
			}
			switch (minor)
			{
			case Minor.PicWid:
			case Minor.PicHt:
				continue;
			case Minor.PngBlip:
				picture.ImageType = minor;
				flag = true;
				break;
			case Minor.WinMetafile:
				picture.ImageType = minor;
				flag = true;
				continue;
			case Minor.PicGoalWid:
				picture.SetWidthFromTwips(param);
				continue;
			case Minor.PicGoalHt:
				picture.SetHeightFromTwips(param);
				continue;
			}
			if (!flag || rtf.rtf_class != TokenClass.Text)
			{
				continue;
			}
			picture.Data.Seek(0L, SeekOrigin.Begin);
			char c = (char)rtf.major;
			while (true)
			{
				if (c == '\n' || c == '\r')
				{
					c = (char)source.Peek();
					if (c != '}')
					{
						c = (char)source.Read();
						continue;
					}
				}
				char c2 = (char)source.Peek();
				if (c2 == '}')
				{
					break;
				}
				c2 = (char)source.Read();
				while (c2 == '\n' || c2 == '\r')
				{
					c2 = (char)source.Peek();
					if (c2 == '}')
					{
						break;
					}
					c2 = (char)source.Read();
				}
				uint num;
				if (char.IsDigit(c))
				{
					num = (uint)(c - 48);
				}
				else if (char.IsLower(c))
				{
					num = (uint)(c - 97 + 10);
				}
				else
				{
					if (!char.IsUpper(c))
					{
						if (c == '\n' || c == '\r')
						{
							continue;
						}
						break;
					}
					num = (uint)(c - 65 + 10);
				}
				uint num2;
				if (char.IsDigit(c2))
				{
					num2 = (uint)(c2 - 48);
				}
				else if (char.IsLower(c2))
				{
					num2 = (uint)(c2 - 97 + 10);
				}
				else
				{
					if (!char.IsUpper(c2))
					{
						if (c2 == '\n' || c2 == '\r')
						{
							continue;
						}
						break;
					}
					num2 = (uint)(c2 - 65 + 10);
				}
				picture.Data.WriteByte((byte)checked(num * 16 + num2));
				c = (char)source.Peek();
				if (c == '}')
				{
					break;
				}
				c = (char)source.Read();
			}
			flag = false;
			break;
		}
		if (picture.ImageType != 0 && !flag)
		{
			this.picture = picture;
			SetToken(TokenClass.Control, Major.PictAttr, picture.ImageType, 0, string.Empty);
		}
	}

	private void ReadObjGroup(RTF rtf)
	{
		rtf.SkipGroup();
		rtf.RouteToken();
	}
}

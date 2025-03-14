using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace System.ComponentModel;

public class MaskedTextProvider : ICloneable
{
	private enum EditState
	{
		None,
		UpperCase,
		LowerCase
	}

	private enum EditType
	{
		DigitRequired,
		DigitOrSpaceOptional,
		DigitOrSpaceOptional_Blank,
		LetterRequired,
		LetterOptional,
		CharacterRequired,
		CharacterOptional,
		AlphanumericRequired,
		AlphanumericOptional,
		DecimalPlaceholder,
		ThousandsPlaceholder,
		TimeSeparator,
		DateSeparator,
		CurrencySymbol,
		Literal
	}

	private class EditPosition
	{
		public MaskedTextProvider Parent;

		public EditType Type;

		public EditState State;

		public char MaskCharacter;

		public char input;

		public char Input
		{
			get
			{
				return input;
			}
			set
			{
				switch (State)
				{
				case EditState.LowerCase:
					input = char.ToLower(value, Parent.Culture);
					break;
				case EditState.UpperCase:
					input = char.ToUpper(value, Parent.Culture);
					break;
				default:
					input = value;
					break;
				}
			}
		}

		public bool FilledIn => Input != '\0';

		public bool Required
		{
			get
			{
				char maskCharacter = MaskCharacter;
				if (maskCharacter == '&' || maskCharacter == '0' || maskCharacter == 'A' || maskCharacter == 'L')
				{
					return true;
				}
				return false;
			}
		}

		public bool Editable
		{
			get
			{
				switch (MaskCharacter)
				{
				case '#':
				case '&':
				case '0':
				case '9':
				case '?':
				case 'A':
				case 'C':
				case 'L':
				case 'a':
					return true;
				default:
					return false;
				}
			}
		}

		public bool Visible
		{
			get
			{
				switch (MaskCharacter)
				{
				case '<':
				case '>':
				case '|':
					return false;
				default:
					return true;
				}
			}
		}

		public string Text
		{
			get
			{
				if (Type == EditType.Literal)
				{
					return MaskCharacter.ToString();
				}
				return MaskCharacter switch
				{
					'.' => Parent.Culture.NumberFormat.NumberDecimalSeparator, 
					',' => Parent.Culture.NumberFormat.NumberGroupSeparator, 
					':' => Parent.Culture.DateTimeFormat.TimeSeparator, 
					'/' => Parent.Culture.DateTimeFormat.DateSeparator, 
					'$' => Parent.Culture.NumberFormat.CurrencySymbol, 
					_ => (!FilledIn) ? Parent.PromptChar.ToString() : Input.ToString(), 
				};
			}
		}

		private EditPosition()
		{
		}

		public EditPosition(MaskedTextProvider Parent, EditType Type, EditState State, char MaskCharacter)
		{
			this.Type = Type;
			this.Parent = Parent;
			this.State = State;
			this.MaskCharacter = MaskCharacter;
		}

		public void Reset()
		{
			input = '\0';
		}

		internal EditPosition Clone()
		{
			EditPosition editPosition = new EditPosition();
			editPosition.Parent = Parent;
			editPosition.Type = Type;
			editPosition.State = State;
			editPosition.MaskCharacter = MaskCharacter;
			editPosition.input = input;
			return editPosition;
		}

		public bool IsAscii(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
		}

		public bool Match(char c, out MaskedTextResultHint resultHint, bool only_test)
		{
			if (!IsValidInputChar(c))
			{
				resultHint = MaskedTextResultHint.InvalidInput;
				return false;
			}
			if (Parent.ResetOnSpace && c == ' ' && Editable)
			{
				resultHint = MaskedTextResultHint.CharacterEscaped;
				if (FilledIn)
				{
					resultHint = MaskedTextResultHint.Success;
					if (!only_test && input != ' ')
					{
						switch (Type)
						{
						case EditType.CharacterRequired:
						case EditType.CharacterOptional:
						case EditType.AlphanumericRequired:
						case EditType.AlphanumericOptional:
							Input = c;
							break;
						default:
							Input = '\0';
							break;
						}
					}
				}
				return true;
			}
			if (Type == EditType.Literal && MaskCharacter == c && Parent.SkipLiterals)
			{
				resultHint = MaskedTextResultHint.Success;
				return true;
			}
			if (!Editable)
			{
				resultHint = MaskedTextResultHint.NonEditPosition;
				return false;
			}
			switch (Type)
			{
			case EditType.AlphanumericRequired:
			case EditType.AlphanumericOptional:
				if (char.IsLetterOrDigit(c))
				{
					if (Parent.AsciiOnly && !IsAscii(c))
					{
						resultHint = MaskedTextResultHint.AsciiCharacterExpected;
						return false;
					}
					if (!only_test)
					{
						Input = c;
					}
					resultHint = MaskedTextResultHint.Success;
					return true;
				}
				resultHint = MaskedTextResultHint.AlphanumericCharacterExpected;
				return false;
			case EditType.CharacterRequired:
			case EditType.CharacterOptional:
				if (Parent.AsciiOnly && !IsAscii(c))
				{
					resultHint = MaskedTextResultHint.LetterExpected;
					return false;
				}
				if (!char.IsControl(c))
				{
					if (!only_test)
					{
						Input = c;
					}
					resultHint = MaskedTextResultHint.Success;
					return true;
				}
				resultHint = MaskedTextResultHint.LetterExpected;
				return false;
			case EditType.DigitOrSpaceOptional:
			case EditType.DigitOrSpaceOptional_Blank:
				if (char.IsDigit(c) || c == ' ')
				{
					if (!only_test)
					{
						Input = c;
					}
					resultHint = MaskedTextResultHint.Success;
					return true;
				}
				resultHint = MaskedTextResultHint.DigitExpected;
				return false;
			case EditType.DigitRequired:
				if (char.IsDigit(c))
				{
					if (!only_test)
					{
						Input = c;
					}
					resultHint = MaskedTextResultHint.Success;
					return true;
				}
				resultHint = MaskedTextResultHint.DigitExpected;
				return false;
			case EditType.LetterRequired:
			case EditType.LetterOptional:
				if (!char.IsLetter(c))
				{
					resultHint = MaskedTextResultHint.LetterExpected;
					return false;
				}
				if (Parent.AsciiOnly && !IsAscii(c))
				{
					resultHint = MaskedTextResultHint.LetterExpected;
					return false;
				}
				if (!only_test)
				{
					Input = c;
				}
				resultHint = MaskedTextResultHint.Success;
				return true;
			default:
				resultHint = MaskedTextResultHint.Unknown;
				return false;
			}
		}
	}

	private bool allow_prompt_as_input;

	private bool ascii_only;

	private CultureInfo culture;

	private bool include_literals;

	private bool include_prompt;

	private bool is_password;

	private string mask;

	private char password_char;

	private char prompt_char;

	private bool reset_on_prompt;

	private bool reset_on_space;

	private bool skip_literals;

	private EditPosition[] edit_positions;

	private static char default_prompt_char;

	private static char default_password_char;

	public bool AllowPromptAsInput => allow_prompt_as_input;

	public bool AsciiOnly => ascii_only;

	public int AssignedEditPositionCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < edit_positions.Length; i++)
			{
				if (edit_positions[i].FilledIn)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int AvailableEditPositionCount
	{
		get
		{
			int num = 0;
			EditPosition[] array = edit_positions;
			foreach (EditPosition editPosition in array)
			{
				if (!editPosition.FilledIn && editPosition.Editable)
				{
					num++;
				}
			}
			return num;
		}
	}

	public CultureInfo Culture => culture;

	public static char DefaultPasswordChar => '*';

	public int EditPositionCount
	{
		get
		{
			int num = 0;
			EditPosition[] array = edit_positions;
			foreach (EditPosition editPosition in array)
			{
				if (editPosition.Editable)
				{
					num++;
				}
			}
			return num;
		}
	}

	public IEnumerator EditPositions
	{
		get
		{
			List<int> list = new List<int>();
			for (int i = 0; i < edit_positions.Length; i++)
			{
				if (edit_positions[i].Editable)
				{
					list.Add(i);
				}
			}
			return list.GetEnumerator();
		}
	}

	public bool IncludeLiterals
	{
		get
		{
			return include_literals;
		}
		set
		{
			include_literals = value;
		}
	}

	public bool IncludePrompt
	{
		get
		{
			return include_prompt;
		}
		set
		{
			include_prompt = value;
		}
	}

	public static int InvalidIndex => -1;

	public bool IsPassword
	{
		get
		{
			return password_char != '\0';
		}
		set
		{
			password_char = (value ? DefaultPasswordChar : '\0');
		}
	}

	public char this[int index]
	{
		get
		{
			if (index < 0 || index >= Length)
			{
				throw new IndexOutOfRangeException(index.ToString());
			}
			return ToString(ignorePasswordChar: true, includePrompt: true, includeLiterals: true, 0, edit_positions.Length)[index];
		}
	}

	public int LastAssignedPosition => FindAssignedEditPositionFrom(edit_positions.Length - 1, direction: false);

	public int Length
	{
		get
		{
			int num = 0;
			for (int i = 0; i < edit_positions.Length; i++)
			{
				if (edit_positions[i].Visible)
				{
					num++;
				}
			}
			return num;
		}
	}

	public string Mask => mask;

	public bool MaskCompleted
	{
		get
		{
			for (int i = 0; i < edit_positions.Length; i++)
			{
				if (edit_positions[i].Required && !edit_positions[i].FilledIn)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool MaskFull
	{
		get
		{
			for (int i = 0; i < edit_positions.Length; i++)
			{
				if (edit_positions[i].Editable && !edit_positions[i].FilledIn)
				{
					return false;
				}
			}
			return true;
		}
	}

	public char PasswordChar
	{
		get
		{
			return password_char;
		}
		set
		{
			password_char = value;
		}
	}

	public char PromptChar
	{
		get
		{
			return prompt_char;
		}
		set
		{
			prompt_char = value;
		}
	}

	public bool ResetOnPrompt
	{
		get
		{
			return reset_on_prompt;
		}
		set
		{
			reset_on_prompt = value;
		}
	}

	public bool ResetOnSpace
	{
		get
		{
			return reset_on_space;
		}
		set
		{
			reset_on_space = value;
		}
	}

	public bool SkipLiterals
	{
		get
		{
			return skip_literals;
		}
		set
		{
			skip_literals = value;
		}
	}

	public MaskedTextProvider(string mask)
		: this(mask, null, allowPromptAsInput: true, default_prompt_char, default_password_char, restrictToAscii: false)
	{
	}

	public MaskedTextProvider(string mask, bool restrictToAscii)
		: this(mask, null, allowPromptAsInput: true, default_prompt_char, default_password_char, restrictToAscii)
	{
	}

	public MaskedTextProvider(string mask, CultureInfo culture)
		: this(mask, culture, allowPromptAsInput: true, default_prompt_char, default_password_char, restrictToAscii: false)
	{
	}

	public MaskedTextProvider(string mask, char passwordChar, bool allowPromptAsInput)
		: this(mask, null, allowPromptAsInput, default_prompt_char, passwordChar, restrictToAscii: false)
	{
	}

	public MaskedTextProvider(string mask, CultureInfo culture, bool restrictToAscii)
		: this(mask, culture, allowPromptAsInput: true, default_prompt_char, default_password_char, restrictToAscii)
	{
	}

	public MaskedTextProvider(string mask, CultureInfo culture, char passwordChar, bool allowPromptAsInput)
		: this(mask, culture, allowPromptAsInput, default_prompt_char, passwordChar, restrictToAscii: false)
	{
	}

	public MaskedTextProvider(string mask, CultureInfo culture, bool allowPromptAsInput, char promptChar, char passwordChar, bool restrictToAscii)
	{
		SetMask(mask);
		if (culture == null)
		{
			this.culture = Thread.CurrentThread.CurrentCulture;
		}
		else
		{
			this.culture = culture;
		}
		allow_prompt_as_input = allowPromptAsInput;
		PromptChar = promptChar;
		PasswordChar = passwordChar;
		ascii_only = restrictToAscii;
		include_literals = true;
		reset_on_prompt = true;
		reset_on_space = true;
		skip_literals = true;
	}

	static MaskedTextProvider()
	{
		default_prompt_char = '_';
	}

	private void SetMask(string mask)
	{
		if (mask == null || mask == string.Empty)
		{
			throw new ArgumentException("The Mask value cannot be null or empty.\r\nParameter name: mask");
		}
		this.mask = mask;
		List<EditPosition> list = new List<EditPosition>(mask.Length);
		EditState state = EditState.None;
		bool flag = false;
		for (int i = 0; i < mask.Length; i++)
		{
			if (flag)
			{
				list.Add(new EditPosition(this, EditType.Literal, state, mask[i]));
				flag = false;
				continue;
			}
			switch (mask[i])
			{
			case '\\':
				flag = true;
				break;
			case '0':
				list.Add(new EditPosition(this, EditType.DigitRequired, state, mask[i]));
				break;
			case '9':
				list.Add(new EditPosition(this, EditType.DigitOrSpaceOptional, state, mask[i]));
				break;
			case '#':
				list.Add(new EditPosition(this, EditType.DigitOrSpaceOptional_Blank, state, mask[i]));
				break;
			case 'L':
				list.Add(new EditPosition(this, EditType.LetterRequired, state, mask[i]));
				break;
			case '?':
				list.Add(new EditPosition(this, EditType.LetterOptional, state, mask[i]));
				break;
			case '&':
				list.Add(new EditPosition(this, EditType.CharacterRequired, state, mask[i]));
				break;
			case 'C':
				list.Add(new EditPosition(this, EditType.CharacterOptional, state, mask[i]));
				break;
			case 'A':
				list.Add(new EditPosition(this, EditType.AlphanumericRequired, state, mask[i]));
				break;
			case 'a':
				list.Add(new EditPosition(this, EditType.AlphanumericOptional, state, mask[i]));
				break;
			case '.':
				list.Add(new EditPosition(this, EditType.DecimalPlaceholder, state, mask[i]));
				break;
			case ',':
				list.Add(new EditPosition(this, EditType.ThousandsPlaceholder, state, mask[i]));
				break;
			case ':':
				list.Add(new EditPosition(this, EditType.TimeSeparator, state, mask[i]));
				break;
			case '/':
				list.Add(new EditPosition(this, EditType.DateSeparator, state, mask[i]));
				break;
			case '$':
				list.Add(new EditPosition(this, EditType.CurrencySymbol, state, mask[i]));
				break;
			case '<':
				state = EditState.LowerCase;
				break;
			case '>':
				state = EditState.UpperCase;
				break;
			case '|':
				state = EditState.None;
				break;
			default:
				list.Add(new EditPosition(this, EditType.Literal, state, mask[i]));
				break;
			}
		}
		edit_positions = list.ToArray();
	}

	private EditPosition[] ClonePositions()
	{
		EditPosition[] array = new EditPosition[edit_positions.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = edit_positions[i].Clone();
		}
		return array;
	}

	private bool AddInternal(string str_input, out int testPosition, out MaskedTextResultHint resultHint, bool only_test)
	{
		EditPosition[] array = ((!only_test) ? edit_positions : ClonePositions());
		if (str_input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (str_input.Length == 0)
		{
			resultHint = MaskedTextResultHint.NoEffect;
			testPosition = LastAssignedPosition + 1;
			return true;
		}
		resultHint = MaskedTextResultHint.Unknown;
		testPosition = 0;
		int num = LastAssignedPosition;
		MaskedTextResultHint resultHint2 = MaskedTextResultHint.Unknown;
		if (num >= array.Length)
		{
			testPosition = num;
			resultHint = MaskedTextResultHint.UnavailableEditPosition;
			return false;
		}
		foreach (char c in str_input)
		{
			num = (testPosition = num + 1);
			if (resultHint2 > resultHint)
			{
				resultHint = resultHint2;
			}
			if (VerifyEscapeChar(c, num))
			{
				resultHint2 = MaskedTextResultHint.CharacterEscaped;
				continue;
			}
			num = (testPosition = FindEditPositionFrom(num, direction: true));
			if (num == InvalidIndex)
			{
				testPosition = array.Length;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return false;
			}
			if (!IsValidInputChar(c))
			{
				testPosition = num;
				resultHint = MaskedTextResultHint.InvalidInput;
				return false;
			}
			if (!array[num].Match(c, out resultHint2, only_test: false))
			{
				testPosition = num;
				resultHint = resultHint2;
				return false;
			}
		}
		if (resultHint2 > resultHint)
		{
			resultHint = resultHint2;
		}
		return true;
	}

	private bool AddInternal(char input, out int testPosition, out MaskedTextResultHint resultHint, bool check_available_positions_first, bool check_escape_char_first)
	{
		testPosition = 0;
		int num = LastAssignedPosition + 1;
		if (check_available_positions_first)
		{
			int i = num;
			bool flag = false;
			for (; i < edit_positions.Length; i++)
			{
				if (edit_positions[i].Editable)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				testPosition = i;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return GetOperationResultFromHint(resultHint);
			}
		}
		if (check_escape_char_first && VerifyEscapeChar(input, num))
		{
			testPosition = num;
			resultHint = MaskedTextResultHint.CharacterEscaped;
			return true;
		}
		num = FindEditPositionFrom(num, direction: true);
		if (num > edit_positions.Length - 1 || num == InvalidIndex)
		{
			testPosition = num;
			resultHint = MaskedTextResultHint.UnavailableEditPosition;
			return GetOperationResultFromHint(resultHint);
		}
		if (!IsValidInputChar(input))
		{
			testPosition = num;
			resultHint = MaskedTextResultHint.InvalidInput;
			return GetOperationResultFromHint(resultHint);
		}
		if (!edit_positions[num].Match(input, out resultHint, only_test: false))
		{
			testPosition = num;
			return GetOperationResultFromHint(resultHint);
		}
		testPosition = num;
		return GetOperationResultFromHint(resultHint);
	}

	private bool VerifyStringInternal(string input, out int testPosition, out MaskedTextResultHint resultHint, int startIndex, bool only_test)
	{
		int position = startIndex;
		resultHint = MaskedTextResultHint.Unknown;
		for (int i = 0; i < input.Length; i++)
		{
			int num = FindEditPositionFrom(position, direction: true);
			if (num == InvalidIndex)
			{
				testPosition = edit_positions.Length;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return false;
			}
			if (!VerifyCharInternal(input[i], num, out var hint, only_test))
			{
				testPosition = num;
				resultHint = hint;
				return false;
			}
			if (hint > resultHint)
			{
				resultHint = hint;
			}
			position = num + 1;
		}
		if (!only_test)
		{
			for (position = FindEditPositionFrom(position, direction: true); position != InvalidIndex; position = FindEditPositionFrom(position + 1, direction: true))
			{
				if (edit_positions[position].FilledIn)
				{
					edit_positions[position].Reset();
					if (resultHint != MaskedTextResultHint.NoEffect)
					{
						resultHint = MaskedTextResultHint.Success;
					}
				}
			}
		}
		if (input.Length > 0)
		{
			testPosition = startIndex + input.Length - 1;
		}
		else
		{
			testPosition = startIndex;
			if (resultHint < MaskedTextResultHint.NoEffect)
			{
				resultHint = MaskedTextResultHint.NoEffect;
			}
		}
		return true;
	}

	private bool VerifyCharInternal(char input, int position, out MaskedTextResultHint hint, bool only_test)
	{
		hint = MaskedTextResultHint.Unknown;
		if (position < 0 || position >= edit_positions.Length)
		{
			hint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (!IsValidInputChar(input))
		{
			hint = MaskedTextResultHint.InvalidInput;
			return false;
		}
		if (input == ' ' && ResetOnSpace && edit_positions[position].Editable && edit_positions[position].FilledIn)
		{
			if (!only_test)
			{
				edit_positions[position].Reset();
			}
			hint = MaskedTextResultHint.SideEffect;
			return true;
		}
		if (edit_positions[position].Editable && edit_positions[position].FilledIn && edit_positions[position].input == input)
		{
			hint = MaskedTextResultHint.NoEffect;
			return true;
		}
		if (SkipLiterals && !edit_positions[position].Editable && edit_positions[position].Text == input.ToString())
		{
			hint = MaskedTextResultHint.CharacterEscaped;
			return true;
		}
		return edit_positions[position].Match(input, out hint, only_test);
	}

	private bool IsInsertableString(string str_input, int position, out int testPosition, out MaskedTextResultHint resultHint)
	{
		int num = position;
		resultHint = MaskedTextResultHint.UnavailableEditPosition;
		testPosition = InvalidIndex;
		foreach (char c in str_input)
		{
			int num2 = FindEditPositionFrom(num, direction: true);
			if (num2 != InvalidIndex && VerifyEscapeChar(c, num2))
			{
				num = num2 + 1;
				continue;
			}
			if (VerifyEscapeChar(c, num))
			{
				num++;
				continue;
			}
			if (num2 == InvalidIndex)
			{
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				testPosition = edit_positions.Length;
				return false;
			}
			testPosition = num2;
			if (!edit_positions[num2].Match(c, out resultHint, only_test: true))
			{
				return false;
			}
			num = num2 + 1;
		}
		resultHint = MaskedTextResultHint.Success;
		return true;
	}

	private bool ShiftPositionsRight(EditPosition[] edit_positions, int start, out int testPosition, out MaskedTextResultHint resultHint)
	{
		int position = FindAssignedEditPositionFrom(edit_positions.Length, direction: false);
		int num = FindUnassignedEditPositionFrom(position, direction: true);
		testPosition = start;
		resultHint = MaskedTextResultHint.Unknown;
		if (num == InvalidIndex)
		{
			testPosition = edit_positions.Length;
			resultHint = MaskedTextResultHint.UnavailableEditPosition;
			return false;
		}
		while (num > start)
		{
			int num2 = FindEditPositionFrom(num - 1, direction: false);
			char input = edit_positions[num2].input;
			if (input == '\0')
			{
				edit_positions[num].input = input;
			}
			else if (!edit_positions[num].Match(input, out resultHint, only_test: false))
			{
				testPosition = num;
				return false;
			}
			num = num2;
		}
		if (num != InvalidIndex)
		{
			edit_positions[num].Reset();
		}
		return true;
	}

	private bool ReplaceInternal(string input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint, bool only_test, bool dont_remove_at_end)
	{
		resultHint = MaskedTextResultHint.Unknown;
		EditPosition[] array = ((!only_test) ? edit_positions : ClonePositions());
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (endPosition >= array.Length)
		{
			testPosition = endPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (startPosition < 0)
		{
			testPosition = startPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (startPosition >= array.Length)
		{
			testPosition = startPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (startPosition > endPosition)
		{
			testPosition = startPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (input.Length == 0)
		{
			return RemoveAtInternal(startPosition, endPosition, out testPosition, out resultHint, only_test);
		}
		int num = startPosition;
		int num2 = num;
		MaskedTextResultHint resultHint2 = MaskedTextResultHint.Unknown;
		testPosition = InvalidIndex;
		for (int i = 0; i < input.Length; i++)
		{
			char c = input[i];
			num2 = num;
			if (VerifyEscapeChar(c, num2))
			{
				if ((array[num2].FilledIn && array[num2].Editable && c == ' ' && ResetOnSpace) || (c == PromptChar && ResetOnPrompt))
				{
					array[num2].Reset();
					resultHint2 = MaskedTextResultHint.SideEffect;
				}
				else
				{
					resultHint2 = MaskedTextResultHint.CharacterEscaped;
				}
			}
			else if (num2 < array.Length && !array[num2].Editable && FindAssignedEditPositionInRange(num2, endPosition, direction: true) == InvalidIndex)
			{
				num2 = FindEditPositionFrom(num2, direction: true);
				if (num2 == InvalidIndex)
				{
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					testPosition = array.Length;
					return false;
				}
				if (!InsertAtInternal(c.ToString(), num2, out testPosition, out resultHint2, only_test))
				{
					resultHint = resultHint2;
					return false;
				}
			}
			else
			{
				num2 = FindEditPositionFrom(num2, direction: true);
				if (num2 == InvalidIndex)
				{
					testPosition = array.Length;
					resultHint = MaskedTextResultHint.UnavailableEditPosition;
					return false;
				}
				if (!IsValidInputChar(c))
				{
					testPosition = num2;
					resultHint = MaskedTextResultHint.InvalidInput;
					return false;
				}
				if (!ReplaceInternal(array, c, num2, out testPosition, out resultHint2, only_test: false))
				{
					resultHint = resultHint2;
					return false;
				}
			}
			if (resultHint2 > resultHint)
			{
				resultHint = resultHint2;
			}
			num = num2 + 1;
		}
		testPosition = num2;
		if (!dont_remove_at_end && num <= endPosition && !RemoveAtInternal(num, endPosition, out var testPosition2, out resultHint2, only_test))
		{
			testPosition = testPosition2;
			resultHint = resultHint2;
			return false;
		}
		if (resultHint2 == MaskedTextResultHint.Success && resultHint < MaskedTextResultHint.SideEffect)
		{
			resultHint = MaskedTextResultHint.SideEffect;
		}
		return true;
	}

	private bool ReplaceInternal(EditPosition[] edit_positions, char input, int position, out int testPosition, out MaskedTextResultHint resultHint, bool only_test)
	{
		testPosition = position;
		if (!IsValidInputChar(input))
		{
			resultHint = MaskedTextResultHint.InvalidInput;
			return false;
		}
		if (VerifyEscapeChar(input, position))
		{
			if ((edit_positions[position].FilledIn && edit_positions[position].Editable && input == ' ' && ResetOnSpace) || (input == PromptChar && ResetOnPrompt))
			{
				edit_positions[position].Reset();
				resultHint = MaskedTextResultHint.SideEffect;
			}
			else
			{
				resultHint = MaskedTextResultHint.CharacterEscaped;
			}
			testPosition = position;
			return true;
		}
		if (!edit_positions[position].Editable)
		{
			resultHint = MaskedTextResultHint.NonEditPosition;
			return false;
		}
		bool filledIn = edit_positions[position].FilledIn;
		if (filledIn && edit_positions[position].input == input)
		{
			if (VerifyEscapeChar(input, position))
			{
				resultHint = MaskedTextResultHint.CharacterEscaped;
			}
			else
			{
				resultHint = MaskedTextResultHint.NoEffect;
			}
		}
		else
		{
			if (input == ' ' && ResetOnSpace)
			{
				if (filledIn)
				{
					resultHint = MaskedTextResultHint.SideEffect;
					edit_positions[position].Reset();
				}
				else
				{
					resultHint = MaskedTextResultHint.CharacterEscaped;
				}
				return true;
			}
			if (VerifyEscapeChar(input, position))
			{
				resultHint = MaskedTextResultHint.SideEffect;
			}
			else
			{
				resultHint = MaskedTextResultHint.Success;
			}
		}
		if (!edit_positions[position].Match(input, out var resultHint2, only_test: false))
		{
			resultHint = resultHint2;
			return false;
		}
		return true;
	}

	private bool RemoveAtInternal(int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint, bool only_testing)
	{
		testPosition = -1;
		resultHint = MaskedTextResultHint.Unknown;
		EditPosition[] array = ((!only_testing) ? edit_positions : ClonePositions());
		if (endPosition < 0 || endPosition >= array.Length)
		{
			testPosition = endPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (startPosition < 0 || startPosition >= array.Length)
		{
			testPosition = startPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (startPosition > endPosition)
		{
			testPosition = startPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		int num = 0;
		for (int i = startPosition; i <= endPosition; i++)
		{
			if (array[i].Editable)
			{
				num++;
			}
		}
		if (num == 0)
		{
			testPosition = startPosition;
			resultHint = MaskedTextResultHint.NoEffect;
			return true;
		}
		for (int num2 = FindEditPositionFrom(startPosition, direction: true); num2 != InvalidIndex; num2 = FindEditPositionFrom(num2 + 1, direction: true))
		{
			int num3 = FindEditPositionFrom(num2 + 1, direction: true);
			for (int j = 1; j < num; j++)
			{
				if (num3 == InvalidIndex)
				{
					break;
				}
				num3 = FindEditPositionFrom(num3 + 1, direction: true);
			}
			if (num3 == InvalidIndex)
			{
				if (array[num2].FilledIn)
				{
					array[num2].Reset();
					resultHint = MaskedTextResultHint.Success;
				}
				else if (resultHint < MaskedTextResultHint.NoEffect)
				{
					resultHint = MaskedTextResultHint.NoEffect;
				}
			}
			else
			{
				if (!array[num3].FilledIn)
				{
					if (array[num2].FilledIn)
					{
						array[num2].Reset();
						resultHint = MaskedTextResultHint.Success;
					}
					else if (resultHint < MaskedTextResultHint.NoEffect)
					{
						resultHint = MaskedTextResultHint.NoEffect;
					}
				}
				else
				{
					MaskedTextResultHint resultHint2 = MaskedTextResultHint.Unknown;
					if (array[num2].FilledIn)
					{
						resultHint = MaskedTextResultHint.Success;
					}
					else if (resultHint < MaskedTextResultHint.SideEffect)
					{
						resultHint = MaskedTextResultHint.SideEffect;
					}
					if (!array[num2].Match(array[num3].input, out resultHint2, only_test: false))
					{
						resultHint = resultHint2;
						testPosition = num2;
						return false;
					}
				}
				array[num3].Reset();
			}
		}
		if (resultHint == MaskedTextResultHint.Unknown)
		{
			resultHint = MaskedTextResultHint.NoEffect;
		}
		testPosition = startPosition;
		return true;
	}

	private bool InsertAtInternal(string str_input, int position, out int testPosition, out MaskedTextResultHint resultHint, bool only_testing)
	{
		testPosition = -1;
		resultHint = MaskedTextResultHint.Unknown;
		EditPosition[] array = ((!only_testing) ? edit_positions : ClonePositions());
		if (position < 0 || position >= array.Length)
		{
			testPosition = 0;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (!IsInsertableString(str_input, position, out testPosition, out resultHint))
		{
			return false;
		}
		resultHint = MaskedTextResultHint.Unknown;
		int num = position;
		for (int i = 0; i < str_input.Length; i++)
		{
			char c = str_input[i];
			int num2 = FindEditPositionFrom(num, direction: true);
			int num3 = FindUnassignedEditPositionFrom(num, direction: true);
			bool flag = false;
			if (VerifyEscapeChar(c, num))
			{
				flag = true;
				if (c.ToString() == array[num].Text)
				{
					if (FindAssignedEditPositionInRange(0, num - 1, direction: true) != InvalidIndex && num3 == InvalidIndex)
					{
						resultHint = MaskedTextResultHint.UnavailableEditPosition;
						testPosition = array.Length;
						return false;
					}
					resultHint = MaskedTextResultHint.CharacterEscaped;
					testPosition = num;
					num++;
					continue;
				}
			}
			if (!flag && num2 == InvalidIndex)
			{
				testPosition = array.Length;
				resultHint = MaskedTextResultHint.UnavailableEditPosition;
				return false;
			}
			if (num2 == InvalidIndex)
			{
				num2 = num;
			}
			bool filledIn = array[num2].FilledIn;
			if (filledIn && !ShiftPositionsRight(array, num2, out testPosition, out resultHint))
			{
				return false;
			}
			testPosition = num2;
			if (flag)
			{
				if (filledIn)
				{
					resultHint = MaskedTextResultHint.Success;
				}
				else if (!array[num2].Editable && c.ToString() == array[num2].Text)
				{
					resultHint = MaskedTextResultHint.CharacterEscaped;
					testPosition = num;
				}
				else
				{
					int num4 = FindEditPositionFrom(num2, direction: true);
					if (num4 == InvalidIndex)
					{
						resultHint = MaskedTextResultHint.UnavailableEditPosition;
						testPosition = array.Length;
						return false;
					}
					resultHint = MaskedTextResultHint.CharacterEscaped;
					if (c.ToString() == array[num].Text)
					{
						testPosition = num;
					}
				}
			}
			else
			{
				if (!array[num2].Match(c, out var resultHint2, only_test: false))
				{
					resultHint = resultHint2;
					return false;
				}
				if (resultHint < resultHint2)
				{
					resultHint = resultHint2;
				}
			}
			num = num2 + 1;
		}
		return true;
	}

	public bool Add(char input)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return Add(input, out testPosition, out resultHint);
	}

	public bool Add(string input)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return Add(input, out testPosition, out resultHint);
	}

	public bool Add(char input, out int testPosition, out MaskedTextResultHint resultHint)
	{
		return AddInternal(input, out testPosition, out resultHint, check_available_positions_first: true, check_escape_char_first: false);
	}

	public bool Add(string input, out int testPosition, out MaskedTextResultHint resultHint)
	{
		bool flag = AddInternal(input, out testPosition, out resultHint, only_test: true);
		if (flag)
		{
			flag = AddInternal(input, out testPosition, out resultHint, only_test: false);
		}
		return flag;
	}

	public void Clear()
	{
		Clear(out var _);
	}

	public void Clear(out MaskedTextResultHint resultHint)
	{
		resultHint = MaskedTextResultHint.NoEffect;
		for (int i = 0; i < edit_positions.Length; i++)
		{
			if (edit_positions[i].Editable && edit_positions[i].FilledIn)
			{
				edit_positions[i].Reset();
				resultHint = MaskedTextResultHint.Success;
			}
		}
	}

	public object Clone()
	{
		MaskedTextProvider maskedTextProvider = new MaskedTextProvider(mask);
		maskedTextProvider.allow_prompt_as_input = allow_prompt_as_input;
		maskedTextProvider.ascii_only = ascii_only;
		maskedTextProvider.culture = culture;
		maskedTextProvider.edit_positions = ClonePositions();
		maskedTextProvider.include_literals = include_literals;
		maskedTextProvider.include_prompt = include_prompt;
		maskedTextProvider.is_password = is_password;
		maskedTextProvider.mask = mask;
		maskedTextProvider.password_char = password_char;
		maskedTextProvider.prompt_char = prompt_char;
		maskedTextProvider.reset_on_prompt = reset_on_prompt;
		maskedTextProvider.reset_on_space = reset_on_space;
		maskedTextProvider.skip_literals = skip_literals;
		return maskedTextProvider;
	}

	public int FindAssignedEditPositionFrom(int position, bool direction)
	{
		if (direction)
		{
			return FindAssignedEditPositionInRange(position, edit_positions.Length - 1, direction);
		}
		return FindAssignedEditPositionInRange(0, position, direction);
	}

	public int FindAssignedEditPositionInRange(int startPosition, int endPosition, bool direction)
	{
		if (startPosition < 0)
		{
			startPosition = 0;
		}
		if (endPosition >= edit_positions.Length)
		{
			endPosition = edit_positions.Length - 1;
		}
		if (startPosition > endPosition)
		{
			return InvalidIndex;
		}
		int num = (direction ? 1 : (-1));
		int num2 = ((!direction) ? endPosition : startPosition);
		int num3 = ((!direction) ? startPosition : endPosition) + num;
		for (int i = num2; i != num3; i += num)
		{
			if (edit_positions[i].Editable && edit_positions[i].FilledIn)
			{
				return i;
			}
		}
		return InvalidIndex;
	}

	public int FindEditPositionFrom(int position, bool direction)
	{
		if (direction)
		{
			return FindEditPositionInRange(position, edit_positions.Length - 1, direction);
		}
		return FindEditPositionInRange(0, position, direction);
	}

	public int FindEditPositionInRange(int startPosition, int endPosition, bool direction)
	{
		if (startPosition < 0)
		{
			startPosition = 0;
		}
		if (endPosition >= edit_positions.Length)
		{
			endPosition = edit_positions.Length - 1;
		}
		if (startPosition > endPosition)
		{
			return InvalidIndex;
		}
		int num = (direction ? 1 : (-1));
		int num2 = ((!direction) ? endPosition : startPosition);
		int num3 = ((!direction) ? startPosition : endPosition) + num;
		for (int i = num2; i != num3; i += num)
		{
			if (edit_positions[i].Editable)
			{
				return i;
			}
		}
		return InvalidIndex;
	}

	public int FindNonEditPositionFrom(int position, bool direction)
	{
		if (direction)
		{
			return FindNonEditPositionInRange(position, edit_positions.Length - 1, direction);
		}
		return FindNonEditPositionInRange(0, position, direction);
	}

	public int FindNonEditPositionInRange(int startPosition, int endPosition, bool direction)
	{
		if (startPosition < 0)
		{
			startPosition = 0;
		}
		if (endPosition >= edit_positions.Length)
		{
			endPosition = edit_positions.Length - 1;
		}
		if (startPosition > endPosition)
		{
			return InvalidIndex;
		}
		int num = (direction ? 1 : (-1));
		int num2 = ((!direction) ? endPosition : startPosition);
		int num3 = ((!direction) ? startPosition : endPosition) + num;
		for (int i = num2; i != num3; i += num)
		{
			if (!edit_positions[i].Editable)
			{
				return i;
			}
		}
		return InvalidIndex;
	}

	public int FindUnassignedEditPositionFrom(int position, bool direction)
	{
		if (direction)
		{
			return FindUnassignedEditPositionInRange(position, edit_positions.Length - 1, direction);
		}
		return FindUnassignedEditPositionInRange(0, position, direction);
	}

	public int FindUnassignedEditPositionInRange(int startPosition, int endPosition, bool direction)
	{
		if (startPosition < 0)
		{
			startPosition = 0;
		}
		if (endPosition >= edit_positions.Length)
		{
			endPosition = edit_positions.Length - 1;
		}
		if (startPosition > endPosition)
		{
			return InvalidIndex;
		}
		int num = (direction ? 1 : (-1));
		int num2 = ((!direction) ? endPosition : startPosition);
		int num3 = ((!direction) ? startPosition : endPosition) + num;
		for (int i = num2; i != num3; i += num)
		{
			if (edit_positions[i].Editable && !edit_positions[i].FilledIn)
			{
				return i;
			}
		}
		return InvalidIndex;
	}

	public static bool GetOperationResultFromHint(MaskedTextResultHint hint)
	{
		return hint == MaskedTextResultHint.CharacterEscaped || hint == MaskedTextResultHint.NoEffect || hint == MaskedTextResultHint.SideEffect || hint == MaskedTextResultHint.Success;
	}

	public bool InsertAt(char input, int position)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return InsertAt(input, position, out testPosition, out resultHint);
	}

	public bool InsertAt(string input, int position)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return InsertAt(input, position, out testPosition, out resultHint);
	}

	public bool InsertAt(char input, int position, out int testPosition, out MaskedTextResultHint resultHint)
	{
		return InsertAt(input.ToString(), position, out testPosition, out resultHint);
	}

	public bool InsertAt(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (position >= edit_positions.Length)
		{
			testPosition = position;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (input == string.Empty)
		{
			testPosition = position;
			resultHint = MaskedTextResultHint.NoEffect;
			return true;
		}
		bool flag = InsertAtInternal(input, position, out testPosition, out resultHint, only_testing: true);
		if (flag)
		{
			flag = InsertAtInternal(input, position, out testPosition, out resultHint, only_testing: false);
		}
		return flag;
	}

	public bool IsAvailablePosition(int position)
	{
		if (position < 0 || position >= edit_positions.Length)
		{
			return false;
		}
		return edit_positions[position].Editable && !edit_positions[position].FilledIn;
	}

	public bool IsEditPosition(int position)
	{
		if (position < 0 || position >= edit_positions.Length)
		{
			return false;
		}
		return edit_positions[position].Editable;
	}

	public static bool IsValidInputChar(char c)
	{
		return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' ';
	}

	public static bool IsValidMaskChar(char c)
	{
		return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' ';
	}

	public static bool IsValidPasswordChar(char c)
	{
		return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || c == ' ' || c == '\0';
	}

	public bool Remove()
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return Remove(out testPosition, out resultHint);
	}

	public bool Remove(out int testPosition, out MaskedTextResultHint resultHint)
	{
		if (LastAssignedPosition == InvalidIndex)
		{
			resultHint = MaskedTextResultHint.NoEffect;
			testPosition = 0;
			return true;
		}
		testPosition = LastAssignedPosition;
		resultHint = MaskedTextResultHint.Success;
		edit_positions[LastAssignedPosition].input = '\0';
		return true;
	}

	public bool RemoveAt(int position)
	{
		return RemoveAt(position, position);
	}

	public bool RemoveAt(int startPosition, int endPosition)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return RemoveAt(startPosition, endPosition, out testPosition, out resultHint);
	}

	public bool RemoveAt(int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
	{
		bool flag = RemoveAtInternal(startPosition, endPosition, out testPosition, out resultHint, only_testing: true);
		if (flag)
		{
			flag = RemoveAtInternal(startPosition, endPosition, out testPosition, out resultHint, only_testing: false);
		}
		return flag;
	}

	public bool Replace(char input, int position)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return Replace(input, position, out testPosition, out resultHint);
	}

	public bool Replace(string input, int position)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return Replace(input, position, out testPosition, out resultHint);
	}

	public bool Replace(char input, int position, out int testPosition, out MaskedTextResultHint resultHint)
	{
		if (position < 0 || position >= edit_positions.Length)
		{
			testPosition = position;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (VerifyEscapeChar(input, position))
		{
			if ((edit_positions[position].FilledIn && edit_positions[position].Editable && input == ' ' && ResetOnSpace) || (input == PromptChar && ResetOnPrompt))
			{
				edit_positions[position].Reset();
				resultHint = MaskedTextResultHint.SideEffect;
			}
			else
			{
				resultHint = MaskedTextResultHint.CharacterEscaped;
			}
			testPosition = position;
			return true;
		}
		int num = FindEditPositionFrom(position, direction: true);
		if (num == InvalidIndex)
		{
			testPosition = position;
			resultHint = MaskedTextResultHint.UnavailableEditPosition;
			return false;
		}
		if (!IsValidInputChar(input))
		{
			testPosition = num;
			resultHint = MaskedTextResultHint.InvalidInput;
			return false;
		}
		return ReplaceInternal(edit_positions, input, num, out testPosition, out resultHint, only_test: false);
	}

	public bool Replace(string input, int position, out int testPosition, out MaskedTextResultHint resultHint)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (position < 0 || position >= edit_positions.Length)
		{
			testPosition = position;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (input.Length == 0)
		{
			return RemoveAt(position, position, out testPosition, out resultHint);
		}
		bool flag = ReplaceInternal(input, position, edit_positions.Length - 1, out testPosition, out resultHint, only_test: true, dont_remove_at_end: true);
		if (flag)
		{
			flag = ReplaceInternal(input, position, edit_positions.Length - 1, out testPosition, out resultHint, only_test: false, dont_remove_at_end: true);
		}
		return flag;
	}

	public bool Replace(char input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
	{
		if (endPosition >= edit_positions.Length)
		{
			testPosition = endPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (startPosition < 0)
		{
			testPosition = startPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (startPosition > endPosition)
		{
			testPosition = startPosition;
			resultHint = MaskedTextResultHint.PositionOutOfRange;
			return false;
		}
		if (startPosition == endPosition)
		{
			return ReplaceInternal(edit_positions, input, startPosition, out testPosition, out resultHint, only_test: false);
		}
		return Replace(input.ToString(), startPosition, endPosition, out testPosition, out resultHint);
	}

	public bool Replace(string input, int startPosition, int endPosition, out int testPosition, out MaskedTextResultHint resultHint)
	{
		bool flag = ReplaceInternal(input, startPosition, endPosition, out testPosition, out resultHint, only_test: true, dont_remove_at_end: false);
		if (flag)
		{
			flag = ReplaceInternal(input, startPosition, endPosition, out testPosition, out resultHint, only_test: false, dont_remove_at_end: false);
		}
		return flag;
	}

	public bool Set(string input)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return Set(input, out testPosition, out resultHint);
	}

	public bool Set(string input, out int testPosition, out MaskedTextResultHint resultHint)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		bool flag = VerifyStringInternal(input, out testPosition, out resultHint, 0, only_test: true);
		if (flag)
		{
			flag = VerifyStringInternal(input, out testPosition, out resultHint, 0, only_test: false);
		}
		return flag;
	}

	public string ToDisplayString()
	{
		return ToString(ignorePasswordChar: false, includePrompt: true, includeLiterals: true, 0, Length);
	}

	public override string ToString()
	{
		return ToString(ignorePasswordChar: true, IncludePrompt, IncludeLiterals, 0, Length);
	}

	public string ToString(bool ignorePasswordChar)
	{
		return ToString(ignorePasswordChar, IncludePrompt, IncludeLiterals, 0, Length);
	}

	public string ToString(bool includePrompt, bool includeLiterals)
	{
		return ToString(ignorePasswordChar: true, includePrompt, includeLiterals, 0, Length);
	}

	public string ToString(int startPosition, int length)
	{
		return ToString(ignorePasswordChar: true, IncludePrompt, IncludeLiterals, startPosition, length);
	}

	public string ToString(bool ignorePasswordChar, int startPosition, int length)
	{
		return ToString(ignorePasswordChar, IncludePrompt, IncludeLiterals, startPosition, length);
	}

	public string ToString(bool includePrompt, bool includeLiterals, int startPosition, int length)
	{
		return ToString(ignorePasswordChar: true, includePrompt, includeLiterals, startPosition, length);
	}

	public string ToString(bool ignorePasswordChar, bool includePrompt, bool includeLiterals, int startPosition, int length)
	{
		if (startPosition < 0)
		{
			startPosition = 0;
		}
		if (length <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = startPosition;
		int num2 = startPosition + length - 1;
		if (num2 >= edit_positions.Length)
		{
			num2 = edit_positions.Length - 1;
		}
		int num3 = FindAssignedEditPositionInRange(num, num2, direction: false);
		if (!includePrompt)
		{
			int num4 = FindNonEditPositionInRange(num, num2, direction: false);
			num2 = ((!includeLiterals) ? num3 : ((num3 <= num4) ? num4 : num3));
		}
		for (int i = num; i <= num2; i++)
		{
			EditPosition editPosition = edit_positions[i];
			if (editPosition.Type == EditType.Literal)
			{
				if (includeLiterals)
				{
					stringBuilder.Append(editPosition.Text);
				}
			}
			else if (editPosition.Editable)
			{
				if (IsPassword)
				{
					if (ignorePasswordChar)
					{
						if (!editPosition.FilledIn)
						{
							if (includePrompt)
							{
								stringBuilder.Append(PromptChar);
							}
							else
							{
								stringBuilder.Append(" ");
							}
						}
						else
						{
							stringBuilder.Append(editPosition.Input);
						}
					}
					else
					{
						stringBuilder.Append(PasswordChar);
					}
				}
				else if (!editPosition.FilledIn)
				{
					if (includePrompt)
					{
						stringBuilder.Append(PromptChar);
					}
					else if (includeLiterals)
					{
						stringBuilder.Append(" ");
					}
					else if (num3 != InvalidIndex && num3 > i)
					{
						stringBuilder.Append(" ");
					}
				}
				else
				{
					stringBuilder.Append(editPosition.Text);
				}
			}
			else if (includeLiterals)
			{
				stringBuilder.Append(editPosition.Text);
			}
		}
		return stringBuilder.ToString();
	}

	public bool VerifyChar(char input, int position, out MaskedTextResultHint hint)
	{
		return VerifyCharInternal(input, position, out hint, only_test: true);
	}

	public bool VerifyEscapeChar(char input, int position)
	{
		if (position >= edit_positions.Length || position < 0)
		{
			return false;
		}
		if (!edit_positions[position].Editable)
		{
			if (SkipLiterals)
			{
				return input.ToString() == edit_positions[position].Text;
			}
			return false;
		}
		if (ResetOnSpace && input == ' ')
		{
			return true;
		}
		if (ResetOnPrompt && input == PromptChar)
		{
			return true;
		}
		return false;
	}

	public bool VerifyString(string input)
	{
		int testPosition;
		MaskedTextResultHint resultHint;
		return VerifyString(input, out testPosition, out resultHint);
	}

	public bool VerifyString(string input, out int testPosition, out MaskedTextResultHint resultHint)
	{
		if (input == null || input.Length == 0)
		{
			testPosition = 0;
			resultHint = MaskedTextResultHint.NoEffect;
			return true;
		}
		return VerifyStringInternal(input, out testPosition, out resultHint, 0, only_test: true);
	}
}

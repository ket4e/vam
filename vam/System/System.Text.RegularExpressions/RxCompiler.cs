using System.Collections;
using System.Globalization;

namespace System.Text.RegularExpressions;

internal class RxCompiler : System.Text.RegularExpressions.ICompiler
{
	protected byte[] program = new byte[32];

	protected int curpos;

	private void MakeRoom(int bytes)
	{
		while (curpos + bytes > program.Length)
		{
			int num = program.Length * 2;
			byte[] dst = new byte[num];
			Buffer.BlockCopy(program, 0, dst, 0, program.Length);
			program = dst;
		}
	}

	private void Emit(byte val)
	{
		MakeRoom(1);
		program[curpos] = val;
		curpos++;
	}

	private void Emit(System.Text.RegularExpressions.RxOp opcode)
	{
		Emit((byte)opcode);
	}

	private void Emit(ushort val)
	{
		MakeRoom(2);
		program[curpos] = (byte)val;
		program[curpos + 1] = (byte)(val >> 8);
		curpos += 2;
	}

	private void Emit(int val)
	{
		MakeRoom(4);
		program[curpos] = (byte)val;
		program[curpos + 1] = (byte)(val >> 8);
		program[curpos + 2] = (byte)(val >> 16);
		program[curpos + 3] = (byte)(val >> 24);
		curpos += 4;
	}

	private void BeginLink(System.Text.RegularExpressions.LinkRef lref)
	{
		System.Text.RegularExpressions.RxLinkRef rxLinkRef = lref as System.Text.RegularExpressions.RxLinkRef;
		rxLinkRef.PushInstructionBase(curpos);
	}

	private void EmitLink(System.Text.RegularExpressions.LinkRef lref)
	{
		System.Text.RegularExpressions.RxLinkRef rxLinkRef = lref as System.Text.RegularExpressions.RxLinkRef;
		rxLinkRef.PushOffsetPosition(curpos);
		Emit((ushort)0);
	}

	public void Reset()
	{
		curpos = 0;
	}

	public System.Text.RegularExpressions.IMachineFactory GetMachineFactory()
	{
		byte[] dst = new byte[curpos];
		Buffer.BlockCopy(program, 0, dst, 0, curpos);
		return new System.Text.RegularExpressions.RxInterpreterFactory(dst, null);
	}

	public void EmitFalse()
	{
		Emit(System.Text.RegularExpressions.RxOp.False);
	}

	public void EmitTrue()
	{
		Emit(System.Text.RegularExpressions.RxOp.True);
	}

	public virtual void EmitOp(System.Text.RegularExpressions.RxOp op, bool negate, bool ignore, bool reverse)
	{
		int num = 0;
		if (negate)
		{
			num++;
		}
		if (ignore)
		{
			num += 2;
		}
		if (reverse)
		{
			num += 4;
		}
		Emit((System.Text.RegularExpressions.RxOp)((uint)op + (uint)num));
	}

	public virtual void EmitOpIgnoreReverse(System.Text.RegularExpressions.RxOp op, bool ignore, bool reverse)
	{
		int num = 0;
		if (ignore)
		{
			num++;
		}
		if (reverse)
		{
			num += 2;
		}
		Emit((System.Text.RegularExpressions.RxOp)((uint)op + (uint)num));
	}

	public virtual void EmitOpNegateReverse(System.Text.RegularExpressions.RxOp op, bool negate, bool reverse)
	{
		int num = 0;
		if (negate)
		{
			num++;
		}
		if (reverse)
		{
			num += 2;
		}
		Emit((System.Text.RegularExpressions.RxOp)((uint)op + (uint)num));
	}

	public void EmitCharacter(char c, bool negate, bool ignore, bool reverse)
	{
		if (ignore)
		{
			c = char.ToLower(c);
		}
		if (c < 'Ā')
		{
			EmitOp(System.Text.RegularExpressions.RxOp.Char, negate, ignore, reverse);
			Emit((byte)c);
		}
		else
		{
			EmitOp(System.Text.RegularExpressions.RxOp.UnicodeChar, negate, ignore, reverse);
			Emit(c);
		}
	}

	private void EmitUniCat(UnicodeCategory cat, bool negate, bool reverse)
	{
		EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryUnicode, negate, reverse);
		Emit((byte)cat);
	}

	private void EmitCatGeneral(System.Text.RegularExpressions.Category cat, bool negate, bool reverse)
	{
		EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryGeneral, negate, reverse);
		Emit((byte)cat);
	}

	public void EmitCategory(System.Text.RegularExpressions.Category cat, bool negate, bool reverse)
	{
		switch (cat)
		{
		case System.Text.RegularExpressions.Category.Any:
		case System.Text.RegularExpressions.Category.EcmaAny:
			EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryAny, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.AnySingleline:
			EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryAnySingleline, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.Word:
			EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryWord, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.Digit:
			EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryDigit, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.WhiteSpace:
			EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryWhiteSpace, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.EcmaWord:
			EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryEcmaWord, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.EcmaDigit:
			EmitRange('0', '9', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.EcmaWhiteSpace:
			EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryEcmaWhiteSpace, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSpecials:
			EmitOpNegateReverse(System.Text.RegularExpressions.RxOp.CategoryUnicodeSpecials, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLu:
			EmitUniCat(UnicodeCategory.UppercaseLetter, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLl:
			EmitUniCat(UnicodeCategory.LowercaseLetter, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLt:
			EmitUniCat(UnicodeCategory.TitlecaseLetter, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLm:
			EmitUniCat(UnicodeCategory.ModifierLetter, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLo:
			EmitUniCat(UnicodeCategory.OtherLetter, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMn:
			EmitUniCat(UnicodeCategory.NonSpacingMark, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMe:
			EmitUniCat(UnicodeCategory.EnclosingMark, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMc:
			EmitUniCat(UnicodeCategory.SpacingCombiningMark, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeNd:
			EmitUniCat(UnicodeCategory.DecimalDigitNumber, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeNl:
			EmitUniCat(UnicodeCategory.LetterNumber, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeNo:
			EmitUniCat(UnicodeCategory.OtherNumber, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeZs:
			EmitUniCat(UnicodeCategory.SpaceSeparator, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeZl:
			EmitUniCat(UnicodeCategory.LineSeparator, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeZp:
			EmitUniCat(UnicodeCategory.ParagraphSeparator, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodePd:
			EmitUniCat(UnicodeCategory.DashPunctuation, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodePs:
			EmitUniCat(UnicodeCategory.OpenPunctuation, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodePi:
			EmitUniCat(UnicodeCategory.InitialQuotePunctuation, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodePe:
			EmitUniCat(UnicodeCategory.ClosePunctuation, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodePf:
			EmitUniCat(UnicodeCategory.FinalQuotePunctuation, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodePc:
			EmitUniCat(UnicodeCategory.ConnectorPunctuation, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodePo:
			EmitUniCat(UnicodeCategory.OtherPunctuation, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSm:
			EmitUniCat(UnicodeCategory.MathSymbol, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSc:
			EmitUniCat(UnicodeCategory.CurrencySymbol, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSk:
			EmitUniCat(UnicodeCategory.ModifierSymbol, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSo:
			EmitUniCat(UnicodeCategory.OtherSymbol, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCc:
			EmitUniCat(UnicodeCategory.Control, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCf:
			EmitUniCat(UnicodeCategory.Format, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCo:
			EmitUniCat(UnicodeCategory.PrivateUse, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCs:
			EmitUniCat(UnicodeCategory.Surrogate, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCn:
			EmitUniCat(UnicodeCategory.OtherNotAssigned, negate, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeBasicLatin:
			EmitRange('\0', '\u007f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLatin1Supplement:
			EmitRange('\u0080', 'ÿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLatinExtendedA:
			EmitRange('Ā', 'ſ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLatinExtendedB:
			EmitRange('ƀ', 'ɏ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeIPAExtensions:
			EmitRange('ɐ', 'ʯ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSpacingModifierLetters:
			EmitRange('ʰ', '\u02ff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCombiningDiacriticalMarks:
			EmitRange('\u0300', '\u036f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeGreek:
			EmitRange('Ͱ', 'Ͽ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCyrillic:
			EmitRange('Ѐ', 'ӿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeArmenian:
			EmitRange('\u0530', '֏', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeHebrew:
			EmitRange('\u0590', '\u05ff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeArabic:
			EmitRange('\u0600', 'ۿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSyriac:
			EmitRange('܀', 'ݏ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeThaana:
			EmitRange('ހ', '\u07bf', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeDevanagari:
			EmitRange('\u0900', 'ॿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeBengali:
			EmitRange('ঀ', '\u09ff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeGurmukhi:
			EmitRange('\u0a00', '\u0a7f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeGujarati:
			EmitRange('\u0a80', '\u0aff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeOriya:
			EmitRange('\u0b00', '\u0b7f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeTamil:
			EmitRange('\u0b80', '\u0bff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeTelugu:
			EmitRange('\u0c00', '౿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeKannada:
			EmitRange('ಀ', '\u0cff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMalayalam:
			EmitRange('\u0d00', 'ൿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSinhala:
			EmitRange('\u0d80', '\u0dff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeThai:
			EmitRange('\u0e00', '\u0e7f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLao:
			EmitRange('\u0e80', '\u0eff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeTibetan:
			EmitRange('ༀ', '\u0fff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMyanmar:
			EmitRange('က', '႟', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeGeorgian:
			EmitRange('Ⴀ', 'ჿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeHangulJamo:
			EmitRange('ᄀ', 'ᇿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeEthiopic:
			EmitRange('ሀ', '\u137f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCherokee:
			EmitRange('Ꭰ', '\u13ff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeUnifiedCanadianAboriginalSyllabics:
			EmitRange('᐀', 'ᙿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeOgham:
			EmitRange('\u1680', '\u169f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeRunic:
			EmitRange('ᚠ', '\u16ff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeKhmer:
			EmitRange('ក', '\u17ff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMongolian:
			EmitRange('᠀', '\u18af', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLatinExtendedAdditional:
			EmitRange('Ḁ', 'ỿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeGreekExtended:
			EmitRange('ἀ', '\u1fff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeGeneralPunctuation:
			EmitRange('\u2000', '\u206f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSuperscriptsandSubscripts:
			EmitRange('⁰', '\u209f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCurrencySymbols:
			EmitRange('₠', '\u20cf', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCombiningMarksforSymbols:
			EmitRange('\u20d0', '\u20ff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLetterlikeSymbols:
			EmitRange('℀', '⅏', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeNumberForms:
			EmitRange('⅐', '\u218f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeArrows:
			EmitRange('←', '⇿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMathematicalOperators:
			EmitRange('∀', '⋿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMiscellaneousTechnical:
			EmitRange('⌀', '⏿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeControlPictures:
			EmitRange('␀', '\u243f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeOpticalCharacterRecognition:
			EmitRange('⑀', '\u245f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeEnclosedAlphanumerics:
			EmitRange('①', '⓿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeBoxDrawing:
			EmitRange('─', '╿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeBlockElements:
			EmitRange('▀', '▟', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeGeometricShapes:
			EmitRange('■', '◿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeMiscellaneousSymbols:
			EmitRange('☀', '⛿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeDingbats:
			EmitRange('✀', '➿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeBraillePatterns:
			EmitRange('⠀', '⣿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCJKRadicalsSupplement:
			EmitRange('⺀', '\u2eff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeKangxiRadicals:
			EmitRange('⼀', '\u2fdf', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeIdeographicDescriptionCharacters:
			EmitRange('⿰', '\u2fff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCJKSymbolsandPunctuation:
			EmitRange('\u3000', '〿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeHiragana:
			EmitRange('\u3040', 'ゟ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeKatakana:
			EmitRange('゠', 'ヿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeBopomofo:
			EmitRange('\u3100', 'ㄯ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeHangulCompatibilityJamo:
			EmitRange('\u3130', '\u318f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeKanbun:
			EmitRange('㆐', '㆟', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeBopomofoExtended:
			EmitRange('ㆠ', 'ㆿ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeEnclosedCJKLettersandMonths:
			EmitRange('㈀', '㋿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCJKCompatibility:
			EmitRange('㌀', '㏿', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCJKUnifiedIdeographsExtensionA:
			EmitRange('㐀', '䶵', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCJKUnifiedIdeographs:
			EmitRange('一', '\u9fff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeYiSyllables:
			EmitRange('ꀀ', '\ua48f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeYiRadicals:
			EmitRange('꒐', '\ua4cf', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeHangulSyllables:
			EmitRange('가', '힣', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeHighSurrogates:
			EmitRange('\ud800', '\udb7f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeHighPrivateUseSurrogates:
			EmitRange('\udb80', '\udbff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeLowSurrogates:
			EmitRange('\udc00', '\udfff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodePrivateUse:
			EmitRange('\ue000', '\uf8ff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCJKCompatibilityIdeographs:
			EmitRange('豈', '\ufaff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeAlphabeticPresentationForms:
			EmitRange('ﬀ', 'ﭏ', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeArabicPresentationFormsA:
			EmitRange('ﭐ', '\ufdff', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCombiningHalfMarks:
			EmitRange('\ufe20', '\ufe2f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeCJKCompatibilityForms:
			EmitRange('︰', '\ufe4f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeSmallFormVariants:
			EmitRange('﹐', '\ufe6f', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeArabicPresentationFormsB:
			EmitRange('ﹰ', '\ufefe', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeHalfwidthandFullwidthForms:
			EmitRange('\uff00', '\uffef', negate, ignore: false, reverse);
			break;
		case System.Text.RegularExpressions.Category.UnicodeL:
		case System.Text.RegularExpressions.Category.UnicodeM:
		case System.Text.RegularExpressions.Category.UnicodeN:
		case System.Text.RegularExpressions.Category.UnicodeZ:
		case System.Text.RegularExpressions.Category.UnicodeP:
		case System.Text.RegularExpressions.Category.UnicodeS:
		case System.Text.RegularExpressions.Category.UnicodeC:
			EmitCatGeneral(cat, negate, reverse);
			break;
		default:
			throw new NotImplementedException("Missing category: " + cat);
		}
	}

	public void EmitNotCategory(System.Text.RegularExpressions.Category cat, bool negate, bool reverse)
	{
		if (negate)
		{
			EmitCategory(cat, negate: false, reverse);
		}
		else
		{
			EmitCategory(cat, negate: true, reverse);
		}
	}

	public void EmitRange(char lo, char hi, bool negate, bool ignore, bool reverse)
	{
		if (lo < 'Ā' && hi < 'Ā')
		{
			EmitOp(System.Text.RegularExpressions.RxOp.Range, negate, ignore, reverse);
			Emit((byte)lo);
			Emit((byte)hi);
		}
		else
		{
			EmitOp(System.Text.RegularExpressions.RxOp.UnicodeRange, negate, ignore, reverse);
			Emit(lo);
			Emit(hi);
		}
	}

	public void EmitSet(char lo, BitArray set, bool negate, bool ignore, bool reverse)
	{
		int num = set.Length + 7 >> 3;
		if (lo < 'Ā' && num < 256)
		{
			EmitOp(System.Text.RegularExpressions.RxOp.Bitmap, negate, ignore, reverse);
			Emit((byte)lo);
			Emit((byte)num);
		}
		else
		{
			EmitOp(System.Text.RegularExpressions.RxOp.UnicodeBitmap, negate, ignore, reverse);
			Emit(lo);
			Emit((ushort)num);
		}
		int num2 = 0;
		while (num-- != 0)
		{
			int num3 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (num2 >= set.Length)
				{
					break;
				}
				if (set[num2++])
				{
					num3 |= 1 << (i & 0x1F);
				}
			}
			Emit((byte)num3);
		}
	}

	public void EmitString(string str, bool ignore, bool reverse)
	{
		bool flag = false;
		int num = 0;
		if (ignore)
		{
			num++;
		}
		if (reverse)
		{
			num += 2;
		}
		if (ignore)
		{
			str = str.ToLower();
		}
		if (str.Length < 256)
		{
			flag = true;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] >= 'Ā')
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			EmitOpIgnoreReverse(System.Text.RegularExpressions.RxOp.String, ignore, reverse);
			Emit((byte)str.Length);
			for (int i = 0; i < str.Length; i++)
			{
				Emit((byte)str[i]);
			}
			return;
		}
		EmitOpIgnoreReverse(System.Text.RegularExpressions.RxOp.UnicodeString, ignore, reverse);
		if (str.Length > 65535)
		{
			throw new NotSupportedException();
		}
		Emit((ushort)str.Length);
		for (int i = 0; i < str.Length; i++)
		{
			Emit(str[i]);
		}
	}

	public void EmitPosition(System.Text.RegularExpressions.Position pos)
	{
		switch (pos)
		{
		case System.Text.RegularExpressions.Position.Any:
			Emit(System.Text.RegularExpressions.RxOp.AnyPosition);
			break;
		case System.Text.RegularExpressions.Position.Start:
			Emit(System.Text.RegularExpressions.RxOp.StartOfString);
			break;
		case System.Text.RegularExpressions.Position.StartOfString:
			Emit(System.Text.RegularExpressions.RxOp.StartOfString);
			break;
		case System.Text.RegularExpressions.Position.StartOfLine:
			Emit(System.Text.RegularExpressions.RxOp.StartOfLine);
			break;
		case System.Text.RegularExpressions.Position.StartOfScan:
			Emit(System.Text.RegularExpressions.RxOp.StartOfScan);
			break;
		case System.Text.RegularExpressions.Position.End:
			Emit(System.Text.RegularExpressions.RxOp.End);
			break;
		case System.Text.RegularExpressions.Position.EndOfString:
			Emit(System.Text.RegularExpressions.RxOp.EndOfString);
			break;
		case System.Text.RegularExpressions.Position.EndOfLine:
			Emit(System.Text.RegularExpressions.RxOp.EndOfLine);
			break;
		case System.Text.RegularExpressions.Position.Boundary:
			Emit(System.Text.RegularExpressions.RxOp.WordBoundary);
			break;
		case System.Text.RegularExpressions.Position.NonBoundary:
			Emit(System.Text.RegularExpressions.RxOp.NoWordBoundary);
			break;
		default:
			throw new NotSupportedException();
		}
	}

	public void EmitOpen(int gid)
	{
		if (gid > 65535)
		{
			throw new NotSupportedException();
		}
		Emit(System.Text.RegularExpressions.RxOp.OpenGroup);
		Emit((ushort)gid);
	}

	public void EmitClose(int gid)
	{
		if (gid > 65535)
		{
			throw new NotSupportedException();
		}
		Emit(System.Text.RegularExpressions.RxOp.CloseGroup);
		Emit((ushort)gid);
	}

	public void EmitBalanceStart(int gid, int balance, bool capture, System.Text.RegularExpressions.LinkRef tail)
	{
		BeginLink(tail);
		Emit(System.Text.RegularExpressions.RxOp.BalanceStart);
		Emit((ushort)gid);
		Emit((ushort)balance);
		Emit((byte)(capture ? 1u : 0u));
		EmitLink(tail);
	}

	public void EmitBalance()
	{
		Emit(System.Text.RegularExpressions.RxOp.Balance);
	}

	public void EmitReference(int gid, bool ignore, bool reverse)
	{
		if (gid > 65535)
		{
			throw new NotSupportedException();
		}
		EmitOpIgnoreReverse(System.Text.RegularExpressions.RxOp.Reference, ignore, reverse);
		Emit((ushort)gid);
	}

	public void EmitIfDefined(int gid, System.Text.RegularExpressions.LinkRef tail)
	{
		if (gid > 65535)
		{
			throw new NotSupportedException();
		}
		BeginLink(tail);
		Emit(System.Text.RegularExpressions.RxOp.IfDefined);
		EmitLink(tail);
		Emit((ushort)gid);
	}

	public void EmitSub(System.Text.RegularExpressions.LinkRef tail)
	{
		BeginLink(tail);
		Emit(System.Text.RegularExpressions.RxOp.SubExpression);
		EmitLink(tail);
	}

	public void EmitTest(System.Text.RegularExpressions.LinkRef yes, System.Text.RegularExpressions.LinkRef tail)
	{
		BeginLink(yes);
		BeginLink(tail);
		Emit(System.Text.RegularExpressions.RxOp.Test);
		EmitLink(yes);
		EmitLink(tail);
	}

	public void EmitBranch(System.Text.RegularExpressions.LinkRef next)
	{
		BeginLink(next);
		Emit(System.Text.RegularExpressions.RxOp.Branch);
		EmitLink(next);
	}

	public void EmitJump(System.Text.RegularExpressions.LinkRef target)
	{
		BeginLink(target);
		Emit(System.Text.RegularExpressions.RxOp.Jump);
		EmitLink(target);
	}

	public void EmitIn(System.Text.RegularExpressions.LinkRef tail)
	{
		BeginLink(tail);
		Emit(System.Text.RegularExpressions.RxOp.TestCharGroup);
		EmitLink(tail);
	}

	public void EmitRepeat(int min, int max, bool lazy, System.Text.RegularExpressions.LinkRef until)
	{
		BeginLink(until);
		Emit((!lazy) ? System.Text.RegularExpressions.RxOp.Repeat : System.Text.RegularExpressions.RxOp.RepeatLazy);
		EmitLink(until);
		Emit(min);
		Emit(max);
	}

	public void EmitUntil(System.Text.RegularExpressions.LinkRef repeat)
	{
		ResolveLink(repeat);
		Emit(System.Text.RegularExpressions.RxOp.Until);
	}

	public void EmitInfo(int count, int min, int max)
	{
		Emit(System.Text.RegularExpressions.RxOp.Info);
		if (count > 65535)
		{
			throw new NotSupportedException();
		}
		Emit((ushort)count);
		Emit(min);
		Emit(max);
	}

	public void EmitFastRepeat(int min, int max, bool lazy, System.Text.RegularExpressions.LinkRef tail)
	{
		BeginLink(tail);
		Emit((!lazy) ? System.Text.RegularExpressions.RxOp.FastRepeat : System.Text.RegularExpressions.RxOp.FastRepeatLazy);
		EmitLink(tail);
		Emit(min);
		Emit(max);
	}

	public void EmitAnchor(bool reverse, int offset, System.Text.RegularExpressions.LinkRef tail)
	{
		BeginLink(tail);
		if (reverse)
		{
			Emit(System.Text.RegularExpressions.RxOp.AnchorReverse);
		}
		else
		{
			Emit(System.Text.RegularExpressions.RxOp.Anchor);
		}
		EmitLink(tail);
		if (offset > 65535)
		{
			throw new NotSupportedException();
		}
		Emit((ushort)offset);
	}

	public void EmitBranchEnd()
	{
	}

	public void EmitAlternationEnd()
	{
	}

	public System.Text.RegularExpressions.LinkRef NewLink()
	{
		return new System.Text.RegularExpressions.RxLinkRef();
	}

	public void ResolveLink(System.Text.RegularExpressions.LinkRef link)
	{
		System.Text.RegularExpressions.RxLinkRef rxLinkRef = link as System.Text.RegularExpressions.RxLinkRef;
		for (int i = 0; i < rxLinkRef.current; i += 2)
		{
			int num = curpos - rxLinkRef.offsets[i];
			if (num > 65535)
			{
				throw new NotSupportedException();
			}
			int num2 = rxLinkRef.offsets[i + 1];
			program[num2] = (byte)num;
			program[num2 + 1] = (byte)(num >> 8);
		}
	}
}

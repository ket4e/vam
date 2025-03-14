namespace System.Windows.Forms.RTF;

internal class StandardCharName
{
	public static string[] Names = new string[352]
	{
		"nothing", "space", "exclam", "quotedbl", "numbersign", "dollar", "percent", "ampersand", "quoteright", "parenleft",
		"parenright", "asterisk", "plus", "comma", "hyphen", "period", "slash", "zero", "one", "two",
		"three", "four", "five", "six", "seven", "eight", "nine", "colon", "semicolon", "less",
		"equal", "greater", "question", "at", "A", "B", "C", "D", "E", "F",
		"G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
		"Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
		"bracketleft", "backslash", "bracketright", "asciicircum", "underscore", "quoteleft", "a", "b", "c", "d",
		"e", "f", "g", "h", "i", "j", "k", "l", "m", "n",
		"o", "p", "q", "r", "s", "t", "u", "v", "w", "x",
		"y", "z", "braceleft", "bar", "braceright", "asciitilde", "exclamdown", "cent", "sterling", "fraction",
		"yen", "florin", "section", "currency", "quotedblleft", "guillemotleft", "guilsinglleft", "guilsinglright", "fi", "fl",
		"endash", "dagger", "daggerdbl", "periodcentered", "paragraph", "bullet", "quotesinglbase", "quotedblbase", "quotedblright", "guillemotright",
		"ellipsis", "perthousand", "questiondown", "grave", "acute", "circumflex", "tilde", "macron", "breve", "dotaccent",
		"dieresis", "ring", "cedilla", "hungarumlaut", "ogonek", "caron", "emdash", "AE", "ordfeminine", "Lslash",
		"Oslash", "OE", "ordmasculine", "ae", "dotlessi", "lslash", "oslash", "oe", "germandbls", "Aacute",
		"Acircumflex", "Adieresis", "Agrave", "Aring", "Atilde", "Ccedilla", "Eacute", "Ecircumflex", "Edieresis", "Egrave",
		"Eth", "Iacute", "Icircumflex", "Idieresis", "Igrave", "Ntilde", "Oacute", "Ocircumflex", "Odieresis", "Ograve",
		"Otilde", "Scaron", "Thorn", "Uacute", "Ucircumflex", "Udieresis", "Ugrave", "Yacute", "Ydieresis", "aacute",
		"acircumflex", "adieresis", "agrave", "aring", "atilde", "brokenbar", "ccedilla", "copyright", "degree", "divide",
		"eacute", "ecircumflex", "edieresis", "egrave", "eth", "iacute", "icircumflex", "idieresis", "igrave", "logicalnot",
		"minus", "multiply", "ntilde", "oacute", "ocircumflex", "odieresis", "ograve", "onehalf", "onequarter", "onesuperior",
		"otilde", "plusminus", "registered", "thorn", "threequarters", "threesuperior", "trademark", "twosuperior", "uacute", "ucircumflex",
		"udieresis", "ugrave", "yacute", "ydieresis", "Alpha", "Beta", "Chi", "Delta", "Epsilon", "Phi",
		"Gamma", "Eta", "Iota", "Kappa", "Lambda", "Mu", "Nu", "Omicron", "Pi", "Theta",
		"Rho", "Sigma", "Tau", "Upsilon", "varUpsilon", "Omega", "Xi", "Psi", "Zeta", "alpha",
		"beta", "chi", "delta", "epsilon", "phi", "varphi", "gamma", "eta", "iota", "kappa",
		"lambda", "mu", "nu", "omicron", "pi", "varpi", "theta", "vartheta", "rho", "sigma",
		"varsigma", "tau", "upsilon", "omega", "xi", "psi", "zeta", "nobrkspace", "nobrkhyphen", "lessequal",
		"greaterequal", "infinity", "integral", "notequal", "radical", "radicalex", "approxequal", "apple", "partialdiff", "opthyphen",
		"formula", "lozenge", "universal", "existential", "suchthat", "congruent", "therefore", "perpendicular", "minute", "club",
		"diamond", "heart", "spade", "arrowboth", "arrowleft", "arrowup", "arrowright", "arrowdown", "second", "proportional",
		"equivalence", "arrowvertex", "arrowhorizex", "carriagereturn", "aleph", "Ifraktur", "Rfraktur", "weierstrass", "circlemultiply", "circleplus",
		"emptyset", "intersection", "union", "propersuperset", "reflexsuperset", "notsubset", "propersubset", "reflexsubset", "element", "notelement",
		"angle", "gradient", "product", "logicaland", "logicalor", "arrowdblboth", "arrowdblleft", "arrowdblup", "arrowdblright", "arrowdbldown",
		"angleleft", "registersans", "copyrightsans", "trademarksans", "angleright", "mathplus", "mathminus", "mathasterisk", "mathnumbersign", "dotmath",
		"mathequal", "mathtilde"
	};

	public static string Name(int index)
	{
		if (index < 0 || index >= Names.Length)
		{
			return string.Empty;
		}
		return Names[index];
	}

	public static int ID(string name)
	{
		for (int i = 0; i < Names.Length; i++)
		{
			if (name.Equals(Names[i]))
			{
				return i;
			}
		}
		return 0;
	}
}

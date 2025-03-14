namespace System.Windows.Forms.RTF;

internal class TextMap
{
	private string[] table;

	internal string this[StandardCharCode c]
	{
		get
		{
			return table[(int)c];
		}
		set
		{
			table[(int)c] = value;
		}
	}

	public string[] Table => table;

	public TextMap()
	{
		table = new string[352];
		for (int i = 0; i < 352; i++)
		{
			table[i] = string.Empty;
		}
	}

	public static void SetupStandardTable(string[] table)
	{
		table[290] = "\u0006";
		table[278] = "\u001e";
		table[289] = "\u001f";
		table[1] = " ";
		table[2] = "!";
		table[3] = "\"";
		table[4] = "#";
		table[5] = "$";
		table[6] = "%";
		table[7] = "&";
		table[9] = "(";
		table[10] = ")";
		table[11] = "*";
		table[12] = "+";
		table[13] = ",";
		table[14] = "-";
		table[15] = ".";
		table[16] = "/";
		table[17] = "0";
		table[18] = "1";
		table[19] = "2";
		table[20] = "3";
		table[21] = "4";
		table[22] = "5";
		table[23] = "6";
		table[24] = "7";
		table[25] = "8";
		table[26] = "9";
		table[27] = ":";
		table[28] = ";";
		table[29] = "<";
		table[30] = "=";
		table[31] = ">";
		table[32] = "?";
		table[33] = "@";
		table[34] = "A";
		table[35] = "B";
		table[36] = "C";
		table[37] = "D";
		table[38] = "E";
		table[39] = "F";
		table[40] = "G";
		table[41] = "H";
		table[42] = "I";
		table[43] = "J";
		table[44] = "K";
		table[45] = "L";
		table[46] = "M";
		table[47] = "N";
		table[48] = "O";
		table[49] = "P";
		table[50] = "Q";
		table[51] = "R";
		table[52] = "S";
		table[53] = "T";
		table[54] = "U";
		table[55] = "V";
		table[56] = "W";
		table[57] = "X";
		table[58] = "Y";
		table[59] = "Z";
		table[60] = "[";
		table[61] = "\\";
		table[62] = "]";
		table[63] = "^";
		table[64] = "_";
		table[65] = "`";
		table[66] = "a";
		table[67] = "b";
		table[68] = "c";
		table[69] = "d";
		table[70] = "e";
		table[71] = "f";
		table[72] = "g";
		table[73] = "h";
		table[74] = "i";
		table[75] = "j";
		table[76] = "k";
		table[77] = "l";
		table[78] = "m";
		table[79] = "n";
		table[80] = "o";
		table[81] = "p";
		table[82] = "q";
		table[83] = "r";
		table[84] = "s";
		table[85] = "t";
		table[86] = "u";
		table[87] = "v";
		table[88] = "w";
		table[89] = "x";
		table[90] = "y";
		table[91] = "z";
		table[92] = "{";
		table[93] = "|";
		table[94] = "}";
		table[95] = "~";
		table[277] = "\u00a0";
		table[96] = "¡";
		table[97] = "¢";
		table[98] = "£";
		table[103] = "¤";
		table[100] = "¥";
		table[185] = "¦";
		table[102] = "§";
		table[130] = "\u00a8";
		table[187] = "©";
		table[138] = "ª";
		table[105] = "«";
		table[199] = "¬";
		table[289] = "\u00ad";
		table[212] = "®";
		table[127] = "\u00af";
		table[188] = "°";
		table[211] = "±";
		table[217] = "²";
		table[215] = "³";
		table[124] = "\u00b4";
		table[261] = "µ";
		table[114] = "¶";
		table[113] = "·";
		table[132] = "\u00b8";
		table[209] = "¹";
		table[142] = "º";
		table[119] = "»";
		table[208] = "¼";
		table[207] = "½";
		table[214] = "¾";
		table[122] = "¿";
		table[152] = "À";
		table[149] = "Á";
		table[150] = "Â";
		table[154] = "Ã";
		table[151] = "Ä";
		table[153] = "Å";
		table[137] = "Æ";
		table[155] = "Ç";
		table[159] = "È";
		table[156] = "É";
		table[157] = "Ê";
		table[158] = "Ë";
		table[164] = "Ì";
		table[161] = "Í";
		table[162] = "Î";
		table[163] = "Ï";
		table[160] = "Ð";
		table[165] = "Ñ";
		table[169] = "Ò";
		table[166] = "Ó";
		table[167] = "Ô";
		table[170] = "Õ";
		table[168] = "Ö";
		table[201] = "×";
		table[140] = "Ø";
		table[176] = "Ù";
		table[173] = "Ú";
		table[174] = "Û";
		table[175] = "Ü";
		table[177] = "Ý";
		table[172] = "Þ";
		table[148] = "ß";
		table[182] = "à";
		table[179] = "á";
		table[180] = "â";
		table[184] = "ã";
		table[181] = "ä";
		table[183] = "å";
		table[143] = "æ";
		table[186] = "ç";
		table[193] = "è";
		table[190] = "é";
		table[191] = "ê";
		table[192] = "ë";
		table[198] = "ì";
		table[195] = "í";
		table[196] = "î";
		table[197] = "ï";
		table[194] = "ð";
		table[202] = "ñ";
		table[206] = "ò";
		table[203] = "ó";
		table[204] = "ô";
		table[210] = "õ";
		table[205] = "ö";
		table[189] = "÷";
		table[146] = "ø";
		table[221] = "ù";
		table[218] = "ú";
		table[219] = "û";
		table[220] = "ü";
		table[222] = "ý";
		table[213] = "þ";
		table[223] = "ÿ";
	}
}

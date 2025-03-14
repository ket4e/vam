using System.Collections;

namespace System.Windows.Forms.RTF;

internal class Charcode
{
	private StandardCharCode[] codes;

	private Hashtable reverse;

	private int size;

	private static Charcode ansi_generic;

	public int this[StandardCharCode c]
	{
		get
		{
			object obj = reverse[c];
			if (obj != null)
			{
				return (int)obj;
			}
			for (int i = 0; i < size; i++)
			{
				if (codes[i] == c)
				{
					return i;
				}
			}
			return -1;
		}
	}

	public StandardCharCode this[int c]
	{
		get
		{
			if (c < 0 || c >= size)
			{
				return StandardCharCode.nothing;
			}
			return codes[c];
		}
		private set
		{
			if (c >= 0 && c < size)
			{
				codes[c] = value;
				reverse[value] = c;
			}
		}
	}

	public static Charcode AnsiGeneric
	{
		get
		{
			if (ansi_generic != null)
			{
				return ansi_generic;
			}
			ansi_generic = new Charcode(256);
			ansi_generic[6] = StandardCharCode.formula;
			ansi_generic[30] = StandardCharCode.nobrkhyphen;
			ansi_generic[31] = StandardCharCode.opthyphen;
			ansi_generic[32] = StandardCharCode.space;
			ansi_generic[33] = StandardCharCode.exclam;
			ansi_generic[34] = StandardCharCode.quotedbl;
			ansi_generic[35] = StandardCharCode.numbersign;
			ansi_generic[36] = StandardCharCode.dollar;
			ansi_generic[37] = StandardCharCode.percent;
			ansi_generic[38] = StandardCharCode.ampersand;
			ansi_generic[92] = StandardCharCode.quoteright;
			ansi_generic[40] = StandardCharCode.parenleft;
			ansi_generic[41] = StandardCharCode.parenright;
			ansi_generic[42] = StandardCharCode.asterisk;
			ansi_generic[43] = StandardCharCode.plus;
			ansi_generic[44] = StandardCharCode.comma;
			ansi_generic[45] = StandardCharCode.hyphen;
			ansi_generic[46] = StandardCharCode.period;
			ansi_generic[47] = StandardCharCode.slash;
			ansi_generic[48] = StandardCharCode.zero;
			ansi_generic[49] = StandardCharCode.one;
			ansi_generic[50] = StandardCharCode.two;
			ansi_generic[51] = StandardCharCode.three;
			ansi_generic[52] = StandardCharCode.four;
			ansi_generic[53] = StandardCharCode.five;
			ansi_generic[54] = StandardCharCode.six;
			ansi_generic[55] = StandardCharCode.seven;
			ansi_generic[56] = StandardCharCode.eight;
			ansi_generic[57] = StandardCharCode.nine;
			ansi_generic[58] = StandardCharCode.colon;
			ansi_generic[59] = StandardCharCode.semicolon;
			ansi_generic[60] = StandardCharCode.less;
			ansi_generic[61] = StandardCharCode.equal;
			ansi_generic[62] = StandardCharCode.greater;
			ansi_generic[63] = StandardCharCode.question;
			ansi_generic[64] = StandardCharCode.at;
			ansi_generic[65] = StandardCharCode.A;
			ansi_generic[66] = StandardCharCode.B;
			ansi_generic[67] = StandardCharCode.C;
			ansi_generic[68] = StandardCharCode.D;
			ansi_generic[69] = StandardCharCode.E;
			ansi_generic[70] = StandardCharCode.F;
			ansi_generic[71] = StandardCharCode.G;
			ansi_generic[72] = StandardCharCode.H;
			ansi_generic[73] = StandardCharCode.I;
			ansi_generic[74] = StandardCharCode.J;
			ansi_generic[75] = StandardCharCode.K;
			ansi_generic[76] = StandardCharCode.L;
			ansi_generic[77] = StandardCharCode.M;
			ansi_generic[78] = StandardCharCode.N;
			ansi_generic[79] = StandardCharCode.O;
			ansi_generic[80] = StandardCharCode.P;
			ansi_generic[81] = StandardCharCode.Q;
			ansi_generic[82] = StandardCharCode.R;
			ansi_generic[83] = StandardCharCode.S;
			ansi_generic[84] = StandardCharCode.T;
			ansi_generic[85] = StandardCharCode.U;
			ansi_generic[86] = StandardCharCode.V;
			ansi_generic[87] = StandardCharCode.W;
			ansi_generic[88] = StandardCharCode.X;
			ansi_generic[89] = StandardCharCode.Y;
			ansi_generic[90] = StandardCharCode.Z;
			ansi_generic[91] = StandardCharCode.bracketleft;
			ansi_generic[92] = StandardCharCode.backslash;
			ansi_generic[93] = StandardCharCode.bracketright;
			ansi_generic[94] = StandardCharCode.asciicircum;
			ansi_generic[95] = StandardCharCode.underscore;
			ansi_generic[96] = StandardCharCode.quoteleft;
			ansi_generic[97] = StandardCharCode.a;
			ansi_generic[98] = StandardCharCode.b;
			ansi_generic[99] = StandardCharCode.c;
			ansi_generic[100] = StandardCharCode.d;
			ansi_generic[101] = StandardCharCode.e;
			ansi_generic[102] = StandardCharCode.f;
			ansi_generic[103] = StandardCharCode.g;
			ansi_generic[104] = StandardCharCode.h;
			ansi_generic[105] = StandardCharCode.i;
			ansi_generic[106] = StandardCharCode.j;
			ansi_generic[107] = StandardCharCode.k;
			ansi_generic[108] = StandardCharCode.l;
			ansi_generic[109] = StandardCharCode.m;
			ansi_generic[110] = StandardCharCode.n;
			ansi_generic[111] = StandardCharCode.o;
			ansi_generic[112] = StandardCharCode.p;
			ansi_generic[113] = StandardCharCode.q;
			ansi_generic[114] = StandardCharCode.r;
			ansi_generic[115] = StandardCharCode.s;
			ansi_generic[116] = StandardCharCode.t;
			ansi_generic[117] = StandardCharCode.u;
			ansi_generic[118] = StandardCharCode.v;
			ansi_generic[119] = StandardCharCode.w;
			ansi_generic[120] = StandardCharCode.x;
			ansi_generic[121] = StandardCharCode.y;
			ansi_generic[122] = StandardCharCode.z;
			ansi_generic[123] = StandardCharCode.braceleft;
			ansi_generic[124] = StandardCharCode.bar;
			ansi_generic[125] = StandardCharCode.braceright;
			ansi_generic[126] = StandardCharCode.asciitilde;
			ansi_generic[160] = StandardCharCode.nobrkspace;
			ansi_generic[161] = StandardCharCode.exclamdown;
			ansi_generic[162] = StandardCharCode.cent;
			ansi_generic[163] = StandardCharCode.sterling;
			ansi_generic[164] = StandardCharCode.currency;
			ansi_generic[165] = StandardCharCode.yen;
			ansi_generic[166] = StandardCharCode.brokenbar;
			ansi_generic[167] = StandardCharCode.section;
			ansi_generic[168] = StandardCharCode.dieresis;
			ansi_generic[169] = StandardCharCode.copyright;
			ansi_generic[170] = StandardCharCode.ordfeminine;
			ansi_generic[171] = StandardCharCode.guillemotleft;
			ansi_generic[172] = StandardCharCode.logicalnot;
			ansi_generic[173] = StandardCharCode.opthyphen;
			ansi_generic[174] = StandardCharCode.registered;
			ansi_generic[175] = StandardCharCode.macron;
			ansi_generic[176] = StandardCharCode.degree;
			ansi_generic[177] = StandardCharCode.plusminus;
			ansi_generic[178] = StandardCharCode.twosuperior;
			ansi_generic[179] = StandardCharCode.threesuperior;
			ansi_generic[180] = StandardCharCode.acute;
			ansi_generic[181] = StandardCharCode.mu;
			ansi_generic[182] = StandardCharCode.paragraph;
			ansi_generic[183] = StandardCharCode.periodcentered;
			ansi_generic[184] = StandardCharCode.cedilla;
			ansi_generic[185] = StandardCharCode.onesuperior;
			ansi_generic[186] = StandardCharCode.ordmasculine;
			ansi_generic[187] = StandardCharCode.guillemotright;
			ansi_generic[188] = StandardCharCode.onequarter;
			ansi_generic[189] = StandardCharCode.onehalf;
			ansi_generic[190] = StandardCharCode.threequarters;
			ansi_generic[191] = StandardCharCode.questiondown;
			ansi_generic[192] = StandardCharCode.Agrave;
			ansi_generic[193] = StandardCharCode.Aacute;
			ansi_generic[194] = StandardCharCode.Acircumflex;
			ansi_generic[195] = StandardCharCode.Atilde;
			ansi_generic[196] = StandardCharCode.Adieresis;
			ansi_generic[197] = StandardCharCode.Aring;
			ansi_generic[198] = StandardCharCode.AE;
			ansi_generic[199] = StandardCharCode.Ccedilla;
			ansi_generic[200] = StandardCharCode.Egrave;
			ansi_generic[201] = StandardCharCode.Eacute;
			ansi_generic[202] = StandardCharCode.Ecircumflex;
			ansi_generic[203] = StandardCharCode.Edieresis;
			ansi_generic[204] = StandardCharCode.Igrave;
			ansi_generic[205] = StandardCharCode.Iacute;
			ansi_generic[206] = StandardCharCode.Icircumflex;
			ansi_generic[207] = StandardCharCode.Idieresis;
			ansi_generic[208] = StandardCharCode.Eth;
			ansi_generic[209] = StandardCharCode.Ntilde;
			ansi_generic[210] = StandardCharCode.Ograve;
			ansi_generic[211] = StandardCharCode.Oacute;
			ansi_generic[212] = StandardCharCode.Ocircumflex;
			ansi_generic[213] = StandardCharCode.Otilde;
			ansi_generic[214] = StandardCharCode.Odieresis;
			ansi_generic[215] = StandardCharCode.multiply;
			ansi_generic[216] = StandardCharCode.Oslash;
			ansi_generic[217] = StandardCharCode.Ugrave;
			ansi_generic[218] = StandardCharCode.Uacute;
			ansi_generic[219] = StandardCharCode.Ucircumflex;
			ansi_generic[220] = StandardCharCode.Udieresis;
			ansi_generic[221] = StandardCharCode.Yacute;
			ansi_generic[222] = StandardCharCode.Thorn;
			ansi_generic[223] = StandardCharCode.germandbls;
			ansi_generic[224] = StandardCharCode.agrave;
			ansi_generic[225] = StandardCharCode.aacute;
			ansi_generic[226] = StandardCharCode.acircumflex;
			ansi_generic[227] = StandardCharCode.atilde;
			ansi_generic[228] = StandardCharCode.adieresis;
			ansi_generic[229] = StandardCharCode.aring;
			ansi_generic[230] = StandardCharCode.ae;
			ansi_generic[231] = StandardCharCode.ccedilla;
			ansi_generic[232] = StandardCharCode.egrave;
			ansi_generic[233] = StandardCharCode.eacute;
			ansi_generic[234] = StandardCharCode.ecircumflex;
			ansi_generic[235] = StandardCharCode.edieresis;
			ansi_generic[236] = StandardCharCode.igrave;
			ansi_generic[237] = StandardCharCode.iacute;
			ansi_generic[238] = StandardCharCode.icircumflex;
			ansi_generic[239] = StandardCharCode.idieresis;
			ansi_generic[240] = StandardCharCode.eth;
			ansi_generic[241] = StandardCharCode.ntilde;
			ansi_generic[242] = StandardCharCode.ograve;
			ansi_generic[243] = StandardCharCode.oacute;
			ansi_generic[244] = StandardCharCode.ocircumflex;
			ansi_generic[245] = StandardCharCode.otilde;
			ansi_generic[246] = StandardCharCode.odieresis;
			ansi_generic[247] = StandardCharCode.divide;
			ansi_generic[248] = StandardCharCode.oslash;
			ansi_generic[249] = StandardCharCode.ugrave;
			ansi_generic[250] = StandardCharCode.uacute;
			ansi_generic[251] = StandardCharCode.ucircumflex;
			ansi_generic[252] = StandardCharCode.udieresis;
			ansi_generic[253] = StandardCharCode.yacute;
			ansi_generic[254] = StandardCharCode.thorn;
			ansi_generic[255] = StandardCharCode.ydieresis;
			return ansi_generic;
		}
	}

	public static Charcode AnsiSymbol
	{
		get
		{
			Charcode charcode = new Charcode(256);
			charcode[6] = StandardCharCode.formula;
			charcode[30] = StandardCharCode.nobrkhyphen;
			charcode[31] = StandardCharCode.opthyphen;
			charcode[32] = StandardCharCode.space;
			charcode[33] = StandardCharCode.exclam;
			charcode[34] = StandardCharCode.universal;
			charcode[35] = StandardCharCode.mathnumbersign;
			charcode[36] = StandardCharCode.existential;
			charcode[37] = StandardCharCode.percent;
			charcode[38] = StandardCharCode.ampersand;
			charcode[92] = StandardCharCode.suchthat;
			charcode[40] = StandardCharCode.parenleft;
			charcode[41] = StandardCharCode.parenright;
			charcode[42] = StandardCharCode.mathasterisk;
			charcode[43] = StandardCharCode.mathplus;
			charcode[44] = StandardCharCode.comma;
			charcode[45] = StandardCharCode.mathminus;
			charcode[46] = StandardCharCode.period;
			charcode[47] = StandardCharCode.slash;
			charcode[48] = StandardCharCode.zero;
			charcode[49] = StandardCharCode.one;
			charcode[50] = StandardCharCode.two;
			charcode[51] = StandardCharCode.three;
			charcode[52] = StandardCharCode.four;
			charcode[53] = StandardCharCode.five;
			charcode[54] = StandardCharCode.six;
			charcode[55] = StandardCharCode.seven;
			charcode[56] = StandardCharCode.eight;
			charcode[57] = StandardCharCode.nine;
			charcode[58] = StandardCharCode.colon;
			charcode[59] = StandardCharCode.semicolon;
			charcode[60] = StandardCharCode.less;
			charcode[61] = StandardCharCode.mathequal;
			charcode[62] = StandardCharCode.greater;
			charcode[63] = StandardCharCode.question;
			charcode[64] = StandardCharCode.congruent;
			charcode[65] = StandardCharCode.Alpha;
			charcode[66] = StandardCharCode.Beta;
			charcode[67] = StandardCharCode.Chi;
			charcode[68] = StandardCharCode.Delta;
			charcode[69] = StandardCharCode.Epsilon;
			charcode[70] = StandardCharCode.Phi;
			charcode[71] = StandardCharCode.Gamma;
			charcode[72] = StandardCharCode.Eta;
			charcode[73] = StandardCharCode.Iota;
			charcode[75] = StandardCharCode.Kappa;
			charcode[76] = StandardCharCode.Lambda;
			charcode[77] = StandardCharCode.Mu;
			charcode[78] = StandardCharCode.Nu;
			charcode[79] = StandardCharCode.Omicron;
			charcode[80] = StandardCharCode.Pi;
			charcode[81] = StandardCharCode.Theta;
			charcode[82] = StandardCharCode.Rho;
			charcode[83] = StandardCharCode.Sigma;
			charcode[84] = StandardCharCode.Tau;
			charcode[85] = StandardCharCode.Upsilon;
			charcode[86] = StandardCharCode.varsigma;
			charcode[87] = StandardCharCode.Omega;
			charcode[88] = StandardCharCode.Xi;
			charcode[89] = StandardCharCode.Psi;
			charcode[90] = StandardCharCode.Zeta;
			charcode[91] = StandardCharCode.bracketleft;
			charcode[92] = StandardCharCode.backslash;
			charcode[93] = StandardCharCode.bracketright;
			charcode[94] = StandardCharCode.asciicircum;
			charcode[95] = StandardCharCode.underscore;
			charcode[96] = StandardCharCode.quoteleft;
			charcode[97] = StandardCharCode.alpha;
			charcode[98] = StandardCharCode.beta;
			charcode[99] = StandardCharCode.chi;
			charcode[100] = StandardCharCode.delta;
			charcode[101] = StandardCharCode.epsilon;
			charcode[102] = StandardCharCode.phi;
			charcode[103] = StandardCharCode.gamma;
			charcode[104] = StandardCharCode.eta;
			charcode[105] = StandardCharCode.iota;
			charcode[107] = StandardCharCode.kappa;
			charcode[108] = StandardCharCode.lambda;
			charcode[109] = StandardCharCode.mu;
			charcode[110] = StandardCharCode.nu;
			charcode[111] = StandardCharCode.omicron;
			charcode[112] = StandardCharCode.pi;
			charcode[113] = StandardCharCode.theta;
			charcode[114] = StandardCharCode.rho;
			charcode[115] = StandardCharCode.sigma;
			charcode[116] = StandardCharCode.tau;
			charcode[117] = StandardCharCode.upsilon;
			charcode[119] = StandardCharCode.omega;
			charcode[120] = StandardCharCode.xi;
			charcode[121] = StandardCharCode.psi;
			charcode[122] = StandardCharCode.zeta;
			charcode[123] = StandardCharCode.braceleft;
			charcode[124] = StandardCharCode.bar;
			charcode[125] = StandardCharCode.braceright;
			charcode[126] = StandardCharCode.mathtilde;
			return charcode;
		}
	}

	public Charcode()
		: this(256)
	{
	}

	private Charcode(int size)
	{
		this.size = size;
		codes = new StandardCharCode[size];
		reverse = new Hashtable(size);
	}
}

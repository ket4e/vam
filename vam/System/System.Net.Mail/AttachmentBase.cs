using System.Collections;
using System.IO;
using System.Net.Mime;

namespace System.Net.Mail;

public abstract class AttachmentBase : IDisposable
{
	private class MimeTypes
	{
		private static Hashtable mimeTypes;

		static MimeTypes()
		{
			mimeTypes = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			mimeTypes.Add("3dm", "x-world/x-3dmf");
			mimeTypes.Add("3dmf", "x-world/x-3dmf");
			mimeTypes.Add("aab", "application/x-authorware-bin");
			mimeTypes.Add("aam", "application/x-authorware-map");
			mimeTypes.Add("aas", "application/x-authorware-seg");
			mimeTypes.Add("abc", "text/vnd.abc");
			mimeTypes.Add("acgi", "text/html");
			mimeTypes.Add("afl", "video/animaflex");
			mimeTypes.Add("ai", "application/postscript");
			mimeTypes.Add("aif", "audio/aiff");
			mimeTypes.Add("aifc", "audio/aiff");
			mimeTypes.Add("aiff", "audio/aiff");
			mimeTypes.Add("aim", "application/x-aim");
			mimeTypes.Add("aip", "text/x-audiosoft-intra");
			mimeTypes.Add("ani", "application/x-navi-animation");
			mimeTypes.Add("aos", "application/x-nokia-9000-communicator-add-on-software");
			mimeTypes.Add("aps", "application/mime");
			mimeTypes.Add("art", "image/x-jg");
			mimeTypes.Add("asf", "video/x-ms-asf");
			mimeTypes.Add("asm", "text/x-asm");
			mimeTypes.Add("asp", "text/asp");
			mimeTypes.Add("asx", "application/x-mplayer2");
			mimeTypes.Add("au", "audio/x-au");
			mimeTypes.Add("avi", "video/avi");
			mimeTypes.Add("avs", "video/avs-video");
			mimeTypes.Add("bcpio", "application/x-bcpio");
			mimeTypes.Add("bm", "image/bmp");
			mimeTypes.Add("bmp", "image/bmp");
			mimeTypes.Add("boo", "application/book");
			mimeTypes.Add("book", "application/book");
			mimeTypes.Add("boz", "application/x-bzip2");
			mimeTypes.Add("bsh", "application/x-bsh");
			mimeTypes.Add("bz", "application/x-bzip");
			mimeTypes.Add("bz2", "application/x-bzip2");
			mimeTypes.Add("c", "text/plain");
			mimeTypes.Add("c++", "text/plain");
			mimeTypes.Add("cat", "application/vnd.ms-pki.seccat");
			mimeTypes.Add("cc", "text/plain");
			mimeTypes.Add("ccad", "application/clariscad");
			mimeTypes.Add("cco", "application/x-cocoa");
			mimeTypes.Add("cdf", "application/cdf");
			mimeTypes.Add("cer", "application/pkix-cert");
			mimeTypes.Add("cha", "application/x-chat");
			mimeTypes.Add("chat", "application/x-chat");
			mimeTypes.Add("class", "application/java");
			mimeTypes.Add("conf", "text/plain");
			mimeTypes.Add("cpio", "application/x-cpio");
			mimeTypes.Add("cpp", "text/plain");
			mimeTypes.Add("cpt", "application/x-cpt");
			mimeTypes.Add("crl", "application/pkix-crl");
			mimeTypes.Add("crt", "application/pkix-cert");
			mimeTypes.Add("csh", "application/x-csh");
			mimeTypes.Add("css", "text/css");
			mimeTypes.Add("cxx", "text/plain");
			mimeTypes.Add("dcr", "application/x-director");
			mimeTypes.Add("deepv", "application/x-deepv");
			mimeTypes.Add("def", "text/plain");
			mimeTypes.Add("der", "application/x-x509-ca-cert");
			mimeTypes.Add("dif", "video/x-dv");
			mimeTypes.Add("dir", "application/x-director");
			mimeTypes.Add("dl", "video/dl");
			mimeTypes.Add("doc", "application/msword");
			mimeTypes.Add("dot", "application/msword");
			mimeTypes.Add("dp", "application/commonground");
			mimeTypes.Add("drw", "application/drafting");
			mimeTypes.Add("dv", "video/x-dv");
			mimeTypes.Add("dvi", "application/x-dvi");
			mimeTypes.Add("dwf", "drawing/x-dwf (old)");
			mimeTypes.Add("dwg", "application/acad");
			mimeTypes.Add("dxf", "application/dxf");
			mimeTypes.Add("dxr", "application/x-director");
			mimeTypes.Add("el", "text/x-script.elisp");
			mimeTypes.Add("elc", "application/x-elc");
			mimeTypes.Add("eps", "application/postscript");
			mimeTypes.Add("es", "application/x-esrehber");
			mimeTypes.Add("etx", "text/x-setext");
			mimeTypes.Add("evy", "application/envoy");
			mimeTypes.Add("f", "text/plain");
			mimeTypes.Add("f77", "text/plain");
			mimeTypes.Add("f90", "text/plain");
			mimeTypes.Add("fdf", "application/vnd.fdf");
			mimeTypes.Add("fif", "image/fif");
			mimeTypes.Add("fli", "video/fli");
			mimeTypes.Add("flo", "image/florian");
			mimeTypes.Add("flx", "text/vnd.fmi.flexstor");
			mimeTypes.Add("fmf", "video/x-atomic3d-feature");
			mimeTypes.Add("for", "text/plain");
			mimeTypes.Add("fpx", "image/vnd.fpx");
			mimeTypes.Add("frl", "application/freeloader");
			mimeTypes.Add("funk", "audio/make");
			mimeTypes.Add("g", "text/plain");
			mimeTypes.Add("g3", "image/g3fax");
			mimeTypes.Add("gif", "image/gif");
			mimeTypes.Add("gl", "video/gl");
			mimeTypes.Add("gsd", "audio/x-gsm");
			mimeTypes.Add("gsm", "audio/x-gsm");
			mimeTypes.Add("gsp", "application/x-gsp");
			mimeTypes.Add("gss", "application/x-gss");
			mimeTypes.Add("gtar", "application/x-gtar");
			mimeTypes.Add("gz", "application/x-gzip");
			mimeTypes.Add("gzip", "application/x-gzip");
			mimeTypes.Add("h", "text/plain");
			mimeTypes.Add("hdf", "application/x-hdf");
			mimeTypes.Add("help", "application/x-helpfile");
			mimeTypes.Add("hgl", "application/vnd.hp-HPGL");
			mimeTypes.Add("hh", "text/plain");
			mimeTypes.Add("hlb", "text/x-script");
			mimeTypes.Add("hlp", "application/x-helpfile");
			mimeTypes.Add("hpg", "application/vnd.hp-HPGL");
			mimeTypes.Add("hpgl", "application/vnd.hp-HPGL");
			mimeTypes.Add("hqx", "application/binhex");
			mimeTypes.Add("hta", "application/hta");
			mimeTypes.Add("htc", "text/x-component");
			mimeTypes.Add("htm", "text/html");
			mimeTypes.Add("html", "text/html");
			mimeTypes.Add("htmls", "text/html");
			mimeTypes.Add("htt", "text/webviewhtml");
			mimeTypes.Add("htx", "text/html");
			mimeTypes.Add("ice", "x-conference/x-cooltalk");
			mimeTypes.Add("ico", "image/x-icon");
			mimeTypes.Add("idc", "text/plain");
			mimeTypes.Add("ief", "image/ief");
			mimeTypes.Add("iefs", "image/ief");
			mimeTypes.Add("iges", "application/iges");
			mimeTypes.Add("igs", "application/iges");
			mimeTypes.Add("ima", "application/x-ima");
			mimeTypes.Add("imap", "application/x-httpd-imap");
			mimeTypes.Add("inf", "application/inf");
			mimeTypes.Add("ins", "application/x-internett-signup");
			mimeTypes.Add("ip", "application/x-ip2");
			mimeTypes.Add("isu", "video/x-isvideo");
			mimeTypes.Add("it", "audio/it");
			mimeTypes.Add("iv", "application/x-inventor");
			mimeTypes.Add("ivr", "i-world/i-vrml");
			mimeTypes.Add("ivy", "application/x-livescreen");
			mimeTypes.Add("jam", "audio/x-jam");
			mimeTypes.Add("jav", "text/plain");
			mimeTypes.Add("java", "text/plain");
			mimeTypes.Add("jcm", "application/x-java-commerce");
			mimeTypes.Add("jfif", "image/jpeg");
			mimeTypes.Add("jfif-tbnl", "image/jpeg");
			mimeTypes.Add("jpe", "image/jpeg");
			mimeTypes.Add("jpeg", "image/jpeg");
			mimeTypes.Add("jpg", "image/jpeg");
			mimeTypes.Add("jps", "image/x-jps");
			mimeTypes.Add("js", "application/x-javascript");
			mimeTypes.Add("jut", "image/jutvision");
			mimeTypes.Add("kar", "audio/midi");
			mimeTypes.Add("ksh", "text/x-script.ksh");
			mimeTypes.Add("la", "audio/nspaudio");
			mimeTypes.Add("lam", "audio/x-liveaudio");
			mimeTypes.Add("latex", "application/x-latex");
			mimeTypes.Add("list", "text/plain");
			mimeTypes.Add("lma", "audio/nspaudio");
			mimeTypes.Add("log", "text/plain");
			mimeTypes.Add("lsp", "application/x-lisp");
			mimeTypes.Add("lst", "text/plain");
			mimeTypes.Add("lsx", "text/x-la-asf");
			mimeTypes.Add("ltx", "application/x-latex");
			mimeTypes.Add("m", "text/plain");
			mimeTypes.Add("m1v", "video/mpeg");
			mimeTypes.Add("m2a", "audio/mpeg");
			mimeTypes.Add("m2v", "video/mpeg");
			mimeTypes.Add("m3u", "audio/x-mpequrl");
			mimeTypes.Add("man", "application/x-troff-man");
			mimeTypes.Add("map", "application/x-navimap");
			mimeTypes.Add("mar", "text/plain");
			mimeTypes.Add("mbd", "application/mbedlet");
			mimeTypes.Add("mc$", "application/x-magic-cap-package-1.0");
			mimeTypes.Add("mcd", "application/mcad");
			mimeTypes.Add("mcf", "image/vasa");
			mimeTypes.Add("mcp", "application/netmc");
			mimeTypes.Add("me", "application/x-troff-me");
			mimeTypes.Add("mht", "message/rfc822");
			mimeTypes.Add("mhtml", "message/rfc822");
			mimeTypes.Add("mid", "audio/midi");
			mimeTypes.Add("midi", "audio/midi");
			mimeTypes.Add("mif", "application/x-mif");
			mimeTypes.Add("mime", "message/rfc822");
			mimeTypes.Add("mjf", "audio/x-vnd.AudioExplosion.MjuiceMediaFile");
			mimeTypes.Add("mjpg", "video/x-motion-jpeg");
			mimeTypes.Add("mm", "application/base64");
			mimeTypes.Add("mme", "application/base64");
			mimeTypes.Add("mod", "audio/mod");
			mimeTypes.Add("moov", "video/quicktime");
			mimeTypes.Add("mov", "video/quicktime");
			mimeTypes.Add("movie", "video/x-sgi-movie");
			mimeTypes.Add("mp2", "video/mpeg");
			mimeTypes.Add("mp3", "audio/mpeg3");
			mimeTypes.Add("mpa", "audio/mpeg");
			mimeTypes.Add("mpc", "application/x-project");
			mimeTypes.Add("mpe", "video/mpeg");
			mimeTypes.Add("mpeg", "video/mpeg");
			mimeTypes.Add("mpg", "video/mpeg");
			mimeTypes.Add("mpga", "audio/mpeg");
			mimeTypes.Add("mpp", "application/vnd.ms-project");
			mimeTypes.Add("mpt", "application/x-project");
			mimeTypes.Add("mpv", "application/x-project");
			mimeTypes.Add("mpx", "application/x-project");
			mimeTypes.Add("mrc", "application/marc");
			mimeTypes.Add("ms", "application/x-troff-ms");
			mimeTypes.Add("mv", "video/x-sgi-movie");
			mimeTypes.Add("my", "audio/make");
			mimeTypes.Add("mzz", "application/x-vnd.AudioExplosion.mzz");
			mimeTypes.Add("nap", "image/naplps");
			mimeTypes.Add("naplps", "image/naplps");
			mimeTypes.Add("nc", "application/x-netcdf");
			mimeTypes.Add("ncm", "application/vnd.nokia.configuration-message");
			mimeTypes.Add("nif", "image/x-niff");
			mimeTypes.Add("niff", "image/x-niff");
			mimeTypes.Add("nix", "application/x-mix-transfer");
			mimeTypes.Add("nsc", "application/x-conference");
			mimeTypes.Add("nvd", "application/x-navidoc");
			mimeTypes.Add("oda", "application/oda");
			mimeTypes.Add("omc", "application/x-omc");
			mimeTypes.Add("omcd", "application/x-omcdatamaker");
			mimeTypes.Add("omcr", "application/x-omcregerator");
			mimeTypes.Add("p", "text/x-pascal");
			mimeTypes.Add("p10", "application/pkcs10");
			mimeTypes.Add("p12", "application/pkcs-12");
			mimeTypes.Add("p7a", "application/x-pkcs7-signature");
			mimeTypes.Add("p7c", "application/pkcs7-mime");
			mimeTypes.Add("p7m", "application/pkcs7-mime");
			mimeTypes.Add("p7r", "application/x-pkcs7-certreqresp");
			mimeTypes.Add("p7s", "application/pkcs7-signature");
			mimeTypes.Add("part", "application/pro_eng");
			mimeTypes.Add("pas", "text/pascal");
			mimeTypes.Add("pbm", "image/x-portable-bitmap");
			mimeTypes.Add("pcl", "application/x-pcl");
			mimeTypes.Add("pct", "image/x-pict");
			mimeTypes.Add("pcx", "image/x-pcx");
			mimeTypes.Add("pdb", "chemical/x-pdb");
			mimeTypes.Add("pdf", "application/pdf");
			mimeTypes.Add("pfunk", "audio/make");
			mimeTypes.Add("pgm", "image/x-portable-graymap");
			mimeTypes.Add("pic", "image/pict");
			mimeTypes.Add("pict", "image/pict");
			mimeTypes.Add("pkg", "application/x-newton-compatible-pkg");
			mimeTypes.Add("pko", "application/vnd.ms-pki.pko");
			mimeTypes.Add("pl", "text/plain");
			mimeTypes.Add("plx", "application/x-PiXCLscript");
			mimeTypes.Add("pm", "image/x-xpixmap");
			mimeTypes.Add("pm4", "application/x-pagemaker");
			mimeTypes.Add("pm5", "application/x-pagemaker");
			mimeTypes.Add("png", "image/png");
			mimeTypes.Add("pnm", "application/x-portable-anymap");
			mimeTypes.Add("pot", "application/mspowerpoint");
			mimeTypes.Add("pov", "model/x-pov");
			mimeTypes.Add("ppa", "application/vnd.ms-powerpoint");
			mimeTypes.Add("ppm", "image/x-portable-pixmap");
			mimeTypes.Add("pps", "application/mspowerpoint");
			mimeTypes.Add("ppt", "application/mspowerpoint");
			mimeTypes.Add("ppz", "application/mspowerpoint");
			mimeTypes.Add("pre", "application/x-freelance");
			mimeTypes.Add("prt", "application/pro_eng");
			mimeTypes.Add("ps", "application/postscript");
			mimeTypes.Add("pvu", "paleovu/x-pv");
			mimeTypes.Add("pwz", "application/vnd.ms-powerpoint");
			mimeTypes.Add("py", "text/x-script.phyton");
			mimeTypes.Add("pyc", "applicaiton/x-bytecode.python");
			mimeTypes.Add("qcp", "audio/vnd.qcelp");
			mimeTypes.Add("qd3", "x-world/x-3dmf");
			mimeTypes.Add("qd3d", "x-world/x-3dmf");
			mimeTypes.Add("qif", "image/x-quicktime");
			mimeTypes.Add("qt", "video/quicktime");
			mimeTypes.Add("qtc", "video/x-qtc");
			mimeTypes.Add("qti", "image/x-quicktime");
			mimeTypes.Add("qtif", "image/x-quicktime");
			mimeTypes.Add("ra", "audio/x-pn-realaudio");
			mimeTypes.Add("ram", "audio/x-pn-realaudio");
			mimeTypes.Add("ras", "application/x-cmu-raster");
			mimeTypes.Add("rast", "image/cmu-raster");
			mimeTypes.Add("rexx", "text/x-script.rexx");
			mimeTypes.Add("rf", "image/vnd.rn-realflash");
			mimeTypes.Add("rgb", "image/x-rgb");
			mimeTypes.Add("rm", "application/vnd.rn-realmedia");
			mimeTypes.Add("rmi", "audio/mid");
			mimeTypes.Add("rmm", "audio/x-pn-realaudio");
			mimeTypes.Add("rmp", "audio/x-pn-realaudio");
			mimeTypes.Add("rng", "application/ringing-tones");
			mimeTypes.Add("rnx", "application/vnd.rn-realplayer");
			mimeTypes.Add("roff", "application/x-troff");
			mimeTypes.Add("rp", "image/vnd.rn-realpix");
			mimeTypes.Add("rpm", "audio/x-pn-realaudio-plugin");
			mimeTypes.Add("rss", "text/xml");
			mimeTypes.Add("rt", "text/richtext");
			mimeTypes.Add("rtf", "text/richtext");
			mimeTypes.Add("rtx", "text/richtext");
			mimeTypes.Add("rv", "video/vnd.rn-realvideo");
			mimeTypes.Add("s", "text/x-asm");
			mimeTypes.Add("s3m", "audio/s3m");
			mimeTypes.Add("sbk", "application/x-tbook");
			mimeTypes.Add("scm", "application/x-lotusscreencam");
			mimeTypes.Add("sdml", "text/plain");
			mimeTypes.Add("sdp", "application/sdp");
			mimeTypes.Add("sdr", "application/sounder");
			mimeTypes.Add("sea", "application/sea");
			mimeTypes.Add("set", "application/set");
			mimeTypes.Add("sgm", "text/sgml");
			mimeTypes.Add("sgml", "text/sgml");
			mimeTypes.Add("sh", "text/x-script.sh");
			mimeTypes.Add("shar", "application/x-bsh");
			mimeTypes.Add("shtml", "text/html");
			mimeTypes.Add("sid", "audio/x-psid");
			mimeTypes.Add("sit", "application/x-sit");
			mimeTypes.Add("skd", "application/x-koan");
			mimeTypes.Add("skm", "application/x-koan");
			mimeTypes.Add("skp", "application/x-koan");
			mimeTypes.Add("skt", "application/x-koan");
			mimeTypes.Add("sl", "application/x-seelogo");
			mimeTypes.Add("smi", "application/smil");
			mimeTypes.Add("smil", "application/smil");
			mimeTypes.Add("snd", "audio/basic");
			mimeTypes.Add("sol", "application/solids");
			mimeTypes.Add("spc", "application/x-pkcs7-certificates");
			mimeTypes.Add("spl", "application/futuresplash");
			mimeTypes.Add("spr", "application/x-sprite");
			mimeTypes.Add("sprite", "application/x-sprite");
			mimeTypes.Add("src", "application/x-wais-source");
			mimeTypes.Add("ssi", "text/x-server-parsed-html");
			mimeTypes.Add("ssm", "application/streamingmedia");
			mimeTypes.Add("sst", "application/vnd.ms-pki.certstore");
			mimeTypes.Add("step", "application/step");
			mimeTypes.Add("stl", "application/sla");
			mimeTypes.Add("stp", "application/step");
			mimeTypes.Add("sv4cpio", "application/x-sv4cpio");
			mimeTypes.Add("sv4crc", "application/x-sv4crc");
			mimeTypes.Add("svf", "image/x-dwg");
			mimeTypes.Add("svr", "application/x-world");
			mimeTypes.Add("swf", "application/x-shockwave-flash");
			mimeTypes.Add("t", "application/x-troff");
			mimeTypes.Add("talk", "text/x-speech");
			mimeTypes.Add("tar", "application/x-tar");
			mimeTypes.Add("tbk", "application/toolbook");
			mimeTypes.Add("tcl", "text/x-script.tcl");
			mimeTypes.Add("tcsh", "text/x-script.tcsh");
			mimeTypes.Add("tex", "application/x-tex");
			mimeTypes.Add("texi", "application/x-texinfo");
			mimeTypes.Add("texinfo", "application/x-texinfo");
			mimeTypes.Add("text", "text/plain");
			mimeTypes.Add("tgz", "application/x-compressed");
			mimeTypes.Add("tif", "image/tiff");
			mimeTypes.Add("tiff", "image/tiff");
			mimeTypes.Add("tr", "application/x-troff");
			mimeTypes.Add("tsi", "audio/tsp-audio");
			mimeTypes.Add("tsp", "audio/tsplayer");
			mimeTypes.Add("tsv", "text/tab-separated-values");
			mimeTypes.Add("turbot", "image/florian");
			mimeTypes.Add("txt", "text/plain");
			mimeTypes.Add("uil", "text/x-uil");
			mimeTypes.Add("uni", "text/uri-list");
			mimeTypes.Add("unis", "text/uri-list");
			mimeTypes.Add("unv", "application/i-deas");
			mimeTypes.Add("uri", "text/uri-list");
			mimeTypes.Add("uris", "text/uri-list");
			mimeTypes.Add("ustar", "multipart/x-ustar");
			mimeTypes.Add("uu", "text/x-uuencode");
			mimeTypes.Add("uue", "text/x-uuencode");
			mimeTypes.Add("vcd", "application/x-cdlink");
			mimeTypes.Add("vcs", "text/x-vCalendar");
			mimeTypes.Add("vda", "application/vda");
			mimeTypes.Add("vdo", "video/vdo");
			mimeTypes.Add("vew", "application/groupwise");
			mimeTypes.Add("viv", "video/vivo");
			mimeTypes.Add("vivo", "video/vivo");
			mimeTypes.Add("vmd", "application/vocaltec-media-desc");
			mimeTypes.Add("vmf", "application/vocaltec-media-file");
			mimeTypes.Add("voc", "audio/voc");
			mimeTypes.Add("vos", "video/vosaic");
			mimeTypes.Add("vox", "audio/voxware");
			mimeTypes.Add("vqe", "audio/x-twinvq-plugin");
			mimeTypes.Add("vqf", "audio/x-twinvq");
			mimeTypes.Add("vql", "audio/x-twinvq-plugin");
			mimeTypes.Add("vrml", "application/x-vrml");
			mimeTypes.Add("vrt", "x-world/x-vrt");
			mimeTypes.Add("vsd", "application/x-visio");
			mimeTypes.Add("vst", "application/x-visio");
			mimeTypes.Add("vsw", "application/x-visio");
			mimeTypes.Add("w60", "application/wordperfect6.0");
			mimeTypes.Add("w61", "application/wordperfect6.1");
			mimeTypes.Add("w6w", "application/msword");
			mimeTypes.Add("wav", "audio/wav");
			mimeTypes.Add("wb1", "application/x-qpro");
			mimeTypes.Add("wbmp", "image/vnd.wap.wbmp");
			mimeTypes.Add("web", "application/vnd.xara");
			mimeTypes.Add("wiz", "application/msword");
			mimeTypes.Add("wk1", "application/x-123");
			mimeTypes.Add("wmf", "windows/metafile");
			mimeTypes.Add("wml", "text/vnd.wap.wml");
			mimeTypes.Add("wmlc", "application/vnd.wap.wmlc");
			mimeTypes.Add("wmls", "text/vnd.wap.wmlscript");
			mimeTypes.Add("wmlsc", "application/vnd.wap.wmlscriptc");
			mimeTypes.Add("word", "application/msword");
			mimeTypes.Add("wp", "application/wordperfect");
			mimeTypes.Add("wp5", "application/wordperfect");
			mimeTypes.Add("wp6", "application/wordperfect");
			mimeTypes.Add("wpd", "application/wordperfect");
			mimeTypes.Add("wq1", "application/x-lotus");
			mimeTypes.Add("wri", "application/mswrite");
			mimeTypes.Add("wrl", "application/x-world");
			mimeTypes.Add("wrz", "model/vrml");
			mimeTypes.Add("wsc", "text/scriplet");
			mimeTypes.Add("wsrc", "application/x-wais-source");
			mimeTypes.Add("wtk", "application/x-wintalk");
			mimeTypes.Add("xbm", "image/x-xbitmap");
			mimeTypes.Add("xdr", "video/x-amt-demorun");
			mimeTypes.Add("xgz", "xgl/drawing");
			mimeTypes.Add("xif", "image/vnd.xiff");
			mimeTypes.Add("xl", "application/excel");
			mimeTypes.Add("xla", "application/excel");
			mimeTypes.Add("xlb", "application/excel");
			mimeTypes.Add("xlc", "application/excel");
			mimeTypes.Add("xld", "application/excel");
			mimeTypes.Add("xlk", "application/excel");
			mimeTypes.Add("xll", "application/excel");
			mimeTypes.Add("xlm", "application/excel");
			mimeTypes.Add("xls", "application/excel");
			mimeTypes.Add("xlt", "application/excel");
			mimeTypes.Add("xlv", "application/excel");
			mimeTypes.Add("xlw", "application/excel");
			mimeTypes.Add("xm", "audio/xm");
			mimeTypes.Add("xml", "text/xml");
			mimeTypes.Add("xmz", "xgl/movie");
			mimeTypes.Add("xpix", "application/x-vnd.ls-xpix");
			mimeTypes.Add("xpm", "image/xpm");
			mimeTypes.Add("x-png", "image/png");
			mimeTypes.Add("xsr", "video/x-amt-showrun");
			mimeTypes.Add("xwd", "image/x-xwd");
			mimeTypes.Add("xyz", "chemical/x-pdb");
			mimeTypes.Add("z", "application/x-compressed");
			mimeTypes.Add("zip", "application/zip");
			mimeTypes.Add("zsh", "text/x-script.zsh");
		}

		public static string GetMimeType(string fileName)
		{
			string text = null;
			int num = fileName.LastIndexOf('.');
			if (num != -1 && fileName.Length > num + 1)
			{
				text = mimeTypes[fileName.Substring(num + 1)] as string;
			}
			if (text == null)
			{
				text = "application/octet-stream";
			}
			return text;
		}
	}

	private string id;

	private ContentType contentType = new ContentType();

	private Stream contentStream;

	private TransferEncoding transferEncoding = TransferEncoding.Base64;

	public string ContentId
	{
		get
		{
			return id;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			id = value;
		}
	}

	public Stream ContentStream => contentStream;

	public ContentType ContentType
	{
		get
		{
			return contentType;
		}
		set
		{
			contentType = value;
		}
	}

	public TransferEncoding TransferEncoding
	{
		get
		{
			return transferEncoding;
		}
		set
		{
			transferEncoding = value;
		}
	}

	protected AttachmentBase(Stream contentStream)
	{
		if (contentStream == null)
		{
			throw new ArgumentNullException();
		}
		this.contentStream = contentStream;
		contentType.MediaType = "application/octet-stream";
		contentType.CharSet = null;
	}

	protected AttachmentBase(Stream contentStream, ContentType contentType)
	{
		if (contentStream == null || contentType == null)
		{
			throw new ArgumentNullException();
		}
		this.contentStream = contentStream;
		this.contentType = contentType;
	}

	protected AttachmentBase(Stream contentStream, string mediaType)
	{
		if (contentStream == null)
		{
			throw new ArgumentNullException();
		}
		this.contentStream = contentStream;
		contentType.MediaType = mediaType;
	}

	protected AttachmentBase(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException();
		}
		contentStream = File.OpenRead(fileName);
		contentType = new ContentType(MimeTypes.GetMimeType(fileName));
	}

	protected AttachmentBase(string fileName, ContentType contentType)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException();
		}
		contentStream = File.OpenRead(fileName);
		this.contentType = contentType;
	}

	protected AttachmentBase(string fileName, string mediaType)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException();
		}
		contentStream = File.OpenRead(fileName);
		contentType.MediaType = mediaType;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			contentStream.Close();
		}
	}
}

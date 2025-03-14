using System;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl;

internal class MSXslScriptManager
{
	private enum ScriptingLanguage
	{
		JScript,
		VisualBasic,
		CSharp
	}

	private class MSXslScript
	{
		private ScriptingLanguage language;

		private string implementsPrefix;

		private string code;

		private Evidence evidence;

		public ScriptingLanguage Language => language;

		public string ImplementsPrefix => implementsPrefix;

		public string Code => code;

		public MSXslScript(XPathNavigator nav, Evidence evidence)
		{
			this.evidence = evidence;
			code = nav.Value;
			if (nav.MoveToFirstAttribute())
			{
				do
				{
					switch (nav.LocalName)
					{
					case "language":
						switch (nav.Value.ToLower(CultureInfo.InvariantCulture))
						{
						case "jscript":
						case "javascript":
							language = ScriptingLanguage.JScript;
							break;
						case "vb":
						case "visualbasic":
							language = ScriptingLanguage.VisualBasic;
							break;
						case "c#":
						case "csharp":
							language = ScriptingLanguage.CSharp;
							break;
						default:
							throw new XsltException("Invalid scripting language!", null);
						}
						break;
					case "implements-prefix":
						implementsPrefix = nav.Value;
						break;
					}
				}
				while (nav.MoveToNextAttribute());
				nav.MoveToParent();
			}
			if (implementsPrefix == null)
			{
				throw new XsltException("need implements-prefix attr", null);
			}
		}

		public object Compile(XPathNavigator node)
		{
			string text = string.Empty;
			byte[] array = MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(code));
			foreach (byte b in array)
			{
				text += b.ToString("x2");
			}
			return language switch
			{
				ScriptingLanguage.CSharp => new CSharpCompilerInfo().GetScriptClass(Code, text, node, evidence), 
				ScriptingLanguage.JScript => new JScriptCompilerInfo().GetScriptClass(Code, text, node, evidence), 
				ScriptingLanguage.VisualBasic => new VBCompilerInfo().GetScriptClass(Code, text, node, evidence), 
				_ => null, 
			};
		}
	}

	private Hashtable scripts = new Hashtable();

	public void AddScript(Compiler c)
	{
		MSXslScript mSXslScript = new MSXslScript(c.Input, c.Evidence);
		string @namespace = c.Input.GetNamespace(mSXslScript.ImplementsPrefix);
		if (@namespace == null)
		{
			throw new XsltCompileException("Specified prefix for msxsl:script was not found: " + mSXslScript.ImplementsPrefix, null, c.Input);
		}
		scripts.Add(@namespace, mSXslScript.Compile(c.Input));
	}

	public object GetExtensionObject(string ns)
	{
		if (!scripts.ContainsKey(ns))
		{
			return null;
		}
		return Activator.CreateInstance((Type)scripts[ns]);
	}
}
